import cv2
import numpy as np


def calculate_mse(original: np.ndarray, processed: np.ndarray) -> float:
    """Среднеквадратичная ошибка между двумя изображениями. MSE"""
    if original.shape != processed.shape:
        raise ValueError("Изображения должны иметь одинаковые размеры и количество каналов.")

    original = original.astype(np.float64) # float64, чтобы не потерять точность при возведении в квадрат
    processed = processed.astype(np.float64)
    return float(np.mean((original - processed) ** 2))


def calculate_psnr(original: np.ndarray, processed: np.ndarray) -> float:
    """Пиковое отношение сигнал/шум (в дБ); чем выше — тем меньше искажений. PSNR"""
    mse = calculate_mse(original, processed)
    if mse == 0:
        return float("inf")  # изображения идентичны

    max_pixel = 255.0 # для 8-битных изображений
    return float(10 * np.log10((max_pixel ** 2) / mse))


def compare_images(original_path: str, processed_path: str) -> dict:
    """Считает MSE и PSNR между исходным и стего-изображением."""
    original = cv2.imread(original_path, cv2.IMREAD_UNCHANGED)
    processed = cv2.imread(processed_path, cv2.IMREAD_UNCHANGED)

    if original is None or processed is None:
        raise ValueError("Не удалось открыть одно из изображений.")

    return {
        "mse": calculate_mse(original, processed),
        "psnr_db": calculate_psnr(original, processed),
    }
