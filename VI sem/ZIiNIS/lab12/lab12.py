import tkinter as tk
from tkinter import filedialog, messagebox, scrolledtext, ttk
from PIL import Image, ImageTk
import numpy as np
import os
import random
import matplotlib.pyplot as plt
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg


class LSBStego:
    END_MARKER = "###END###"
    
    def text_to_bits(self, text):
        text_bytes = text.encode('utf-8')
        bits = []
        for byte in text_bytes:
            for i in range(7, -1, -1):
                bits.append((byte >> i) & 1)
        return bits
    
    def bits_to_text(self, bits):
        bytes_list = []
        for i in range(0, len(bits), 8):
            if i + 8 <= len(bits):
                byte = 0
                for j in range(8):
                    byte = (byte << 1) | bits[i + j]
                bytes_list.append(byte)
        return bytes(bytes_list).decode('utf-8', errors='ignore')
    
    def embed_sequential(self, image_path, message, output_path, bits_to_use=2):
        """Последовательное встраивание (биты по порядку)"""
        img = Image.open(image_path)
        if img.mode != 'RGB':
            img = img.convert('RGB')
        
        pixels = list(img.getdata())
        width, height = img.size
        
        message_with_end = message + self.END_MARKER
        bits = self.text_to_bits(message_with_end)
        
        max_bits = len(pixels) * 3
        if len(bits) > max_bits:
            raise Exception(f"Сообщение слишком большое! Нужно {len(bits)} бит, доступно {max_bits}")
        
        bit_index = 0
        new_pixels = []
        
        for pixel in pixels:
            r, g, b = pixel
            new_r, new_g, new_b = r, g, b
            
            for channel_idx, channel_val in enumerate([r, g, b]):
                if bit_index < len(bits):
                    new_val = (channel_val & 0xFE) | bits[bit_index]
                    
                    if new_val < 0:
                        new_val = 0
                    if new_val > 255:
                        new_val = 255
                    
                    if channel_idx == 0:
                        new_r = new_val
                    elif channel_idx == 1:
                        new_g = new_val
                    else:
                        new_b = new_val
                    
                    bit_index += 1
            
            new_pixels.append((new_r, new_g, new_b))
        
        result_img = Image.new('RGB', (width, height))
        result_img.putdata(new_pixels)
        result_img.save(output_path, 'PNG')
        
        return bit_index // 8
    
    def extract_sequential(self, image_path, bits_to_use=2):
        """Извлечение для последовательного метода"""
        img = Image.open(image_path)
        if img.mode != 'RGB':
            img = img.convert('RGB')
        
        pixels = list(img.getdata())
        
        bits = []
        for pixel in pixels:
            r, g, b = pixel
            for channel_val in [r, g, b]:
                bit = channel_val & 1
                bits.append(bit)
        
        full_text = self.bits_to_text(bits)
        end_pos = full_text.find(self.END_MARKER)
        
        if end_pos != -1:
            return full_text[:end_pos]
        return "⚠️ Сообщение не найдено!"
    
    def embed_random(self, image_path, message, output_path, password, bits_to_use=2):
        rng = random.Random()
        rng.seed(password)
        
        img = Image.open(image_path)
        if img.mode != 'RGB':
            img = img.convert('RGB')
        
        pixels = list(img.getdata())
        width, height = img.size
        total_channels = len(pixels) * 3
        
        message_with_end = message + self.END_MARKER
        bits = self.text_to_bits(message_with_end)
        
        if len(bits) > total_channels:
            raise Exception(f"Сообщение слишком большое! Нужно {len(bits)} бит, доступно {total_channels}")
        
        indices = list(range(total_channels))
        rng.shuffle(indices)
        
        bit_index = 0
        new_pixels = list(pixels)
        
        for idx in indices:
            if bit_index >= len(bits):
                break
            
            pixel_idx = idx // 3
            channel_idx = idx % 3
            
            r, g, b = new_pixels[pixel_idx]
            channel_vals = [r, g, b]
            
            new_val = (channel_vals[channel_idx] & 0xFE) | bits[bit_index]
            
            if new_val < 0:
                new_val = 0
            if new_val > 255:
                new_val = 255
            
            channel_vals[channel_idx] = new_val
            new_pixels[pixel_idx] = tuple(channel_vals)
            bit_index += 1
        
        result_img = Image.new('RGB', (width, height))
        result_img.putdata(new_pixels)
        result_img.save(output_path, 'PNG')
        
        return bit_index // 8
    
    def extract_random(self, image_path, password, bits_to_use=2):
        rng = random.Random()
        rng.seed(password)
        
        img = Image.open(image_path)
        if img.mode != 'RGB':
            img = img.convert('RGB')
        
        pixels = list(img.getdata())
        total_channels = len(pixels) * 3
        
        indices = list(range(total_channels))
        rng.shuffle(indices)
        
        bits = []
        for idx in indices:
            pixel_idx = idx // 3
            channel_idx = idx % 3
            
            r, g, b = pixels[pixel_idx]
            channel_val = [r, g, b][channel_idx]
            
            bit = channel_val & 1
            bits.append(bit)
            
            if len(bits) % 1000 == 0 and len(bits) > 0:
                full_text = self.bits_to_text(bits)
                if self.END_MARKER in full_text:
                    break
        
        full_text = self.bits_to_text(bits)
        end_pos = full_text.find(self.END_MARKER)
        
        if end_pos != -1:
            return full_text[:end_pos]
        
        if len(bits) < total_channels:
            for idx in indices[len(bits)//3:]:
                pixel_idx = idx // 3
                channel_idx = idx % 3
                r, g, b = pixels[pixel_idx]
                channel_val = [r, g, b][channel_idx]
                bit = channel_val & 1
                bits.append(bit)
                
                full_text = self.bits_to_text(bits)
                end_pos = full_text.find(self.END_MARKER)
                if end_pos != -1:
                    return full_text[:end_pos]
        
        return "⚠️ Сообщение не найдено! Проверьте правильность пароля."
    
    def get_bit_plane(self, image_path, channel=0, bit_level=0):
        img = Image.open(image_path)
        if img.mode != 'RGB':
            img = img.convert('RGB')
        
        pixels = list(img.getdata())
        width, height = img.size
        
        plane = [[0 for _ in range(width)] for _ in range(height)]
        
        for y in range(height):
            for x in range(width):
                idx = y * width + x
                r, g, b = pixels[idx]
                val = [r, g, b][channel]
                bit = (val >> bit_level) & 1
                plane[y][x] = bit * 255
        
        return np.array(plane, dtype=np.uint8)
    
    def get_capacity(self, image_path, bits_to_use=2):
        img = Image.open(image_path)
        width, height = img.size
        return (width * height * 3 * bits_to_use) // 8


class SteganoGUI:
    def __init__(self):
        self.root = tk.Tk()
        self.root.title("LSB Steganography - 2 метода")
        self.root.geometry("1300x800")
        self.root.configure(bg='#2c3e50')
        
        self.stego = LSBStego()
        self.current_image = None
        self.current_output = None
        
        self.setup_ui()
        
    def setup_ui(self):
        main = tk.Frame(self.root, bg='#ecf0f1')
        main.pack(fill=tk.BOTH, expand=True, padx=10, pady=10)
        
        left = tk.Frame(main, bg='#ecf0f1', width=400)
        left.pack(side=tk.LEFT, fill=tk.BOTH, padx=5, pady=5)
        left.pack_propagate(False)
        
        right = tk.Frame(main, bg='#ecf0f1')
        right.pack(side=tk.RIGHT, fill=tk.BOTH, expand=True, padx=5, pady=5)
        
        tk.Label(left, text="🔐 LSB STEGANOGRAPHY", font=('Arial', 16, 'bold'), 
                bg='#2c3e50', fg='white', pady=10).pack(fill=tk.X)
        
        frame_file = tk.LabelFrame(left, text="📁 Файл-контейнер", bg='#ecf0f1', font=('Arial', 10, 'bold'))
        frame_file.pack(fill=tk.X, pady=10, padx=10)
        
        self.file_label = tk.Label(frame_file, text="Файл не выбран", bg='#ecf0f1', fg='red')
        self.file_label.pack(pady=5)
        
        tk.Button(frame_file, text="Выбрать изображение", command=self.select_image,
                 bg='#3498db', fg='white', font=('Arial', 10), pady=5).pack(fill=tk.X, padx=10, pady=5)
        
        tk.Button(frame_file, text="Информация о контейнере", command=self.show_info,
                 bg='#95a5a6', fg='white', font=('Arial', 10), pady=5).pack(fill=tk.X, padx=10, pady=5)
        
        frame_settings = tk.LabelFrame(left, text="⚙️ Настройки", bg='#ecf0f1', font=('Arial', 10, 'bold'))
        frame_settings.pack(fill=tk.X, pady=10, padx=10)
        
        tk.Label(frame_settings, text="Метод встраивания:", bg='#ecf0f1').pack(pady=5)
        self.method_var = tk.StringVar(value="sequential")
        tk.Radiobutton(frame_settings, text="1. Последовательный (без пароля)", 
                      variable=self.method_var, value="sequential", bg='#ecf0f1').pack(anchor=tk.W, padx=20)
        tk.Radiobutton(frame_settings, text="2. Случайный (с паролем)", 
                      variable=self.method_var, value="random", bg='#ecf0f1').pack(anchor=tk.W, padx=20)
        
        tk.Label(frame_settings, text="Пароль (для случайного метода):", bg='#ecf0f1').pack(pady=5)
        self.password_entry = tk.Entry(frame_settings, show="*")
        self.password_entry.pack(pady=5, padx=20, fill=tk.X)
        
        frame_msg = tk.LabelFrame(left, text="💬 Сообщение", bg='#ecf0f1', font=('Arial', 10, 'bold'))
        frame_msg.pack(fill=tk.BOTH, expand=True, pady=10, padx=10)
        
        notebook = ttk.Notebook(frame_msg)
        notebook.pack(fill=tk.BOTH, expand=True, padx=5, pady=5)
        
        self.fio_text = scrolledtext.ScrolledText(notebook, height=5)
        self.report_text = scrolledtext.ScrolledText(notebook, height=5)
        self.custom_text = scrolledtext.ScrolledText(notebook, height=5)
        
        notebook.add(self.fio_text, text="📝 ФИО")
        notebook.add(self.report_text, text="📄 Текст отчета")
        notebook.add(self.custom_text, text="✏️ Свой текст")
        
        self.fio_text.insert("1.0", "Иванов Иван Иванович")
        self.report_text.insert("1.0", "ЛАБОРАТОРНАЯ РАБОТА №12\nИсследование метода LSB стеганографии\n\nЦель: Изучение метода наименее значащих битов.\n\nВывод: Метод эффективен для сокрытия информации.")
        
        btn_frame = tk.Frame(left, bg='#ecf0f1')
        btn_frame.pack(fill=tk.X, pady=10, padx=10)
        
        tk.Button(btn_frame, text="▶ ВСТРОИТЬ", command=self.embed,
                 bg='#27ae60', fg='white', font=('Arial', 12, 'bold'), pady=10).pack(fill=tk.X, pady=5)
        
        tk.Button(btn_frame, text="◀ ИЗВЛЕЧЬ", command=self.extract,
                 bg='#e67e22', fg='white', font=('Arial', 12, 'bold'), pady=10).pack(fill=tk.X, pady=5)
        
        preview_frame = tk.LabelFrame(right, text="🖼️ Предпросмотр", bg='#ecf0f1', font=('Arial', 10, 'bold'))
        preview_frame.pack(fill=tk.BOTH, expand=True, pady=5)
        
        self.preview_label = tk.Label(preview_frame, text="Изображение не выбрано", 
                                      bg='white', height=20, width=50)
        self.preview_label.pack(fill=tk.BOTH, expand=True, padx=5, pady=5)
        
        result_frame = tk.LabelFrame(right, text="📋 Результат извлечения", bg='#ecf0f1', font=('Arial', 10, 'bold'))
        result_frame.pack(fill=tk.BOTH, expand=True, pady=5)
        
        self.result_text = scrolledtext.ScrolledText(result_frame, height=8, bg='#fff8e1')
        self.result_text.pack(fill=tk.BOTH, expand=True, padx=5, pady=5)
        
        analiz_frame = tk.Frame(right, bg='#ecf0f1')
        analiz_frame.pack(fill=tk.X, pady=5)
        
        tk.Button(analiz_frame, text="📊 Битовые плоскости", command=self.show_bit_planes,
                 bg='#9b59b6', fg='white').pack(side=tk.LEFT, expand=True, fill=tk.X, padx=5)
        
        tk.Button(analiz_frame, text="📈 Гистограммы", command=self.show_histograms,
                 bg='#1abc9c', fg='white').pack(side=tk.LEFT, expand=True, fill=tk.X, padx=5)
        
        self.status_var = tk.StringVar(value="✅ Готов к работе")
        status = tk.Label(self.root, textvariable=self.status_var, bg='#2c3e50', fg='white', 
                         font=('Arial', 9), pady=5)
        status.pack(side=tk.BOTTOM, fill=tk.X)
        
    def select_image(self):
        path = filedialog.askopenfilename(
            title="Выберите изображение",
            filetypes=[("Images", "*.png *.bmp *.jpg *.jpeg"), ("All", "*.*")]
        )
        if path:
            self.current_image = path
            self.file_label.config(text=os.path.basename(path), fg='green')
            img = Image.open(path)
            img.thumbnail((400, 300))
            photo = ImageTk.PhotoImage(img)
            self.preview_label.config(image=photo, text="")
            self.preview_label.image = photo
            self.update_status(f"Выбрано: {os.path.basename(path)}")
    
    def show_info(self):
        if not self.current_image:
            messagebox.showerror("Ошибка", "Сначала выберите изображение!")
            return
        
        img = Image.open(self.current_image)
        w, h = img.size
        capacity = self.stego.get_capacity(self.current_image, 2)
        
        info = f"""
        📷 ИНФОРМАЦИЯ О КОНТЕЙНЕРЕ
        
        Файл: {os.path.basename(self.current_image)}
        Размер: {w} x {h} пикселей
        Всего пикселей: {w * h}
        Всего цветовых каналов: {w * h * 3}
        
        ⚡ ЕМКОСТЬ (при 1 бите на канал):
        Максимум: {capacity} байт (~{capacity//1024} КБ)
        
        📌 Доступные методы:
        1. Последовательный - биты записываются по порядку
        2. Случайный - биты записываются в случайном порядке (нужен пароль)
        """
        messagebox.showinfo("Информация", info)
    
    def get_current_message(self):
        if self.fio_text.get("1.0", tk.END).strip():
            return self.fio_text.get("1.0", tk.END).strip()
        return self.fio_text.get("1.0", tk.END).strip()
    
    def embed(self):
        if not self.current_image:
            messagebox.showerror("Ошибка", "Выберите изображение!")
            return
        
        message = self.get_current_message()
        if not message:
            messagebox.showerror("Ошибка", "Введите сообщение!")
            return
        
        output_path = filedialog.asksaveasfilename(
            defaultextension=".png",
            filetypes=[("PNG image", "*.png")]
        )
        if not output_path:
            return
        
        try:
            self.update_status("Встраивание...")
            
            method = self.method_var.get()
            
            if method == "sequential":
                bytes_embedded = self.stego.embed_sequential(self.current_image, message, output_path, 2)
                method_name = "ПОСЛЕДОВАТЕЛЬНЫЙ"
            else:
                password = self.password_entry.get()
                if not password:
                    messagebox.showerror("Ошибка", "Для случайного метода введите пароль!")
                    return
                bytes_embedded = self.stego.embed_random(self.current_image, message, output_path, password, 2)
                method_name = "СЛУЧАЙНЫЙ (с паролем)"
            
            self.current_output = output_path
            self.update_status(f"✅ Встроено {bytes_embedded} байт методом {method_name}")
            messagebox.showinfo("Успех", 
                f"Сообщение встроено!\n\n"
                f"Метод: {method_name}\n"
                f"Файл: {os.path.basename(output_path)}\n"
                f"Байт: {bytes_embedded}")
            
        except Exception as e:
            messagebox.showerror("Ошибка", str(e))
            self.update_status("❌ Ошибка встраивания")
    
    def extract(self):
        if not self.current_image:
            messagebox.showerror("Ошибка", "Выберите изображение с сообщением!")
            return
        
        try:
            self.update_status("Извлечение...")
            
            method = self.method_var.get()
            
            if method == "sequential":
                result = self.stego.extract_sequential(self.current_image, 2)
                method_name = "ПОСЛЕДОВАТЕЛЬНЫЙ"
            else:
                password = self.password_entry.get()
                if not password:
                    messagebox.showerror("Ошибка", "Для случайного метода введите пароль!")
                    return
                result = self.stego.extract_random(self.current_image, password, 2)
                method_name = "СЛУЧАЙНЫЙ (с паролем)"
            
            self.result_text.delete("1.0", tk.END)
            self.result_text.insert("1.0", result)
            self.update_status(f"✅ Извлечено {len(result)} символов методом {method_name}")
            
        except Exception as e:
            messagebox.showerror("Ошибка", str(e))
            self.update_status("❌ Ошибка извлечения")
    
    def show_bit_planes(self):
        if not self.current_image:
            messagebox.showerror("Ошибка", "Выберите изображение!")
            return
        
        win = tk.Toplevel(self.root)
        win.title("Битовые плоскости")
        win.geometry("1000x800")
        
        fig, axes = plt.subplots(3, 4, figsize=(12, 10))
        fig.suptitle("Битовые плоскости (0 - младший бит, 3 - старший)", fontsize=14)
        
        for channel in range(3):
            for bit in range(4):
                plane = self.stego.get_bit_plane(self.current_image, channel, bit)
                axes[channel, bit].imshow(plane, cmap='gray')
                if channel == 0:
                    axes[channel, bit].set_title(f'Бит {bit}')
                if bit == 0:
                    axes[channel, bit].set_ylabel(['R','G','B'][channel])
                axes[channel, bit].set_xticks([])
                axes[channel, bit].set_yticks([])
        
        plt.tight_layout()
        canvas = FigureCanvasTkAgg(fig, master=win)
        canvas.draw()
        canvas.get_tk_widget().pack(fill=tk.BOTH, expand=True)
    
    def show_histograms(self):
        if not self.current_output or not os.path.exists(self.current_output):
            messagebox.showerror("Ошибка", "Сначала встройте сообщение!")
            return
        
        win = tk.Toplevel(self.root)
        win.title("Сравнение гистограмм")
        win.geometry("1000x600")
        
        original = Image.open(self.current_image).convert('RGB')
        stego = Image.open(self.current_output).convert('RGB')
        
        fig, axes = plt.subplots(2, 3, figsize=(14, 8))
        fig.suptitle("Сравнение гистограмм исходного и стеганоконтейнера", fontsize=14)
        
        for i, color in enumerate(['red', 'green', 'blue']):
            hist_orig = original.histogram()[i*256:(i+1)*256]
            axes[0, i].bar(range(256), hist_orig, color=color, alpha=0.7, width=1)
            axes[0, i].set_title(f'{["R","G","B"][i]} - Исходный')
            axes[0, i].set_xlabel('Интенсивность')
            axes[0, i].set_ylabel('Частота')
            
            hist_stego = stego.histogram()[i*256:(i+1)*256]
            axes[1, i].bar(range(256), hist_stego, color=color, alpha=0.7, width=1)
            axes[1, i].set_title(f'{["R","G","B"][i]} - Стеганоконтейнер')
            axes[1, i].set_xlabel('Интенсивность')
            axes[1, i].set_ylabel('Частота')
        
        plt.tight_layout()
        canvas = FigureCanvasTkAgg(fig, master=win)
        canvas.draw()
        canvas.get_tk_widget().pack(fill=tk.BOTH, expand=True)
    
    def update_status(self, msg):
        self.status_var.set(msg)
        print(f"[STATUS] {msg}")
    
    def run(self):
        self.root.mainloop()


if __name__ == "__main__":
    app = SteganoGUI()
    app.run()