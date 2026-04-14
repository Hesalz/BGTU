const WebSocket = require('ws');
const fs = require('fs');
const path = require('path');

const uploadDir = path.join(__dirname, 'upload');
if (!fs.existsSync(uploadDir)) {
  fs.mkdirSync(uploadDir);
}

const wss = new WebSocket.Server({ port: 4000 }, () => {
  console.log('Сервер запущен на ws://localhost:4000');
});

wss.on('connection', (ws) => {
  console.log('Клиент подключился');

  let fileName = '';
  let fileChunks = [];

  ws.on('message', (data, isBinary) => {
    if (!isBinary) {
      let msg = data.toString();
      if (msg.startsWith('FILENAME:')) {
        fileName = msg.replace('FILENAME:', '').trim();
        fileChunks = [];
        console.log(`Ожидаем файл: ${fileName}`);
      }
    } else {
      fileChunks.push(data);
      console.log(`Получен фрагмент (${data.length} байт)`);
    }
  });

  ws.on('close', () => {
    if (fileName && fileChunks.length > 0) {
      let fullBuffer = Buffer.concat(fileChunks);
      let savePath = path.join(uploadDir, fileName);

      fs.writeFile(savePath, fullBuffer, (err) => {
        if (err) return console.error('Ошибка записи файла:', err);
        console.log(`Файл успешно сохранён: ${savePath}`);
      });
    } else {
      console.log('Файл не был получен или имя не указано');
    }
  });
});
