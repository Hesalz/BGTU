from pathlib import Path
import argparse
import tempfile

from app.analysis.benchmark import run_single_experiment


def main():
    parser = argparse.ArgumentParser(
        description="Benchmark DCT steganography strength values."
    )
    parser.add_argument(
        "--image",
        default="data/test_image.png",
        help="Path to the source image.",
    )
    parser.add_argument(
        "--message",
        default="DCT steganography benchmark message",
    )
    parser.add_argument(
        "--password",
        default="course-project-password",
    )
    args = parser.parse_args()

    image = Path(args.image)
    if not image.exists():
        print(f"Image not found: {image}")
        print("Use --image path/to/image.png")
        raise SystemExit(1)

    print("DCT benchmark")
    print("=" * 100)

    with tempfile.TemporaryDirectory() as work:
        for strength in (1, 2, 3, 4, 5):
            result = run_single_experiment(
                image,
                work,
                args.message,
                args.password,
                strength,
            )

            status = "OK" if result["verified"] else "FAIL"

            print(
                f"strength={result['strength']} | "
                f"status={status:4} | "
                f"PSNR={result['psnr_db']:.2f} dB | "
                f"MSE={result['mse']:.6f} | "
                f"embed={result['embedding_time_s']:.4f}s | "
                f"extract={result['extraction_time_s']:.4f}s"
            )

            if not result["verified"] and result["extraction_error"]:
                print(f"  error: {result['extraction_error']}")

    print("=" * 100)


if __name__ == "__main__":
    main()
