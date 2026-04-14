import time
import sys
from functools import wraps

# Декоратор @uppercase
def uppercase(func):
    @wraps(func)
    def wrapper(*args, **kwargs):
        result = func(*args, **kwargs)
        if isinstance(result, str):
            return result.upper()
        return result
    return wrapper

# Декоратор @count_calls
def count_calls(func):
    @wraps(func)
    def wrapper(*args, **kwargs):
        wrapper.call_count += 1
        return func(*args, **kwargs)
    wrapper.call_count = 0
    return wrapper

# Декоратор @html_tag(tag)
def html_tag(tag):
    def decorator(func):
        @wraps(func)
        def wrapper(*args, **kwargs):
            result = func(*args, **kwargs)
            return f"<{tag}>{result}</{tag}>"
        return wrapper
    return decorator

# Декоратор для замера времени выполнения
def timer(func):
    def wrapper(*args, **kwargs):
        start_time = time.time()
        result = func(*args, **kwargs)
        end_time = time.time()
        print(f"Функция {func.__name__} выполнилась за {end_time - start_time:.4f} секунд")
        return result
    return wrapper

# Декоратор для логирования вызовов функции в файл
def log(func):
    def wrapper(*args, **kwargs):
        result = func(*args, **kwargs)
        with open('function_logs.txt', 'a') as f:
            f.write(f"Функция: {func.__name__}\n")
            f.write(f"Аргументы: args={args}, kwargs={kwargs}\n")
            f.write(f"Результат: {result}\n")
            f.write("-" * 50 + "\n")
        return result
    return wrapper

# Декоратор для измерения использования памяти
def memory_usage(func):
    def wrapper(*args, **kwargs):
        before = sys.getsizeof(args) + sys.getsizeof(kwargs)
        result = func(*args, **kwargs)
        after = sys.getsizeof(result)
        print(f"Функция {func.__name__} использовала {after - before} байт памяти")
        return result
    return wrapper

@count_calls
def greet(name):
    print(f"Hello, {name}!")

@html_tag("div")
def get_text():
    return "Hello, World!"

@uppercase
def shout(text):
    return text

greet("Tom")
greet("Denis")
print(f"Функция greet вызвана {greet.call_count} раз(а).")
print(get_text())
print(shout("Hello мир"))

@timer
@log
@memory_usage
def some_function(n):
    return sum(i * i for i in range(n))
some_function(10**6)
