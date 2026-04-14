const nodemailer = require("nodemailer");
const gmail = 'ваша почта';
const password = 'ваш ключ';

const transporter = nodemailer.createTransport({
    host: 'smtp.gmail.com',
    port: 587,
    secure: false,
    auth: {
        user: gmail,
        pass: password
    }
});

function send(email) {
    transporter.sendMail({
        from: 'ваша почта',
        to: gmail,
        subject: 'lab06',
        text: email
    }, (smtpErr) => {
        if (smtpErr) {
            console.error('Ошибка отправки через nodemailer:', smtpErr);
        } else {
            console.log('Сообщение успешно отправлено!');
        }
    });
    return email.toString();
}

exports.send = send;