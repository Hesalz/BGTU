import subprocess
import sys
import os
import time
import win32event
import win32api
import datetime

def worker_process(process_name, username, iterations=90):
    """Функция рабочего процесса с синхронизацией через событие"""
    letters = [char for char in username if char.isalpha()]
    letter_index = 0
    
    event_name = "Global\\Lab06d_StartEvent"
    start_event = win32event.OpenEvent(win32event.EVENT_ALL_ACCESS, False, event_name)
    
    print(f"=== {process_name} запущен (PID: {os.getpid()}) ===")
    print("-" * 50)
    
    if process_name != "Главный процесс":
        print(f"--- {process_name} ОЖИДАЕТ СИГНАЛА ДЛЯ СТАРТА ---")
        win32event.WaitForSingleObject(start_event, win32event.INFINITE)
        print(f"--- {process_name} ПОЛУЧИЛ СИГНАЛ, НАЧИНАЕТ РАБОТУ ---")
    
    i = 1
    while i <= iterations:
        letter = letters[letter_index]
        letter_index = (letter_index + 1) % len(letters)
        
        critical_mark = ""
        current_time = datetime.datetime.now().strftime("%H:%M:%S")
        print(f"{current_time} - {process_name}: итерация {i:2d}, буква '{letter}'{critical_mark}")
        time.sleep(0.1)
        i += 1
    
    current_time = datetime.datetime.now().strftime("%H:%M:%S")
    print(f"=== {process_name} завершен в {current_time} ===")
    input("Нажмите Enter для выхода...")

def main():
    """Главная функция для запуска процессов"""
    username = "User-9a444f52"
    
    event_name = "Global\\Lab06d_StartEvent"
    start_event = win32event.CreateEvent(None, True, False, event_name)
    
    current_file = os.path.abspath(__file__)
    
    processes = []
    
    cmd_a = [sys.executable, current_file, "Процесс A", username]
    process_a = subprocess.Popen(cmd_a, creationflags=subprocess.CREATE_NEW_CONSOLE)
    processes.append(process_a)
    
    cmd_b = [sys.executable, current_file, "Процесс B", username]
    process_b = subprocess.Popen(cmd_b, creationflags=subprocess.CREATE_NEW_CONSOLE)
    processes.append(process_b)
    
    print("\n--- ЭТАП 1: РОДИТЕЛЬСКИЙ ПРОЦЕСС ВЫПОЛНЯЕТ ИТЕРАЦИИ 1-15 ---")
    letters = [char for char in username if char.isalpha()]
    letter_index = 0
    
    for i in range(1, 16):
        letter = letters[letter_index]
        letter_index = (letter_index + 1) % len(letters)
        current_time = datetime.datetime.now().strftime("%H:%M:%S")
        print(f"{current_time} - Главный процесс: итерация {i:2d}, буква '{letter}'")
        time.sleep(0.1)
    
    current_time = datetime.datetime.now().strftime("%H:%M:%S")
    print(f"\n{current_time} --- ДАЕМ КОМАНДУ ДОЧЕРНИМ ПРОЦЕССАМ НАЧАТЬ РАБОТУ ---")
    win32event.SetEvent(start_event)
    
    print("--- ЭТАП 2: ОДНОВРЕМЕННОЕ ВЫПОЛНЕНИЕ ВСЕМИ ПРОЦЕССАМИ ---")
    for i in range(16, 91):
        letter = letters[letter_index]
        letter_index = (letter_index + 1) % len(letters)
        current_time = datetime.datetime.now().strftime("%H:%M:%S")
        print(f"{current_time} - Главный процесс: итерация {i:2d}, буква '{letter}'")
        time.sleep(0.1)
    
    for process in processes:
        process.wait()
    
    win32api.CloseHandle(start_event)
    
    current_time = datetime.datetime.now().strftime("%H:%M:%S")
    print("=" * 60)
    print(f"{current_time} - Все процессы завершили выполнение")
    input("Нажмите Enter для выхода...")

if __name__ == "__main__":
    if len(sys.argv) >= 3:
        worker_process(sys.argv[1], sys.argv[2])
    else:
        main()