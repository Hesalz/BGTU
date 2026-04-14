import numpy as np
import random

def calculate_required_r(k):
    """Вычисление минимального необходимого r"""
    r = 1
    while 2**r < k + r + 1:
        r += 1
    return r

def build_hamming_matrix(k, r):
    """Правильное построение проверочной матрицы Хемминга"""
    n = k + r
    
    H = np.zeros((r, n), dtype=int)

    for i in range(r):
        H[i, k + i] = 1
    
    available_vectors = []
    for num in range(1, 2**r):
        if (num & (num - 1)) == 0:
            continue
        bin_vec = [int(bit) for bit in format(num, f'0{r}b')]
        available_vectors.append(bin_vec)
    
    for col_index in range(k):
        if col_index < len(available_vectors):
            H[:, col_index] = available_vectors[col_index]
        else:
            while True:
                col = np.random.randint(0, 2, r)
                if np.sum(col) >= 2: 
                    is_unique = True
                    for j in range(n):
                        if np.array_equal(col, H[:, j]):
                            is_unique = False
                            break
                    if is_unique:
                        H[:, col_index] = col
                        break
    
    return H

def text_to_binary(text, max_bits=16):
    """Преобразование текста в двоичный вид (еще короче для демонстрации)"""
    binary_string = ''.join(format(ord(char), '08b') for char in text)
    
    if len(binary_string) > max_bits:
        binary_string = binary_string[:max_bits]
        print(f"Текст усечен до {max_bits} бит для демонстрации")
    
    return binary_string

def check_syndrome_uniqueness(H):
    """Проверка уникальности синдромов"""
    syndromes = {}
    for j in range(H.shape[1]):
        syndrome = tuple(H[:, j])
        if syndrome in syndromes:
            print(f"Дублирование: позиции {syndromes[syndrome]} и {j+1} -> одинаковый синдром")
            return False
        syndromes[syndrome] = j + 1
    print("✓ Все вектор столбцы уникальны!")
    return True

def main():
    print("=== Лабораторная работа №4: Код Хемминга ===\n")
    
    print("--- 1. Подготовка данных ---")
    text = open("input.txt", "r", encoding="utf-8").read().strip() or "Hi!"
    print(f"Исходный текст: '{text}'")
    
    binary_data = text_to_binary(text, max_bits=16)
    print(f"Двоичное представление ({len(binary_data)} бит): {binary_data}")
    
    print("\n--- 2. Параметры кода ---")
    k = len(binary_data)
    r = calculate_required_r(k)
    n = k + r
    
    print(f"k = {k} (информационные биты)")
    print(f"Требуется: 2ʳ ≥ {k} + {r} + 1 = {k + r + 1}")
    print(f"2^{r} = {2**r} ≥ {k + r + 1}? {' ✓ДА' if 2**r >= k + r + 1 else '✕ НЕТ'}")
    print(f"r = {r} (проверочные биты)")
    print(f"n = {n} (общая длина)")
    print(f"Код Хемминга ({n}, {k})")
    
    print("\n--- 3. Построение проверочной матрицы ---")
    H = build_hamming_matrix(k, r)
    print(f"Матрица H ({r}x{n}):")
    for i in range(r):
        print(f"  {' '.join(map(str, H[i]))}")
    
    print("\n--- Проверка матрицы ---")
    check_syndrome_uniqueness(H)
    
    print("\nСоответствие вектор столбцов:")
    for j in range(min(n, 20)):
        syndrome = ''.join(map(str, H[:, j]))
        print(f"  Позиция {j+1:2d} -> Вектор столбец {syndrome}")
    if n > 20:
        print(f"  ... и еще {n-20} позиций")
    
    print("\n--- 4. Кодирование ---")
    Xk = np.array([int(bit) for bit in binary_data])
    P = H[:, :k]
    Xr = P @ Xk % 2
    Xn = np.concatenate((Xk, Xr))
    
    print(f"Информационные биты: {''.join(map(str, Xk))}")
    print(f"Проверочные биты Xr: {''.join(map(str, Xr))}")
    print(f"Полное кодовое слово: {''.join(map(str, Xn))}")
    
    syndrome = H @ Xn % 2
    print(f"Проверка кодирования (синдром): {''.join(map(str, syndrome))}")
    assert np.all(syndrome == 0), "✕ Ошибка кодирования!"
    
    print("\n--- 5-7. Эксперименты с ошибками ---")
    
    scenarios = [
        ("0 ошибок", 0),
        ("1 ошибка", 1), 
        ("2 ошибки", 2)
    ]
    
    for desc, num_errors in scenarios:
        print(f"\n★ Сценарий: {desc}")
        print("-" * 40)
        
        Yn = Xn.copy()
        error_positions = []
        
        if num_errors > 0:
            error_positions = random.sample(range(n), num_errors)
            print(f"Вносим ошибки в позиции: {[p+1 for p in error_positions]}")
            
            for pos in error_positions:
                Yn[pos] = 1 - Yn[pos]
        
        print(f"Принятое слово Yn: {''.join(map(str, Yn))}")
        
        Yk = Yn[:k]
        Yr_calculated = P @ Yk % 2 
        Yr_actual = Yn[k:]

        print(f"Yr (принятые проверочные биты): {''.join(map(str, Yr_actual))}")
        print(f"Yr' (вычисленные из Yk): {''.join(map(str, Yr_calculated))}")
        
        S = H @ Yn % 2
        syndrome_str = ''.join(map(str, S))
        print(f"Синдром S: {syndrome_str}")
        
        En = np.zeros(n, dtype=int)
        if np.all(S == 0):
            if num_errors == 0:
                print("✓ Синдром нулевой - ошибок нет")
            else:
                print("Синдром нулевой, но ошибки есть! (ненадежное обнаружение)")
            corrected = Yn
        else:
            found_pos = -1
            for j in range(n):
                if np.array_equal(S, H[:, j]):
                    found_pos = j
                    break
            
            if found_pos != -1:
                print(f"Обнаружена ошибка в позиции {found_pos + 1}")
                En[found_pos] = 1
                
                if found_pos in error_positions:
                    print("✓ Ошибка правильно идентифицирована!")
                    corrected = Yn.copy()
                    corrected[found_pos] = 1 - corrected[found_pos]
                    print(f"✓ Исправлена ошибка в позиции {found_pos + 1}")
                else:
                    print("Обнаружена ошибка в другой позиции (маска двойной ошибки)")
                    corrected = Yn 
            else:
                print("Неисправляемая ошибка (вероятно, двойная)")
                corrected = Yn
    
        print(f"Вектор ошибки En: {''.join(map(str, En))}")
        
        if np.array_equal(corrected, Xn):
            print("✓ Кодовое слово восстановлено правильно!")
        else:
            print("✕ Ошибка восстановления!")
            
        print(f"Исходное:  {''.join(map(str, Xn))}")
        print(f"Исправлен: {''.join(map(str, corrected))}")

if __name__ == "__main__":
    random.seed()
    main()