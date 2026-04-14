import os
import sys
import subprocess
import time

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

def create_child_process(process_num, priority):
    child_code = f'''
import os
import time
import sys

def get_current_processor():
    try:
        return os.sched_getcpu()
    except:
        return "Недоступно"

def main():
    start_time = time.perf_counter()
    pid = os.getpid()
    
    try:
        if {priority} != 0:
            os.nice({priority})
    except Exception as e:
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
            current_nice = os.nice(0)
            print(f"--- Итерация {{i}} ---")
            print(f"Идентификатор процесса: {{pid}}")
            print(f"Уровень любезности: {{current_nice}}")
            print(f"Номер назначенного для выполнения процессора: {{get_current_processor()}}")
    
    end_time = time.perf_counter()
    elapsed_time = end_time - start_time
    
    print("Выполнение завершено")
    print(f"Всего выполнено итераций: {{iteration_count}}")
    print(f"Время выполнения процесса: {{elapsed_time:.2f}} секунд")
    
    # Записываем время выполнения в файл для основного процесса
    with open(f"time_{{pid}}.txt", "w") as f:
        f.write(str(elapsed_time))

if __name__ == "__main__":
    main()
'''

    temp_script = f"lab05b_temp_{process_num}.py"
    with open(temp_script, 'w', encoding='utf-8') as f:
        f.write(child_code)
    
    env = os.environ.copy()
    process = subprocess.Popen([sys.executable, temp_script], env=env)
    
    return process, temp_script

def main():
    if len(sys.argv) != 4:
        print("Использование: python3 lab05b.py <P1> <P2> <P3>")
        print("  P1 - маска родственности процессоров:")
        print("       0 = все процессоры")
        print("       1 = только 1-й процессор")  
        print("  P2 - приоритет 1-го процесса (-20 to 19)")
        print("  P3 - приоритет 2-го процесса (-20 to 19)")
        print("\nТЕСТЫ:")
        print("  python3 lab05b.py 0 0 0    # Все процессоры, 0, 0")
        print("  python3 lab05b.py 0 19 -20 # Все процессоры, 19, -20") 
        print("  python3 lab05b.py 1 19 -20 # 1 процессор, 19, -20")
        return

    try:
        start_time = time.perf_counter()
        
        P1 = int(sys.argv[1])
        P2 = int(sys.argv[2]) 
        P3 = int(sys.argv[3])
        
        print("=" * 70)
        print(f"Маска процессоров (P1): {P1}")
        print(f"Приоритет процесса 1 (P2): {P2}")
        print(f"Приоритет процесса 2 (P3): {P3}")
        
        set_processor_affinity(P1)
        
        proc1, script1 = create_child_process(1, P2)
        proc2, script2 = create_child_process(2, P3)
        
        proc1.wait()
        proc2.wait()
        
        # Собираем времена выполнения процессов
        process_times = []
        for pid in [proc1.pid, proc2.pid]:
            try:
                with open(f"time_{pid}.txt", "r") as f:
                    process_time = float(f.read().strip())
                    process_times.append(process_time)
                os.remove(f"time_{pid}.txt")
            except:
                process_times.append(0)
        
        end_time = time.perf_counter()
        total_time = end_time - start_time
        
        print("\n" + "=" * 70)
        print("РЕЗУЛЬТАТЫ:")
        print(f"Время выполнения процесса 1: {process_times[0]:.2f} секунд")
        print(f"Время выполнения процесса 2: {process_times[1]:.2f} секунд")
        print(f"Общее время выполнения: {total_time:.2f} секунд")
        
        try:
            os.remove(script1)
            os.remove(script2)
        except:
            pass

        input("Нажмите ENTER для завершения программы...")
        
    except ValueError:
        print("Ошибка: все параметры должны быть целыми числами!")
    except Exception as e:
        print(f"Ошибка: {e}")

if __name__ == "__main__":
    main()