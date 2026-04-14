import ctypes
from ctypes import wintypes

def get_current_time_ms():
    """Получает текущее время в миллисекундах с помощью WinAPI"""
    kernel32 = ctypes.windll.kernel32
    get_tick_count = kernel32.GetTickCount
    get_tick_count.restype = wintypes.DWORD
    return get_tick_count()

def is_prime(n):
    """Проверяет, является ли число простым"""
    if n < 2:
        return False
    if n == 2:
        return True
    if n % 2 == 0:
        return False
    
    i = 3
    while i * i <= n:
        if n % i == 0:
            return False
        i += 2
    return True

def main():
    start_time = get_current_time_ms()
    
    print("Вычисление простых чисел:")
    print("-" * 30)
    
    count = 0
    number = 2
    max_primes = 100
    
    try:
        while count < max_primes:
            if is_prime(number):
                count += 1
                print(f"{count:3d}: {number}")
            number += 1
            
    except KeyboardInterrupt:
        print("\nПрервано пользователем")
    
    end_time = get_current_time_ms()
    elapsed_time = (end_time - start_time) / 1000.0
    
    print("-" * 30)
    print(f"Вычислено простых чисел: {count}")
    print(f"Отработанное время: {elapsed_time:.3f} секунд")

if __name__ == "__main__":
    main()