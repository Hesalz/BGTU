import os
import sys
import threading
import time
import psutil
import ctypes
from ctypes import wintypes

THREAD_PRIORITY_IDLE = -15
THREAD_PRIORITY_LOWEST = -2
THREAD_PRIORITY_BELOW_NORMAL = -1
THREAD_PRIORITY_NORMAL = 0
THREAD_PRIORITY_ABOVE_NORMAL = 1
THREAD_PRIORITY_HIGHEST = 2
THREAD_PRIORITY_TIME_CRITICAL = 15

kernel32 = ctypes.windll.kernel32
kernel32.GetCurrentThread.argtypes = []
kernel32.GetCurrentThread.restype = wintypes.HANDLE
kernel32.SetThreadPriority.argtypes = [wintypes.HANDLE, ctypes.c_int]
kernel32.SetThreadPriority.restype = wintypes.BOOL

def set_thread_priority(priority_code):
    """Установить приоритет потока через Win32 API"""
    try:
        priority_map = {
            -2: THREAD_PRIORITY_LOWEST,
            -1: THREAD_PRIORITY_BELOW_NORMAL,
            0: THREAD_PRIORITY_NORMAL,
            1: THREAD_PRIORITY_ABOVE_NORMAL,
            2: THREAD_PRIORITY_HIGHEST
        }
        
        if priority_code in priority_map:
            thread_handle = kernel32.GetCurrentThread()
            success = kernel32.SetThreadPriority(thread_handle, priority_map[priority_code])
            return success
        return False
    except Exception as e:
        print(f"Ошибка установки приоритета потока: {e}")
        return False

def set_processor_affinity(mask):
    """Установить маску родственности процессоров"""
    try:
        current_process = psutil.Process()
        if mask == 0:
            cpu_count = os.cpu_count()
            affinity = list(range(cpu_count))
        else:
            affinity = []
            for i in range(32):
                if mask & (1 << i):
                    affinity.append(i)
        current_process.cpu_affinity(affinity)
        return True
    except Exception as e:
        print(f"Ошибка установки маски процессоров: {e}")
        return False

def set_process_priority(priority_code):
    """Установить приоритет процесса"""
    try:
        current_process = psutil.Process()
        priority_map = {
            0: psutil.IDLE_PRIORITY_CLASS,
            1: psutil.BELOW_NORMAL_PRIORITY_CLASS,
            2: psutil.NORMAL_PRIORITY_CLASS,
            3: psutil.ABOVE_NORMAL_PRIORITY_CLASS,
            4: psutil.HIGH_PRIORITY_CLASS,
            5: psutil.REALTIME_PRIORITY_CLASS
        }
        if priority_code in priority_map:
            current_process.nice(priority_map[priority_code])
            return True
        return False
    except Exception as e:
        print(f"Ошибка установки приоритета процесса: {e}")
        return False

def get_priority_name(priority_code):
    """Получить имя приоритета по коду"""
    names = {
        0: "IDLE_PRIORITY_CLASS",
        1: "BELOW_NORMAL_PRIORITY_CLASS", 
        2: "NORMAL_PRIORITY_CLASS",
        3: "ABOVE_NORMAL_PRIORITY_CLASS",
        4: "HIGH_PRIORITY_CLASS",
        5: "REALTIME_PRIORITY_CLASS"
    }
    return names.get(priority_code, f"UNKNOWN({priority_code})")

def get_thread_priority_name(priority_code):
    """Получить имя приоритета потока по коду"""
    names = {
        -2: "LOWEST",
        -1: "BELOW_NORMAL",
        0: "NORMAL", 
        1: "ABOVE_NORMAL",
        2: "HIGHEST"
    }
    return names.get(priority_code, f"UNKNOWN({priority_code})")

def thread_function(thread_num, priority_code, results, start_event):
    """Потоковая функция аналогичная Lab-05x"""
    start_event.wait()
    
    start_time = time.perf_counter()
    
    set_thread_priority(priority_code)
    
    total_iterations = 1000000
    report_interval = 100000
    
    print(f"[Thread{thread_num}] Запущен с приоритетом: {get_thread_priority_name(priority_code)}")
    print(f"[Thread{thread_num}] ID потока: {threading.get_ident()}")
    
    local_sum = 0.0
    iteration_count = 0
    
    for i in range(total_iterations + 1):
        iteration_count = i
        x = 0.0
        for j in range(100):
            x += (i * i * j) / 3.14159
            x -= (i + j * j) * 2.71828
            x *= 1.0001
        
        if i % 100 == 0:
            local_sum += x
        
        if i % report_interval == 0 and i > 0:
            current_time = time.perf_counter() - start_time
            speed = i / current_time if current_time > 0 else 0
            print(f"Thread{thread_num} | Iter: {i:7,} | "
                  f"Time: {current_time:6.2f}s | "
                  f"Speed: {speed:7,.0f} iter/s | "
                  f"Priority: {get_thread_priority_name(priority_code)}")
    
    end_time = time.perf_counter()
    elapsed_time = end_time - start_time
    
    results[thread_num] = {
        'iterations': iteration_count,
        'time': elapsed_time,
        'speed': iteration_count / elapsed_time,
        'sum': local_sum,
        'completed': True
    }
    
    print(f"\nПоток {thread_num} завершен.")
    print(f"Итераций: {iteration_count:,} | Время: {elapsed_time:.2f}с | "
          f"Скорость: {iteration_count/elapsed_time:,.0f} итер/сек")
    print("=" * 60)

def main():
    if len(sys.argv) != 5:
        print("Использование: python lab05c.py <P1> <P2> <P3> <P4>")
        print("  P1 - маска родственности процессоров:")
        print("       0 = все процессоры")
        print("       1 = только 1-й процессор")  
        print("  P2 - приоритет процесса (0-5):")
        print("       0=IDLE, 1=BELOW_NORMAL, 2=NORMAL, 3=ABOVE_NORMAL, 4=HIGH, 5=REALTIME")
        print("  P3 - приоритет 1-го потока (-2 to 2):")
        print("       -2=LOWEST, -1=BELOW_NORMAL, 0=NORMAL, 1=ABOVE_NORMAL, 2=HIGHEST")
        print("  P4 - приоритет 2-го потока (-2 to 2)")
        return

    try:
        P1 = int(sys.argv[1])
        P2 = int(sys.argv[2])
        P3 = int(sys.argv[3])
        P4 = int(sys.argv[4])
    
        print("=" * 70)
        print(f"Маска процессоров (P1): {P1}")
        print(f"Приоритет процесса (P2): {get_priority_name(P2)}")
        print(f"Приоритет потока 1 (P3): {get_thread_priority_name(P3)}")
        print(f"Приоритет потока 2 (P4): {get_thread_priority_name(P4)}")
        print("=" * 70)
        
        set_processor_affinity(P1)
        set_process_priority(P2)
        
        current_process = psutil.Process()
        print(f"PID процесса: {current_process.pid}")
        print("=" * 70)
        
        results = {}
        start_event = threading.Event()
        
        thread1 = threading.Thread(target=thread_function, args=(1, P3, results, start_event))
        thread2 = threading.Thread(target=thread_function, args=(2, P4, results, start_event))
        
        thread1.start()
        thread2.start()
        
        time.sleep(0.1)
        start_event.set()
        
        thread1.join()
        thread2.join()
        
        print("\n" + "=" * 70)
        print("Все потоки завершены.")
        print("Результаты выполнения:")
        
        if 1 in results and 2 in results:
            r1 = results[1]
            r2 = results[2]
            
            print(f"Поток 1 ({get_thread_priority_name(P3)}): {r1['time']:.2f}с, {r1['speed']:,.0f} итер/сек")
            print(f"Поток 2 ({get_thread_priority_name(P4)}): {r2['time']:.2f}с, {r2['speed']:,.0f} итер/сек")
            
            if r1['time'] > 0 and r2['time'] > 0:
                time_diff = abs(r1['time'] - r2['time'])
                speed_diff = abs(r1['speed'] - r2['speed']) / min(r1['speed'], r2['speed']) * 100
                print(f"Разница во времени: {time_diff:.2f}с ({speed_diff:.1f}%)")
        
        input("Нажмите ENTER для завершения программы...")
        
    except ValueError:
        print("Ошибка: все параметры должны быть целыми числами!")
    except Exception as e:
        print(f"Ошибка: {e}")

if __name__ == "__main__":
    main()