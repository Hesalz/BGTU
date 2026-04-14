import requests ## библиотека для отправки HTTP-запросов и получения данных с веб-страниц.
from bs4 import BeautifulSoup ##  инструмент для парсинга HTML-кода страниц.
import pandas as pd ## библиотека для работы с табличными данными (DataFrame).
import numpy as np ## библиотека для работы с массивами чисел.
import matplotlib.pyplot as plt ## модуль для построения графиков.
import seaborn as sns ## библиотека для визуализации данных на основе matplotlib.

##1
print("Задание 1,2:")
url = "https://stroi-instrum.ru/internet-magazin/folder/almaznye-diski"

response = requests.get(url)

soup = BeautifulSoup(response.content, 'html.parser')

product_items = soup.find_all('div', class_='product-item shop2-product-item')

products = []
for item in product_items:
    name_tag = item.find('p', class_='product-name')
    price_tag = item.find('div', class_='product-price')
    article_tag = item.find('p', class_='product-part')

    if name_tag and price_tag and article_tag:
        name = name_tag.text.strip()
        price = price_tag.text.strip()
        article = article_tag.text.strip()
        products.append({'Name': name, 'Price': price, 'Articul': article})

df = pd.DataFrame(products)

csv_file_path = 'products.csv'
df.to_csv(csv_file_path, index=False)

print(df)


##3
print("Задание 3:")

csv_file_path = 'products.csv'
df = pd.read_csv(csv_file_path)

df['Price'] = df['Price'].replace('[^\d.]', '', regex=True).str.rstrip('.')
df['Price'] = df['Price'].astype(float)

df_sorted = df.sort_values(by='Price', ascending=False)

print("Первые 5 значений отсортированного DataFrame:")
print(df_sorted.head())

description = df['Price'].describe()
print("\nОсновные метрики статистики для цен:")
print(description)

grouped_df = df.groupby('Name')['Price'].mean().reset_index()
print("\nСредняя цена для повторяющихся товаров:")
print(grouped_df)

plt.figure(figsize=(12, 6))

plt.subplot(1, 2, 1)
sns.histplot(df['Price'], bins=30, kde=True, color='skyblue', linewidth=2, edgecolor='black')
plt.title('Гистограмма распределения цен', fontsize=14)
plt.xlabel('Цена', fontsize=12)
plt.ylabel('Частота', fontsize=12)
plt.grid(True, linestyle='--', alpha=0.7)

plt.subplot(1, 2, 2)
sns.boxplot(x=df['Price'], color='lightgreen', linewidth=2)
plt.title('Диаграмма box-plot распределения цен', fontsize=14)
plt.xlabel('Цена', fontsize=12)
plt.grid(True, linestyle='--', alpha=0.7)

plt.tight_layout()
plt.show()