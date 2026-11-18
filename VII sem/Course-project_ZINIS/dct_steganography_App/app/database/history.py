import sqlite3
from datetime import datetime
from pathlib import Path


class HistoryDatabase:

    def __init__(self, db_path: str | Path | None = None):
        if db_path is None: # Использование стандартного пути к базе данных, если путь не задан.
            db_path = Path("data") / "history.db"

        self.db_path = Path(db_path)  # Сохранение пути к базе данных и создание каталога при необходимости.
        self.db_path.parent.mkdir(parents=True, exist_ok=True)
        self._create_table() # Создание таблицы для хранения истории операций.

    def _connect(self):
        return sqlite3.connect(self.db_path) # Открытие соединения с базой данных SQLite.

    def _create_table(self):
        with self._connect() as connection: # Создание таблицы operations, если она ещё не существует.
            connection.execute(
                """
                CREATE TABLE IF NOT EXISTS operations (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    created_at TEXT NOT NULL,
                    operation TEXT NOT NULL,
                    file_path TEXT NOT NULL,
                    message_bytes INTEGER,
                    strength REAL,
                    psnr REAL,
                    mse REAL,
                    success INTEGER NOT NULL
                )
                """
            )
            connection.commit()

    def add(
        self,
        operation,
        file_path,
        message_bytes,
        strength,
        psnr,
        mse,
        success,
    ):
        with self._connect() as connection: # Добавление записи об операции с датой, параметрами и результатом выполнения.
            connection.execute(
                """
                INSERT INTO operations (
                    created_at, operation, file_path,
                    message_bytes, strength, psnr, mse, success
                )
                VALUES (?, ?, ?, ?, ?, ?, ?, ?)
                """,
                (
                    datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
                    operation,
                    str(file_path),
                    message_bytes,
                    strength,
                    psnr,
                    mse,
                    int(bool(success)),
                ),
            )
            connection.commit()

    def list_recent(self, limit=100): 
        with self._connect() as connection: # Получение последних записей истории в порядке от новых к старым.
            connection.row_factory = sqlite3.Row
            rows = connection.execute(
                """
                SELECT created_at, operation, file_path,
                       message_bytes, strength, psnr, mse, success
                FROM operations
                ORDER BY id DESC
                LIMIT ?
                """,
                (limit,),
            ).fetchall()
            return [dict(row) for row in rows]  # Преобразование результатов запроса в список словарей.
