import hashlib
import tkinter as tk
from tkinter import ttk, scrolledtext, filedialog, messagebox
import time
import threading
from datetime import datetime

class HashApp:
    def __init__(self, root):
        self.root = root
        self.root.title("Hash Calculator & Performance Test (SHA-256 / MD5)")
        self.root.geometry("850x750")
        self.root.resizable(True, True)
        
        style = ttk.Style()
        style.theme_use('clam')
        
        main_frame = ttk.Frame(root, padding="10")
        main_frame.pack(fill=tk.BOTH, expand=True)
        
        title_label = ttk.Label(main_frame, text="Hash Calculator & Performance Test", 
                                font=('Arial', 16, 'bold'))
        title_label.pack(pady=(0, 10))
        
        algo_frame = ttk.LabelFrame(main_frame, text="Выбор алгоритма хеширования", padding="10")
        algo_frame.pack(fill=tk.X, pady=(0, 10))
        
        self.algo_var = tk.StringVar(value="sha256")
        
        sha256_radio = ttk.Radiobutton(algo_frame, text="SHA-256", variable=self.algo_var, 
                                       value="sha256")
        sha256_radio.pack(side=tk.LEFT, padx=(0, 20))
        
        md5_radio = ttk.Radiobutton(algo_frame, text="MD5", variable=self.algo_var, 
                                   value="md5")
        md5_radio.pack(side=tk.LEFT)
        
        input_frame = ttk.LabelFrame(main_frame, text="Входное сообщение", padding="10")
        input_frame.pack(fill=tk.BOTH, expand=True, pady=(0, 10))
        
        self.input_text = scrolledtext.ScrolledText(input_frame, height=8, font=('Courier', 10))
        self.input_text.pack(fill=tk.BOTH, expand=True)
        
        button_frame = ttk.Frame(input_frame)
        button_frame.pack(pady=(10, 0))
        
        ttk.Button(button_frame, text="Загрузить из файла", 
                  command=self.load_file).pack(side=tk.LEFT, padx=5)
        ttk.Button(button_frame, text="Очистить", 
                  command=self.clear_input).pack(side=tk.LEFT, padx=5)
        
        result_frame = ttk.LabelFrame(main_frame, text="Результат хеширования", padding="10")
        result_frame.pack(fill=tk.X, pady=(0, 10))
        
        result_text_frame = ttk.Frame(result_frame)
        result_text_frame.pack(fill=tk.BOTH, expand=True)
        
        self.hash_result = tk.Text(result_text_frame, height=3, font=('Courier', 10), wrap=tk.WORD)
        scrollbar = ttk.Scrollbar(result_text_frame, orient="vertical", command=self.hash_result.yview)
        self.hash_result.configure(yscrollcommand=scrollbar.set)
        
        self.hash_result.pack(side=tk.LEFT, fill=tk.BOTH, expand=True)
        scrollbar.pack(side=tk.RIGHT, fill=tk.Y)
        
        self.calc_button = ttk.Button(main_frame, text="Вычислить хеш", 
                                      command=self.calculate_hash)
        self.calc_button.pack(pady=(0, 10))
        
        perf_frame = ttk.LabelFrame(main_frame, text="Тестирование производительности", padding="10")
        perf_frame.pack(fill=tk.BOTH, expand=True, pady=(0, 10))
        
        size_frame = ttk.Frame(perf_frame)
        size_frame.pack(fill=tk.X, pady=(0, 5))
        
        ttk.Label(size_frame, text="Размер данных (байт):").pack(side=tk.LEFT, padx=(0, 10))
        
        self.size_var = tk.StringVar(value="1048576")
        size_combo = ttk.Combobox(size_frame, textvariable=self.size_var, width=15)
        size_combo['values'] = ('1024', '10240', '102400', '1048576', '10485760', '104857600')
        size_combo.pack(side=tk.LEFT)
        
        ttk.Label(size_frame, text="байт (1 KB - 100 MB)").pack(side=tk.LEFT, padx=(10, 0))
        
        iter_frame = ttk.Frame(perf_frame)
        iter_frame.pack(fill=tk.X, pady=(0, 10))
        
        ttk.Label(iter_frame, text="Кол-во итераций:").pack(side=tk.LEFT, padx=(0, 10))
        
        self.iter_var = tk.StringVar(value="100")
        iter_combo = ttk.Combobox(iter_frame, textvariable=self.iter_var, width=15)
        iter_combo['values'] = ('10', '50', '100', '500', '1000')
        iter_combo.pack(side=tk.LEFT)
        
        test_algo_frame = ttk.Frame(perf_frame)
        test_algo_frame.pack(fill=tk.X, pady=(0, 10))
        
        ttk.Label(test_algo_frame, text="Тестировать алгоритм:").pack(side=tk.LEFT, padx=(0, 10))
        
        self.test_algo_var = tk.StringVar(value="both")
        ttk.Radiobutton(test_algo_frame, text="SHA-256", variable=self.test_algo_var, 
                       value="sha256").pack(side=tk.LEFT, padx=(0, 10))
        ttk.Radiobutton(test_algo_frame, text="MD5", variable=self.test_algo_var, 
                       value="md5").pack(side=tk.LEFT, padx=(0, 10))
        ttk.Radiobutton(test_algo_frame, text="Оба", variable=self.test_algo_var, 
                       value="both").pack(side=tk.LEFT)
        
        self.test_button = ttk.Button(perf_frame, text="Запустить тест производительности", 
                                      command=self.run_performance_test)
        self.test_button.pack(pady=(0, 10))
        
        results_label = ttk.Label(perf_frame, text="Результаты тестирования:")
        results_label.pack(anchor=tk.W)
        
        self.results_text = scrolledtext.ScrolledText(perf_frame, height=8, font=('Courier', 9))
        self.results_text.pack(fill=tk.BOTH, expand=True)
        
        self.status_var = tk.StringVar(value="Готов")
        status_bar = ttk.Label(main_frame, textvariable=self.status_var, relief=tk.SUNKEN, anchor=tk.W)
        status_bar.pack(fill=tk.X, side=tk.BOTTOM)
    
    def get_hash_function(self, algo=None):
        """Получение функции хеширования"""
        if algo is None:
            algo = self.algo_var.get()
        
        if algo == "sha256":
            return hashlib.sha256()
        else:
            return hashlib.md5()
    
    def get_hash_name(self, algo=None):
        """Получение названия алгоритма"""
        if algo is None:
            algo = self.algo_var.get()
        
        return "SHA-256" if algo == "sha256" else "MD5"
    
    def load_file(self):
        """Загрузка данных из файла"""
        filename = filedialog.askopenfilename(
            title="Выберите файл",
            filetypes=[("Все файлы", "*.*"), ("Текстовые файлы", "*.txt"), ("Бинарные файлы", "*.bin")]
        )
        if filename:
            try:
                with open(filename, 'rb') as file:
                    data = file.read()
                    try:
                        text_data = data.decode('utf-8')
                        if len(text_data) > 1000:
                            text_data = text_data[:1000] + "\n...(файл слишком большой, показана только часть)"
                        self.input_text.delete(1.0, tk.END)
                        self.input_text.insert(1.0, text_data)
                    except:
                        self.input_text.delete(1.0, tk.END)
                        self.input_text.insert(1.0, f"[Бинарные данные, размер: {len(data)} байт]")
                self.status_var.set(f"Загружен файл: {filename} ({len(data)} байт)")
            except Exception as e:
                messagebox.showerror("Ошибка", f"Не удалось загрузить файл: {str(e)}")
    
    def clear_input(self):
        """Очистка поля ввода"""
        self.input_text.delete(1.0, tk.END)
        self.hash_result.delete(1.0, tk.END)
        self.status_var.set("Поле ввода очищено")
    
    def calculate_hash(self):
        """Вычисление хеша для введенного текста"""
        input_data = self.input_text.get(1.0, tk.END).strip()
        
        if not input_data:
            messagebox.showwarning("Предупреждение", "Введите сообщение для хеширования")
            return
        
        # Вычисляем хеш
        start_time = time.perf_counter()
        hash_object = self.get_hash_function()
        hash_object.update(input_data.encode('utf-8'))
        hash_hex = hash_object.hexdigest()
        end_time = time.perf_counter()
        
        calculation_time = (end_time - start_time) * 1000 
        
        self.hash_result.delete(1.0, tk.END)
        self.hash_result.insert(1.0, hash_hex)
        
        data_size = len(input_data.encode('utf-8'))
        algo_name = self.get_hash_name()
        self.status_var.set(f"{algo_name} хеш вычислен за {calculation_time:.3f} мс | Размер данных: {data_size} байт")
        
        self.root.clipboard_clear()
        self.root.clipboard_append(hash_hex)
        
        self.status_var.set(f"{algo_name} хеш вычислен за {calculation_time:.3f} мс и скопирован в буфер обмена")
    
    def run_performance_test(self):
        """Запуск теста производительности в отдельном потоке"""
        self.calc_button.config(state=tk.DISABLED)
        self.test_button.config(state=tk.DISABLED, text="Тестирование...")
        self.status_var.set("Выполняется тестирование производительности...")

        thread = threading.Thread(target=self._performance_test)
        thread.daemon = True
        thread.start()
    
    def test_single_algorithm(self, algo, test_data, iterations):
        """Тестирование одного алгоритма"""
        times = []
        
        for i in range(iterations):
            start_time = time.perf_counter()
            hash_object = self.get_hash_function(algo)
            hash_object.update(test_data)
            hash_result = hash_object.hexdigest()
            end_time = time.perf_counter()
            
            times.append(end_time - start_time)
        
        avg_time = sum(times) / len(times) * 1000
        min_time = min(times) * 1000
        max_time = max(times) * 1000
        total_time = sum(times)
        
        return {
            'avg_time': avg_time,
            'min_time': min_time,
            'max_time': max_time,
            'total_time': total_time
        }
    
    def _performance_test(self):
        """Собственно тест производительности"""
        try:
            size = int(self.size_var.get())
            iterations = int(self.iter_var.get())
            test_algo = self.test_algo_var.get()
            
            test_data = b'X' * size
            
            results = {}
            
            if test_algo in ['sha256', 'both']:
                self.root.after(0, lambda: self.status_var.set("Тестирование SHA-256..."))
                time.sleep(0.1) 
                results['sha256'] = self.test_single_algorithm('sha256', test_data, iterations)
            
            if test_algo in ['md5', 'both']:
                self.root.after(0, lambda: self.status_var.set("Тестирование MD5..."))
                time.sleep(0.1)
                results['md5'] = self.test_single_algorithm('md5', test_data, iterations)
            
            report = f"""
=== Отчет о тестировании хеш-функций ===
Дата и время: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}

Параметры теста:
• Размер данных: {self.format_size(size)}
• Количество итераций: {iterations}
• Общий объем данных на алгоритм: {self.format_size(size * iterations)}

"""
            
            for algo, data in results.items():
                algo_name = "SHA-256" if algo == "sha256" else "MD5"
                throughput = (size * iterations) / data['total_time'] / 1024 / 1024 
                
                report += f"""
=== {algo_name} ===
Результаты:
• Среднее время хеширования: {data['avg_time']:.3f} мс
• Минимальное время: {data['min_time']:.3f} мс
• Максимальное время: {data['max_time']:.3f} мс
• Пропускная способность: {throughput:.2f} MB/сек

"""
            
            if test_algo == 'both':
                sha_time = results['sha256']['avg_time']
                md5_time = results['md5']['avg_time']
                speed_ratio = md5_time / sha_time
                
                report += f"""
=== Сравнение производительности ===
• MD5 быстрее SHA-256 в {speed_ratio:.2f} раза
• Абсолютная разница: {abs(sha_time - md5_time):.3f} мс на {self.format_size(size)}
"""
            
            self.root.after(0, self._update_results, report)
            
        except Exception as e:
            self.root.after(0, lambda: messagebox.showerror("Ошибка", f"Ошибка при тестировании: {str(e)}"))
            self.root.after(0, self._enable_buttons)
    
    def _update_results(self, report):
        """Обновление результатов теста в UI"""
        self.results_text.delete(1.0, tk.END)
        self.results_text.insert(1.0, report)
        self.status_var.set("Тестирование завершено")
        self._enable_buttons()
    
    def _enable_buttons(self):
        """Включение кнопок после теста"""
        self.calc_button.config(state=tk.NORMAL)
        self.test_button.config(state=tk.NORMAL, text="Запустить тест производительности")
    
    def format_size(self, size_bytes):
        """Форматирование размера в байтах"""
        for unit in ['байт', 'KB', 'MB', 'GB']:
            if size_bytes < 1024.0:
                return f"{size_bytes:.2f} {unit}"
            size_bytes /= 1024.0
        return f"{size_bytes:.2f} TB"

def main():
    root = tk.Tk()
    app = HashApp(root)
    root.mainloop()

if __name__ == "__main__":
    main()