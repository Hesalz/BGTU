import string
import time
import collections
import matplotlib.pyplot as plt

class CaesarCipherWithKeyword:
    def __init__(self, keyword):
        keyword = keyword.upper()
        seen = set()
        unique_key = []
        for ch in keyword:
            if ch not in seen and ch in string.ascii_uppercase:
                seen.add(ch)
                unique_key.append(ch)
        alphabet = string.ascii_uppercase
        remaining = [ch for ch in alphabet if ch not in seen]
        self.cipher_alphabet = unique_key + remaining 
        self.enc_map = {alphabet[i]: self.cipher_alphabet[i] for i in range(26)}
        self.dec_map = {v: k for k, v in self.enc_map.items()}

    def _transform(self, text, mapping):
        result = []
        for ch in text:
            if ch.isalpha():
                upper = ch.upper()
                if upper in mapping:
                    transformed = mapping[upper]
                    if ch.islower():
                        transformed = transformed.lower()
                    result.append(transformed)
                else:
                    result.append(ch) 
            else:
                result.append(ch)
        return ''.join(result)

    def encrypt(self, text):
        return self._transform(text, self.enc_map)

    def decrypt(self, text):
        return self._transform(text, self.dec_map)


class TrisemusTableCipher:
    def __init__(self, keyword):
        self.alphabet = []
        for ch in string.ascii_uppercase:
            if ch != 'J':
                self.alphabet.append(ch)
        keyword = keyword.upper().replace('J', 'I')
        seen = set()
        unique_key = []
        for ch in keyword:
            if ch in self.alphabet and ch not in seen:
                seen.add(ch)
                unique_key.append(ch)
        remaining = [ch for ch in self.alphabet if ch not in seen]
        table = []
        for i in range(0, 25, 5):
            row = (unique_key + remaining)[i:i+5]
            table.append(row)
        self.table = table
        self.coord = {}
        for r in range(5):
            for c in range(5):
                self.coord[self.table[r][c]] = (r, c)

    def _encrypt_char(self, ch):
        if not ch.isalpha():
            return ch
        upper = ch.upper()
        if upper == 'J':
            upper = 'I'
        if upper not in self.coord:
            return ch
        r, c = self.coord[upper]
        new_c = (c + 1) % 5
        new_ch = self.table[r][new_c]
        if ch.islower():
            new_ch = new_ch.lower()
        return new_ch

    def _decrypt_char(self, ch):
        if not ch.isalpha():
            return ch
        upper = ch.upper()
        if upper == 'J':
            upper = 'I'
        if upper not in self.coord:
            return ch
        r, c = self.coord[upper]
        new_c = (c - 1) % 5
        new_ch = self.table[r][new_c]
        if ch.islower():
            new_ch = new_ch.lower()
        return new_ch

    def encrypt(self, text):
        return ''.join(self._encrypt_char(ch) for ch in text)

    def decrypt(self, text):
        return ''.join(self._decrypt_char(ch) for ch in text)

def plot_histogram(original_text, encrypted_text, title):
    orig_counter = collections.Counter(original_text)
    enc_counter = collections.Counter(encrypted_text)

    all_chars = sorted(set(orig_counter.keys()) | set(enc_counter.keys()))

    orig_total = len(original_text)
    enc_total = len(encrypted_text)
    orig_freq = [orig_counter.get(ch, 0) / orig_total * 100 for ch in all_chars]
    enc_freq = [enc_counter.get(ch, 0) / enc_total * 100 for ch in all_chars]

    x = range(len(all_chars))
    width = 0.4

    fig, ax = plt.subplots(figsize=(12, 6))
    ax.bar([i - width/2 for i in x], orig_freq, width, label='Исходный текст', alpha=0.7)
    ax.bar([i + width/2 for i in x], enc_freq, width, label='Зашифрованный текст', alpha=0.7)

    ax.set_xlabel('Символы')
    ax.set_ylabel('Частота (%)')
    ax.set_title(title)
    ax.set_xticks(x)
    ax.set_xticklabels(all_chars, rotation=90)
    ax.legend()

    plt.tight_layout()
    plt.show()

def main():
    input_filename = "input.txt"
    try:
        with open(input_filename, 'r', encoding='utf-8') as f:
            plaintext = f.read()
    except FileNotFoundError:
        print(f"Файл {input_filename} не найден. Создайте его с текстом не менее 5000 знаков.")
        return

    if len(plaintext) < 5000:
        print(f"Предупреждение: длина текста {len(plaintext)} < 5000. Рекомендуется использовать текст большего объёма.")

    caesar_cipher = CaesarCipherWithKeyword("BABASHINSKII")
    trisemus_cipher = TrisemusTableCipher("HLEB")

    print("Шифр Цезаря с ключевым словом:")
    start = time.perf_counter()
    caesar_enc = caesar_cipher.encrypt(plaintext)
    enc_time = time.perf_counter() - start
    print(f"Время шифрования: {enc_time:.6f} с")

    start = time.perf_counter()
    caesar_dec = caesar_cipher.decrypt(caesar_enc)
    dec_time = time.perf_counter() - start
    print(f"Время расшифрования: {dec_time:.6f} с")

    with open("caesar_encrypted.txt", 'w', encoding='utf-8') as f:
        f.write(caesar_enc)
    with open("caesar_decrypted.txt", 'w', encoding='utf-8') as f:
        f.write(caesar_dec)

    plot_histogram(plaintext, caesar_enc, "Шифр Цезаря с ключевым словом")

    print("\nТаблица Трисемуса:")
    start = time.perf_counter()
    trisemus_enc = trisemus_cipher.encrypt(plaintext)
    enc_time = time.perf_counter() - start
    print(f"Время шифрования: {enc_time:.6f} с")

    start = time.perf_counter()
    trisemus_dec = trisemus_cipher.decrypt(trisemus_enc)
    dec_time = time.perf_counter() - start
    print(f"Время расшифрования: {dec_time:.6f} с")

    with open("trisemus_encrypted.txt", 'w', encoding='utf-8') as f:
        f.write(trisemus_enc)
    with open("trisemus_decrypted.txt", 'w', encoding='utf-8') as f:
        f.write(trisemus_dec)

    plot_histogram(plaintext, trisemus_enc, "Таблица Трисемуса")

    if plaintext == caesar_dec:
        print("\nЦезарь: расшифрование успешно.")
    else:
        print("\nЦезарь: расшифрование не совпало с исходным текстом!")
    modified_plain = plaintext.replace('J', 'I').replace('j', 'i')
    if modified_plain == trisemus_dec:
        print("Трисемус: расшифрование успешно")
    else:
        print("Трисемус: расшифрование не совпало с ожидаемым!")


if __name__ == "__main__":
    main()