# Лабораторная работа № 6. gRPC-сервис

## Структура

- `gRPCServer01` — сервер на C# / ASP.NET Core.
- `gRPCClient01` — консольный клиент на C#.
- `gRPCServer02` — сервер на Node.js.
- `gRPCClient02` — консольный клиент на Node.js.
- Во всех проектах используется один контракт `calculator.proto`.

Оба сервера используют адрес `localhost:5001`. Одновременно запускать их нельзя: для проверки межъязыкового взаимодействия запускается только один сервер за раз.

## Требования

- .NET SDK 8.0 или новее.
- Node.js 18 или новее.
- npm.

## 1. C# сервер и C# клиент

Откройте первый терминал:

```powershell
cd gRPCServer01
dotnet restore
dotnet run
```

Во втором терминале:

```powershell
cd gRPCClient01
dotnet restore
dotnet run
```

## 2. Node.js сервер и Node.js клиент

Откройте первый терминал:

```powershell
cd gRPCServer02
npm install
npm start
```

Во втором терминале:

```powershell
cd gRPCClient02
npm install
npm start
```

## 3. Node.js клиент с C# сервером

Запустите C# сервер из раздела 1. Затем выполните:

```powershell
cd gRPCClient02
npm install
npm start
```

## 4. C# клиент с Node.js сервером

Остановите C# сервер и запустите Node.js сервер из раздела 2. Затем выполните:

```powershell
cd gRPCClient01
dotnet run
```

## Проверяемые операции

Клиенты выполняют следующие запросы:

| Операция | Аргументы | Ожидаемый результат |
|---|---:|---:|
| SUM | 20, 5 | 25 |
| SUB | 20, 5 | 15 |
| MUL | 20, 5 | 100 |
| DIV | 20, 5 | 4 |
| FACT | 5 | 120 |
| DIV | 10, 0 | Ошибка INVALID_ARGUMENT |
| FACT | 13 | Ошибка OUT_OF_RANGE |

Факториал вычисляется в диапазоне `Int32`. Отрицательный аргумент факториала отклоняется с ошибкой `INVALID_ARGUMENT`.