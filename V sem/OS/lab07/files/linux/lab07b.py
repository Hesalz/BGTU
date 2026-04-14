import ctypes
import ctypes.util

def get_process_time_ms():
    """Получает процессорное время процесса в миллисекундах с помощью POSIX функций"""
    libc = ctypes.CDLL(ctypes.util.find_library('c'))
    
    class tms(ctypes.Structure):
        _fields_ = [
            ("tms_utime", ctypes.c_long),
            ("tms_stime", ctypes.c_long),
            ("tms_cutime", ctypes.c_long),
            ("tms_cstime", ctypes.c_long)
        ]
    
    clocks_per_sec = libc.sysconf(84)
    
    times_struct = tms()
    libc.times(ctypes.byref(times_struct))
    
    total_ticks = times_struct.tms_utime + times_struct.tms_stime
    
    total_ms = (total_ticks * 1000) // clocks_per_sec
    
    return total_ms

def main():
    start_time = get_process_time_ms()
    
    target_5s = 5000
    target_10s = 10000
    target_15s = 15000
    
    iteration_count = 0
    
    printed_5s = False
    printed_10s = False
    
    while True:
        iteration_count += 1
        
        current_time = get_process_time_ms()
        elapsed_time = current_time - start_time
        
        if not printed_5s and elapsed_time >= target_5s:
            print(f"Через 5 секунд: {iteration_count} итераций")
            printed_5s = True
        
        if not printed_10s and elapsed_time >= target_10s:
            print(f"Через 10 секунд: {iteration_count} итераций")
            printed_10s = True
        
        if elapsed_time >= target_15s:
            real_elapsed_time = elapsed_time / 1000.0
            print(f"Итоговое значение: {iteration_count} итераций")
            print(f"Реальное время работы: {real_elapsed_time:.3f} секунд")
            break

if __name__ == "__main__":
    main()