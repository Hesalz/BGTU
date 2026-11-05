```powershell
powershell -ExecutionPolicy Bypass -File .\start-all.ps1
powershell -ExecutionPolicy Bypass -File .\test.ps1
```

<details>
<summary>Подробнее</summary>

[Сборка](#1-сборка-проекта) · [Запуск](#2-запуск-проекта) · [Тестирование](#3-запуск-тестирования)

## 1. Сборка проекта

Откройте PowerShell в папке `lab05` и выполните команды:

```powershell
dotnet restore .\ApiGateway\ApiGateway.csproj
dotnet restore .\TDWA05-01\TDWA05-01.csproj

dotnet build .\ApiGateway\ApiGateway.csproj
dotnet build .\TDWA05-01\TDWA05-01.csproj
```

## 2. Запуск проекта

Запустите скрипт:

```powershell
powershell -ExecutionPolicy Bypass -File .\start-all.ps1
```

Выберите режим балансировки:

1. **Round Robin** — последовательное распределение запросов между серверами.
2. **Sticky Sessions** — закрепление клиента за одним сервером с помощью cookie.
3. **Weighted (50/30/20)** — распределение запросов в пропорции 50%, 30% и 20%.

## 3. Запуск тестирования

Откройте второе окно PowerShell в папке `lab05` и выполните:

```powershell
powershell -ExecutionPolicy Bypass -File .\test.ps1
```

</details>