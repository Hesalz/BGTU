from selenium.webdriver.common.by import By
from .base_page import BasePage


class WebTablesPage(BasePage):
    ADD_BUTTON = (By.ID, "addNewRecordButton")
    FIRST_NAME = (By.ID, "firstName")
    LAST_NAME = (By.ID, "lastName")
    EMAIL = (By.ID, "userEmail")
    AGE = (By.ID, "age")
    SALARY = (By.ID, "salary")
    DEPARTMENT = (By.ID, "department")
    SUBMIT = (By.ID, "submit")
    BODY = (By.TAG_NAME, "body")

    def add_record(self, first, last, email, age, salary, department):
        self.open("/webtables")
        self.clickable(self.ADD_BUTTON).click()
        self.visible(self.FIRST_NAME).send_keys(first)
        self.driver.find_element(*self.LAST_NAME).send_keys(last)
        self.driver.find_element(*self.EMAIL).send_keys(email)
        self.driver.find_element(*self.AGE).send_keys(str(age))
        self.driver.find_element(*self.SALARY).send_keys(str(salary))
        self.driver.find_element(*self.DEPARTMENT).send_keys(department)
        self.clickable(self.SUBMIT).click()
        return self.visible(self.BODY).text
