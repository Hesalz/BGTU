import os
import threading
import time
import ctypes
import sys

libc = ctypes.CDLL('libc.so.6')
libc.sched_getcpu.restype = ctypes.c_int

def set_thread_priority(priority):
    try:
        result = libc.setpriority(0, 0, priority)
        return result == 0
    except:
        return False

def get_thread_priority():
    try:
        libc.getpriority.restype = ctypes.c_int
        priority = libc.getpriority(0, 0)
        if priority == -1:
            errno = ctypes.get_errno()
            if errno != 0:
                return "Недоступно"
        return priority
    except:
        return "Недоступно"

def get_current_processor():
    try:
        return libc.sched_getcpu()
    except:
        return "Недоступно"

def set_processor_affinity(mask):
    try:
        if mask == 0:
            cpu_count = os.cpu_count()
            affinity = list(range(cpu_count))
        else:
            affinity = [0]
        
        os.sched_setaffinity(0, affinity)
        return True
    except Exception as e:
        print(f"Ошибка установки маски процессоров: {e}")
        return False

def thread_function(thread_id, priority, results):
    start_time = time.perf_counter()
    
    try:
        if priority != 0:
            set_thread_priority(priority)
    except:
        pass
    
    total_iterations = 1000000
    report_interval = 1000
    
    iteration_count = 0
    
    for i in range(total_iterations + 1):
        iteration_count = i
        
        result = 0
        for j in range(1000):
            result += j * j
            
        if i % report_interval == 0:
            current_priority = get_thread_priority()
            print(f"--- Поток {thread_id}, Итерация {i} ---")
            print(f"Идентификатор процесса: {os.getpid()}")
            print(f"Идентификатор потока: {threading.get_ident()}")
            print(f"Уровень любезности: {current_priority}")
            print(f"Номер назначенного для выполнения процессора: {get_current_processor()}")
    
    end_time = time.perf_counter()
    elapsed_time = end_time - start_time
    
    results[thread_id] = {
        'iterations': iteration_count,
        'time': elapsed_time
    }

def main():
    if len(sys.argv) != 4:
        print("Использование: sudo python3 lab05c.py <P1> <P2> <P3>")
        print("  P1 - маска родственности процессоров:")
        print("       0 = все процессоры")
        print("       1 = только 1-й процессор")  
        print("  P2 - приоритет 1-го потока (-20 to 19)")
        print("  P3 - приоритет 2-го потока (-20 to 19)")
        print("\nТЕСТЫ:")
        print("  sudo python3 lab05c.py 0 0 0    # Все процессоры, 0, 0")
        print("  sudo python3 lab05c.py 0 19 -20 # Все процессоры, 19, -20") 
        print("  sudo python3 lab05c.py 1 19 -20 # 1 процессор, 19, -20")
        return

    try:
        start_time = time.perf_counter()
        
        P1 = int(sys.argv[1])
        P2 = int(sys.argv[2]) 
        P3 = int(sys.argv[3])
        
        print("=" * 70)
        print(f"Маска процессоров (P1): {P1}")
        print(f"Приоритет потока 1 (P2): {P2}")
        print(f"Приоритет потока 2 (P3): {P3}")
        
        set_processor_affinity(P1)
        
        results = {}
        thread1 = threading.Thread(target=thread_function, args=(1, P2, results))
        thread2 = threading.Thread(target=thread_function, args=(2, P3, results))
        
        thread1.start()
        thread2.start()
        
        thread1.join()
        thread2.join()
        
        end_time = time.perf_counter()
        total_time = end_time - start_time
        
        print("\n" + "=" * 70)
        print("РЕЗУЛЬТАТЫ:")
        print(f"Поток 1 - выполнено итераций: {results[1]['iterations']}, время: {results[1]['time']:.2f} секунд")
        print(f"Поток 2 - выполнено итераций: {results[2]['iterations']}, время: {results[2]['time']:.2f} секунд")
        print(f"Общее время выполнения: {total_time:.2f} секунд")
        
    except ValueError:
        print("Ошибка: все параметры должны быть целыми числами!")
    except Exception as e:
        print(f"Ошибка: {e}")

if __name__ == "__main__":
    main()