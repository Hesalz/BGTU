import random
import numpy as np
from math import ceil, log2

class CyclicCode:
    def __init__(self, n, k):
        self.n = n
        self.k = k 

        self.generator_poly = 0b111010001
        self.poly_degree = 8
        
    def poly_to_bits(self, poly, length):
        """Преобразование полинома в битовую строку"""
        return [int(b) for b in format(poly, f'0{length}b')]
    
    def bits_to_poly(self, bits):
        """Преобразование битовой строки в полином"""
        return int(''.join(str(b) for b in bits), 2)
    
    def poly_mod(self, dividend, divisor):
        """Деление полиномов по модулю 2"""
        dividend_len = dividend.bit_length()
        divisor_len = divisor.bit_length()
        
        while dividend_len >= divisor_len:
            shift = dividend_len - divisor_len
            dividend ^= (divisor << shift)
            dividend_len = dividend.bit_length()
            
        return dividend
    
    def encode(self, info_bits):
        """Кодирование циклическим кодом"""
        if len(info_bits) != self.k:
            raise ValueError(f"Информационное слово должно быть длиной {self.k} бит")
        
        info_poly = self.bits_to_poly(info_bits)
        shifted_poly = info_poly << (self.n - self.k)
        remainder = self.poly_mod(shifted_poly, self.generator_poly)
        codeword_poly = shifted_poly | remainder

        return self.poly_to_bits(codeword_poly, self.n)
    
    def decode(self, received_bits):
        """Декодирование циклического кода с исправлением ошибок"""
        if len(received_bits) != self.n:
            raise ValueError(f"Принятое слово должно быть длиной {self.n} бит")
        
        received_poly = self.bits_to_poly(received_bits)
        
        syndrome = self.poly_mod(received_poly, self.generator_poly)
        
        if syndrome != 0:
            corrected = self.correct_error(received_bits, syndrome)
            return corrected
        else:
            return received_bits
    
    def correct_error(self, received_bits, syndrome):
        """Коррекция одиночной ошибки"""
        corrected = received_bits.copy()
        error_pattern = self.poly_to_bits(syndrome, self.n)
        
        for i in range(self.n):
            if error_pattern[i] == 1:
                corrected[i] ^= 1
                break
                
        return corrected
    
    def extract_info_bits(self, codeword_bits):
        """Извлечение информационных битов из кодового слова"""
        return codeword_bits[:self.k]

def generate_random_binary_string(length):
    """Генерация случайной бинарной строки"""
    return ''.join(str(random.randint(0, 1)) for _ in range(length))

def split_message(msg, size):
    """Разбиение сообщения на части заданного размера"""
    return [msg[i:i+size].ljust(size, '0') for i in range(0, len(msg), size)]

def interleave_flat(data, columns):
    """Блочное перемежение по столбцам"""
    rows = ceil(len(data) / columns)
    
    matrix = np.zeros((rows, columns), dtype=int)
    idx = 0
    for i in range(rows):
        for j in range(columns):
            if idx < len(data):
                matrix[i, j] = data[idx]
                idx += 1
    
    result = []
    for j in range(columns):
        for i in range(rows):
            if i * columns + j < len(data):
                result.append(matrix[i, j])
    
    return result

def deinterleave_flat(data, columns):
    """Деперемежение"""
    rows = ceil(len(data) / columns)
    
    matrix = np.zeros((rows, columns), dtype=int)
    idx = 0
    for j in range(columns):
        for i in range(rows):
            if idx < len(data):
                matrix[i, j] = data[idx]
                idx += 1
    
    result = []
    for i in range(rows):
        for j in range(columns):
            if i * columns + j < len(data):
                result.append(matrix[i, j])
    
    return result

def count_bit_differences(a, b):
    """Подсчет количества различающихся битов"""
    return sum(1 for x, y in zip(a, b) if x != y)

def print_array(arr, title=""):
    """Печать массива"""
    if title:
        print(title)
    print(''.join(str(x) for x in arr))

def print_matrix(data, rows, columns, title=""):
    """Печать матрицы"""
    if title:
        print(title)
    
    idx = 0
    for i in range(rows):
        row = []
        for j in range(columns):
            if idx < len(data):
                row.append(str(data[idx]))
                idx += 1
            else:
                row.append(" ")
        print(''.join(row))

def main():
    
    k = 7
    n = 15
    columns = 5
    message_bytes = 15
    message_bits = message_bytes * 8
    
    print("Параметры:")
    print(f"Длина информационного слова (k): {k}")
    print(f"Длина кодового слова (n): {n}")
    print(f"Число столбцов перемежителя: {columns}")
    print(f"Длина сообщения: {message_bytes} байт ({message_bits} бит)\n")
    
    cyclic_code = CyclicCode(n, k)
    
    print("-" * 50)
    print("ГЕНЕРАЦИЯ ДАННЫХ")
    
    msg = generate_random_binary_string(message_bits)
    print(f"Исходное сообщение ({len(msg)} бит):")
    print(msg)
    print()
    
    print("-" * 50)
    print("КОДИРОВАНИЕ")
    
    info_words_str = split_message(msg, k)
    print(f"Разбиение на {len(info_words_str)} информационных слов по {k} бит:")
    for i, word in enumerate(info_words_str):
        print(f"Слово {i+1}: {word}")
    print()
    
    info_words = [[int(bit) for bit in word] for word in info_words_str]
    codewords = [cyclic_code.encode(word) for word in info_words]
    
    print("Закодированные слова:")
    for i, cw in enumerate(codewords):
        print(f"Слово {i+1}: {''.join(str(bit) for bit in cw)}")
    print()
    
    print("-" * 50)
    print("ПЕРЕМЕЖЕНИЕ")
    
    full_data = [bit for codeword in codewords for bit in codeword]
    print(f"Полная кодовая последовательность ({len(full_data)} бит):")
    print_array(full_data)
    
    rows = ceil(len(full_data) / columns)
    print(f"\nМатрица перемежителя: {rows} строк × {columns} столбцов")
    
    print("\nМатрица до перемежения:")
    print_matrix(full_data, rows, columns)
    
    interleaved = interleave_flat(full_data, columns)
    print("\nМатрица после перемежения (чтение по столбцам):")
    print_matrix(interleaved, rows, columns)
    
    print("\nПеремеженная последовательность:")
    print_array(interleaved)
    
    error_lengths = [4, 5, 6]
    
    for error_len in error_lengths:
        print("\n" + "-" * 60)
        print(f"МОДЕЛИРОВАНИЕ ПАКЕТНОЙ ОШИБКИ ДЛИНОЙ {error_len} БИТ")
        
        corrupted = interleaved.copy()
        
        error_pos = random.randint(0, len(corrupted) - error_len)
        
        for i in range(error_pos, error_pos + error_len):
            corrupted[i] ^= 1 
        
        print(f"Ошибка внесена с позиции {error_pos}, длина {error_len} бит")
        print("Перемеженная последовательность с ошибками:")
        print_array(corrupted)
        
        deinterleaved = deinterleave_flat(corrupted, columns)
        print("\nДеперемеженная последовательность с ошибками:")
        print_array(deinterleaved)
        
        received_codewords = []
        for i in range(len(codewords)):
            start_idx = i * n
            end_idx = start_idx + n
            if end_idx <= len(deinterleaved):
                received_word = deinterleaved[start_idx:end_idx]
                received_codewords.append(received_word)
        
        corrected_info_bits = []
        for i, rcw in enumerate(received_codewords):
            corrected_cw = cyclic_code.decode(rcw)
            info_bits = cyclic_code.extract_info_bits(corrected_cw)
            corrected_info_bits.extend(info_bits)
        
        restored_msg = ''.join(str(bit) for bit in corrected_info_bits[:len(msg)])
        
        print("\nВосстановленное сообщение:")
        print(restored_msg)
        
        diff_count = count_bit_differences(msg, restored_msg)
        error_percent = (diff_count / len(msg)) * 100
        
        print(f"\nРезультат сравнения:")
        print(f"Количество ошибочных бит: {diff_count}")
        print(f"Процент ошибок: {error_percent:.2f}%")
    
    print("\n" + "-" * 60)
    print("АНАЛИЗ ЭФФЕКТИВНОСТИ ЭКСПЕРИМЕНТОВ:")
    
    experiments = 35
    total_results = {length: [] for length in error_lengths}
    
    for error_len in error_lengths:
        print(f"\nАнализ для ошибок длиной {error_len} бит ({experiments} экспериментов):")
        
        error_counts = []
        
        for exp in range(experiments):
            corrupted = interleaved.copy()
            
            error_pos = random.randint(0, len(corrupted) - error_len)
            
            for i in range(error_pos, error_pos + error_len):
                corrupted[i] ^= 1
            
            deinterleaved_data = deinterleave_flat(corrupted, columns)
            
            received_codewords = []
            for i in range(len(codewords)):
                start_idx = i * n
                end_idx = start_idx + n
                if end_idx <= len(deinterleaved_data):
                    received_word = deinterleaved_data[start_idx:end_idx]
                    received_codewords.append(received_word)
            
            corrected_info_bits = []
            for rcw in received_codewords:
                corrected_cw = cyclic_code.decode(rcw)
                info_bits = cyclic_code.extract_info_bits(corrected_cw)
                corrected_info_bits.extend(info_bits)
            
            restored_msg = ''.join(str(bit) for bit in corrected_info_bits[:len(msg)])
            
            diff_count = count_bit_differences(msg, restored_msg)
            error_counts.append(diff_count)
        
        avg_errors = sum(error_counts) / len(error_counts)
        avg_error_percent = (avg_errors / len(msg)) * 100
        min_errors = min(error_counts)
        max_errors = max(error_counts)
        
        print(f"Среднее количество ошибок: {avg_errors:.2f}")
        print(f"Средний процент ошибок: {avg_error_percent:.2f}%")
        print(f"Минимальное количество ошибок: {min_errors}")
        print(f"Максимальное количество ошибок: {max_errors}")
        
        total_results[error_len] = error_counts
    
    print("\n" + "-" * 60)
    print("ОБЩАЯ СТАТИСТИКА")
    
    all_errors = [error for errors in total_results.values() for error in errors]
    overall_avg_errors = sum(all_errors) / len(all_errors)
    overall_avg_percent = (overall_avg_errors / len(msg)) * 100
    
    print(f"Общий средний процент ошибок по всем экспериментам: {overall_avg_percent:.2f}%")
    print(f"Эффективность восстановления: {100 - overall_avg_percent:.2f}%")

if __name__ == "__main__":
    main()