## Установка
1. Скопировать `TDWA03-01`, `TDWA02-02` в `\nginx-1.30.4\\html\\`. В результате должны быть `html\\TDWA03-01\\` и `html\\TDWA02-02\\`.
2. Cкопировать новый `nginx/conf/nginx.conf`.
3. Создать `\\nginx-1.30.4\\nginx-1.30.4\\conf\\certs\\` и в PowerShell выполните (если OpenSSL установлен):

```powershell
openssl req -x509 -nodes -days 365 -newkey rsa:2048 `
  -keyout "D:\\nginx-1.30.4\\nginx-1.30.4\\conf\\certs\\localhost.key" `
  -out "D:\\nginx-1.30.4\\nginx-1.30.4\\conf\\certs\\localhost.crt" `
  -subj "/CN=localhost" `
  -addext "subjectAltName=DNS:localhost,IP:127.0.0.1"
```

4. Выполнить npm install в терминале
5. Запустить сервер node server.js
6. Проверить NGINX:

```powershell
cd \\nginx-1.30.4\\nginx-1.30.4
.\\nginx.exe -t
```

P.S Для локального самоподписанного сертификата браузер может показать предупреждение.