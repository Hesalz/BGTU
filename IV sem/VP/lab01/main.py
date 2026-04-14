import random

# 1
first_name = input("Введите ваше имя: ")
last_name = input("Введите вашу фамилию: ")

if isinstance(first_name, str) and isinstance(last_name, str):
    full_name = f"{first_name.capitalize()} {last_name.capitalize()}"
    print("Имя и фамилия с заглавных букв:", full_name)

    initials = f"{first_name[0].upper()}.{last_name[0].upper()}."
    print("Инициалы:", initials)
else:
    print("Введите только буквы.")

# 2
elements = [x**2 for x in range(10, 20)]
print("Список квадратов чисел:", elements)

sum_elements = sum(elements)
print("Сумма элементов:", sum_elements)

odd= [x for x in elements if x % 2 != 0]
print("Список без четных элементов:", odd)

print("Количество оставшихся элементов:", len(odd))

# 3
n = int(input("Введите количество элементов списка: "))
A = [random.randint(1, 100) for _ in range(n)]
print("Список A:", A)

B = []
total = 0
for num in A:
    total += num
    B.append(total)

print("Список B:", B)