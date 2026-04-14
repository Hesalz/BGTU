from Cryptodome.Cipher import DES3
from Cryptodome.Hash import SHA384
from Cryptodome.Random import get_random_bytes
import binascii

def pad_data(data, block_size=8):
    padding_length = block_size - (len(data) % block_size)
    return data + bytes([padding_length] * padding_length)

def unpad_data(data):
    padding_length = data[-1]
    return data[:-padding_length]

# 1. Шифрование и дешифрование TripleDES
def encrypt_decrypt_demo():
    surname = "Бабашинский".encode('utf-8')

    key = get_random_bytes(16)
    iv = get_random_bytes(8)

    cipher = DES3.new(key, DES3.MODE_CBC, iv)
    padded_data = pad_data(surname)
    encrypted = cipher.encrypt(padded_data)

    decipher = DES3.new(key, DES3.MODE_CBC, iv)
    decrypted_padded = decipher.decrypt(encrypted)
    decrypted = unpad_data(decrypted_padded).decode('utf-8')

    print(f"Исходная фамилия: {surname.decode('utf-8')}")
    print(f"Ключ (hex): {binascii.hexlify(key).decode('utf-8')}")
    print(f"IV (hex): {binascii.hexlify(iv).decode('utf-8')}")
    print(f"Зашифрованные данные (hex): {binascii.hexlify(encrypted).decode('utf-8')}")
    print(f"Расшифрованная фамилия: {decrypted}")

    with open("key.bin", "wb") as f:
        f.write(key)
    with open("iv.bin", "wb") as f:
        f.write(iv)
    with open("encrypted.bin", "wb") as f:
        f.write(encrypted)

# 2. Хеширование SHA-384
def hash_demo():
    surname = "Бабашинский".encode('utf-8')
    hash_obj = SHA384.new(surname)
    hash_hex = hash_obj.hexdigest()

    print(f"\nХеш SHA-384 фамилии '{surname.decode('utf-8')}':")
    print(hash_hex)

    with open("hash.bin", "wb") as f:
        f.write(binascii.unhexlify(hash_hex))

# 3. Проверка подписи (аналог ЭЦП)
def verify_signature_demo():
    original_message = "Бабашинский"
    fake_message = "Бабашиннский"

    original_hash = SHA384.new(original_message.encode('utf-8')).hexdigest()

    print(f"\nПроверка подписи (оригинал): {original_hash}")

    fake_hash = SHA384.new(fake_message.encode('utf-8')).hexdigest()
    print(f"Проверка подписи (подделка): {fake_hash} \n{'✓ OK' if fake_hash == original_hash else '✗ FAIL'}")

if __name__ == "__main__":
    encrypt_decrypt_demo()
    hash_demo()
    verify_signature_demo()