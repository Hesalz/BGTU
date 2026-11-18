import hashlib
import struct

from cryptography.fernet import Fernet, InvalidToken
from cryptography.hazmat.primitives import hashes
from cryptography.hazmat.primitives.kdf.pbkdf2 import PBKDF2HMAC


MAGIC = b"DCTS" # Константы формата скрытых данных.
VERSION = 3
SALT_SIZE = 16


HEADER_FORMAT = ">4sBI16s32s" # Формат заголовка: идентификатор, версия, длина данных, соль и хеш сообщения.
HEADER_SIZE = struct.calcsize(HEADER_FORMAT)


def encrypted_size_for_message_bytes(message_bytes: int) -> int:
    if message_bytes < 0: # Формат заголовка: идентификатор, версия, длина данных, соль и хеш сообщения.
        raise ValueError("Размер сообщения не может быть отрицательным.")


    ciphertext_size = ((message_bytes // 16) + 1) * 16 # Расчёт размера шифрованных данных с учётом блочного шифрования Fernet и кодирования Base64.
    raw_size = 57 + ciphertext_size
    return 4 * ((raw_size + 2) // 3)


def payload_size_for_message_bytes(message_bytes: int) -> int:  # Общий размер данных с учётом заголовка и полезной нагрузки.
    return HEADER_SIZE + encrypted_size_for_message_bytes(message_bytes)


def max_message_bytes_for_payload_capacity(capacity_bytes: int) -> int: # Если вместимости недостаточно даже для заголовка, сообщение не поместится.
    if capacity_bytes < HEADER_SIZE:
        return 0

    low, high = 0, max(0, capacity_bytes - HEADER_SIZE) # Поиск максимального размера сообщения, помещающегося в заданную вместимость.
    best = 0

    while low <= high: # Бинарный поиск позволяет определить максимальный допустимый размер сообщения.
        middle = (low + high) // 2
        if payload_size_for_message_bytes(middle) <= capacity_bytes:
            best = middle
            low = middle + 1
        else:
            high = middle - 1

    return best


def _derive_key(password: str, salt: bytes) -> bytes:
    if not password: # Проверка наличия пароля для формирования ключа шифрования.
        raise ValueError("Пароль не должен быть пустым.")

    kdf = PBKDF2HMAC( # Получение криптографического ключа на основе пароля и случайной соли.
        algorithm=hashes.SHA256(),
        length=32,
        salt=salt,
        iterations=300_000,
    )
    return __import__("base64").urlsafe_b64encode( # Преобразование ключа в формат Base64, необходимый для Fernet.
        kdf.derive(password.encode("utf-8"))
    )


def build_payload(message: str, password: str | None = None) -> bytes:
    message_bytes = message.encode("utf-8") # Преобразование текстового сообщения в байты UTF-8.
    integrity = hashlib.sha256(message_bytes).digest() # Формирование SHA-256 хеша для последующей проверки целостности данных.

    if password: # При наличии пароля выполняется шифрование сообщения.
        salt = __import__("secrets").token_bytes(SALT_SIZE)  # Генерация случайной соли и получение ключа шифрования.
        key = _derive_key(password, salt)
        stored_data = Fernet(key).encrypt(message_bytes)
    else: # При отсутствии пароля сообщение сохраняется без шифрования.
        salt = bytes(SALT_SIZE)
        stored_data = message_bytes

    header = struct.pack( # Формирование заголовка с параметрами скрытого сообщения.
        HEADER_FORMAT,
        MAGIC,
        VERSION,
        len(stored_data),
        salt,
        integrity,
    )
    return header + stored_data # Объединение заголовка и данных в единую полезную нагрузку.


def parse_payload(payload: bytes, password: str | None = None) -> str:
    if len(payload) < HEADER_SIZE: # Проверка наличия полного заголовка.
        raise ValueError("Скрытые данные неполные.")

    magic, version, data_length, salt, integrity = struct.unpack( # Чтение параметров скрытого сообщения из заголовка.
        HEADER_FORMAT, payload[:HEADER_SIZE]
    )
    if magic != MAGIC: # Проверка идентификатора и версии формата данных.
        raise ValueError("Совместимое DCT-сообщение не найдено.")
    if version not in (2, 3):
        raise ValueError("Неподдерживаемая версия данных.")

    start = HEADER_SIZE # Определение границ полезной нагрузки и проверка её длины.
    end = start + data_length
    if data_length <= 0 or end > len(payload):
        raise ValueError("Скрытое сообщение неполное.")

    stored_data = payload[start:end]
    try:
        # Для зашифрованных сообщений выполняется получение ключа и расшифровка.
        # Версия 2 всегда требует пароль; версия 3 использует соль
        # для определения наличия шифрования.
        if version == 2 or salt != bytes(SALT_SIZE):
            if not password:
                raise ValueError("Для извлечения этого сообщения необходимо ввести пароль.")
            key = _derive_key(password, salt)
            message_bytes = Fernet(key).decrypt(stored_data)
        else:
            message_bytes = stored_data # В незашифрованном сообщении данные используются напрямую.
    except (InvalidToken, ValueError) as exc:
        if isinstance(exc, ValueError) and str(exc).startswith("Для извлечения"): # Обработка неверного пароля или повреждённых зашифрованных данных.
            raise
        raise ValueError("Неверный пароль или скрытые данные повреждены.") from exc

    if hashlib.sha256(message_bytes).digest() != integrity: # Проверка целостности восстановленного сообщения по SHA-256.
        raise ValueError("Проверка целостности скрытых данных не пройдена.")
    try:
        return message_bytes.decode("utf-8") # Преобразование восстановленных байтов в текст UTF-8.
    except UnicodeDecodeError as exc:
        raise ValueError("Скрытое сообщение не является корректным текстом UTF-8.") from exc


def bytes_to_bits(data: bytes) -> list[int]: # Преобразование каждого байта в последовательность из 8 бит в порядке от старшего бита к младшему.
    return [
        (byte >> shift) & 1
        for byte in data
        for shift in range(7, -1, -1)
    ]


def bits_to_bytes(bits: list[int]) -> bytes:
    if len(bits) % 8 != 0: # Проверка, что количество бит позволяет сформировать целые байты.
        raise ValueError("Длина последовательности бит должна быть кратна 8.")

    result = bytearray()

    for index in range(0, len(bits), 8):  # Объединение последовательностей из 8 бит в отдельные байты.
        value = 0
        for bit in bits[index:index + 8]:
            value = (value << 1) | bit
        result.append(value)

    return bytes(result) # Преобразование буфера байтов в неизменяемый тип bytes.
