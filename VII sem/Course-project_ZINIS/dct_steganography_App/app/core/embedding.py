from pathlib import Path

import cv2
import numpy as np

from .dct import BLOCK_SIZE, dct_block, idct_block
from .payload import (
    HEADER_SIZE,
    build_payload,
    bytes_to_bits,
    max_message_bytes_for_payload_capacity,
)
from .image_utils import load_luma_channel, save_luma_channel


# Два среднечастотных коэффициента DCT.
# Они используются для кодирования одного бита.
COEFFICIENT_A = (3, 4)
COEFFICIENT_B = (4, 3)

# Минимальный гарантированный зазор между коэффициентами.
MIN_SEPARATION = 2.0


def _force_bit(
    coefficients: np.ndarray,
    bit: int,
    strength: float,
    colour_mode: bool = False,
) -> None:
    """
    Принудительно задаёт значение одного бита.

    Бит 1:
        |A| > |B|

    Бит 0:
        |A| < |B|

    Strength определяет минимальный зазор между коэффициентами.
    Это повышает вероятность сохранения бита после:
        DCT -> изменение -> IDCT -> округление -> DCT
    """

    y1, x1 = COEFFICIENT_A
    y2, x2 = COEFFICIENT_B

    a = float(coefficients[y1, x1])
    b = float(coefficients[y2, x2])

    # Чем больше strength, тем больше расстояние
    # между коэффициентами.
    # Colour images are reconstructed through BGR <-> YCrCb conversion.
    # This introduces a small additional rounding error in the luminance
    # channel, so a wider DCT margin is used to keep extraction reliable.
    separation_scale = 6.0 if colour_mode else 2.0
    separation = max(
        MIN_SEPARATION,
        float(strength) * separation_scale,
    )

    if bit == 1:
        required_a = abs(b) + separation

        if abs(a) < required_a:
            coefficients[y1, x1] = np.copysign(
                required_a,
                a if a != 0 else 1.0,
            )

    else:
        required_b = abs(a) + separation

        if abs(b) < required_b:
            coefficients[y2, x2] = np.copysign(
                required_b,
                b if b != 0 else 1.0,
            )


def calculate_capacity(
    image_path: str | Path,
) -> int:
    """
    Возвращает приблизительную вместимость изображения
    в байтах.

    Один блок 8x8 хранит один бит.
    """

    image, _, _ = load_luma_channel(image_path)

    height, width = image.shape

    block_count = (
        (height // BLOCK_SIZE)
        * (width // BLOCK_SIZE)
    )

    return block_count // 8


def calculate_message_capacity(
    image_path: str | Path,
) -> dict:
    """Return physical payload capacity and actual UTF-8 message capacity."""
    payload_capacity = calculate_capacity(image_path)
    max_message_bytes = max_message_bytes_for_payload_capacity(
        payload_capacity
    )

    return {
        "payload_bytes": payload_capacity,
        "header_bytes": HEADER_SIZE,
        "message_bytes": max_message_bytes,
    }


def embed_message(
    image_path: str | Path,
    output_path: str | Path,
    message: str,
    password: str | None = None,
    strength: float = 3.0,
) -> dict:
    """
    Встраивает сообщение в изображение
    с использованием блочного DCT.
    """

    if strength <= 0:
        raise ValueError(
            "Сила встраивания должна быть положительным числом."
        )

    image, is_color, original_shape = load_luma_channel(image_path)

    # Формируем зашифрованный payload.
    payload = build_payload(
        message,
        password,
    )

    # Преобразуем payload в последовательность битов.
    bits = bytes_to_bits(payload)

    height, width = image.shape

    block_rows = height // BLOCK_SIZE
    block_cols = width // BLOCK_SIZE

    capacity_bits = (
        block_rows * block_cols
    )

    if len(bits) > capacity_bits:
        raise ValueError(
            f"Сообщение слишком большое. "
            f"Требуется: {len(bits)} бит, доступно: "
            f"{capacity_bits} бит."
        )

    result = image.astype(
        np.float32
    ).copy()

    bit_index = 0

    for row in range(block_rows):

        for col in range(block_cols):

            if bit_index >= len(bits):
                break

            y = row * BLOCK_SIZE
            x = col * BLOCK_SIZE

            block = result[
                y:y + BLOCK_SIZE,
                x:x + BLOCK_SIZE,
            ]

            # Прямое DCT.
            coefficients = dct_block(
                block
            )

            # Встраиваем текущий бит.
            _force_bit(
                coefficients,
                bits[bit_index],
                strength,
                colour_mode=is_color,
            )

            # Обратное DCT.
            restored = idct_block(
                coefficients
            )

            result[
                y:y + BLOCK_SIZE,
                x:x + BLOCK_SIZE,
            ] = restored

            bit_index += 1

        if bit_index >= len(bits):
            break

    # Возвращаем значения пикселей
    # в допустимый диапазон.
    save_luma_channel(
        image_path,
        output_path,
        result,
    )

    return {
        "message_bytes": len(
            message.encode("utf-8")
        ),
        "payload_bytes": len(payload),
        "embedded_bits": len(bits),
        "capacity_bits": capacity_bits,
        "strength": strength,
        "is_color": is_color,
        "original_shape": original_shape,
        "output_path": str(Path(output_path)),
    }