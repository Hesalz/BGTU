from pathlib import Path

import cv2
import numpy as np


def load_luma_channel(image_path: str | Path) -> tuple[np.ndarray, bool, tuple[int, ...]]:
    image = cv2.imread(str(image_path), cv2.IMREAD_UNCHANGED)
    if image is None:
        raise ValueError("Не удалось открыть изображение.")

    if image.ndim == 2:
        return image, False, image.shape

    if image.ndim == 3 and image.shape[2] == 3:
        ycrcb = cv2.cvtColor(image, cv2.COLOR_BGR2YCrCb)
        return ycrcb[:, :, 0], True, image.shape

    raise ValueError("Неподдерживаемый формат изображения. Используйте изображение в градациях серого или цветное 24-битное изображение.")


def save_luma_channel(
    original_path: str | Path,
    output_path: str | Path,
    processed_luma: np.ndarray,
) -> None:
    original = cv2.imread(str(original_path), cv2.IMREAD_UNCHANGED)
    if original is None:
        raise ValueError("Не удалось открыть исходное изображение.")

    processed_luma = np.clip(np.rint(processed_luma), 0, 255).astype(np.uint8)

    if original.ndim == 2:
        output = processed_luma
    elif original.ndim == 3 and original.shape[2] == 3:
        ycrcb = cv2.cvtColor(original, cv2.COLOR_BGR2YCrCb)
        ycrcb[:, :, 0] = processed_luma
        output = cv2.cvtColor(ycrcb, cv2.COLOR_YCrCb2BGR)
    else:
        raise ValueError("Неподдерживаемый формат изображения. Используйте изображение в градациях серого или цветное 24-битное изображение.")

    if not cv2.imwrite(str(output_path), output):
        raise IOError("Не удалось сохранить стего-изображение.")
