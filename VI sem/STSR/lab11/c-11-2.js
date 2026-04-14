const WebSocket = require('ws');
const fs = require('fs');
const path = require('path');
const readline = require('readline');

const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout
});

rl.question('Введите имя файла для загрузки: ', (fileName) => {
  const ws = new WebSocket('ws://localhost:4000');

  ws.on('open', () => {
    console.log('Соединение установлено');
    ws.send(fileName);
    console.log(`Отправляем запрос на файл: ${fileName}`);

    const writeStream = fs.createWriteStream(path.join(__dirname, fileName));

    ws.on('message', (data) => {
      if (typeof data === 'string' && data === 'Файл не найден') {
        console.log('Ошибка: файл не найден на сервере');
        ws.close();
        rl.close();
      } else {
        writeStream.write(data);
        console.log(`Получен фрагмент (${data.length} байт)`);
      }
    });

    ws.on('close', () => {
      console.log('Файл успешно загружен');
      writeStream.end();
      rl.close();
    });
  });

  ws.on('error', (err) => {
    console.error('Ошибка WebSocket:', err);
    rl.close();
  });
});
