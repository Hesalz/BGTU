
<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Афиша кино - Контакты</title>
    <?php wp_head(); ?>
</head>
<body>
    <main>
        <h2>Связаться с нами</h2>
        <form id="contactForm">
            <label for="name">Имя:</label>
            <input type="text" id="name" name="name" required />
            <label for="email">Email:</label>
            <input type="email" id="email" name="email" required />
            <label for="message">Сообщение:</label>
            <textarea id="message" name="message" rows="4" required></textarea>
            <button type="submit">Отправить</button>
            <div class="message" id="successMessage">Спасибо за ваше сообщение!</div>
        </form>

        <section class="contact-info">
            <h3>Контакты администрации</h3>
            <p><strong>Телефон:</strong> +7 (495) 123-45-67</p>
            <p><strong>Email:</strong> admin@afisha-kino.ru</p>
            <p><strong>Адрес:</strong> г. Москва, ул. Кино, д. 10</p>
        </section>
    </main>
    <?php wp_footer(); ?>
</body>
</html>