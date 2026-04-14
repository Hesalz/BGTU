import time
import locale

try:
    locale.setlocale(locale.LC_ALL, 'ru_RU.UTF-8')
except:
    try:
        locale.setlocale(locale.LC_ALL, 'Russian_Russia.1251')
    except:
        pass

def bwt_transform(s):
    """Прямое преобразование Барроуза-Уилера с правильной сортировкой кириллицы"""
    start_time = time.perf_counter()
    
    if not s:
        return "", 0, 0, [], []
    
    rotations = []
    n = len(s)
    for i in range(n):
        rotation = s[i:] + s[:i]
        rotations.append(rotation)
    
    try:
        sorted_rotations = sorted(rotations, key=locale.strxfrm)
    except:
        sorted_rotations = sorted(rotations)
    
    last_column = ''.join(rotation[-1] for rotation in sorted_rotations)
    original_index = sorted_rotations.index(s)
    
    end_time = time.perf_counter()
    transform_time = (end_time - start_time) * 1000 
    
    return last_column, original_index, transform_time, rotations, sorted_rotations

def bwt_inverse(last_column, original_index):
    """Обратное преобразование Барроуза-Уилера с правильной сортировкой кириллицы"""
    start_time = time.perf_counter()
    
    n = len(last_column)
    table = [''] * n
    
    reconstruction_steps = []
    
    for i in range(n):
        table = [last_column[j] + table[j] for j in range(n)]
        
        try:
            table.sort(key=locale.strxfrm)
        except:
            table.sort()
        
        reconstruction_steps.append(table.copy())
    
    original_string = table[original_index]
    
    end_time = time.perf_counter()
    inverse_time = (end_time - start_time) * 1000 
    
    return original_string, inverse_time, reconstruction_steps

def text_to_binary(text):
    """Преобразование текста в бинарную последовательность ASCII"""
    binary_result = ""
    for char in text:
        ascii_code = ord(char)
        binary_char = format(ascii_code, '08b')
        binary_result += binary_char
    return binary_result

def print_reconstruction_steps(reconstruction_steps, n):
    """Печать шагов восстановления как в примере"""
    for step in range(n):
        current_step = reconstruction_steps[step]
        for j in range(n):
            print(current_step[j])
        if step < n - 1:
            print()

def main():
    data_blocks = {
        "Глеб": "Глеб",
        "Бабашинский": "Бабашинский", 
        "времяпрепровождение": "времяпрепровождение"
    }
    
    lengths = []
    direct_times = []
    inverse_times = []
    
    for name, text in data_blocks.items():
        print('=' * 60)
        print(f'РАБОТА СО СТРОКОЙ: {name}')
        print('=' * 60)
        print(f"M = {text} | k = {len(text)}")
        
        last_col, orig_idx, time_direct, rotations, sorted_rotations = bwt_transform(text)
        
        print("\n1. Формируем таблицу W1 ({}x{}) - все циклические сдвиги:".format(len(text), len(text)))
        for i, rotation in enumerate(rotations):
            print(f"{i+1:2}. {rotation}")
        
        print("\n2. Сортируем строки таблицы W1 и получаем W2:")
        for i, rotation in enumerate(sorted_rotations):
            print(f"{i+1:2}. {rotation}")
        
        print("\n3. Извлекаем последний столбец Mk и вычисляем позицию исходной строки z:")
        print(f"Mk = {last_col}")
        print(f"z = {orig_idx}")
        print(f"Результат кодирования: {last_col}{orig_idx}")
        
        print("\n" + "=" * 40)
        print("ОБРАТНОЕ ПРЕОБРАЗОВАНИЕ")
        print("=" * 40)
        
        original, time_inverse, reconstruction_steps = bwt_inverse(last_col, orig_idx)
        
        print(f"4. Восстанавливаем таблицу по Mk = {last_col}")
        n = len(last_col)
        print_reconstruction_steps(reconstruction_steps, n)
        
        print(f"Имея z = {orig_idx}, восстанавливаем исходное сообщение: M = '{original}'")
        print(f"Проверка: исходная строка '{text}', восстановленная '{original}' - {'СОВПАДАЕТ' if text == original else 'НЕ СОВПАДАЕТ'}")
        print(f"\nВремя прямого преобразования: {time_direct:.6f} мс")
        print(f"Время обратного преобразования: {time_inverse:.6f} мс")
        
        lengths.append(len(text))
        direct_times.append(time_direct)
        inverse_times.append(time_inverse)
        print()
    
    print('=' * 60)
    variant_text = "времяпрепровождение"
    first_three = variant_text[:3]
    
    print(f"3 первых символа слова по варианту: '{first_three}'")
    binary_representation = text_to_binary(first_three)
    print(f"Бинарное представление по ASCII: {binary_representation}")
    
    print('=' * 60)
    print(f'РАБОТА С БИНАРНОЙ ПОСЛЕДОВАТЕЛЬНОСТЬЮ')
    print('=' * 60)
    print(f"M = {binary_representation} | k = {len(binary_representation)}")
    
    last_col_bin, orig_idx_bin, time_direct_bin, rotations_bin, sorted_rotations_bin = bwt_transform(binary_representation)
    
    print("\n1. Формируем таблицу W1 ({}x{}) - все циклические сдвиги:".format(len(binary_representation), len(binary_representation)))
    for i, rotation in enumerate(rotations_bin):
        print(f"{i+1:2}. {rotation}")
    
    print("\n2. Сортируем строки таблицы W1 и получаем W2:")
    for i, rotation in enumerate(sorted_rotations_bin):
        print(f"{i+1:2}. {rotation}")
    
    print("\n3. Извлекаем последний столбец Mk и вычисляем позицию исходной строки z:")
    print(f"Mk = {last_col_bin}")
    print(f"z = {orig_idx_bin}")
    print(f"Результат кодирования: {last_col_bin}{orig_idx_bin}")
    
    print("\n" + "=" * 40)
    print("ОБРАТНОЕ ПРЕОБРАЗОВАНИЕ")
    print("=" * 40)
    
    original_bin, time_inverse_bin, reconstruction_steps_bin = bwt_inverse(last_col_bin, orig_idx_bin)
    
    print(f"4. Восстанавливаем таблицу по Mk = {last_col_bin}")
    n_bin = len(last_col_bin)
    print_reconstruction_steps(reconstruction_steps_bin, n_bin)
    
    print(f"Имея z = {orig_idx_bin}, восстанавливаем исходное сообщение: M = '{original_bin}'")
    print(f"Проверка: исходная бинарная строка '{binary_representation}', восстановленная '{original_bin}' - {'СОВПАДАЕТ' if binary_representation == original_bin else 'НЕ СОВПАДАЕТ'}")
    print(f"\nВремя прямого преобразования: {time_direct_bin:.6f} мс")
    print(f"Время обратного преобразования: {time_inverse_bin:.6f} мс")
    
    lengths.append(len(binary_representation))
    direct_times.append(time_direct_bin)
    inverse_times.append(time_inverse_bin)
    
    print('\n' + '=' * 60)
    print('СРАВНИТЕЛЬНЫЙ АНАЛИЗ ДЛИТЕЛЬНОСТИ ПРЕОБРАЗОВАНИЙ')
    print('=' * 60)
    print(f"{'Блок данных':<25} {'Длина':<8} {'Время прямого (мс)':<18} {'Время обратного (мс)':<18}")
    print('-' * 70)
    
    blocks = ["Глеб", "Бабашинский", "времяпрепровождение", "Бинарная последовательность"]
    
    for i in range(4):
        print(f"{blocks[i]:<25} {lengths[i]:<8} {direct_times[i]:<18.6f} {inverse_times[i]:<18.6f}")

if __name__ == "__main__":
    main()