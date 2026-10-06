
from pathlib import Path

from PySide6.QtCore import Qt
from PySide6.QtGui import QPixmap, QFont
from PySide6.QtWidgets import (
    QApplication, QComboBox, QFileDialog, QFormLayout, QFrame, QGridLayout,
    QGroupBox, QHBoxLayout, QLabel, QLineEdit, QMainWindow, QMessageBox,
    QPlainTextEdit, QProgressBar, QPushButton, QSlider, QSpinBox, QTabWidget,
    QTableWidget, QTableWidgetItem, QVBoxLayout, QWidget
)

from app.analysis.benchmark import run_single_experiment
from app.analysis.metrics import compare_images
from app.core.embedding import (
    calculate_capacity,
    calculate_message_capacity,
    embed_message,
)
from app.core.extraction import extract_message
from app.database.history import HistoryDatabase


APP_TITLE = "DCT Steganography"
DEFAULT_STRENGTH = 3


class ImagePreview(QLabel):
    def __init__(self, title: str):
        super().__init__()
        self.title = title
        self.setAlignment(Qt.AlignmentFlag.AlignCenter)
        self.setMinimumSize(300, 210)
        self.setFrameShape(QFrame.Shape.StyledPanel)
        self.setText(title)

    def set_image(self, path: str):
        pixmap = QPixmap(path)
        if pixmap.isNull():
            self.setText("Не удалось загрузить изображение")
            return
        self.setPixmap(
            pixmap.scaled(
                self.size(),
                Qt.AspectRatioMode.KeepAspectRatio,
                Qt.TransformationMode.SmoothTransformation,
            )
        )

    def resizeEvent(self, event):
        super().resizeEvent(event)
        if self.pixmap() and not self.pixmap().isNull():
            self.setPixmap(
                self.pixmap().scaled(
                    self.size(),
                    Qt.AspectRatioMode.KeepAspectRatio,
                    Qt.TransformationMode.SmoothTransformation,
                )
            )


class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle(APP_TITLE)
        self.resize(1180, 780)
        self.db = HistoryDatabase()
        self._build_ui()
        self._apply_style()
        self.refresh_history()

    def _build_ui(self):
        central = QWidget()
        root_layout = QVBoxLayout(central)
        root_layout.setContentsMargins(22, 18, 22, 18)
        root_layout.setSpacing(14)

        header = QHBoxLayout()
        title = QLabel("DCT Steganography")
        title.setObjectName("title")
        subtitle = QLabel("Скрытие и извлечение защищённых сообщений в изображениях")
        subtitle.setObjectName("subtitle")
        header.addWidget(title)
        header.addStretch()
        header.addWidget(subtitle)
        root_layout.addLayout(header)

        self.tabs = QTabWidget()
        self.tabs.addTab(self._build_embed_tab(), "🔐 Скрыть сообщение")
        self.tabs.addTab(self._build_extract_tab(), "🔓 Извлечь сообщение")
        self.tabs.addTab(self._build_analysis_tab(), "📊 Анализ")
        self.tabs.addTab(self._build_history_tab(), "🕘 История")
        root_layout.addWidget(self.tabs)

        self.setCentralWidget(central)

    def _build_embed_tab(self):
        tab = QWidget()
        layout = QVBoxLayout(tab)

        files = QGroupBox("Изображение")
        form = QFormLayout(files)

        self.embed_source = QLineEdit()
        self.embed_source.setPlaceholderText("Выберите исходное изображение")
        btn = QPushButton("Выбрать…")
        btn.clicked.connect(lambda: self._choose_file(self.embed_source, save=False))
        row = QHBoxLayout()
        row.addWidget(self.embed_source)
        row.addWidget(btn)
        form.addRow("Исходник:", row)

        self.embed_output = QLineEdit()
        self.embed_output.setPlaceholderText("Куда сохранить stego-изображение")
        btn2 = QPushButton("Сохранить как…")
        btn2.clicked.connect(lambda: self._choose_file(self.embed_output, save=True))
        row2 = QHBoxLayout()
        row2.addWidget(self.embed_output)
        row2.addWidget(btn2)
        form.addRow("Результат:", row2)

        self.capacity_label = QLabel("Вместимость: —")
        self.capacity_label.setToolTip(
            "Один блок 8×8 хранит один бит. Для цветных изображений DCT выполняется по каналу яркости Y."
        )
        form.addRow("", self.capacity_label)
        self.embed_source.textChanged.connect(self._update_capacity)
        layout.addWidget(files)

        message_box = QGroupBox("Сообщение и параметры")
        msg_layout = QFormLayout(message_box)

        self.message_edit = QPlainTextEdit()
        self.message_edit.setPlaceholderText("Введите текст, который необходимо скрыть…")
        self.message_edit.setMinimumHeight(130)
        msg_layout.addRow("Сообщение:", self.message_edit)

        self.embed_password = QLineEdit()
        self.embed_password.setEchoMode(QLineEdit.EchoMode.Password)
        msg_layout.addRow("Пароль:", self.embed_password)

        strength_row = QHBoxLayout()
        self.strength_slider = QSlider(Qt.Orientation.Horizontal)
        self.strength_slider.setRange(2, 5)
        self.strength_slider.setValue(DEFAULT_STRENGTH)
        self.strength_value = QLabel(str(DEFAULT_STRENGTH))
        self.strength_slider.valueChanged.connect(
            lambda value: self.strength_value.setText(str(value))
        )
        strength_row.addWidget(self.strength_slider)
        strength_row.addWidget(self.strength_value)
        msg_layout.addRow("Сила:", strength_row)

        layout.addWidget(message_box)

        self.embed_status = QLabel("Готово к работе.")
        self.embed_status.setObjectName("status")
        layout.addWidget(self.embed_status)

        self.embed_button = QPushButton("Скрыть сообщение")
        self.embed_button.setObjectName("primary")
        self.embed_button.clicked.connect(self.embed)
        layout.addWidget(self.embed_button)

        previews = QHBoxLayout()
        self.source_preview = ImagePreview("Исходное изображение")
        self.stego_preview = ImagePreview("Stego-изображение")
        previews.addWidget(self.source_preview)
        previews.addWidget(self.stego_preview)
        layout.addLayout(previews)

        self.embed_metrics = QLabel("PSNR: —    MSE: —    Проверка: —")
        self.embed_metrics.setObjectName("metrics")
        layout.addWidget(self.embed_metrics)

        return tab

    def _build_extract_tab(self):
        tab = QWidget()
        layout = QVBoxLayout(tab)

        files = QGroupBox("Stego-изображение")
        form = QFormLayout(files)

        self.extract_source = QLineEdit()
        self.extract_source.setPlaceholderText("Выберите изображение со скрытым сообщением")
        btn = QPushButton("Выбрать…")
        btn.clicked.connect(lambda: self._choose_file(self.extract_source, save=False))
        row = QHBoxLayout()
        row.addWidget(self.extract_source)
        row.addWidget(btn)
        form.addRow("Файл:", row)
        layout.addWidget(files)

        self.extract_password = QLineEdit()
        self.extract_password.setEchoMode(QLineEdit.EchoMode.Password)
        form.addRow("Пароль:", self.extract_password)

        self.extract_button = QPushButton("Извлечь сообщение")
        self.extract_button.setObjectName("primary")
        self.extract_button.clicked.connect(self.extract)
        layout.addWidget(self.extract_button)

        self.extract_status = QLabel("Выберите stego-изображение.")
        self.extract_status.setObjectName("status")
        layout.addWidget(self.extract_status)

        result_box = QGroupBox("Результат")
        result_layout = QVBoxLayout(result_box)
        self.extracted_message = QPlainTextEdit()
        self.extracted_message.setReadOnly(True)
        self.extracted_message.setMinimumHeight(250)
        result_layout.addWidget(self.extracted_message)
        layout.addWidget(result_box)

        return tab

    def _build_analysis_tab(self):
        tab = QWidget()
        layout = QVBoxLayout(tab)

        compare_box = QGroupBox("Сравнение изображений")
        form = QFormLayout(compare_box)

        self.analysis_original = QLineEdit()
        self.analysis_stego = QLineEdit()

        for edit, label in (
            (self.analysis_original, "Исходное:"),
            (self.analysis_stego, "Stego:"),
        ):
            button = QPushButton("Выбрать…")
            button.clicked.connect(lambda checked=False, e=edit: self._choose_file(e, save=False))
            row = QHBoxLayout()
            row.addWidget(edit)
            row.addWidget(button)
            form.addRow(label, row)

        layout.addWidget(compare_box)

        compare_button = QPushButton("Рассчитать MSE / PSNR")
        compare_button.clicked.connect(self.compare)
        layout.addWidget(compare_button)

        self.analysis_result = QLabel(
            "MSE: —\nPSNR: —"
        )
        self.analysis_result.setObjectName("analysis_result")
        layout.addWidget(self.analysis_result)

        benchmark_box = QGroupBox("Быстрый benchmark")
        bench_layout = QFormLayout(benchmark_box)

        self.bench_image = QLineEdit()
        bench_button = QPushButton("Выбрать…")
        bench_button.clicked.connect(lambda: self._choose_file(self.bench_image, save=False))
        row = QHBoxLayout()
        row.addWidget(self.bench_image)
        row.addWidget(bench_button)
        bench_layout.addRow("Изображение:", row)

        self.bench_message = QLineEdit("DCT benchmark message")
        self.bench_password = QLineEdit("benchmark-password")
        self.bench_password.setEchoMode(QLineEdit.EchoMode.Password)
        bench_layout.addRow("Сообщение:", self.bench_message)
        bench_layout.addRow("Пароль:", self.bench_password)

        self.bench_button = QPushButton("Запустить benchmark 2–5")
        self.bench_button.clicked.connect(self.benchmark)
        bench_layout.addRow("", self.bench_button)

        self.bench_table = QTableWidget(0, 6)
        self.bench_table.setHorizontalHeaderLabels(
            ["Strength", "Проверка", "PSNR", "MSE", "Внедрение", "Извлечение"]
        )
        self.bench_table.horizontalHeader().setStretchLastSection(True)
        bench_layout.addRow(self.bench_table)

        layout.addWidget(benchmark_box)
        layout.addStretch()
        return tab

    def _build_history_tab(self):
        tab = QWidget()
        layout = QVBoxLayout(tab)

        self.history_table = QTableWidget(0, 7)
        self.history_table.setHorizontalHeaderLabels(
            ["Дата", "Операция", "Файл", "Сообщение, байт", "Strength", "PSNR", "Результат"]
        )
        self.history_table.horizontalHeader().setStretchLastSection(True)
        layout.addWidget(self.history_table)

        refresh = QPushButton("Обновить историю")
        refresh.clicked.connect(self.refresh_history)
        layout.addWidget(refresh)
        return tab

    def _choose_file(self, edit, save=False):
        if save:
            path, _ = QFileDialog.getSaveFileName(
                self, "Сохранить stego-изображение", "", "PNG image (*.png)"
            )
        else:
            path, _ = QFileDialog.getOpenFileName(
                self, "Выбрать изображение", "", "Images (*.png *.jpg *.jpeg *.bmp)"
            )
        if path:
            edit.setText(path)

    def _update_capacity(self):
        path = self.embed_source.text().strip()
        if not path:
            self.capacity_label.setText("Вместимость: —")
            return
        try:
            capacity = calculate_message_capacity(path)
            self.capacity_label.setText(
                "Вместимость: "
                f"payload {capacity['payload_bytes']:,} байт | "
                f"сообщение до {capacity['message_bytes']:,} байт UTF-8 | "
                f"заголовок {capacity['header_bytes']} байт"
            .replace(",", " ")
            )
            self.source_preview.set_image(path)
        except ValueError:
            self.capacity_label.setText("Вместимость: не удалось определить")

    def embed(self):
        source = self.embed_source.text().strip()
        output = self.embed_output.text().strip()
        message = self.message_edit.toPlainText()
        password = self.embed_password.text()
        strength = self.strength_slider.value()

        if not source or not Path(source).exists():
            self._error("Выберите существующее исходное изображение.")
            return
        if not output:
            self._error("Укажите путь для сохранения stego-изображения.")
            return
        if not message:
            self._error("Введите сообщение.")
            return
        if not password:
            self._error("Введите пароль.")
            return

        try:
            result = embed_message(source, output, message, password, strength)
            extracted = extract_message(output, password)
            metrics = compare_images(source, output)
            verified = extracted == message

            self.source_preview.set_image(source)
            self.stego_preview.set_image(output)

            status = "УСПЕШНО" if verified else "ОШИБКА ПРОВЕРКИ"
            self.embed_status.setText(
                f"{status}. Файл сохранён: {output}"
            )
            self.embed_metrics.setText(
                f"PSNR: {metrics['psnr_db']:.2f} dB    "
                f"MSE: {metrics['mse']:.6f}    "
                f"Проверка: {'OK' if verified else 'FAIL'}"
            )

            self.db.add(
                operation="Внедрение",
                file_path=output,
                message_bytes=result["message_bytes"],
                strength=strength,
                psnr=metrics["psnr_db"],
                mse=metrics["mse"],
                success=verified,
            )
            self.refresh_history()

            if not verified:
                self._error(
                    "Встраивание завершено, но контрольное извлечение "
                    "не подтвердило сообщение. Попробуйте strength 2–5."
                )
        except Exception as exc:
            self._error(str(exc))

    def extract(self):
        source = self.extract_source.text().strip()
        password = self.extract_password.text()

        if not source or not Path(source).exists():
            self._error("Выберите stego-изображение.")
            return
        if not password:
            self._error("Введите пароль.")
            return

        try:
            message = extract_message(source, password)
            self.extracted_message.setPlainText(message)
            self.extract_status.setText("Сообщение успешно извлечено и проверено.")
            self.db.add(
                operation="Извлечение",
                file_path=source,
                message_bytes=len(message.encode("utf-8")),
                strength=None,
                psnr=None,
                mse=None,
                success=True,
            )
            self.refresh_history()
        except Exception as exc:
            self.extracted_message.clear()
            self.extract_status.setText("Не удалось извлечь сообщение.")
            self._error(str(exc))

    def compare(self):
        original = self.analysis_original.text().strip()
        stego = self.analysis_stego.text().strip()

        try:
            metrics = compare_images(original, stego)
            self.analysis_result.setText(
                f"MSE: {metrics['mse']:.8f}\n"
                f"PSNR: {metrics['psnr_db']:.2f} dB"
            )
        except Exception as exc:
            self._error(str(exc))

    def benchmark(self):
        image = self.bench_image.text().strip()
        message = self.bench_message.text()
        password = self.bench_password.text()

        if not image or not Path(image).exists():
            self._error("Выберите изображение для benchmark.")
            return

        self.bench_table.setRowCount(0)

        import tempfile
        with tempfile.TemporaryDirectory() as work:
            for strength in range(2, 6):
                try:
                    result = run_single_experiment(
                        image, work, message, password, strength
                    )
                    row = self.bench_table.rowCount()
                    self.bench_table.insertRow(row)
                    values = [
                        str(strength),
                        "OK" if result["verified"] else "FAIL",
                        f"{result['psnr_db']:.2f} dB",
                        f"{result['mse']:.6f}",
                        f"{result['embedding_time_s']:.4f} s",
                        f"{result['extraction_time_s']:.4f} s",
                    ]
                    for col, value in enumerate(values):
                        self.bench_table.setItem(row, col, QTableWidgetItem(value))
                except Exception as exc:
                    self._error(f"Strength {strength}: {exc}")
                    break

    def refresh_history(self):
        rows = self.db.list_recent(100)
        self.history_table.setRowCount(0)
        for record in rows:
            row = self.history_table.rowCount()
            self.history_table.insertRow(row)
            values = [
                record["created_at"],
                record["operation"],
                record["file_path"],
                str(record["message_bytes"]),
                "—" if record["strength"] is None else str(record["strength"]),
                "—" if record["psnr"] is None else f"{record['psnr']:.2f}",
                "OK" if record["success"] else "FAIL",
            ]
            for col, value in enumerate(values):
                self.history_table.setItem(row, col, QTableWidgetItem(value))

    def _error(self, message: str):
        QMessageBox.critical(self, "Ошибка", message)

    def _apply_style(self):
        self.setStyleSheet("""
            QMainWindow, QWidget {
                background: #f4f6f8;
                color: #20252b;
                font-size: 14px;
            }
            QGroupBox {
                background: white;
                border: 1px solid #d9dee5;
                border-radius: 10px;
                margin-top: 10px;
                padding: 12px;
                font-weight: 600;
            }
            QGroupBox::title {
                subcontrol-origin: margin;
                left: 12px;
                padding: 0 5px;
            }
            QLineEdit, QPlainTextEdit, QTableWidget, QComboBox {
                background: white;
                border: 1px solid #cbd2da;
                border-radius: 7px;
                padding: 7px;
            }
            QPushButton {
                background: #e9edf2;
                border: 1px solid #cbd2da;
                border-radius: 7px;
                padding: 9px 14px;
            }
            QPushButton:hover {
                background: #dfe5eb;
            }
            QPushButton#primary {
                background: #2f6fed;
                color: white;
                font-weight: 600;
                padding: 11px;
            }
            QLabel#title {
                font-size: 25px;
                font-weight: 700;
            }
            QLabel#subtitle {
                color: #66717d;
            }
            QLabel#status, QLabel#metrics {
                background: white;
                border: 1px solid #d9dee5;
                border-radius: 8px;
                padding: 10px;
            }
            QLabel#analysis_result {
                background: white;
                border: 1px solid #d9dee5;
                border-radius: 8px;
                padding: 18px;
                font-size: 18px;
            }
            QTabBar::tab {
                padding: 10px 18px;
            }
            QTabBar::tab:selected {
                background: white;
                border-bottom: 2px solid #2f6fed;
            }
        """)


def run():
    app = QApplication.instance() or QApplication([])
    app.setApplicationName(APP_TITLE)
    window = MainWindow()
    window.show()
    return app.exec()
