from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC


def main():
    options = webdriver.ChromeOptions()
    options.add_argument("--window-size=1440,1000")
    driver = webdriver.Chrome(options=options)
    driver.implicitly_wait(3)

    try:
        driver.get("https://demoqa.com/automation-practice-form")
        wait = WebDriverWait(driver, 10)

        element = wait.until(EC.presence_of_element_located((By.ID, "firstName")))
        print("By.ID:", element.get_attribute("id"))


        by_name = driver.find_elements(By.NAME, "gender")
        if by_name:
            print("By.NAME: найдено элементов:", len(by_name))
        else:
            print("By.NAME: атрибут name='gender' не найден; используем доступный ID/label.")

        css1 = wait.until(EC.presence_of_element_located(
            (By.CSS_SELECTOR, "form#userForm input#lastName")))
        css2 = wait.until(EC.presence_of_element_located(
            (By.CSS_SELECTOR, "form#userForm label[for='gender-radio-1']")))
        print("CSS 1:", css1.get_attribute("id"))
        print("CSS 2:", css2.text)

        xpath1 = wait.until(EC.presence_of_element_located(
            (By.XPATH, "//form[@id='userForm']//input[@id='userEmail']")
        ))
        xpath2 = wait.until(EC.presence_of_element_located(
            (By.XPATH, "//form[@id='userForm']//label[@for='gender-radio-1']")
        ))

        print("XPath 1:", xpath1.get_attribute("id"))
        print("XPath 2:", xpath2.text)

        driver.get("https://demoqa.com/elements")
        wait = WebDriverWait(driver, 15)

        link = wait.until(
            EC.presence_of_element_located((By.PARTIAL_LINK_TEXT, "Text Box"))
        )
        print("Частичный текст ссылки:", link.text)

        links = driver.find_elements(By.TAG_NAME, "a")
        print("Количество ссылок на странице:", len(links))
        print("Ссылки:", [item.text for item in links if item.text.strip()])

        assert len(links) > 0, "На странице не найдено ни одной ссылки"

    finally:
        driver.quit()


if __name__ == "__main__":
    main()