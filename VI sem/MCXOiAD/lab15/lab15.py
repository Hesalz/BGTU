# Списки
fruits = ['яблоко', 'банан', 'апельсин']
fruits.append('киви')
print(f'Список: {fruits[0]}, {fruits[-1]}, всего {len(fruits)}')

# Кортежи
rgb = (255, 128, 0)
r, g, b = rgb
print(f'Кортеж: R={r}, G={g}, B={b}')

# Словари
person = {'name': 'Анна', 'age': 28}
person['city'] = 'Москва'
for key, value in person.items():
    print(f'{key}: {value}')