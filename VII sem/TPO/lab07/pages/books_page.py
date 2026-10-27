from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.ui import WebDriverWait
from selenium.common.exceptions import TimeoutException
from .base_page import BasePage


class BooksPage(BasePage):
    SEARCH = (By.ID, "searchBox")
    ADD_BUTTON = (By.XPATH, "//button[normalize-space()='Add To Your Collection']")

    def search(self, title):
        self.open("/books")
        field = self.visible(self.SEARCH)
        field.clear()
        field.send_keys(title)
        return self.clickable((By.XPATH, f"//a[normalize-space()='{title}']"))

    def add_to_collection(self, title):
        self.search(title).click()
        button = self.clickable(self.ADD_BUTTON)
        self.driver.execute_script(
            "arguments[0].scrollIntoView({block: 'center'});", button
        )
        button.click()

        try:
            alert = WebDriverWait(self.driver, 8, poll_frequency=0.2).until(
                EC.alert_is_present()
            )
        except TimeoutException as exc:
            page_text = self.driver.find_element(By.TAG_NAME, "body").text
            raise AssertionError(
                "После нажатия 'Add To Your Collection' DemoQA не показал "
                f"подтверждение. URL: {self.driver.current_url}. "
                f"Текст страницы: {page_text[:1200]!r}"
            ) from exc

        message = alert.text
        alert.accept()
        return message
