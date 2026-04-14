import random
import time
import math
from typing import List

def is_prime_miller_rabin(n: int, k: int = 40) -> bool:
    if n < 2:
        return False
    small_primes = [2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31]
    for p in small_primes:
        if n % p == 0:
            return n == p
    d = n - 1
    s = 0
    while d % 2 == 0:
        d //= 2
        s += 1
    for _ in range(k):
        a = random.randrange(2, n - 1)
        x = pow(a, d, n)
        if x == 1 or x == n - 1:
            continue
        for _ in range(s - 1):
            x = pow(x, 2, n)
            if x == n - 1:
                break
        else:
            return False
    return True

def generate_large_prime(bits: int, condition_mod4: int = None) -> int:
    while True:
        p = random.getrandbits(bits)
        p |= (1 << (bits - 1)) | 1
        if condition_mod4 is not None and p % 4 != condition_mod4:
            continue
        if is_prime_miller_rabin(p):
            return p

class BBSGenerator:
    def __init__(self, bits_n: int = 256):
        self.bits_n = bits_n
        self.p = None
        self.q = None
        self.n = None
        self.state = None
        self._generate_params()

    def _generate_params(self):
        half_bits = self.bits_n // 2
        print("[BBS] Генерация p и q ...")
        self.p = generate_large_prime(half_bits, condition_mod4=3)
        self.q = generate_large_prime(half_bits, condition_mod4=3)
        self.n = self.p * self.q
        print(f"[BBS] p = {self.p}")
        print(f"[BBS] q = {self.q}")
        print(f"[BBS] n = {self.n} (длина {self.n.bit_length()} бит)")

    def set_seed(self, seed: int = None):
        if seed is None:
            while True:
                seed = random.randrange(2, self.n - 1)
                if math.gcd(seed, self.n) == 1:
                    break
        else:
            if math.gcd(seed, self.n) != 1:
                raise ValueError("seed должен быть взаимно прост с n")
        self.state = seed

    def next_bit(self) -> int:
        self.state = pow(self.state, 2, self.n)
        return self.state & 1

    def generate_bits(self, count: int) -> List[int]:
        return [self.next_bit() for _ in range(count)]

def app1_bbs():
    print("\n========== ПРИЛОЖЕНИЕ 1: Генератор BBS ==========")
    bbs = BBSGenerator(bits_n=256)
    bbs.set_seed()

    bits = bbs.generate_bits(100)
    bits_str = ''.join(str(b) for b in bits)
    print(f"\nПервые 100 бит последовательности:\n{bits_str}")

    print("\nОценка скорости генерации:")
    start = time.perf_counter()
    bbs.generate_bits(10000)
    elapsed = time.perf_counter() - start
    print(f"Генерация 10000 бит заняла {elapsed:.6f} сек.")

class RC4:
    def __init__(self, key: List[int], size: int = 8):
        self.size = size
        self.S = list(range(size))
        self.i = 0
        self.j = 0
        self._ksa(key)

    def _ksa(self, key):
        j = 0
        key_len = len(key)
        for i in range(self.size):
            j = (j + self.S[i] + key[i % key_len]) % self.size
            self.S[i], self.S[j] = self.S[j], self.S[i]

    def _prga(self) -> int:
        self.i = (self.i + 1) % self.size
        self.j = (self.j + self.S[self.i]) % self.size
        self.S[self.i], self.S[self.j] = self.S[self.j], self.S[self.i]
        t = (self.S[self.i] + self.S[self.j]) % self.size
        return self.S[t]

    def encrypt(self, data: bytes) -> bytes:
        cipher = bytearray()
        for byte in data:
            keystream_byte = self._prga()
            cipher.append(byte ^ keystream_byte)
        return bytes(cipher)

    def decrypt(self, data: bytes) -> bytes:
        return self.encrypt(data)

def parse_key_input(input_str: str) -> List[int]:
    if not input_str.strip():
        return [] 
    parts = input_str.replace(',', ' ').split()
    return [int(p) for p in parts]

def app2_rc4():
    print("\n========== ПРИЛОЖЕНИЕ 2: Шифр RC4 ==========")
    default_key = [43, 45, 100, 21, 1]
    default_message = b"Hello, world! This is a test message for RC4 encryption."
    size = 8

    key_input = input(f"Введите ключ (числа через пробел/запятую) [по умолч. {default_key}]: ").strip()
    if key_input:
        try:
            key = parse_key_input(key_input)
            if not key:
                print("Не удалось распознать ключ, используется стандартный.")
                key = default_key
        except ValueError:
            print("Ошибка ввода, используется стандартный ключ.")
            key = default_key
    else:
        key = default_key

    msg_input = input("Введите сообщение для шифрования [по умолч. тестовое]: ").strip()
    if msg_input:
        message = msg_input.encode('utf-8')
    else:
        message = default_message

    print(f"\nКлюч: {key}")
    print(f"Размер состояния (n): {size}")
    print(f"Исходное сообщение: {message}")

    rc4 = RC4(key, size)
    encrypted = rc4.encrypt(message)
    print(f"Зашифрованное (hex): {encrypted.hex()}")

    rc4_dec = RC4(key, size)
    decrypted = rc4_dec.decrypt(encrypted)
    print(f"Расшифрованное: {decrypted}")

    if decrypted == message:
        print("Расшифровка успешна!")
    else:
        print("Ошибка расшифровки!")

    print("\nОценка скорости генерации потока:")
    rc4_speed = RC4(key, size)
    start = time.perf_counter()
    rc4_speed.encrypt(b'\x00' * 10000)
    elapsed = time.perf_counter() - start
    print(f"Генерация 10000 байт ключевого потока заняла {elapsed:.6f} сек.")

def main():
    while True:
        print("\nМеню:")
        print("1 - Генератор ПСП (BBS)")
        print("2 - Шифр RC4")
        print("0 - Выход")
        choice = input("Ваш выбор (0, 1 или 2): ").strip()

        if choice == '1':
            app1_bbs()
        elif choice == '2':
            app2_rc4()
        elif choice == '0':
            print("Выход из программы.")
            break
        else:
            print("Неверный выбор. Пожалуйста, введите 0, 1 или 2.")

if __name__ == "__main__":
    main()