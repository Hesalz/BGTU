import sqlite3
from datetime import datetime
from pathlib import Path


class HistoryDatabase:
    """SQLite storage for application operations."""

    def __init__(self, db_path: str | Path | None = None):
        if db_path is None:
            db_path = Path("data") / "history.db"
        self.db_path = Path(db_path)
        self.db_path.parent.mkdir(parents=True, exist_ok=True)
        self._create_table()

    def _connect(self):
        return sqlite3.connect(self.db_path)

    def _create_table(self):
        with self._connect() as connection:
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
        with self._connect() as connection:
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
        with self._connect() as connection:
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
            return [dict(row) for row in rows]
