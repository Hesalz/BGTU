import math
import time

# ===================== Triple =====================

class Triple:
    def __init__(self, offset, length, nxt):
        self.Offset = offset
        self.Length = length
        self.Next = nxt


# ===================== Helpers =====================

def display_char(c: str):
    if c == "\r": return "␍"
    if c == "\n": return "␊"
    if ord(c) < 32: return "."
    return c


def build_dictionary(data, pos, n1):
    out = []
    start = pos - n1
    for i in range(n1):
        idx = start + i
        out.append(display_char(data[idx]) if 0 <= idx < len(data) else ".")
    return "".join(out)


def build_buffer(data, pos, n2):
    out = []
    for i in range(n2):
        idx = pos + i
        out.append(display_char(data[idx]) if 0 <= idx < len(data) else ".")
    return "".join(out)


# ===================== Compression =====================

def lz77_compress(data, n1, n2):
    triples = []
    pos = 0
    step = 0
    chars = list(data)

    while pos < len(chars):
        step += 1
        print("-" * 60)
        print(f"Шаг {step}")
        print("Словарь:", build_dictionary(chars, pos, n1))
        print("Буфер:  ", build_buffer(chars, pos, n2))

        bestLen = 0
        bestOff = 0
        searchStart = max(0, pos - n1)
        maxLook = min(n2, len(chars) - pos)

        for i in range(searchStart, pos):
            k = 0
            while k < maxLook and i + k < pos and pos + k < len(chars) and chars[i + k] == chars[pos + k]:
                k += 1
            if k > bestLen or (k == bestLen and k > 0 and (pos - i) < bestOff):
                bestLen = k
                bestOff = pos - i

        nxt = chars[pos + bestLen] if pos + bestLen < len(chars) else None

        triples.append(Triple(bestOff, bestLen, nxt))
        print(f"Триада: p={bestOff}, q={bestLen}, s={nxt if nxt else ''}")

        pos += bestLen + 1

    print("-" * 60)
    return triples


def build_C_string(triples):
    C = []
    for t in triples:
        p = str(t.Offset)
        q = str(t.Length)
        s = t.Next if t.Next else ''
        C.append(p + q + s)
    return "".join(C)


# ===================== Decompression =====================

def parse_C(C):
    triples = []
    i = 0
    while i < len(C):
        if not C[i].isdigit():
            print(f"Ошибка: строка C должна быть результатом сжатия.")
            return []
        if not C[i].isdigit():
            print(f"Ошибка: строка C должна быть результатом сжатия.")
            return []
        p = int(C[i]); i += 1

        if i >= len(C) or not C[i].isdigit():
            q = 0
        else:
            q = int(C[i])
            i += 1

        if i < len(C) and not C[i].isdigit():
            s = C[i]
            i += 1
        else:
            s = None

        triples.append(Triple(p, q, s))

    return triples


def lz77_decompress(triples):
    out = []
    step = 0
    for t in triples:
        step += 1
        print("-" * 60)
        print(f"Шаг {step}")
        print(f"Триада: p={t.Offset}, q={t.Length}, s={t.Next if t.Next else ''}")

        if t.Offset == 0 or t.Length == 0:
            print("Случай: нет ссылки в словарь (p=0 или q=0)")
            if t.Next is not None:
                print(f"Добавляем символ: {t.Next}")
                out.append(t.Next)
            continue

        start = len(out) - t.Offset
        if start < 0: start = 0
        print(f"Копируем {t.Length} символов из позиции {start} (смещение {t.Offset})")

        for k in range(t.Length):
            idx = start + k
            copied = out[idx] if 0 <= idx < len(out) else '\x00'
            print(f"  копия[{k}]: {copied}")
            out.append(copied)

        if t.Next is not None:
            print(f"Добавляем следующий символ: {t.Next}")
            out.append(t.Next)

        print(f"Текущее состояние выходной строки: {''.join(out)}")

    return "".join(out)


# ===================== Interactive =====================

def compress_interactive():
    print("Введите текст:")
    data = input()

    print("Введите n1 (размер окна словаря):")
    n1 = int(input() or 16)
    print("Введите n2 (размер буфера просмотра):")
    n2 = int(input() or 8)

    start = time.time()
    triples = lz77_compress(data, n1, n2)
    elapsed = (time.time() - start) * 1000

    C = build_C_string(triples)
    print("C:", C)

    R1 = len(C) / len(data) * 100
    R2 = 100 - R1
    print(f"R1 = {R1:.2f}%  (доля после сжатия)")
    print(f"R2 = {R2:.2f}%  (степень сжатия)")

    print(f"Время: {elapsed:.3f} мс")



def decompress_interactive():
    print("Введите строку C:")
    C = input().strip()

    print("Введите n1 (размер окна словаря):")
    n1 = int(input() or 16)

    print("Введите n2 (размер буфера просмотра):")
    n2 = int(input() or 8)

    triples = parse_C(C)
    print("Восстановление...")
    out = lz77_decompress(triples)
    print("Текст:", out)



def main():
    while True:
        print("===== СЖАТИЕ/РАСПАКОВКА ДАННЫХ МЕТОДОМ ЛЕМПЕЛЯ − ЗИВА (LZ77) =====")
        print("1. Сжатие")
        print("2. Распаковка")
        print("3. Выход")
        cmd = input()
        if cmd == "1": compress_interactive()
        elif cmd == "2": decompress_interactive()
        elif cmd == "3": return
        else: print("Ошибка")

if __name__ == "__main__":
    main()
