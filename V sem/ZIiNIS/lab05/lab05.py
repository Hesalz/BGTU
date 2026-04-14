import math
from typing import List, Tuple

class IterativeCode:
    @staticmethod
    def msg_to_2dim_matrix(msg: List[int], height: int, width: int) -> List[List[int]]:
        """Преобразует сообщение в двумерную матрицу"""
        if len(msg) != width * height:
            raise ValueError("Размеры матрицы не соответствуют размерам сообщения")
        
        matrix = []
        for i in range(width):
            row = []
            for j in range(height):
                row.append(msg[i * height + j])
            matrix.append(row)
        return matrix

    @staticmethod
    def calculate_check_bits(matrix: List[List[int]]) -> List[int]:
        """Вычисляет контрольные биты для матрицы"""
        width = len(matrix)
        height = len(matrix[0])
        
        bits = [0] * (width + height + 1)
        
        # Вертикальные паритеты (столбцы)
        for i in range(height):
            col_sum = 0
            for j in range(width):
                col_sum += matrix[j][i]
            bits[i] = col_sum % 2
        
        # Горизонтальные паритеты (строки)
        all_sum = 0
        for i in range(width):
            row_sum = 0
            for j in range(height):
                row_sum += matrix[i][j]
            all_sum += row_sum
            bits[i + height] = row_sum % 2
        
        # Суперпаритет
        for i in range(len(bits) - 1):
            all_sum += bits[i]
        bits[-1] = all_sum % 2
        
        return bits

    @staticmethod
    def find_error_positions(matrix: List[List[int]], check_bits: List[int]) -> List[int]:
        """Находит позиции ошибок в матрице"""
        check_bits_for_matrix = IterativeCode.calculate_check_bits(matrix)
        
        width = len(matrix)
        height = len(matrix[0])
        
        row_mismatch = []
        col_mismatch = []
        
        # Проверяем вертикальные паритеты
        for i in range(height):
            if check_bits[i] != check_bits_for_matrix[i]:
                col_mismatch.append(i)
        
        # Проверяем горизонтальные паритеты
        for i in range(width):
            if check_bits[i + height] != check_bits_for_matrix[i + height]:
                row_mismatch.append(i)
        
        # Находим пересечения ошибочных строк и столбцов
        result = []
        for row in row_mismatch:
            for col in col_mismatch:
                result.append(row * height + col)
        
        return result

class Printer:
    @staticmethod
    def print_matrix(msg: str, matrix: List[List[int]], reverse: bool = True) -> None:
        """Печатает матрицу"""
        print(msg)
        if reverse:
            for i in range(len(matrix[0])):
                print("    ", end="")
                for j in range(len(matrix)):
                    print(matrix[j][i], end="")
                print()
        else:
            for i in range(len(matrix)):
                print("    ", end="")
                for j in range(len(matrix[i])):
                    print(matrix[i][j], end="")
                print()

    @staticmethod
    def print_bits(msg: str, bits: List[int]) -> None:
        """Печатает битовую последовательность"""
        print(f"{msg}", end="")
        for bit in bits:
            print(bit, end="")
        print()

def main():
    msg = [1, 1, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 1, 0, 0,
           1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 1]

    Printer.print_bits("Сообщение: ", msg)

    matrix = IterativeCode.msg_to_2dim_matrix(msg, 5, 8)
    check_bits = IterativeCode.calculate_check_bits(matrix)

    Printer.print_matrix("Двумерная матрица:", matrix, False)
    print()

    cols = len(matrix[0])  # количество столбцов
    rows = len(matrix)     # количество строк

    print("Горизонтальные паритеты (Xh): ", end="")
    for i in range(cols, cols + rows):
        print(check_bits[i], end="")
    print()

    print("Вертикальные паритеты (Xv): ", end="")
    for i in range(cols):
        print(check_bits[i], end="")
    print()

    print(f"Суперпаритет (Xhv): {check_bits[-1]}")
    print()

    Printer.print_bits("Все контрольные биты Xr= ", check_bits)
    print("=========================================\n")

    # === Далее начинается часть тестирования ===
    N1 = 0
    N2 = 0
    N3 = 0

    while True:
        print("Введите номера битов с ошибками через пробел (Enter — завершить): ")
        input_str = input().strip()
        if not input_str:
            break

        N1 += 1
        test_msg = msg.copy()
        parts = input_str.split()

        for p in parts:
            try:
                pos = int(p)
                if 0 <= pos < len(test_msg):
                    test_msg[pos] = 0 if test_msg[pos] == 1 else 1
            except ValueError:
                continue

        new_matrix = IterativeCode.msg_to_2dim_matrix(test_msg, 5, 8)
        Printer.print_matrix("Матрица с ошибками:", new_matrix, False)

        found_errors = IterativeCode.find_error_positions(new_matrix, check_bits)

        if found_errors:
            print("Найдены ошибки в позициях: ", end="")
            for pos in found_errors:
                print(f"{pos} ", end="")
            print()
        else:
            print("Ошибки не найдены.")

        # Проверяем правильность определения кратности
        if len(found_errors) == len(parts):
            N2 += 1

        # Проверяем, все ли ошибки корректно определены
        expected_errors = [int(p) for p in parts]
        all_corrected = all(error in expected_errors for error in found_errors)
        if all_corrected and len(found_errors) == len(expected_errors):
            N3 += 1

        print("----------------------------------")

    print("\nРЕЗУЛЬТАТЫ ЭКСПЕРИМЕНТОВ:")
    print(f"N1 = {N1}  (всего экспериментов)")
    print(f"N2 = {N2}  (правильно определена кратность)")
    print(f"N3 = {N3}  (все ошибки корректно определены)")

    if N1 > 0:
        print(f"N2/N1 = {N2 / N1:.2f}")
        print(f"N3/N1 = {N3 / N1:.2f}")

    print("\nРабота завершена.")

if __name__ == "__main__":
    main()