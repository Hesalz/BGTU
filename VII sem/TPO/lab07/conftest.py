import os
from pathlib import Path
import pytest
from selenium import webdriver


@pytest.fixture
def browser(request):
    options = webdriver.ChromeOptions()
    options.add_argument("--window-size=1440,1000")
    options.add_argument("--disable-notifications")
    options.add_argument("--disable-popup-blocking")
    options.add_argument("--disable-infobars")
    if os.getenv("HEADLESS") == "1":
        options.add_argument("--headless=new")

    driver = webdriver.Chrome(options=options)
    driver.implicitly_wait(1)
    yield driver

    report = getattr(request.node, "rep_call", None)
    if report and report.failed:
        Path("screenshots").mkdir(exist_ok=True)
        filename = "".join(c if c.isalnum() or c in "-_" else "_" for c in request.node.nodeid)
        driver.save_screenshot(str(Path("screenshots") / f"{filename}.png"))
    driver.quit()


@pytest.hookimpl(hookwrapper=True)
def pytest_runtest_makereport(item, call):
    outcome = yield
    report = outcome.get_result()
    setattr(item, "rep_" + report.when, report)
