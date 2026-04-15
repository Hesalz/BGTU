const WebSocket = require('ws');

const socket = new WebSocket('ws://localhost:3000');

socket.on('open', () => {
    console.log('Поключился к  серверу');
});

socket.on('message', (data) => {
    console.log('Уведомление:', JSON.parse(data).message);
});

socket.on('close', () => {
    console.log('Отлючился от сервера');
});