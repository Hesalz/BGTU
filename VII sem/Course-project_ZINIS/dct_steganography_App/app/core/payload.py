import hashlib
import struct

from cryptography.fernet import Fernet, InvalidToken
from cryptography.hazmat.primitives import hashes
from cryptography.hazmat.primitives.kdf.pbkdf2 import PBKDF2HMAC


MAGIC = b"DCTS"
VERSION = 3
SALT_SIZE = 16

# magic + version + message length + salt + integrity hash
HEADER_FORMAT = ">4sBI16s32s"
HEADER_SIZE = struct.calcsize(HEADER_FORMAT)


def encrypted_size_for_message_bytes(message_bytes: int) -> int:
    """Return the Fernet token size for a plaintext of the given byte length."""
    if message_bytes < 0:
        raise ValueError("Размер сообщения не может быть отрицательным.")

    # Fernet uses AES-CBC with PKCS7 padding. The token contains:
    # version(1) + timestamp(8) + IV(16) + ciphertext + HMAC(32).
    ciphertext_size = ((message_bytes // 16) + 1) * 16
    raw_size = 57 + ciphertext_size
    return 4 * ((raw_size + 2) // 3)


def payload_size_for_message_bytes(message_bytes: int) -> int:
    """Return the exact payload size for a plaintext of the given byte length."""
    return HEADER_SIZE + encrypted_size_for_message_bytes(message_bytes)


def max_message_bytes_for_payload_capacity(capacity_bytes: int) -> int:
    """Return the largest UTF-8 byte length that fits into a payload capacity."""
    if capacity_bytes < HEADER_SIZE:
        return 0

    low, high = 0, max(0, capacity_bytes - HEADER_SIZE)
    best = 0

    while low <= high:
        middle = (low + high) // 2
        if payload_size_for_message_bytes(middle) <= capacity_bytes:
            best = middle
            low = middle + 1
        else:
            high = middle - 1

    return best


def _derive_key(password: str, salt: bytes) -> bytes:
    """Derive a Fernet key from a user password."""
    if not password:
        raise ValueError("Пароль не должен быть пустым.")

    kdf = PBKDF2HMAC(
        algorithm=hashes.SHA256(),
        length=32,
        salt=salt,
        iterations=300_000,
    )
    return __import__("base64").urlsafe_b64encode(
        kdf.derive(password.encode("utf-8"))
    )


def build_payload(message: str, password: str | None = None) -> bytes:
    """Build a payload. A blank password stores plaintext with integrity checking only."""
    message_bytes = message.encode("utf-8")
    integrity = hashlib.sha256(message_bytes).digest()

    if password:
        salt = __import__("secrets").token_bytes(SALT_SIZE)
        key = _derive_key(password, salt)
        stored_data = Fernet(key).encrypt(message_bytes)
    else:
        # All-zero salt marks an unencrypted version-3 payload.
        # SHA-256 detects accidental corruption but is not authentication.
        salt = bytes(SALT_SIZE)
        stored_data = message_bytes

    header = struct.pack(
        HEADER_FORMAT,
        MAGIC,
        VERSION,
        len(stored_data),
        salt,
        integrity,
    )
    return header + stored_data


def parse_payload(payload: bytes, password: str | None = None) -> str:
    """Validate and decode both legacy encrypted and version-3 payloads."""
    if len(payload) < HEADER_SIZE:
        raise ValueError("Скрытые данные неполные.")

    magic, version, data_length, salt, integrity = struct.unpack(
        HEADER_FORMAT, payload[:HEADER_SIZE]
    )
    if magic != MAGIC:
        raise ValueError("Совместимое DCT-сообщение не найдено.")
    if version not in (2, 3):
        raise ValueError("Неподдерживаемая версия данных.")

    start = HEADER_SIZE
    end = start + data_length
    if data_length <= 0 or end > len(payload):
        raise ValueError("Скрытое сообщение неполное.")

    stored_data = payload[start:end]
    try:
        if version == 2 or salt != bytes(SALT_SIZE):
            if not password:
                raise ValueError("Для извлечения этого сообщения необходимо ввести пароль.")
            key = _derive_key(password, salt)
            message_bytes = Fernet(key).decrypt(stored_data)
        else:
            message_bytes = stored_data
    except (InvalidToken, ValueError) as exc:
        if isinstance(exc, ValueError) and str(exc).startswith("Для извлечения"):
            raise
        raise ValueError("Неверный пароль или скрытые данные повреждены.") from exc

    if hashlib.sha256(message_bytes).digest() != integrity:
        raise ValueError("Проверка целостности скрытых данных не пройдена.")
    try:
        return message_bytes.decode("utf-8")
    except UnicodeDecodeError as exc:
        raise ValueError("Скрытое сообщение не является корректным текстом UTF-8.") from exc


def bytes_to_bits(data: bytes) -> list[int]:
    """Convert bytes to a sequence of bits."""
    return [
        (byte >> shift) & 1
        for byte in data
        for shift in range(7, -1, -1)
    ]


def bits_to_bytes(bits: list[int]) -> bytes:
    """Convert a sequence of bits back to bytes."""
    if len(bits) % 8 != 0:
        raise ValueError("Длина последовательности бит должна быть кратна 8.")

    result = bytearray()

    for index in range(0, len(bits), 8):
        value = 0
        for bit in bits[index:index + 8]:
            value = (value << 1) | bit
        result.append(value)

    return bytes(result)
