const express = require('express');
const router = express.Router();

const routes = require('../routes/routes');
const controllers = require('../controllers/controllers');

routes.forEach(route => {
    router.all(route.url, (req, res) => {
        const controllerName = route.controller;
        const method = req.method;

        const controller = controllers[controllerName];

        if (!controller) {
            return res.status(404).send('Контроллер не найден');
        }

        const action = controller[method];

        if (!action) {
            return res.status(405).send('Метод не поддерживается');
        }

        action(req, res);
    });
});

module.exports = router;