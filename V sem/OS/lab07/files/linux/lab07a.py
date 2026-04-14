import ctypes
import ctypes.util

def get_local_time():
    libc = ctypes.CDLL(ctypes.util.find_library('c'))
    
    class timeval(ctypes.Structure):
        _fields_ = [
            ("tv_sec", ctypes.c_long),
            ("tv_usec", ctypes.c_long)
        ]
    
    class timezone(ctypes.Structure):
        _fields_ = [
            ("tz_minuteswest", ctypes.c_int),
            ("tz_dsttime", ctypes.c_int)
        ]
    
    class tm(ctypes.Structure):
        _fields_ = [
            ("tm_sec", ctypes.c_int),
            ("tm_min", ctypes.c_int),
            ("tm_hour", ctypes.c_int),
            ("tm_mday", ctypes.c_int),
            ("tm_mon", ctypes.c_int),
            ("tm_year", ctypes.c_int),
            ("tm_wday", ctypes.c_int),
            ("tm_yday", ctypes.c_int),
            ("tm_isdst", ctypes.c_int)
        ]
    
    tv = timeval()
    tz = timezone()
    libc.gettimeofday(ctypes.byref(tv), ctypes.byref(tz))
    
    local_tm = tm()
    
    time_sec = ctypes.c_long(tv.tv_sec)
    libc.localtime_r(ctypes.byref(time_sec), ctypes.byref(local_tm))
    
    bias_minutes = tz.tz_minuteswest
    bias_hours = abs(bias_minutes) // 60
    bias_sign = '-' if bias_minutes > 0 else '+'
    
    year = local_tm.tm_year + 1900
    month = local_tm.tm_mon + 1
    day = local_tm.tm_mday
    hour = local_tm.tm_hour
    minute = local_tm.tm_min
    second = local_tm.tm_sec
    
    timezone_offset = f"{bias_sign}{bias_hours:02d}"
    
    result = f"{year:04d}-{month:02d}-{day:02d}T{hour:02d}:{minute:02d}:{second:02d}{timezone_offset}"
    
    return result

def main():
    try:
        current_time = get_local_time()
        print(f"Текущее локальное время: {current_time}")
    except Exception as e:
        print(f"Ошибка при получении времени: {e}")

if __name__ == "__main__":
    main()