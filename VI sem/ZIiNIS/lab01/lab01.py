import math

def gcd_two_numbers(a, b):
    a, b = abs(a), abs(b)
    while b:
        a, b = b, a % b
    return a

def gcd_three_numbers(a, b, c):
    return gcd_two_numbers(gcd_two_numbers(a, b), c)

def is_prime(num):
    if num < 2:
        return False
    if num == 2:
        return True
    if num % 2 == 0:
        return False
    
    for i in range(3, int(math.sqrt(num)) + 1, 2):
        if num % i == 0:
            return False
    return True

def find_primes_in_range(start, end):
    primes = []
    for num in range(start, end + 1):
        if is_prime(num):
            primes.append(num)
    return primes

def compare_with_prime_theorem(prime_count, n):
    if n < 2:
        return None
    
    n_ln_n = n / math.log(n)
    difference = prime_count - n_ln_n
    percentage = (prime_count / n_ln_n) * 100 if n_ln_n > 0 else 0
    
    return {
        'prime_count': prime_count,
        'n_ln_n': n_ln_n,
        'difference': difference,
        'percentage': percentage
    }

def main():
    
    while True:
        print("1. Вычислить НОД двух чисел")
        print("2. Вычислить НОД трех чисел")
        print("3. Найти простые числа в интервале [2, n]")
        print("4. Найти простые числа в интервале [m, n] (вариант 3)")
        print("5. Выход")
        
        choice = input("\nВведите номер операции (1-5): ").strip()
        
        if choice == '1':
            try:
                a = int(input("Введите первое число: "))
                b = int(input("Введите второе число: "))
                result = gcd_two_numbers(a, b)
                print(f"НОД({a}, {b}) = {result}")
            except ValueError:
                print("Ошибка: введите целые числа!")
        
        elif choice == '2':
            try:
                a = int(input("Введите первое число: "))
                b = int(input("Введите второе число: "))
                c = int(input("Введите третье число: "))
                result = gcd_three_numbers(a, b, c)
                print(f"НОД({a}, {b}, {c}) = {result}")
            except ValueError:
                print("Ошибка: введите целые числа!")
        
        elif choice == '3':
            try:
                n = int(input("Введите верхнюю границу n: "))
                if n < 2:
                    print("Интервал должен быть [2, n], где n >= 2")
                else:
                    primes = find_primes_in_range(2, n)
                    prime_count = len(primes)
                    
                    print(f"\nПростые числа в интервале [2, {n}]:")
                    print(f"Найдено {prime_count} чисел")
                    print(primes)
                    
                    comparison = compare_with_prime_theorem(prime_count, n)
                    
                    if comparison:
                        print(f"Соотношение π({n}) / (n/ln(n)) = {comparison['percentage']:.2f}%")
                    
            except ValueError:
                print("Ошибка: введите целое число!")
        
        elif choice == '4':
            print("\n" + "=" * 50)
            print(f"m = 367, n = 401")
            print("=" * 50)
            
            m, n = 367, 401
            
            if m > n:
                print("Ошибка: m должно быть меньше или равно n")
            elif m < 2:
                print("Ошибка: нижняя граница должна быть >= 2")
            else:
                primes = find_primes_in_range(m, n)
                
                print(f"\nПростые числа в интервале [{m}, {n}]:")
                if primes:
                    print(f"Найдено {len(primes)} простых чисел:")
                    print(primes)
                    
                    print(f"\nВсе найденные простые числа:")
                    for i, prime in enumerate(primes, 1):
                        print(f"{i}. {prime}")
                        
                    print("\n" + "=" * 60)
                    print("Примечание: Теорема о распределении простых чисел")
                    print("обычно применяется к интервалу [2, n], а не к [m, n]")
                    print("=" * 60)
                else:
                    print("Простых чисел не найдено")
        
        elif choice == '5':
            print("\nПрограмма завершена.")
            break
        
        else:
            print("Неверный выбор. Пожалуйста, введите число от 1 до 5.")
        
        input("\nНажмите Enter для продолжения...")

if __name__ == "__main__":
    main()