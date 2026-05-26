const express = require('express');
const session = require('express-session');
const Users = require('./users.json');

const app = express();

// ============================================
// Собственный middleware для FORMS-аутентификации
// ============================================
const formsAuthMiddleware = (req, res, next) => {
    // Проверяем, есть ли активная сессия с аутентификацией
    if (req.session && req.session.isAuthenticated === true) {
        req.isAuthenticated = true;
        req.user = req.session.username;
    } else {
        req.isAuthenticated = false;
        req.user = null;
    }
    
    // Добавляем вспомогательный метод для проверки аутентификации
    req.isAuthenticatedFn = function() {
        return this.isAuthenticated === true;
    };
    
    // Логируем попытки доступа к защищенным ресурсам
    if (req.url === '/resource' && !req.isAuthenticated) {
        console.log(`[AUTH] Unauthorized access attempt to ${req.url} from ${req.ip}`);
    }
    
    next();
};
// ============================================

// Настройка сессий
app.use(session({
    resave: false,
    saveUninitialized: false,
    secret: '1111',
    cookie: { maxAge: 60000 } // сессия живет 1 минута
}));

// Middleware для обработки данных форм
app.use(express.urlencoded({ extended: true }));
app.use(express.json());

// Подключаем собственный middleware для forms-аутентификации
app.use(formsAuthMiddleware);

// Главная страница (информационная)
app.get('/', (req, res) => {
    res.send(`
        <h1>Forms Authentication Demo</h1>
        <p>Status: ${req.isAuthenticated ? 'Authenticated' : 'Not authenticated'}</p>
        ${req.isAuthenticated ? '<a href="/resource">Go to resource</a> | <a href="/logout">Logout</a>' : '<a href="/login">Login</a>'}
    `);
});

// GET /login - показывает форму входа
app.get('/login', (req, res) => {
    // Если уже аутентифицирован, перенаправляем на ресурс
    if (req.isAuthenticated) {
        return res.redirect('/resource');
    }
    
    // Показываем форму входа
    res.send(`
        <!DOCTYPE html>
        <html>
        <head>
            <title>Login</title>
            <style>
                body { font-family: Arial; margin: 50px; }
                .error { color: red; }
                form { width: 300px; }
                input { margin: 5px 0; padding: 5px; width: 100%; }
                button { margin-top: 10px; padding: 5px 15px; }
            </style>
        </head>
        <body>
            <h2>Login Form</h2>
            ${req.session.loginError ? `<p class="error">${req.session.loginError}</p>` : ''}
            <form method="POST" action="/login">
                <div>
                    <label>Username:</label>
                    <input type="text" name="username" required>
                </div>
                <div>
                    <label>Password:</label>
                    <input type="password" name="password" required>
                </div>
                <button type="submit">Login</button>
            </form>
        </body>
        </html>
    `);
});

// POST /login - обрабатывает отправку формы
app.post('/login', (req, res) => {
    const { username, password } = req.body;
    
    // Ищем пользователя в users.json
    const user = Users.find(u => 
        u.user.toUpperCase() === username.toUpperCase() && 
        u.password === password
    );
    
    if (user) {
        // Аутентификация успешна
        req.isAuthenticated = true;
        req.session.isAuthenticated = true;
        req.session.username = user.user;
        req.session.loginError = null;
        res.redirect('/resource');
    } else {
        // Ошибка аутентификации
        req.isAuthenticated = false;
        req.session.isAuthenticated = false;
        req.session.loginError = 'Invalid username or password';
        res.redirect('/login');
    }
});

// GET /logout - выход из системы
app.get('/logout', (req, res) => {
    console.log('Logout');
    req.isAuthenticated = false;
    req.session.isAuthenticated = false;
    req.session.destroy((err) => {
        if (err) console.log('Session destroy error:', err);
        res.redirect('/login');
    });
});

app.get('/resource', (req, res) => {
    if (!req.isAuthenticated) {
        return res.redirect('/login');
    }
    res.send(`
        <h1>RESOURCE</h1>
        <p>Welcome ${req.session.username || 'User'}! You have accessed the protected resource.</p>
        <a href="/">Home</a> | <a href="/logout">Logout</a>
    `);
});

app.use((req, res) => {
    console.log('Error 404 - Page not found:', req.url);
    res.status(404).send(`
        <h1>404 - Page Not Found</h1>
        <p>The requested resource "${req.url}" was not found.</p>
        <a href="/">Go to Home</a>
    `);
});

app.listen(3000, () => {
    console.log('http://localhost:3000');
});