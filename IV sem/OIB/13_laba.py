import random
import secrets
import hashlib
from cryptography.hazmat.primitives.ciphers import Cipher, algorithms, modes
from cryptography.hazmat.primitives import padding
from cryptography.hazmat.backends import default_backend

# 1. Генерация случайных чисел
print("=== Генерация случайных чисел ===")
print("1. random.random() →", random.random())
print("2. random.randint(1, 100) →", random.randint(1, 100))
print("3. secrets.randbelow(100) →", secrets.randbelow(100))
print("4. random.uniform(10.0, 20.0) →", random.uniform(10.0, 20.0))

# 2. Шифрование и дешифрование фамилии AES-CBC
surname = "Бабашинский".encode('utf-8')

key = secrets.token_bytes(32)  
iv = secrets.token_bytes(16) 

# Шифрование
padder = padding.PKCS7(128).padder()
padded_data = padder.update(surname) + padder.finalize()

cipher = Cipher(algorithms.AES(key), modes.CBC(iv), backend=default_backend())
encryptor = cipher.encryptor()
encrypted = encryptor.update(padded_data) + encryptor.finalize()

# Дешифрование
cipher = Cipher(algorithms.AES(key), modes.CBC(iv), backend=default_backend())
decryptor = cipher.decryptor()
decrypted_padded = decryptor.update(encrypted) + decryptor.finalize()

unpadder = padding.PKCS7(128).unpadder()
decrypted = unpadder.update(decrypted_padded) + unpadder.finalize()

print("\n=== Шифрование AES-256-CBC ===")
print("Исходная фамилия:", surname.decode('utf-8'))
print("Ключ (hex):", key.hex())
print("IV (hex):", iv.hex())
print("Зашифрованные данные (hex):", encrypted.hex())
print("\n=== Дешифрование ===")
print("Расшифрованная фамилия:", decrypted.decode('utf-8'))

# 3. Хеширование фамилии SHA-384
hash_sha384 = hashlib.sha384(surname).hexdigest()

print("\n=== Хеширование SHA-384 ===")
print("Фамилия:", surname.decode('utf-8'))
print("SHA-384 хеш:", hash_sha384)