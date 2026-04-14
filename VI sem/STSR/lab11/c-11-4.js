const WebSocket = require('ws');
const readline = require('readline');

const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout
});

rl.question('Введите ваше имя: ', (clientName) => {
  const ws = new WebSocket('ws://localhost:4000');

  ws.on('open', () => {
    console.log('Соединение с сервером установлено');

    const message = {
      client: clientName,
      timestamp: new Date().toISOString(),
    };

    ws.send(JSON.stringify(message));
    console.log('Сообщение отправлено:', message);

    ws.on('message', (data) => {
      const response = JSON.parse(data);
      console.log(`Ответ от сервера: ${JSON.stringify(response)}`);
      ws.close(); 
      rl.close();
    });
  });

  ws.on('error', (err) => {
    console.error('Ошибка WebSocket:', err);
    rl.close();
  });
});
