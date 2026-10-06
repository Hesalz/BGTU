import cv2
import numpy as np


BLOCK_SIZE = 8


def dct_block(block: np.ndarray) -> np.ndarray:
    """Calculate the 2D DCT for an 8x8 image block."""
    block = np.float32(block)
    return cv2.dct(block)


def idct_block(coefficients: np.ndarray) -> np.ndarray:
    """Restore an image block from its DCT coefficients."""
    return cv2.idct(np.float32(coefficients))


def split_into_blocks(channel: np.ndarray) -> list[np.ndarray]:
    """Split a grayscale channel into complete 8x8 blocks."""
    height, width = channel.shape
    blocks = []

    usable_height = height - height % BLOCK_SIZE
    usable_width = width - width % BLOCK_SIZE

    for y in range(0, usable_height, BLOCK_SIZE):
        for x in range(0, usable_width, BLOCK_SIZE):
            blocks.append(channel[y:y + BLOCK_SIZE, x:x + BLOCK_SIZE])

    return blocks


def pad_to_block_size(channel: np.ndarray) -> np.ndarray:
    """Pad an image channel so both dimensions are divisible by 8."""
    height, width = channel.shape
    padded_height = ((height + BLOCK_SIZE - 1) // BLOCK_SIZE) * BLOCK_SIZE
    padded_width = ((width + BLOCK_SIZE - 1) // BLOCK_SIZE) * BLOCK_SIZE

    if padded_height == height and padded_width == width:
        return channel.copy()

    return cv2.copyMakeBorder(
        channel,
        0,
        padded_height - height,
        0,
        padded_width - width,
        cv2.BORDER_REPLICATE,
    )
