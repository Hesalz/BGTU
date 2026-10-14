import cv2
import numpy as np


BLOCK_SIZE = 8


def dct_block(block: np.ndarray) -> np.ndarray:
    block = np.float32(block)
    return cv2.dct(block)


def idct_block(coefficients: np.ndarray) -> np.ndarray:
    return cv2.idct(np.float32(coefficients))
