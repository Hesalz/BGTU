def let2num(c):
    return ord(c) - ord('A')

def num2let(n):
    return chr(n + ord('A'))

rotor_data = {
    'I':    [4,10,12,5,11,6,3,16,21,25,13,19,14,22,24,7,23,20,18,15,0,8,1,17,2,9],
    'II':   [0,9,3,10,18,8,17,20,23,1,11,7,22,19,12,2,16,6,25,13,15,24,5,21,14,4],
    'III':  [1,3,5,7,9,11,2,15,17,19,23,21,25,13,24,4,8,22,6,0,10,12,20,18,16,14],
    'IV':   [4,18,14,21,15,25,9,0,24,16,20,8,17,7,23,11,13,5,19,6,10,3,2,12,22,1],
    'V':    [21,25,1,17,6,8,19,24,20,15,18,3,13,7,11,23,0,22,12,9,16,14,5,4,2,10],
    'VI':   [9,15,6,21,14,20,12,5,24,16,1,4,13,7,25,17,3,10,0,18,23,11,8,2,19,22],
    'VII':  [13,25,9,7,6,17,2,23,12,24,18,22,1,14,20,5,0,8,21,11,15,4,10,16,3,19],
    'VIII': [5,10,16,7,19,11,23,14,2,1,9,18,15,3,25,17,0,12,4,22,13,8,20,24,6,21],
    'Beta': [11,4,24,9,21,2,13,8,23,22,15,1,16,12,3,17,19,0,10,25,6,5,20,7,14,18],
    'Gamma':[5,18,14,10,0,13,20,4,17,7,12,1,19,8,24,2,22,11,16,15,25,23,21,6,9,3]
}

def make_reflector(pairs):
    ref = [0]*26
    for a,b in pairs:
        i = let2num(a)
        j = let2num(b)
        ref[i] = j
        ref[j] = i
    return ref

reflector_data = {
    'B': make_reflector([('A','Y'), ('B','R'), ('C','U'), ('D','H'), ('E','Q'),
                          ('F','S'), ('G','L'), ('I','P'), ('J','X'), ('K','N'),
                          ('M','O'), ('T','Z'), ('V','W')]),
    'C': make_reflector([('A','F'), ('B','V'), ('C','P'), ('D','J'), ('E','I'),
                          ('G','O'), ('H','Y'), ('K','R'), ('L','Z'), ('M','X'),
                          ('N','W'), ('T','Q'), ('S','U')]),
    'B Dünn': make_reflector([('A','E'), ('B','N'), ('C','K'), ('D','Q'), ('F','U'),
                               ('G','Y'), ('H','W'), ('I','J'), ('L','O'), ('M','P'),
                               ('R','X'), ('S','Z'), ('T','V')]),
    'C Dünn': make_reflector([('A','R'), ('B','D'), ('C','O'), ('E','J'), ('F','N'),
                               ('G','T'), ('H','K'), ('I','V'), ('L','M'), ('P','W'),
                               ('Q','Z'), ('S','X'), ('U','Y')])
}

class Rotor:
    def __init__(self, wiring):
        self.wiring = wiring[:]      
        self.inv = [0]*26
        for i, out in enumerate(wiring):
            self.inv[out] = i
        self.pos = 0       

    def set_position(self, p):
        self.pos = p % 26

    def forward(self, c):
        idx = (c + self.pos) % 26
        out = self.wiring[idx]
        return (out - self.pos) % 26

    def backward(self, c):
        idx = (c + self.pos) % 26
        out = self.inv[idx]
        return (out - self.pos) % 26

class Enigma:
    def __init__(self, left_rotor, middle_rotor, right_rotor, reflector,
                 step_left, step_mid, step_right):
        self.left = left_rotor
        self.mid = middle_rotor
        self.right = right_rotor
        self.ref = reflector
        self.sL = step_left
        self.sM = step_mid
        self.sR = step_right
        self.encrypt_count = 0 

    def set_positions(self, l, m, r):
        self.left.set_position(l)
        self.mid.set_position(m)
        self.right.set_position(r)
        self.encrypt_count = 0

    def step_rotors(self):
        stepsR = self.sR
        wrapsR = (self.right.pos + stepsR) // 26
        self.right.pos = (self.right.pos + stepsR) % 26

        stepsM = self.sM + wrapsR
        wrapsM = (self.mid.pos + stepsM) // 26
        self.mid.pos = (self.mid.pos + stepsM) % 26

        stepsL = self.sL + wrapsM
        self.left.pos = (self.left.pos + stepsL) % 26

    def encrypt_letter(self, letter):
        self.step_rotors()
        self.encrypt_count += 1

        c = let2num(letter)

        c = self.right.forward(c)
        c = self.mid.forward(c)
        c = self.left.forward(c)

        c = self.ref[c]

        c = self.left.backward(c)
        c = self.mid.backward(c)
        c = self.right.backward(c)

        return num2let(c)

    def encrypt(self, text):
        result = []
        for ch in text.upper():
            if 'A' <= ch <= 'Z':
                result.append(self.encrypt_letter(ch))
            else:
                result.append(ch)
        return ''.join(result)


def evaluate_cryptostrength():
    rotor_choices = 10
    rotor_positions = 26**3
    reflector_choices = 4
    
    key_space = rotor_choices * rotor_positions * reflector_choices
    print(f"Размер ключевого пространства: {key_space:,} комбинаций")


def main():
    rotor_III = Rotor(rotor_data['III'])
    rotor_VII = Rotor(rotor_data['VII']) 
    rotor_I = Rotor(rotor_data['I']) 

    ref_B_Dunn = reflector_data['B Dünn']
    
    enigma = Enigma(rotor_III, rotor_VII, rotor_I, ref_B_Dunn, 1, 0, 1)
    
    print("Конфигурация: L=III, M=VII, R=I, Re=B Dünn, шаги 1-0-1")
    
    while True:
        print("\nМеню:")
        print("1. Установить начальные позиции роторов")
        print("2. Зашифровать сообщение")
        print("3. Показать текущие позиции роторов")
        print("4. Оценить криптостойкость")
        print("5. Выход")
        
        choice = input("\nВыберите действие (1-5): ").strip()
        
        if choice == '1':
            try:
                l = input("Позиция левого ротора (A-Z): ").strip().upper()
                m = input("Позиция среднего ротора (A-Z): ").strip().upper()
                r = input("Позиция правого ротора (A-Z): ").strip().upper()
                
                if len(l) == 1 and len(m) == 1 and len(r) == 1:
                    if l in 'ABCDEFGHIJKLMNOPQRSTUVWXYZ' and \
                       m in 'ABCDEFGHIJKLMNOPQRSTUVWXYZ' and \
                       r in 'ABCDEFGHIJKLMNOPQRSTUVWXYZ':
                        enigma.set_positions(let2num(l), let2num(m), let2num(r))
                        print(f"Позиции установлены: L={l}, M={m}, R={r}")
                    else:
                        print("Ошибка: введите буквы от A до Z")
                else:
                    print("Ошибка: введите по одной букве")
            except Exception as e:
                print(f"Ошибка ввода: {e}")
        
        elif choice == '2':
            text = input("Введите текст для шифрования: ").strip()
            if text:
                start_pos = (enigma.left.pos, enigma.mid.pos, enigma.right.pos)
                cipher = enigma.encrypt(text)
                end_pos = (enigma.left.pos, enigma.mid.pos, enigma.right.pos)
                
                print(f"\nИсходный текст: {text}")
                print(f"Зашифрованный текст: {cipher}")
                print(f"\nСтатистика:")
                print(f"- Зашифровано символов: {enigma.encrypt_count}")
                print(f"- Начальные позиции: L={num2let(start_pos[0])}, M={num2let(start_pos[1])}, R={num2let(start_pos[2])}")
                print(f"- Конечные позиции: L={num2let(end_pos[0])}, M={num2let(end_pos[1])}, R={num2let(end_pos[2])}")
                
                enigma.set_positions(start_pos[0], start_pos[1], start_pos[2])
                decrypted = enigma.encrypt(cipher)
                if decrypted == text.upper():
                    print("- Проверка: шифр обратим (расшифровка работает)")
                else:
                    print("- ВНИМАНИЕ: проблема с обратимостью!")
            else:
                print("Текст не может быть пустым")
        
        elif choice == '3':
            print(f"\nТекущие позиции роторов:")
            print(f"Левый (III): {num2let(enigma.left.pos)}")
            print(f"Средний (VII): {num2let(enigma.mid.pos)}")
            print(f"Правый (I): {num2let(enigma.right.pos)}")
            print(f"Зашифровано символов с последней установки: {enigma.encrypt_count}")
        
        elif choice == '4':
            evaluate_cryptostrength()
        
        elif choice == '5':
            print("Выход из программы.")
            break
        
        else:
            print("Неверный выбор. Пожалуйста, выберите 1-5.")

if __name__ == "__main__":
    main()