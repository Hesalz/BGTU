import cv2
import numpy as np


BLOCK_SIZE = 8 # размер блока DCT (8×8)


def dct_block(block: np.ndarray) -> np.ndarray: # Прямое DCT: пиксели → коэффициенты частот
    block = np.float32(block)
    return cv2.dct(block)


def idct_block(coefficients: np.ndarray) -> np.ndarray: # Обратное DCT: коэффициенты → пиксели
    return cv2.idct(np.float32(coefficients))
