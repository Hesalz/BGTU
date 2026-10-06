import tempfile
from pathlib import Path

import cv2
import numpy as np
import pytest

from app.core.embedding import (
    calculate_capacity,
    calculate_message_capacity,
    embed_message,
)
from app.core.extraction import extract_message


def create_test_image(path: Path, width: int = 512, height: int = 512) -> None:
    """Create a deterministic textured test image."""
    rng = np.random.default_rng(42)
    image = rng.integers(0, 256, (height, width), dtype=np.uint8)

    # Add a smooth gradient to avoid a completely random-only image.
    gradient = np.linspace(0, 80, width, dtype=np.float32)
    image = np.clip(image.astype(np.float32) * 0.7 + gradient, 0, 255).astype(np.uint8)

    cv2.imwrite(str(path), image)


def test_round_trip():
    with tempfile.TemporaryDirectory() as tmp:
        tmp = Path(tmp)
        source = tmp / "source.png"
        stego = tmp / "stego.png"

        create_test_image(source)

        message = "Курсовой проект DCT. Проверка внедрения и извлечения."
        password = "test-password"

        result = embed_message(source, stego, message, password, strength=3.0)
        extracted = extract_message(stego, password)

        assert result["message_bytes"] == len(message.encode("utf-8"))
        assert result["payload_bytes"] > result["message_bytes"]
        assert extracted == message


def test_wrong_password_is_rejected():
    with tempfile.TemporaryDirectory() as tmp:
        tmp = Path(tmp)
        source = tmp / "source.png"
        stego = tmp / "stego.png"

        create_test_image(source)
        embed_message(source, stego, "secret message", "correct", strength=3.0)

        with pytest.raises(ValueError):
            extract_message(stego, "wrong")


def test_capacity_is_positive():
    with tempfile.TemporaryDirectory() as tmp:
        tmp = Path(tmp)
        source = tmp / "source.png"
        create_test_image(source)

        assert calculate_capacity(source) > 0


def test_oversized_message_is_rejected():
    with tempfile.TemporaryDirectory() as tmp:
        tmp = Path(tmp)
        source = tmp / "source.png"
        stego = tmp / "stego.png"

        create_test_image(source)

        capacity = calculate_capacity(source)

        # The payload contains an additional header, so this message must exceed capacity.
        oversized_message = "A" * (capacity + 100)

        with pytest.raises(ValueError):
            embed_message(source, stego, oversized_message, "password")


def test_strength_3_round_trip():
    with tempfile.TemporaryDirectory() as tmp:
        tmp = Path(tmp)
        source = tmp / "source.png"
        stego = tmp / "stego.png"
        create_test_image(source)

        message = "strength 3"
        password = "password"
        embed_message(source, stego, message, password, strength=3)
        assert extract_message(stego, password) == message


def test_history_database():
    from app.database.history import HistoryDatabase

    with tempfile.TemporaryDirectory() as tmp:
        db = HistoryDatabase(Path(tmp) / "history.db")
        db.add("Внедрение", "test.png", 10, 3, 60.0, 0.05, True)
        rows = db.list_recent()
        assert len(rows) == 1
        assert rows[0]["operation"] == "Внедрение"
        assert rows[0]["success"] == 1


def test_colour_round_trip_preserves_colour_image():
    with tempfile.TemporaryDirectory() as tmp:
        tmp = Path(tmp)
        source = tmp / "colour_source.png"
        stego = tmp / "colour_stego.png"

        rng = np.random.default_rng(123)
        image = rng.integers(0, 256, (512, 512, 3), dtype=np.uint8)
        assert cv2.imwrite(str(source), image)

        message = "Проверка цветного изображения DCT."
        password = "colour-password"

        result = embed_message(source, stego, message, password, strength=2.0)
        extracted = extract_message(stego, password)

        saved = cv2.imread(str(stego), cv2.IMREAD_UNCHANGED)
        assert saved is not None
        assert saved.ndim == 3
        assert saved.shape == image.shape
        assert result["is_color"] is True
        assert extracted == message


def test_message_capacity_accounts_for_payload_overhead():
    with tempfile.TemporaryDirectory() as tmp:
        tmp = Path(tmp)
        source = tmp / "source.png"
        create_test_image(source)

        capacity = calculate_message_capacity(source)

        assert capacity["payload_bytes"] == calculate_capacity(source)
        assert capacity["header_bytes"] == 57
        assert 0 < capacity["message_bytes"] < capacity["payload_bytes"]


def test_benchmark_series_returns_matrix():
    from app.analysis.benchmark import run_benchmark_series

    with tempfile.TemporaryDirectory() as tmp:
        tmp = Path(tmp)
        source = tmp / "source.png"
        create_test_image(source, width=512, height=512)

        results = run_benchmark_series(
            source,
            tmp / "work",
            "benchmark-password",
            strengths=(1, 2, 3, 4, 5),
            message_sizes=(10, 50),
        )

        assert len(results) == 10
        assert {row["strength"] for row in results} == {1.0, 2.0, 3.0, 4.0, 5.0}
        assert {row["message_bytes"] for row in results} == {10, 50}
        assert all(row["status"] in {"OK", "FAIL", "SKIP"} for row in results)


def test_round_trip_without_password():
    """A blank password stores the message without encryption, with SHA-256 integrity check."""
    with tempfile.TemporaryDirectory() as tmp:
        tmp = Path(tmp)
        source = tmp / "source.png"
        stego = tmp / "stego.png"
        create_test_image(source)
        message = "Сообщение без пароля"
        embed_message(source, stego, message, password=None, strength=3)
        assert extract_message(stego, password=None) == message


def test_lossless_output_formats_round_trip():
    """BMP and TIFF preserve the embedded DCT bits when saved losslessly."""
    with tempfile.TemporaryDirectory() as tmp:
        tmp = Path(tmp)
        source = tmp / "source.png"
        create_test_image(source)
        message = "Проверка формата"
        for extension in ("bmp", "tiff"):
            stego = tmp / f"stego.{extension}"
            embed_message(source, stego, message, "format-test", strength=3)
            assert extract_message(stego, "format-test") == message
