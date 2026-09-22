import numpy as np
import sympy as sp
import pandas as pd
import matplotlib.pyplot as plt

print("2. SYMPY")

x = sp.symbols('x')
f = x**2 + 1
derivative = sp.diff(f, x)
print("\n2.1. Производная функции f(x) = x^2 + 1:")
print("f'(x) =", derivative)

integral = sp.integrate(f, (x, 0, 1))
print("\n2.2. Интеграл функции f(x) = x^2 + 1 на [0, 1]:")
print("Результат =", integral)


limit_function = 1 / x**2 + 1
limit = sp.limit(limit_function, x, sp.oo)
print("\n2.3. Предел функции f(x) = 1/x^2 + 1 при x -> infinity:")
print("Предел =", limit)


print("\n" + "-" * 50)
print("3. NUMPY")

np.random.seed(42)
array_1d = np.random.randint(1, 11, 20)
print("\n3.1. Одномерный массив из 20 случайных целых чисел:")
print(array_1d)

array_2d = array_1d.reshape(4, 5)
print("\n3.2. Двумерный массив размером 4x5:")
print(array_2d)

array_first, array_second = np.array_split(array_2d, 2)
print("\n3.3. Первый массив:")
print(array_first)
print("\nВторой массив:")
print(array_second)

search_value = 6
found_elements = array_first[array_first == search_value]
print(f"\n3.4. Элементы первого массива, равные {search_value}:")
print(found_elements)

count = np.sum(array_first == search_value)
print(f"\n3.5. Количество найденных элементов: {count}")

minimum = np.min(array_second)
maximum = np.max(array_second)
mean = np.mean(array_second)

print("\n3.6. Статистические характеристики второго массива:")
print("Минимум:", minimum)
print("Максимум:", maximum)
print("Среднее:", mean)

print("\n" + "-" * 50)
print("4. PANDAS")

series_numpy = pd.Series(array_1d)
print("4.2. Series из массива NumPy:")
print(series_numpy)

data_dict = {
    'Январь': 10,
    'Февраль': 20,
    'Март': 30,
    'Апрель': 40
}
series_dict = pd.Series(data_dict)
print("\nSeries из словаря:")
print(series_dict)

print("\n4.3. Математические операции:")
print("\nИсходная Series:")
print(series_dict)
print("\nСложение 10:")
print(series_dict + 10)
print("\nУмножение на 2:")
print(series_dict * 2)
print("\nВозведение в квадрат:")
print(series_dict ** 2)
print("\nСреднее значение:")
print(series_dict.mean())
print("\nМинимальное значение:")
print(series_dict.min())
print("\nМаксимальное значение:")
print(series_dict.max())


data_numpy = np.array([
    [1, 2, 3],
    [4, 5, 6],
    [7, 8, 9]
])
df_numpy = pd.DataFrame(
    data_numpy,
    columns=['A', 'B', 'C']
)
print("\n4.4. DataFrame из массива NumPy:")
print(df_numpy)

data_dict_df = {
    'Имя': ['Анна', 'Иван', 'Петр'],
    'Возраст': [20, 21, 22],
    'Оценка': [90, 85, 95]
}
df_dict = pd.DataFrame(data_dict_df)
print("\nDataFrame из словаря:")
print(df_dict)


name_series = pd.Series(
    ['Анна', 'Иван', 'Петр'],
    name='Имя'
)
age_series = pd.Series(
    [20, 21, 22],
    name='Возраст'
)
grade_series = pd.Series(
    [90, 85, 95],
    name='Оценка'
)
df_series = pd.DataFrame({
    'Имя': name_series,
    'Возраст': age_series,
    'Оценка': grade_series
})
print("\nDataFrame из объектов Series:")
print(df_series)

print("\n" + "-" * 50)
print("5. MATPLOTLIB")

x_values = np.linspace(-5, 5, 200)
y_values = x_values**2 + 1
plt.figure(figsize=(8, 5))
plt.plot(x_values, y_values)
plt.title('График функции f(x) = x² + 1')
plt.xlabel('x')
plt.ylabel('f(x)')
plt.grid(True)
plt.show()

from mpl_toolkits.mplot3d import Axes3D
x_surface = np.linspace(-5, 5, 100)
y_surface = np.linspace(-5, 5, 100)
X, Y = np.meshgrid(x_surface, y_surface)
Z = X**2 + 2 * Y**2 + 1
fig = plt.figure(figsize=(9, 7))
ax = fig.add_subplot(111, projection='3d')
ax.plot_surface(X, Y, Z)
ax.set_title('Поверхность f(x, y) = x² + 2y² + 1')
ax.set_xlabel('x')
ax.set_ylabel('y')
ax.set_zlabel('f(x, y)')
plt.show()

categories = ['A', 'B', 'C', 'D']
values = [10, 25, 15, 30]
plt.figure(figsize=(8, 5))
plt.bar(categories, values)
plt.title('Столбчатая диаграмма')
plt.xlabel('Категории')
plt.ylabel('Значения')
plt.show()

plt.figure(figsize=(7, 7))
plt.pie(
    values,
    labels=categories,
    autopct='%1.1f%%'
)
plt.title('Круговая диаграмма')
plt.show()

plt.figure(figsize=(8, 5))
plt.plot(categories, values, marker='o')
plt.title('Линейная диаграмма')
plt.xlabel('Категории')
plt.ylabel('Значения')
plt.grid(True)
plt.show()

random_data = np.random.normal(50, 10, 1000)
plt.figure(figsize=(8, 5))
plt.hist(random_data, bins=20)
plt.title('Гистограмма')
plt.xlabel('Значение')
plt.ylabel('Частота')
plt.show()


import scipy
import IPython
import sklearn
import mglearn