import pytest
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.common.exceptions import (
    TimeoutException,
    ElementClickInterceptedException,
    StaleElementReferenceException,
)

BASE_URL = "https://realt.by"
TWO_ROOM_SALE_URL = f"{BASE_URL}/sale/flats/2k/"


@pytest.fixture
def driver():
    """Браузер с неявным ожиданием; явные ожидания используются в тестах."""
    options = webdriver.ChromeOptions()
    options.add_argument("--window-size=1440,1000")
    options.add_argument("--disable-notifications")
    options.add_argument("--disable-popup-blocking")

    browser = webdriver.Chrome(options=options)
    browser.implicitly_wait(2)

    yield browser

    browser.quit()


def wait_visible(driver, locator, timeout=12):
    """Явное ожидание видимости элемента."""
    return WebDriverWait(
        driver,
        timeout,
        ignored_exceptions=(StaleElementReferenceException,),
    ).until(EC.visibility_of_element_located(locator))


def wait_visible_text(driver, locator, timeout=12):
    """Возвращает текст актуального видимого элемента, переживая перерисовку DOM."""
    def read_text(browser):
        for element in browser.find_elements(*locator):
            try:
                if element.is_displayed():
                    value = element.text.strip()
                    if value:
                        return value
            except StaleElementReferenceException:
                continue
        return False

    return WebDriverWait(
        driver,
        timeout,
        ignored_exceptions=(StaleElementReferenceException,),
    ).until(read_text)


def wait_clickable(driver, locator, timeout=12):
    """Явное ожидание кликабельности элемента."""
    return WebDriverWait(driver, timeout).until(
        EC.element_to_be_clickable(locator)
    )


def xpath_literal(value):
    """Безопасно помещает произвольный текст в XPath-литерал."""
    if "'" not in value:
        return "'" + value + "'"

    if '"' not in value:
        return '"' + value + '"'

    parts = value.split("'")
    return "concat(" + ", \"'\", ".join(
        "'" + part + "'" for part in parts
    ) + ")"


def visible_text_matches(driver, text):
    """Возвращает видимые элементы с точным текстом."""
    candidates = driver.find_elements(
        By.XPATH,
        "//*[self::a or self::button or self::label or "
        "self::div or self::span]"
        f"[normalize-space(.)={xpath_literal(text)}]",
    )

    visible = []

    for element in candidates:
        try:
            if element.is_displayed():
                visible.append(element)
        except StaleElementReferenceException:
            continue

    return visible


def click_visible_text(driver, text, timeout=12):
    """Нажимает на видимый элемент с указанным текстом."""

    def find_element(browser):
        for element in visible_text_matches(browser, text):
            try:
                if element.is_enabled():
                    return element
            except StaleElementReferenceException:
                continue

        return False

    element = WebDriverWait(
        driver,
        timeout,
        ignored_exceptions=(StaleElementReferenceException,),
    ).until(find_element)

    try:
        element.click()
    except ElementClickInterceptedException:
        driver.execute_script("arguments[0].click();", element)

    return element


def result_listing_links(driver):
    """На странице выдачи возвращает ссылки на объявления, а не разделы."""
    current_path = driver.current_url.rstrip("/")
    candidates = driver.find_elements(
        By.CSS_SELECTOR,
        "a[href*='/sale/flats/']",
    )

    links = []

    for element in candidates:
        try:
            href = (element.get_attribute("href") or "").rstrip("/")

            if (
                element.is_displayed()
                and href
                and href != current_path
                and "/sale/flats/2k" not in href
                and "/sale/flats/1k" not in href
                and "/sale/flats/3k" not in href
            ):
                links.append(element)

        except StaleElementReferenceException:
            continue

    return links


def wait_popup_listing_links(driver, timeout=8):
    """Ожидает появления актуальных видимых ссылок на объявления."""

    def find_links(browser):
        try:
            links = browser.find_elements(
                By.CSS_SELECTOR,
                "a[href*='/sale/flats/']",
            )

            visible_links = []

            for link in links:
                try:
                    href = link.get_attribute("href") or ""

                    if (
                        link.is_displayed()
                        and href
                        and "/sale/flats/2k/" not in href
                        and "/sale/flats/1k/" not in href
                        and "/sale/flats/3k/" not in href
                    ):
                        visible_links.append(link)

                except StaleElementReferenceException:
                    continue

            return visible_links or False

        except StaleElementReferenceException:
            return False

    return WebDriverWait(
        driver,
        timeout,
        ignored_exceptions=(StaleElementReferenceException,),
    ).until(find_links)


def click_listing_link(driver, timeout=8):
    """Находит актуальную ссылку на объявление и открывает её."""

    last_error = None

    for _ in range(2):
        links = wait_popup_listing_links(driver, timeout)
        target = links[0]

        try:
            target.click()
            return

        except StaleElementReferenceException as exc:
            last_error = exc

        except ElementClickInterceptedException:
            try:
                driver.execute_script(
                    "arguments[0].click();",
                    target,
                )
                return
            except StaleElementReferenceException as exc:
                last_error = exc

    if last_error:
        raise last_error


def test_authorization_form_opens(driver):
    """Проверяет открытие формы авторизации."""
    driver.get(BASE_URL)

    old_url = driver.current_url
    old_handles = set(driver.window_handles)

    login_link = wait_clickable(
        driver,
        (By.PARTIAL_LINK_TEXT, "Войти"),
    )

    try:
        login_link.click()
    except ElementClickInterceptedException:
        driver.execute_script("arguments[0].click();", login_link)

    def auth_interface_is_open(browser):
        if set(browser.window_handles) - old_handles:
            return True

        if (
            browser.current_url != old_url
            and any(
                token in browser.current_url.lower()
                for token in ("login", "auth", "account", "profile")
            )
        ):
            return True

        dialogs = browser.find_elements(
            By.CSS_SELECTOR,
            "[role='dialog'], dialog",
        )

        for dialog in dialogs:
            try:
                if dialog.is_displayed():
                    return True
            except StaleElementReferenceException:
                continue

        body = browser.find_element(By.TAG_NAME, "body").text.lower()

        auth_words = (
            "войти",
            "телефон",
            "электронная почта",
            "пароль",
            "email",
            "авторизация",
        )

        has_auth_text = sum(
            word in body for word in auth_words
        ) >= 2

        auth_controls = browser.find_elements(
            By.CSS_SELECTOR,
            "input[type='password'], input[type='tel'], "
            "input[autocomplete='username'], "
            "input[placeholder*='телефон' i], "
            "input[placeholder*='почт' i], "
            "button, a[role='button']",
        )

        visible_auth_control = False

        for item in auth_controls:
            try:
                if item.is_displayed():
                    visible_auth_control = True
                    break
            except StaleElementReferenceException:
                continue

        return has_auth_text and visible_auth_control

    assert WebDriverWait(driver, 15).until(
        auth_interface_is_open
    ), (
        "После нажатия «Войти» не обнаружены модальное окно, "
        "страница или элементы авторизации"
    )


def test_tc01_filter_apartments_by_two_rooms(driver):
    """TC-01: фильтрация объектов по количеству комнат."""
    driver.get(TWO_ROOM_SALE_URL)

    heading_text = wait_visible_text(
        driver,
        (By.TAG_NAME, "h1"),
        timeout=20,
    )

    assert "двухкомнатн" in heading_text.lower(), (
        f"Ожидалась выдача двухкомнатных квартир, "
        f"заголовок: {heading_text!r}"
    )

    body_text = wait_visible_text(
        driver,
        (By.TAG_NAME, "body"),
        timeout=20,
    ).lower()

    assert (
        "комнат: 2" in body_text
        or "2 комн." in body_text
    ), "В выдаче не отображается выбранный фильтр на 2 комнаты"

    assert result_listing_links(driver), (
        "Не найдены ссылки на объявления в результатах"
    )


def test_tc02_open_listing_from_map(driver):
    """TC-02: открыть карточку квартиры именно из метки объекта на карте."""
    import re
    import time

    driver.get(TWO_ROOM_SALE_URL)
    start_url = driver.current_url

    try:
        click_visible_text(driver, "На карте", timeout=10)
    except TimeoutException:
        controls = driver.find_elements(
            By.XPATH,
            "//*[self::a or self::button or @role='button']"
            "[contains(translate(normalize-space(.), 'КАРТА', 'карта'), 'карта')]",
        )
        clicked = False
        for control in controls:
            try:
                if control.is_displayed() and control.is_enabled():
                    control.click()
                    clicked = True
                    break
            except (ElementClickInterceptedException, StaleElementReferenceException):
                continue
        if not clicked:
            raise AssertionError("Не удалось переключиться в режим карты.")

    WebDriverWait(driver, 15).until(lambda d: "/map/" in d.current_url)
    map_url = driver.current_url

    marker_selectors = (
        "ymaps.ymaps-2-1-79-placemark-overlay",
        "[class*='placemark-overlay']",
    )

    def is_specific_listing_url(href):
        """Принимает реальные URL карточек Realt, например /sale-flats/object/12345/."""
        if not href:
            return False
        path = href.split("?", 1)[0].split("#", 1)[0].rstrip("/")
        if re.search(r"/sale-flats/object/\d+$", path):
            return True
        match = re.search(r"/sale/flats/([^/]+)$", path)
        if not match:
            return False
        return match.group(1).lower() not in {
            "1k", "2k", "3k", "4k", "5k", "new",
            "new-builds", "obmen-kvartir", "s-remontom", "bez-otdelki", "object",
        }

    def visible_listing_links(browser):
        """Ищет только ссылки на карточки объектов недвижимости, не на специалистов."""
        found = []
        seen = set()
        selectors = (
            "a[href*='/sale-flats/object/']",
            "a[href*='/sale/flats/']",
        )
        for selector in selectors:
            for link in browser.find_elements(By.CSS_SELECTOR, selector):
                try:
                    href = link.get_attribute("href") or ""
                    if (
                        link.is_displayed()
                        and is_specific_listing_url(href)
                        and href not in seen
                    ):
                        seen.add(href)
                        found.append(link)
                except StaleElementReferenceException:
                    continue
        return found

    def wait_for_map_markers(browser):
        return WebDriverWait(browser, 15).until(
            lambda d: any(
                element.is_displayed()
                for selector in marker_selectors
                for element in d.find_elements(By.CSS_SELECTOR, selector)
            )
        )

    wait_for_map_markers(driver)

    points = []
    seen_points = set()
    for selector in marker_selectors:
        for marker in driver.find_elements(By.CSS_SELECTOR, selector):
            try:
                if not marker.is_displayed():
                    continue
                point = driver.execute_script("""
                    const root = arguments[0];
                    const nodes = [root, ...root.querySelectorAll('*')];
                    const candidates = [];
                    for (const el of nodes) {
                        const r = el.getBoundingClientRect();
                        const s = getComputedStyle(el);
                        const area = r.width * r.height;
                        if (r.width >= 6 && r.height >= 6 && area <= 10000 &&
                            s.display !== 'none' && s.visibility !== 'hidden' &&
                            r.right > 0 && r.bottom > 0 &&
                            r.left < window.innerWidth && r.top < window.innerHeight) {
                            candidates.push({
                                x: Math.round(r.left + r.width / 2),
                                y: Math.round(r.top + r.height / 2),
                                area: area
                            });
                        }
                    }
                    candidates.sort((a, b) => a.area - b.area);
                    return candidates.length ? candidates[0] : null;
                """, marker)
                if point:
                    xy = (point["x"], point["y"])
                    if xy not in seen_points:
                        seen_points.add(xy)
                        points.append(xy)
            except StaleElementReferenceException:
                continue

    assert points, "На карте не удалось обнаружить координаты видимых меток."

    target_url = None
    tried = []
    for x, y in points[:30]:
        tried.append((x, y))
        try:
            driver.execute_cdp_cmd("Input.dispatchMouseEvent", {
                "type": "mouseMoved", "x": x, "y": y,
            })
            driver.execute_cdp_cmd("Input.dispatchMouseEvent", {
                "type": "mousePressed", "x": x, "y": y,
                "button": "left", "clickCount": 1,
            })
            driver.execute_cdp_cmd("Input.dispatchMouseEvent", {
                "type": "mouseReleased", "x": x, "y": y,
                "button": "left", "clickCount": 1,
            })

            deadline = time.time() + 1.5
            while time.time() < deadline:
                current_url = driver.current_url

                if "activeAgentOpen=true" in current_url or "activeAgentUserUuid=" in current_url:
                    break

                links = visible_listing_links(driver)
                if links:
                    target_url = links[0].get_attribute("href")
                    break
                time.sleep(0.2)

            if target_url:
                break

        except (StaleElementReferenceException, ElementClickInterceptedException):
            pass
        
        if not target_url:
            driver.get(map_url)
            wait_for_map_markers(driver)

    if not target_url:
        raise AssertionError(
            "Не удалось открыть карточку квартиры с метки объекта на карте. "
            "Метки специалистов (activeAgentOpen/activeAgentUserUuid) отбрасывались. "
            f"URL после последней попытки={driver.current_url!r}; "
            f"проверено координат={len(tried)}; "
            f"видимые ссылки={[link.get_attribute('href') for link in visible_listing_links(driver)]!r}"
        )

    assert is_specific_listing_url(target_url), (
        f"Ожидалась ссылка на карточку квартиры, получено: {target_url!r}"
    )
    driver.get(target_url)
    WebDriverWait(driver, 12).until(
        lambda d: d.current_url != start_url and is_specific_listing_url(d.current_url)
    )

    assert is_specific_listing_url(driver.current_url), (
        f"Ожидалась карточка конкретной квартиры, открыт URL: {driver.current_url!r}"
    )
    page_text = wait_visible(driver, (By.TAG_NAME, "body")).text
    assert len(page_text.strip()) > 100, "Карточка квартиры не содержит ожидаемой информации."

def test_end_to_end_search_and_open_apartment(driver):
    """Сквозной сценарий: поиск двухкомнатных квартир и открытие объявления."""
    driver.get(BASE_URL)

    preset_links = driver.find_elements(
        By.CSS_SELECTOR,
        "a[href*='/sale/flats/2k']",
    )

    visible_preset_links = []

    for link in preset_links:
        try:
            if link.is_displayed() and link.is_enabled():
                visible_preset_links.append(link)
        except StaleElementReferenceException:
            continue

    if visible_preset_links:
        link = visible_preset_links[0]

        driver.execute_script(
            "arguments[0].scrollIntoView({block: 'center'});",
            link,
        )

        try:
            link.click()
        except ElementClickInterceptedException:
            driver.execute_script(
                "arguments[0].click();",
                link,
            )

        try:
            WebDriverWait(driver, 8).until(
                lambda d: "/sale/flats/2k" in d.current_url
            )
        except TimeoutException:
            driver.get(TWO_ROOM_SALE_URL)

    else:
        driver.get(TWO_ROOM_SALE_URL)

    WebDriverWait(driver, 15).until(
        lambda d: "/sale/flats/" in d.current_url
    )

    assert "/sale/flats/2k" in driver.current_url, (
        f"Ожидалась выдача квартир с двумя комнатами, "
        f"URL: {driver.current_url}"
    )

    
    old_url = driver.current_url

    def open_first_listing(browser):
        """Повторно ищет объявление при обновлении DOM."""
        try:
            listings = result_listing_links(browser)

            if not listings:
                return False

            first_listing = listings[0]

            href = first_listing.get_attribute("href")
            if not href:
                return False

            browser.execute_script(
                "arguments[0].scrollIntoView({block: 'center'});",
                first_listing,
            )

            try:
                first_listing.click()
            except ElementClickInterceptedException:
                browser.execute_script(
                    "arguments[0].click();",
                    first_listing,
                )

            return href

        except StaleElementReferenceException:
            return False

    target_url = WebDriverWait(
        driver,
        20,
        ignored_exceptions=(StaleElementReferenceException,),
    ).until(open_first_listing)

    WebDriverWait(driver, 15).until(
        lambda d: (
            d.current_url != old_url
            and "/sale/flats/" in d.current_url
        )
    )

    assert "/sale/flats/" in driver.current_url, (
        "После выбора результата ожидалась страница объявления"
    )

    detail_text = wait_visible(
        driver,
        (By.TAG_NAME, "body"),
        timeout=15,
    ).text

    assert len(detail_text.strip()) > 100, (
        "В карточке объявления нет подробной информации"
    )



def test_room_filter_control_can_be_selected(driver):
    """Проверяет взаимодействие с фильтром количества комнат."""
    driver.get(BASE_URL)

    click_visible_text(driver, "Кол-во комнат")

    option_xpath = (
        "//*[self::label or self::button or self::li or "
        "self::div or self::span or self::a]"
        "[contains(normalize-space(.), '2') "
        "and (contains(translate(normalize-space(.), "
        "'КОМНАТ', 'комнат'), 'комнат') "
        "or normalize-space(.)='2' "
        "or contains(normalize-space(.), '2-комн') "
        "or contains(normalize-space(.), '2 комн'))]"
    )

    candidates = WebDriverWait(driver, 8).until(
        lambda d: [
            element
            for element in d.find_elements(
                By.XPATH,
                option_xpath,
            )
            if element.is_displayed() and element.is_enabled()
        ] or False
    )

    candidates.sort(
        key=lambda e: len((e.text or "").strip())
    )

    selected = False
    last_error = None

    for option in candidates:
        try:
            driver.execute_script(
                "arguments[0].scrollIntoView({block: 'center'});",
                option,
            )

            try:
                option.click()
            except ElementClickInterceptedException:
                driver.execute_script(
                    "arguments[0].click();",
                    option,
                )

            selected = True
            break

        except Exception as exc:
            last_error = exc

    if not selected:
        controls = driver.find_elements(
            By.CSS_SELECTOR,
            "input[type='checkbox'], input[type='radio'], "
            "input[role='checkbox']",
        )

        for control in controls:
            try:
                if not control.is_enabled():
                    continue

                descriptor = " ".join([
                    control.get_attribute("value") or "",
                    control.get_attribute("name") or "",
                    control.get_attribute("id") or "",
                    control.get_attribute("aria-label") or "",
                ]).lower()

                label_text = ""
                control_id = control.get_attribute("id")

                if control_id:
                    labels = driver.find_elements(
                        By.CSS_SELECTOR,
                        f"label[for='{control_id}']",
                    )

                    label_text = " ".join(
                        label.text for label in labels
                    ).lower()

                if (
                    "2" in descriptor
                    or "2" in label_text
                    or "комнат" in descriptor
                ):
                    driver.execute_script(
                        "arguments[0].click();",
                        control,
                    )
                    selected = True
                    break

            except StaleElementReferenceException as exc:
                last_error = exc

    assert selected, (
        "Не удалось выбрать вариант на 2 комнаты в фильтре Realt.by. "
        "Возможно, сайт изменил названия или структуру элементов."
    )

    selected_controls = driver.find_elements(
        By.CSS_SELECTOR,
        "input[type='checkbox']:checked, "
        "input[type='radio']:checked",
    )

    body_text = driver.find_element(
        By.TAG_NAME,
        "body",
    ).text.lower()

    state_confirmed = (
        "/2k" in driver.current_url
        or any(
            token in body_text
            for token in (
                "2 комнаты",
                "комнат: 2",
                "2 комн.",
                "2-комнат",
            )
        )
        or any(
            "2" in " ".join([
                item.get_attribute("value") or "",
                item.get_attribute("name") or "",
                item.get_attribute("id") or "",
                item.get_attribute("aria-label") or "",
            ])
            for item in selected_controls
        )
    )

    assert state_confirmed, (
        "Элемент был нажат, но выбранное состояние фильтра "
        "на две комнаты не подтверждено"
    )
