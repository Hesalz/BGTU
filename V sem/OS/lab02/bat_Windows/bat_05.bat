@echo off
REM Получаем параметры
set mode=%1
set filename=%2

REM Проверка параметров
if "%mode%"=="" (
    echo Usage: bat_05 create delete filename
    goto :eof
)

if "%filename%"=="" (
    echo Error: filename not specified
    goto :eof
)

REM Создание файла
if /i "%mode%"=="create" (
    if exist "%filename%" (
        echo File "%filename%" already exists.
    ) else (
        echo.>"%filename%"
        echo File "%filename%" created.
    )
    goto :eof
)

REM Удаление файла
if /i "%mode%"=="delete" (
    if exist "%filename%" (
        del "%filename%"
        echo File "%filename%" deleted.
    ) else (
        echo File "%filename%" does not exist.
    )
    goto :eof
)

REM Если режим неизвестен
echo Unknown mode "%mode%". Use "create" or "delete".
