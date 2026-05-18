import sympy as sp
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

# 2. SymPy
x = sp.Symbol('x')
f = x**2 + 1
print(f'2.1 Производная: {sp.diff(f, x)}')
print(f'2.2 Интеграл [0,1]: {sp.integrate(f, (x, 0, 1))}')
print(f'2.3 Предел x→∞: {sp.limit(1/(x**2) + 1, x, sp.oo)}')

# 3. NumPy
np.random.seed(42)
arr1d = np.random.randint(0, 20, 20)
arr2d = arr1d.reshape(4, 5)
arr_a, arr_b = np.vsplit(arr2d, 2)
print(f'3.1 Одномерный: {arr1d}')
print(f'3.2 Двумерный 4x5:\n{arr2d}')
print(f'3.3 Первая половина:\n{arr_a}')
print(f'3.3 Вторая половина:\n{arr_b}')
mask = arr_a == 6
print(f'3.4 Значения =6: {arr_a[mask]}')
print(f'3.5 Количество =6: {np.sum(mask)}')
print(f'3.6 Мин: {arr_b.min()}, Макс: {arr_b.max()}, Сред: {arr_b.mean():.2f}')

# 4. Pandas
series_np = pd.Series(arr1d, name='Случайные')
series_dict = pd.Series({'a': 10, 'b': 20, 'c': 30})
print(f'4.2 Series из словаря:\n{series_dict}')
print(f'4.3 Матем. операции (Series*2):\n{series_np * 2}')
print(f'4.3 Матем. операции (Series+Series):\n{series_np + series_np}')
df_np = pd.DataFrame(arr2d, columns=['A','B','C','D','E'])
df_dict = pd.DataFrame({'Name': ['Иван','Мария','Петр'], 'Age': [25,30,35]})
df_from_series = pd.DataFrame({'Original': series_np, 'Doubled': series_np*2})
print(f'4.4 DataFrame из NumPy:\n{df_np}')
print(f'4.4 DataFrame из словаря:\n{df_dict}')
print(f'4.4 DataFrame из Series:\n{df_from_series.head()}')

# 5. Matplotlib
x_vals = np.linspace(-3, 3, 100)
plt.figure()
plt.plot(x_vals, x_vals**2 + 1)
plt.title('f(x)=x²+1')
plt.grid()
plt.show()

from mpl_toolkits.mplot3d import Axes3D
X, Y = np.meshgrid(np.linspace(-2,2,30), np.linspace(-2,2,30))
Z = X**2 + 2*Y**2 + 1
fig = plt.figure()
ax = fig.add_subplot(111, projection='3d')
ax.plot_surface(X, Y, Z)
ax.set_title('f(x,y)=x²+2y²+1')
plt.show()

labels = ['A', 'B', 'C', 'D']
values = [23, 45, 12, 34]
plt.figure()
plt.bar(labels, values, color=['red','blue','green','orange'])
plt.title('Столбчатая диаграмма')
plt.xlabel('Категории')
plt.ylabel('Значения')
plt.show()

plt.figure()
plt.pie(values, labels=labels, autopct='%1.1f%%')
plt.title('Круговая диаграмма')
plt.show()

plt.figure()
plt.hist(arr1d, bins=10, edgecolor='black')
plt.title('Гистограмма случайных чисел')
plt.xlabel('Значения')
plt.ylabel('Частота')
plt.show()