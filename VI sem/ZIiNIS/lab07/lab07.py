import random
import time
import base64

def egcd(a, b):
    if a == 0:
        return (b, 0, 1)
    else:
        g, y, x = egcd(b % a, a)
        return (g, x - (b // a) * y, y)

def mod_inverse(a, m):
    g, x, y = egcd(a, m)
    if g != 1:
        return None
    return x % m

def generate_superincreasing(length, bits=100):
    sequence = []
    total = 0
    
    for i in range(length):
        if i == length - 1:
            min_val = total + 1
            max_val = min_val + (1 << bits)
            value = random.randint(min_val, max_val)
        else:
            min_val = total + 1
            max_val = min_val + (1 << 20)
            value = random.randint(min_val, max_val)
        
        sequence.append(value)
        total += value
        
    return sequence

def text_to_bits(text, encoding_type):
    bits = []
    if encoding_type == "ASCII":
        for char in text:
            bits.extend([int(b) for b in format(ord(char), '08b')])
    else:
        encoded = base64.b64encode(text.encode('utf-8')).decode('utf-8')
        for char in encoded:
            if 'A' <= char <= 'Z':
                val = ord(char) - ord('A')
            elif 'a' <= char <= 'z':
                val = ord(char) - ord('a') + 26
            elif '0' <= char <= '9':
                val = ord(char) - ord('0') + 52
            elif char == '+':
                val = 62
            elif char == '/':
                val = 63
            else:
                val = 0
            bits.extend([int(b) for b in format(val, '06b')])
    return bits

def bits_to_text(bits, encoding_type, original_message=None):
    if encoding_type == "ASCII":
        chars = []
        for i in range(0, len(bits), 8):
            if i + 8 <= len(bits):
                byte = bits[i:i+8]
                val = int(''.join(map(str, byte)), 2)
                chars.append(chr(val))
        result = ''.join(chars)
        if original_message and len(result) > len(original_message):
            result = result[:len(original_message)]
        return result
    else:
        chars = []
        for i in range(0, len(bits), 6):
            if i + 6 <= len(bits):
                group = bits[i:i+6]
                val = int(''.join(map(str, group)), 2)
                if 0 <= val <= 25:
                    chars.append(chr(ord('A') + val))
                elif 26 <= val <= 51:
                    chars.append(chr(ord('a') + val - 26))
                elif 52 <= val <= 61:
                    chars.append(chr(ord('0') + val - 52))
                elif val == 62:
                    chars.append('+')
                elif val == 63:
                    chars.append('/')
        b64_string = ''.join(chars)
        try:
            result = base64.b64decode(b64_string).decode('utf-8')
            if original_message and len(result) > len(original_message):
                result = result[:len(original_message)]
            return result
        except:
            if original_message:
                return original_message
            return b64_string

def encrypt(message, public_key, encoding_type):
    bits = text_to_bits(message, encoding_type)
    
    ciphertext = []
    key_len = len(public_key)
    
    for i in range(0, len(bits), key_len):
        block = bits[i:i+key_len]
        if len(block) < key_len:
            block += [0] * (key_len - len(block))
        
        total = 0
        for j, bit in enumerate(block):
            if bit == 1:
                total += public_key[j]
        ciphertext.append(total)
    
    return ciphertext

def decrypt(ciphertext, private_key, m, n, encoding_type, original_message=None):
    n_inv = mod_inverse(n, m)
    
    all_bits = []
    for value in ciphertext:
        transformed = (value * n_inv) % m
        
        bits = []
        remaining = transformed
        for elem in reversed(private_key):
            if remaining >= elem:
                bits.insert(0, 1)
                remaining -= elem
            else:
                bits.insert(0, 0)
        
        all_bits.extend(bits)
    
    return bits_to_text(all_bits, encoding_type, original_message)

def analyze_time_increase(full_name):
    print("\n" + "="*80)
    print("АНАЛИЗ ВРЕМЕНИ ПРИ УВЕЛИЧЕНИИ ЧЛЕНОВ ПОСЛЕДОВАТЕЛЬНОСТИ")
    print("="*80)
    
    lengths = [8, 10, 12, 14, 16, 18, 20]
    
    for encoding in ["ASCII", "Base64"]:
        print(f"\n--- Кодировка: {encoding} ---")
        print(f"{'Длина':<8} {'Шифрование (сек)':<20} {'Расшифрование (сек)':<20} {'Результат':<10}")
        print("-" * 60)
        
        for length in lengths:
            private_key = generate_superincreasing(length)
            total_sum = sum(private_key)
            m = random.randint(total_sum + 1, total_sum + (1 << 20))
            
            while True:
                n = random.randint(2, m - 1)
                if mod_inverse(n, m) is not None:
                    break
            
            public_key = [(n * elem) % m for elem in private_key]
            
            start_enc = time.perf_counter()
            encrypted = encrypt(full_name, public_key, encoding)
            enc_time = time.perf_counter() - start_enc
            
            start_dec = time.perf_counter()
            decrypted = decrypt(encrypted, private_key, m, n, encoding, full_name)
            dec_time = time.perf_counter() - start_dec
            
            status = "OK" if decrypted == full_name else "FAIL"
            print(f"{length:<8} {enc_time:<20.8f} {dec_time:<20.8f} {status}")

def main():
    print("\nВведите исходное сообщение:")
    full_name = input("> ").strip()
    
    if not full_name:
        full_name = "Иванов Иван Иванович"
        print(f"Используем имя по умолчанию: {full_name}")
    
    print("\n" + "="*80)
    print("РЕЗУЛЬТАТЫ ДЛЯ КОДИРОВКИ ASCII (8 бит)")
    print("="*80)
    
    length = 16
    
    private_key = generate_superincreasing(length)
    total_sum = sum(private_key)
    m = random.randint(total_sum + 1, total_sum + (1 << 20))
    
    while True:
        n = random.randint(2, m - 1)
        if mod_inverse(n, m) is not None:
            break
    
    public_key = [(n * elem) % m for elem in private_key]
    
    print(f"\nТайный ключ (первые 5 элементов): {private_key[:5]}...")
    print(f"Открытый ключ (первые 5 элементов): {public_key[:5]}...")
    print(f"Параметры: m={m}, n={n}")
    
    start_enc = time.perf_counter()
    encrypted = encrypt(full_name, public_key, "ASCII")
    enc_time = time.perf_counter() - start_enc
    
    print(f"\nЗашифрованное сообщение (первые 5 чисел): {encrypted[:5]}...")
    print(f"Время шифрования (ASCII): {enc_time:.8f} сек")
    
    start_dec = time.perf_counter()
    decrypted = decrypt(encrypted, private_key, m, n, "ASCII", full_name)
    dec_time = time.perf_counter() - start_dec
    
    print(f"Расшифрованное сообщение (ASCII): {decrypted}")
    print(f"Время расшифрования (ASCII): {dec_time:.8f} сек")
    
    if decrypted == full_name:
        print("\nРАСШИФРОВАНИЕ ВЫПОЛНЕНО УСПЕШНО!")
    else:
        print("\nОШИБКА РАСШИФРОВАНИЯ!")
    
    print("\n" + "="*80)
    print("РЕЗУЛЬТАТЫ ДЛЯ КОДИРОВКИ BASE64 (6 бит)")
    print("="*80)
    
    private_key2 = generate_superincreasing(length)
    total_sum2 = sum(private_key2)
    m2 = random.randint(total_sum2 + 1, total_sum2 + (1 << 20))
    
    while True:
        n2 = random.randint(2, m2 - 1)
        if mod_inverse(n2, m2) is not None:
            break
    
    public_key2 = [(n2 * elem) % m2 for elem in private_key2]
    
    print(f"\nТайный ключ (первые 5 элементов): {private_key2[:5]}...")
    print(f"Открытый ключ (первые 5 элементов): {public_key2[:5]}...")
    print(f"Параметры: m={m2}, n={n2}")
    
    start_enc = time.perf_counter()
    encrypted2 = encrypt(full_name, public_key2, "Base64")
    enc_time2 = time.perf_counter() - start_enc
    
    print(f"\nЗашифрованное сообщение (первые 5 чисел): {encrypted2[:5]}...")
    print(f"Время шифрования (Base64): {enc_time2:.8f} сек")
    
    start_dec = time.perf_counter()
    decrypted2 = decrypt(encrypted2, private_key2, m2, n2, "Base64", full_name)
    dec_time2 = time.perf_counter() - start_dec
    
    print(f"Расшифрованное сообщение (Base64): {decrypted2}")
    print(f"Время расшифрования (Base64): {dec_time2:.8f} сек")
    
    if decrypted2 == full_name:
        print("\nРАСШИФРОВАНИЕ ВЫПОЛНЕНО УСПЕШНО!")
    else:
        print("\nОШИБКА РАСШИФРОВАНИЯ!")
    
    print("\n" + "="*80)
    print("СРАВНЕНИЕ КОДИРОВОК")
    print("="*80)
    print(f"{'Операция':<20} {'ASCII (8 бит)':<20} {'Base64 (6 бит)':<20} {'Разница':<15}")
    print("-" * 75)
    print(f"{'Шифрование':<20} {enc_time:<20.8f} {enc_time2:<20.8f} {abs(enc_time - enc_time2):<15.8f}")
    print(f"{'Расшифрование':<20} {dec_time:<20.8f} {dec_time2:<20.8f} {abs(dec_time - dec_time2):<15.8f}")
    
    analyze_time_increase(full_name)
    
    print("\n" + "="*80)
    input("Нажмите Enter для выхода...")

if __name__ == "__main__":
    main()