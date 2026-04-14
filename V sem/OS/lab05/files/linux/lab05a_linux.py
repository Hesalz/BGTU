import os
import threading
import multiprocessing

def get_process_affinity_mask(pid):
    try:
        with open(f'/proc/{pid}/status') as f:
            for line in f:
                if line.startswith('Cpus_allowed:'):
                    hex_mask = line.split()[1]
                    return bin(int(hex_mask, 16))[2:].zfill(32)
        return "Недоступно"
    except Exception:
        return "Недоступно"

def get_process_priority(pid):
    try:
        with open(f'/proc/{pid}/stat') as f:
            stats = f.read().split()
            return stats[17]
    except Exception:
        return "Недоступно"

def get_current_processor():
    try:
        return os.sched_getcpu()
    except Exception:
        return "Невозможно определить"

def main():
    pid = os.getpid()
    
    print(f"Идентификатор текущего процесса: {pid}")
    print(f"Идентификатор текущего потока: {threading.get_ident()}")
    print(f"Класс приоритетов текущего процесса: {get_process_priority(pid)}")
    print(f"Приоритет текущего потока: Thread ID: {threading.current_thread().ident}")
    print(f"Маска родственности процесса: {get_process_affinity_mask(pid)}")
    print(f"Доступных процессоров: {multiprocessing.cpu_count()}")
    print(f"Текущий процессор: {get_current_processor()}")

if __name__ == "__main__":
    main()