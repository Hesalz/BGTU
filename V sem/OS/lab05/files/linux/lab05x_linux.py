import os
import threading
import time

def main():
    start_time = time.perf_counter()
    
    pid = os.getpid()
    total_iterations = 1000000
    report_interval = 1000
    
    for i in range(total_iterations + 1):
        if i % report_interval == 0:
            print(f"--- Итерация {i} ---")
            print(f"Идентификатор процесса: {pid}")
            print(f"Идентификатор потока: {threading.get_ident()}")
            
            try:
                nice = os.nice(0)
                print(f"Уровень любезности: {nice}")
            except:
                print(f"Уровень любезности: Недоступно")
            
            try:
                cpu = os.sched_getcpu()
                print(f"Номер назначенного для выполнения процессора: {cpu}")
            except:
                print(f"Номер назначенного для выполнения процессора: Недоступно")
            
            time.sleep(0.2)
    
    end_time = time.perf_counter()
    elapsed_time = end_time - start_time
    
    print(f"Выполнение завершено")
    print(f"Время прошедшее с момента запуска: {elapsed_time:.2f} секунд")

if __name__ == "__main__":
    main()