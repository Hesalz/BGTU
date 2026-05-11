import tkinter as tk
from tkinter import ttk, scrolledtext, messagebox
import hashlib
import random
import time
from dataclasses import dataclass
from typing import Tuple, Optional
import math

def mod_pow(a, d, n):
    res = 1
    while d > 0:
        if d & 1:
            res = (res * a) % n
        a = (a * a) % n
        d >>= 1
    return res

def gcd_extended(a, b):
    if a == 0:
        return b, 0, 1
    g, x1, y1 = gcd_extended(b % a, a)
    x = y1 - (b // a) * x1
    y = x1
    return g, x, y

def mod_inverse(a, m):
    g, x, _ = gcd_extended(a, m)
    if g != 1:
        raise ValueError("Обратного элемента не существует")
    return x % m

def is_prime(n, k=10):
    if n < 2:
        return False
    if n in (2, 3):
        return True
    if n % 2 == 0:
        return False
    
    r, d = 0, n-1
    while d % 2 == 0:
        r += 1
        d //= 2
    
    def check_composite(a):
        x = mod_pow(a, d, n)
        if x == 1 or x == n-1:
            return False
        for _ in range(r-1):
            x = (x * x) % n
            if x == n-1:
                return False
        return True
    
    for _ in range(k):
        a = random.randrange(2, n-1)
        if check_composite(a):
            return False
    return True

def generate_prime(bits):
    while True:
        n = random.getrandbits(bits)
        n |= (1 << bits-1) | 1 
        if is_prime(n):
            return n

@dataclass
class RSAKeys:
    n: int
    e: int
    d: int

class RSASignature:
    @staticmethod
    def generate_keys(bits=1024):
        p = generate_prime(bits//2)
        q = generate_prime(bits//2)
        n = p * q
        phi = (p-1)*(q-1)
        e = 65537
        d = mod_inverse(e, phi)
        return RSAKeys(n, e, d)
    
    @staticmethod
    def sign(message: str, keys: RSAKeys) -> int:
        h = int(hashlib.sha256(message.encode()).hexdigest(), 16)
        signature = mod_pow(h, keys.d, keys.n)
        return signature
    
    @staticmethod
    def verify(message: str, signature: int, keys: RSAKeys) -> bool:
        h = int(hashlib.sha256(message.encode()).hexdigest(), 16)
        h_decrypted = mod_pow(signature, keys.e, keys.n)
        return h == h_decrypted

@dataclass
class ElGamalKeys:
    p: int
    g: int
    x: int
    y: int

class ElGamalSignature:
    @staticmethod
    def generate_keys(bits=512):
        p = generate_prime(bits)
        g = 2
        while True:
            if mod_pow(g, (p-1)//2, p) != 1:
                break
            g += 1
        x = random.randrange(2, p-1)
        y = mod_pow(g, x, p)
        return ElGamalKeys(p, g, x, y)
    
    @staticmethod
    def sign(message: str, keys: ElGamalKeys) -> Tuple[int, int]:
        h = int(hashlib.sha256(message.encode()).hexdigest(), 16) % (keys.p-1)
        while True:
            k = random.randrange(2, keys.p-1)
            if math.gcd(k, keys.p-1) == 1:
                break
        r = mod_pow(keys.g, k, keys.p)
        k_inv = mod_inverse(k, keys.p-1)
        s = (h - keys.x * r) * k_inv % (keys.p-1)
        if s == 0:
            return ElGamalSignature.sign(message, keys) 
        return r, s
    
    @staticmethod
    def verify(message: str, signature: Tuple[int, int], keys: ElGamalKeys) -> bool:
        r, s = signature
        if not (0 < r < keys.p):
            return False
        h = int(hashlib.sha256(message.encode()).hexdigest(), 16) % (keys.p-1)
        left = mod_pow(keys.g, h, keys.p)
        right = (mod_pow(keys.y, r, keys.p) * mod_pow(r, s, keys.p)) % keys.p
        return left == right

@dataclass
class SchnorrKeys:
    p: int
    q: int
    g: int
    x: int
    y: int

class SchnorrSignature:
    @staticmethod
    def generate_keys(bits=512):
        q_bits = 160
        q = generate_prime(q_bits)
        while True:
            k = random.getrandbits(bits - q_bits)
            p = k * q + 1
            if is_prime(p):
                break
        h = random.randrange(2, p-1)
        g = mod_pow(h, (p-1)//q, p)
        while g == 1:
            h = random.randrange(2, p-1)
            g = mod_pow(h, (p-1)//q, p)
        x = random.randrange(1, q)
        y = mod_pow(g, x, p)
        return SchnorrKeys(p, q, g, x, y)
    
    @staticmethod
    def sign(message: str, keys: SchnorrKeys) -> Tuple[int, int]:
        h_m = hashlib.sha256(message.encode()).hexdigest()
        while True:
            k = random.randrange(1, keys.q)
            r = mod_pow(keys.g, k, keys.p)
            combined = str(r) + h_m
            e = int(hashlib.sha256(combined.encode()).hexdigest(), 16) % keys.q
            s = (k - keys.x * e) % keys.q
            if s != 0:
                return e, s
    
    @staticmethod
    def verify(message: str, signature: Tuple[int, int], keys: SchnorrKeys) -> bool:
        e, s = signature
        if not (0 < s < keys.q):
            return False
        h_m = hashlib.sha256(message.encode()).hexdigest()
        r_prime = (mod_pow(keys.g, s, keys.p) * mod_pow(keys.y, e, keys.p)) % keys.p
        combined = str(r_prime) + h_m
        e_prime = int(hashlib.sha256(combined.encode()).hexdigest(), 16) % keys.q
        return e == e_prime

class SignatureApp:
    def __init__(self, root):
        self.root = root
        self.root.title("ЭЦП: RSA, Эль-Гамаль, Шнорр")
        self.root.geometry("900x700")
        
        self.algorithms = {
            "RSA": (RSASignature, RSAKeys),
            "Эль-Гамаль": (ElGamalSignature, ElGamalKeys),
            "Шнорр": (SchnorrSignature, SchnorrKeys)
        }
        self.current_keys = {}
        
        self.create_widgets()
        
    def create_widgets(self):
        frame_algo = ttk.LabelFrame(self.root, text="Алгоритм", padding=10)
        frame_algo.pack(fill=tk.X, padx=10, pady=5)
        
        self.algo_var = tk.StringVar(value="RSA")
        algo_combo = ttk.Combobox(frame_algo, textvariable=self.algo_var, values=list(self.algorithms.keys()), state="readonly")
        algo_combo.pack(fill=tk.X)
        algo_combo.bind("<<ComboboxSelected>>", self.on_algo_change)
        
        frame_keys = ttk.LabelFrame(self.root, text="Ключи", padding=10)
        frame_keys.pack(fill=tk.X, padx=10, pady=5)
        
        self.btn_gen_keys = ttk.Button(frame_keys, text="Сгенерировать ключи", command=self.generate_keys)
        self.btn_gen_keys.pack(side=tk.LEFT, padx=5)
        
        self.txt_keys = scrolledtext.ScrolledText(frame_keys, height=8, width=80)
        self.txt_keys.pack(fill=tk.BOTH, expand=True, pady=5)
        
        frame_msg = ttk.LabelFrame(self.root, text="Сообщение", padding=10)
        frame_msg.pack(fill=tk.X, padx=10, pady=5)
        
        self.txt_message = scrolledtext.ScrolledText(frame_msg, height=5)
        self.txt_message.pack(fill=tk.BOTH, expand=True)
        self.txt_message.insert(tk.END, "Hello, world!")
        
        frame_ops = ttk.LabelFrame(self.root, text="Операции", padding=10)
        frame_ops.pack(fill=tk.X, padx=10, pady=5)
        
        btn_frame = ttk.Frame(frame_ops)
        btn_frame.pack(fill=tk.X)
        
        self.btn_sign = ttk.Button(btn_frame, text="Подписать", command=self.sign_message)
        self.btn_sign.pack(side=tk.LEFT, padx=5)
        
        self.btn_verify = ttk.Button(btn_frame, text="Проверить подпись", command=self.verify_signature)
        self.btn_verify.pack(side=tk.LEFT, padx=5)
        
        frame_sig = ttk.LabelFrame(self.root, text="Подпись", padding=10)
        frame_sig.pack(fill=tk.X, padx=10, pady=5)
        
        self.txt_signature = scrolledtext.ScrolledText(frame_sig, height=3)
        self.txt_signature.pack(fill=tk.BOTH, expand=True)
        
        frame_time = ttk.LabelFrame(self.root, text="Время выполнения (сек)", padding=10)
        frame_time.pack(fill=tk.X, padx=10, pady=5)
        
        self.lbl_sign_time = ttk.Label(frame_time, text="Подписание: --")
        self.lbl_sign_time.pack(anchor=tk.W)
        self.lbl_verify_time = ttk.Label(frame_time, text="Верификация: --")
        self.lbl_verify_time.pack(anchor=tk.W)
        
        self.status_var = tk.StringVar(value="Готово")
        status_bar = ttk.Label(self.root, textvariable=self.status_var, relief=tk.SUNKEN, anchor=tk.W)
        status_bar.pack(side=tk.BOTTOM, fill=tk.X)
        
        self.on_algo_change()
    
    def on_algo_change(self, event=None):
        self.txt_keys.delete(1.0, tk.END)
        self.txt_signature.delete(1.0, tk.END)
        self.lbl_sign_time.config(text="Подписание: --")
        self.lbl_verify_time.config(text="Верификация: --")
        self.current_keys = {}
    
    def generate_keys(self):
        algo_name = self.algo_var.get()
        algo_class, keys_class = self.algorithms[algo_name]
        
        bits = 512 if algo_name == "Эль-Гамаль" else 1024 if algo_name == "RSA" else 512
        self.status_var.set(f"Генерация ключей {algo_name} (битность ключа: {bits})...")
        self.root.update()
        
        start = time.time()
        try:
            keys = algo_class.generate_keys(bits)
            elapsed = time.time() - start
            self.current_keys = keys
            self.display_keys(keys)
            self.status_var.set(f"Ключи сгенерированы за {elapsed:.3f} сек")
        except Exception as e:
            messagebox.showerror("Ошибка", f"Не удалось сгенерировать ключи: {e}")
            self.status_var.set("Ошибка генерации ключей")
    
    def display_keys(self, keys):
        self.txt_keys.delete(1.0, tk.END)
        if isinstance(keys, RSAKeys):
            self.txt_keys.insert(tk.END, f"RSA Ключи:\n")
            self.txt_keys.insert(tk.END, f"n = {keys.n}\n")
            self.txt_keys.insert(tk.END, f"e (открытый) = {keys.e}\n")
            self.txt_keys.insert(tk.END, f"d (секретный) = {keys.d}\n")
        elif isinstance(keys, ElGamalKeys):
            self.txt_keys.insert(tk.END, f"Эль-Гамаль Ключи:\n")
            self.txt_keys.insert(tk.END, f"p = {keys.p}\n")
            self.txt_keys.insert(tk.END, f"g = {keys.g}\n")
            self.txt_keys.insert(tk.END, f"y (открытый) = {keys.y}\n")
            self.txt_keys.insert(tk.END, f"x (секретный) = {keys.x}\n")
        elif isinstance(keys, SchnorrKeys):
            self.txt_keys.insert(tk.END, f"Шнорр Ключи:\n")
            self.txt_keys.insert(tk.END, f"p = {keys.p}\n")
            self.txt_keys.insert(tk.END, f"q = {keys.q}\n")
            self.txt_keys.insert(tk.END, f"g = {keys.g}\n")
            self.txt_keys.insert(tk.END, f"y (открытый) = {keys.y}\n")
            self.txt_keys.insert(tk.END, f"x (секретный) = {keys.x}\n")
    
    def sign_message(self):
        if not self.current_keys:
            messagebox.showwarning("Предупреждение", "Сначала сгенерируйте ключи")
            return
        
        algo_name = self.algo_var.get()
        algo_class, _ = self.algorithms[algo_name]
        message = self.txt_message.get(1.0, tk.END).strip()
        
        if not message:
            messagebox.showwarning("Предупреждение", "Введите сообщение")
            return
        
        self.status_var.set("Подписание...")
        self.root.update()
        
        start = time.time()
        try:
            if algo_name == "RSA":
                sig = algo_class.sign(message, self.current_keys)
                sig_str = str(sig)
            elif algo_name == "Эль-Гамаль":
                r, s = algo_class.sign(message, self.current_keys)
                sig_str = f"r={r}, s={s}"
            else:
                e, s = algo_class.sign(message, self.current_keys)
                sig_str = f"e={e}, s={s}"
            elapsed = time.time() - start
            
            self.txt_signature.delete(1.0, tk.END)
            self.txt_signature.insert(tk.END, sig_str)
            self.lbl_sign_time.config(text=f"Подписание: {elapsed:.6f} сек")
            self.status_var.set(f"Подпись создана за {elapsed:.6f} сек")
        except Exception as e:
            messagebox.showerror("Ошибка", f"Ошибка подписания: {e}")
            self.status_var.set("Ошибка подписания")
    
    def verify_signature(self):
        if not self.current_keys:
            messagebox.showwarning("Предупреждение", "Сначала сгенерируйте ключи")
            return
        
        algo_name = self.algo_var.get()
        algo_class, _ = self.algorithms[algo_name]
        message = self.txt_message.get(1.0, tk.END).strip()
        sig_text = self.txt_signature.get(1.0, tk.END).strip()
        
        if not message or not sig_text:
            messagebox.showwarning("Предупреждение", "Введите сообщение и подпись")
            return
        
        try:
            if algo_name == "RSA":
                signature = int(sig_text)
            elif algo_name == "Эль-Гамаль":
                parts = sig_text.replace("r=", "").replace("s=", "").split(",")
                r = int(parts[0].strip())
                s = int(parts[1].strip())
                signature = (r, s)
            else:
                parts = sig_text.replace("e=", "").replace("s=", "").split(",")
                e = int(parts[0].strip())
                s = int(parts[1].strip())
                signature = (e, s)
        except:
            messagebox.showerror("Ошибка", "Неверный формат подписи")
            return
        
        self.status_var.set("Верификация...")
        self.root.update()
        
        start = time.time()
        try:
            if algo_name == "RSA":
                valid = algo_class.verify(message, signature, self.current_keys)
            elif algo_name == "Эль-Гамаль":
                valid = algo_class.verify(message, signature, self.current_keys)
            else:
                valid = algo_class.verify(message, signature, self.current_keys)
            elapsed = time.time() - start
            
            self.lbl_verify_time.config(text=f"Верификация: {elapsed:.6f} сек")
            if valid:
                messagebox.showinfo("Результат", "Подпись ВЕРНА!")
                self.status_var.set(f"Подпись верна. Время: {elapsed:.6f} сек")
            else:
                messagebox.showerror("Результат", "Подпись НЕВЕРНА!")
                self.status_var.set(f"Подпись неверна. Время: {elapsed:.6f} сек")
        except Exception as e:
            messagebox.showerror("Ошибка", f"Ошибка верификации: {e}")
            self.status_var.set("Ошибка верификации")

if __name__ == "__main__":
    root = tk.Tk()
    app = SignatureApp(root)
    root.mainloop()