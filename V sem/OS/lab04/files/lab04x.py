import threading
import time
import random
import os
import sys

def Lab_04x(iterations, user_name="User-9a444f52"):
    """
    Функция выполняет N-ое количество итераций в цикле.
    """
    try:
        letters = [char for char in user_name if char.isalpha()]
        if not letters:
            letters = ['U', 's', 'e', 'r', '-', '9', 'a', '4', '4', '4', 'f', '5', '2']
    
        letters_count = len(letters)
        
        for i in range(iterations):
            current_letter = letters[i % letters_count]
            print(f"{os.getpid()} – {threading.get_ident()} - {i + 1} – {current_letter}")
            delay = random.uniform(0.3, 0.4)
            time.sleep(delay)
            
    except Exception as e:
        print(f"Ошибка в потоке {threading.get_ident()}: {e}")
    finally:
        print(f"Поток {threading.get_ident()} завершил работу")

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Использование: python lab04x.py <количество_итераций>")
        print("Пример: python lab04x.py 10")
        sys.exit(1)
    
    try:
        iterations = int(sys.argv[1])
        if iterations <= 0:
            print("Количество итераций должно быть положительным числом")
            sys.exit(1)
            
        print(f"=== Запуск Lab_04x с {iterations} итерациями ===")
        Lab_04x(iterations)
        print("Выполнение завершено!")
        
    except ValueError:
        print("Ошибка: количество итераций должно быть целым числом")
        sys.exit(1)