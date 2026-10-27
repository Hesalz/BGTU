from selenium.webdriver.common.by import By
from .base_page import BasePage


class TextBoxPage(BasePage):
    NAME = (By.ID, "userName")
    EMAIL = (By.ID, "userEmail")
    CURRENT_ADDRESS = (By.ID, "currentAddress")
    PERMANENT_ADDRESS = (By.ID, "permanentAddress")
    SUBMIT = (By.ID, "submit")
    OUTPUT = (By.ID, "output")

    def submit_form(self, name, email, current_address, permanent_address):
        self.open("/text-box")
        self.visible(self.NAME).send_keys(name)
        self.driver.find_element(*self.EMAIL).send_keys(email)
        self.driver.find_element(*self.CURRENT_ADDRESS).send_keys(current_address)
        self.driver.find_element(*self.PERMANENT_ADDRESS).send_keys(permanent_address)
        self.clickable(self.SUBMIT).click()
        return self.visible(self.OUTPUT).text
