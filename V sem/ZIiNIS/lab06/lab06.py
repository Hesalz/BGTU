import random

class lab06:
    def __init__(self, g_poly_str="10101", n=15):
        self.g_poly_str = g_poly_str
        self.g_bits = [int(b) for b in self.g_poly_str]
        self.r = len(self.g_bits) - 1
        self.n = n
        self.k = self.n - self.r

        print(f"Порождающий полином: g(x) = {self.g_poly_str}")
        print(f"Параметры кода: ({self.n}, {self.k})")
        print(f"r = {self.r} (избыточные символы)")
        print(f"k = {self.k} (информационные символы)")
        print(f"n = {self.n} (длина кодового слова)\n")

        self.g_int = self.poly_bits_to_int(self.g_bits)
        self.g_deg = len(self.g_bits) - 1 

        self.build_systematic_generator()
        self.build_check_matrix_rows()

    def poly_bits_to_int(self, bits):
        val = 0
        for b in bits:
            val = (val << 1) | (1 if b else 0)
        return val

    def int_to_bits(self, value, length):
        return [(value >> (length - 1 - i)) & 1 for i in range(length)]

    def array_to_str(self, arr):
        return ''.join(str(int(x)) for x in arr)

    def remainder_x_pow_mod_g_int(self, power):
        """
        Вычисляет x^power mod g(x). Возвращает целое число (остаток).
        power: неотрицательное целое
        """
        poly = 1 << power
        while (poly.bit_length() - 1) >= self.g_deg:
            shift = (poly.bit_length() - 1) - self.g_deg
            poly ^= (self.g_int << shift)
        return poly

    def remainder_list_mod_g(self, dividend_bits):
        """
        Возвращает остаток как список бит длины r.
        dividend_bits: список бит (старшая степень слева)
        """
        val = self.poly_bits_to_int(dividend_bits)
        if val == 0:
            return [0] * self.r
        power = val.bit_length() - 1
        work = val
        while (work.bit_length() - 1) >= self.g_deg:
            shift = (work.bit_length() - 1) - self.g_deg
            work ^= (self.g_int << shift)
        rem_int = work
        return self.int_to_bits(rem_int, self.r)

    def encode_message(self, message_bits):
        if len(message_bits) != self.k:
            raise ValueError(f"Длина сообщения должна быть {self.k} бит")
        m_int = self.poly_bits_to_int(message_bits)
        shifted = m_int << self.r 
        rem = shifted
        work = shifted
        while (work.bit_length() - 1) >= self.g_deg:
            shift = (work.bit_length() - 1) - self.g_deg
            work ^= (self.g_int << shift)
        rem_int = work
        code_int = shifted ^ rem_int
        return self.int_to_bits(code_int, self.n)

    def build_systematic_generator(self):
        self.G_canonical = []
        for i in range(self.k):
            msg = [0] * self.k
            msg[i] = 1
            cw = self.encode_message(msg)
            self.G_canonical.append(cw)
        print("-" * 70)
        print("Порождающая матрица G (исходная):")
        for row in self.G_canonical:
            print("  " + self.array_to_str(row))
        print("\nПорождающая матрица G (каноническая форма [I|P]):")
        for row in self.G_canonical:
            print("  " + self.array_to_str(row))

    def build_check_matrix_rows(self):
        self.H_columns = []
        for pos in range(self.n):
            power = self.n - 1 - pos
            rem_int = self.remainder_x_pow_mod_g_int(power)
            rem_bits = self.int_to_bits(rem_int, self.r)
            self.H_columns.append(tuple(rem_bits))

        print("\nПроверочная матрица H ({}×{}):".format(self.r, self.n))
        print("(строки — биты синдрома, столбцы — позиции ошибок)\n")
        header = "     "
        for i in range(self.n):
            header += f"{i:2d} "
        print(header)
        print("    " + "-" * (3 * self.n))
        for bit_pos in range(self.r):
            row = f"бит {bit_pos}: "
            for pos in range(self.n):
                row += f" {self.H_columns[pos][bit_pos]} "
            print(row)

        unique_count = len(set(self.H_columns))
       # print(f"\nКоличество уникальных синдромов: {unique_count} (ожидается {self.n} для полной различимости одиночных ошибок)")

    def calculate_syndrome(self, received_bits):
        val = self.poly_bits_to_int(received_bits)
        work = val
        while (work.bit_length() - 1) >= self.g_deg:
            shift = (work.bit_length() - 1) - self.g_deg
            work ^= (self.g_int << shift)
        rem_int = work
        return self.int_to_bits(rem_int, self.r)

    def find_error_position(self, syndrome_bits):
        t = tuple(syndrome_bits)
        for pos in range(self.n):
            if self.H_columns[pos] == t:
                return pos 
        return -1

    def generate_test_cases(self, original_codeword):
        for errors_count in range(3):
            print(f"\n{errors_count} ошибок в кодовом слове")
            received = original_codeword.copy()
            error_positions = random.sample(range(self.n), errors_count)
            error_vector = [0] * self.n
            for p in error_positions:
                error_vector[p] = 1
                received[p] ^= 1
            print(f"Исходное кодовое слово:  {self.array_to_str(original_codeword)}")
            print(f"Принятое слово Yn:       {self.array_to_str(received)}")
            print(f"Унарный вектор ошибок:   {self.array_to_str(error_vector)}")
            syndrome = self.calculate_syndrome(received)
            print(f"Вычисленный синдром:     {self.array_to_str(syndrome)}")
            if errors_count == 0:
                if all(b == 0 for b in syndrome):
                    print("РЕЗУЛЬТАТ: Ошибок не обнаружено")
                else:
                    print("РЕЗУЛЬТАТ: Обнаружена ошибка (ложное срабатывание)")
            elif errors_count == 1:
                pos = self.find_error_position(syndrome)
                if pos != -1:
                    print(f"РЕЗУЛЬТАТ: Обнаружена одиночная ошибка в позиции {pos}")
                    corrected = received.copy()
                    corrected[pos] ^= 1
                    print(f"Исправленное слово:      {self.array_to_str(corrected)}")
                    if corrected == original_codeword:
                        print("СТАТУС: Ошибка успешно исправлена!")
                    else:
                        print("СТАТУС: Ошибка исправлена некорректно")
                else:
                    print("РЕЗУЛЬТАТ: Ошибка не локализована (синдром не соответствует ни одному столбцу H)")
            else:
                pos = self.find_error_position(syndrome)
                if pos != -1:
                    print(f"РЕЗУЛЬТАТ: Синдром совпал с одиночной ошибкой в позиции {pos}")
                    print("СТАТУС: Обнаружены множественные ошибки - исправление по одиночному синдрому некорректно")
                else:
                    print("РЕЗУЛЬТАТ: Обнаружены множественные ошибки")
                    print("СТАТУС: Код может только обнаружить, но не исправить двойные ошибки")

def main():
    random.seed(0)
    code = lab06(g_poly_str="10101", n=15)

    print("\n")
    info_word = [random.randint(0, 1) for _ in range(code.k)]
    print(f"Информационное слово Xk: {code.array_to_str(info_word)}")
    print(f"Длина: {code.k} бит")
    codeword = code.encode_message(info_word)
    print(f"Кодовое слово Xn:        {code.array_to_str(codeword)}")
    print(f"Длина: {code.n} бит")
    print(f"Проверочные биты:        {code.array_to_str(codeword[code.k:])}")
    syndrome_check = code.calculate_syndrome(codeword)
    print(f"Проверка синдрома:       {code.array_to_str(syndrome_check)}")
    if all(b == 0 for b in syndrome_check):
        print("СТАТУС КОДИРОВАНИЯ: Успешно (синдром нулевой)")
    else:
        print("СТАТУС КОДИРОВАНИЯ: Ошибка!")

    code.generate_test_cases(codeword)

if __name__ == "__main__":
    main()
