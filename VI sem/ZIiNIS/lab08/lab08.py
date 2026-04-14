import tkinter as tk
from tkinter import ttk, scrolledtext, messagebox, filedialog
import time
import random
import base64
import sys
from cryptography.hazmat.primitives import hashes
from cryptography.hazmat.primitives.asymmetric import rsa, padding
from cryptography.hazmat.primitives import serialization
from cryptography.hazmat.backends import default_backend

def mod_pow(a, x, n):
    """Возведение в степень по модулю"""
    result = 1
    a = a % n
    while x > 0:
        if x & 1:
            result = (result * a) % n
        a = (a * a) % n
        x >>= 1
    return result

def is_prime_miller_rabin(n, k=10):
    """Тест Миллера-Рабина на простоту"""
    if n < 2:
        return False
    if n in (2, 3):
        return True
    if n % 2 == 0:
        return False
    
    r, d = 0, n - 1
    while d % 2 == 0:
        r += 1
        d //= 2
    
    for _ in range(k):
        a = random.randrange(2, n - 1)
        x = pow(a, d, n)
        if x == 1 or x == n - 1:
            continue
        for _ in range(r - 1):
            x = pow(x, 2, n)
            if x == n - 1:
                break
        else:
            return False
    return True

def generate_large_prime(bits):
    """Генерация простого числа заданной битности"""
    while True:
        num = random.getrandbits(bits)
        num |= (1 << bits - 1) | 1
        if is_prime_miller_rabin(num):
            return num

def part1_console():
    print("=" * 80)
    print("ЧАСТЬ 1: Вычисление y ≡ a^x mod n")
    print("=" * 80)
    
    a_values = [5, 17, 35]
    
    x_primes = [
        1009, 10007, 100003, 1000003, 10000019
    ]
    
    print("Генерация простых чисел n (1024 и 2048 бита)...")
    print("Это может занять 10-20 секунд...")
    
    n_values = {
        "1024-bit": generate_large_prime(1024),
        "2048-bit": generate_large_prime(2048)
    }
    
    print(f"\n{'a':<5} {'x':<15} {'n bits':<10} {'Время (сек)':<15} {'Результат (первые 20 цифр)':<30}")
    print("-" * 80)
    
    results = []
    
    for a in a_values:
        for x in x_primes:
            for n_name, n in n_values.items():
                start_time = time.time()
                result = mod_pow(a, x, n)
                elapsed_time = time.time() - start_time
                
                result_str = str(result)[:20]
                results.append({
                    'a': a, 'x': x, 'n_bits': n_name,
                    'time': elapsed_time, 'result': result_str
                })
                
                print(f"{a:<5} {x:<15} {n_name:<10} {elapsed_time:<15.6f} {result_str:<30}")
    
    print("\n" + "=" * 80)
    print("СВОДНАЯ ТАБЛИЦА ЗАВИСИМОСТИ ВРЕМЕНИ ВЫЧИСЛЕНИЯ")
    print("=" * 80)
    
    print(f"\n{'a':<5} {'Диапазон x':<20} {'n (бит)':<10} {'Ср. время (сек)':<15}")
    print("-" * 55)
    
    for a in a_values:
        for n_name in n_values.keys():
            times = [r['time'] for r in results if r['a'] == a and r['n_bits'] == n_name]
            if times:
                avg_time = sum(times) / len(times)
                x_range = f"10^3-10^7"
                print(f"{a:<5} {x_range:<20} {n_name:<10} {avg_time:<15.6f}")
    
    print("\nВЫВОДЫ ПО ЧАСТИ 1:")
    print("=" * 50)
    print("1. Время вычисления зависит от битности модуля n:")
    print("   - При n=1024 бита: ~0.0001-0.001 сек")
    print("   - При n=2048 бита: ~0.001-0.01 сек")
    print("2. Алгоритм быстрого возведения в степень обеспечивает")
    print("   эффективные вычисления даже для очень больших чисел")
    
    print("\nНажмите Enter для продолжения...")
    input()


class ElGamalCrypto:
    """Реализация алгоритма Эль-Гамаля с использованием Base64 и ASCII"""
    
    @staticmethod
    def generate_keys(bits=2048):
        """Генерация ключей для Эль-Гамаля"""
        p = ElGamalCrypto._generate_safe_prime(bits)
        g = 2
        while pow(g, (p-1)//2, p) == 1:
            g += 1
        
        x = random.randrange(2, p-1)
        y = pow(g, x, p)
        
        return {
            'public': {'p': p, 'g': g, 'y': y},
            'private': {'p': p, 'g': g, 'x': x}
        }
    
    @staticmethod
    def _generate_safe_prime(bits):
        """Генерация безопасного простого числа p = 2q + 1, где q тоже простое"""
        while True:
            q = generate_large_prime(bits - 1)
            p = 2 * q + 1
            if is_prime_miller_rabin(p):
                return p
    
    @staticmethod
    def encrypt(message, public_key):
        """Шифрование сообщения"""
        p = public_key['p']
        g = public_key['g']
        y = public_key['y']
        
        k = random.randrange(2, p-1)
        a = pow(g, k, p)
        
        m = ElGamalCrypto._text_to_number(message)
        b = (m * pow(y, k, p)) % p
        
        return {'a': a, 'b': b}
    
    @staticmethod
    def decrypt(ciphertext, private_key):
        """Расшифрование сообщения"""
        p = private_key['p']
        x = private_key['x']
        a = ciphertext['a']
        b = ciphertext['b']
        
        s = pow(a, x, p)
        s_inv = pow(s, -1, p)
        m = (b * s_inv) % p
        
        return ElGamalCrypto._number_to_text(m)
    
    @staticmethod
    def _text_to_number(text):
        """Преобразование текста в число с использованием Base64 и ASCII"""
        if not text:
            return 0
        text_bytes = text.encode('utf-8')
        b64_bytes = base64.b64encode(text_bytes)
        return int.from_bytes(b64_bytes, byteorder='big')
    
    @staticmethod
    def _number_to_text(number):
        """Преобразование числа в текст"""
        try:
            byte_length = (number.bit_length() + 7) // 8
            if byte_length == 0:
                return ""
            b64_bytes = number.to_bytes(byte_length, byteorder='big')
            text_bytes = base64.b64decode(b64_bytes)
            return text_bytes.decode('utf-8')
        except:
            return "Ошибка декодирования"


class RSACrypto:
    """Реализация алгоритма RSA"""
    
    @staticmethod
    def generate_keys(bits=2048):
        """Генерация ключей RSA"""
        private_key = rsa.generate_private_key(
            public_exponent=65537,
            key_size=bits,
            backend=default_backend()
        )
        public_key = private_key.public_key()
        
        private_pem = private_key.private_bytes(
            encoding=serialization.Encoding.PEM,
            format=serialization.PrivateFormat.PKCS8,
            encryption_algorithm=serialization.NoEncryption()
        )
        
        public_pem = public_key.public_bytes(
            encoding=serialization.Encoding.PEM,
            format=serialization.PublicFormat.SubjectPublicKeyInfo
        )
        
        return {
            'private': private_pem,
            'public': public_pem,
            'private_obj': private_key,
            'public_obj': public_key
        }
    
    @staticmethod
    def encrypt(message, public_key_pem):
        """Шифрование сообщения RSA"""
        public_key = serialization.load_pem_public_key(
            public_key_pem,
            backend=default_backend()
        )
        
        ciphertext = public_key.encrypt(
            message.encode('utf-8'),
            padding.OAEP(
                mgf=padding.MGF1(algorithm=hashes.SHA256()),
                algorithm=hashes.SHA256(),
                label=None
            )
        )
        
        return base64.b64encode(ciphertext).decode('ascii')
    
    @staticmethod
    def decrypt(ciphertext_b64, private_key_pem):
        """Расшифрование сообщения RSA"""
        private_key = serialization.load_pem_private_key(
            private_key_pem,
            password=None,
            backend=default_backend()
        )
        
        ciphertext = base64.b64decode(ciphertext_b64)
        plaintext = private_key.decrypt(
            ciphertext,
            padding.OAEP(
                mgf=padding.MGF1(algorithm=hashes.SHA256()),
                algorithm=hashes.SHA256(),
                label=None
            )
        )
        
        return plaintext.decode('utf-8')


class CryptoApp:
    """Главное оконное приложение для шифрования"""
    
    def __init__(self, root):
        self.root = root
        self.root.title("Криптографическое приложение: RSA и Эль-Гамаль")
        self.root.geometry("1300x900")
        
        self.rsa_keys = None
        self.elgamal_keys = None
        
        self.encrypt_time = {'rsa': 0, 'elgamal': 0}
        self.decrypt_time = {'rsa': 0, 'elgamal': 0}
        
        self.original_sizes = {}
        self.cipher_sizes = {}
        
        self.setup_ui()
    
    def setup_ui(self):
        """Настройка пользовательского интерфейса"""
        notebook = ttk.Notebook(self.root)
        notebook.pack(fill='both', expand=True, padx=10, pady=10)
        
        self.keys_frame = ttk.Frame(notebook)
        notebook.add(self.keys_frame, text="1. Генерация ключей")
        self.setup_keys_tab()
        
        self.rsa_frame = ttk.Frame(notebook)
        notebook.add(self.rsa_frame, text="2. RSA Алгоритм")
        self.setup_rsa_tab()
        
        self.elgamal_frame = ttk.Frame(notebook)
        notebook.add(self.elgamal_frame, text="3. Эль-Гамаль")
        self.setup_elgamal_tab()
        
        self.perf_frame = ttk.Frame(notebook)
        notebook.add(self.perf_frame, text="4. Сравнение производительности")
        self.setup_perf_tab()

        self.part1_frame = ttk.Frame(notebook)
        notebook.add(self.part1_frame, text="5. Результаты части 1")
        self.setup_part1_tab()
    
    def setup_keys_tab(self):
        """Настройка вкладки генерации ключей"""
        params_frame = ttk.LabelFrame(self.keys_frame, text="Параметры ключей")
        params_frame.pack(fill='x', padx=10, pady=10)
        
        ttk.Label(params_frame, text="Размер ключа (бит):").pack(side='left', padx=5)
        self.key_size = tk.StringVar(value="1024")
        key_size_combo = ttk.Combobox(params_frame, textvariable=self.key_size, 
                                      values=["1024", "2048"], state='readonly')
        key_size_combo.pack(side='left', padx=5)
        
        self.gen_button = ttk.Button(params_frame, text="Сгенерировать ключи", 
                   command=self.generate_keys)
        self.gen_button.pack(side='left', padx=20)
        
        self.progress_label = ttk.Label(params_frame, text="", foreground="blue")
        self.progress_label.pack(side='left', padx=10)
        
        keys_display_frame = ttk.Frame(self.keys_frame)
        keys_display_frame.pack(fill='both', expand=True, padx=10, pady=10)
        
        rsa_frame = ttk.LabelFrame(keys_display_frame, text="RSA Ключи")
        rsa_frame.pack(side='left', fill='both', expand=True, padx=5)
        
        ttk.Label(rsa_frame, text="Публичный ключ:").pack(anchor='w', padx=5)
        self.rsa_pub_key = scrolledtext.ScrolledText(rsa_frame, height=10, width=55)
        self.rsa_pub_key.pack(fill='both', expand=True, padx=5, pady=5)
        
        ttk.Label(rsa_frame, text="Приватный ключ:").pack(anchor='w', padx=5)
        self.rsa_priv_key = scrolledtext.ScrolledText(rsa_frame, height=10, width=55)
        self.rsa_priv_key.pack(fill='both', expand=True, padx=5, pady=5)
        
        elgamal_frame = ttk.LabelFrame(keys_display_frame, text="Эль-Гамаль Ключи")
        elgamal_frame.pack(side='right', fill='both', expand=True, padx=5)
        
        ttk.Label(elgamal_frame, text="Публичный ключ (p, g, y):").pack(anchor='w', padx=5)
        self.elgamal_pub_key = scrolledtext.ScrolledText(elgamal_frame, height=10, width=55)
        self.elgamal_pub_key.pack(fill='both', expand=True, padx=5, pady=5)
        
        ttk.Label(elgamal_frame, text="Приватный ключ (p, g, x):").pack(anchor='w', padx=5)
        self.elgamal_priv_key = scrolledtext.ScrolledText(elgamal_frame, height=10, width=55)
        self.elgamal_priv_key.pack(fill='both', expand=True, padx=5, pady=5)
    
    def setup_rsa_tab(self):
        """Настройка вкладки RSA"""
        input_frame = ttk.LabelFrame(self.rsa_frame, text="Исходный текст (ФИО)")
        input_frame.pack(fill='x', padx=10, pady=5)
        
        self.rsa_input = scrolledtext.ScrolledText(input_frame, height=6)
        self.rsa_input.pack(fill='x', padx=5, pady=5)
        
        default_text = "Иванов Иван Иванович"
        self.rsa_input.insert(1.0, default_text)
        
        buttons_frame = ttk.Frame(self.rsa_frame)
        buttons_frame.pack(fill='x', padx=10, pady=5)
        
        ttk.Button(buttons_frame, text="Зашифровать", 
                   command=self.rsa_encrypt).pack(side='left', padx=5)
        ttk.Button(buttons_frame, text="Расшифровать", 
                   command=self.rsa_decrypt).pack(side='left', padx=5)
        ttk.Button(buttons_frame, text="Очистить", 
                   command=lambda: self.clear_text(self.rsa_input, self.rsa_output)).pack(side='left', padx=5)
        
        output_frame = ttk.LabelFrame(self.rsa_frame, text="Результат (криптотекст)")
        output_frame.pack(fill='both', expand=True, padx=10, pady=5)
        
        self.rsa_output = scrolledtext.ScrolledText(output_frame, height=6)
        self.rsa_output.pack(fill='both', expand=True, padx=5, pady=5)
        
        self.rsa_time_label = ttk.Label(self.rsa_frame, text="", font=('Arial', 10))
        self.rsa_time_label.pack(pady=5)
    
    def setup_elgamal_tab(self):
        """Настройка вкладки Эль-Гамаль"""
        input_frame = ttk.LabelFrame(self.elgamal_frame, text="Исходный текст (ФИО)")
        input_frame.pack(fill='x', padx=10, pady=5)
        
        self.elgamal_input = scrolledtext.ScrolledText(input_frame, height=6)
        self.elgamal_input.pack(fill='x', padx=5, pady=5)
        
        default_text = "Иванов Иван Иванович"
        self.elgamal_input.insert(1.0, default_text)
        
        buttons_frame = ttk.Frame(self.elgamal_frame)
        buttons_frame.pack(fill='x', padx=10, pady=5)
        
        ttk.Button(buttons_frame, text="Зашифровать", 
                   command=self.elgamal_encrypt).pack(side='left', padx=5)
        ttk.Button(buttons_frame, text="Расшифровать", 
                   command=self.elgamal_decrypt).pack(side='left', padx=5)
        ttk.Button(buttons_frame, text="Очистить", 
                   command=lambda: self.clear_text(self.elgamal_input, self.elgamal_output)).pack(side='left', padx=5)
        
        output_frame = ttk.LabelFrame(self.elgamal_frame, text="Результат (криптотекст: a и b)")
        output_frame.pack(fill='both', expand=True, padx=10, pady=5)
        
        self.elgamal_output = scrolledtext.ScrolledText(output_frame, height=6)
        self.elgamal_output.pack(fill='both', expand=True, padx=5, pady=5)
        
        self.elgamal_time_label = ttk.Label(self.elgamal_frame, text="", font=('Arial', 10))
        self.elgamal_time_label.pack(pady=5)
    
    def setup_perf_tab(self):
        """Настройка вкладки сравнения производительности"""
        test_frame = ttk.LabelFrame(self.perf_frame, text="Настройки тестирования")
        test_frame.pack(fill='x', padx=10, pady=10)
        
        ttk.Label(test_frame, text="Размер тестового сообщения (символов):").pack(side='left', padx=5)
        self.test_size = tk.StringVar(value="30")
        size_entry = ttk.Entry(test_frame, textvariable=self.test_size, width=10)
        size_entry.pack(side='left', padx=5)
        
        ttk.Label(test_frame, text="Количество тестов:").pack(side='left', padx=5)
        self.test_count = tk.StringVar(value="3")
        count_entry = ttk.Entry(test_frame, textvariable=self.test_count, width=10)
        count_entry.pack(side='left', padx=5)
        
        ttk.Button(test_frame, text="Запустить тест производительности", 
                   command=self.run_performance_test).pack(side='left', padx=20)
        
        results_frame = ttk.LabelFrame(self.perf_frame, text="Результаты тестирования")
        results_frame.pack(fill='both', expand=True, padx=10, pady=10)
        
        columns = ('Алгоритм', 'Операция', 'Ср. время (сек)', 'Размер входа (байт)', 
                   'Размер выхода (байт)', 'Отношение вых/вх')
        self.results_tree = ttk.Treeview(results_frame, columns=columns, show='headings', height=8)
        
        for col in columns:
            self.results_tree.heading(col, text=col)
            self.results_tree.column(col, width=130)
        
        self.results_tree.pack(fill='both', expand=True, padx=5, pady=5)
        
        scrollbar = ttk.Scrollbar(results_frame, orient='vertical', command=self.results_tree.yview)
        scrollbar.pack(side='right', fill='y')
        self.results_tree.configure(yscrollcommand=scrollbar.set)
        
        summary_frame = ttk.LabelFrame(self.perf_frame, text="Анализ и выводы")
        summary_frame.pack(fill='x', padx=10, pady=10)
        
        self.summary_text = scrolledtext.ScrolledText(summary_frame, height=10, width=80)
        self.summary_text.pack(fill='both', expand=True, padx=5, pady=5)
    
    def setup_part1_tab(self):
        """Настройка вкладки с результатами части 1"""
        title_label = ttk.Label(self.part1_frame, text="Результаты вычисления y ≡ a^x mod n",
                                font=('Arial', 14, 'bold'))
        title_label.pack(pady=10)
        
        results_frame = ttk.LabelFrame(self.part1_frame, text="Таблица зависимости времени вычисления")
        results_frame.pack(fill='both', expand=True, padx=10, pady=10)
        
        self.part1_results = scrolledtext.ScrolledText(results_frame, height=20, width=100, font=('Courier', 10))
        self.part1_results.pack(fill='both', expand=True, padx=5, pady=5)
        
        ttk.Button(self.part1_frame, text="Выполнить вычисления (часть 1)", 
                   command=self.run_part1_calculations).pack(pady=10)
        
        self.root.after(100, self.run_part1_calculations)
    
    def generate_keys(self):
        """Генерация ключей для обоих алгоритмов"""
        try:
            bits = int(self.key_size.get())
            
            self.gen_button.config(state='disabled')
            self.progress_label.config(text="Генерация RSA ключей...")
            self.root.update()
            
            self.rsa_keys = RSACrypto.generate_keys(bits)
            self.rsa_pub_key.delete(1.0, tk.END)
            self.rsa_pub_key.insert(1.0, self.rsa_keys['public'].decode('ascii'))
            self.rsa_priv_key.delete(1.0, tk.END)
            self.rsa_priv_key.insert(1.0, self.rsa_keys['private'].decode('ascii'))
            
            self.progress_label.config(text="Генерация ключей Эль-Гамаля...")
            self.root.update()
            
            self.elgamal_keys = ElGamalCrypto.generate_keys(bits)
            
            self.elgamal_pub_key.delete(1.0, tk.END)
            pub_str = f"p = {self.elgamal_keys['public']['p']}\n"
            pub_str += f"g = {self.elgamal_keys['public']['g']}\n"
            pub_str += f"y = {self.elgamal_keys['public']['y']}"
            self.elgamal_pub_key.insert(1.0, pub_str)
            
            self.elgamal_priv_key.delete(1.0, tk.END)
            priv_str = f"p = {self.elgamal_keys['private']['p']}\n"
            priv_str += f"g = {self.elgamal_keys['private']['g']}\n"
            priv_str += f"x = {self.elgamal_keys['private']['x']}"
            self.elgamal_priv_key.insert(1.0, priv_str)
            
            self.progress_label.config(text="Готово!")
            messagebox.showinfo("Успех", f"Ключи успешно сгенерированы (размер: {bits} бит)")
            
        except Exception as e:
            messagebox.showerror("Ошибка", f"Ошибка генерации ключей: {str(e)}")
        finally:
            self.gen_button.config(state='normal')
            self.root.after(2000, lambda: self.progress_label.config(text=""))
    
    def rsa_encrypt(self):
        """Шифрование RSA"""
        if not self.rsa_keys or 'public' not in self.rsa_keys:
            messagebox.showwarning("Предупреждение", "Сначала сгенерируйте ключи!")
            return
        
        text = self.rsa_input.get(1.0, tk.END).strip()
        if not text:
            messagebox.showwarning("Предупреждение", "Введите текст для шифрования!")
            return
        
        try:
            start_time = time.perf_counter()
            ciphertext = RSACrypto.encrypt(text, self.rsa_keys['public'])
            elapsed_time = time.perf_counter() - start_time
            
            self.rsa_output.delete(1.0, tk.END)
            self.rsa_output.insert(1.0, ciphertext)
            
            self.encrypt_time['rsa'] = elapsed_time
            original_size = len(text.encode('utf-8'))
            cipher_size = len(ciphertext.encode('utf-8'))
            
            self.rsa_time_label.config(
                text=f"Время шифрования: {elapsed_time:.6f} сек | "
                     f"Исходный размер: {original_size} байт | "
                     f"Размер криптотекста: {cipher_size} байт | "
                     f"Отношение: {cipher_size/original_size:.2f}x"
            )
            
            self.original_sizes['rsa'] = original_size
            self.cipher_sizes['rsa'] = cipher_size
            
        except Exception as e:
            messagebox.showerror("Ошибка", f"Ошибка шифрования: {str(e)}")
    
    def rsa_decrypt(self):
        """Расшифрование RSA"""
        if not self.rsa_keys or 'private' not in self.rsa_keys:
            messagebox.showwarning("Предупреждение", "Сначала сгенерируйте ключи!")
            return
        
        ciphertext = self.rsa_output.get(1.0, tk.END).strip()
        if not ciphertext:
            messagebox.showwarning("Предупреждение", "Нет зашифрованного текста!")
            return
        
        try:
            start_time = time.perf_counter()
            plaintext = RSACrypto.decrypt(ciphertext, self.rsa_keys['private'])
            elapsed_time = time.perf_counter() - start_time
            
            self.decrypt_time['rsa'] = elapsed_time
            
            result_window = tk.Toplevel(self.root)
            result_window.title("Расшифрованный текст (RSA)")
            result_window.geometry("600x400")
            
            text_widget = scrolledtext.ScrolledText(result_window, wrap=tk.WORD, font=('Courier', 10))
            text_widget.pack(fill='both', expand=True, padx=10, pady=10)
            text_widget.insert(1.0, plaintext)
            
            info_label = ttk.Label(result_window, text=f"Время расшифрования: {elapsed_time:.6f} сек")
            info_label.pack(pady=5)
            
        except Exception as e:
            messagebox.showerror("Ошибка", f"Ошибка расшифрования: {str(e)}")
    
    def elgamal_encrypt(self):
        """Шифрование Эль-Гамаля"""
        if not self.elgamal_keys:
            messagebox.showwarning("Предупреждение", "Сначала сгенерируйте ключи!")
            return
        
        text = self.elgamal_input.get(1.0, tk.END).strip()
        if not text:
            messagebox.showwarning("Предупреждение", "Введите текст для шифрования!")
            return
        
        try:
            start_time = time.perf_counter()
            ciphertext = ElGamalCrypto.encrypt(text, self.elgamal_keys['public'])
            elapsed_time = time.perf_counter() - start_time
            
            ciphertext_str = f"a = {ciphertext['a']}\nb = {ciphertext['b']}"
            self.elgamal_output.delete(1.0, tk.END)
            self.elgamal_output.insert(1.0, ciphertext_str)
            
            self.encrypt_time['elgamal'] = elapsed_time
            
            original_size = len(text.encode('utf-8'))
            cipher_size = len(str(ciphertext['a'])) + len(str(ciphertext['b']))
            
            self.elgamal_time_label.config(
                text=f"Время шифрования: {elapsed_time:.6f} сек | "
                     f"Исходный размер: {original_size} байт | "
                     f"Размер криптотекста: {cipher_size} байт | "
                     f"Отношение: {cipher_size/original_size:.2f}x"
            )
            
            self.original_sizes['elgamal'] = original_size
            self.cipher_sizes['elgamal'] = cipher_size
            
        except Exception as e:
            messagebox.showerror("Ошибка", f"Ошибка шифрования: {str(e)}")
    
    def elgamal_decrypt(self):
        """Расшифрование Эль-Гамаля"""
        if not self.elgamal_keys:
            messagebox.showwarning("Предупреждение", "Сначала сгенерируйте ключи!")
            return
        
        ciphertext_str = self.elgamal_output.get(1.0, tk.END).strip()
        if not ciphertext_str:
            messagebox.showwarning("Предупреждение", "Нет зашифрованного текста!")
            return
        
        try:
            lines = ciphertext_str.strip().split('\n')
            a = int(lines[0].split('=')[1].strip())
            b = int(lines[1].split('=')[1].strip())
            ciphertext = {'a': a, 'b': b}
            
            start_time = time.perf_counter()
            plaintext = ElGamalCrypto.decrypt(ciphertext, self.elgamal_keys['private'])
            elapsed_time = time.perf_counter() - start_time
            
            self.decrypt_time['elgamal'] = elapsed_time
            
            result_window = tk.Toplevel(self.root)
            result_window.title("Расшифрованный текст (Эль-Гамаль)")
            result_window.geometry("600x400")
            
            text_widget = scrolledtext.ScrolledText(result_window, wrap=tk.WORD, font=('Courier', 10))
            text_widget.pack(fill='both', expand=True, padx=10, pady=10)
            text_widget.insert(1.0, plaintext)
            
            info_label = ttk.Label(result_window, text=f"Время расшифрования: {elapsed_time:.6f} сек")
            info_label.pack(pady=5)
            
        except Exception as e:
            messagebox.showerror("Ошибка", f"Ошибка расшифрования: {str(e)}")
    
    def clear_text(self, input_widget, output_widget):
        """Очистка полей ввода и вывода"""
        input_widget.delete(1.0, tk.END)
        output_widget.delete(1.0, tk.END)
    
    def run_performance_test(self):
        """Запуск теста производительности"""
        if not self.rsa_keys or not self.elgamal_keys:
            messagebox.showwarning("Предупреждение", "Сначала сгенерируйте ключи на вкладке 1!")
            return
        
        try:
            test_size = min(int(self.test_size.get()), 200)
            test_count = min(int(self.test_count.get()), 5)
            
            test_message = "Тестовое сообщение для криптографии " * (test_size // 35 + 1)
            test_message = test_message[:test_size]
            
            for item in self.results_tree.get_children():
                self.results_tree.delete(item)
            
            rsa_encrypt_times = []
            rsa_decrypt_times = []
            elgamal_encrypt_times = []
            elgamal_decrypt_times = []
            rsa_sizes = []
            elgamal_sizes = []
            
            for i in range(test_count):
                start = time.perf_counter()
                rsa_cipher = RSACrypto.encrypt(test_message, self.rsa_keys['public'])
                rsa_encrypt_times.append(time.perf_counter() - start)
                rsa_sizes.append(len(rsa_cipher.encode('utf-8')))
                
                start = time.perf_counter()
                RSACrypto.decrypt(rsa_cipher, self.rsa_keys['private'])
                rsa_decrypt_times.append(time.perf_counter() - start)
                
                start = time.perf_counter()
                elgamal_cipher = ElGamalCrypto.encrypt(test_message, self.elgamal_keys['public'])
                elgamal_encrypt_times.append(time.perf_counter() - start)
                elgamal_sizes.append(len(str(elgamal_cipher['a'])) + len(str(elgamal_cipher['b'])))
                
                start = time.perf_counter()
                ElGamalCrypto.decrypt(elgamal_cipher, self.elgamal_keys['private'])
                elgamal_decrypt_times.append(time.perf_counter() - start)
            
            avg_rsa_encrypt = sum(rsa_encrypt_times) / len(rsa_encrypt_times)
            avg_rsa_decrypt = sum(rsa_decrypt_times) / len(rsa_decrypt_times)
            avg_elgamal_encrypt = sum(elgamal_encrypt_times) / len(elgamal_encrypt_times)
            avg_elgamal_decrypt = sum(elgamal_decrypt_times) / len(elgamal_decrypt_times)
            
            avg_rsa_size = sum(rsa_sizes) / len(rsa_sizes)
            avg_elgamal_size = sum(elgamal_sizes) / len(elgamal_sizes)
            
            original_size = len(test_message.encode('utf-8'))
            
            self.results_tree.insert('', 'end', values=(
                'RSA', 'Шифрование', f'{avg_rsa_encrypt:.6f}', 
                original_size, f'{avg_rsa_size:.0f}', 
                f'{avg_rsa_size/original_size:.2f}'
            ))
            
            self.results_tree.insert('', 'end', values=(
                'RSA', 'Расшифрование', f'{avg_rsa_decrypt:.6f}', 
                f'{avg_rsa_size:.0f}', original_size,
                f'{original_size/avg_rsa_size:.2f}'
            ))
            
            self.results_tree.insert('', 'end', values=(
                'Эль-Гамаль', 'Шифрование', f'{avg_elgamal_encrypt:.6f}', 
                original_size, f'{avg_elgamal_size:.0f}',
                f'{avg_elgamal_size/original_size:.2f}'
            ))
            
            self.results_tree.insert('', 'end', values=(
                'Эль-Гамаль', 'Расшифрование', f'{avg_elgamal_decrypt:.6f}', 
                f'{avg_elgamal_size:.0f}', original_size,
                f'{original_size/avg_elgamal_size:.2f}'
            ))
            
            summary = "=" * 70 + "\n"
            summary += "СРАВНИТЕЛЬНЫЙ АНАЛИЗ ПРОИЗВОДИТЕЛЬНОСТИ\n"
            summary += "=" * 70 + "\n\n"
            
            summary += f"Параметры тестирования:\n"
            summary += f"  Размер сообщения: {test_size} символов ({original_size} байт)\n"
            summary += f"  Количество тестов: {test_count}\n"
            summary += f"  Размер ключей: {self.key_size.get()} бит\n\n"
            
            summary += "1. СРАВНЕНИЕ ВРЕМЕНИ ВЫПОЛНЕНИЯ:\n"
            summary += "-" * 50 + "\n"
            summary += f"  RSA шифрование:      {avg_rsa_encrypt:.6f} сек\n"
            summary += f"  RSA расшифрование:   {avg_rsa_decrypt:.6f} сек\n"
            summary += f"  Эль-Гамаль шифрование: {avg_elgamal_encrypt:.6f} сек\n"
            summary += f"  Эль-Гамаль расшифрование: {avg_elgamal_decrypt:.6f} сек\n\n"
            
            summary += "2. СРАВНЕНИЕ РАЗМЕРА КРИПТОТЕКСТА:\n"
            summary += "-" * 50 + "\n"
            summary += f"  Исходный текст:      {original_size} байт\n"
            summary += f"  RSA криптотекст:     {avg_rsa_size:.0f} байт\n"
            summary += f"  Эль-Гамаль криптотекст: {avg_elgamal_size:.0f} байт\n\n"
            
            summary += "3. ВЫВОДЫ:\n"
            summary += "-" * 50 + "\n"
            
            if avg_rsa_encrypt < avg_elgamal_encrypt:
                summary += f"RSA шифрует быстрее Эль-Гамаля в {avg_elgamal_encrypt/avg_rsa_encrypt:.2f} раз\n"
            else:
                summary += f"Эль-Гамаль шифрует быстрее RSA в {avg_rsa_encrypt/avg_elgamal_encrypt:.2f} раз\n"
            
            if avg_rsa_size < avg_elgamal_size:
                summary += f"RSA создает более компактный криптотекст (в {avg_elgamal_size/avg_rsa_size:.2f} раз меньше)\n"
            else:
                summary += f"Эль-Гамаль создает более компактный криптотекст (в {avg_rsa_size/avg_elgamal_size:.2f} раз меньше)\n"
            
            self.summary_text.delete(1.0, tk.END)
            self.summary_text.insert(1.0, summary)
            
        except Exception as e:
            messagebox.showerror("Ошибка", f"Ошибка тестирования: {str(e)}")
    
    def run_part1_calculations(self):
        """Выполнение вычислений для части 1"""
        self.part1_results.delete(1.0, tk.END)
        
        self.part1_results.insert(1.0, "Выполняется генерация простых чисел...\n")
        self.part1_results.insert(1.0, "Это может занять 10-20 секунд...\n\n")
        self.root.update()
        
        a_values = [5, 17, 35]
        x_primes = [1009, 10007, 100003, 1000003, 10000019]
        
        n_values = {
            "1024-bit": generate_large_prime(1024),
            "2048-bit": generate_large_prime(2048)
        }
        
        output = "=" * 80 + "\n"
        output += "ЧАСТЬ 1: Вычисление y ≡ a^x mod n\n"
        output += "=" * 80 + "\n"
        output += f"\n{'a':<5} {'x':<15} {'n bits':<10} {'Время (сек)':<15} {'Результат (первые 20 цифр)':<30}\n"
        output += "-" * 80 + "\n"
        
        results = []
        
        for a in a_values:
            for x in x_primes:
                for n_name, n in n_values.items():
                    start_time = time.time()
                    result = mod_pow(a, x, n)
                    elapsed_time = time.time() - start_time
                    
                    result_str = str(result)[:20]
                    results.append({
                        'a': a, 'x': x, 'n_bits': n_name,
                        'time': elapsed_time, 'result': result_str
                    })
                    
                    output += f"{a:<5} {x:<15} {n_name:<10} {elapsed_time:<15.6f} {result_str:<30}\n"
                    self.part1_results.delete(1.0, tk.END)
                    self.part1_results.insert(1.0, output)
                    self.root.update()
        
        output += "\n" + "=" * 80 + "\n"
        output += "СВОДНАЯ ТАБЛИЦА ЗАВИСИМОСТИ ВРЕМЕНИ ВЫЧИСЛЕНИЯ\n"
        output += "=" * 80 + "\n"
        
        output += f"\n{'a':<5} {'Диапазон x':<20} {'n (бит)':<10} {'Ср. время (сек)':<15}\n"
        output += "-" * 55 + "\n"
        
        for a in a_values:
            for n_name in n_values.keys():
                times = [r['time'] for r in results if r['a'] == a and r['n_bits'] == n_name]
                if times:
                    avg_time = sum(times) / len(times)
                    x_range = f"10^3-10^7"
                    output += f"{a:<5} {x_range:<20} {n_name:<10} {avg_time:<15.6f}\n"
        
        output += "\nВЫВОДЫ ПО ЧАСТИ 1:\n"
        output += "=" * 50 + "\n"
        output += "1. Время вычисления зависит от битности модуля n\n"
        output += "2. Алгоритм быстрого возведения в степень обеспечивает\n"
        output += "   эффективные вычисления даже для очень больших чисел\n"
        
        self.part1_results.delete(1.0, tk.END)
        self.part1_results.insert(1.0, output)


def main():
    """Главная функция"""
    print("=" * 60)
    print("КРИПТОГРАФИЧЕСКАЯ ЛАБОРАТОРНАЯ РАБОТА")
    print("=" * 60)
    print("\nВыберите режим работы:")
    print("1. Только консольная часть (вычисление y ≡ a^x mod n)")
    print("2. Полное оконное приложение (RSA + Эль-Гамаль)")
    print("3. Запустить всё последовательно")
    
    choice = input("\nВаш выбор (1/2/3): ").strip()
    
    if choice == "1":
        part1_console()
    elif choice == "2":
        print("\nЗапуск оконного приложения...")
        root = tk.Tk()
        app = CryptoApp(root)
        root.mainloop()
    else:
        print("\n=== ЗАПУСК ЧАСТИ 1 ===")
        part1_console()
        print("\n=== ЗАПУСК ЧАСТИ 2 И 3 ===")
        print("Запуск оконного приложения...")
        root = tk.Tk()
        app = CryptoApp(root)
        root.mainloop()

if __name__ == "__main__":
    main()