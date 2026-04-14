import ctypes
from ctypes import wintypes
import sys

def main():
    kernel32 = ctypes.windll.kernel32
    
    INFINITE = 0xFFFFFFFF
    WAIT_OBJECT_0 = 0x00000000
    
    timer_handle = kernel32.CreateWaitableTimerW(
        None,
        True, 
        None 
    )
    
    if not timer_handle:
        print("Ошибка создания таймера")
        return
    
    iteration_count = 0
    print_count = 0
    total_prints = 5
    
    try:
        while print_count < total_prints:
            due_time = wintypes.LARGE_INTEGER(-30000000)
            
            success = kernel32.SetWaitableTimer(
                timer_handle,
                ctypes.byref(due_time),
                0,
                None,
                None,
                False
            )
            
            if not success:
                print("Ошибка установки таймера")
                break
            
            timer_set = True
            while timer_set:
                iteration_count += 1
                
                wait_result = kernel32.WaitForSingleObject(timer_handle, 0)
                
                if wait_result == WAIT_OBJECT_0:
                    print_count += 1
                    print(f"Через {print_count * 3} секунд: {iteration_count} итераций")
                    timer_set = False
        
        print(f"Итоговое значение: {iteration_count} итераций")
        
    finally:
        kernel32.CloseHandle(timer_handle)

if __name__ == "__main__":
    main()