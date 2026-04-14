import datetime
import ctypes
from ctypes import wintypes

def get_local_time():
    kernel32 = ctypes.windll.kernel32
    
    class SYSTEMTIME(ctypes.Structure):
        _fields_ = [
            ("wYear", wintypes.WORD),
            ("wMonth", wintypes.WORD),
            ("wDayOfWeek", wintypes.WORD),
            ("wDay", wintypes.WORD),
            ("wHour", wintypes.WORD),
            ("wMinute", wintypes.WORD),
            ("wSecond", wintypes.WORD),
            ("wMilliseconds", wintypes.WORD)
        ]
    
    local_time = SYSTEMTIME()
    kernel32.GetLocalTime(ctypes.byref(local_time))
    
    utc_time = SYSTEMTIME()
    kernel32.GetSystemTime(ctypes.byref(utc_time))
    
    offset_hours, offset_sign = calculate_timezone_offset(local_time, utc_time)
    
    year = local_time.wYear
    month = local_time.wMonth
    day = local_time.wDay
    hour = local_time.wHour
    minute = local_time.wMinute
    second = local_time.wSecond
    
    timezone_offset = f"{offset_sign}{offset_hours:02d}"
    
    result = f"{year:04d}-{month:02d}-{day:02d}T{hour:02d}:{minute:02d}:{second:02d}{timezone_offset}"
    
    return result

def calculate_timezone_offset(local_st, utc_st):
    """
    Вычисляет смещение часового пояса на основе сравнения локального и UTC времени
    """
    local_dt = datetime.datetime(
        local_st.wYear,
        local_st.wMonth,
        local_st.wDay,
        local_st.wHour,
        local_st.wMinute,
        local_st.wSecond
    )
    
    utc_dt = datetime.datetime(
        utc_st.wYear,
        utc_st.wMonth,
        utc_st.wDay,
        utc_st.wHour,
        utc_st.wMinute,
        utc_st.wSecond
    )
    
    time_difference = local_dt - utc_dt
    total_seconds = time_difference.total_seconds()
    
    if total_seconds >= 0:
        sign = '+'
        offset_hours = int(total_seconds // 3600)
    else:
        sign = '-'
        offset_hours = int(abs(total_seconds) // 3600)
    
    return offset_hours, sign

def main():
    try:
        current_time = get_local_time()
        print(f"Текущее локальное время: {current_time}")
    except Exception as e:
        print(f"Ошибка при получении времени: {e}")

if __name__ == "__main__":
    main()