import cv2
import numpy as np


def calculate_mse(original: np.ndarray, processed: np.ndarray) -> float:
    """Среднеквадратичная ошибка между двумя изображениями. MSE"""
    if original.shape != processed.shape: # Проверка совпадения размеров изображений и количества каналов.
        raise ValueError("Изображения должны иметь одинаковые размеры и количество каналов.")

    original = original.astype(np.float64) # Преобразование к float64 для повышения точности вычислений при возведении в квадрат.
    processed = processed.astype(np.float64)
    return float(np.mean((original - processed) ** 2)) # Вычисление среднего значения квадратов разностей соответствующих пикселей.


def calculate_psnr(original: np.ndarray, processed: np.ndarray) -> float:
    """Пиковое отношение сигнал/шум (в дБ); чем выше — тем меньше искажений. PSNR"""
    mse = calculate_mse(original, processed) # Расчёт среднеквадратичной ошибки.
    if mse == 0: # При нулевой ошибке изображения полностью совпадают.
        return float("inf")

    max_pixel = 255.0 # Максимальное значение пикселя для 8-битного изображения.
    return float(10 * np.log10((max_pixel ** 2) / mse)) # Расчёт PSNR: чем выше значение, тем меньше искажения изображения.


def compare_images(original_path: str, processed_path: str) -> dict:
    """Считает MSE и PSNR между исходным и стего-изображением."""
    original = cv2.imread(original_path, cv2.IMREAD_UNCHANGED) # Загрузка исходного и обработанного изображений без изменения каналов.
    processed = cv2.imread(processed_path, cv2.IMREAD_UNCHANGED)

    if original is None or processed is None: # Проверка успешности загрузки обоих изображений.
        raise ValueError("Не удалось открыть одно из изображений.")

    return { # Возврат рассчитанных метрик качества изображения.
        "mse": calculate_mse(original, processed),
        "psnr_db": calculate_psnr(original, processed),
    }
