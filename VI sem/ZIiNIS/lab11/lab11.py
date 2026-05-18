import tkinter as tk
from tkinter import ttk, messagebox
import math

def egcd(a, b):
    if a == 0:
        return (b, 0, 1)
    else:
        g, y, x = egcd(b % a, a)
        return (g, x - (b // a) * y, y)


def modinv(a, m):
    a = a % m
    g, x, _ = egcd(a, m)
    if g != 1:
        return None
    return x % m


def point_add(P, Q, a, p):
    if P is None or P == "O":
        return Q
    if Q is None or Q == "O":
        return P

    x1, y1 = P
    x2, y2 = Q

    if x1 == x2 and (y1 + y2) % p == 0:
        return None

    if x1 == x2 and y1 == y2:
        if y1 == 0:
            return None
        inv = modinv(2 * y1, p)
        if inv is None:
            return None
        m = (3 * x1 * x1 + a) * inv % p
    else:
        if x1 == x2:
            return None
        inv = modinv(x2 - x1, p)
        if inv is None:
            return None
        m = (y2 - y1) * inv % p

    x3 = (m * m - x1 - x2) % p
    y3 = (m * (x1 - x3) - y1) % p
    return (x3, y3)


def scalar_mult(k, P, a, p):
    if k == 0 or P is None:
        return None
    
    result = None
    addend = P
    
    while k:
        if k & 1:
            result = point_add(result, addend, a, p)
            if result is None and k != 1:
                return None
        addend = point_add(addend, addend, a, p)
        if addend is None:
            return None
        k >>= 1
    
    return result


def point_neg(P, p):
    if P is None:
        return None
    return (P[0], (-P[1]) % p)



SYMBOL_TO_POINT = {
    'А': (189, 297), 'Б': (189, 454), 'В': (192, 32), 'Г': (192, 719),
    'Д': (194, 205), 'Е': (194, 546), 'Ж': (197, 145), 'З': (197, 606),
    'И': (198, 224), 'Й': (198, 527), 'К': (200, 30), 'Л': (200, 721),
    'М': (203, 324), 'Н': (203, 427), 'О': (205, 372), 'П': (205, 379),
    'Р': (206, 106), 'С': (206, 645), 'Т': (209, 82), 'У': (209, 669),
    'Ф': (210, 31), 'Х': (210, 720), 'Ц': (215, 247), 'Ч': (215, 504),
    'Ш': (218, 150), 'Щ': (218, 601), 'Ъ': (221, 138), 'Ы': (221, 613),
    'Ь': (226, 9), 'Э': (226, 742), 'Ю': (227, 299), 'Я': (227, 452)
}

POINT_TO_SYMBOL = {v: k for k, v in SYMBOL_TO_POINT.items()}


class CurveParams:
    def __init__(self):
        self.a = -1
        self.b = 1
        self.p = 751
        self.G = (0, 1) 
        self.d = 25       
        
        self.Q = scalar_mult(self.d, self.G, self.a, self.p)
        
        self.G_ecdsa = (416, 55)
        self.q_ecdsa = 13
        self.d_ecdsa = 7    
        self.Q_ecdsa = scalar_mult(self.d_ecdsa, self.G_ecdsa, self.a, self.p)

class EllipticCurveApp:
    def __init__(self, root):
        self.root = root
        self.root.title("Эллиптическая кривая E751(-1,1) - Вариант 3")
        self.root.geometry("1000x750")
        self.root.resizable(True, True)
        
        self.params = CurveParams()
        self.surname = "Иванов"
        self.name = "Иван"        
        
        self.setup_ui()
        self.update_status("Вариант 3, фамилия: " + self.surname)
    
    def setup_ui(self):
        self.notebook = ttk.Notebook(self.root)
        self.notebook.pack(fill=tk.BOTH, expand=True, padx=10, pady=10)
        
        self.tab_encrypt = ttk.Frame(self.notebook)
        self.notebook.add(self.tab_encrypt, text="Шифрование/Дешифрование")
        self.setup_encrypt_tab()
        
        self.tab_ecdsa = ttk.Frame(self.notebook)
        self.notebook.add(self.tab_ecdsa, text="ECDSA")
        self.setup_ecdsa_tab()
        
        self.tab_ops = ttk.Frame(self.notebook)
        self.notebook.add(self.tab_ops, text="Операции над точками")
        self.setup_ops_tab()
        
        self.status_var = tk.StringVar()
        self.status_bar = ttk.Label(self.root, textvariable=self.status_var, 
                                    relief=tk.SUNKEN, anchor=tk.W)
        self.status_bar.pack(side=tk.BOTTOM, fill=tk.X)
    
    def setup_encrypt_tab(self):
        frame_input = ttk.LabelFrame(self.tab_encrypt, text="Исходные данные")
        frame_input.pack(fill=tk.X, padx=10, pady=10)
        
        ttk.Label(frame_input, text="Фамилия:").grid(row=0, column=0, padx=5, pady=5, sticky=tk.W)
        self.surname_var = tk.StringVar(value=self.surname)
        ttk.Entry(frame_input, textvariable=self.surname_var, width=40).grid(row=0, column=1, padx=5, pady=5)
        
        ttk.Label(frame_input, text="Имя:").grid(row=1, column=0, padx=5, pady=5, sticky=tk.W)
        self.name_var = tk.StringVar(value=self.name)
        ttk.Entry(frame_input, textvariable=self.name_var, width=40).grid(row=1, column=1, padx=5, pady=5)
        
        ttk.Label(frame_input, text="Эфемерный ключ k (1 < k < 751):").grid(row=2, column=0, padx=5, pady=5, sticky=tk.W)
        self.k_enc_var = tk.StringVar(value="13")
        ttk.Entry(frame_input, textvariable=self.k_enc_var, width=40).grid(row=2, column=1, padx=5, pady=5)
        
        frame_buttons = ttk.Frame(self.tab_encrypt)
        frame_buttons.pack(fill=tk.X, padx=10, pady=10)
        
        ttk.Button(frame_buttons, text="Зашифровать фамилию", 
                   command=self.encrypt_surname).pack(side=tk.LEFT, padx=5)
        ttk.Button(frame_buttons, text="Зашифровать имя", 
                   command=self.encrypt_name).pack(side=tk.LEFT, padx=5)
        ttk.Button(frame_buttons, text="Расшифровать", 
                   command=self.decrypt_message).pack(side=tk.LEFT, padx=5)
        ttk.Button(frame_buttons, text="Очистить", 
                   command=self.clear_encrypt_output).pack(side=tk.LEFT, padx=5)
        
        frame_result = ttk.LabelFrame(self.tab_encrypt, text="Результаты")
        frame_result.pack(fill=tk.BOTH, expand=True, padx=10, pady=10)
        
        ttk.Label(frame_result, text="Открытый ключ Q = d*G:").grid(row=0, column=0, padx=5, pady=5, sticky=tk.W)
        q_text = f"({self.params.Q[0]}, {self.params.Q[1]})" if self.params.Q else "Ошибка"
        ttk.Label(frame_result, text=q_text).grid(row=0, column=1, padx=5, pady=5, sticky=tk.W)
        
        ttk.Label(frame_result, text="Шифротекст C1 (k*G):").grid(row=1, column=0, padx=5, pady=5, sticky=tk.W)
        self.c1_label = ttk.Label(frame_result, text="", wraplength=500)
        self.c1_label.grid(row=1, column=1, padx=5, pady=5, sticky=tk.W)
        
        ttk.Label(frame_result, text="Шифротекст C2 (Pm + k*Q):").grid(row=2, column=0, padx=5, pady=5, sticky=tk.W)
        self.c2_label = ttk.Label(frame_result, text="", wraplength=500)
        self.c2_label.grid(row=2, column=1, padx=5, pady=5, sticky=tk.W)
        
        ttk.Label(frame_result, text="Расшифрованное сообщение:").grid(row=3, column=0, padx=5, pady=5, sticky=tk.W)
        self.decrypted_label = ttk.Label(frame_result, text="", foreground="green", wraplength=500)
        self.decrypted_label.grid(row=3, column=1, padx=5, pady=5, sticky=tk.W)
        
        text_frame = ttk.Frame(frame_result)
        text_frame.grid(row=4, column=0, columnspan=2, padx=5, pady=10, sticky=tk.NSEW)
        frame_result.columnconfigure(0, weight=1)
        frame_result.rowconfigure(4, weight=1)
        
        self.details_text = tk.Text(text_frame, height=12, width=80) 
        scrollbar = ttk.Scrollbar(text_frame, orient=tk.VERTICAL, command=self.details_text.yview)
        self.details_text.configure(yscrollcommand=scrollbar.set)
        
        self.details_text.pack(side=tk.LEFT, fill=tk.BOTH, expand=True)
        scrollbar.pack(side=tk.RIGHT, fill=tk.Y)
    
    def setup_ecdsa_tab(self):
        frame_info = ttk.LabelFrame(self.tab_ecdsa, text="Параметры ECDSA")
        frame_info.pack(fill=tk.X, padx=10, pady=10)
        
        ttk.Label(frame_info, text=f"G = {self.params.G_ecdsa}").grid(row=0, column=0, padx=5, pady=2, sticky=tk.W)
        ttk.Label(frame_info, text=f"q (порядок G) = {self.params.q_ecdsa}").grid(row=1, column=0, padx=5, pady=2, sticky=tk.W)
        ttk.Label(frame_info, text=f"d (секретный ключ) = {self.params.d_ecdsa}").grid(row=2, column=0, padx=5, pady=2, sticky=tk.W)
        q_ecdsa_text = f"({self.params.Q_ecdsa[0]}, {self.params.Q_ecdsa[1]})" if self.params.Q_ecdsa else "Ошибка"
        ttk.Label(frame_info, text=f"Q (открытый ключ) = {q_ecdsa_text}").grid(row=3, column=0, padx=5, pady=2, sticky=tk.W)
        
        frame_input = ttk.LabelFrame(self.tab_ecdsa, text="Подписываемое сообщение")
        frame_input.pack(fill=tk.X, padx=10, pady=10)
        
        ttk.Label(frame_input, text="Символ (русская буква):").grid(row=0, column=0, padx=5, pady=5)
        self.sign_char_var = tk.StringVar(value="Б")
        ttk.Combobox(frame_input, textvariable=self.sign_char_var, 
                     values=list(SYMBOL_TO_POINT.keys()), width=10).grid(row=0, column=1, padx=5, pady=5)
        
        ttk.Label(frame_input, text="Эфемерный ключ k (1 < k < 13, gcd(k,13)=1):").grid(row=1, column=0, padx=5, pady=5)
        self.k_ecdsa_var = tk.StringVar(value="5")
        ttk.Entry(frame_input, textvariable=self.k_ecdsa_var, width=20).grid(row=1, column=1, padx=5, pady=5)
        
        frame_buttons = ttk.Frame(self.tab_ecdsa)
        frame_buttons.pack(fill=tk.X, padx=10, pady=10)
        
        ttk.Button(frame_buttons, text="Сгенерировать подпись", 
                   command=self.generate_signature).pack(side=tk.LEFT, padx=5)
        ttk.Button(frame_buttons, text="Проверить подпись", 
                   command=self.verify_signature).pack(side=tk.LEFT, padx=5)
        
        frame_result = ttk.LabelFrame(self.tab_ecdsa, text="Результаты")
        frame_result.pack(fill=tk.BOTH, expand=True, padx=10, pady=10)
        
        ttk.Label(frame_result, text="Хеш H(M) = x mod q:").grid(row=0, column=0, padx=5, pady=5, sticky=tk.W)
        self.hash_label = ttk.Label(frame_result, text="")
        self.hash_label.grid(row=0, column=1, padx=5, pady=5, sticky=tk.W)
        
        ttk.Label(frame_result, text="Подпись (r, s):").grid(row=1, column=0, padx=5, pady=5, sticky=tk.W)
        self.signature_label = ttk.Label(frame_result, text="")
        self.signature_label.grid(row=1, column=1, padx=5, pady=5, sticky=tk.W)
        
        ttk.Label(frame_result, text="Результат проверки:").grid(row=2, column=0, padx=5, pady=5, sticky=tk.W)
        self.verify_label = ttk.Label(frame_result, text="")
        self.verify_label.grid(row=2, column=1, padx=5, pady=5, sticky=tk.W)
        
        text_frame = ttk.Frame(frame_result)
        text_frame.grid(row=3, column=0, columnspan=2, padx=5, pady=10, sticky=tk.NSEW)
        frame_result.columnconfigure(0, weight=1)
        frame_result.rowconfigure(3, weight=1)
        
        self.ecdsa_details_text = tk.Text(text_frame, height=10, width=80)
        scrollbar = ttk.Scrollbar(text_frame, orient=tk.VERTICAL, command=self.ecdsa_details_text.yview)
        self.ecdsa_details_text.configure(yscrollcommand=scrollbar.set)
        
        self.ecdsa_details_text.pack(side=tk.LEFT, fill=tk.BOTH, expand=True)
        scrollbar.pack(side=tk.RIGHT, fill=tk.Y)
    
    def setup_ops_tab(self):
        frame_params = ttk.LabelFrame(self.tab_ops, text="Точки (вариант 3, табл. 11.8)")
        frame_params.pack(fill=tk.X, padx=10, pady=10)
        
        ttk.Label(frame_params, text="P = (62, 372)").grid(row=0, column=0, padx=5, pady=2, sticky=tk.W)
        ttk.Label(frame_params, text="Q = (70, 195)").grid(row=1, column=0, padx=5, pady=2, sticky=tk.W)
        ttk.Label(frame_params, text="R = (67, 84)").grid(row=2, column=0, padx=5, pady=2, sticky=tk.W)
        ttk.Label(frame_params, text="k = 7, l = 8").grid(row=3, column=0, padx=5, pady=2, sticky=tk.W)
        
        frame_buttons = ttk.Frame(self.tab_ops)
        frame_buttons.pack(fill=tk.X, padx=10, pady=10)
        
        ttk.Button(frame_buttons, text="Вычислить все операции", 
                   command=self.calculate_operations).pack(side=tk.LEFT, padx=5)
        
        frame_result = ttk.LabelFrame(self.tab_ops, text="Результаты операций")
        frame_result.pack(fill=tk.BOTH, expand=True, padx=10, pady=10)
        
        self.ops_text = tk.Text(frame_result, height=12, width=80)
        scrollbar = ttk.Scrollbar(frame_result, orient=tk.VERTICAL, command=self.ops_text.yview)
        self.ops_text.configure(yscrollcommand=scrollbar.set)
        
        self.ops_text.pack(side=tk.LEFT, fill=tk.BOTH, expand=True, padx=5, pady=5)
        scrollbar.pack(side=tk.RIGHT, fill=tk.Y)
    
    def text_to_points(self, input_text):
        """Преобразует строку в список точек ЭК"""
        points = []
        missing_chars = []
        for ch in input_text.upper():
            if ch in SYMBOL_TO_POINT:
                points.append(SYMBOL_TO_POINT[ch])
            else:
                missing_chars.append(ch)
        return points, missing_chars
    
    def points_to_text(self, points):
        result_text = ""
        for pt in points:
            if pt in POINT_TO_SYMBOL:
                result_text += POINT_TO_SYMBOL[pt]
            else:
                result_text += "?"
        return result_text
    
    def encrypt_text(self, input_text, k):
        points, missing = self.text_to_points(input_text)
        
        if missing:
            return None, None, missing
        
        C1_list = []
        C2_list = []
        
        for Pm in points:
            C1 = scalar_mult(k, self.params.G, self.params.a, self.params.p)
            if C1 is None:
                return None, None, None
            
            kQ = scalar_mult(k, self.params.Q, self.params.a, self.params.p)
            if kQ is None:
                return None, None, None
            
            C2 = point_add(Pm, kQ, self.params.a, self.params.p)
            if C2 is None:
                return None, None, None
            
            C1_list.append(C1)
            C2_list.append(C2)
        
        return C1_list, C2_list, None
    
    def encrypt_surname(self):
        try:
            surname = self.surname_var.get().strip().upper()
            if not surname:
                messagebox.showwarning("Предупреждение", "Введите фамилию")
                return
            
            k = int(self.k_enc_var.get())
            if k <= 1 or k >= self.params.p:
                messagebox.showwarning("Предупреждение", 
                    f"Эфемерный ключ k должен быть 1 < k < {self.params.p}")
                return
            
            C1_list, C2_list, missing = self.encrypt_text(surname, k)
            
            if missing:
                messagebox.showwarning("Предупреждение", 
                    f"Следующие буквы отсутствуют в таблице: {missing}")
                return
            
            if C1_list is None:
                messagebox.showerror("Ошибка", "Ошибка при шифровании. Попробуйте другой ключ k.")
                return
            
            self.last_C1 = C1_list
            self.last_C2 = C2_list
            self.last_text = surname
            
            self.c1_label.config(text=str(C1_list))
            self.c2_label.config(text=str(C2_list))
            self.decrypted_label.config(text="")
            
            self.details_text.delete(1.0, tk.END)
            self.details_text.insert(tk.END, f"Исходная фамилия: {surname}\n")
            self.details_text.insert(tk.END, f"Количество символов: {len(surname)}\n")
            points, _ = self.text_to_points(surname)
            self.details_text.insert(tk.END, f"Точки: {points}\n\n")
            self.details_text.insert(tk.END, f"Эфемерный ключ k = {k}\n")
            self.details_text.insert(tk.END, f"Открытый ключ Q = {self.params.Q}\n\n")
            self.details_text.insert(tk.END, f"C1 (k*G) = {C1_list}\n")
            self.details_text.insert(tk.END, f"C2 (Pm + k*Q) = {C2_list}\n")
            
            self.update_status(f"Фамилия '{surname}' зашифрована")
            
        except ValueError:
            messagebox.showerror("Ошибка", "Неверный формат ключа k")
        except Exception as e:
            messagebox.showerror("Ошибка", f"Ошибка: {str(e)}")
    
    def encrypt_name(self):
        try:
            name = self.name_var.get().strip().upper()
            if not name:
                messagebox.showwarning("Предупреждение", "Введите имя")
                return
            
            k = int(self.k_enc_var.get())
            if k <= 1 or k >= self.params.p:
                messagebox.showwarning("Предупреждение", 
                    f"Эфемерный ключ k должен быть 1 < k < {self.params.p}")
                return
            
            C1_list, C2_list, missing = self.encrypt_text(name, k)
            
            if missing:
                messagebox.showwarning("Предупреждение", 
                    f"Следующие буквы отсутствуют в таблице: {missing}")
                return
            
            if C1_list is None:
                messagebox.showerror("Ошибка", "Ошибка при шифровании. Попробуйте другой ключ k.")
                return
            
            self.last_C1 = C1_list
            self.last_C2 = C2_list
            self.last_text = name
            
            self.c1_label.config(text=str(C1_list))
            self.c2_label.config(text=str(C2_list))
            self.decrypted_label.config(text="")
            
            self.details_text.delete(1.0, tk.END)
            self.details_text.insert(tk.END, f"Исходное имя: {name}\n")
            self.details_text.insert(tk.END, f"Количество символов: {len(name)}\n")
            points, _ = self.text_to_points(name)
            self.details_text.insert(tk.END, f"Точки: {points}\n\n")
            self.details_text.insert(tk.END, f"Эфемерный ключ k = {k}\n")
            self.details_text.insert(tk.END, f"Открытый ключ Q = {self.params.Q}\n\n")
            self.details_text.insert(tk.END, f"C1 (k*G) = {C1_list}\n")
            self.details_text.insert(tk.END, f"C2 (Pm + k*Q) = {C2_list}\n")
            
            self.update_status(f"Имя '{name}' зашифровано")
            
        except ValueError:
            messagebox.showerror("Ошибка", "Неверный формат ключа k")
        except Exception as e:
            messagebox.showerror("Ошибка", f"Ошибка: {str(e)}")
    
    def decrypt_message(self):
        if not hasattr(self, 'last_C1') or not hasattr(self, 'last_C2'):
            messagebox.showwarning("Предупреждение", "Сначала зашифруйте сообщение")
            return
        
        decrypted_points = []
        
        for C1, C2 in zip(self.last_C1, self.last_C2):
            dC1 = scalar_mult(self.params.d, C1, self.params.a, self.params.p)
            if dC1 is None:
                messagebox.showerror("Ошибка", "Ошибка при расшифровании")
                return
            
            dC1_neg = point_neg(dC1, self.params.p)
            Pm = point_add(C2, dC1_neg, self.params.a, self.params.p)
            if Pm is None:
                messagebox.showerror("Ошибка", "Ошибка при расшифровании")
                return
            
            decrypted_points.append(Pm)
        
        decrypted_text = self.points_to_text(decrypted_points)
        self.decrypted_label.config(text=decrypted_text)
        
        self.details_text.insert(tk.END, f"\n--- Расшифрование ---\n")
        dC1_list = []
        for c1 in self.last_C1:
            val = scalar_mult(self.params.d, c1, self.params.a, self.params.p)
            dC1_list.append(val)
        self.details_text.insert(tk.END, f"d*C1 = {dC1_list}\n")
        self.details_text.insert(tk.END, f"Расшифрованные точки: {decrypted_points}\n")
        self.details_text.insert(tk.END, f"Расшифровано: {decrypted_text}\n")
        
        if decrypted_text == self.last_text:
            self.details_text.insert(tk.END, f"\n✓ Расшифрование успешно!\n")
        else:
            self.details_text.insert(tk.END, f"\n✗ Расшифрование не совпадает с исходным!\n")
        
        self.update_status(f"Расшифровано: {decrypted_text}")
    
    def clear_encrypt_output(self):
        self.c1_label.config(text="")
        self.c2_label.config(text="")
        self.decrypted_label.config(text="")
        self.details_text.delete(1.0, tk.END)
        if hasattr(self, 'last_C1'):
            del self.last_C1
            del self.last_C2
            del self.last_text
    
    def generate_signature(self):
        try:
            char = self.sign_char_var.get()
            k = int(self.k_ecdsa_var.get())
            
            if char not in SYMBOL_TO_POINT:
                messagebox.showwarning("Предупреждение", f"Буква '{char}' отсутствует в таблице")
                return
            
            point = SYMBOL_TO_POINT[char]
            h = point[0] % self.params.q_ecdsa
            
            if k <= 1 or k >= self.params.q_ecdsa:
                messagebox.showwarning("Предупреждение", 
                    f"k должно быть 1 < k < {self.params.q_ecdsa}")
                return
            
            if math.gcd(k, self.params.q_ecdsa) != 1:
                messagebox.showwarning("Предупреждение", 
                    f"k должно быть взаимно просто с {self.params.q_ecdsa}")
                return
            
            R = scalar_mult(k, self.params.G_ecdsa, self.params.a, self.params.p)
            if R is None:
                messagebox.showerror("Ошибка", "Ошибка при вычислении R = k*G")
                return
            
            r = R[0] % self.params.q_ecdsa
            if r == 0:
                messagebox.showerror("Ошибка", "r = 0, выберите другой k")
                return
            
            k_inv = modinv(k, self.params.q_ecdsa)
            if k_inv is None:
                messagebox.showerror("Ошибка", "Не удалось найти обратный элемент для k")
                return
            
            s = (k_inv * (h + self.params.d_ecdsa * r)) % self.params.q_ecdsa
            if s == 0:
                messagebox.showerror("Ошибка", "s = 0, выберите другой k")
                return
            
            self.last_signature = (r, s)
            self.last_hash = h
            self.last_char = char
            
            self.hash_label.config(text=f"{h} (x={point[0]} mod {self.params.q_ecdsa})")
            self.signature_label.config(text=f"({r}, {s})")
            self.verify_label.config(text="")
            
            self.ecdsa_details_text.delete(1.0, tk.END)
            self.ecdsa_details_text.insert(tk.END, f"Символ: '{char}' -> точка {point}\n")
            self.ecdsa_details_text.insert(tk.END, f"H(M) = {point[0]} mod {self.params.q_ecdsa} = {h}\n")
            self.ecdsa_details_text.insert(tk.END, f"Эфемерный ключ k = {k}\n")
            self.ecdsa_details_text.insert(tk.END, f"R = k*G = {R}\n")
            self.ecdsa_details_text.insert(tk.END, f"r = {r}, s = {s}\n")
            self.ecdsa_details_text.insert(tk.END, f"k^(-1) mod q = {k_inv}\n")
            self.ecdsa_details_text.insert(tk.END, f"s = k^(-1)*(h + d*r) mod q = {s}\n")
            
            self.update_status(f"Подпись для '{char}' сгенерирована: ({r}, {s})")
            
        except ValueError:
            messagebox.showerror("Ошибка", "Неверный формат ключа k")
        except Exception as e:
            messagebox.showerror("Ошибка", str(e))
    
    def verify_signature(self):
        if not hasattr(self, 'last_signature'):
            messagebox.showwarning("Предупреждение", "Сначала сгенерируйте подпись")
            return
        
        r, s = self.last_signature
        h = self.last_hash
        
        if not (1 <= r < self.params.q_ecdsa and 1 <= s < self.params.q_ecdsa):
            self.verify_label.config(text="НЕВЕРНА (r или s вне диапазона)", foreground="red")
            return
        
        w = modinv(s, self.params.q_ecdsa)
        if w is None:
            self.verify_label.config(text="НЕВЕРНА (s не имеет обратного)", foreground="red")
            return
        
        u1 = (h * w) % self.params.q_ecdsa
        u2 = (r * w) % self.params.q_ecdsa
        
        u1G = scalar_mult(u1, self.params.G_ecdsa, self.params.a, self.params.p)
        u2Q = scalar_mult(u2, self.params.Q_ecdsa, self.params.a, self.params.p)
        
        if u1G is None or u2Q is None:
            self.verify_label.config(text="НЕВЕРНА (ошибка вычислений)", foreground="red")
            return
        
        X = point_add(u1G, u2Q, self.params.a, self.params.p)
        
        if X is None:
            self.verify_label.config(text="НЕВЕРНА (X = бесконечность)", foreground="red")
            return
        
        valid = (X[0] % self.params.q_ecdsa) == r
        
        if valid:
            self.verify_label.config(text="ВЕРНА ✓", foreground="green")
            self.update_status(f"Подпись для '{self.last_char}' проверена: ВЕРНА")
        else:
            self.verify_label.config(text="НЕВЕРНА ✗", foreground="red")
            self.update_status(f"Подпись для '{self.last_char}' проверена: НЕВЕРНА")
        
        self.ecdsa_details_text.insert(tk.END, f"\n--- Проверка подписи ---\n")
        self.ecdsa_details_text.insert(tk.END, f"w = s^(-1) mod q = {w}\n")
        self.ecdsa_details_text.insert(tk.END, f"u1 = h*w mod q = {u1}\n")
        self.ecdsa_details_text.insert(tk.END, f"u2 = r*w mod q = {u2}\n")
        self.ecdsa_details_text.insert(tk.END, f"u1*G = {u1G}\n")
        self.ecdsa_details_text.insert(tk.END, f"u2*Q = {u2Q}\n")
        self.ecdsa_details_text.insert(tk.END, f"X = u1*G + u2*Q = {X}\n")
        self.ecdsa_details_text.insert(tk.END, f"X[0] mod q = {X[0] % self.params.q_ecdsa}\n")
        self.ecdsa_details_text.insert(tk.END, f"r = {r}\n")
        self.ecdsa_details_text.insert(tk.END, f"Результат: {'ВЕРНА' if valid else 'НЕВЕРНА'}\n")
    
    def calculate_operations(self):
        self.ops_text.delete(1.0, tk.END)
        
        P = (62, 372)
        Q = (70, 195)
        R = (67, 84)
        k = 7
        l = 8
        a = self.params.a
        p = self.params.p
        
        self.ops_text.insert(tk.END, "=" * 60 + "\n")
        self.ops_text.insert(tk.END, "ОПЕРАЦИИ НАД ТОЧКАМИ (Вариант 3)\n")
        self.ops_text.insert(tk.END, "=" * 60 + "\n\n")
        self.ops_text.insert(tk.END, f"P = {P}\n")
        self.ops_text.insert(tk.END, f"Q = {Q}\n")
        self.ops_text.insert(tk.END, f"R = {R}\n")
        self.ops_text.insert(tk.END, f"k = {k}, l = {l}\n\n")
        
        kP = scalar_mult(k, P, a, p)
        self.ops_text.insert(tk.END, f"a) {k}*P = {kP}\n")
        
        P_plus_Q = point_add(P, Q, a, p)
        self.ops_text.insert(tk.END, f"б) P + Q = {P_plus_Q}\n")
        
        lQ = scalar_mult(l, Q, a, p)
        kP_plus_lQ = point_add(kP, lQ, a, p)
        if kP_plus_lQ is not None:
            R_neg = point_neg(R, p)
            kP_plus_lQ_minus_R = point_add(kP_plus_lQ, R_neg, a, p)
        else:
            kP_plus_lQ_minus_R = None
        self.ops_text.insert(tk.END, f"в) {k}*P + {l}*Q - R = {kP_plus_lQ_minus_R}\n")
        
        Q_neg = point_neg(Q, p)
        P_minus_Q = point_add(P, Q_neg, a, p)
        if P_minus_Q is not None:
            P_minus_Q_plus_R = point_add(P_minus_Q, R, a, p)
        else:
            P_minus_Q_plus_R = None
        self.ops_text.insert(tk.END, f"г) P - Q + R = {P_minus_Q_plus_R}\n")
        
        self.update_status("Операции над точками вычислены")
    
    def update_status(self, message):
        self.status_var.set(message)

if __name__ == "__main__":
    root = tk.Tk()
    app = EllipticCurveApp(root)
    root.mainloop()