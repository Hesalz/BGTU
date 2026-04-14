import os
import time

IP = [
    58, 50, 42, 34, 26, 18, 10, 2,
    60, 52, 44, 36, 28, 20, 12, 4,
    62, 54, 46, 38, 30, 22, 14, 6,
    64, 56, 48, 40, 32, 24, 16, 8,
    57, 49, 41, 33, 25, 17, 9,  1,
    59, 51, 43, 35, 27, 19, 11, 3,
    61, 53, 45, 37, 29, 21, 13, 5,
    63, 55, 47, 39, 31, 23, 15, 7
]

FP = [
    40, 8, 48, 16, 56, 24, 64, 32,
    39, 7, 47, 15, 55, 23, 63, 31,
    38, 6, 46, 14, 54, 22, 62, 30,
    37, 5, 45, 13, 53, 21, 61, 29,
    36, 4, 44, 12, 52, 20, 60, 28,
    35, 3, 43, 11, 51, 19, 59, 27,
    34, 2, 42, 10, 50, 18, 58, 26,
    33, 1, 41, 9,  49, 17, 57, 25
]

E = [
    32, 1,  2,  3,  4,  5,
    4,  5,  6,  7,  8,  9,
    8,  9,  10, 11, 12, 13,
    12, 13, 14, 15, 16, 17,
    16, 17, 18, 19, 20, 21,
    20, 21, 22, 23, 24, 25,
    24, 25, 26, 27, 28, 29,
    28, 29, 30, 31, 32, 1
]

P = [
    16, 7,  20, 21, 29, 12, 28, 17,
    1,  15, 23, 26, 5,  18, 31, 10,
    2,  8,  24, 14, 32, 27, 3,  9,
    19, 13, 30, 6,  22, 11, 4,  25
]

SBOXES = [
    [
        [14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7],
        [0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8],
        [4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0],
        [15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13]
    ],
    # S2
    [
        [15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10],
        [3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5],
        [0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15],
        [13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9]
    ],
    # S3
    [
        [10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8],
        [13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1],
        [13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7],
        [1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12]
    ],
    # S4
    [
        [7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15],
        [13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9],
        [10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4],
        [3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14]
    ],
    # S5
    [
        [2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9],
        [14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6],
        [4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14],
        [11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3]
    ],
    # S6
    [
        [12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11],
        [10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8],
        [9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6],
        [4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13]
    ],
    # S7
    [
        [4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1],
        [13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6],
        [1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2],
        [6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12]
    ],
    # S8
    [
        [13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7],
        [1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2],
        [7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8],
        [2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11]
    ]
]

PC1 = [
    57, 49, 41, 33, 25, 17, 9,
    1,  58, 50, 42, 34, 26, 18,
    10, 2,  59, 51, 43, 35, 27,
    19, 11, 3,  60, 52, 44, 36,
    63, 55, 47, 39, 31, 23, 15,
    7,  62, 54, 46, 38, 30, 22,
    14, 6,  61, 53, 45, 37, 29,
    21, 13, 5,  28, 20, 12, 4
]

PC2 = [
    14, 17, 11, 24, 1,  5,
    3,  28, 15, 6,  21, 10,
    23, 19, 12, 4,  26, 8,
    16, 7,  27, 20, 13, 2,
    41, 52, 31, 37, 47, 55,
    30, 40, 51, 45, 33, 48,
    44, 49, 39, 56, 34, 53,
    46, 42, 50, 36, 29, 32
]

SHIFT_SCHEDULE = [1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1]

def bytes_to_bits(data):
    bits = []
    for byte in data:
        for i in range(7, -1, -1):
            bits.append((byte >> i) & 1)
    return bits

def bits_to_bytes(bits):
    if len(bits) % 8 != 0:
        raise ValueError("Количество бит должно быть кратно 8")
    result = []
    for i in range(0, len(bits), 8):
        byte = 0
        for j in range(8):
            byte = (byte << 1) | bits[i + j]
        result.append(byte)
    return bytes(result)

def permute(block, table):
    return [block[i - 1] for i in table]

def left_shift(lst, n):
    return lst[n:] + lst[:n]

def xor_lists(a, b):
    return [x ^ y for x, y in zip(a, b)]

class DES:
    def __init__(self, key_bytes):
        if len(key_bytes) != 8:
            raise ValueError("Ключ DES должен быть ровно 8 байт")
        self.key = key_bytes
        self.subkeys = self._generate_subkeys(key_bytes)

    def _generate_subkeys(self, key):
        bits = bytes_to_bits(key)
        key56 = permute(bits, PC1)
        C = key56[:28]
        D = key56[28:]
        subkeys = []
        for i in range(16):
            C = left_shift(C, SHIFT_SCHEDULE[i])
            D = left_shift(D, SHIFT_SCHEDULE[i])
            CD = C + D
            subkey = permute(CD, PC2)
            subkeys.append(subkey)
        return subkeys

    def _f(self, R, K):
        expanded = permute(R, E)
        xored = xor_lists(expanded, K)
        sbox_out = []
        for i in range(8):
            chunk = xored[i * 6:(i + 1) * 6]
            row = (chunk[0] << 1) | chunk[5]
            col = (chunk[1] << 3) | (chunk[2] << 2) | (chunk[3] << 1) | chunk[4]
            val = SBOXES[i][row][col]
            sbox_out.extend([(val >> 3) & 1, (val >> 2) & 1, (val >> 1) & 1, val & 1])
        f_result = permute(sbox_out, P)
        return f_result

    def encrypt_block(self, block):
        if len(block) != 8:
            raise ValueError("Блок должен быть 8 байт")
        bits = bytes_to_bits(block)
        bits = permute(bits, IP)
        L = bits[:32]
        R = bits[32:]
        for i in range(16):
            L, R = R, xor_lists(L, self._f(R, self.subkeys[i]))
        combined = R + L
        cipher_bits = permute(combined, FP)
        return bits_to_bytes(cipher_bits)

    def decrypt_block(self, block):
        if len(block) != 8:
            raise ValueError("Блок должен быть 8 байт")
        bits = bytes_to_bits(block)
        bits = permute(bits, IP)
        L = bits[:32]
        R = bits[32:]
        for i in range(15, -1, -1):
            L, R = R, xor_lists(L, self._f(R, self.subkeys[i]))
        combined = R + L
        plain_bits = permute(combined, FP)
        return bits_to_bytes(plain_bits)

class TripleDES:
    def __init__(self, key1, key2, key3):
        self.des1 = DES(key1)
        self.des2 = DES(key2)
        self.des3 = DES(key3)

    def encrypt_block(self, block):
        return self.des1.encrypt_block(
            self.des2.decrypt_block(
                self.des3.encrypt_block(block)
            )
        )

    def decrypt_block(self, block):
        return self.des3.decrypt_block(
            self.des2.encrypt_block(
                self.des1.decrypt_block(block)
            )
        )

def pad(data, block_size=8):
    padding_len = block_size - (len(data) % block_size)
    return data + bytes([padding_len] * padding_len)

def unpad(data):
    if not data:
        raise ValueError("Нет данных для удаления дополнения")
    padding_len = data[-1]
    if padding_len < 1 or padding_len > 8:
        raise ValueError("Некорректная длина дополнения")
    if data[-padding_len:] != bytes([padding_len] * padding_len):
        raise ValueError("Некорректное дополнение")
    return data[:-padding_len]

def encrypt_data(tdes, data):
    padded = pad(data)
    encrypted = b''
    for i in range(0, len(padded), 8):
        block = padded[i:i+8]
        encrypted += tdes.encrypt_block(block)
    return encrypted

def decrypt_data(tdes, data):
    if len(data) % 8 != 0:
        raise ValueError("Длина зашифрованных данных должна быть кратна 8")
    decrypted = b''
    for i in range(0, len(data), 8):
        block = data[i:i+8]
        decrypted += tdes.decrypt_block(block)
    return unpad(decrypted)

def avalanche_effect(tdes, plaintext_block, bit_position):
    if len(plaintext_block) != 8:
        raise ValueError("Блок должен быть 8 байт")
    if bit_position < 0 or bit_position > 63:
        raise ValueError("Позиция бита должна быть от 0 до 63")

    original_cipher = tdes.encrypt_block(plaintext_block)

    plain_list = list(plaintext_block)
    byte_idx = bit_position // 8
    bit_in_byte = 7 - (bit_position % 8)
    plain_list[byte_idx] ^= (1 << bit_in_byte)
    modified_plain = bytes(plain_list)

    modified_cipher = tdes.encrypt_block(modified_plain)

    orig_bits = bytes_to_bits(original_cipher)
    mod_bits = bytes_to_bits(modified_cipher)
    diff_count = sum(1 for i in range(64) if orig_bits[i] != mod_bits[i])

    return diff_count, original_cipher, modified_cipher

def speed_test(tdes, data_size_mb=1):
    test_block = b'\x00' * 8
    if tdes.decrypt_block(tdes.encrypt_block(test_block)) != test_block:
        print("Ошибка: базовое шифрование блока не работает!")
        return

    data = os.urandom(int(data_size_mb * 1024 * 1024))

    start = time.perf_counter()
    encrypted = encrypt_data(tdes, data)
    mid = time.perf_counter()
    decrypted = decrypt_data(tdes, encrypted)
    end = time.perf_counter()

    if data != decrypted:
        print("Ошибка: расшифрованные данные не совпадают с исходными!")
        print(f"Длина исходных: {len(data)}, расшифрованных: {len(decrypted)}")
        print(f"Исходные (первые 16): {data[:16].hex()}")
        print(f"Расшифрованные (первые 16): {decrypted[:16].hex()}")
        return

    encrypt_time = mid - start
    decrypt_time = end - mid
    total_time = end - start

    print(f"\n--- Скоростной тест ({data_size_mb} МБ) ---")
    print(f"Время шифрования:   {encrypt_time:.3f} с  ({data_size_mb/encrypt_time:.2f} МБ/с)")
    print(f"Время расшифрования: {decrypt_time:.3f} с  ({data_size_mb/decrypt_time:.2f} МБ/с)")
    print(f"Общее время:         {total_time:.3f} с")

def input_keys():
    print("\n--- Ввод ключей для Triple-DES ---")
    k1_str = input("Ключ 1: ")
    k2_str = input("Ключ 2: ")
    k3_str = input("Ключ 3: ")

    key1 = k1_str.encode('utf-8')[:8].ljust(8, b'\0')
    key2 = k2_str.encode('utf-8')[:8].ljust(8, b'\0')
    key3 = k3_str.encode('utf-8')[:8].ljust(8, b'\0')

    print("\nКлючи (в hex):")
    print(f"K1: {key1.hex().upper()}")
    print(f"K2: {key2.hex().upper()}")
    print(f"K3: {key3.hex().upper()}")
    return key1, key2, key3

def main():
    default_key = b'\x00' * 8
    tdes = TripleDES(default_key, default_key, default_key)

    while True:
        print("\n1. Задать ключи")
        print("2. Зашифровать текст")
        print("3. Расшифровать текст")
        print("4. Анализ лавинного эффекта")
        print("5. Выход")
        choice = input("Выберите действие (1-5): ").strip()

        if choice == '1':
            key1, key2, key3 = input_keys()
            tdes = TripleDES(key1, key2, key3)
            print("Ключи успешно установлены.")

        elif choice == '2':
            text = input("Введите текст для шифрования: ")
            data = text.encode('utf-8')
            try:
                start = time.perf_counter()
                encrypted = encrypt_data(tdes, data)
                end = time.perf_counter()
                print("\nЗашифрованные данные (hex):")
                print(encrypted.hex().upper())
                print(f"Время шифрования: {end - start:.6f} с  (размер данных: {len(data)} байт)")
            except Exception as e:
                print(f"Ошибка: {e}")

        elif choice == '3':
            hex_str = input("Введите зашифрованные данные в hex (без пробелов): ").strip()
            try:
                encrypted = bytes.fromhex(hex_str)
                start = time.perf_counter()
                decrypted = decrypt_data(tdes, encrypted)
                end = time.perf_counter()
                try:
                    print("\nРасшифрованный текст:")
                    print(decrypted.decode('utf-8'))
                except UnicodeDecodeError:
                    print("\nРасшифрованные данные (не UTF-8, показаны в hex):")
                    print(decrypted.hex().upper())
                print(f"Время расшифрования: {end - start:.6f} с  (размер данных: {len(encrypted)} байт)")
            except Exception as e:
                print(f"Ошибка: {e}")

        elif choice == '4':
            text = input("Введите текст (будет взят первый блок из 8 байт): ")
            data = text.encode('utf-8')[:8]
            if len(data) < 8:
                print("Текст слишком короткий, дополняется нулями до 8 байт.")
                data = data.ljust(8, b'\0')
            print(f"Исходный блок (hex): {data.hex().upper()}")

            try:
                bit_pos = int(input("Введите номер бита для изменения (0-63): "))
                diff, orig_cipher, mod_cipher = avalanche_effect(tdes, data, bit_pos)
                print(f"\nИсходный шифротекст:    {orig_cipher.hex().upper()}")
                print(f"Изменённый шифротекст:   {mod_cipher.hex().upper()}")
                print(f"Количество изменившихся бит: {diff} из 64 ({diff/64*100:.1f}%)")
            except ValueError as e:
                print(f"Ошибка ввода: {e}")
            except Exception as e:
                print(f"Ошибка: {e}")

        elif choice == '5':
            print("Выход из программы.")
            break
        else:
            print("Неверный выбор. Попробуйте снова.")

if __name__ == "__main__":
    main()