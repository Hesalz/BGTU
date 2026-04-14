const WebSocket = require('ws');

const ws = new WebSocket('ws://localhost:4000');

ws.on('open', () => {
  console.log('Соединение с сервером установлено');
});

ws.on('message', (data) => {
  console.log(`Сообщение от сервера: ${data}`);
});

ws.on('ping', (data) => {
  console.log(`Получен ping от сервера: ${data}`);
  ws.pong('client pong');
});

ws.on('error', (err) => {
  console.error('Ошибка WebSocket:', err);
});

ws.on('close', () => {
  console.log('Соединение с сервером закрыто');
});
