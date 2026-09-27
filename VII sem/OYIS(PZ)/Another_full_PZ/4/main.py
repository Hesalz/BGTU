#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Стеганографические скрипты для скрытия сообщений в тексте
Поддерживает два метода: пробелы и невидимые символы
"""

def space_stego_embed(text, message):
    """
    Встраивает сообщение в текст используя количество пробелов между словами.
    1 пробел = бит '0', 2 пробела = бит '1'
    
    Args:
        text (str): Исходный текст-контейнер
        message (str): Сообщение для скрытия
    
    Returns:
        str: Текст со скрытым сообщением
    """
    # Преобразуем сообщение в двоичный код
    binary = ''.join(format(b, '08b') for b in message.encode('utf-8'))
    words = text.split()
    
    if len(words) - 1 < len(binary):
        raise ValueError(f"Текст слишком короткий. Нужно минимум {len(binary)} промежутков между словами, есть {len(words) - 1}")
    
    result = []
    i = 0
    
    for word in words[:-1]:  # Все слова кроме последнего
        result.append(word)
        if i < len(binary):
            if binary[i] == '0':
                result.append(' ')      # Один пробел для '0'
            else:
                result.append('  ')     # Два пробела для '1'
            i += 1
        else:
            result.append(' ')          # Обычный пробел для остальных слов
    
    result.append(words[-1])  # Последнее слово
    return ''.join(result)


def space_stego_extract(stego_text):
    """
    Извлекает скрытое сообщение из текста с пробелами.
    
    Args:
        stego_text (str): Текст со скрытым сообщением
    
    Returns:
        str: Извлеченное сообщение
    """
    words = stego_text.split()
    binary_bits = []
    
    # Анализируем пробелы между словами
    current_pos = 0
    for i, word in enumerate(words[:-1]):
        current_pos += len(word)
        
        # Подсчитываем пробелы до следующего слова
        space_count = 0
        while current_pos + space_count < len(stego_text) and stego_text[current_pos + space_count] == ' ':
            space_count += 1
        
        if space_count == 1:
            binary_bits.append('0')
        elif space_count == 2:
            binary_bits.append('1')
        else:
            # Если пробелов больше 2 или 0, прекращаем извлечение
            break
            
        current_pos += space_count
    
    # Преобразуем биты обратно в символы
    binary_string = ''.join(binary_bits)
    message_bytes = []
    
    for i in range(0, len(binary_string), 8):
        byte = binary_string[i:i+8]
        if len(byte) == 8:
            message_bytes.append(int(byte, 2))
        else:
            break
    
    try:
        message = bytes(message_bytes).decode('utf-8')
    except UnicodeDecodeError:
        message = bytes(message_bytes).decode('utf-8', errors='ignore')
    
    return message


def zero_width_embed(text, message):
    """
    Встраивает сообщение используя невидимые символы нулевой ширины.
    ZWNJ (\u200C) = бит '0', ZWJ (\u200D) = бит '1'
    
    Args:
        text (str): Исходный текст-контейнер
        message (str): Сообщение для скрытия
    
    Returns:
        str: Текст со скрытым сообщением
    """
    # Преобразуем сообщение в двоичный код
    binary = ''.join(format(b, '08b') for b in message.encode('utf-8'))
    
    if len(text) < len(binary):
        raise ValueError(f"Текст слишком короткий. Нужно минимум {len(binary)} символов, есть {len(text)}")
    
    result = []
    binary_index = 0
    
    for char in text:
        result.append(char)
        if binary_index < len(binary):
            bit = binary[binary_index]
            if bit == '0':
                result.append('\u200C')  # ZWNJ для '0'
            else:
                result.append('\u200D')  # ZWJ для '1'
            binary_index += 1
    
    return ''.join(result)


def zero_width_extract(stego_text):
    """
    Извлекает скрытое сообщение из текста с невидимыми символами.
    
    Args:
        stego_text (str): Текст со скрытым сообщением
    
    Returns:
        str: Извлеченное сообщение
    """
    binary_bits = []
    
    for char in stego_text:
        if char == '\u200C':  # ZWNJ
            binary_bits.append('0')
        elif char == '\u200D':  # ZWJ
            binary_bits.append('1')
    
    # Преобразуем биты обратно в символы
    binary_string = ''.join(binary_bits)
    message_bytes = []
    
    for i in range(0, len(binary_string), 8):
        byte = binary_string[i:i+8]
        if len(byte) == 8:
            message_bytes.append(int(byte, 2))
        else:
            break
    
    try:
        message = bytes(message_bytes).decode('utf-8')
    except UnicodeDecodeError:
        message = bytes(message_bytes).decode('utf-8', errors='ignore')
    
    return message


def unicode_invisible_embed(text, message):
    """
    Альтернативный метод с использованием различных невидимых символов Unicode.
    Использует больше символов для лучшего сокрытия.
    """
    # Словарь невидимых символов для представления битов
    invisible_chars = {
        '00': '\u200B',  # Zero Width Space
        '01': '\u200C',  # Zero Width Non-Joiner
        '10': '\u200D',  # Zero Width Joiner
        '11': '\uFEFF',  # Zero Width No-Break Space
    }
    
    # Преобразуем сообщение в двоичный код
    binary = ''.join(format(b, '08b') for b in message.encode('utf-8'))
    
    # Добавляем padding если необходимо
    if len(binary) % 2 != 0:
        binary += '0'
    
    if len(text) < len(binary) // 2:
        raise ValueError(f"Текст слишком короткий для сообщения")
    
    result = []
    binary_index = 0
    
    for char in text:
        result.append(char)
        if binary_index < len(binary) - 1:
            bit_pair = binary[binary_index:binary_index + 2]
            result.append(invisible_chars[bit_pair])
            binary_index += 2
    
    return ''.join(result)


def unicode_invisible_extract(stego_text):
    """
    Извлекает сообщение, скрытое методом unicode_invisible_embed.
    """
    char_to_bits = {
        '\u200B': '00',  # Zero Width Space
        '\u200C': '01',  # Zero Width Non-Joiner
        '\u200D': '10',  # Zero Width Joiner
        '\uFEFF': '11',  # Zero Width No-Break Space
    }
    
    binary_bits = []
    
    for char in stego_text:
        if char in char_to_bits:
            binary_bits.append(char_to_bits[char])
    
    # Преобразуем пары битов обратно в символы
    binary_string = ''.join(binary_bits)
    message_bytes = []
    
    for i in range(0, len(binary_string), 8):
        byte = binary_string[i:i+8]
        if len(byte) == 8:
            try:
                message_bytes.append(int(byte, 2))
            except ValueError:
                break
        else:
            break
    
    try:
        message = bytes(message_bytes).decode('utf-8')
    except UnicodeDecodeError:
        message = bytes(message_bytes).decode('utf-8', errors='ignore')
    
    return message


def demonstrate_steganography():
    """
    Демонстрация работы всех методов стеганографии.
    """
    original_text = "Это простой пример текста для демонстрации стеганографии в Python программировании и информационной безопасности и защите данных."
    secret_message = "Hi"  # Короткое сообщение для демонстрации
    
    print("=== ДЕМОНСТРАЦИЯ СТЕГАНОГРАФИИ ===\n")
    print(f"Исходный текст: '{original_text}'")
    print(f"Секретное сообщение: '{secret_message}'")
    print()
    
    # Метод с пробелами
    print("1. МЕТОД С ПРОБЕЛАМИ:")
    try:
        stego_space = space_stego_embed(original_text, secret_message)
        print(f"Текст со скрытым сообщением: '{stego_space}'")
        
        extracted_space = space_stego_extract(stego_space)
        print(f"Извлеченное сообщение: '{extracted_space}'")
        print(f"Совпадает: {secret_message == extracted_space}")
        print()
    except ValueError as e:
        print(f"Ошибка: {e}\n")
    
    # Метод с невидимыми символами
    print("2. МЕТОД С НЕВИДИМЫМИ СИМВОЛАМИ:")
    try:
        stego_zero = zero_width_embed(original_text, secret_message)
        print(f"Текст со скрытым сообщением: '{stego_zero}' (невидимые символы не отображаются)")
        print(f"Длина исходного текста: {len(original_text)}")
        print(f"Длина текста со скрытым сообщением: {len(stego_zero)}")
        
        extracted_zero = zero_width_extract(stego_zero)
        print(f"Извлеченное сообщение: '{extracted_zero}'")
        print(f"Совпадает: {secret_message == extracted_zero}")
        print()
    except ValueError as e:
        print(f"Ошибка: {e}\n")
    
    # Расширенный метод с Unicode
    print("3. РАСШИРЕННЫЙ UNICODE МЕТОД:")
    try:
        stego_unicode = unicode_invisible_embed(original_text, secret_message)
        print(f"Текст со скрытым сообщением создан (невидимые символы не отображаются)")
        print(f"Длина исходного текста: {len(original_text)}")
        print(f"Длина текста со скрытым сообщением: {len(stego_unicode)}")
        
        extracted_unicode = unicode_invisible_extract(stego_unicode)
        print(f"Извлеченное сообщение: '{extracted_unicode}'")
        print(f"Совпадает: {secret_message == extracted_unicode}")
    except ValueError as e:
        print(f"Ошибка: {e}")
    
    # Дополнительная демонстрация с русским текстом
    print("\n4. ТЕСТ С РУССКИМ СООБЩЕНИЕМ:")
    russian_message = "Тест"
    try:
        stego_russian = unicode_invisible_embed(original_text, russian_message)
        extracted_russian = unicode_invisible_extract(stego_russian)
        print(f"Русское сообщение: '{russian_message}'")
        print(f"Извлеченное сообщение: '{extracted_russian}'")
        print(f"Совпадает: {russian_message == extracted_russian}")
    except ValueError as e:
        print(f"Ошибка: {e}")


def analyze_text_capacity(text):
    """
    Анализирует возможности текста для сокрытия сообщений.
    """
    words = text.split()
    chars = len(text)
    spaces = len(words) - 1
    
    print(f"АНАЛИЗ ЕМКОСТИ ТЕКСТА:")
    print(f"Символов в тексте: {chars}")
    print(f"Слов в тексте: {len(words)}")
    print(f"Промежутков между словами: {spaces}")
    print(f"Максимум символов для метода с пробелами: {spaces // 8}")
    print(f"Максимум символов для метода с невидимыми символами: {chars // 8}")
    print(f"Максимум символов для расширенного Unicode метода: {chars // 4}")


if __name__ == "__main__":
    demonstrate_steganography()
    print("\n" + "="*50 + "\n")
    
    test_text = "Пример текста для анализа его возможностей по сокрытию информации методами стеганографии."
    analyze_text_capacity(test_text)