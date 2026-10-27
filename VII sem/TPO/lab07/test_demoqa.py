import json
from pathlib import Path
import pytest

from pages.login_page import LoginPage
from pages.text_box_page import TextBoxPage
from pages.web_tables_page import WebTablesPage
from pages.books_page import BooksPage
from pages.profile_page import ProfilePage
from pages.select_menu_page import SelectMenuPage

USERNAME = "TestUser"
PASSWORD = "cffPBehyu64R@R9"


@pytest.mark.smoke
def test_login_with_invalid_credentials(browser):
    page = LoginPage(browser)
    page.open_login()
    page.login("invalid_test_user", "invalid_test_password")
    assert "invalid username or password" in page.error_text().lower()


@pytest.mark.parametrize("name,email,current_address,permanent_address", [
    ("Test User", "test.user@example.com", "Current address", "Permanent address"),
    ("Ivan Petrov", "ivan.petrov@example.com", "Minsk", "Brest"),
    ("Anna Smith", "anna.smith@example.com", "London", "Oxford Street"),
])
@pytest.mark.smoke
def test_text_box_form(browser, name, email, current_address, permanent_address):
    page = TextBoxPage(browser)
    result = page.submit_form(name, email, current_address, permanent_address)
    for value in (name, email, current_address, permanent_address):
        assert value in result


@pytest.mark.smoke
def test_web_tables_add_record(browser):
    page = WebTablesPage(browser)
    email = "alex.tester@example.com"
    text = page.add_record("Alex", "Tester", email, 28, 4500, "QA")
    for value in ("Alex", "Tester", email, "28", "4500", "QA"):
        assert value in text


@pytest.mark.parametrize("color", ["Purple", "Red", "Blue"])
def test_select_color(browser, color):
    assert SelectMenuPage(browser).choose_color(color) == color


@pytest.mark.cookies
def test_read_and_save_cookies(browser):
    browser.get("https://demoqa.com")
    browser.add_cookie({"name": "lab07_demo_cookie", "value": "selenium-pom"})
    cookies = browser.get_cookies()
    assert any(cookie["name"] == "lab07_demo_cookie" for cookie in cookies)
    Path("artifacts").mkdir(exist_ok=True)
    with open("artifacts/cookies.json", "w", encoding="utf-8") as file:
        json.dump(cookies, file, ensure_ascii=False, indent=2)
    print(f"\nСохранено cookies: {len(cookies)}; файл artifacts/cookies.json")


@pytest.mark.screenshot
def test_save_screenshot(browser):
    browser.get("https://demoqa.com")
    Path("screenshots").mkdir(exist_ok=True)
    assert browser.save_screenshot("screenshots/demoqa-home.png")


@pytest.mark.xfail(reason="Демонстрация ожидаемо падающего теста для управления запуском", strict=False)
def test_expected_failure_example():
    assert 1 == 2


@pytest.mark.skip(reason="Демонстрация пропуска теста по условию лабораторной")
def test_skipped_example():
    assert True


@pytest.mark.e2e
def test_end_to_end_book_collection(browser):
    login = LoginPage(browser)
    login.open_login()
    login.login(USERNAME, PASSWORD)
    assert login.profile_username() == USERNAME

    title = "Git Pocket Guide"
    profile = ProfilePage(browser)
    profile.open_profile()

    if profile.has_book(title):
        assert profile.delete_book(title), (
            f"Не удалось удалить книгу {title!r} перед тестом"
        )

    try:
        message = BooksPage(browser).add_to_collection(title)
        assert any(word in message.lower() for word in ("added", "already")), (
            f"Неожиданное сообщение при добавлении книги: {message!r}"
        )

        profile.open_profile()
        assert profile.username() == USERNAME
        row = profile.wait_for_book(title)
        assert title in row.text, (
            f"Книга {title!r} не найдена в коллекции пользователя {USERNAME!r}"
        )

        assert profile.delete_book(title), (
            f"Не удалось удалить книгу {title!r} после проверки"
        )
        assert not profile.has_book(title), (
            f"Книга {title!r} всё ещё отображается в профиле после удаления"
        )
    finally:
        try:
            if "/profile" not in browser.current_url:
                profile.open_profile()
            if profile.has_book(title):
                profile.delete_book(title)
        except Exception as cleanup_error:
            print(f"Предупреждение: не удалось очистить тестовую книгу: {cleanup_error}")
