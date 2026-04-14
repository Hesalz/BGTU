const WebSocket = require('ws');
const fs = require('fs');
const path = require('path');

const downloadDir = path.join(__dirname, 'download');

const wss = new WebSocket.Server({ port: 4000 }, () => {
  console.log('Сервер запущен на ws://localhost:4000');
});

wss.on('connection', (ws) => {
  console.log('Клиент подключился');

  ws.on('message', (data) => {
    let fileName = data.toString().trim();
    let filePath = path.join(downloadDir, fileName);

    if (fs.existsSync(filePath)) {
      let readStream = fs.createReadStream(filePath);
      let duplex = WebSocket.createWebSocketStream(ws, { encoding: 'utf8' });

      readStream.pipe(duplex);
      console.log(`Отправляем файл: ${fileName}`);
    } else {
      ws.send('Файл не найден');
      console.log(`Файл ${fileName} не найден`);
    }
  });
});
