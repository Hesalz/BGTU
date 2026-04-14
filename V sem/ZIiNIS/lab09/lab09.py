import math
from collections import Counter
from typing import Dict, List, Tuple, Optional
import sys

class CharacterInfo:
    def __init__(self, character: str, probability: float):
        self.character = character
        self.probability = probability

MANTISSA = 4

def build_dynamic_table(text: str) -> Dict[str, float]:
    """Таблица динамических вероятностей из самого сообщения"""
    total = len(text)
    char_count = Counter(text)
    
    probabilities = {}
    for char, count in char_count.items():
        probabilities[char] = round(count / total, MANTISSA)
    
    return dict(sorted(probabilities.items(), key=lambda x: x[1], reverse=True))

def print_table(table: Dict[str, float], title: str) -> None:
    """Вывод таблицы"""
    print(f"\n{title}")
    for char, prob in table.items():
        print(f"{char} : {prob}")

def shannon_fano(table: Dict[str, float]) -> Dict[str, str]:
    """Кодирование Шеннона-Фано"""
    items = list(table.items())
    codes = {char: "" for char in table.keys()}
    
    class SFNode:
        def __init__(self, symbols: List[str], prob: float, left=None, right=None):
            self.symbols = symbols
            self.prob = prob
            self.left = left
            self.right = right
    
    sf_tree_root = None
    
    def shannon_split(start: int, end: int) -> SFNode:
        if end - start <= 1:
            char = items[start][0]
            return SFNode([char], items[start][1])
        
        total = sum(item[1] for item in items[start:end])
        
        partial = 0.0
        split = start
        for i in range(start, end):
            partial += items[i][1]
            if partial >= total / 2:
                split = i + 1
                break
        
        for i in range(start, split):
            char, _ = items[i]
            codes[char] += "1"
        
        for i in range(split, end):
            char, _ = items[i]
            codes[char] += "0"
        
        left_node = shannon_split(start, split)
        right_node = shannon_split(split, end)
        
        return SFNode(
            [item[0] for item in items[start:end]],
            total,
            left_node,
            right_node
        )
    
    sf_tree_root = shannon_split(0, len(items))
    
    print_shannon_fano_tree(sf_tree_root)
    
    return codes

def print_shannon_fano_tree(node, prefix="", is_left=True):
    """Рекурсивный вывод дерева Шеннона-Фано"""
    if not node:
        return
    
    symbols_str = "".join(node.symbols) if len(node.symbols) <= 3 else f"{len(node.symbols)} символов"
    print(f"{prefix}{'├── ' if is_left else '└── '}{symbols_str} (p={node.prob:.4f})")
    
    if node.left:
        print_shannon_fano_tree(node.left, prefix + ("│   " if is_left else "    "), True)
    if node.right:
        print_shannon_fano_tree(node.right, prefix + ("│   " if is_left else "    "), False)

class Node:
    """Узел для дерева Хаффмана"""
    def __init__(self, symbol: str = "", probability: float = 0.0):
        self.symbol = symbol
        self.probability = probability
        self.left = None
        self.right = None
    
    def __lt__(self, other):
        return self.probability < other.probability

def huffman(table: Dict[str, float]) -> Dict[str, str]:
    """Кодирование Хаффмана"""
    nodes = [Node(char, prob) for char, prob in table.items()]
    
    while len(nodes) > 1:
        nodes.sort(key=lambda x: (x.probability, x.symbol))
        
        left = nodes.pop(0)
        right = nodes.pop(0)
        
        parent = Node(left.symbol + right.symbol, left.probability + right.probability)
        parent.left = left
        parent.right = right
        
        nodes.append(parent)
    
    root = nodes[0]
    
    codes = {}
    
    def build_codes(node: Node, prefix: str = "") -> None:
        if node.left is None and node.right is None:
            codes[node.symbol] = prefix
            return
        
        if node.left:
            build_codes(node.left, prefix + "0")
        if node.right:
            build_codes(node.right, prefix + "1")
    
    build_codes(root)
    
    print_huffman_tree(root)
    
    return codes

def print_huffman_tree(node: Node, prefix="", is_left=True):
    """Рекурсивный вывод дерева Хаффмана"""
    if not node:
        return
    
    if len(node.symbol) == 1:
        node_label = f"'{node.symbol}'"
    else:
        if len(node.symbol) > 5:
            node_label = f"'{node.symbol[:5]}...'"
        else:
            node_label = f"'{node.symbol}'"
    
    print(f"{prefix}{'├── ' if is_left else '└── '}{node_label} (p={node.probability:.4f})")
    
    if node.left:
        print_huffman_tree(node.left, prefix + ("│   " if is_left else "    "), True)
    if node.right:
        print_huffman_tree(node.right, prefix + ("│   " if is_left else "    "), False)

def encode(text: str, codes: Dict[str, str]) -> str:
    """Прямое кодирование"""
    return ''.join(codes[char] for char in text)

def decode(encoded: str, codes: Dict[str, str]) -> str:
    """Обратное кодирование"""
    reverse_codes = {code: char for char, code in codes.items()}
    
    result = []
    current = ""
    
    for bit in encoded:
        current += bit
        if current in reverse_codes:
            result.append(reverse_codes[current])
            current = ""
    
    return ''.join(result)

def calculate_entropy(table: Dict[str, float]) -> float:
    """Расчет энтропии"""
    entropy = 0.0
    for prob in table.values():
        if prob > 0:
            entropy -= prob * math.log2(prob)
    return entropy

def calculate_average_code_length(table: Dict[str, float], codes: Dict[str, str]) -> float:
    """Расчет средней длины кода"""
    avg_length = 0.0
    for char, code in codes.items():
        avg_length += len(code) * table[char]
    return avg_length

def main():
    text = "БабашинскийГлеб"
    print(f"Исходное сообщение: '{text}'")
    print(f"Длина сообщения: {len(text)} символов")
    
    dynamic_table = build_dynamic_table(text)
    print_table(dynamic_table, "ДИНАМИЧЕСКИЕ ВЕРОЯТНОСТИ")
    
    entropy = calculate_entropy(dynamic_table)
    print(f"\nЭнтропия сообщения: {entropy:.4f} бит/символ")
    
    print("\n" + "-" * 60)
    print("КОДИРОВАНИЕ ШЕННОНА-ФАНО")
    print("\nДерево Шеннона-Фано:")
    sf_codes = shannon_fano(dynamic_table)
    print("\nКоды символов:")
    for char, code in sf_codes.items():
        print(f"  '{char}' : {code}")
    
    print("\n" + "-" * 60)
    print("КОДИРОВАНИЕ ХАФФМАНА")
    print("\nДерево Хаффмана:")
    huff_codes = huffman(dynamic_table)
    print("\nКоды символов:")
    for char, code in huff_codes.items():
        print(f"  '{char}' : {code}")
    
    print("\n" + "-" * 60)
    print("ПРЯМОЕ КОДИРОВАНИЕ")
    
    sf_encoded = encode(text, sf_codes)
    hf_encoded = encode(text, huff_codes)
    
    print(f"\nЗакодированное сообщение (Шеннона-Фано):")
    print(f"  {sf_encoded}")
    print(f"  Длина: {len(sf_encoded)} бит")
    
    print(f"\nЗакодированное сообщение (Хаффмана):")
    print(f"  {hf_encoded}")
    print(f"  Длина: {len(hf_encoded)} бит")
    
    print("\n" + "-" * 60)
    print("ОБРАТНОЕ КОДИРОВАНИЕ")
    
    sf_decoded = decode(sf_encoded, sf_codes)
    hf_decoded = decode(hf_encoded, huff_codes)
    
    print(f"\nДекодирование Шеннона-Фано: '{sf_decoded}'")
    print(f"Совпадает с исходным: {sf_decoded == text}")
    
    print(f"\nДекодирование Хаффмана: '{hf_decoded}'")
    print(f"Совпадает с исходным: {hf_decoded == text}")
    
    ascii_bits = len(text) * 8
    avg_sf_length = calculate_average_code_length(dynamic_table, sf_codes)
    avg_huff_length = calculate_average_code_length(dynamic_table, huff_codes)
    sf_efficiency = (entropy / avg_sf_length) * 100 if avg_sf_length > 0 else 0
    huff_efficiency = (entropy / avg_huff_length) * 100 if avg_huff_length > 0 else 0

    print(f"\nASCII кодирование:")
    print(f"  Размер: {ascii_bits} бит ({len(text)} × 8)")
    
    print(f"\nШеннона-Фано:")
    print(f"  Размер: {len(sf_encoded)} бит")
    print(f"  Средняя длина кода: {avg_sf_length:.4f} бит/символ")
    print(f"  Коэффициент сжатия: {ascii_bits / len(sf_encoded):.2f}:1")
    print(f"  Избыточность: {(avg_sf_length - entropy):.4f} бит/символ")
    print(f"  Эффективность: {sf_efficiency:.2f}%")

    print(f"\nХаффмана:")
    print(f"  Размер: {len(hf_encoded)} бит")
    print(f"  Средняя длина кода: {avg_huff_length:.4f} бит/символ")
    print(f"  Коэффициент сжатия: {ascii_bits / len(hf_encoded):.2f}:1")
    print(f"  Избыточность: {(avg_huff_length - entropy):.4f} бит/символ")
    print(f"  Эффективность: {huff_efficiency:.2f}%")

    input("\nНажмите Enter для выхода...")

if __name__ == "__main__":
    main()