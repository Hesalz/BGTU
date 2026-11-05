import numpy as np
import pytest
from unittest.mock import Mock
import app.core.embedding as embedding
from app.core.extraction import _read_bits


def test_embed_rejects_non_positive_strength_before_io():
    loader = Mock(side_effect=AssertionError('image loader should not be called'))
    original = embedding.load_luma_channel
    embedding.load_luma_channel = loader
    try:
        with pytest.raises(ValueError, match='положительным'):
            embedding.embed_message('input.png', 'output.png', 'hello', strength=0)
        loader.assert_not_called()
    finally:
        embedding.load_luma_channel = original


def test_embed_rejects_payload_larger_than_capacity(monkeypatch, tmp_path):
    monkeypatch.setattr(embedding, 'load_luma_channel', lambda path: (np.zeros((8, 8), dtype=np.uint8), False, (8, 8)))
    monkeypatch.setattr(embedding, 'build_payload', lambda message, password: b'x' * 2)
    monkeypatch.setattr(embedding, 'bytes_to_bits', lambda payload: [0] * 16)
    save = Mock()
    monkeypatch.setattr(embedding, 'save_luma_channel', save)
    with pytest.raises(ValueError, match='слишком большое'):
        embedding.embed_message('input.png', tmp_path / 'out.png', 'x')
    save.assert_not_called()


def test_read_bits_rejects_negative_count():
    with pytest.raises(ValueError, match='отрицательным'):
        _read_bits(np.zeros((8, 8)), -1)


def test_read_bits_rejects_count_over_capacity():
    with pytest.raises(ValueError, match='превышают вместимость'):
        _read_bits(np.zeros((8, 8)), 2)
