from pathlib import Path

import cv2
import numpy as np


def load_luma_channel(image_path: str | Path) -> tuple[np.ndarray, bool, tuple[int, ...]]:
    image = cv2.imread(str(image_path), cv2.IMREAD_UNCHANGED) # Загрузка изображения с сохранением исходного количества каналов и глубины цвета.
    if image is None:
        raise ValueError("Не удалось открыть изображение.")
    
    if image.dtype != np.uint8: # Проверка поддерживаемой глубины цвета.
        raise ValueError("Поддерживаются только 8-битные изображения.")

    if image.ndim == 2: # Для изображения в градациях серого яркостный канал совпадает с исходным.
        return image, False, image.shape

    if image.ndim == 3 and image.shape[2] == 3: # Для цветного изображения преобразование в YCrCb позволяет работать с яркостью,
        ycrcb = cv2.cvtColor(image, cv2.COLOR_BGR2YCrCb) # не изменяя цветовые компоненты изображения напрямую.
        return ycrcb[:, :, 0], True, image.shape

    raise ValueError("Неподдерживаемый формат изображения. Используйте изображение в градациях серого или цветное 24-битное изображение.")


def save_luma_channel(
    original_path: str | Path,
    output_path: str | Path,
    processed_luma: np.ndarray,
) -> None: 
    original = cv2.imread(str(original_path), cv2.IMREAD_UNCHANGED) # Повторная загрузка исходного изображения для сохранения его цветовых компонентов.
    if original is None:
        raise ValueError("Не удалось открыть исходное изображение.")

    processed_luma = np.clip(np.rint(processed_luma), 0, 255).astype(np.uint8) # Округление значений яркости и ограничение диапазоном 0–255.

    if original.ndim == 2: # Для изображения в градациях серого яркостный канал используется напрямую.
        output = processed_luma

    elif original.ndim == 3 and original.shape[2] == 3: # Для цветного изображения заменяется только канал яркости Y,
        ycrcb = cv2.cvtColor(original, cv2.COLOR_BGR2YCrCb) # после чего изображение преобразуется обратно в цветовое пространство BGR.
        ycrcb[:, :, 0] = processed_luma
        output = cv2.cvtColor(ycrcb, cv2.COLOR_YCrCb2BGR)
    else:
        raise ValueError("Неподдерживаемый формат изображения. Используйте изображение в градациях серого или цветное 24-битное изображение.")

    if not cv2.imwrite(str(output_path), output): # Сохранение обработанного изображения и проверка успешности записи.
        raise IOError("Не удалось сохранить стего-изображение.")
