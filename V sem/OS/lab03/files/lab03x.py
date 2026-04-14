import os
import sys
import time

def main():
    if len(sys.argv) > 1:
        iterations = int(sys.argv[1])
    else:
        env_var = os.getenv("ITER_NUM")
        if env_var:
            iterations = int(env_var)
        else:
            print("Ошибка: не указано количество итераций (ни аргумент, ни переменная окружения)!")
            sys.exit(1)

    pid = os.getpid()
    print(f"PID процесса: {pid}")
    print(f"Количество итераций: {iterations}")

    for i in range(iterations):
        print(f"Итерация {i + 1} (PID={pid})")
        time.sleep(0.5)

    print("Работа завершена!")

if __name__ == "__main__":
    main()
