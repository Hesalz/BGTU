import struct
from pathlib import Path

import cv2

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
    """
    Извлекает один бит из пары DCT-коэффициентов.

    Если:
        |A| > |B| -> 1
        иначе     -> 0
    """

    y1, x1 = COEFFICIENT_A
    y2, x2 = COEFFICIENT_B

    return int(
        abs(float(coefficients[y1, x1]))
        >
        abs(float(coefficients[y2, x2]))
    )


def _read_bits(
    image,
    count: int,
) -> list[int]:
    """
    Считывает заданное количество битов
    из изображения.
    """

    if count < 0:
        raise ValueError(
            "Requested bit count cannot be negative."
        )

    height, width = image.shape

    block_rows = height // BLOCK_SIZE
    block_cols = width // BLOCK_SIZE

    capacity_bits = (
        block_rows * block_cols
    )

    if count > capacity_bits:
        raise ValueError(
            "The requested payload exceeds "
            "the image capacity."
        )

    bits = []

    for row in range(block_rows):

        for col in range(block_cols):

            if len(bits) >= count:
                return bits

            y = row * BLOCK_SIZE
            x = col * BLOCK_SIZE

            block = image[
                y:y + BLOCK_SIZE,
                x:x + BLOCK_SIZE,
            ]

            # Повторно выполняем DCT,
            # чтобы получить коэффициенты
            # из stego-изображения.
            coefficients = dct_block(
                block
            )

            bits.append(
                _read_bit(coefficients)
            )

    return bits


def extract_message(
    image_path: str | Path,
    password: str,
) -> str:
    """
    Извлекает, проверяет и расшифровывает
    скрытое сообщение.
    """

    image, _, _ = load_luma_channel(image_path)

    # ==========================================================
    # ЭТАП 1. Читаем фиксированный заголовок.
    # ==========================================================

    try:

        header_bits = _read_bits(
            image,
            HEADER_SIZE * 8,
        )

        # ВАЖНО:
        # struct.unpack нельзя вызывать,
        # пока мы не убедились, что получили
        # полный заголовок.
        if len(header_bits) != HEADER_SIZE * 8:
            raise ValueError(
                "The hidden message header "
                "is incomplete."
            )

        header = bits_to_bytes(
            header_bits
        )

        if len(header) != HEADER_SIZE:
            raise ValueError(
                "The hidden message header "
                "is incomplete."
            )

        (
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

        raise ValueError(
            "Unable to read a valid "
            "DCT message header."
        ) from exc

    # ==========================================================
    # ЭТАП 2. Проверяем заголовок.
    # ==========================================================

    if magic != b"DCTS":
        raise ValueError(
            "No compatible DCT message "
            "was found."
        )

    if version != 2:
        raise ValueError(
            "Unsupported payload version."
        )

    if encrypted_length <= 0:
        raise ValueError(
            "The hidden message has "
            "an invalid length."
        )

    # ==========================================================
    # ЭТАП 3. Определяем полный размер payload.
    # ==========================================================

    total_bytes = (
        HEADER_SIZE
        + encrypted_length
    )

    total_bits = total_bytes * 8

    # ==========================================================
    # ЭТАП 4. Считываем полный payload.
    # ==========================================================

    try:

        all_bits = _read_bits(
            image,
            total_bits,
        )

        if len(all_bits) != total_bits:
            raise ValueError(
                "The hidden message "
                "is incomplete."
            )

        payload = bits_to_bytes(
            all_bits
        )

    except ValueError as exc:

        raise ValueError(
            "The hidden message "
            "is incomplete."
        ) from exc

    # ==========================================================
    # ЭТАП 5. Расшифровываем и проверяем
    # целостность сообщения.
    # ==========================================================

    return parse_payload(
        payload,
        password,
    )
