# Лабораторная работа № 5 — Static Analysis и Logging
**Вариант 2 — Кинотеатр**

## Проверка локально

```powershell
mvn clean compile
mvn checkstyle:check
mvn test
mvn verify -DskipTests
```

## Правила Checkstyle

Файл `checkstyle.xml` содержит следующие правила:

| Правило | Назначение |
|---|---|
| `FileTabCharacter` | Запрещает символы табуляции |
| `NewlineAtEndOfFile` | Требует перевод строки в конце файла |
| `AvoidStarImport` | Запрещает импорт всех классов пакета через `*` |
| `UnusedImports` | Находит неиспользуемые импорты |
| `NeedBraces` | Требует фигурные скобки у управляющих конструкций |
| `OneStatementPerLine` | Ограничивает одной инструкцией на строку |
| `TypeName` | Проверяет имена классов и интерфейсов |
| `MethodName` | Проверяет имена методов |
| `ConstantName` | Проверяет имена констант |
| `LocalVariableName` | Проверяет имена локальных переменных |
| `MemberName` | Проверяет имена полей |
| `ParameterName` | Проверяет имена параметров |
| `LineLength` | Ограничивает длину строки 120 символами |

## Эксперимент с нарушением Checkstyle

1. Откройте любой Java-файл, например `src/main/java/cinema/Main.java`.
2. Временно добавьте в `Main.java` комментарий длиной более 120 символов, например:
   `// This comment is intentionally longer than the configured Checkstyle line limit of 120 characters.`
3. Выполните:

```powershell
git add .
git commit -m "Demonstrate Checkstyle violation"
git push
```

4. Дождитесь автоматического запуска Jenkins. Откройте Console Output. Ожидается успешный Build и ошибка `LineLength` на этапе Static Analysis. Этапы Test и Package не должны выполниться.
5. Удалите комментарий, сохраните файл и проверьте локально:

```powershell
mvn checkstyle:check
```

6. Отправьте исправление:

```powershell
git add .
git commit -m "Fix Checkstyle violation"
git push
```

7. Дождитесь автоматической сборки Jenkins. Все четыре этапа должны завершиться успешно, итог — `Finished: SUCCESS`.

## Проверка Logging

В `TicketService` сообщения формируются в точках выполнения операции:

- `INFO` — запрос на покупку получен и билет успешно сохранён.
- `WARN` — место недоступно или уже занято, покупка отклоняется.
- `ERROR` — репозиторий выбросил исключение при сохранении билета; ошибка записывается в журнал и передаётся выше.

Настройки формата и уровня журнала находятся в `src/main/resources/logback.xml`.
