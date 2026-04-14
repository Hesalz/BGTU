import ctypes
import ctypes.util
import subprocess
import os
import sys

def get_current_time_ms():
    """Получает текущее время в миллисекундах с помощью POSIX функций"""
    libc = ctypes.CDLL(ctypes.util.find_library('c'))
    
    class timeval(ctypes.Structure):
        _fields_ = [
            ("tv_sec", ctypes.c_long),
            ("tv_usec", ctypes.c_long)
        ]
    
    tv = timeval()
    libc.gettimeofday(ctypes.byref(tv), None)
    
    return (tv.tv_sec * 1000) + (tv.tv_usec // 1000)

def create_lab07x_script(duration_minutes, process_name):
    duration_seconds = duration_minutes * 60
    script_content = f'''
import ctypes
import ctypes.util

def get_current_time_ms():
    libc = ctypes.CDLL(ctypes.util.find_library('c'))
    
    class timeval(ctypes.Structure):
        _fields_ = [
            ("tv_sec", ctypes.c_long),
            ("tv_usec", ctypes.c_long)
        ]
    
    tv = timeval()
    libc.gettimeofday(ctypes.byref(tv), None)
    
    return (tv.tv_sec * 1000) + (tv.tv_usec // 1000)

def is_prime(n):
    if n < 2:
        return False
    if n == 2:
        return True
    if n % 2 == 0:
        return False
    i = 3
    while i * i <= n:
        if n % i == 0:
            return False
        i += 2
    return True

def main():
    start_time = get_current_time_ms()
    duration_ms = {duration_seconds} * 1000
    
    count = 0
    number = 2
    
    while get_current_time_ms() - start_time < duration_ms:
        if is_prime(number):
            count += 1
        number += 1
    
    end_time = get_current_time_ms()
    elapsed_time = (end_time - start_time) / 1000.0
    print(f"{process_name}: {{count}} простых чисел за {{elapsed_time:.3f}} секунд")

if __name__ == "__main__":
    main()
'''
    filename = f"lab07x_{process_name}.py"
    with open(filename, 'w', encoding='utf-8') as f:
        f.write(script_content)
    return filename

def main():
    script1 = create_lab07x_script(1, "process_1min")
    script2 = create_lab07x_script(2, "process_2min")
    
    try:
        process1 = subprocess.Popen([sys.executable, script1])
        process2 = subprocess.Popen([sys.executable, script2])
        
        process1.wait()
        process2.wait()
        
    finally:
        if os.path.exists(script1):
            os.remove(script1)
        if os.path.exists(script2):
            os.remove(script2)

if __name__ == "__main__":
    main()