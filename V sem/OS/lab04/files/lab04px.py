import threading
import time
import random
import os
import sys

def Lab_04x(iterations, user_name="User-9a444f52"):
    """
    Функция выполняет N-ое количество итераций в цикле.
    """
    try:
        letters = [char for char in user_name if char.isalpha()]
        if not letters:
            letters = ['U', 's', 'e', 'r', '-', '9', 'a', '4', '4', '4', 'f', '5', '2']
    
        letters_count = len(letters)
        
        for i in range(iterations):
            current_letter = letters[i % letters_count]
            print(f"{os.getpid()} – {threading.get_ident()} - {i + 1} – {current_letter}")
            delay = random.uniform(0.3, 0.4)
            time.sleep(delay)
            
    except Exception as e:
        print(f"Ошибка в потоке {threading.get_ident()}: {e}")
    finally:
        print(f"Поток {threading.get_ident()} завершил работу")

def show_threads_info():
    """
    Показывает информацию о потоках через /proc и ps.
    """
    pid = os.getpid()
    
    print("-" * 40)
    print("1. /proc:")
    
    try:
        task_dir = f"/proc/{pid}/task"
        if os.path.exists(task_dir):
            thread_ids = os.listdir(task_dir)
            for tid in sorted(thread_ids):
                print(f"  TID: {tid}")
        else:
            print(f"Директория {task_dir} не найдена")
    except Exception as e:
        print(f"Ошибка чтения /proc: {e}")
    
    print("-" * 40)
    print(f"2. ps:")
    
    try:
        ps_command = f"ps -T -p {pid} -o pid,tid,psr,pcpu"
        print(f"{ps_command}")
        os.system(ps_command)
    except Exception as e:
        print(f"Ошибка выполнения ps: {e}")
    print("-" * 40)

def main():
    print(f"Главный процесс PID: {os.getpid()}")

    show_threads_info()

    thread1 = threading.Thread(
        target=Lab_04x, 
        args=(50,),
        name="Thread-50-iterations"
    )
    
    thread2 = threading.Thread(
        target=Lab_04x, 
        args=(125,),
        name="Thread-125-iterations"
    )

    thread1.start()
    thread2.start()
    
    print(f"\nПоток 1 запущен: TID={thread1.ident}")
    print(f"Поток 2 запущен: TID={thread2.ident}\n")
    
    show_threads_info()
    Lab_04x(100)
    print("\nГлавный поток завершил работу, ожидание дочерних потоков...")
    show_threads_info()
    
    thread1.join()
    thread2.join()
    
    print("\nВсе потоки завершили работу")
    show_threads_info()

if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        print("\nПрограмма прервана пользователем")
    except Exception as e:
        print(f"Критическая ошибка: {e}")
    finally:
        print("\nОчистка ресурсов завершена")