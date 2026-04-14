import base64
import math
from collections import Counter

def encode_to_base64(input_file, output_file):
    """Кодирует файл в base64."""
    with open(input_file, 'rb') as f:
        data = f.read()
    encoded_data = base64.b64encode(data)
    with open(output_file, 'wb') as f:
        f.write(encoded_data)
    return encoded_data

def decode_from_base64(input_file, output_file):
    """Декодирует файл из base64."""
    with open(input_file, 'rb') as f:
        data = f.read()
    decoded_data = base64.b64decode(data)
    with open(output_file, 'wb') as f:
        f.write(decoded_data)
    return decoded_data

def calculate_frequencies(data):
    """Вычисляет частоты символов в данных."""
    if isinstance(data, bytes):
        total = len(data)
        freq = Counter(data)
    else:
        text = data.decode('utf-8') if isinstance(data, bytes) else data
        total = len(text)
        freq = Counter(text)
    return {k: v / total for k, v in freq.items()}

def shannon_entropy(frequencies):
    """Вычисляет энтропию по Шеннону."""
    return -sum(p * math.log2(p) for p in frequencies.values() if p > 0)

def hartley_entropy(alphabet_size):
    """Вычисляет энтропию по Хартли."""
    return math.log2(alphabet_size)

def redundancy(shannon, hartley):
    """Вычисляет избыточность алфавита в процентах."""
    if hartley == 0:
        return 0
    return (1 - (shannon / hartley)) * 100

def bytes_to_binary_string(data):
    """Преобразует байты в строку двоичного представления."""
    return ' '.join(format(byte, '08b') for byte in data)

def xor_buffers(a, b, input_type='ascii'):
    max_len = max(len(a), len(b))
    a_aligned = a.ljust(max_len, b'\0')
    b_aligned = b.ljust(max_len, b'\0')
    
    xor_result = bytes(x ^ y for x, y in zip(a_aligned, b_aligned))
    
    a_binary = bytes_to_binary_string(a_aligned)
    b_binary = bytes_to_binary_string(b_aligned) 
    xor_binary = bytes_to_binary_string(xor_result)
    
    return xor_result, a_binary, b_binary, xor_binary

def main():
    input_file = "input.txt"
    base64_file = "output.b64"
    decoded_file = "decoded.txt"

    try:
        with open(input_file, 'rb') as f:
            original_data = f.read()
        
        base64_data = encode_to_base64(input_file, base64_file)
        
        decoded_data = decode_from_base64(base64_file, decoded_file)
        
    except FileNotFoundError:
        print("Ошибка: файл input.txt не найден. Создайте файл с текстом, например, 'Hello, World!'")
        return

    original_freq = calculate_frequencies(original_data)
    base64_freq = calculate_frequencies(base64_data)

    shannon_orig = shannon_entropy(original_freq)
    hartley_orig = hartley_entropy(256)
    redund_orig = redundancy(shannon_orig, hartley_orig)

    shannon_b64 = shannon_entropy(base64_freq)
    hartley_b64 = hartley_entropy(64)
    redund_b64 = redundancy(shannon_b64, hartley_b64)

    surname = b"Babashinskii"
    name = b"Hleb"
    
    print(f"Размер оригинальных данных: {len(original_data)} байт")
    print(f"Размер base64 данных: {len(base64_data)} байт")
    print(f"Уникальных байтов в оригинале: {len(original_freq)}")
    print(f"Уникальных символов в base64: {len(base64_freq)}")

    print("\n1. Анализ энтропии:")
    print(f"Энтропия Шеннона (оригинал): {shannon_orig:.4f} бит/символ")
    print(f"Энтропия Хартли (оригинал): {hartley_orig:.4f} бит/символ")
    print(f"Избыточность (оригинал): {redund_orig:.2f}%")
    print(f"Энтропия Шеннона (base64): {shannon_b64:.4f} бит/символ")
    print(f"Энтропия Хартли (base64): {hartley_b64:.4f} бит/символ")
    print(f"Избыточность (base64): {redund_b64:.2f}%")
    
    print("\n2. Операция XOR:")
    
    print("\n=== XOR для ASCII представления ===")
    print(f"Фамилия (a): {surname}")
    print(f"Имя (b): {name}")
    
    xor_ascii, a_ascii_bin, b_ascii_bin, xor_ascii_bin = xor_buffers(surname, name, 'ascii')
    
    print(f"Фамилия в двоичном виде: {a_ascii_bin}")
    print(f"Имя в двоичном виде: {b_ascii_bin}")
    print(f"XOR в двоичном виде: {xor_ascii_bin}")
    
    print("\n=== XOR для Base64 представления ===")
    surname_b64 = base64.b64encode(surname)
    name_b64 = base64.b64encode(name)
    
    print(f"Фамилия в base64: {surname_b64}")
    print(f"Имя в base64: {name_b64}")
    
    xor_base64, a_b64_bin, b_b64_bin, xor_b64_bin = xor_buffers(surname_b64, name_b64, 'base64')
    
    print(f"Фамилия base64 в двоичном виде: {a_b64_bin}")
    print(f"Имя base64 в двоичном виде: {b_b64_bin}")
    print(f"XOR в двоичном виде: {xor_b64_bin}")
    
    temp_ascii, _, _, _ = xor_buffers(surname, name, 'ascii')
    result_ascii, _, _, _ = xor_buffers(temp_ascii, name, 'ascii')
    print(f"Для ASCII: a XOR b XOR b == a: {result_ascii[:len(surname)] == surname}")
    
    temp_base64, _, _, _ = xor_buffers(surname_b64, name_b64, 'base64')
    result_base64, _, _, _ = xor_buffers(temp_base64, name_b64, 'base64')
    print(f"Для Base64: a XOR b XOR b == a: {result_base64[:len(surname_b64)] == surname_b64}")

if __name__ == "__main__":
    main()