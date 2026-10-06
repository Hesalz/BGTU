from pathlib import Path
import argparse
import tempfile

from app.analysis.benchmark import (
    DEFAULT_MESSAGE_SIZES,
    DEFAULT_STRENGTHS,
    run_benchmark_series,
    save_benchmark_csv,
)


def main():
    parser = argparse.ArgumentParser(
        description="Серия экспериментов для DCT-стеганографии."
    )
    parser.add_argument(
        "--image",
        default="data/test_image.png",
        help="Путь к исходному изображению.",
    )
    parser.add_argument(
        "--password",
        default="course-project-password",
        help="Пароль для шифрования экспериментальных данных.",
    )
    parser.add_argument(
        "--sizes",
        nargs="+",
        type=int,
        default=list(DEFAULT_MESSAGE_SIZES),
        help="Размеры сообщений в байтах UTF-8.",
    )
    parser.add_argument(
        "--strengths",
        nargs="+",
        type=float,
        default=list(DEFAULT_STRENGTHS),
        help="Значения силы встраивания для проверки.",
    )
    parser.add_argument(
        "--csv",
        default="data/benchmark_results.csv",
        help="Путь для сохранения CSV-отчёта.",
    )
    args = parser.parse_args()

    image = Path(args.image)
    if not image.exists():
        print(f"Изображение не найдено: {image}")
        print("Используйте --image путь/к/изображению.png")
        raise SystemExit(1)

    total = len(args.sizes) * len(args.strengths)
    print("Серия экспериментов DCT")
    print(f"Изображение: {image}")
    print(f"Экспериментов: {total}")
    print(f"Сила: {', '.join(map(str, args.strengths))}")
    print(f"Размеры сообщений: {', '.join(map(str, args.sizes))} bytes")
    print("=" * 125)

    with tempfile.TemporaryDirectory() as work:
        results = run_benchmark_series(
            image,
            work,
            args.password,
            strengths=args.strengths,
            message_sizes=args.sizes,
        )

        for result in results:
            psnr = "—" if result["psnr_db"] is None else f"{result['psnr_db']:.2f} dB"
            mse = "—" if result["mse"] is None else f"{result['mse']:.6f}"
            embed = "—" if result["embedding_time_s"] is None else f"{result['embedding_time_s']:.4f}s"
            extract = "—" if result["extraction_time_s"] is None else f"{result['extraction_time_s']:.4f}s"

            print(
                f"size={result['message_bytes']:4} B | "
                f"strength={result['strength']:g} | "
                f"status={result['status']:4} | "
                f"PSNR={psnr:>10} | "
                f"MSE={mse:>10} | "
                f"embed={embed:>9} | "
                f"extract={extract:>9}"
            )

            if result["status"] == "SKIP":
                print(f"  причина: {result['extraction_error']}")

    csv_path = save_benchmark_csv(results, args.csv)

    ok = sum(row["status"] == "OK" for row in results)
    fail = sum(row["status"] == "FAIL" for row in results)
    skip = sum(row["status"] == "SKIP" for row in results)

    print("=" * 125)
    print(f"Итоги: успешно={ok}, ошибки={fail}, пропущено={skip}, всего={len(results)}")
    print(f"CSV-отчёт: {csv_path}")


if __name__ == "__main__":
    main()
