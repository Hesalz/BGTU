import os
import subprocess
import threading
from time import sleep

def read_output(pipe):
    try:
        for line in iter(pipe.readline, b''):
            try:
                decoded_line = line.decode('cp866').strip()
            except UnicodeDecodeError:
                decoded_line = line.decode('utf-8', errors='replace').strip()
            
            if decoded_line:
                print(decoded_line)
    except Exception:
        pass
    finally:
        pipe.close()

def create_process_1():
    env = os.environ.copy()
    env['PYTHONIOENCODING'] = 'cp866'
    
    process = subprocess.Popen(
        "py lab03x.py 5",
        shell=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        universal_newlines=False,
        env=env
    )
    
    stdout_thread = threading.Thread(target=read_output, args=(process.stdout,), daemon=True)
    stderr_thread = threading.Thread(target=read_output, args=(process.stderr,), daemon=True)
    
    stdout_thread.start()
    stderr_thread.start()
    
    return process

def create_process_2():
    env = os.environ.copy()
    env['PYTHONIOENCODING'] = 'cp866'
    
    process = subprocess.Popen(
        "py lab03x.py 7",
        shell=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        universal_newlines=False,
        env=env
    )
    
    stdout_thread = threading.Thread(target=read_output, args=(process.stdout,), daemon=True)
    stderr_thread = threading.Thread(target=read_output, args=(process.stderr,), daemon=True)
    
    stdout_thread.start()
    stderr_thread.start()
    
    return process

def create_process_3():
    env = os.environ.copy()
    env['PYTHONIOENCODING'] = 'cp866'
    
    process = subprocess.Popen(
        "py lab03x.py 3",
        shell=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        universal_newlines=False,
        env=env
    )
    
    stdout_thread = threading.Thread(target=read_output, args=(process.stdout,), daemon=True)
    stderr_thread = threading.Thread(target=read_output, args=(process.stderr,), daemon=True)
    
    stdout_thread.start()
    stderr_thread.start()
    
    return process

def main():
    processes = []

    p1 = create_process_1()
    processes.append(p1)

    p2 = create_process_2()
    processes.append(p2)

    p3 = create_process_3()
    processes.append(p3)

    for p in processes:
        p.wait()

if __name__ == "__main__":
    main()