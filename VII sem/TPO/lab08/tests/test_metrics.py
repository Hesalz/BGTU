import numpy as np
import pytest
from app.analysis.metrics import calculate_mse, calculate_psnr


def test_mse_identical_images_is_zero():
    image = np.zeros((4, 4), dtype=np.uint8)
    assert calculate_mse(image, image) == 0.0


def test_mse_calculates_average_squared_difference():
    original = np.array([[0, 10]], dtype=np.uint8)
    processed = np.array([[2, 14]], dtype=np.uint8)
    assert calculate_mse(original, processed) == 10.0


def test_metrics_reject_different_shapes():
    with pytest.raises(ValueError, match='одинаковые размеры'):
        calculate_mse(np.zeros((2, 2)), np.zeros((2, 3)))


def test_psnr_is_infinite_for_identical_images():
    image = np.full((3, 3), 128, dtype=np.uint8)
    assert calculate_psnr(image, image) == float('inf')


