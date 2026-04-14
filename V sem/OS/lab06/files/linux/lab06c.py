import subprocess
import sys
import os
import time
import datetime
import posix_ipc
import mmap
import contextlib

def worker_process(process_name, username, iterations=90):
    """Функция рабочего процесса с семафором для Linux"""
    letters = [char for char in username if char.isalpha()]
    letter_index = 0
    
    semaphore_name = "/lab06c_semaphore"
    try:
        semaphore = posix_ipc.Semaphore(semaphore_name, posix_ipc.O_CREX, initial_value=1)
    except posix_ipc.ExistentialError:
        semaphore = posix_ipc.Semaphore(semaphore_name)
    
    print(f"=== {process_name} запущен (PID: {os.getpid()}) ===")
    print("-" * 50)
    
    i = 1
    while i <= iterations:
        letter = letters[letter_index]
        letter_index = (letter_index + 1) % len(letters)
        
        current_time = datetime.datetime.now().strftime("%H:%M:%S")
        
        if 30 <= i <= 60:
            print(f"\n{current_time} --- {process_name} ОЖИДАЕТ КРИТИЧЕСКУЮ СЕКЦИЮ ---")
            
            semaphore.acquire() 
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
                semaphore.release()
        else:
            critical_mark = ""
            print(f"{current_time} - {process_name}: итерация {i:2d}, буква '{letter}'{critical_mark}")
            time.sleep(0.1)
            i += 1
    
    semaphore.close()
    current_time = datetime.datetime.now().strftime("%H:%M:%S")
    print(f"{current_time} === {process_name} завершен ===")
    
    input("Нажмите Enter для закрытия окна...")

def main():
    """Главная функция для Linux"""
    username = "User-9a444f52"
    
    current_time = datetime.datetime.now().strftime("%H:%M:%S")
    current_file = os.path.abspath(__file__)
    
    processes = []
    
    terminals = ["gnome-terminal", "konsole", "xterm", "terminator"]
    terminal_cmd = None
    
    for term in terminals:
        if os.system(f"which {term} > /dev/null 2>&1") == 0:
            terminal_cmd = term
            break
    
    if terminal_cmd == "gnome-terminal":
        cmd_a = [terminal_cmd, "--", "python3", current_file, "Процесс A", username]
        cmd_b = [terminal_cmd, "--", "python3", current_file, "Процесс B", username]
    elif terminal_cmd == "konsole":
        cmd_a = [terminal_cmd, "-e", f"python3 {current_file} 'Процесс A' '{username}'; read -p 'Нажмите Enter...'"]
        cmd_b = [terminal_cmd, "-e", f"python3 {current_file} 'Процесс B' '{username}'; read -p 'Нажмите Enter...'"]
    elif terminal_cmd == "xterm":
        cmd_a = [terminal_cmd, "-e", f"python3 {current_file} 'Процесс A' '{username}'; read -p 'Нажмите Enter...'"]
        cmd_b = [terminal_cmd, "-e", f"python3 {current_file} 'Процесс B' '{username}'; read -p 'Нажмите Enter...'"]
    else:
        cmd_a = [terminal_cmd, "-e", f"python3 {current_file} 'Процесс A' '{username}'; echo 'Нажмите Enter...'; read"]
        cmd_b = [terminal_cmd, "-e", f"python3 {current_file} 'Процесс B' '{username}'; echo 'Нажмите Enter...'; read"]
    
    process_a = subprocess.Popen(cmd_a)
    processes.append(process_a)
    
    process_b = subprocess.Popen(cmd_b)
    processes.append(process_b)
    
    worker_process("Главный процесс", username)
    
    for process in processes:
        process.wait()
    
    try:
        posix_ipc.unlink_semaphore("/lab06c_semaphore")
    except:
        pass
    
    current_time = datetime.datetime.now().strftime("%H:%M:%S")
    print("=" * 60)
    print(f"{current_time} - Все процессы завершили выполнение")
    input("Нажмите Enter для выхода...")

if __name__ == "__main__":
    if len(sys.argv) >= 3:
        worker_process(sys.argv[1], sys.argv[2])
    else:
        main()