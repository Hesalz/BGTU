import subprocess
import sys
import os
import time
import win32event
import win32api
import datetime

def worker_process(process_name, username, iterations=90):
    """Функция рабочего процесса с бинарным семафором"""
    letters = [char for char in username if char.isalpha()]
    letter_index = 0
    
    semaphore_name = "Global\\Lab06c_BinarySemaphore"
    semaphore = win32event.CreateSemaphore(None, 1, 1, semaphore_name)
    
    print(f"=== {process_name} запущен (PID: {os.getpid()}) ===")
    print("-" * 50)
    
    i = 1
    while i <= iterations:
        letter = letters[letter_index]
        letter_index = (letter_index + 1) % len(letters)
        
        current_time = datetime.datetime.now().strftime("%H:%M:%S")
        
        if 30 <= i <= 60:
            print(f"\n{current_time} --- {process_name} ОЖИДАЕТ КРИТИЧЕСКУЮ СЕКЦИЮ ---")
            
            win32event.WaitForSingleObject(semaphore, win32event.INFINITE)
            try:
                print(f"{current_time} --- {process_name} ВОШЕЛ В КРИТИЧЕСКУЮ СЕКЦИЮ ---")
                
                while i <= 60 and i <= iterations:
                    current_time = datetime.datetime.now().strftime("%H:%M:%S")
                    critical_mark = " [CRITICAL]"
                    print(f"{current_time} - {process_name}: итерация {i:2d}, буква '{letter}'{critical_mark}")
                    time.sleep(0.1)
                    i += 1
                    if i <= iterations:
                        letter = letters[letter_index]
                        letter_index = (letter_index + 1) % len(letters)
                
                current_time = datetime.datetime.now().strftime("%H:%M:%S")
                print(f"{current_time} --- {process_name} ВЫШЕЛ ИЗ КРИТИЧЕСКОЙ СЕКЦИИ ---\n")
            finally:
                win32event.ReleaseSemaphore(semaphore, 1)
        else:
            critical_mark = ""
            print(f"{current_time} - {process_name}: итерация {i:2d}, буква '{letter}'{critical_mark}")
            time.sleep(0.1)
            i += 1
    
    win32api.CloseHandle(semaphore)
    current_time = datetime.datetime.now().strftime("%H:%M:%S")
    print(f"{current_time} === {process_name} завершен ===")
    input("Нажмите Enter для выхода...")

def main():
    """Главная функция для запуска процессов в отдельных консолях"""
    username = "User-9a444f52"
    
    current_time = datetime.datetime.now().strftime("%H:%M:%S")
    
    current_file = os.path.abspath(__file__)
    
    processes = []
    
    current_time = datetime.datetime.now().strftime("%H:%M:%S")
    cmd_a = [sys.executable, current_file, "Процесс A", username]
    process_a = subprocess.Popen(cmd_a, creationflags=subprocess.CREATE_NEW_CONSOLE)
    processes.append(process_a)
    
    current_time = datetime.datetime.now().strftime("%H:%M:%S")
    cmd_b = [sys.executable, current_file, "Процесс B", username]
    process_b = subprocess.Popen(cmd_b, creationflags=subprocess.CREATE_NEW_CONSOLE)
    processes.append(process_b)
    
    current_time = datetime.datetime.now().strftime("%H:%M:%S")
    worker_process("Главный процесс", username)
    
    for process in processes:
        process.wait()
    
    current_time = datetime.datetime.now().strftime("%H:%M:%S")
    print("=" * 60)
    print(f"{current_time} - Все процессы завершили выполнение")
    input("Нажмите Enter для выхода...")

if __name__ == "__main__":
    if len(sys.argv) >= 3:
        worker_process(sys.argv[1], sys.argv[2])
    else:
        main()