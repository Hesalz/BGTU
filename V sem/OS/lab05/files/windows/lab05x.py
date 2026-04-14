import os
import threading
import time
import psutil

def get_process_priority_class(process):
    try:
        priority_classes = {
            psutil.REALTIME_PRIORITY_CLASS: "REALTIME_PRIORITY_CLASS",
            psutil.HIGH_PRIORITY_CLASS: "HIGH_PRIORITY_CLASS", 
            psutil.ABOVE_NORMAL_PRIORITY_CLASS: "ABOVE_NORMAL_PRIORITY_CLASS",
            psutil.NORMAL_PRIORITY_CLASS: "NORMAL_PRIORITY_CLASS",
            psutil.BELOW_NORMAL_PRIORITY_CLASS: "BELOW_NORMAL_PRIORITY_CLASS",
            psutil.IDLE_PRIORITY_CLASS: "IDLE_PRIORITY_CLASS"
        }
        return priority_classes.get(process.nice(), f"Unknown: {process.nice()}")
    except Exception:
        return "Недоступно"

def get_thread_priority():
    """Получить базовый приоритет потока через процесс"""
    try:
        current_process = psutil.Process()
        base_priority = current_process.nice()
        
        priority_map = {
            psutil.IDLE_PRIORITY_CLASS: 4, 
            psutil.BELOW_NORMAL_PRIORITY_CLASS: 6,
            psutil.NORMAL_PRIORITY_CLASS: 8,
            psutil.ABOVE_NORMAL_PRIORITY_CLASS: 10,
            psutil.HIGH_PRIORITY_CLASS: 13,
            psutil.REALTIME_PRIORITY_CLASS: 24
        }
        
        base_priority_value = priority_map.get(base_priority, 8)
        
        return f"Base Priority: {base_priority_value}"
    except Exception:
        return "Недоступно"

def get_current_processor():
    try:
        current_cpu = os.sched_getaffinity(0)
        return min(current_cpu) if current_cpu else 'Не определен'
    except Exception:
        return "Невозможно определить"

def main():
    start_time = time.perf_counter()
    
    current_process = psutil.Process()
    
    total_iterations = 1_000_000
    report_interval = 1000
    
    for i in range(total_iterations + 1):
        if i % report_interval == 0:
            print(f"\n--- Итерация {i:,} ---")
            print(f"Идентификатор процесса: {current_process.pid}")
            print(f"Идентификатор потока: {threading.get_ident()}")
            print(f"Класс приоритета процесса: {get_process_priority_class(current_process)}")
            print(f"Базовый приоритет потока: {get_thread_priority()}")
            print(f"Номер процессора: {get_current_processor()}")
            
            time.sleep(0.2)
    
    end_time = time.perf_counter()
    elapsed_time = end_time - start_time
    
    print("\n" + "=" * 70)
    print(f"Выполнение завершено")
    print(f"Общее время выполнения: {elapsed_time:.2f} секунд")
    print(f"Количество итераций: {total_iterations:,}")

if __name__ == "__main__":
    main()