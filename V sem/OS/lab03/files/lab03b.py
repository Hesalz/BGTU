import os
import subprocess

def create_process_1():
    os.environ["ITER_NUM"] = "4"
    process = subprocess.Popen(
        "python lab03x.py",
        shell=False
    )
    return process

def create_process_3():
    env = os.environ.copy()
    env["ITER_NUM"] = "8"
    process = subprocess.Popen(
        ["python", "lab03x.py"],
        env=env,
        shell=False
    )
    return process

def main():
    processes = []

    p1 = create_process_1()
    if p1:
        processes.append(p1)
        p1.wait()

    p3 = create_process_3()
    if p3:
        processes.append(p3)
        p3.wait()

if __name__ == "__main__":
    main()