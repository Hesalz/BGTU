import time
from pathlib import Path

from app.analysis.metrics import compare_images
from app.core.embedding import embed_message
from app.core.extraction import extract_message
from app.core.payload import payload_size_for_message_bytes


DEFAULT_STRENGTHS = (1, 2, 3, 4, 5)
DEFAULT_MESSAGE_SIZES = (10, 50, 100, 150, 200, 250)


def _make_benchmark_message(size_bytes: int) -> str:
    if size_bytes < 0:
        raise ValueError("Размер сообщения не может быть отрицательным.")
    return ("DCT benchmark message. " * ((size_bytes // 23) + 1))[:size_bytes]


def run_single_experiment(
    source_image: str | Path,
    work_directory: str | Path,
    message: str,
    password: str,
    strength: float,
) -> dict:
    source_image = Path(source_image)
    work_directory = Path(work_directory)
    work_directory.mkdir(parents=True, exist_ok=True)

    output = work_directory / (
        f"stego_s{strength:g}_{len(message.encode('utf-8'))}b.png"
    )

    started = time.perf_counter()
    result = embed_message(
        source_image,
        output,
        message,
        password,
        strength=strength,
    )
    embedding_time = time.perf_counter() - started

    started = time.perf_counter()
    extracted = None
    extraction_error = ""
    try:
        extracted = extract_message(output, password)
    except ValueError as exc:
        extraction_error = str(exc)
    extraction_time = time.perf_counter() - started

    metrics = compare_images(str(source_image), str(output))
    verified = extracted == message

    return {
        "message_bytes": result["message_bytes"],
        "payload_bytes": result["payload_bytes"],
        "strength": strength,
        "embedding_time_s": embedding_time,
        "extraction_time_s": extraction_time,
        "mse": metrics["mse"],
        "psnr_db": metrics["psnr_db"],
        "verified": verified,
        "extraction_error": extraction_error,
        "output_path": str(output),
    }


def run_benchmark_series(
    source_image: str | Path,
    work_directory: str | Path,
    password: str,
    strengths=DEFAULT_STRENGTHS,
    message_sizes=DEFAULT_MESSAGE_SIZES,
) -> list[dict]:
    source_image = Path(source_image)
    work_directory = Path(work_directory)
    work_directory.mkdir(parents=True, exist_ok=True)

    results = []

    for message_size in message_sizes:
        message = _make_benchmark_message(int(message_size))
        actual_size = len(message.encode("utf-8"))
        required_payload = payload_size_for_message_bytes(actual_size)

        for strength in strengths:
            base = {
                "strength": float(strength),
                "message_bytes": actual_size,
                "required_payload_bytes": required_payload,
                "status": "SKIP",
                "verified": False,
                "embedding_time_s": None,
                "extraction_time_s": None,
                "mse": None,
                "psnr_db": None,
                "payload_bytes": None,
                "extraction_error": "",
                "output_path": "",
            }

            try:
                result = run_single_experiment(
                    source_image,
                    work_directory,
                    message,
                    password,
                    float(strength),
                )
                base.update(result)
                base["status"] = "OK" if result["verified"] else "FAIL"
            except ValueError as exc:
                base["status"] = "SKIP"
                base["extraction_error"] = str(exc)

            results.append(base)

    return results
