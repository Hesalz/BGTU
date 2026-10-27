from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import Select
from .base_page import BasePage


class SelectMenuPage(BasePage):
    OLD_SELECT = (By.ID, "oldSelectMenu")

    def choose_color(self, color):
        self.open("/select-menu")
        dropdown = Select(self.visible(self.OLD_SELECT))
        dropdown.select_by_visible_text(color)
        return dropdown.first_selected_option.text.strip()
