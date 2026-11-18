import struct
from pathlib import Path


from .dct import BLOCK_SIZE, dct_block
from .embedding import (
    COEFFICIENT_A,
    COEFFICIENT_B,
)
from .image_utils import load_luma_channel
from .payload import (
    HEADER_FORMAT,
    HEADER_SIZE,
    bits_to_bytes,
    parse_payload,
)


def _read_bit(
    coefficients,
) -> int: 

    y1, x1 = COEFFICIENT_A # Координаты коэффициентов DCT, по соотношению которых определяется скрытый бит.
    y2, x2 = COEFFICIENT_B

    return int( # Бит определяется сравнением абсолютных значений двух коэффициентов.
        abs(float(coefficients[y1, x1]))
        >
        abs(float(coefficients[y2, x2]))
    )


def _read_bits(
    image,
    count: int,
) -> list[int]:

    if count < 0: # Проверка корректности запрашиваемого количества бит.
        raise ValueError(
            "Запрошенное количество бит не может быть отрицательным."
        )

    height, width = image.shape

    block_rows = height // BLOCK_SIZE # Определение количества полных блоков 8x8, доступных для чтения.
    block_cols = width // BLOCK_SIZE

    capacity_bits = (
        block_rows * block_cols
    )

    if count > capacity_bits: # Проверка, что изображение содержит достаточное количество блоков.
        raise ValueError(
            "Запрашиваемые данные превышают вместимость изображения."
        )

    bits = []

    for row in range(block_rows): # Последовательное чтение блоков изображения слева направо и сверху вниз.

        for col in range(block_cols):

            if len(bits) >= count: # Завершение чтения после получения требуемого количества бит.
                return bits

            y = row * BLOCK_SIZE
            x = col * BLOCK_SIZE

            block = image[ # Выделение текущего блока изображения размером 8x8.
                y:y + BLOCK_SIZE,
                x:x + BLOCK_SIZE,
            ]


            coefficients = dct_block( # Преобразование блока в частотную область.
                block
            )

            bits.append( # Извлечение одного скрытого бита из коэффициентов DCT.
                _read_bit(coefficients)
            )

    return bits


def extract_message(
    image_path: str | Path,
    password: str,
) -> str:

    image, _, _ = load_luma_channel(image_path) # Загрузка яркостного канала изображения для извлечения данных.


    try:

        header_bits = _read_bits( # Чтение бит заголовка, содержащего служебную информацию сообщения.
            image,
            HEADER_SIZE * 8,
        )


        if len(header_bits) != HEADER_SIZE * 8: # Проверка полноты извлечённого заголовка.
            raise ValueError(
                "Заголовок скрытого сообщения неполный."
            )

        header = bits_to_bytes( # Преобразование бит заголовка в байты.
            header_bits
        )

        if len(header) != HEADER_SIZE:
            raise ValueError(
                "Заголовок скрытого сообщения неполный."
            )

        ( # Расшифровка структуры заголовка: идентификатор, версия, длина данных, соль и контрольная сумма.
            magic,
            version,
            encrypted_length,
            salt,
            integrity,
        ) = struct.unpack(
            HEADER_FORMAT,
            header,
        )

    except (
        ValueError,
        struct.error,
    ) as exc:

        raise ValueError(  # Обработка ошибок чтения или декодирования заголовка.
            "Не удалось прочитать корректный заголовок DCT-сообщения."
        ) from exc


    if magic != b"DCTS": # Проверка идентификатора формата скрытого сообщения.
        raise ValueError(
            "Совместимое DCT-сообщение не найдено."
        )

    if version not in (2, 3): # Проверка поддерживаемой версии формата данных.
        raise ValueError(
            "Неподдерживаемая версия данных."
        )

    if encrypted_length <= 0:  # Проверка длины зашифрованной части сообщения.
        raise ValueError(
            "Скрытое сообщение имеет некорректную длину."
        )


    total_bytes = ( # Определение общего размера данных с учётом заголовка.
        HEADER_SIZE
        + encrypted_length
    )

    total_bits = total_bytes * 8


    try: 

        all_bits = _read_bits( # Извлечение полного сообщения вместе с заголовком.
            image,
            total_bits,
        )

        if len(all_bits) != total_bits:
            raise ValueError(
                "Скрытое сообщение неполное."
            )

        payload = bits_to_bytes( # Преобразование извлечённых бит в последовательность байтов.
            all_bits
        )

    except ValueError as exc:

        raise ValueError( # Обработка ситуации, когда сообщение не удалось извлечь полностью.
            "Скрытое сообщение неполное."
        ) from exc


    return parse_payload( # Проверка целостности и расшифровка сообщения с использованием пароля.
        payload,
        password,
    )
