import time
from pathlib import Path

from app.analysis.metrics import (
    compare_images,
)
from app.core.embedding import (
    embed_message,
)
from app.core.extraction import (
    extract_message,
)


def run_single_experiment(
    source_image: str | Path,
    work_directory: str | Path,
    message: str,
    password: str,
    strength: float,
) -> dict:
    """
    Выполняет один эксперимент benchmark.

    Измеряется:
    - время внедрения;
    - время извлечения;
    - MSE;
    - PSNR;
    - успешность извлечения.
    """

    source_image = Path(
        source_image
    )

    work_directory = Path(
        work_directory
    )

    work_directory.mkdir(
        parents=True,
        exist_ok=True,
    )

    output = (
        work_directory
        / (
            f"stego_s{strength:g}_"
            f"{len(message.encode('utf-8'))}b.png"
        )
    )

    # ==========================================================
    # ВНЕДРЕНИЕ
    # ==========================================================

    started = time.perf_counter()

    result = embed_message(
        source_image,
        output,
        message,
        password,
        strength=strength,
    )

    embedding_time = (
        time.perf_counter()
        - started
    )

    # ==========================================================
    # ИЗВЛЕЧЕНИЕ
    # ==========================================================

    started = time.perf_counter()

    extracted = None
    extraction_error = ""

    try:

        extracted = extract_message(
            output,
            password,
        )

    except ValueError as exc:

        # Ошибка конкретного эксперимента
        extraction_error = str(exc)

    extraction_time = (
        time.perf_counter()
        - started
    )

    # ==========================================================
    # МЕТРИКИ ИЗОБРАЖЕНИЯ
    # ==========================================================

    metrics = compare_images(
        str(source_image),
        str(output),
    )

    verified = (
        extracted == message
    )

    return {
        "message_bytes": result[
            "message_bytes"
        ],
        "payload_bytes": result[
            "payload_bytes"
        ],
        "strength": strength,
        "embedding_time_s": embedding_time,
        "extraction_time_s": extraction_time,
        "mse": metrics["mse"],
        "psnr_db": metrics["psnr_db"],
        "verified": verified,
        "extraction_error": extraction_error,
        "output_path": str(output),
    }