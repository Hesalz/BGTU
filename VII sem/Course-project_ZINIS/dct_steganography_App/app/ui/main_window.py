
from pathlib import Path

from PySide6.QtCore import Qt, QTimer
from PySide6.QtGui import QPixmap, QFont
from PySide6.QtWidgets import (
    QApplication, QComboBox, QFileDialog, QFormLayout, QFrame, QGridLayout,
    QGroupBox, QHBoxLayout, QLabel, QLineEdit, QMainWindow, QMessageBox,
    QPlainTextEdit, QProgressBar, QPushButton, QSlider, QSpinBox, QTabWidget,
    QScrollArea, QTableWidget, QTableWidgetItem, QVBoxLayout, QWidget
)

from matplotlib.backends.backend_qtagg import FigureCanvasQTAgg as FigureCanvas
from matplotlib.figure import Figure

from app.analysis.benchmark import (
    run_benchmark_series,
)
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


class PlotCanvas(FigureCanvas):
    """Matplotlib canvas that lets the parent tab handle mouse-wheel scrolling."""

    def wheelEvent(self, event):
        parent = self.parentWidget()
        while parent is not None and not isinstance(parent, QScrollArea):
            parent = parent.parentWidget()

        if parent is not None:
            delta = event.angleDelta().y()
            if delta:
                bar = parent.verticalScrollBar()
                bar.setValue(bar.value() - delta)
                event.accept()
                return

        event.ignore()


class ImagePreview(QLabel):
    def __init__(self, title: str):
        super().__init__()
        self.title = title
        self._source_pixmap = QPixmap()
        self.setAlignment(Qt.AlignmentFlag.AlignCenter)
        self.setMinimumHeight(210)
        self.setFrameShape(QFrame.Shape.StyledPanel)
        self.setText(title)

    def set_image(self, path: str):
        pixmap = QPixmap(path)
        if pixmap.isNull():
            self._source_pixmap = QPixmap()
            self.clear()
            self.setText("Не удалось загрузить изображение")
            return

        self._source_pixmap = pixmap
        self._update_pixmap()

    def _update_pixmap(self):
        if self._source_pixmap.isNull():
            return

        available = self.size()
        if available.width() <= 0 or available.height() <= 0:
            return

        scaled = self._source_pixmap.scaled(
            available,
            Qt.AspectRatioMode.KeepAspectRatio,
            Qt.TransformationMode.SmoothTransformation,
        )
        self.setPixmap(scaled)

    def resizeEvent(self, event):
        super().resizeEvent(event)
        self._update_pixmap()


class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle(APP_TITLE)

        screen = QApplication.primaryScreen()
        if screen is not None:
            available = screen.availableGeometry()
            width = min(1180, max(900, available.width() - 40))
            height = min(760, max(620, available.height() - 40))
            self.resize(width, height)
        else:
            self.resize(1180, 760)

        self.db = HistoryDatabase()
        self._benchmark_results = []
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
        self.tabs.addTab(self._wrap_tab(self._build_embed_tab()), "🔐 Скрыть сообщение")
        self.tabs.addTab(self._wrap_tab(self._build_extract_tab()), "🔓 Извлечь сообщение")
        self.tabs.addTab(self._wrap_tab(self._build_analysis_tab()), "📊 Анализ")
        self.tabs.addTab(self._wrap_tab(self._build_history_tab()), "🕘 История")
        root_layout.addWidget(self.tabs)

        self.setCentralWidget(central)

    @staticmethod
    def _wrap_tab(content):
        """Помещает содержимое вкладки в прокручиваемую область."""
        scroll = QScrollArea()
        scroll.setWidgetResizable(True)
        scroll.setFrameShape(QFrame.Shape.NoFrame)
        scroll.setHorizontalScrollBarPolicy(Qt.ScrollBarPolicy.ScrollBarAsNeeded)
        scroll.setVerticalScrollBarPolicy(Qt.ScrollBarPolicy.ScrollBarAsNeeded)
        content.setMinimumSize(0, 0)
        scroll.setWidget(content)
        return scroll

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
        self.embed_output.setPlaceholderText("Куда сохранить стего-изображение")
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
        self.embed_password.setPlaceholderText("Без пароля сообщение не шифруется")
        msg_layout.addRow("Пароль (необязательно):", self.embed_password)

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
        self.stego_preview = ImagePreview("Стего-изображение")
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

        files = QGroupBox("Стего-изображение")
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
        self.extract_password.setPlaceholderText("Оставьте пустым, если сообщение без пароля")
        form.addRow("Пароль (если использовался):", self.extract_password)

        self.extract_button = QPushButton("Извлечь сообщение")
        self.extract_button.setObjectName("primary")
        self.extract_button.clicked.connect(self.extract)
        layout.addWidget(self.extract_button)

        self.extract_status = QLabel("Выберите стего-изображение.")
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
            (self.analysis_stego, "Стего:"),
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

        benchmark_box = QGroupBox("Серия экспериментов")
        bench_layout = QFormLayout(benchmark_box)

        self.bench_image = QLineEdit()
        bench_button = QPushButton("Выбрать…")
        bench_button.clicked.connect(lambda: self._choose_file(self.bench_image, save=False))
        row = QHBoxLayout()
        row.addWidget(self.bench_image)
        row.addWidget(bench_button)
        bench_layout.addRow("Изображение:", row)

        self.bench_password = QLineEdit("пароль-для-экспериментов")
        self.bench_password.setEchoMode(QLineEdit.EchoMode.Password)
        bench_layout.addRow("Пароль:", self.bench_password)

        self.bench_series_sizes = QLineEdit("10, 50, 100, 150, 200, 250")
        self.bench_series_sizes.setToolTip(
            "Размеры сообщений в байтах. Каждый размер проверяется при силе 1–5."
        )
        bench_layout.addRow("Размеры серии:", self.bench_series_sizes)

        self.bench_series_button = QPushButton("Запустить серию экспериментов")
        self.bench_series_button.clicked.connect(self.benchmark_series)
        bench_layout.addRow("", self.bench_series_button)

        self.bench_series_status = QLabel(
            "Каждый размер сообщения будет проверен при силе 1–5."
        )
        self.bench_series_status.setObjectName("status")
        bench_layout.addRow("", self.bench_series_status)

        self.bench_table = QTableWidget(0, 8)
        self.bench_table.setHorizontalHeaderLabels(
            [
                "Размер, Б", "Сила", "Результат", "PSNR", "MSE",
                "Внедрение", "Извлечение", "Данные, Б"
            ]
        )
        self.bench_table.horizontalHeader().setStretchLastSection(True)
        self.bench_table.setMinimumHeight(260)
        self.bench_table.setMaximumHeight(360)
        self.bench_table.setVerticalScrollBarPolicy(Qt.ScrollBarPolicy.ScrollBarAsNeeded)
        self.bench_table.setAlternatingRowColors(True)
        self.bench_table.verticalHeader().setDefaultSectionSize(28)
        bench_layout.addRow(self.bench_table)

        charts_box = QGroupBox("Визуальный анализ серии экспериментов")
        charts_layout = QGridLayout(charts_box)
        self.psnr_plot = self._create_plot_canvas()
        self.mse_plot = self._create_plot_canvas()
        self.time_plot = self._create_plot_canvas()
        self.size_psnr_plot = self._create_plot_canvas()
        charts_layout.addWidget(self.psnr_plot, 0, 0)
        charts_layout.addWidget(self.mse_plot, 0, 1)
        charts_layout.addWidget(self.size_psnr_plot, 1, 0)
        charts_layout.addWidget(self.time_plot, 1, 1)
        layout.addWidget(charts_box)

        self.benchmark_plot_status = QLabel(
            "Запустите серию экспериментов, чтобы построить графики."
        )
        self.benchmark_plot_status.setObjectName("status")
        layout.addWidget(self.benchmark_plot_status)

        layout.addWidget(benchmark_box)
        layout.addStretch()
        return tab

    @staticmethod
    def _create_plot_canvas():
        figure = Figure(figsize=(5.2, 3.1), tight_layout=True)
        canvas = PlotCanvas(figure)
        canvas.setMinimumHeight(250)
        return canvas

    @staticmethod
    def _group_average(results, x_key, y_key):
        grouped = {}
        for result in results:
            if result.get("status") != "OK":
                continue
            x = result.get(x_key)
            y = result.get(y_key)
            if x is None or y is None:
                continue
            grouped.setdefault(x, []).append(float(y))
        return sorted((x, sum(values) / len(values)) for x, values in grouped.items())

    def _plot_results(self, results):
        self._benchmark_results = list(results)

        plots = [
            (self.psnr_plot, "PSNR в зависимости от силы встраивания", "Сила", "PSNR, дБ", "strength", "psnr_db"),
            (self.mse_plot, "MSE в зависимости от силы встраивания", "Сила", "MSE", "strength", "mse"),
            (self.size_psnr_plot, "PSNR в зависимости от размера сообщения", "Размер сообщения, Б", "PSNR, дБ", "message_bytes", "psnr_db"),
        ]

        for canvas, title, xlabel, ylabel, x_key, y_key in plots:
            figure = canvas.figure
            figure.clear()
            axis = figure.add_subplot(111)
            points = self._group_average(results, x_key, y_key)
            if points:
                x_values = [point[0] for point in points]
                y_values = [point[1] for point in points]
                axis.plot(x_values, y_values, marker="o")
                axis.grid(True, alpha=0.25)
            else:
                axis.text(0.5, 0.5, "Нет успешных результатов", ha="center", va="center", transform=axis.transAxes)
            axis.set_title(title)
            axis.set_xlabel(xlabel)
            axis.set_ylabel(ylabel)
            figure.tight_layout()
            canvas.draw()

        # Total processing time = embedding + extraction; averaged by message size.
        grouped_time = {}
        for result in results:
            if result.get("status") != "OK":
                continue
            embedding = result.get("embedding_time_s")
            extraction = result.get("extraction_time_s")
            size = result.get("message_bytes")
            if embedding is None or extraction is None or size is None:
                continue
            grouped_time.setdefault(size, []).append(float(embedding) + float(extraction))
        time_points = sorted((x, sum(values) / len(values)) for x, values in grouped_time.items())

        canvas = self.time_plot
        figure = canvas.figure
        figure.clear()
        axis = figure.add_subplot(111)
        if time_points:
            axis.plot(
                [point[0] for point in time_points],
                [point[1] for point in time_points],
                marker="o",
            )
            axis.grid(True, alpha=0.25)
        else:
            axis.text(0.5, 0.5, "Нет успешных результатов", ha="center", va="center", transform=axis.transAxes)
        axis.set_title("Время обработки в зависимости от размера сообщения")
        axis.set_xlabel("Размер сообщения, Б")
        axis.set_ylabel("Время, с")
        figure.tight_layout()
        canvas.draw()

        ok = sum(result.get("status") == "OK" for result in results)
        fail = sum(result.get("status") == "FAIL" for result in results)
        skip = sum(result.get("status") == "SKIP" for result in results)
        self.benchmark_plot_status.setText(
            f"Результаты серии: успешно={ok}, ошибки={fail}, пропущено={skip}. "
            "На графиках показаны средние значения по соответствующим группам."
        )

    def _build_history_tab(self):
        tab = QWidget()
        layout = QVBoxLayout(tab)

        self.history_table = QTableWidget(0, 7)
        self.history_table.setHorizontalHeaderLabels(
            ["Дата", "Операция", "Файл", "Сообщение, байт", "Сила", "PSNR", "Результат"]
        )
        self.history_table.horizontalHeader().setStretchLastSection(True)
        layout.addWidget(self.history_table)

        refresh = QPushButton("Обновить историю")
        refresh.clicked.connect(self.refresh_history)
        layout.addWidget(refresh)
        return tab

    def _choose_file(self, edit, save=False):
        if save:
            path, selected_filter = QFileDialog.getSaveFileName(
                self,
                "Сохранить стего-изображение",
                "",
                "PNG (*.png);;BMP (*.bmp);;TIFF (*.tif *.tiff)",
            )
            if path and not Path(path).suffix:
                extension = ".png"
                if selected_filter.startswith("BMP"):
                    extension = ".bmp"
                elif selected_filter.startswith("TIFF"):
                    extension = ".tif"
                path += extension
        else:
            path, _ = QFileDialog.getOpenFileName(
                self,
                "Выбрать изображение",
                "",
                "Изображения (*.png *.jpg *.jpeg *.bmp *.tif *.tiff)",
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
                f"данные {capacity['payload_bytes']:,} байт | "
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
            self._error("Укажите путь для сохранения стего-изображения.")
            return
        if not message:
            self._error("Введите сообщение.")
            return
        # Пустой пароль означает внедрение без шифрования.

        try:
            result = embed_message(source, output, message, password or None, strength)
            extracted = extract_message(output, password or None)
            metrics = compare_images(source, output)
            verified = extracted == message

            self.source_preview.set_image(source)
            self.stego_preview.set_image(output)

            if verified:
                self._show_operation_status(
                    self.embed_status,
                    f"Встраивание выполнено успешно. Файл сохранён: {output}",
                    True,
                    "Готово к работе.",
                )
            else:
                self._show_operation_status(
                    self.embed_status,
                    "Встраивание завершено, но контрольная проверка не пройдена.",
                    False,
                    "Готово к работе.",
                )
            self.embed_metrics.setText(
                f"PSNR: {metrics['psnr_db']:.2f} дБ    "
                f"MSE: {metrics['mse']:.6f}    "
                f"Проверка: {'УСПЕХ' if verified else 'ОШИБКА'}"
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
                self._show_operation_status(
                    self.embed_status,
                    "Контрольное извлечение не подтвердило сообщение. Попробуйте изменить силу встраивания.",
                    False,
                    "Готово к работе.",
                )
        except Exception as exc:
            self._show_operation_status(
                self.embed_status,
                f"Встраивание не выполнено: {exc}",
                False,
                "Готово к работе.",
            )

    def extract(self):
        source = self.extract_source.text().strip()
        password = self.extract_password.text()

        if not source or not Path(source).exists():
            self._error("Выберите стего-изображение.")
            return
        try:
            message = extract_message(source, password or None)
            self.extracted_message.setPlainText(message)
            self._show_operation_status(
                self.extract_status,
                "Сообщение успешно извлечено и проверено.",
                True,
                "Выберите стего-изображение.",
            )
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
            self._show_operation_status(
                self.extract_status,
                f"Извлечение не выполнено: {exc}",
                False,
                "Выберите стего-изображение.",
            )

    def compare(self):
        original = self.analysis_original.text().strip()
        stego = self.analysis_stego.text().strip()

        try:
            metrics = compare_images(original, stego)
            self.analysis_result.setText(
                f"MSE: {metrics['mse']:.8f}\n"
                f"PSNR: {metrics['psnr_db']:.2f} дБ"
            )
        except Exception as exc:
            self._error(str(exc))

    def benchmark_series(self):
        image = self.bench_image.text().strip()
        password = self.bench_password.text()

        if not image or not Path(image).exists():
            self._error("Выберите изображение для серии экспериментов.")
            return
        try:
            sizes = [
                int(value.strip())
                for value in self.bench_series_sizes.text().split(",")
                if value.strip()
            ]
            if not sizes or any(size <= 0 for size in sizes):
                raise ValueError("Размеры сообщений должны быть положительными числами.")
            if len(sizes) > 10:
                raise ValueError("Можно указать не более 10 размеров сообщения.")
        except ValueError as exc:
            self._error(str(exc))
            return

        self.bench_table.setRowCount(0)
        self.bench_series_status.setText(
            f"Выполняется серия: {len(sizes) * 5} экспериментов…"
        )
        self.bench_series_button.setEnabled(False)

        try:
            import tempfile
            with tempfile.TemporaryDirectory() as work:
                results = run_benchmark_series(
                    image,
                    work,
                    password,
                    strengths=(1, 2, 3, 4, 5),
                    message_sizes=sizes,
                )

            for result in results:
                row = self.bench_table.rowCount()
                self.bench_table.insertRow(row)
                values = [
                    str(result["message_bytes"]),
                    str(result["strength"]).rstrip("0").rstrip("."),
                    {"OK": "УСПЕХ", "FAIL": "ОШИБКА", "SKIP": "ПРОПУСК"}.get(result["status"], result["status"]),
                    "—" if result["psnr_db"] is None else f"{result['psnr_db']:.2f} дБ",
                    "—" if result["mse"] is None else f"{result['mse']:.6f}",
                    "—" if result["embedding_time_s"] is None else f"{result['embedding_time_s']:.4f} с",
                    "—" if result["extraction_time_s"] is None else f"{result['extraction_time_s']:.4f} с",
                    str(result["payload_bytes"] or "—"),
                ]
                for col, value in enumerate(values):
                    self.bench_table.setItem(row, col, QTableWidgetItem(value))

            self._plot_results(results)

            ok = sum(r["status"] == "OK" for r in results)
            fail = sum(r["status"] == "FAIL" for r in results)
            skip = sum(r["status"] == "SKIP" for r in results)
            self.bench_series_status.setText(
                f"Серия завершена: успешно={ok}, ошибки={fail}, пропущено={skip}. "
                f"Всего экспериментов: {len(results)}."
            )
        except Exception as exc:
            self.bench_series_status.setText("Серия завершилась с ошибкой.")
            self._error(str(exc))
        finally:
            self.bench_series_button.setEnabled(True)

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
                "УСПЕХ" if record["success"] else "ОШИБКА",
            ]
            for col, value in enumerate(values):
                self.history_table.setItem(row, col, QTableWidgetItem(value))

    def _show_operation_status(
        self,
        label: QLabel,
        message: str,
        success: bool,
        default_text: str,
        timeout_ms: int = 5000,
    ):
        """Показывает цветное уведомление об операции на 5 секунд."""
        token_name = f"_status_token_{id(label)}"
        token = getattr(self, token_name, 0) + 1
        setattr(self, token_name, token)

        label.setText(message)
        if success:
            label.setStyleSheet(
                "background:#e9f8ee; border:1px solid #9bd3ae; "
                "border-radius:8px; padding:10px; color:#18743a; font-weight:600;"
            )
        else:
            label.setStyleSheet(
                "background:#fdecec; border:1px solid #e3a1a1; "
                "border-radius:8px; padding:10px; color:#a52828; font-weight:600;"
            )

        def clear_status():
            if getattr(self, token_name, 0) != token:
                return
            label.setText(default_text)
            label.setStyleSheet("")

        QTimer.singleShot(timeout_ms, clear_status)

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
