## Состав проекта

- `pages/` — классы страниц и действия над ними.
- `test_demoqa.py` — автоматизированные тесты.
- `conftest.py` — настройки Chrome, опции браузера и скриншоты при падении теста.
- `pytest.ini` — метки тестов.
- `requirements.txt` — зависимости.

## Установка

В терминале из папки проекта выполните:

```powershell
python -m pip install -r requirements.txt
```

Должен быть установлен Google Chrome. Selenium Manager обычно самостоятельно подбирает ChromeDriver.

## Запуск тестов

Запуск всех тестов:

```powershell
pytest -v
```

Запуск тестов по меткам:

```powershell
pytest -v -m smoke
pytest -v -m e2e
pytest -v -m cookies
pytest -v -m screenshot
```

Запуск одного теста:

```powershell
pytest -v test_demoqa.py::test_text_box_form
```

Запуск в headless-режиме:

```powershell
$env:HEADLESS="1"
pytest -v
```

## HTML-отчёт

```powershell
pytest -v --html=reports/report.html --self-contained-html
```

Если папки `reports` нет, сначала создайте её:
Откройте `reports/report.html` в браузере.

## Что реализовано по заданию
1. **Page Object Model:** отдельные классы `LoginPage`, `TextBoxPage`, `WebTablesPage`, `BooksPage`, `ProfilePage`, `SelectMenuPage`; локаторы и действия страницы находятся внутри классов.
2. **Опции браузера:** размер окна, отключение уведомлений и блокировка всплывающих окон; headless включается переменной `HEADLESS=1`.
3. **Cookies:** создаётся cookie, читаются все cookies браузера, результат сохраняется в `artifacts/cookies.json`.
4. **Скриншот:** тест сохраняет `screenshots/demoqa-home.png`; при падении теста дополнительно сохраняется скриншот с именем теста в папке `screenshots`.
5. **Параметризация:** форма Text Box проверяется с тремя наборами данных, выбор цвета — с тремя значениями.
6. **Управление тестами:** маркеры `smoke`, `e2e`, `cookies`, `screenshot`; один тест помечен `xfail`, один — `skip`; выборочный запуск показан в командах выше.
7. **Отчёт:** `pytest-html` создаёт HTML-отчёт.