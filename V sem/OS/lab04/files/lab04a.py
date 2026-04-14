import threading
import time
import random
import os
import sys

def Lab_04x(iterations, user_name="User-9a444f52", thread_name=""):
    """
    Функция выполняет N-ое количество итераций в цикле.
    """
    try:
        letters = [char for char in user_name if char.isalpha()]
        if not letters:
            letters = ['U', 's', 'e', 'r']
    
        letters_count = len(letters)
    
        if thread_name:
            print(f"[{thread_name}] Запущен: PID={os.getpid()}, TID={threading.get_ident()}")
        
        for i in range(iterations):
            current_letter = letters[i % letters_count]
            print(f"{os.getpid()} – {threading.get_ident()} - {i + 1} – {current_letter}")
            delay = random.uniform(0.3, 0.4)
            time.sleep(delay)
            
    except Exception as e:
        print(f"Ошибка в потоке {threading.get_ident()}: {e}")
    finally:
        print(f"Поток {threading.get_ident()} завершил работу")

def main():
    print("=== Lab-04a ===")
    print(f"Главный процесс PID: {os.getpid()}")
    
    thread1 = threading.Thread(target=Lab_04x, args=(50, "User-9a444f52", "Поток-1 (50 итераций)"))
    thread2 = threading.Thread(target=Lab_04x, args=(125, "User-9a444f52", "Поток-2 (125 итераций)"))
    
    thread1.start()
    thread2.start()
    
    Lab_04x(100, "User-9a444f52", "Главный поток (100 итераций)")
    
    print("Главный поток завершил работу, ожидание дочерних потоков...")
    
    thread1.join()
    thread2.join()
    
    print("Все потоки завершили работу")

if __name__ == "__main__":
    main()