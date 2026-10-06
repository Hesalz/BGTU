import cv2
import numpy as np


def calculate_mse(original: np.ndarray, processed: np.ndarray) -> float:
    """Calculate mean squared error for grayscale or colour images."""
    if original.shape != processed.shape:
        raise ValueError("Images must have the same dimensions and channels.")

    original = original.astype(np.float64)
    processed = processed.astype(np.float64)
    return float(np.mean((original - processed) ** 2))


def calculate_psnr(original: np.ndarray, processed: np.ndarray) -> float:
    """Calculate PSNR in decibels for grayscale or colour images."""
    mse = calculate_mse(original, processed)
    if mse == 0:
        return float("inf")

    max_pixel = 255.0
    return float(10 * np.log10((max_pixel ** 2) / mse))


def compare_images(original_path: str, processed_path: str) -> dict:
    """Calculate MSE and PSNR using all available image channels."""
    original = cv2.imread(original_path, cv2.IMREAD_UNCHANGED)
    processed = cv2.imread(processed_path, cv2.IMREAD_UNCHANGED)

    if original is None or processed is None:
        raise ValueError("Unable to open one of the images.")

    return {
        "mse": calculate_mse(original, processed),
        "psnr_db": calculate_psnr(original, processed),
    }
