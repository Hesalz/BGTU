import os
import sys
import subprocess
import psutil

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

def create_child_process(process_num, priority_code):
    """Создать дочерний процесс с заданным приоритетом"""
    child_code = f'''
import os
import time
import psutil

def get_process_priority_class(process):
    """Получить класс приоритетов процесса"""
    try:
        priority_classes = {{
            psutil.REALTIME_PRIORITY_CLASS: "REALTIME",
            psutil.HIGH_PRIORITY_CLASS: "HIGH",
            psutil.ABOVE_NORMAL_PRIORITY_CLASS: "ABOVE_NORMAL",
            psutil.NORMAL_PRIORITY_CLASS: "NORMAL",
            psutil.BELOW_NORMAL_PRIORITY_CLASS: "BELOW_NORMAL",
            psutil.IDLE_PRIORITY_CLASS: "IDLE"
        }}
        return priority_classes.get(process.nice(), f"Unknown: {{process.nice()}}")
    except Exception:
        return "Недоступно"

def set_process_priority(priority_code):
    """Установить приоритет процесса"""
    try:
        current_process = psutil.Process()
        priority_map = {{
            0: psutil.IDLE_PRIORITY_CLASS,
            1: psutil.BELOW_NORMAL_PRIORITY_CLASS,
            2: psutil.NORMAL_PRIORITY_CLASS,
            3: psutil.ABOVE_NORMAL_PRIORITY_CLASS,
            4: psutil.HIGH_PRIORITY_CLASS,
            5: psutil.REALTIME_PRIORITY_CLASS
        }}
        if priority_code in priority_map:
            current_process.nice(priority_map[priority_code])
            return True
        return False
    except Exception as e:
        print(f"Ошибка установки приоритета: {{e}}")
        return False

def main():
    start_time = time.perf_counter()
    current_process = psutil.Process()
    
    priority_code = {priority_code}
    set_process_priority(priority_code)
    
    total_iterations = 1000000
    report_interval = 100000
    
    print(f"ДОЧЕРНИЙ ПРОЦЕСС {{ {process_num} }}")
    print(f"PID: {{current_process.pid}}")
    print(f"Установлен приоритет: {{get_priority_name({priority_code})}}")
    print(f"Текущий приоритет: {{get_process_priority_class(current_process)}}")
    print("=" * 70)
    
    results = []
    
    for i in range(total_iterations + 1):
        x = 0.0
        for j in range(50):
            x += (i * i * j) / 3.14159
            x -= (i + j * j) * 2.71828
            x *= 1.0001
        
        if i % 100 == 0:
            results.append(x)
        
        if i % report_interval == 0 and i > 0:
            current_time = time.perf_counter() - start_time
            speed = i / current_time if current_time > 0 else 0
            print(f"Proc{{ {process_num} }} | Iter: {{i:7,}} | "
                  f"Time: {{current_time:6.2f}}s | "
                  f"Speed: {{speed:7,.0f}} iter/s | "
                  f"Priority: {{get_process_priority_class(current_process)}}")
    
    end_time = time.perf_counter()
    elapsed_time = end_time - start_time
    
    print("=" * 70)
    print(f"Процесс {{ {process_num} }} завершен")
    print(f"Итераций: {{i:,}} | Время: {{elapsed_time:.2f}}с | "
          f"Скорость: {{i/elapsed_time:,.0f}} итер/сек")
    print(f"Контрольная сумма: {{sum(results):.2f}}")
    input("Нажмите ENTER для закрытия этого окна...")

def get_priority_name(priority_code):
    """Получить имя приоритета по коду"""
    names = {{
        0: "IDLE",
        1: "BELOW_NORMAL", 
        2: "NORMAL",
        3: "ABOVE_NORMAL",
        4: "HIGH",
        5: "REALTIME"
    }}
    return names.get(priority_code, f"UNKNOWN({{priority_code}})")

if __name__ == "__main__":
    main()
'''

    temp_script = f"lab05x_temp_{process_num}.py"
    with open(temp_script, 'w', encoding='utf-8') as f:
        f.write(child_code)
    
    process = subprocess.Popen(
        [sys.executable, temp_script],
        creationflags=subprocess.CREATE_NEW_CONSOLE
    )
    
    return process, temp_script

def main():
    if len(sys.argv) != 4:
        print("Использование: python lab05b.py <P1> <P2> <P3>")
        print("  P1 - маска родственности процессоров:")
        print("       0 = все процессоры")
        print("       1 = только 1-й процессор")  
        print("  P2 - приоритет 1-го процесса (0-5):")
        print("       2=NORMAL, 1=BELOW_NORMAL, 4=HIGH")
        print("  P3 - приоритет 2-го процесса (0-5)")
        return

    try:
        P1 = int(sys.argv[1])
        P2 = int(sys.argv[2]) 
        P3 = int(sys.argv[3])
        
        print("=" * 70)
        print(f"Маска процессоров (P1): {P1}")
        print(f"Приоритет процесса 1 (P2): {get_priority_name(P2)}")
        print(f"Приоритет процесса 2 (P3): {get_priority_name(P3)}")
        
        set_processor_affinity(P1)
        
        proc1, script1 = create_child_process(1, P2)
        proc2, script2 = create_child_process(2, P3)
        
        proc1.wait()
        proc2.wait()
        
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