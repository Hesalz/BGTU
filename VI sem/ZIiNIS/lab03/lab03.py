import numpy as np
import matplotlib.pyplot as plt
import time
from collections import Counter

class RouteCipher:
    def __init__(self, rows, cols):
        self.rows = rows
        self.cols = cols
    
    def encrypt(self, text):
        block_size = self.rows * self.cols
        result = []
        
        for i in range(0, len(text), block_size):
            block = text[i:i+block_size]
            
            if len(block) < block_size:
                block = block.ljust(block_size)
            
            table = []
            for r in range(self.rows):
                row = list(block[r*self.cols:(r+1)*self.cols])
                table.append(row)
            
            for j in range(self.cols):
                if j % 2 == 0:
                    for i in range(self.rows):
                        result.append(table[i][j])
                else:
                    for i in range(self.rows-1, -1, -1):
                        result.append(table[i][j])
        
        return ''.join(result)
    
    def decrypt(self, text):
        block_size = self.rows * self.cols
        result = []
        
        for block_start in range(0, len(text), block_size):
            block = text[block_start:block_start+block_size]
            
            table = [['' for _ in range(self.cols)] for _ in range(self.rows)]
            
            idx = 0
            for j in range(self.cols):
                if j % 2 == 0:
                    for i in range(self.rows):
                        if idx < len(block):
                            table[i][j] = block[idx]
                            idx += 1
                else:
                    for i in range(self.rows-1, -1, -1):
                        if idx < len(block):
                            table[i][j] = block[idx]
                            idx += 1
            
            for i in range(self.rows):
                for j in range(self.cols):
                    result.append(table[i][j])
        
        return ''.join(result).rstrip()


class MultiplePermutationCipher:
    def __init__(self, keyword1="BABASHINSKII", keyword2="HLEB"):
        self.keyword1 = keyword1.upper()
        self.keyword2 = keyword2.upper()
        self.key1_perm = self._get_permutation(self.keyword1)
        self.key2_perm = self._get_permutation(self.keyword2)
    
    def _get_permutation(self, keyword):
        sorted_chars = sorted([(char, i) for i, char in enumerate(keyword)])
        permutation = [0] * len(keyword)
        for new_pos, (_, old_pos) in enumerate(sorted_chars):
            permutation[old_pos] = new_pos
        return permutation
    
    def encrypt(self, text):
        rows = len(self.keyword2)
        cols = len(self.keyword1)
        block_size = rows * cols
        result = []
        
        for i in range(0, len(text), block_size):
            block = text[i:i+block_size]
            
            if len(block) < block_size:
                block = block.ljust(block_size)
            
            table = []
            for r in range(rows):
                row = list(block[r*cols:(r+1)*cols])
                table.append(row)
            
            permuted_rows = [None] * rows
            for r, perm in enumerate(self.key2_perm):
                permuted_rows[perm] = table[r]
            
            result_table = [['' for _ in range(cols)] for _ in range(rows)]
            for r in range(rows):
                for c, perm in enumerate(self.key1_perm):
                    result_table[r][perm] = permuted_rows[r][c]
            
            for r in range(rows):
                for c in range(cols):
                    result.append(result_table[r][c])
        
        return ''.join(result)
    
    def decrypt(self, text):
        rows = len(self.keyword2)
        cols = len(self.keyword1)
        block_size = rows * cols
        
        inv_key1 = [0] * cols
        for i, p in enumerate(self.key1_perm):
            inv_key1[p] = i
        
        inv_key2 = [0] * rows
        for i, p in enumerate(self.key2_perm):
            inv_key2[p] = i
        
        result = []
        
        for block_start in range(0, len(text), block_size):
            block = text[block_start:block_start+block_size]
            
            table = [['' for _ in range(cols)] for _ in range(rows)]
            idx = 0
            for r in range(rows):
                for c in range(cols):
                    if idx < len(block):
                        table[r][c] = block[idx]
                        idx += 1
            
            col_permuted = [['' for _ in range(cols)] for _ in range(rows)]
            for r in range(rows):
                for c in range(cols):
                    col_permuted[r][inv_key1[c]] = table[r][c]
            
            result_table = [['' for _ in range(cols)] for _ in range(rows)]
            for r in range(rows):
                result_table[inv_key2[r]] = col_permuted[r]
            
            for r in range(rows):
                for c in range(cols):
                    result.append(result_table[r][c])
        
        return ''.join(result).rstrip()


class EncryptionApp:
    def __init__(self):
        self.route_cipher = None
        self.multiple_cipher = MultiplePermutationCipher()
        self.original_text = ""
        self.last_encrypted_route = ""
        self.last_encrypted_multiple = ""
        self.last_decrypted_route = ""
        self.last_decrypted_multiple = ""
    
    def read_text_from_file(self, filename):
        try:
            with open(filename, 'r', encoding='utf-8') as file:
                return file.read()
        except FileNotFoundError:
            print(f"Файл {filename} не найден. Используется дефолтный текст.")
            return self.generate_test_text()
    
    def generate_test_text(self):
        text = """The sun was setting behind the mountains, casting long shadows across the valley. Birds were returning to their nests, singing their evening songs. The air was filled with the sweet scent of flowers blooming in the garden. Children were playing in the streets, their laughter echoing through the neighborhood. It was a perfect summer evening, peaceful and beautiful. The sky was painted in shades of orange and pink, creating a breathtaking view. As the night approached, the first stars began to appear, twinkling in the darkening sky."""
        return text
    
    def calculate_frequency(self, text):
        letters = [char.lower() for char in text if char.isalpha()]
        counter = Counter(letters)
        total = len(letters)
        
        if total == 0:
            return {}
        
        frequencies = {char: count/total for char, count in counter.items()}
        return frequencies
    
    def plot_comparison_histogram(self, freq1, freq2, title1, title2, main_title):
        all_chars = sorted(set(list(freq1.keys()) + list(freq2.keys())))
        
        if not all_chars:
            print("Нет данных для построения гистограммы")
            return
        
        freqs1 = [freq1.get(char, 0) for char in all_chars]
        freqs2 = [freq2.get(char, 0) for char in all_chars]
        
        plt.figure(figsize=(15, 6))
        
        x = np.arange(len(all_chars))
        width = 0.35
        
        plt.bar(x - width/2, freqs1, width, label=title1, color='skyblue', edgecolor='black')
        plt.bar(x + width/2, freqs2, width, label=title2, color='lightcoral', edgecolor='black')
        
        plt.xlabel('Буквы', fontsize=12)
        plt.ylabel('Частота', fontsize=12)
        plt.title(main_title, fontsize=14, fontweight='bold')
        plt.xticks(x, all_chars)
        plt.legend()
        plt.grid(True, alpha=0.3, axis='y')
        plt.tight_layout()
        plt.show()
    
    def run(self):
        filename = input("Введите имя файла с текстом (или Enter для дефолтного): ").strip()
        if filename:
            self.original_text = self.read_text_from_file(filename)
        else:
            self.original_text = self.generate_test_text()
        
        print(f"\nИсходный текст (длина: {len(self.original_text)} символов):")
        print("-" * 40)
        print(self.original_text[:300] + "..." if len(self.original_text) > 300 else self.original_text)
        
        while True:
            print("\n1. Зашифровать маршрутной перестановкой (зигзаг)")
            print("2. Зашифровать множественной перестановкой")
            print("3. Расшифровать маршрутную перестановку")
            print("4. Расшифровать множественную перестановку")
            print("5. Сравнить частоты букв (расшифрованный vs зашифрованный)")
            print("0. Выход")
            
            choice = input("\nВыберите действие: ").strip()
            
            if choice == '1':
                try:
                    rows = int(input("Введите количество строк: "))
                    cols = int(input("Введите количество столбцов: "))
                    
                    self.route_cipher = RouteCipher(rows, cols)
                    
                    start_time = time.time()
                    self.last_encrypted_route = self.route_cipher.encrypt(self.original_text)
                    end_time = time.time()
                    
                    print(f"\nЗашифрованный текст (длина: {len(self.last_encrypted_route)}):")
                    
                    display_text = self.last_encrypted_route[:300] + "..." if len(self.last_encrypted_route) > 300 else self.last_encrypted_route
                    for i in range(0, len(display_text), 80):
                        print(display_text[i:i+80])
                    
                    print(f"\nВремя шифрования: {(end_time - start_time)*1000:.3f} мс")
                    
                    with open("encrypted_route.txt", "w", encoding='utf-8') as f:
                        f.write(self.last_encrypted_route)
                    print("Результат сохранен в файл encrypted_route.txt")
                    
                except ValueError:
                    print("Ошибка: введите целые числа")
            
            elif choice == '2':
                start_time = time.time()
                self.last_encrypted_multiple = self.multiple_cipher.encrypt(self.original_text)
                end_time = time.time()
                
                print(f"\nЗашифрованный текст (длина: {len(self.last_encrypted_multiple)}):")
                
                display_text = self.last_encrypted_multiple[:300] + "..." if len(self.last_encrypted_multiple) > 300 else self.last_encrypted_multiple
                for i in range(0, len(display_text), 80):
                    print(display_text[i:i+80])
                
                print(f"\nВремя шифрования: {(end_time - start_time)*1000:.3f} мс")
                
                with open("encrypted_multiple.txt", "w", encoding='utf-8') as f:
                    f.write(self.last_encrypted_multiple)
                print("Результат сохранен в файл encrypted_multiple.txt")
            
            elif choice == '3':
                if not self.route_cipher:
                    print("Сначала выполните шифрование маршрутной перестановкой")
                    continue
                
                try:
                    filename = input("Введите имя файла с зашифрованным текстом (Enter для использования последнего): ").strip()
                    if filename:
                        with open(filename, 'r', encoding='utf-8') as f:
                            encrypted = f.read()
                    else:
                        if self.last_encrypted_route:
                            encrypted = self.last_encrypted_route
                        else:
                            with open("encrypted_route.txt", 'r', encoding='utf-8') as f:
                                encrypted = f.read()
                    
                    start_time = time.time()
                    self.last_decrypted_route = self.route_cipher.decrypt(encrypted)
                    end_time = time.time()
                    
                    print(f"\nРасшифрованный текст (длина: {len(self.last_decrypted_route)}):")
    
                    print(self.last_decrypted_route[:300] + "..." if len(self.last_decrypted_route) > 300 else self.last_decrypted_route)
                    print(f"\nВремя расшифрования: {(end_time - start_time)*1000:.3f} мс")
                    
                    with open("decrypted_route.txt", "w", encoding='utf-8') as f:
                        f.write(self.last_decrypted_route)
                    print("Расшифрованный текст сохранен в файл decrypted_route.txt")
                    
                    original_trimmed = self.original_text[:len(self.last_decrypted_route)]
                    if self.last_decrypted_route.rstrip() == original_trimmed.rstrip():
                        print("\nРасшифрование выполнено успешно! Текст совпадает с оригиналом.")
                    else:
                        print("\nРасшифрованный текст отличается от оригинала!")
                        
                except FileNotFoundError:
                    print("Файл не найден")
            
            elif choice == '4':
                try:
                    filename = input("Введите имя файла с зашифрованным текстом (Enter для использования последнего): ").strip()
                    if filename:
                        with open(filename, 'r', encoding='utf-8') as f:
                            encrypted = f.read()
                    else:
                        if self.last_encrypted_multiple:
                            encrypted = self.last_encrypted_multiple
                        else:
                            with open("encrypted_multiple.txt", 'r', encoding='utf-8') as f:
                                encrypted = f.read()
                    
                    start_time = time.time()
                    self.last_decrypted_multiple = self.multiple_cipher.decrypt(encrypted)
                    end_time = time.time()
                    
                    print(f"\nРасшифрованный текст (длина: {len(self.last_decrypted_multiple)}):")
    
                    print(self.last_decrypted_multiple[:300] + "..." if len(self.last_decrypted_multiple) > 300 else self.last_decrypted_multiple)
                    print(f"\nВремя расшифрования: {(end_time - start_time)*1000:.3f} мс")
                    
                    with open("decrypted_multiple.txt", "w", encoding='utf-8') as f:
                        f.write(self.last_decrypted_multiple)
                    print("Расшифрованный текст сохранен в файл decrypted_multiple.txt")
                    
                    original_trimmed = self.original_text[:len(self.last_decrypted_multiple)]
                    if self.last_decrypted_multiple.rstrip() == original_trimmed.rstrip():
                        print("\nРасшифрование выполнено успешно! Текст совпадает с оригиналом.")
                    else:
                        print("\nРасшифрованный текст отличается от оригинала!")
                        
                except FileNotFoundError:
                    print("Файл не найден")
            
            elif choice == '5':
                print("\nВыберите шифр для сравнения:")
                print("1. Маршрутная перестановка")
                print("2. Множественная перестановка")
                
                sub_choice = input("Ваш выбор: ").strip()
                
                if sub_choice == '1':
                    if not self.last_decrypted_route:
                        try:
                            with open("decrypted_route.txt", 'r', encoding='utf-8') as f:
                                self.last_decrypted_route = f.read()
                        except FileNotFoundError:
                            print("Сначала выполните расшифрование маршрутной перестановки")
                            continue
                    
                    if not self.last_encrypted_route:
                        try:
                            with open("encrypted_route.txt", 'r', encoding='utf-8') as f:
                                self.last_encrypted_route = f.read()
                        except FileNotFoundError:
                            print("Сначала выполните шифрование маршрутной перестановкой")
                            continue
                    
                    freq_decrypted = self.calculate_frequency(self.last_decrypted_route)
                    freq_encrypted = self.calculate_frequency(self.last_encrypted_route)
                    
                    self.plot_comparison_histogram(
                        freq_decrypted, freq_encrypted,
                        "Расшифрованный текст", "Зашифрованный текст",
                        "Сравнение частот букв: расшифрованный vs зашифрованный (маршрутный)"
                    )
                
                elif sub_choice == '2':
                    if not self.last_decrypted_multiple:
                        try:
                            with open("decrypted_multiple.txt", 'r', encoding='utf-8') as f:
                                self.last_decrypted_multiple = f.read()
                        except FileNotFoundError:
                            print("Сначала выполните расшифрование множественной перестановкой")
                            continue
                    
                    if not self.last_encrypted_multiple:
                        try:
                            with open("encrypted_multiple.txt", 'r', encoding='utf-8') as f:
                                self.last_encrypted_multiple = f.read()
                        except FileNotFoundError:
                            print("Сначала выполните шифрование множественной перестановкой")
                            continue
                    
                    freq_decrypted = self.calculate_frequency(self.last_decrypted_multiple)
                    freq_encrypted = self.calculate_frequency(self.last_encrypted_multiple)
                    
                    self.plot_comparison_histogram(
                        freq_decrypted, freq_encrypted,
                        "Расшифрованный текст", "Зашифрованный текст",
                        "Сравнение частот букв: расшифрованный vs зашифрованный (множественный)"
                    )
                
                else:
                    print("Неверный выбор")
            
            elif choice == '0':
                print("Программа завершена.")
                break
            
            else:
                print("Неверный выбор. Попробуйте снова.")


if __name__ == "__main__":
    app = EncryptionApp()
    app.run()