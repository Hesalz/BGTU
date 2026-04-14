import math
from collections import defaultdict
from decimal import Decimal, getcontext
# low = old_low + (old_high - old_low) * n.low
getcontext().prec = 100

class Node:
    def __init__(self, symbol=None, low=Decimal('0'), high=Decimal('0')):
        self.symbol = symbol
        self.low = Decimal(low)
        self.high = Decimal(high)

class Compressor:
    def __init__(self):
        self.nodes = []
        self.frequencies = {}
        self.range = Node()
    
    def build(self, src):
        """Построение частот и интервалов"""
        self.frequencies = defaultdict(Decimal)
        
        inc = Decimal(1) / Decimal(len(src))
        for ch in src:
            self.frequencies[ch] += inc
        
        sorted_freq = sorted(self.frequencies.items(), key=lambda x: x[1])
        self.frequencies = dict(sorted_freq)
        
        self.nodes = []
        low = Decimal('0')
        
        for ch, freq in self.frequencies.items():
            high = low + freq
            self.nodes.append(Node(ch, low, high))
            low = high
    
    def compress(self, src):
        """Арифметическое сжатие строки"""
        self.range = Node(low=Decimal('0'), high=Decimal('1'))
        step = 0
        
        for ch in src:
            step += 1
            old_low = self.range.low
            old_high = self.range.high
            
            display_precision = min(40, 5 + step * 2)
            
            print(f"[{self._format_decimal(old_low, display_precision)} ; "
                  f"{self._format_decimal(old_high, display_precision)}]")
            
            for n in self.nodes:
                low = old_low + (old_high - old_low) * n.low
                high = old_low + (old_high - old_low) * n.high
                mark = "   <= выбран" if n.symbol == ch else ""
                print(f"{n.symbol}  {self._format_decimal(low, display_precision)} - "
                      f"{self._format_decimal(high, display_precision)}{mark}")
            
            node = next(n for n in self.nodes if n.symbol == ch)
            self.range.low = old_low + (old_high - old_low) * node.low
            self.range.high = old_low + (old_high - old_low) * node.high
            
            print()
        
        return self.range.low
    
    def decompress(self, code, length):
        """Обратное преобразование из кода в строку"""
        result = []
        print("=== Обратное преобразование ===")
        current_code = Decimal(code)
        
        for i in range(length):
            display_precision = min(40, 10 + i * 2)
            
            print(f"\nШаг {i + 1}:")
            print(f"Текущее значение кода (K): {self._format_decimal(current_code, display_precision)}")
            
            selected_node = None
            for node in self.nodes:
                if current_code >= node.low and current_code < node.high:
                    selected_node = node
                    break
            
            if selected_node is None:
                selected_node = self.nodes[-1]
            
            print(f"K попадает в [{self._format_decimal(selected_node.low, display_precision)}; "
                  f"{self._format_decimal(selected_node.high, display_precision)}] -> '{selected_node.symbol}'")
            
            new_code = (current_code - selected_node.low) / (selected_node.high - selected_node.low)
            print(f"Новый код: (K - n1) / (n2 - n1) = {self._format_decimal(new_code, display_precision)}")
            
            result.append(selected_node.symbol)
            current_code = new_code
        
        return ''.join(result)
    
    def _format_decimal(self, value, max_digits=15):
        """Форматирование десятичного числа для вывода"""
        str_value = str(value)
        
        if len(str_value) > max_digits + 2:
            decimal_point = str_value.find('.')
            if decimal_point == -1:
                decimal_point = str_value.find(',')
            
            if decimal_point != -1 and decimal_point < len(str_value) - 1:
                digits_to_keep = min(max_digits, len(str_value) - decimal_point - 1)
                if digits_to_keep > 0:
                    str_value = str_value[:decimal_point + digits_to_keep + 1]
            elif len(str_value) > max_digits:
                str_value = str_value[:max_digits]
        
        return str_value

def run(word):
    """Запуск сжатия и распаковки для одного слова"""
    print(f"=== Арифметическое сжатие слова '{word}' ===\n")
    
    compressor = Compressor()
    compressor.build(word)
    
    print("Вероятности:")
    for n in compressor.nodes:
        prob = n.high - n.low
        print(f"{n.symbol}: {compressor._format_decimal(prob)}")
    
    print("\nИнтервалы:")
    for n in compressor.nodes:
        print(f"{n.symbol}  {compressor._format_decimal(n.low)} - {compressor._format_decimal(n.high)}")
    
    print()
    code = compressor.compress(word)
    
    print(f"\nВ качестве выходной дроби берется левая граница последнего диапазона")
    print(f"Результат: {code}\n")
    
    restored = compressor.decompress(code, len(word))
    print(f"\nРезультат: {restored}")
    print(f"Совпадение с исходным: {restored == word}")

def main():
    """Основная функция"""
    word1 = "времяпрепровождение"
    run(word1)
    
    print("\n" + "="*50 + "\n")
    
    word2 = "времяпрепровождениемультимиллионер"
    run(word2)
    
    input("\nНажмите Enter для выхода...")

if __name__ == "__main__":
    main()