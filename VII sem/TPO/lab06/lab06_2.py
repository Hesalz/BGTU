from __future__ import annotations

import os
import pytest

from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.ui import Select, WebDriverWait
from selenium.common.exceptions import TimeoutException


BASE_URL = "https://demoqa.com"
WAIT_SECONDS = 10
USERNAME = "TestUser"
PASSWORD = "cffPBehyu64R@R9"


@pytest.fixture
def browser():
    options = webdriver.ChromeOptions()
    options.add_argument("--start-maximized")
    if os.getenv("HEADLESS") == "1":
        options.add_argument("--headless=new")
        options.add_argument("--window-size=1440,1000")

    driver = webdriver.Chrome(options=options)
    driver.implicitly_wait(2)
    yield driver
    driver.quit()


def wait_for(browser, timeout=WAIT_SECONDS):
    return WebDriverWait(browser, timeout)


def open_page(browser, path):
    browser.get(f"{BASE_URL}{path}")
    wait_for(browser).until(
        lambda d: d.execute_script("return document.readyState") == "complete"
    )


def test_login_to_demoqa_profile(browser):
    open_page(browser, "/login")

    wait_for(browser).until(
        EC.visibility_of_element_located((By.ID, "userName"))
    ).send_keys("invalid_test_user")
    browser.find_element(By.ID, "password").send_keys("invalid_test_password")
    wait_for(browser).until(
        EC.element_to_be_clickable((By.ID, "login"))
    ).click()

    error = wait_for(browser).until(
        EC.visibility_of_element_located((By.ID, "name"))
    )
    assert "invalid username or password" in error.text.lower(), (
        "При вводе неверных данных ожидалось сообщение об ошибке авторизации"
    )


def test_text_box_form_submission(browser):
    open_page(browser, "/text-box")

    wait_for(browser).until(
        EC.visibility_of_element_located((By.ID, "userName"))
    ).send_keys("Test User")
    browser.find_element(By.ID, "userEmail").send_keys("test.user@example.com")
    browser.find_element(By.ID, "currentAddress").send_keys("Current address")
    browser.find_element(By.ID, "permanentAddress").send_keys("Permanent address")

    wait_for(browser).until(
        EC.element_to_be_clickable((By.ID, "submit"))
    ).click()

    output = wait_for(browser).until(
        EC.visibility_of_element_located((By.ID, "output"))
    )
    output_text = output.text
    assert "Test User" in output_text, "Имя не появилось в результате отправки формы"
    assert "test.user@example.com" in output_text, "Email не появился в результате"
    assert "Current address" in output_text, "Текущий адрес не появился в результате"
    assert "Permanent address" in output_text, "Постоянный адрес не появился в результате"


def test_web_tables_add_record(browser):
    open_page(browser, "/webtables")

    wait_for(browser).until(
        EC.element_to_be_clickable((By.ID, "addNewRecordButton"))
    ).click()

    wait_for(browser).until(
        EC.visibility_of_element_located((By.ID, "firstName"))
    ).send_keys("Alex")
    browser.find_element(By.ID, "lastName").send_keys("Tester")
    browser.find_element(By.ID, "userEmail").send_keys("alex.tester@example.com")
    browser.find_element(By.ID, "age").send_keys("28")
    browser.find_element(By.ID, "salary").send_keys("4500")
    browser.find_element(By.ID, "department").send_keys("QA")

    wait_for(browser).until(
        EC.element_to_be_clickable((By.ID, "submit"))
    ).click()

    email = "alex.tester@example.com"
    wait_for(browser).until(
        lambda driver: email in driver.find_element(By.TAG_NAME, "body").text
    )
    page_text = browser.find_element(By.TAG_NAME, "body").text
    for value in ("Alex", "Tester", email, "28", "4500", "QA"):
        assert value in page_text, f"Добавленная запись не содержит значение {value!r}"



def test_end_to_end_book_collection(browser):
    wait = wait_for(browser)

    open_page(browser, "/login")
    wait.until(EC.visibility_of_element_located((By.ID, "userName"))).send_keys(USERNAME)
    browser.find_element(By.ID, "password").send_keys(PASSWORD)
    wait.until(EC.element_to_be_clickable((By.ID, "login"))).click()

    wait.until(EC.url_contains("/profile"))
    profile_username = wait.until(
        EC.visibility_of_element_located((By.ID, "userName-value"))
    )
    assert profile_username.text.strip() == USERNAME, (
        "После входа в профиле должен отображаться пользователь TestUser"
    )

    book_title = "Git Pocket Guide"
    open_page(browser, "/books")

    search_box = wait.until(
        EC.visibility_of_element_located((By.ID, "searchBox"))
    )
    search_box.clear()
    search_box.send_keys(book_title)

    book_link = wait.until(
        EC.element_to_be_clickable(
            (By.XPATH, f"//a[normalize-space()='{book_title}']")
        )
    )
    book_link.click()

    add_button = wait.until(
        EC.element_to_be_clickable(
            (By.XPATH, "//button[normalize-space()='Add To Your Collection']")
        )
    )
    browser.execute_script("arguments[0].scrollIntoView({block: 'center'});", add_button)
    add_button.click()

    try:
        alert = WebDriverWait(browser, 8, poll_frequency=0.2).until(
            EC.alert_is_present()
        )
    except TimeoutException as exc:
        page_text = browser.find_element(By.TAG_NAME, "body").text
        raise AssertionError(
            "После нажатия 'Add To Your Collection' DemoQA не показал подтверждение. "
            f"URL: {browser.current_url}. Текст страницы: {page_text[:1200]!r}"
        ) from exc

    alert_text = alert.text
    alert.accept()
    assert any(word in alert_text.lower() for word in ("added", "already")), (
        f"Неожиданное сообщение при добавлении книги {book_title!r}: {alert_text!r}"
    )

    open_page(browser, "/profile")

    wait.until(EC.visibility_of_element_located((By.ID, "userName-value")))
    profile_wait = WebDriverWait(browser, 30, poll_frequency=0.5)

    title_locator = (By.XPATH, f"//*[normalize-space(text())='{book_title}']")

    def find_book_row(driver):
        title_elements = driver.find_elements(*title_locator)
        for title_element in title_elements:
            ancestors = title_element.find_elements(
                By.XPATH,
                "./ancestor::div[.//*[@title='Delete']][1]"
            )
            if ancestors:
                return ancestors[0]
        return False

    try:
        row = profile_wait.until(lambda driver: find_book_row(driver))
    except TimeoutException as exc:
        visible_text = browser.find_element(By.TAG_NAME, "body").text
        raise AssertionError(
            f"После добавления книга {book_title!r} не появилась в профиле. "
            f"Текущий URL: {browser.current_url}. Текст страницы: {visible_text[:1500]!r}"
        ) from exc

    assert book_title in row.text, (
        f"Книга {book_title!r} не найдена в строке коллекции пользователя {USERNAME!r}"
    )

    delete_icon = row.find_element(By.XPATH, ".//*[@title='Delete']")
    browser.execute_script("arguments[0].scrollIntoView({block: 'center'});", delete_icon)
    delete_icon.click()

    confirm_button = wait.until(
        EC.element_to_be_clickable((By.ID, "closeSmallModal-ok"))
    )
    confirm_button.click()

    deletion_alert = wait.until(EC.alert_is_present())
    deletion_alert.accept()

    wait.until(lambda driver: not find_book_row(driver))
    assert not find_book_row(browser), (
        f"Книга {book_title!r} всё ещё отображается в профиле после удаления"
    )


def test_select_dropdown_and_checkbox(browser):
    open_page(browser, "/select-menu")

    select_element = wait_for(browser).until(
        EC.visibility_of_element_located((By.ID, "oldSelectMenu"))
    )
    dropdown = Select(select_element)
    dropdown.select_by_visible_text("Purple")
    assert dropdown.first_selected_option.text.strip() == "Purple", (
        "В выпадающем списке должно быть выбрано значение Purple"
    )

    open_page(browser, "/automation-practice-form")
    sports_checkbox = wait_for(browser).until(
        EC.presence_of_element_located((By.ID, "hobbies-checkbox-1"))
    )
    browser.execute_script(
        "arguments[0].scrollIntoView({block: 'center'});", sports_checkbox
    )

    if not sports_checkbox.is_selected():
        sports_label = wait_for(browser).until(
            EC.element_to_be_clickable((By.CSS_SELECTOR, "label[for='hobbies-checkbox-1']"))
        )
        sports_label.click()

    wait_for(browser).until(lambda driver: sports_checkbox.is_selected())
    assert sports_checkbox.is_selected(), (
        "После выбора Sports checkbox должен быть отмечен"
    )