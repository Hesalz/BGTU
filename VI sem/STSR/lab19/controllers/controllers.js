module.exports = {
    home: {
        GET: (req, res) => {
            res.send('Главная страница');
        }
    },

    user: {
        GET: (req, res) => {
            res.send('Получение пользователя ');
        },

        POST: (req, res) => {
            const name = req.body.name;

            res.send(`Пользователь создан: ${name}`);
        }
    },

    about: {
        GET: (req, res) => {
            res.send('Информация о сервере');
        }
    }
};