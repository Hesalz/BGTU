from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.ui import WebDriverWait
from selenium.common.exceptions import TimeoutException
from .base_page import BasePage


class ProfilePage(BasePage):
    USERNAME = (By.ID, "userName-value")
    DELETE_CONFIRM = (By.ID, "closeSmallModal-ok")

    def open_profile(self):
        self.open("/profile")

    def username(self):
        return self.visible(self.USERNAME).text.strip()

    @staticmethod
    def _find_book_row(driver, title):
        title_locator = (By.XPATH, f"//*[normalize-space(text())='{title}']")
        for element in driver.find_elements(*title_locator):
            ancestors = element.find_elements(
                By.XPATH, "./ancestor::div[.//*[@title='Delete']][1]"
            )
            if ancestors:
                return ancestors[0]
        return False

    def has_book(self, title):
        return bool(self._find_book_row(self.driver, title))

    def wait_for_book(self, title, timeout=30):
        return WebDriverWait(self.driver, timeout, poll_frequency=0.5).until(
            lambda driver: self._find_book_row(driver, title)
        )

    def delete_book(self, title):
        row = self._find_book_row(self.driver, title)
        if not row:
            return False

        delete_icon = row.find_element(By.XPATH, ".//*[@title='Delete']")
        self.driver.execute_script(
            "arguments[0].scrollIntoView({block: 'center'});", delete_icon
        )
        delete_icon.click()

        self.clickable(self.DELETE_CONFIRM).click()
        alert = WebDriverWait(self.driver, 10).until(EC.alert_is_present())
        alert.accept()

        try:
            WebDriverWait(self.driver, 10, poll_frequency=0.3).until(
                lambda driver: not self._find_book_row(driver, title)
            )
        except TimeoutException:
            return False
        return True
