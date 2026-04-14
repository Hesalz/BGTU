import threading
import time
import random
import os
import sys

def Lab_04x(iterations, user_name="User-9a444f52", thread_name="", suspend_event=None, resume_event=None):
    """
    Функция выполняет N-ое количество итераций в цикле.
    """
    try:
        letters = [char for char in user_name if char.isalpha()]
        if not letters:
            letters = ['U', 's', 'e', 'r', '-', '9', 'a', '4', '4', '4', 'f', '5', '2']
    
        letters_count = len(letters)
        
        print(f"[{thread_name}] Запущен: PID={os.getpid()}, TID={threading.get_ident()}")
        
        for i in range(iterations):
            if suspend_event and suspend_event.is_set():
                print(f"[{thread_name}] ПРИОСТАНОВЛЕН на итерации {i+1}")
                resume_event.wait()
                print(f"[{thread_name}] ВОЗОБНОВЛЕН на итерации {i+1}")
                suspend_event.clear()
                resume_event.clear()
            
            current_letter = letters[i % letters_count]
            print(f"{os.getpid()} – {threading.get_ident()} - {i + 1} – {current_letter}")
            delay = random.uniform(0.3, 0.4)
            time.sleep(delay)
            
    except Exception as e:
        print(f"Ошибка в потоке {threading.get_ident()}: {e}")
    finally:
        print(f"Поток {threading.get_ident()} завершил работу")

def main():
    print("=== Lab-04b ===")
    print(f"Главный процесс PID: {os.getpid()}")
    
    suspend_thread1 = threading.Event()
    resume_thread1 = threading.Event()
    suspend_thread2 = threading.Event()
    resume_thread2 = threading.Event()
    
    thread1 = threading.Thread(
        target=Lab_04x, 
        args=(125, "User-9a444f52", "Поток-1 (125 итераций)", suspend_thread1, resume_thread1),
        name="Thread-125-suspendable"
    )
    
    thread2 = threading.Thread(
        target=Lab_04x, 
        args=(125, "User-9a444f52", "Поток-2 (125 итераций)", suspend_thread2, resume_thread2),
        name="Thread-125-suspendable"
    )

    thread1.start()
    thread2.start()
    
    letters = [char for char in "User-9a444f52" if char.isalpha()]
    if not letters:
        letters = ['U', 's', 'e', 'r', '-', '9', 'a', '4', '4', '4', 'f', '5', '2']
    
    letters_count = len(letters)
    
    for i in range(100):
        if i == 20:
            print("\n=== ПРИОСТАНОВКА Потока-1 на 20 итерации ===")
            suspend_thread1.set()
        
        if i == 60:
            print("\n=== ВОЗОБНОВЛЕНИЕ Потока-1 на 60 итерации ===")
            resume_thread1.set()
        
        if i == 40:
            print("\n=== ПРИОСТАНОВКА Потока-2 на 40 итерации ===")
            suspend_thread2.set()
        
        current_letter = letters[i % letters_count]
        print(f"{os.getpid()} – {threading.get_ident()} - {i + 1} – {current_letter}")
        delay = random.uniform(0.3, 0.4)
        time.sleep(delay)
    
    print("\n=== ВОЗОБНОВЛЕНИЕ Потока-2 после завершения цикла ===")
    resume_thread2.set()
    
    print("Главный поток завершил работу, ожидание дочерних потоков...")

    thread1.join()
    thread2.join()
    
    print("Все потоки завершили работу")

if __name__ == "__main__":
    main()