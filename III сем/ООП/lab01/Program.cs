using System;
using System.Text;

class First
{
    static void Main(){ // a
        Console.WriteLine("\nВыберите задание:\n 1.a - 1\n 1.b - 2\n 1.c - 3\n 1.d - 4\n 1.e - 5\n 1.f - 6\n 2.a - 7\n 2.b - 8\n 2.c - 9\n 2.d - 10\n 3.a - 11\n 3.b - 12\n 3.c - 13\n 3.d - 14\n 4.a - 15\n 4.b - 16\n 4.c - 17\n 4.d - 18\n 5 - 19\n 6 - 20\n 0 - Выход\n");
        Console.Write("Ответ: "); int a = int.Parse(Console.ReadLine()); Console.WriteLine();
        switch (a)
        {
            case 0: Environment.Exit(0);
                break;
            case 1: One();
                break;
            case 2: Second();
                break;
            case 3: Third();
                break;
            case 4: Four();
                break;
            case 5: Five();
                break;
            case 6: Six();
                break;
            case 7: First2();
                break;
            case 8: Second2();
                break;
            case 9: Third2();
                break;
            case 10: Four2();
                break;
            case 11: First3();
                break;
            case 12: Second3();
                break;
            case 13: Third3();
                break;
            case 14: Four3();
                break;
            case 15: First4();
                break;
            case 16: First4();
                break;
            case 17: Third4();
                break;
            case 18: Four4();
                break;
            case 19: Five5();
                break;
            case 20: Six6();
                break;
            default: Console.WriteLine("ошибочка, выберите норм");
                break;
        }
    }

    static void One()
    {
        bool myBool = true;
        byte myByte = 255;
        sbyte mySByte = -128;
        char myChar = 'A';
        decimal myDecimal = 100.5m;
        double myDouble = 100.99;
        float myFloat = 50.99f;
        int myInt = 12345;
        uint myUInt = 12345u;
        long myLong = 123456789L;
        ulong myULong = 123456789UL;
        short myShort = -32768;
        ushort myUShort = 65535;

        Console.WriteLine("bool:" + myBool);
        Console.WriteLine("bool:" + myByte);
        Console.WriteLine("bool:" + mySByte);
        Console.WriteLine("char:" + myChar);
        Console.WriteLine("bool:" + myDecimal);
        Console.WriteLine("double:" + myDouble);
        Console.WriteLine("float:" + myFloat);
        Console.WriteLine("int:" + myInt);
        Console.WriteLine("uint:" + myUInt);
        Console.WriteLine("long:" + myLong);
        Console.WriteLine("ulong:" + myULong);
        Console.WriteLine("short:" + myShort);
        Console.WriteLine("ushort:" + myUShort);

        Console.WriteLine("\nВведите новое значение для переменной типа bool (true/false):");
        myBool = bool.Parse(Console.ReadLine());
        Console.WriteLine("Введите новое значение для переменной типа byte (0-255):");
        myByte = byte.Parse(Console.ReadLine());
        Console.WriteLine("Введите новое значение для переменной типа sbyte (-128 до 127):");
        mySByte = sbyte.Parse(Console.ReadLine());
        Console.WriteLine("Введите новое значение для переменной типа char (символ):");
        myChar = char.Parse(Console.ReadLine());
        Console.WriteLine("Введите новое значение для переменной типа decimal:");
        myDecimal = decimal.Parse(Console.ReadLine());
        Console.WriteLine("Введите новое значение для переменной типа double:");
        myDouble = double.Parse(Console.ReadLine());
        Console.WriteLine("Введите новое значение для переменной типа float:");
        myFloat = float.Parse(Console.ReadLine());
        Console.WriteLine("Введите новое значение для переменной типа int:");
        myInt = int.Parse(Console.ReadLine());
        Console.WriteLine("Введите новое значение для переменной типа uint:");
        myUInt = uint.Parse(Console.ReadLine());
        Console.WriteLine("Введите новое значение для переменной типа long:");
        myLong = long.Parse(Console.ReadLine());
        Console.WriteLine("Введите новое значение для переменной типа ulong:");
        myULong = ulong.Parse(Console.ReadLine());
        Console.WriteLine("Введите новое значение для переменной типа short:");
        myShort = short.Parse(Console.ReadLine());
        Console.WriteLine("Введите новое значение для переменной типа ushort:");
        myUShort = ushort.Parse(Console.ReadLine());

        Console.WriteLine("bool:" + myBool);
        Console.WriteLine("byte:" + myByte);
        Console.WriteLine("SByte:" + mySByte);
        Console.WriteLine("char:" + myChar);
        Console.WriteLine("decimal:" + myDecimal);
        Console.WriteLine("double:" + myDouble);
        Console.WriteLine("float:" + myFloat);
        Console.WriteLine("int:" + myInt);
        Console.WriteLine("uint:" + myUInt);
        Console.WriteLine("long:" + myLong);
        Console.WriteLine("ulong:" + myULong);
        Console.WriteLine("short:" + myShort);
        Console.WriteLine("ushort:" + myUShort);
        Thread.Sleep(5000);
        Main();
    }

    static void Second(){ // b

        // Неявное привидение
        int numInt = 100;
        long numLong = numInt;
        Console.WriteLine($"Неявное приведение int к long: {numLong}");
        byte byteVal = 10;
        int intVal = byteVal;
        Console.WriteLine($"Неявное приведение byte к int: {intVal}");
        float floatVal = 5.75F;
        double doubleVal = floatVal;
        Console.WriteLine($"Неявное приведение float к double: {doubleVal}");
        char charVal = 'A';
        int charToInt = charVal;
        Console.WriteLine($"Неявное приведение char к int: {charToInt}");
        short shortVal = 1234;
        int intFromShort = shortVal;
        Console.WriteLine($"Неявное приведение short к int: {intFromShort}");

        // Явное привидение
        double numDouble = 123.45;
        int doubleToInt = (int)numDouble;
        Console.WriteLine($"Явное приведение double к int: {doubleToInt}");
        long longVal = 5000000000;
        int longToInt = (int)longVal;
        Console.WriteLine($"Явное приведение long к int: {longToInt}");
        float numFloat = 9.99F;
        int floatToInt = (int)numFloat;
        Console.WriteLine($"Явное приведение float к int: {floatToInt}");
        int largeInt = 300;
        byte intToByte = (byte)largeInt;
        Console.WriteLine($"Явное приведение int к byte: {intToByte}");
        double numDouble2 = 123.456;
        float doubleToFloat = (float)numDouble2;
        Console.WriteLine($"Явное приведение double к float: {doubleToFloat}");

        // Convert
        Console.WriteLine("\nПримеры использования класса Convert:");
        int roundedInt = Convert.ToInt32(numDouble);
        Console.WriteLine($"Convert.ToInt32 (округление) double {numDouble} к int: {roundedInt}");
        string numString = "123";
        int stringToInt = Convert.ToInt32(numString);
        Console.WriteLine($"Convert.ToInt32 string \"{numString}\" к int: {stringToInt}");
        bool boolVal = true;
        int boolToInt = Convert.ToInt32(boolVal);
        Console.WriteLine($"Convert.ToInt32 bool {boolVal} к int: {boolToInt}");
        string intToString = Convert.ToString(numInt);
        Console.WriteLine($"Convert.ToString int {numInt} к string: \"{intToString}\"");
        int charToIntConvert = Convert.ToInt32(charVal);
        Console.WriteLine($"Convert.ToInt32 char '{charVal}' к int: {charToIntConvert}");
        Thread.Sleep(5000);
        Main();
    }

    static void Third() // c
    {
        int num = 1234;               
        object boxedNum = num;     
        Console.WriteLine($"Упакованное значение: {boxedNum}");

        int unboxedNum = (int)boxedNum; 
        Console.WriteLine($"Распакованное значение: {unboxedNum}");
        Thread.Sleep(5000);
        Main();
    }

    static void Four() // d
    {
        var integer = 42; 
        Console.WriteLine($"Значение: {integer}, Тип: {integer.GetType()}");
        Thread.Sleep(5000);
        Main();
    }

    static void Five() // e
    {
        int? age = null;

        if (age.HasValue)
        {
            Console.WriteLine($"Возраст: {age.Value}");
        }
        else
        {
            Console.WriteLine("Возраст не указан.");
        }

        age = 30;
        Console.WriteLine($"Обновленный возраст: {age.Value}");
        age = null;
        Console.WriteLine($"Возраст после сброса: {(age.HasValue ? age.Value.ToString() : "не указан")}");
        Thread.Sleep(5000);
        Main();
    }

    static void Six()
    {
        var myVar = 10;
        // myVar = "Hello"; 
        // Попытка присвоить переменной значение другого типа
        // Это вызовет ошибку компиляции
        // myVar = "Hello"; // Ошибка: Невозможно присвоить значение типа string переменной типа int
        Console.WriteLine(myVar);
        Thread.Sleep(5000);
        Main();
    }

    static void First2()
    {
        string str1 = "Hello";
        string str2 = "Hello";
        string str3 = "World";

        Console.WriteLine($"str1 == str2: {str1 == str2}");
        Console.WriteLine($"str1 == str3: {str1 == str3}"); 
        Thread.Sleep(5000);
        Main();
    }

    static void Second2()
    {
        string str1 = "Hello";
        string str2 = "World";
        string str3 = "My super house is elContaro";

        string concatenated = String.Concat(str1, " ", str2);
        Console.WriteLine($"Сцепление строк: {concatenated}");

        string copied = new string(str1.ToCharArray());
        Console.WriteLine($"Копированная строка: {copied}");

        string substring = str3.Substring(3, 4);
        Console.WriteLine($"Подстрока: {substring}");

        string[] words = str3.Split(' ');
        Console.WriteLine("Разделение строки на слова:");
        foreach (string word in words)
        {
            Console.WriteLine(word);
        }

        string inserted = str3.Insert(5, "VSTAVKA");
        Console.WriteLine($"Строка после вставки: {inserted}");

        string removed = str3.Remove(4, 11);
        Console.WriteLine($"Строка после удаления: {removed}");

        int number = 15;
        string worde = "Five" + number;
        Console.WriteLine(worde);
        Thread.Sleep(5000);
        Main();
    }

    static void Third2()
    {
        string emptyString = string.Empty;
        string nullString = null;

        Console.WriteLine($"Пустая строка is null or empty: {string.IsNullOrEmpty(emptyString)}");
        Console.WriteLine($"Null строка is null or empty: {string.IsNullOrEmpty(nullString)}");

        Console.WriteLine($"Длина пустой строки: {emptyString.Length}");

        if (nullString != null)
        {
            Console.WriteLine($"Длина Null строки: {nullString.Length}");
        }
        else
        {
            Console.WriteLine("Невозможно узнать длину null строки");
        }
        Thread.Sleep(5000);
        Main();
    }

    static void Four2()
    {
        StringBuilder sb = new StringBuilder("Hello, World!");
        Console.WriteLine("Начальная строка: " + sb.ToString());
        sb.Remove(5, 7);
        Console.WriteLine("После удаления: " + sb.ToString());
        sb.Insert(0, "Начало - ");
        sb.Append(" - Конец");
        Console.WriteLine("После добавления в начало и конец: " + sb.ToString());
        sb.Insert(11, "Вставка ");
        Console.WriteLine("После вставки строки: " + sb.ToString());
        sb.Replace("Вставка", "Замена");
        Console.WriteLine("После замены строки: " + sb.ToString());
        Thread.Sleep(5000);
        Main();
    }

    static void First3()
    {
        int[,] matrix = {
            {1, 2, 3},
            {4, 5, 6},
            {7, 8, 9}
        };

        Console.WriteLine("Двумерный массив:");
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                Console.Write($"{matrix[i, j],4}");
            }
            Console.WriteLine();
        }
        Thread.Sleep(5000);
        Main();
    }

    static void Second3()
    {
        string[] stringArray = { "январь", "февраль", "март", "апрель" };

        Console.WriteLine("Содержимое массива строк:");
        foreach (var item in stringArray)
        {
            Console.WriteLine(item);
        }
        Console.WriteLine($"Длина массива: {stringArray.Length}");

        Console.WriteLine("Введите индекс элемента для замены (0 - 3):");
        int index = int.Parse(Console.ReadLine());
        Console.WriteLine("Введите новое значение:");
        string newValue = Console.ReadLine();

        if (index >= 0 && index < stringArray.Length)
        {
            stringArray[index] = newValue;
            Console.WriteLine("Обновленный массив:");
            foreach (var item in stringArray)
            {
                Console.WriteLine(item);
            }
        }
        else
        {
            Console.WriteLine("Неверный индекс.");
        }
        Thread.Sleep(5000);
        Main();
    }

    static void Third3()
    {
        double[][] jaggedArray = new double[3][];
        jaggedArray[0] = new double[2];
        jaggedArray[1] = new double[3];
        jaggedArray[2] = new double[4];

        Console.WriteLine("Введите значения для ступенчатого массива:");

        for (int i = 0; i < jaggedArray.Length; i++)
        {
            Console.WriteLine($"Строка {i + 1} ({jaggedArray[i].Length} элементов):");
            for (int j = 0; j < jaggedArray[i].Length; j++)
            {
                Console.Write($"Элемент [{i}, {j}]: ");
                jaggedArray[i][j] = double.Parse(Console.ReadLine());
            }
        }

        Console.WriteLine("Ступенчатый массив:");
        for (int i = 0; i < jaggedArray.Length; i++)
        {
            for (int j = 0; j < jaggedArray[i].Length; j++)
            {
                Console.Write($"{jaggedArray[i][j],8:F2}");
            }
            Console.WriteLine();
        }
        Thread.Sleep(5000);
        Main();
    }

    static void Four3()
    {
        var array = new[] { 1, 2, 3, 4, 5 };
        var str = "Hello, world!";

        Console.WriteLine("Массив:");
        foreach (var item in array)
        {
            Console.WriteLine(item);
        }

        Console.WriteLine($"Строка: {str}");
        Thread.Sleep(5000);
        Main();
    }

    static void First4()
    {
        (int, string, char, string, ulong) myTuple = (19, "Hello", 'm', "World", 35ul);
        Console.WriteLine($"Вывод всего кортежа:{myTuple}");
        Console.WriteLine($"Вывод некоторых элементов:{myTuple.Item1}, {myTuple.Item3}, {myTuple.Item4}");
        Thread.Sleep(5000);
        Main();
    }

    static void Second4()
    {
        /*(int, string, char, string, ulong) tuple = (5, "Hello", '4', "word", 132);
        Console.WriteLine(tuple);
        Console.WriteLine(tuple.Item1);
        Console.WriteLine(tuple.Item3);
        Console.WriteLine(tuple.Item4);

        var (intValue, stringValue1, charValue, stringVAlue2, ulongValue) = tuple;
        Console.WriteLine(intValue + " " + stringValue1 + " " + charValue + " " + stringVAlue2 + " " + ulongValue);
        Thread.Sleep(5000);
        Main();
 */
    }

    static void Third4()
    {
        var tuple = (42, "Hello", 'A', "World", 123456789UL);

        var (intValue, stringValue1, charValue, stringValue2, ulongValue) = tuple;
        Console.WriteLine($"Полная распаковка: {intValue}, {stringValue1}, {charValue}, {stringValue2}, {ulongValue}");

        var (intValue2, stringValue3, _, stringValue4, _) = tuple;
        Console.WriteLine($"Частичная распаковка: {intValue2}, {stringValue3}, {stringValue4}");
        Thread.Sleep(5000);
        Main();
    }

    static void Four4()
    {
        var tuple1 = (1, 2, 3);
        var tuple2 = (1, 2, 4);
        bool isEqual = tuple1.Equals(tuple2);

        Console.WriteLine(isEqual);
        Thread.Sleep(5000);
        Main();
    }

    static void Five5()
    {
        static Tuple<int, int, int, char> LocalFunc(int[] intArr, string str)
        {
            int len = intArr.Length;
            int sum = 0;
            int min = intArr[0];
            int max = intArr[0];
            for (int i = 0; i < len; i++)
            {
                sum = sum + intArr[i];
                if (intArr[i] < min)
                {
                    min = intArr[i];
                }
                if (intArr[i] > max)
                {
                    max = intArr[i];
                }
            }
            return Tuple.Create<int, int, int, char>(max, min, sum, str[0]);
        }
        int[] intArr = new int[] { 5, 20, 8, 6 };
        string strCheck = "imagination";
        Console.WriteLine($"{LocalFunc(intArr, strCheck)}");
        Thread.Sleep(5000);
        Main();
    }

    static void Six6()
    {
        int FuncCheck()
        {
            checked
            {
                int a = int.MaxValue;
                try
                {
                    return a + 3;
                }
                catch (OverflowException)
                {
                    Console.WriteLine($"ПЕРЕПОЛНЕННО\nзначение с проверкой на переполнение:{a}");
                    return a;
                }
            }
        }
        int FuncNoCheck()
        {
            unchecked
            {
                int b = int.MaxValue;
                b += 3;
                Console.WriteLine($"значение без проверки на переполнение:{b}");
                return b;
            }
        }
        FuncNoCheck();
        FuncCheck();
        Thread.Sleep(5000);
        Main();
    }
}