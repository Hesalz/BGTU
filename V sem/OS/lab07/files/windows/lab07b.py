import ctypes
from ctypes import wintypes
import time

def get_current_time_ms():
    """Получает текущее время в миллисекундах с помощью WinAPI"""
    kernel32 = ctypes.windll.kernel32

    get_tick_count = kernel32.GetTickCount
    get_tick_count.restype = wintypes.DWORD
    
    return get_tick_count()

def main():
    start_time = get_current_time_ms()
    
    target_5s = 5000
    target_10s = 10000
    target_15s = 15000
    
    iteration_count = 0
    
    printed_5s = False
    printed_10s = False
    
    while True:
        iteration_count += 1
        
        current_time = get_current_time_ms()
        elapsed_time = current_time - start_time
        
        if not printed_5s and elapsed_time >= target_5s:
            print(f"Через 5 секунд: {iteration_count} итераций")
            printed_5s = True
        
        if not printed_10s and elapsed_time >= target_10s:
            print(f"Через 10 секунд: {iteration_count} итераций")
            printed_10s = True
        
        if elapsed_time >= target_15s:
            print(f"Итоговое значение: {iteration_count} итераций")
            break

if __name__ == "__main__":
    main()