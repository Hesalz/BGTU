dotnet restore .\ApiGateway\ApiGateway.csproj
powershell -ExecutionPolicy Bypass -File .\start-all.ps1
powershell -ExecutionPolicy Bypass -File .\test.ps1


# Подробнее
````
## Структура

- `TDWA04-01/` — сервер, принимающий параметры `--Nick` и `--Port`;
- `ApiGateway/` — API Gateway на Ocelot;
- `start-all.ps1` — запуск четырёх процессов в Windows PowerShell;
- `test.ps1` — проверка всех маршрутов.

## Запуск в Windows

Откройте PowerShell в каталоге проекта и выполните:

```powershell С ПЕРВОГО РАЗА МОЖЕТ ЗАВЕРШИТЬСЯ ОШИБКАМИ!
dotnet restore .\ApiGateway\ApiGateway.csproj
```

Затем запустите все четыре процесса:

```powershell
powershell -ExecutionPolicy Bypass -File .\start-all.ps1
```

## ИЛИ Ручной запуск

В отдельных терминалах из корня проекта:

```powershell
dotnet run --project .\TDWA04-01\TDWA04-01.csproj -- --Nick X --Port 5001
dotnet run --project .\TDWA04-01\TDWA04-01.csproj -- --Nick Y --Port 5002
dotnet run --project .\TDWA04-01\TDWA04-01.csproj -- --Nick Z --Port 5003
dotnet run --project .\ApiGateway\ApiGateway.csproj
```

## Маршрутизация

| Запрос к Gateway | Экземпляр TDWA04-01 | Downstream |
|---|---|---|
| `GET /api` | Nick `X`, порт 5001 | `GET http://localhost:5001/A` |
| `POST /api` | Nick `Y`, порт 5002 | `POST http://localhost:5002/A` |
| `PUT /api` | Nick `Z`, порт 5003 | `PUT http://localhost:5003/A` |
| `DELETE /api` | Nick `Z`, порт 5003 | `DELETE http://localhost:5003/A` |

## Проверка CURL

После запуска всех процессов откройте отдельный терминал и выполните:

```powershell
powershell -ExecutionPolicy Bypass -File .\test.ps1
```

ИЛИ вручную:

```powershell
curl.exe -i http://localhost:5000/api
curl.exe -i -X POST http://localhost:5000/api
curl.exe -i -X PUT http://localhost:5000/api
curl.exe -i -X DELETE http://localhost:5000/api
```

## Проверка самих серверов

Можно обратиться к корневому адресу каждого экземпляра:

```powershell
curl.exe http://localhost:5001/
curl.exe http://localhost:5002/
curl.exe http://localhost:5003/
```

````