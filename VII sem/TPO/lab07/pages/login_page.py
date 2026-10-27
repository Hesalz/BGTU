from selenium.webdriver.common.by import By
from .base_page import BasePage


class LoginPage(BasePage):
    USERNAME = (By.ID, "userName")
    PASSWORD = (By.ID, "password")
    LOGIN_BUTTON = (By.ID, "login")
    ERROR = (By.ID, "name")
    PROFILE_USERNAME = (By.ID, "userName-value")

    def open_login(self):
        self.open("/login")

    def login(self, username, password):
        self.visible(self.USERNAME).send_keys(username)
        self.driver.find_element(*self.PASSWORD).send_keys(password)
        self.clickable(self.LOGIN_BUTTON).click()

    def error_text(self):
        return self.visible(self.ERROR).text

    def profile_username(self):
        return self.visible(self.PROFILE_USERNAME).text.strip()
