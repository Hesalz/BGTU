import ctypes
import ctypes.util
import signal

def main():
    libc = ctypes.CDLL(ctypes.util.find_library('c'))
    
    class timeval(ctypes.Structure):
        _fields_ = [
            ("tv_sec", ctypes.c_long),
            ("tv_usec", ctypes.c_long)
        ]
    
    class itimerval(ctypes.Structure):
        _fields_ = [
            ("it_interval", timeval),
            ("it_value", timeval)
        ]
    
    global iteration_count, print_count, timer_expired
    iteration_count = 0
    print_count = 0
    timer_expired = False
    
    def timer_handler(signum, frame):
        global print_count, timer_expired
        print_count += 1
        timer_expired = True
    
    signal.signal(signal.SIGALRM, timer_handler)
    
    timer = itimerval()
    timer.it_interval.tv_sec = 3
    timer.it_interval.tv_usec = 0
    timer.it_value.tv_sec = 3
    timer.it_value.tv_usec = 0
    
    libc.setitimer(0, ctypes.byref(timer), None)
    
    try:
        while print_count < 5:
            timer_expired = False
            
            while not timer_expired:
                iteration_count += 1
            
            print(f"Через {print_count * 3} секунд: {iteration_count} итераций")
            
            if print_count < 5:
                timer.it_value.tv_sec = 3
                timer.it_value.tv_usec = 0
                libc.setitimer(0, ctypes.byref(timer), None)
        
        print(f"Итоговое значение: {iteration_count} итераций")
        
    except KeyboardInterrupt:
        timer.it_value.tv_sec = 0
        timer.it_value.tv_usec = 0
        libc.setitimer(0, ctypes.byref(timer), None)

if __name__ == "__main__":
    main()