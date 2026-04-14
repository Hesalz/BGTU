import os
import subprocess

def main():
    processes = []

    cmd1 = ["python3", "lab03x.py", "5"]
    processes.append(subprocess.Popen(cmd1))

    env = os.environ.copy()
    env["ITER_NUM"] = "8"
    cmd2 = ["python3", "lab03x.py"]
    processes.append(subprocess.Popen(cmd2, env=env))

    for p in processes:
        p.wait()

    print("Оба дочерних процесса завершены.")

if __name__ == "__main__":
    main()
