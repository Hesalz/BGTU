import threading
import time
import datetime

class Lab06a:
    def __init__(self):
        self.mutex = threading.Lock()
        self.print_lock = threading.Lock()
        self.username = "User-9a444f52"
        self.letters = [char for char in self.username if char.isalpha()]
        self.letter_index = 0
        self.index_lock = threading.Lock()
    
    def get_next_letter(self):
        """Получить следующую букву имени пользователя (циклически) с синхронизацией"""
        with self.index_lock:
            letter = self.letters[self.letter_index]
            self.letter_index = (self.letter_index + 1) % len(self.letters)
            return letter
    
    def print_iteration(self, thread_name, iteration, letter, in_critical=False):
        """Вывод информации об итерации с синхронизацией"""
        with self.print_lock:
            current_time = datetime.datetime.now().strftime("%H:%M:%S")
            critical_mark = " [CRITICAL]" if in_critical else ""
            print(f"{current_time} - {thread_name}: итерация {iteration:2d}, буква '{letter}'{critical_mark}")
    
    def thread_function(self, thread_name, iterations=90):
        """Функция, выполняемая в каждом потоке"""
        i = 1
        while i <= iterations:
            letter = self.get_next_letter()
            
            if 30 <= i <= 60:
                with self.mutex:
                    while i <= 60 and i <= iterations:
                        self.print_iteration(thread_name, i, letter, True)
                        time.sleep(0.1) 
                        i += 1
                        if i <= iterations:
                            letter = self.get_next_letter()
            else:
                self.print_iteration(thread_name, i, letter, False)
                time.sleep(0.1)
                i += 1
    
    def run(self):
        """Запуск приложения"""
        self.letter_index = 0
        
        current_time = datetime.datetime.now().strftime("%H:%M:%S")

        thread_a = threading.Thread(target=self.thread_function, args=("Поток A",))
        thread_b = threading.Thread(target=self.thread_function, args=("Поток B",))
        
        thread_a.start()
        thread_b.start()
        
        self.thread_function("Главный поток")
        
        thread_a.join()
        thread_b.join()
        
        current_time = datetime.datetime.now().strftime("%H:%M:%S")
        print("-" * 60)
        print(f"{current_time} - Все потоки завершили выполнение")

if __name__ == "__main__":
    app = Lab06a()
    app.run()