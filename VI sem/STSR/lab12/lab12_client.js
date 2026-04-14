const WebSocket = require('ws');

const socket = new WebSocket('ws://localhost:3000');

socket.on('open', () => {
    console.log('поключился к  серверу');
});

socket.on('message', (data) => {
    console.log('уведомление:', JSON.parse(data).message);
});

socket.on('close', () => {
    console.log('отлючился от сервера');
});