const express = require('express');
const cors = require('cors');
const { google } = require('googleapis');
const path = require('path');
const fs = require('fs');
const open = require('open');

const CLIENT_ID = ''; 
const CLIENT_SECRET = '';
const REDIRECT_URI = 'http://localhost:5000/oauth2callback';
const oauth2Client = new google.auth.OAuth2(CLIENT_ID, CLIENT_SECRET, REDIRECT_URI);
const app = express();

app.use(cors()); 
app.use(express.json());

const authUrl = oauth2Client.generateAuthUrl({
  access_type: 'offline',
  scope: ['https://www.googleapis.com/auth/gmail.send']
});

app.get('/', (req, res) => {
  res.send(`<h1>Авторизуйтесь через Google</h1><a href="${authUrl}">Перейти к авторизации</a>`);
});

app.get('/oauth2callback', async (req, res) => {
  const code = req.query.code;

  if (!code) {
    return res.status(400).send('Ошибка авторизации.');
  }

  try {
    const { tokens } = await oauth2Client.getToken(code);
    oauth2Client.setCredentials(tokens);

    const TOKEN_PATH = path.join(__dirname, 'token.json');
    fs.writeFileSync(TOKEN_PATH, JSON.stringify(tokens));

    res.send('Авторизация прошла успешно! Теперь вы можете использовать ваше приложение.');
    console.log('Токены сохранены в token.json');
  } catch (error) {
    res.status(500).send('Ошибка получения токенов.');
    console.error('Ошибка при получении токенов:', error);
  }
});

app.post('/send-email', async (req, res) => {
  const { name, phone, date, time, message, recipientEmail } = req.body;

  if (!name || !phone || !date || !time || !recipientEmail) {
    return res.status(400).send('Отсутствуют обязательные данные');
  }

  try {
    const tokens = JSON.parse(fs.readFileSync('token.json'));
    oauth2Client.setCredentials(tokens);

    const gmail = google.gmail({ version: 'v1', auth: oauth2Client });
    const BGTUEmail = 'core.such.react@gmail.com';
    const emailLines = [
      'Content-Type: text/plain; charset="UTF-8"',
      'MIME-Version: 1.0',
      `From: "awdefin@gmail.com"`,
      `To: ${recipientEmail},${BGTUEmail}`,
      `Subject: =?UTF-8?B?${Buffer.from('Новая заявка на прием в CUBE').toString('base64')}?=`,
      '',
      `Имя: ${name}`,
      `Телефон: ${phone}`,
      `Дата записи: ${date}`,
      `Время записи: ${time}`,
      `Сообщение: ${message}`,
      
      'Курсовой проект - Бабашинский Г.А.',
    ];

    const email = emailLines.join('\n');
    const encodedEmail = Buffer.from(email).toString('base64').replace(/\+/g, '-').replace(/\//g, '_');

    await gmail.users.messages.send({
      userId: 'me',
      requestBody: {
        raw: encodedEmail,
      },
    });

    res.send('Письмо отправлено');
  } catch (error) {
    console.error('Ошибка при отправке письма:', error);
    res.status(500).send('Ошибка при отправке письма');
  }
});

app.listen(5000, () => {
  console.log('Сервер работает на http://localhost:5000');
  open(authUrl);
});
