import time
from pathlib import Path

from app.analysis.metrics import compare_images
from app.core.embedding import embed_message
from app.core.extraction import extract_message
from app.core.payload import payload_size_for_message_bytes


DEFAULT_STRENGTHS = (1, 2, 3, 4, 5) # Значения силы встраивания, используемые по умолчанию в экспериментах.
DEFAULT_MESSAGE_SIZES = (10, 50, 100, 150, 200, 250) # Размеры сообщений в байтах для серии экспериментов.


def _make_benchmark_message(size_bytes: int) -> str:
    if size_bytes < 0: # Проверка допустимости размера сообщения.
        raise ValueError("Размер сообщения не может быть отрицательным.")
    return ("DCT benchmark message. " * ((size_bytes // 23) + 1))[:size_bytes] # Формирование тестового сообщения заданной длины.


def run_single_experiment(
    source_image: str | Path,
    work_directory: str | Path,
    message: str,
    password: str,
    strength: float,
) -> dict:
    source_image = Path(source_image) # Преобразование пути исходного изображения в объект Path.
    work_directory = Path(work_directory) # Подготовка пути к рабочему каталогу.
    work_directory.mkdir(parents=True, exist_ok=True) # Создание каталога для результатов, если он отсутствует.

    output = work_directory / (
        f"stego_s{strength:g}_{len(message.encode('utf-8'))}b.png" # Формирование имени выходного изображения с указанием силы встраивания и размера сообщения.
    )

    started = time.perf_counter() # Начало измерения времени встраивания.
    result = embed_message(
        source_image,
        output,
        message,
        password,
        strength=strength,
    )
    embedding_time = time.perf_counter() - started # Вычисление времени встраивания сообщения.

    started = time.perf_counter() # Начало измерения времени извлечения.
    extracted = None
    extraction_error = ""
    try:
        extracted = extract_message(output, password) # Попытка извлечения сообщения из созданного стего-изображения.
    except ValueError as exc:
        extraction_error = str(exc) # Сохранение текста ошибки при неудачном извлечении.
    extraction_time = time.perf_counter() - started # Вычисление времени извлечения сообщения.

    metrics = compare_images(str(source_image), str(output)) # Расчёт показателей качества стего-изображения.
    verified = extracted == message # Проверка совпадения извлечённого сообщения с исходным.

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
    source_image = Path(source_image) # Преобразование пути исходного изображения в объект Path.
    work_directory = Path(work_directory) # Подготовка пути к каталогу результатов серии экспериментов.
    work_directory.mkdir(parents=True, exist_ok=True) # Создание каталога, если он отсутствует.

    results = []

    # Последовательный запуск экспериментов для каждого размера сообщения и значения strength.
    for message_size in message_sizes:
        message = _make_benchmark_message(int(message_size)) # Создание тестового сообщения заданного размера.
        actual_size = len(message.encode("utf-8")) # Определение фактического размера сообщения в байтах.
        required_payload = payload_size_for_message_bytes(actual_size) # Расчёт необходимой вместимости с учётом служебных данных.

        for strength in strengths:
            base = {
                "strength": float(strength),
                "message_bytes": actual_size,
                "required_payload_bytes": required_payload,
                "status": "SKIP", # Статус по умолчанию до завершения эксперимента.
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
                base.update(result) # Добавление полученных результатов эксперимента к базовым данным.
                base["status"] = "OK" if result["verified"] else "FAIL" # Определение статуса по результату контрольного извлечения.
            except ValueError as exc:
                base["status"] = "SKIP" # Пропуск эксперимента при возникновении ошибки.
                base["extraction_error"] = str(exc) # Сохранение причины пропуска.

            results.append(base) # Добавление результата текущего эксперимента в общий список.

    return results # Возврат результатов всей серии экспериментов.