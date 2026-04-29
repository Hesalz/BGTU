import sys
import pyodbc
from PyQt5.QtWidgets import (QApplication, QMainWindow, QWidget, QVBoxLayout, QHBoxLayout, 
                             QLabel, QLineEdit, QPushButton, QTableWidget, QTableWidgetItem,
                             QTabWidget, QMessageBox, QComboBox, QDateTimeEdit, QSpinBox,
                             QGroupBox, QFormLayout, QTextEdit, QDateEdit, QDialog, QDoubleSpinBox)
from PyQt5.QtCore import Qt, QDateTime

class DatabaseConnection:
    def __init__(self):
        self.connection = None
        
    def connect(self):
        try:
            self.connection = pyodbc.connect(
                'DRIVER={ODBC Driver 17 for SQL Server};'
                'SERVER=PC;'
                'DATABASE=Cinema;'
                'Trusted_Connection=yes;'
                'charset=utf8;'
            )
            print("Успешное подключение к БД!")
            return True
        except pyodbc.Error as e:
            print(f"Database connection error: {e}")
            return False
    
    def disconnect(self):
        if self.connection:
            self.connection.close()
    
    def execute_query(self, query, params=None):
        try:
            cursor = self.connection.cursor()
            if params:
                cursor.execute(query, params)
            else:
                cursor.execute(query)
            self.connection.commit()
            return cursor
        except pyodbc.Error as e:
            print(f"Query execution error: {e}")
            return None
    
    def fetch_all(self, query, params=None):
        try:
            cursor = self.connection.cursor()
            if params:
                cursor.execute(query, params)
            else:
                cursor.execute(query)

            results = cursor.fetchall()
            cursor.close()  # Закрытие курсора после получения данных
            return results
        except pyodbc.Error as e:
            print(f"Ошибка при получении данных: {e}")
            return None



class LoginWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("Cinema Management System - Login")
        self.setFixedSize(300, 200)
        
        self.db = DatabaseConnection()
        if not self.db.connect():
            QMessageBox.critical(self, "Error", "Failed to connect to database!")
            sys.exit(1)
        
        self.init_ui()
        
    def init_ui(self):
        central_widget = QWidget()
        layout = QVBoxLayout()
        
        self.role_combo = QComboBox()
        self.role_combo.addItems(["Client", "Operator", "Support Staff", "Admin"])
        
        self.username_input = QLineEdit()
        self.username_input.setPlaceholderText("Username")
        
        self.password_input = QLineEdit()
        self.password_input.setPlaceholderText("Password")
        self.password_input.setEchoMode(QLineEdit.Password)
        
        login_button = QPushButton("Login")
        login_button.clicked.connect(self.handle_login)
        
        layout.addWidget(QLabel("Select Role:"))
        layout.addWidget(self.role_combo)
        layout.addWidget(QLabel("Username:"))
        layout.addWidget(self.username_input)
        layout.addWidget(QLabel("Password:"))
        layout.addWidget(self.password_input)
        layout.addWidget(login_button)
        
        central_widget.setLayout(layout)
        self.setCentralWidget(central_widget)
    
    def handle_login(self):
        role = self.role_combo.currentText()
        username = self.username_input.text()
        password = self.password_input.text()
        
        # In a real application, you would verify credentials against the database
        if not username or not password:
            QMessageBox.warning(self, "Warning", "Please enter username and password!")
            return
        
        # Simplified authentication for demo purposes
        if role == "Client" and username == "client" and password == "client":
            self.open_client_window()
        elif role == "Operator" and username == "operator" and password == "operator":
            self.open_operator_window()
        elif role == "Support Staff" and username == "support" and password == "support":
            self.open_support_window()
        elif role == "Admin" and username == "admin" and password == "admin":
            self.open_admin_window()
        else:
            QMessageBox.warning(self, "Login Failed", "Invalid credentials!")
    
    def open_client_window(self):
        self.client_window = ClientWindow(self.db)
        self.client_window.show()
        self.hide()
    
    def open_operator_window(self):
        self.operator_window = OperatorWindow(self.db)
        self.operator_window.show()
        self.hide()
    
    def open_support_window(self):
        self.support_window = SupportWindow(self.db)
        self.support_window.show()
        self.hide()
    
    def open_admin_window(self):
        self.admin_window = AdminWindow(self.db)
        self.admin_window.show()
        self.hide()
    
    def closeEvent(self, event):
        self.db.disconnect()
        event.accept()

class ClientWindow(QMainWindow):
    def __init__(self, db):
        super().__init__()
        self.db = db
        self.setWindowTitle("Cinema Management System - Client")
        self.setMinimumSize(800, 600)
        
        self.init_ui()
        self.load_movies()
        self.load_sessions()
        self.load_tickets()
        self.load_reviews()
    
    def init_ui(self):
        central_widget = QWidget()
        layout = QVBoxLayout()
        
        self.tabs = QTabWidget()
        
        # Movies Tab
        movies_tab = QWidget()
        movies_layout = QVBoxLayout()
        
        self.movies_table = QTableWidget()
        self.movies_table.setColumnCount(9)
        self.movies_table.setHorizontalHeaderLabels(["ID", "Title", "Year", "Director", "Genre", "Rating", "Cast", "Country", "Description"])
        self.movies_table.setSelectionBehavior(QTableWidget.SelectRows)
        
        filter_group = QGroupBox("Filter Movies")
        filter_layout = QHBoxLayout()
        
        self.genre_combo = QComboBox()
        self.genre_combo.addItem("All Genres")
        genres = self.db.fetch_all("SELECT DISTINCT Жанр FROM Фильмы")
        if genres:
            for genre in genres:
                self.genre_combo.addItem(genre[0])
        
        self.year_combo = QComboBox()
        self.year_combo.addItem("All Years")
        years = self.db.fetch_all("SELECT DISTINCT Год_выпуска FROM Фильмы ORDER BY Год_выпуска DESC")
        if years:
            for year in years:
                self.year_combo.addItem(str(year[0]))
        
        filter_button = QPushButton("Filter")
        filter_button.clicked.connect(self.filter_movies)
        
        filter_layout.addWidget(QLabel("Genre:"))
        filter_layout.addWidget(self.genre_combo)
        filter_layout.addWidget(QLabel("Year:"))
        filter_layout.addWidget(self.year_combo)
        filter_layout.addWidget(filter_button)
        filter_group.setLayout(filter_layout)
        
        movies_layout.addWidget(filter_group)
        movies_layout.addWidget(self.movies_table)
        movies_tab.setLayout(movies_layout)
        
        # Sessions Tab
        sessions_tab = QWidget()
        sessions_layout = QVBoxLayout()
        
        self.sessions_table = QTableWidget()
        self.sessions_table.setColumnCount(6)
        self.sessions_table.setHorizontalHeaderLabels(["ID", "Time", "Movie", "Hall", "Cinema", "Available Seats"])
        self.sessions_table.setSelectionBehavior(QTableWidget.SelectRows)
        
        sessions_filter_group = QGroupBox("Filter Sessions")
        sessions_filter_layout = QHBoxLayout()
        
        self.date_from_edit = QDateEdit()
        self.date_from_edit.setDate(QDateTime.currentDateTime().date())
        self.date_from_edit.setDisplayFormat("yyyy-MM-dd")
        
        self.date_to_edit = QDateEdit()
        self.date_to_edit.setDate(QDateTime.currentDateTime().date().addDays(7))
        self.date_to_edit.setDisplayFormat("yyyy-MM-dd")
        
        sessions_filter_button = QPushButton("Filter")
        sessions_filter_button.clicked.connect(self.filter_sessions)
        
        sessions_filter_layout.addWidget(QLabel("From:"))
        sessions_filter_layout.addWidget(self.date_from_edit)
        sessions_filter_layout.addWidget(QLabel("To:"))
        sessions_filter_layout.addWidget(self.date_to_edit)
        sessions_filter_layout.addWidget(sessions_filter_button)
        sessions_filter_group.setLayout(sessions_filter_layout)
        
        sessions_layout.addWidget(sessions_filter_group)
        sessions_layout.addWidget(self.sessions_table)
        sessions_tab.setLayout(sessions_layout)
        
        # Tickets Tab
        tickets_tab = QWidget()
        tickets_layout = QVBoxLayout()
        
        self.tickets_table = QTableWidget()
        self.tickets_table.setColumnCount(7)
        self.tickets_table.setHorizontalHeaderLabels(["ID", "Movie", "Time", "Hall", "Seat", "Status", "Price"])
        self.tickets_table.setSelectionBehavior(QTableWidget.SelectRows)
        
        buttons_layout = QHBoxLayout()
        
        buy_ticket_button = QPushButton("Buy Ticket")
        buy_ticket_button.clicked.connect(self.buy_ticket)
        
        return_ticket_button = QPushButton("Return Ticket")
        return_ticket_button.clicked.connect(self.return_ticket)
        
        pay_ticket_button = QPushButton("Pay Ticket")
        pay_ticket_button.clicked.connect(self.pay_ticket)
        
        buttons_layout.addWidget(buy_ticket_button)
        buttons_layout.addWidget(return_ticket_button)
        buttons_layout.addWidget(pay_ticket_button)
        
        tickets_layout.addWidget(self.tickets_table)
        tickets_layout.addLayout(buttons_layout)
        tickets_tab.setLayout(tickets_layout)
        
        # Reviews Tab
        reviews_tab = QWidget()
        reviews_layout = QVBoxLayout()
        
        self.reviews_table = QTableWidget()
        self.reviews_table.setColumnCount(5)
        self.reviews_table.setHorizontalHeaderLabels(["ID", "Movie", "Rating", "Text", "Date"])
        self.reviews_table.setSelectionBehavior(QTableWidget.SelectRows)
        
        add_review_group = QGroupBox("Add Review")
        add_review_layout = QFormLayout()
        
        self.review_movie_combo = QComboBox()
        movies = self.db.fetch_all("SELECT ID_фильма, Название FROM Фильмы")
        if movies:
            for movie in movies:
                self.review_movie_combo.addItem(movie[1], movie[0])
        
        self.review_rating_spin = QSpinBox()
        self.review_rating_spin.setRange(1, 10)
        
        self.review_text = QTextEdit()
        
        add_review_button = QPushButton("Add Review")
        add_review_button.clicked.connect(self.add_review)
        
        add_review_layout.addRow(QLabel("Movie:"), self.review_movie_combo)
        add_review_layout.addRow(QLabel("Rating (1-10):"), self.review_rating_spin)
        add_review_layout.addRow(QLabel("Review Text:"), self.review_text)
        add_review_layout.addRow(add_review_button)
        add_review_group.setLayout(add_review_layout)
        
        reviews_layout.addWidget(self.reviews_table)
        reviews_layout.addWidget(add_review_group)
        reviews_tab.setLayout(reviews_layout)
        
        self.tabs.addTab(movies_tab, "Movies")
        self.tabs.addTab(sessions_tab, "Sessions")
        self.tabs.addTab(tickets_tab, "My Tickets")
        self.tabs.addTab(reviews_tab, "My Reviews")
        
        layout.addWidget(self.tabs)
        central_widget.setLayout(layout)
        self.setCentralWidget(central_widget)
    
    def load_movies(self):
        movies = self.db.fetch_all("SELECT * FROM Фильмы")
        if movies:
            self.movies_table.setRowCount(len(movies))
            for row_idx, movie in enumerate(movies):
                for col_idx in range(9):
                    self.movies_table.setItem(row_idx, col_idx, QTableWidgetItem(str(movie[col_idx])))
    
    def filter_movies(self):
        genre = self.genre_combo.currentText()
        year = self.year_combo.currentText()
        
        query = "SELECT * FROM Фильмы WHERE 1=1"
        params = []
        
        if genre != "All Genres":
            query += " AND Жанр = ?"
            params.append(genre)
        
        if year != "All Years":
            query += " AND Год_выпуска = ?"
            params.append(int(year))
        
        query += " ORDER BY Название"
        
        movies = self.db.fetch_all(query, params)
        if movies:
            self.movies_table.setRowCount(len(movies))
            for row_idx, movie in enumerate(movies):
                for col_idx in range(9):
                    self.movies_table.setItem(row_idx, col_idx, QTableWidgetItem(str(movie[col_idx])))
    
    def load_sessions(self):
        sessions = self.db.fetch_all("""
            SELECT s.ID_сеанса, s.Время, f.Название, z.Номер, k.Название, 
                   dbo.GetAvailableSeatsCount(s.ID_сеанса) 
            FROM Сеанс s
            JOIN Фильмы f ON s.ID_фильма = f.ID_фильма
            JOIN Зал z ON s.ID_зала = z.ID_зала
            JOIN Кинотеатр k ON z.ID_кинотеатра = k.ID_кинотеатра
            WHERE s.Время >= GETDATE()
            ORDER BY s.Время
        """)
        
        if sessions:
            self.sessions_table.setRowCount(len(sessions))
            for row_idx, session in enumerate(sessions):
                for col_idx in range(6):
                    if col_idx == 1:
                        self.sessions_table.setItem(row_idx, col_idx, QTableWidgetItem(str(session[col_idx])[:16]))
                    else:
                        self.sessions_table.setItem(row_idx, col_idx, QTableWidgetItem(str(session[col_idx])))
    
    def filter_sessions(self):
        date_from = self.date_from_edit.date().toString("yyyy-MM-dd")
        date_to = self.date_to_edit.date().toString("yyyy-MM-dd")
        
        sessions = self.db.fetch_all("""
            SELECT s.ID_сеанса, s.Время, f.Название, z.Номер, k.Название, 
                   dbo.GetAvailableSeatsCount(s.ID_сеанса) 
            FROM Сеанс s
            JOIN Фильмы f ON s.ID_фильма = f.ID_фильма
            JOIN Зал z ON s.ID_зала = z.ID_зала
            JOIN Кинотеатр k ON z.ID_кинотеатра = k.ID_кинотеатра
            WHERE CAST(s.Время AS DATE) BETWEEN ? AND ?
            ORDER BY s.Время
        """, [date_from, date_to])
        
        if sessions:
            self.sessions_table.setRowCount(len(sessions))
            for row_idx, session in enumerate(sessions):
                for col_idx in range(6):
                    if col_idx == 1:
                        self.sessions_table.setItem(row_idx, col_idx, QTableWidgetItem(str(session[col_idx])[:16]))
                    else:
                        self.sessions_table.setItem(row_idx, col_idx, QTableWidgetItem(str(session[col_idx])))
    
    def buy_ticket(self):
        selected_row = self.sessions_table.currentRow()
        if selected_row == -1:
            QMessageBox.warning(self, "Warning", "Please select a session first!")
            return
        
        session_id = int(self.sessions_table.item(selected_row, 0).text())
        
        seats = self.db.fetch_all("""
            SELECT m.ID_места, m.Номер_ряда, m.Номер_места
            FROM Место m
            JOIN Зал z ON m.ID_зала = z.ID_зала
            JOIN Сеанс s ON z.ID_зала = s.ID_зала
            WHERE s.ID_сеанса = ? AND dbo.IsSeatAvailable(?, m.ID_места) = 1
        """, [session_id, session_id])
        
        if not seats:
            QMessageBox.warning(self, "Warning", "No available seats for this session!")
            return
        
        seat_dialog = QDialog(self)
        seat_dialog.setWindowTitle("Select Seat")
        seat_layout = QVBoxLayout()
        
        seat_combo = QComboBox()
        for seat in seats:
            seat_combo.addItem(f"Row {seat[1]}, Seat {seat[2]}", seat[0])
        
        price_spin = QDoubleSpinBox()
        price_spin.setRange(5, 50)
        price_spin.setValue(10)
        price_spin.setPrefix("$ ")
        
        confirm_button = QPushButton("Confirm Purchase")
        
        def confirm_purchase():
            seat_id = seat_combo.currentData()
            price = price_spin.value()
            
            client_id = 1  # Assuming client with ID 1 for demo
            
            result = self.db.execute_query("""
                EXEC dbo.ДобавитьБилет @Цена=?, @Льгота=NULL, @Дата_время=GETDATE(), 
                @ID_сеанса=?, @ID_места=?, @ID_клиента=?
            """, [price, session_id, seat_id, client_id])
            
            if result:
                QMessageBox.information(seat_dialog, "Success", "Ticket purchased successfully!")
                seat_dialog.accept()
                self.load_sessions()
                self.load_tickets()
            else:
                QMessageBox.warning(seat_dialog, "Error", "Failed to purchase ticket!")
        
        confirm_button.clicked.connect(confirm_purchase)
        
        seat_layout.addWidget(QLabel("Available Seats:"))
        seat_layout.addWidget(seat_combo)
        seat_layout.addWidget(QLabel("Price:"))
        seat_layout.addWidget(price_spin)
        seat_layout.addWidget(confirm_button)
        seat_dialog.setLayout(seat_layout)
        
        seat_dialog.exec_()
    
    def pay_ticket(self):
        selected_row = self.tickets_table.currentRow()
        if selected_row == -1:
            QMessageBox.warning(self, "Warning", "Please select a ticket first!")
            return
        
        ticket_id = int(self.tickets_table.item(selected_row, 0).text())
        status = self.tickets_table.item(selected_row, 5).text()
        
        if status == "Активен":
            QMessageBox.warning(self, "Warning", "Ticket is already paid!")
            return
        
        reply = QMessageBox.question(self, "Confirm Payment", 
                                    "Are you sure you want to pay for this ticket?",
                                    QMessageBox.Yes | QMessageBox.No)
        
        if reply == QMessageBox.Yes:
            result = self.db.execute_query("EXEC dbo.ОплатитьБилет @ID_билета=?", [ticket_id])
            if result:
                QMessageBox.information(self, "Success", "Ticket paid successfully!")
                self.load_tickets()
            else:
                QMessageBox.warning(self, "Error", "Failed to pay ticket!")
    
    def return_ticket(self):
        selected_row = self.tickets_table.currentRow()
        if selected_row == -1:
            QMessageBox.warning(self, "Warning", "Please select a ticket first!")
            return
        
        ticket_id = int(self.tickets_table.item(selected_row, 0).text())
        
        reply = QMessageBox.question(self, "Confirm Return", 
                                    "Are you sure you want to return this ticket?",
                                    QMessageBox.Yes | QMessageBox.No)
        
        if reply == QMessageBox.Yes:
            result = self.db.execute_query("EXEC dbo.УдалитьБилет @ID_билета=?", [ticket_id])
            if result:
                QMessageBox.information(self, "Success", "Ticket return request submitted!")
                self.load_tickets()
            else:
                QMessageBox.warning(self, "Error", "Failed to return ticket!")
    
    def add_review(self):
        movie_id = self.review_movie_combo.currentData()
        rating = self.review_rating_spin.value()
        text = self.review_text.toPlainText()
        
        client_id = 1  # Assuming client with ID 1 for demo
        
        result = self.db.execute_query("""
            EXEC dbo.ДобавитьОтзыв @ID_клиента=?, @ID_фильма=?, @Оценка=?, @Текст=?
        """, [client_id, movie_id, rating, text])
        
        if result:
            QMessageBox.information(self, "Success", "Review added successfully!")
            self.review_text.clear()
            self.load_reviews()
        else:
            QMessageBox.warning(self, "Error", "Failed to add review!")
    
    def load_tickets(self):
        client_id = 1  # Assuming client with ID 1 for demo
        
        tickets = self.db.fetch_all("""
            SELECT b.ID_билета, f.Название, s.Время, z.Номер, 
                   CONCAT(m.Номер_ряда, '-', m.Номер_места), b.Статус, b.Цена
            FROM Билет b
            JOIN Сеанс s ON b.ID_сеанса = s.ID_сеанса
            JOIN Фильмы f ON s.ID_фильма = f.ID_фильма
            JOIN Зал z ON s.ID_зала = z.ID_зала
            JOIN Место m ON b.ID_места = m.ID_места
            WHERE b.ID_клиента = ?
            ORDER BY s.Время DESC
        """, [client_id])
        
        if tickets:
            self.tickets_table.setRowCount(len(tickets))
            for row_idx, ticket in enumerate(tickets):
                for col_idx in range(7):
                    if col_idx == 2:
                        self.tickets_table.setItem(row_idx, col_idx, QTableWidgetItem(str(ticket[col_idx])[:16]))
                    else:
                        self.tickets_table.setItem(row_idx, col_idx, QTableWidgetItem(str(ticket[col_idx])))
    
    def load_reviews(self):
        client_id = 1  # Assuming client with ID 1 for demo
        
        reviews = self.db.fetch_all("""
            SELECT r.ID_отзыва, f.Название, r.Оценка, r.Текст, r.Дата
            FROM Отзывы r
            JOIN Фильмы f ON r.ID_фильма = f.ID_фильма
            WHERE r.ID_клиента = ?
            ORDER BY r.Дата DESC
        """, [client_id])
        
        if reviews:
            self.reviews_table.setRowCount(len(reviews))
            for row_idx, review in enumerate(reviews):
                for col_idx in range(5):
                    if col_idx == 4:
                        self.reviews_table.setItem(row_idx, col_idx, QTableWidgetItem(str(review[col_idx])[:10]))
                    else:
                        self.reviews_table.setItem(row_idx, col_idx, QTableWidgetItem(str(review[col_idx])))
    def __init__(self, db):
        super().__init__()
        self.db = db
        self.setWindowTitle("Cinema Management System - Client")
        self.setMinimumSize(800, 600)
        
        self.init_ui()
        self.load_movies()
        self.load_sessions()
    
    def init_ui(self):
        central_widget = QWidget()
        layout = QVBoxLayout()
        
        self.tabs = QTabWidget()
        
        # Movies Tab
        movies_tab = QWidget()
        movies_layout = QVBoxLayout()
        
        self.movies_table = QTableWidget()
        self.movies_table.setColumnCount(9)
        self.movies_table.setHorizontalHeaderLabels(["ID", "Title", "Year", "Director", "Genre", "Rating", "Cast", "Country", "Description"])
        self.movies_table.setSelectionBehavior(QTableWidget.SelectRows)
        
        filter_group = QGroupBox("Filter Movies")
        filter_layout = QHBoxLayout()
        
        self.genre_combo = QComboBox()
        self.genre_combo.addItem("All Genres")
        genres = self.db.fetch_all("SELECT DISTINCT Жанр FROM Фильмы")
        if genres:
            for genre in genres:
                self.genre_combo.addItem(genre[0])
        
        self.year_combo = QComboBox()
        self.year_combo.addItem("All Years")
        years = self.db.fetch_all("SELECT DISTINCT Год_выпуска FROM Фильмы ORDER BY Год_выпуска DESC")
        if years:
            for year in years:
                self.year_combo.addItem(str(year[0]))
        
        filter_button = QPushButton("Filter")
        filter_button.clicked.connect(self.filter_movies)
        
        filter_layout.addWidget(QLabel("Genre:"))
        filter_layout.addWidget(self.genre_combo)
        filter_layout.addWidget(QLabel("Year:"))
        filter_layout.addWidget(self.year_combo)
        filter_layout.addWidget(filter_button)
        filter_group.setLayout(filter_layout)
        
        movies_layout.addWidget(filter_group)
        movies_layout.addWidget(self.movies_table)
        movies_tab.setLayout(movies_layout)
        
        # Sessions Tab
        sessions_tab = QWidget()
        sessions_layout = QVBoxLayout()
        
        self.sessions_table = QTableWidget()
        self.sessions_table.setColumnCount(6)
        self.sessions_table.setHorizontalHeaderLabels(["ID", "Time", "Movie", "Hall", "Cinema", "Available Seats"])
        self.sessions_table.setSelectionBehavior(QTableWidget.SelectRows)
        
        sessions_filter_group = QGroupBox("Filter Sessions")
        sessions_filter_layout = QHBoxLayout()
        
        self.date_from_edit = QDateTimeEdit()
        self.date_from_edit.setDateTime(QDateTime.currentDateTime())
        
        self.date_to_edit = QDateTimeEdit()
        self.date_to_edit.setDateTime(QDateTime.currentDateTime().addDays(7))
        
        sessions_filter_button = QPushButton("Filter")
        sessions_filter_button.clicked.connect(self.filter_sessions)
        
        sessions_filter_layout.addWidget(QLabel("From:"))
        sessions_filter_layout.addWidget(self.date_from_edit)
        sessions_filter_layout.addWidget(QLabel("To:"))
        sessions_filter_layout.addWidget(self.date_to_edit)
        sessions_filter_layout.addWidget(sessions_filter_button)
        sessions_filter_group.setLayout(sessions_filter_layout)
        
        sessions_layout.addWidget(sessions_filter_group)
        sessions_layout.addWidget(self.sessions_table)
        sessions_tab.setLayout(sessions_layout)
        
        # Tickets Tab
        tickets_tab = QWidget()
        tickets_layout = QVBoxLayout()
        
        self.tickets_table = QTableWidget()
        self.tickets_table.setColumnCount(6)
        self.tickets_table.setHorizontalHeaderLabels(["ID", "Movie", "Time", "Hall", "Seat", "Status"])
        self.tickets_table.setSelectionBehavior(QTableWidget.SelectRows)
        
        buy_ticket_button = QPushButton("Buy Ticket")
        buy_ticket_button.clicked.connect(self.buy_ticket)
        
        return_ticket_button = QPushButton("Return Ticket")
        return_ticket_button.clicked.connect(self.return_ticket)
        
        tickets_buttons_layout = QHBoxLayout()
        tickets_buttons_layout.addWidget(buy_ticket_button)
        tickets_buttons_layout.addWidget(return_ticket_button)
        
        tickets_layout.addWidget(self.tickets_table)
        tickets_layout.addLayout(tickets_buttons_layout)
        tickets_tab.setLayout(tickets_layout)
        
        # Reviews Tab
        reviews_tab = QWidget()
        reviews_layout = QVBoxLayout()
        
        self.reviews_table = QTableWidget()
        self.reviews_table.setColumnCount(5)
        self.reviews_table.setHorizontalHeaderLabels(["ID", "Movie", "Rating", "Text", "Date"])
        self.reviews_table.setSelectionBehavior(QTableWidget.SelectRows)
        
        add_review_group = QGroupBox("Add Review")
        add_review_layout = QFormLayout()
        
        self.review_movie_combo = QComboBox()
        movies = self.db.fetch_all("SELECT ID_фильма, Название FROM Фильмы")
        if movies:
            for movie in movies:
                self.review_movie_combo.addItem(movie[1], movie[0])
        
        self.review_rating_spin = QSpinBox()
        self.review_rating_spin.setRange(1, 10)
        
        self.review_text = QTextEdit()
        
        add_review_button = QPushButton("Add Review")
        add_review_button.clicked.connect(self.add_review)
        
        add_review_layout.addRow(QLabel("Movie:"), self.review_movie_combo)
        add_review_layout.addRow(QLabel("Rating (1-10):"), self.review_rating_spin)
        add_review_layout.addRow(QLabel("Review Text:"), self.review_text)
        add_review_layout.addRow(add_review_button)
        add_review_group.setLayout(add_review_layout)
        
        reviews_layout.addWidget(self.reviews_table)
        reviews_layout.addWidget(add_review_group)
        reviews_tab.setLayout(reviews_layout)
        
        self.tabs.addTab(movies_tab, "Movies")
        self.tabs.addTab(sessions_tab, "Sessions")
        self.tabs.addTab(tickets_tab, "My Tickets")
        self.tabs.addTab(reviews_tab, "My Reviews")
        
        layout.addWidget(self.tabs)
        central_widget.setLayout(layout)
        self.setCentralWidget(central_widget)
    
    def load_movies(self):
        movies = self.db.fetch_all("SELECT * FROM Фильмы")
        if movies:
            self.movies_table.setRowCount(len(movies))
            for row_idx, movie in enumerate(movies):
                for col_idx in range(9):
                    self.movies_table.setItem(row_idx, col_idx, QTableWidgetItem(str(movie[col_idx])))
    
    def filter_movies(self):
        genre = self.genre_combo.currentText()
        year = self.year_combo.currentText()
        
        query = "SELECT * FROM Фильмы WHERE 1=1"
        params = []
        
        if genre != "All Genres":
            query += " AND Жанр = ?"
            params.append(genre)
        
        if year != "All Years":
            query += " AND Год_выпуска = ?"
            params.append(int(year))
        
        query += " ORDER BY Название"
        
        movies = self.db.fetch_all(query, params)
        if movies:
            self.movies_table.setRowCount(len(movies))
            for row_idx, movie in enumerate(movies):
                for col_idx in range(9):
                    self.movies_table.setItem(row_idx, col_idx, QTableWidgetItem(str(movie[col_idx])))
    
    def load_sessions(self):
        sessions = self.db.fetch_all("""
            SELECT s.ID_сеанса, s.Время, f.Название, z.Номер, k.Название, 
                   dbo.GetAvailableSeatsCount(s.ID_сеанса) 
            FROM Сеанс s
            JOIN Фильмы f ON s.ID_фильма = f.ID_фильма
            JOIN Зал z ON s.ID_зала = z.ID_зала
            JOIN Кинотеатр k ON z.ID_кинотеатра = k.ID_кинотеатра
            WHERE s.Время >= GETDATE()
            ORDER BY s.Время
        """)
        
        if sessions:
            self.sessions_table.setRowCount(len(sessions))
            for row_idx, session in enumerate(sessions):
                for col_idx in range(6):
                    if col_idx == 1:  # Format datetime
                        self.sessions_table.setItem(row_idx, col_idx, QTableWidgetItem(str(session[col_idx])[:16]))
                    else:
                        self.sessions_table.setItem(row_idx, col_idx, QTableWidgetItem(str(session[col_idx])))
    
    def filter_sessions(self):
        date_from = self.date_from_edit.dateTime().toString("yyyy-MM-dd HH:mm:ss")
        date_to = self.date_to_edit.dateTime().toString("yyyy-MM-dd HH:mm:ss")
        
        sessions = self.db.fetch_all("""
            SELECT s.ID_сеанса, s.Время, f.Название, z.Номер, k.Название, 
                   dbo.GetAvailableSeatsCount(s.ID_сеанса) 
            FROM Сеанс s
            JOIN Фильмы f ON s.ID_фильма = f.ID_фильма
            JOIN Зал z ON s.ID_зала = z.ID_зала
            JOIN Кинотеатр k ON z.ID_кинотеатра = k.ID_кинотеатра
            WHERE s.Время BETWEEN ? AND ?
            ORDER BY s.Время
        """, [date_from, date_to])
        
        if sessions:
            self.sessions_table.setRowCount(len(sessions))
            for row_idx, session in enumerate(sessions):
                for col_idx in range(6):
                    if col_idx == 1:  # Format datetime
                        self.sessions_table.setItem(row_idx, col_idx, QTableWidgetItem(str(session[col_idx])[:16]))
                    else:
                        self.sessions_table.setItem(row_idx, col_idx, QTableWidgetItem(str(session[col_idx])))
    
    def buy_ticket(self):
        selected_row = self.sessions_table.currentRow()
        if selected_row == -1:
            QMessageBox.warning(self, "Warning", "Please select a session first!")
            return
        
        session_id = int(self.sessions_table.item(selected_row, 0).text())
        
        # Get available seats for this session
        seats = self.db.fetch_all("""
            SELECT m.ID_места, m.Номер_ряда, m.Номер_места
            FROM Место m
            JOIN Зал z ON m.ID_зала = z.ID_зала
            JOIN Сеанс s ON z.ID_зала = s.ID_зала
            WHERE s.ID_сеанса = ? AND dbo.IsSeatAvailable(?, m.ID_места) = 1
        """, [session_id, session_id])
        
        if not seats:
            QMessageBox.warning(self, "Warning", "No available seats for this session!")
            return
        
        # Show seat selection dialog
        seat_dialog = QDialog(self)
        seat_dialog.setWindowTitle("Select Seat")
        seat_layout = QVBoxLayout()
        
        seat_combo = QComboBox()
        for seat in seats:
            seat_combo.addItem(f"Row {seat[1]}, Seat {seat[2]}", seat[0])
        
        price_spin = QDoubleSpinBox()
        price_spin.setRange(5, 50)
        price_spin.setValue(10)
        price_spin.setPrefix("$ ")
        
        confirm_button = QPushButton("Confirm Purchase")
        
        def confirm_purchase():
            seat_id = seat_combo.currentData()
            price = price_spin.value()
            
            # In a real app, you would use the actual client ID
            client_id = 1  # Assuming client with ID 1 for demo
            
            result = self.db.execute_query("""
                EXEC dbo.ДобавитьБилет @Цена=?, @Льгота=NULL, @Дата_время=GETDATE(), 
                @ID_сеанса=?, @ID_места=?, @ID_клиента=?
            """, [price, session_id, seat_id, client_id])
            
            if result:
                QMessageBox.information(seat_dialog, "Success", "Ticket purchased successfully!")
                seat_dialog.accept()
                self.load_sessions()
            else:
                QMessageBox.warning(seat_dialog, "Error", "Failed to purchase ticket!")
        
        confirm_button.clicked.connect(confirm_purchase)
        
        seat_layout.addWidget(QLabel("Available Seats:"))
        seat_layout.addWidget(seat_combo)
        seat_layout.addWidget(QLabel("Price:"))
        seat_layout.addWidget(price_spin)
        seat_layout.addWidget(confirm_button)
        seat_dialog.setLayout(seat_layout)
        
        seat_dialog.exec_()
    
    def return_ticket(self):
        selected_row = self.tickets_table.currentRow()
        if selected_row == -1:
            QMessageBox.warning(self, "Warning", "Please select a ticket first!")
            return
        
        ticket_id = int(self.tickets_table.item(selected_row, 0).text())
        
        reply = QMessageBox.question(self, "Confirm Return", 
                                    "Are you sure you want to return this ticket?",
                                    QMessageBox.Yes | QMessageBox.No)
        
        if reply == QMessageBox.Yes:
            result = self.db.execute_query("EXEC dbo.УдалитьБилет @ID_билета=?", [ticket_id])
            if result:
                QMessageBox.information(self, "Success", "Ticket return request submitted!")
                self.load_tickets()
            else:
                QMessageBox.warning(self, "Error", "Failed to return ticket!")
    
    def add_review(self):
        movie_id = self.review_movie_combo.currentData()
        rating = self.review_rating_spin.value()
        text = self.review_text.toPlainText()
        
        # In a real app, you would use the actual client ID
        client_id = 1  # Assuming client with ID 1 for demo
        
        result = self.db.execute_query("""
            EXEC dbo.ДобавитьОтзыв @ID_клиента=?, @ID_фильма=?, @Оценка=?, @Текст=?
        """, [client_id, movie_id, rating, text])
        
        if result:
            QMessageBox.information(self, "Success", "Review added successfully!")
            self.review_text.clear()
            self.load_reviews()
        else:
            QMessageBox.warning(self, "Error", "Failed to add review!")
    
    def load_tickets(self):
        # In a real app, you would use the actual client ID
        client_id = 1  # Assuming client with ID 1 for demo
        
        tickets = self.db.fetch_all("""
            SELECT b.ID_билета, f.Название, s.Время, z.Номер, 
                   CONCAT(m.Номер_ряда, '-', m.Номер_места), b.Статус
            FROM Билет b
            JOIN Сеанс s ON b.ID_сеанса = s.ID_сеанса
            JOIN Фильмы f ON s.ID_фильма = f.ID_фильма
            JOIN Зал z ON s.ID_зала = z.ID_зала
            JOIN Место m ON b.ID_места = m.ID_места
            WHERE b.ID_клиента = ?
            ORDER BY s.Время DESC
        """, [client_id])
        
        if tickets:
            self.tickets_table.setRowCount(len(tickets))
            for row_idx, ticket in enumerate(tickets):
                for col_idx in range(6):
                    if col_idx == 2:  # Format datetime
                        self.tickets_table.setItem(row_idx, col_idx, QTableWidgetItem(str(ticket[col_idx])[:16]))
                    else:
                        self.tickets_table.setItem(row_idx, col_idx, QTableWidgetItem(str(ticket[col_idx])))
    
    def load_reviews(self):
        # In a real app, you would use the actual client ID
        client_id = 1  # Assuming client with ID 1 for demo
        
        reviews = self.db.fetch_all("""
            SELECT r.ID_отзыва, f.Название, r.Оценка, r.Текст, r.Дата
            FROM Отзывы r
            JOIN Фильмы f ON r.ID_фильма = f.ID_фильма
            WHERE r.ID_клиента = ?
            ORDER BY r.Дата DESC
        """, [client_id])
        
        if reviews:
            self.reviews_table.setRowCount(len(reviews))
            for row_idx, review in enumerate(reviews):
                for col_idx in range(5):
                    if col_idx == 4:  # Format date
                        self.reviews_table.setItem(row_idx, col_idx, QTableWidgetItem(str(review[col_idx])[:10]))
                    else:
                        self.reviews_table.setItem(row_idx, col_idx, QTableWidgetItem(str(review[col_idx])))

class OperatorWindow(QMainWindow):
    def __init__(self, db):
        super().__init__()
        self.db = db
        self.setWindowTitle("Cinema Management System - Operator")
        self.setMinimumSize(800, 600)
        
        self.init_ui()
        self.load_movies()
        self.load_sessions()
        self.load_halls()
        self.load_seats()
    
    def init_ui(self):
        central_widget = QWidget()
        layout = QVBoxLayout()
        
        self.tabs = QTabWidget()
        
        seats_tab = QWidget()
        seats_layout = QVBoxLayout()
        
        self.seats_table = QTableWidget()
        self.seats_table.setColumnCount(4)
        self.seats_table.setHorizontalHeaderLabels(["ID", "Зал", "Ряд", "Место"])
        self.seats_table.setSelectionBehavior(QTableWidget.SelectRows)
        
        seats_buttons_layout = QHBoxLayout()
        
        add_seat_button = QPushButton("Добавить место")
        add_seat_button.clicked.connect(self.show_add_seat_dialog)
        
        delete_seat_button = QPushButton("Удалить место")
        delete_seat_button.clicked.connect(self.delete_seat)
        
        seats_buttons_layout.addWidget(add_seat_button)
        seats_buttons_layout.addWidget(delete_seat_button)
        
        seats_layout.addWidget(self.seats_table)
        seats_layout.addLayout(seats_buttons_layout)
        seats_tab.setLayout(seats_layout)
    
        
        layout.addWidget(self.tabs)
        central_widget.setLayout(layout)
        self.setCentralWidget(central_widget)

        # Movies Tab
        movies_tab = QWidget()
        movies_layout = QVBoxLayout()
        
        self.movies_table = QTableWidget()
        self.movies_table.setColumnCount(9)
        self.movies_table.setHorizontalHeaderLabels(["ID", "Title", "Year", "Director", "Genre", "Rating", "Cast", "Country", "Description"])
        self.movies_table.setSelectionBehavior(QTableWidget.SelectRows)
        
        movie_buttons_layout = QHBoxLayout()
        
        add_movie_button = QPushButton("Add Movie")
        add_movie_button.clicked.connect(self.show_add_movie_dialog)
        
        edit_movie_button = QPushButton("Edit Movie")
        edit_movie_button.clicked.connect(self.show_edit_movie_dialog)
        
        delete_movie_button = QPushButton("Delete Movie")
        delete_movie_button.clicked.connect(self.delete_movie)
        
        movie_buttons_layout.addWidget(add_movie_button)
        movie_buttons_layout.addWidget(edit_movie_button)
        movie_buttons_layout.addWidget(delete_movie_button)
        
        movies_layout.addWidget(self.movies_table)
        movies_layout.addLayout(movie_buttons_layout)
        movies_tab.setLayout(movies_layout)
        
        # Sessions Tab
        sessions_tab = QWidget()
        sessions_layout = QVBoxLayout()
        
        self.sessions_table = QTableWidget()
        self.sessions_table.setColumnCount(6)
        self.sessions_table.setHorizontalHeaderLabels(["ID", "Time", "Movie", "Hall", "Cinema", "Available Seats"])
        self.sessions_table.setSelectionBehavior(QTableWidget.SelectRows)
        
        session_buttons_layout = QHBoxLayout()
        
        add_session_button = QPushButton("Add Session")
        add_session_button.clicked.connect(self.show_add_session_dialog)
        
        edit_session_button = QPushButton("Edit Session")
        edit_session_button.clicked.connect(self.show_edit_session_dialog)
        
        delete_session_button = QPushButton("Delete Session")
        delete_session_button.clicked.connect(self.delete_session)
        
        session_buttons_layout.addWidget(add_session_button)
        session_buttons_layout.addWidget(edit_session_button)
        session_buttons_layout.addWidget(delete_session_button)
        
        sessions_layout.addWidget(self.sessions_table)
        sessions_layout.addLayout(session_buttons_layout)
        sessions_tab.setLayout(sessions_layout)
        
        # Halls Tab
        halls_tab = QWidget()
        halls_layout = QVBoxLayout()
        
        self.halls_table = QTableWidget()
        self.halls_table.setColumnCount(4)
        self.halls_table.setHorizontalHeaderLabels(["ID", "Number", "Type", "Cinema"])
        self.halls_table.setSelectionBehavior(QTableWidget.SelectRows)
        
        hall_buttons_layout = QHBoxLayout()
        
        add_hall_button = QPushButton("Add Hall")
        add_hall_button.clicked.connect(self.show_add_hall_dialog)
        
        edit_hall_button = QPushButton("Edit Hall")
        edit_hall_button.clicked.connect(self.show_edit_hall_dialog)
        
        delete_hall_button = QPushButton("Delete Hall")
        delete_hall_button.clicked.connect(self.delete_hall)
        
        hall_buttons_layout.addWidget(add_hall_button)
        hall_buttons_layout.addWidget(edit_hall_button)
        hall_buttons_layout.addWidget(delete_hall_button)
        
        halls_layout.addWidget(self.halls_table)
        halls_layout.addLayout(hall_buttons_layout)
        halls_tab.setLayout(halls_layout)
        
        self.tabs.addTab(movies_tab, "Movies")
        self.tabs.addTab(sessions_tab, "Sessions")
        self.tabs.addTab(halls_tab, "Halls")
        self.tabs.addTab(seats_tab, "Места")
        
        layout.addWidget(self.tabs)
        central_widget.setLayout(layout)
        self.setCentralWidget(central_widget)
    
    def load_seats(self):
        """Загрузка списка мест из базы данных"""
        seats = self.db.fetch_all("""
            SELECT m.ID_места, z.Номер, m.Номер_ряда, m.Номер_места
            FROM Место m
            JOIN Зал z ON m.ID_зала = z.ID_зала
            ORDER BY z.Номер, m.Номер_ряда, m.Номер_места
        """)
        
        if seats:
            self.seats_table.setRowCount(len(seats))
            for row_idx, seat in enumerate(seats):
                for col_idx in range(4):
                    self.seats_table.setItem(row_idx, col_idx, QTableWidgetItem(str(seat[col_idx])))
    
    def show_add_seat_dialog(self):
        """Диалог добавления нового места"""
        dialog = QDialog(self)
        dialog.setWindowTitle("Добавить новое место")
        layout = QFormLayout()
        
        hall_combo = QComboBox()
        halls = self.db.fetch_all("SELECT ID_зала, Номер FROM Зал ORDER BY Номер")
        if halls:
            for hall in halls:
                hall_combo.addItem(f"Зал {hall[1]}", hall[0])
        
        row_spin = QSpinBox()
        row_spin.setRange(1, 50)
        row_spin.setValue(1)
        
        seat_spin = QSpinBox()
        seat_spin.setRange(1, 100)
        seat_spin.setValue(1)
        
        save_button = QPushButton("Сохранить")
        
        def save_seat():
            hall_id = hall_combo.currentData()
            row = row_spin.value()
            seat = seat_spin.value()
            
            result = self.db.execute_query("""
                EXEC dbo.ДобавитьМесто @ID_зала=?, @Номер_ряда=?, @Номер_места=?
            """, [hall_id, row, seat])
            
            if result:
                QMessageBox.information(dialog, "Успех", "Место успешно добавлено!")
                dialog.accept()
                self.load_seats()
            else:
                QMessageBox.warning(dialog, "Ошибка", "Не удалось добавить место!")
        
        save_button.clicked.connect(save_seat)
        
        layout.addRow(QLabel("Зал:"), hall_combo)
        layout.addRow(QLabel("Номер ряда:"), row_spin)
        layout.addRow(QLabel("Номер места:"), seat_spin)
        layout.addRow(save_button)
        
        dialog.setLayout(layout)
        dialog.exec_()
    
    def delete_seat(self):
        """Удаление выбранного места"""
        selected_row = self.seats_table.currentRow()
        if selected_row == -1:
            QMessageBox.warning(self, "Ошибка", "Выберите место для удаления!")
            return
        
        seat_id = int(self.seats_table.item(selected_row, 0).text())
        
        reply = QMessageBox.question(
            self, 
            "Подтверждение", 
            "Вы уверены, что хотите удалить это место?",
            QMessageBox.Yes | QMessageBox.No
        )
        
        if reply == QMessageBox.Yes:
            result = self.db.execute_query("EXEC dbo.УдалитьМесто @ID_места=?", [seat_id])
            if result:
                QMessageBox.information(self, "Успех", "Место успешно удалено!")
                self.load_seats()
            else:
                QMessageBox.warning(self, "Ошибка", "Не удалось удалить место!")

    def load_movies(self):
        movies = self.db.fetch_all("SELECT * FROM Фильмы")
        if movies:
            self.movies_table.setRowCount(len(movies))
            for row_idx, movie in enumerate(movies):
                for col_idx in range(9):
                    self.movies_table.setItem(row_idx, col_idx, QTableWidgetItem(str(movie[col_idx])))
    
    def show_add_movie_dialog(self):
        dialog = QDialog(self)
        dialog.setWindowTitle("Add New Movie")
        layout = QFormLayout()
        
        title_edit = QLineEdit()
        year_edit = QSpinBox()
        year_edit.setRange(1900, 2100)
        year_edit.setValue(2023)
        director_edit = QLineEdit()
        genre_edit = QLineEdit()
        rating_edit = QLineEdit()
        cast_edit = QTextEdit()
        country_edit = QLineEdit()
        description_edit = QTextEdit()
        
        save_button = QPushButton("Save")
        
        def save_movie():
            result = self.db.execute_query("""
                EXEC dbo.ДобавитьФильм 
                @Название=?, @Год_выпуска=?, @Режиссёр=?, @Жанр=?, 
                @Рейтинг=?, @Роли=?, @Страна=?, @Описание=?, @Ограничение=?
            """, [
                title_edit.text(),
                year_edit.value(),
                director_edit.text(),
                genre_edit.text(),
                rating_edit.text(),
                cast_edit.toPlainText(),
                country_edit.text(),
                description_edit.toPlainText(),
                "PG-13"  # Default restriction
            ])
            
            if result:
                QMessageBox.information(dialog, "Success", "Movie added successfully!")
                dialog.accept()
                self.load_movies()
            else:
                QMessageBox.warning(dialog, "Error", "Failed to add movie!")
        
        save_button.clicked.connect(save_movie)
        
        layout.addRow(QLabel("Title:"), title_edit)
        layout.addRow(QLabel("Year:"), year_edit)
        layout.addRow(QLabel("Director:"), director_edit)
        layout.addRow(QLabel("Genre:"), genre_edit)
        layout.addRow(QLabel("Rating:"), rating_edit)
        layout.addRow(QLabel("Cast:"), cast_edit)
        layout.addRow(QLabel("Country:"), country_edit)
        layout.addRow(QLabel("Description:"), description_edit)
        layout.addRow(save_button)
        
        dialog.setLayout(layout)
        dialog.exec_()
    
    def show_edit_movie_dialog(self):
        selected_row = self.movies_table.currentRow()
        if selected_row == -1:
            QMessageBox.warning(self, "Warning", "Please select a movie first!")
            return
        
        movie_id = int(self.movies_table.item(selected_row, 0).text())
        
        dialog = QDialog(self)
        dialog.setWindowTitle("Edit Movie")
        layout = QFormLayout()
        
        # Get current movie data
        movie = self.db.fetch_all("SELECT * FROM Фильмы WHERE ID_фильма = ?", [movie_id])[0]
        
        title_edit = QLineEdit(movie[1])
        year_edit = QSpinBox()
        year_edit.setRange(1900, 2100)
        year_edit.setValue(movie[2])
        director_edit = QLineEdit(movie[3])
        genre_edit = QLineEdit(movie[4])
        rating_edit = QLineEdit(movie[5] if movie[5] else "")
        cast_edit = QTextEdit(movie[6] if movie[6] else "")
        country_edit = QLineEdit(movie[7] if movie[7] else "")
        description_edit = QTextEdit(movie[8] if movie[8] else "")
        
        save_button = QPushButton("Save")
        
        def save_movie():
            result = self.db.execute_query("""
                EXEC dbo.ОбновитьФильм 
                @ID_фильма=?, @Название=?, @Год_выпуска=?, @Режиссёр=?, @Жанр=?, 
                @Рейтинг=?, @Роли=?, @Страна=?, @Описание=?, @Ограничение=?
            """, [
                movie_id,
                title_edit.text(),
                year_edit.value(),
                director_edit.text(),
                genre_edit.text(),
                rating_edit.text(),
                cast_edit.toPlainText(),
                country_edit.text(),
                description_edit.toPlainText(),
                "PG-13"  # Default restriction
            ])
            
            if result:
                QMessageBox.information(dialog, "Success", "Movie updated successfully!")
                dialog.accept()
                self.load_movies()
            else:
                QMessageBox.warning(dialog, "Error", "Failed to update movie!")
        
        save_button.clicked.connect(save_movie)
        
        layout.addRow(QLabel("Title:"), title_edit)
        layout.addRow(QLabel("Year:"), year_edit)
        layout.addRow(QLabel("Director:"), director_edit)
        layout.addRow(QLabel("Genre:"), genre_edit)
        layout.addRow(QLabel("Rating:"), rating_edit)
        layout.addRow(QLabel("Cast:"), cast_edit)
        layout.addRow(QLabel("Country:"), country_edit)
        layout.addRow(QLabel("Description:"), description_edit)
        layout.addRow(save_button)
        
        dialog.setLayout(layout)
        dialog.exec_()
    
    def delete_movie(self):
        selected_row = self.movies_table.currentRow()
        if selected_row == -1:
            QMessageBox.warning(self, "Warning", "Please select a movie first!")
            return
        
        movie_id = int(self.movies_table.item(selected_row, 0).text())
        
        reply = QMessageBox.question(self, "Confirm Delete", 
                                    "Are you sure you want to delete this movie?",
                                    QMessageBox.Yes | QMessageBox.No)
        
        if reply == QMessageBox.Yes:
            result = self.db.execute_query("EXEC dbo.УдалитьФильм @ID_фильма=?", [movie_id])
            if result:
                QMessageBox.information(self, "Success", "Movie deleted successfully!")
                self.load_movies()
                self.load_sessions()
            else:
                QMessageBox.warning(self, "Error", "Failed to delete movie!")
    
    def load_sessions(self):
        sessions = self.db.fetch_all("""
            SELECT s.ID_сеанса, s.Время, f.Название, z.Номер, k.Название, 
                   dbo.GetAvailableSeatsCount(s.ID_сеанса) 
            FROM Сеанс s
            JOIN Фильмы f ON s.ID_фильма = f.ID_фильма
            JOIN Зал z ON s.ID_зала = z.ID_зала
            JOIN Кинотеатр k ON z.ID_кинотеатра = k.ID_кинотеатра
            WHERE s.Время >= GETDATE()
            ORDER BY s.Время
        """)
        
        if sessions:
            self.sessions_table.setRowCount(len(sessions))
            for row_idx, session in enumerate(sessions):
                for col_idx in range(6):
                    if col_idx == 1:  # Format datetime
                        self.sessions_table.setItem(row_idx, col_idx, QTableWidgetItem(str(session[col_idx])[:16]))
                    else:
                        self.sessions_table.setItem(row_idx, col_idx, QTableWidgetItem(str(session[col_idx])))
    
    def show_add_session_dialog(self):
        dialog = QDialog(self)
        dialog.setWindowTitle("Add New Session")
        layout = QFormLayout()
        
        time_edit = QDateTimeEdit()
        time_edit.setDateTime(QDateTime.currentDateTime().addDays(1))
        time_edit.setDisplayFormat("yyyy-MM-dd HH:mm")
        
        movie_combo = QComboBox()
        movies = self.db.fetch_all("SELECT ID_фильма, Название FROM Фильмы")
        if movies:
            for movie in movies:
                movie_combo.addItem(movie[1], movie[0])
        
        hall_combo = QComboBox()
        halls = self.db.fetch_all("""
            SELECT z.ID_зала, z.Номер, k.Название 
            FROM Зал z
            JOIN Кинотеатр k ON z.ID_кинотеатра = k.ID_кинотеатра
        """)
        if halls:
            for hall in halls:
                hall_combo.addItem(f"Hall {hall[1]} ({hall[2]})", hall[0])
        
        save_button = QPushButton("Save")
        
        def save_session():
            result = self.db.execute_query("""
                EXEC dbo.ДобавитьСеанс @Время=?, @ID_фильма=?, @ID_зала=?
            """, [
                time_edit.dateTime().toString("yyyy-MM-dd HH:mm:ss"),
                movie_combo.currentData(),
                hall_combo.currentData()
            ])
            
            if result:
                QMessageBox.information(dialog, "Success", "Session added successfully!")
                dialog.accept()
                self.load_sessions()
            else:
                QMessageBox.warning(dialog, "Error", "Failed to add session!")
        
        save_button.clicked.connect(save_session)
        
        layout.addRow(QLabel("Time:"), time_edit)
        layout.addRow(QLabel("Movie:"), movie_combo)
        layout.addRow(QLabel("Hall:"), hall_combo)
        layout.addRow(save_button)
        
        dialog.setLayout(layout)
        dialog.exec_()
    
    def show_edit_session_dialog(self):
        selected_row = self.sessions_table.currentRow()
        if selected_row == -1:
            QMessageBox.warning(self, "Warning", "Please select a session first!")
            return
        
        session_id = int(self.sessions_table.item(selected_row, 0).text())
        
        dialog = QDialog(self)
        dialog.setWindowTitle("Edit Session")
        layout = QFormLayout()
        
        # Get current session data
        session = self.db.fetch_all("""
            SELECT s.Время, s.ID_фильма, s.ID_зала, f.Название, z.Номер, k.Название
            FROM Сеанс s
            JOIN Фильмы f ON s.ID_фильма = f.ID_фильма
            JOIN Зал z ON s.ID_зала = z.ID_зала
            JOIN Кинотеатр k ON z.ID_кинотеатра = k.ID_кинотеатра
            WHERE s.ID_сеанса = ?
        """, [session_id])[0]
        
        time_edit = QDateTimeEdit()
        time_edit.setDateTime(QDateTime.fromString(str(session[0]), "yyyy-MM-dd HH:mm:ss"))
        time_edit.setDisplayFormat("yyyy-MM-dd HH:mm")
        
        movie_combo = QComboBox()
        movies = self.db.fetch_all("SELECT ID_фильма, Название FROM Фильмы")
        if movies:
            for movie in movies:
                movie_combo.addItem(movie[1], movie[0])
                if movie[0] == session[1]:
                    movie_combo.setCurrentIndex(movie_combo.count() - 1)
        
        hall_combo = QComboBox()
        halls = self.db.fetch_all("""
            SELECT z.ID_зала, z.Номер, k.Название 
            FROM Зал z
            JOIN Кинотеатр k ON z.ID_кинотеатра = k.ID_кинотеатра
        """)
        if halls:
            for hall in halls:
                hall_combo.addItem(f"Hall {hall[1]} ({hall[2]})", hall[0])
                if hall[0] == session[2]:
                    hall_combo.setCurrentIndex(hall_combo.count() - 1)
        
        save_button = QPushButton("Save")
        
        def save_session():
            result = self.db.execute_query("""
                EXEC dbo.ОбновитьСеанс @ID_сеанса=?, @Время=?, @ID_фильма=?, @ID_зала=?
            """, [
                session_id,
                time_edit.dateTime().toString("yyyy-MM-dd HH:mm:ss"),
                movie_combo.currentData(),
                hall_combo.currentData()
            ])
            
            if result:
                QMessageBox.information(dialog, "Success", "Session updated successfully!")
                dialog.accept()
                self.load_sessions()
            else:
                QMessageBox.warning(dialog, "Error", "Failed to update session!")
        
        save_button.clicked.connect(save_session)
        
        layout.addRow(QLabel("Time:"), time_edit)
        layout.addRow(QLabel("Movie:"), movie_combo)
        layout.addRow(QLabel("Hall:"), hall_combo)
        layout.addRow(save_button)
        
        dialog.setLayout(layout)
        dialog.exec_()
    
    def delete_session(self):
        selected_row = self.sessions_table.currentRow()
        if selected_row == -1:
            QMessageBox.warning(self, "Warning", "Please select a session first!")
            return
        
        session_id = int(self.sessions_table.item(selected_row, 0).text())
        
        reply = QMessageBox.question(self, "Confirm Delete", 
                                    "Are you sure you want to delete this session?",
                                    QMessageBox.Yes | QMessageBox.No)
        
        if reply == QMessageBox.Yes:
            result = self.db.execute_query("EXEC dbo.УдалитьСеанс @ID_сеанса=?", [session_id])
            if result:
                QMessageBox.information(self, "Success", "Session deleted successfully!")
                self.load_sessions()
            else:
                QMessageBox.warning(self, "Error", "Failed to delete session!")
    
    def load_halls(self):
        halls = self.db.fetch_all("""
            SELECT z.ID_зала, z.Номер, z.Тип_зала, k.Название
            FROM Зал z
            JOIN Кинотеатр k ON z.ID_кинотеатра = k.ID_кинотеатра
        """)
        
        if halls:
            self.halls_table.setRowCount(len(halls))
            for row_idx, hall in enumerate(halls):
                for col_idx in range(4):
                    self.halls_table.setItem(row_idx, col_idx, QTableWidgetItem(str(hall[col_idx])))
    
    def show_add_hall_dialog(self):
        dialog = QDialog(self)
        dialog.setWindowTitle("Add New Hall")
        layout = QFormLayout()
        
        number_edit = QSpinBox()
        number_edit.setRange(1, 100)
        
        type_edit = QLineEdit()
        type_edit.setPlaceholderText("e.g., Standard, VIP, IMAX")
        
        cinema_combo = QComboBox()
        cinemas = self.db.fetch_all("SELECT ID_кинотеатра, Название FROM Кинотеатр")
        if cinemas:
            for cinema in cinemas:
                cinema_combo.addItem(cinema[1], cinema[0])
        
        save_button = QPushButton("Save")
        
        def save_hall():
            result = self.db.execute_query("""
                EXEC dbo.ДобавитьЗал @Номер=?, @Тип_зала=?, @ID_кинотеатра=?
            """, [
                number_edit.value(),
                type_edit.text(),
                cinema_combo.currentData()
            ])
            
            if result:
                QMessageBox.information(dialog, "Success", "Hall added successfully!")
                dialog.accept()
                self.load_halls()
            else:
                QMessageBox.warning(dialog, "Error", "Failed to add hall!")
        
        save_button.clicked.connect(save_hall)
        
        layout.addRow(QLabel("Hall Number:"), number_edit)
        layout.addRow(QLabel("Hall Type:"), type_edit)
        layout.addRow(QLabel("Cinema:"), cinema_combo)
        layout.addRow(save_button)
        
        dialog.setLayout(layout)
        dialog.exec_()
    
    def show_edit_hall_dialog(self):
        selected_row = self.halls_table.currentRow()
        if selected_row == -1:
            QMessageBox.warning(self, "Warning", "Please select a hall first!")
            return
        
        hall_id = int(self.halls_table.item(selected_row, 0).text())
        
        dialog = QDialog(self)
        dialog.setWindowTitle("Edit Hall")
        layout = QFormLayout()
        
        # Get current hall data
        hall = self.db.fetch_all("""
            SELECT z.Номер, z.Тип_зала, z.ID_кинотеатра, k.Название
            FROM Зал z
            JOIN Кинотеатр k ON z.ID_кинотеатра = k.ID_кинотеатра
            WHERE z.ID_зала = ?
        """, [hall_id])[0]
        
        number_edit = QSpinBox()
        number_edit.setRange(1, 100)
        number_edit.setValue(hall[0])
        
        type_edit = QLineEdit(hall[1] if hall[1] else "")
        
        cinema_combo = QComboBox()
        cinemas = self.db.fetch_all("SELECT ID_кинотеатра, Название FROM Кинотеатр")
        if cinemas:
            for cinema in cinemas:
                cinema_combo.addItem(cinema[1], cinema[0])
                if cinema[0] == hall[2]:
                    cinema_combo.setCurrentIndex(cinema_combo.count() - 1)
        
        save_button = QPushButton("Save")
        
        def save_hall():
            result = self.db.execute_query("""
                EXEC dbo.ОбновитьЗал @ID_зала=?, @Номер=?, @Тип_зала=?, @ID_кинотеатра=?
            """, [
                hall_id,
                number_edit.value(),
                type_edit.text(),
                cinema_combo.currentData()
            ])
            
            if result:
                QMessageBox.information(dialog, "Success", "Hall updated successfully!")
                dialog.accept()
                self.load_halls()
            else:
                QMessageBox.warning(dialog, "Error", "Failed to update hall!")
        
        save_button.clicked.connect(save_hall)
        
        layout.addRow(QLabel("Hall Number:"), number_edit)
        layout.addRow(QLabel("Hall Type:"), type_edit)
        layout.addRow(QLabel("Cinema:"), cinema_combo)
        layout.addRow(save_button)
        
        dialog.setLayout(layout)
        dialog.exec_()
    
    def delete_hall(self):
        selected_row = self.halls_table.currentRow()
        if selected_row == -1:
            QMessageBox.warning(self, "Warning", "Please select a hall first!")
            return
        
        hall_number = int(self.halls_table.item(selected_row, 1).text())
        
        reply = QMessageBox.question(self, "Confirm Delete", 
                                    "Are you sure you want to delete this hall?",
                                    QMessageBox.Yes | QMessageBox.No)
        
        if reply == QMessageBox.Yes:
            result = self.db.execute_query("EXEC dbo.УдалитьЗал @Номер=?", [hall_number])
            if result:
                QMessageBox.information(self, "Success", "Hall deleted successfully!")
                self.load_halls()
                self.load_sessions()
            else:
                QMessageBox.warning(self, "Error", "Failed to delete hall!")

class SupportWindow(QMainWindow):
    def __init__(self, db):
        super().__init__()
        self.db = db
        self.setWindowTitle("Cinema Management System - Support Staff")
        self.setMinimumSize(800, 600)
        
        self.init_ui()
        self.load_tickets()
        self.load_requests("Ожидает")
    
    def init_ui(self):
        central_widget = QWidget()
        layout = QVBoxLayout()
        
        self.tabs = QTabWidget()
        
        # Tickets Tab
        tickets_tab = QWidget()
        tickets_layout = QVBoxLayout()
        
        self.tickets_table = QTableWidget()
        self.tickets_table.setColumnCount(7)
        self.tickets_table.setHorizontalHeaderLabels(["ID", "Movie", "Time", "Hall", "Seat", "Client", "Status"])
        self.tickets_table.setSelectionBehavior(QTableWidget.SelectRows)
        
        process_ticket_button = QPushButton("Process Ticket Return")
        process_ticket_button.clicked.connect(self.process_ticket_return)
        
        tickets_layout.addWidget(self.tickets_table)
        tickets_layout.addWidget(process_ticket_button)
        tickets_tab.setLayout(tickets_layout)
        
        # Requests Tab
        requests_tab = QWidget()
        requests_layout = QVBoxLayout()
        
        self.requests_table = QTableWidget()
        self.requests_table.setColumnCount(6)
        self.requests_table.setHorizontalHeaderLabels(["ID", "Client", "Subject", "Date", "Status", "Response"])
        self.requests_table.setSelectionBehavior(QTableWidget.SelectRows)
        
        status_combo = QComboBox()
        status_combo.addItems(["Ожидает", "Обработан"])
        status_combo.currentTextChanged.connect(self.load_requests)
        
        response_group = QGroupBox("Respond to Request")
        response_layout = QFormLayout()
        
        self.response_text = QTextEdit()
        
        respond_button = QPushButton("Submit Response")
        respond_button.clicked.connect(self.submit_response)
        
        response_layout.addRow(QLabel("Response:"), self.response_text)
        response_layout.addRow(respond_button)
        response_group.setLayout(response_layout)
        
        requests_layout.addWidget(QLabel("Filter by Status:"))
        requests_layout.addWidget(status_combo)
        requests_layout.addWidget(self.requests_table)
        requests_layout.addWidget(response_group)
        requests_tab.setLayout(requests_layout)
        
        self.tabs.addTab(tickets_tab, "Ticket Returns")
        self.tabs.addTab(requests_tab, "Support Requests")
        
        layout.addWidget(self.tabs)
        central_widget.setLayout(layout)
        self.setCentralWidget(central_widget)
    
    def load_tickets(self):
        tickets = self.db.fetch_all("""
            SELECT r.ID_возврата, f.Название, s.Время, z.Номер, 
                   CONCAT(m.Номер_ряда, '-', m.Номер_места),
                   c.Фамилия + ' ' + c.Имя, r.Статус
            FROM Возвраты r
            JOIN Билет b ON r.ID_билета = b.ID_билета
            JOIN Сеанс s ON b.ID_сеанса = s.ID_сеанса
            JOIN Фильмы f ON s.ID_фильма = f.ID_фильма
            JOIN Зал z ON s.ID_зала = z.ID_зала
            JOIN Место m ON b.ID_места = m.ID_места
            JOIN Клиент c ON b.ID_клиента = c.ID_клиента
            WHERE r.Статус = 'Не обработан'
            ORDER BY r.Дата_время
        """)
        
        if tickets:
            self.tickets_table.setRowCount(len(tickets))
            for row_idx, ticket in enumerate(tickets):
                for col_idx in range(7):
                    if col_idx == 2:  # Format datetime
                        self.tickets_table.setItem(row_idx, col_idx, QTableWidgetItem(str(ticket[col_idx])[:16]))
                    else:
                        self.tickets_table.setItem(row_idx, col_idx, QTableWidgetItem(str(ticket[col_idx])))
    
    def process_ticket_return(self):
        selected_row = self.tickets_table.currentRow()
        if selected_row == -1:
            QMessageBox.warning(self, "Warning", "Please select a ticket first!")
            return
        
        return_id = int(self.tickets_table.item(selected_row, 0).text())
        
        reply = QMessageBox.question(self, "Confirm Process", 
                                    "Are you sure you want to process this ticket return?",
                                    QMessageBox.Yes | QMessageBox.No)
        
        if reply == QMessageBox.Yes:
            result = self.db.execute_query("EXEC dbo.ОбработатьВозврат @ID_возврата=?", [return_id])
            if result:
                QMessageBox.information(self, "Success", "Ticket return processed successfully!")
                self.load_tickets()
            else:
                QMessageBox.warning(self, "Error", "Failed to process ticket return!")
    
    def load_requests(self, status):
        requests = self.db.fetch_all("""
            SELECT o.ID_обращения, c.Фамилия + ' ' + c.Имя, o.Тема, o.Дата, o.Статус, o.Ответ
            FROM Обращения o
            JOIN Клиент c ON o.ID_клиента = c.ID_клиента
            WHERE o.Статус = ?
            ORDER BY o.Дата DESC
        """, [status])
        
        if requests:
            self.requests_table.setRowCount(len(requests))
            for row_idx, request in enumerate(requests):
                for col_idx in range(6):
                    if col_idx == 3:  # Format date
                        self.requests_table.setItem(row_idx, col_idx, QTableWidgetItem(str(request[col_idx])[:10]))
                    else:
                        self.requests_table.setItem(row_idx, col_idx, QTableWidgetItem(str(request[col_idx])))
    
    def submit_response(self):
        selected_row = self.requests_table.currentRow()
        if selected_row == -1:
            QMessageBox.warning(self, "Warning", "Please select a request first!")
            return
        
        request_id = int(self.requests_table.item(selected_row, 0).text())
        response = self.response_text.toPlainText()
        
        if not response:
            QMessageBox.warning(self, "Warning", "Please enter a response!")
            return
        
        result = self.db.execute_query("""
            EXEC dbo.ОбновитьСтатусОбращения @ID_обращения=?, @Статус=?, @Ответ=?
        """, [request_id, "Обработан", response])
        
        if result:
            QMessageBox.information(self, "Success", "Response submitted successfully!")
            self.response_text.clear()
            self.load_requests("Ожидает")
        else:
            QMessageBox.warning(self, "Error", "Failed to submit response!")

class AdminWindow(QMainWindow):
    def __init__(self, db):
        super().__init__()
        self.db = db
        self.setWindowTitle("Cinema Management System - Admin")
        self.setMinimumSize(800, 600)
        
        self.init_ui()
        self.load_users()
        self.load_cinemas()  # Загружаем список кинотеатров
    
    def init_ui(self):
        central_widget = QWidget()
        layout = QVBoxLayout()
        
        self.tabs = QTabWidget()
        
        # Users Tab
        users_tab = QWidget()
        users_layout = QVBoxLayout()
        
        self.users_table = QTableWidget()
        self.users_table.setColumnCount(4)
        self.users_table.setHorizontalHeaderLabels(["ID", "Name", "Email", "Role"])
        self.users_table.setSelectionBehavior(QTableWidget.SelectRows)
        
        user_buttons_layout = QHBoxLayout()
        
        add_user_button = QPushButton("Add User")
        add_user_button.clicked.connect(self.show_add_user_dialog)
        
        edit_user_button = QPushButton("Edit User")
        edit_user_button.clicked.connect(self.show_edit_user_dialog)
        
        delete_user_button = QPushButton("Delete User")
        delete_user_button.clicked.connect(self.delete_user)
        
        user_buttons_layout.addWidget(add_user_button)
        user_buttons_layout.addWidget(edit_user_button)
        user_buttons_layout.addWidget(delete_user_button)
        
        users_layout.addWidget(self.users_table)
        users_layout.addLayout(user_buttons_layout)
        users_tab.setLayout(users_layout)
        
        # Cinemas Tab (новая вкладка для кинотеатров)
        cinemas_tab = QWidget()
        cinemas_layout = QVBoxLayout()
        
        self.cinemas_table = QTableWidget()
        self.cinemas_table.setColumnCount(5)
        self.cinemas_table.setHorizontalHeaderLabels(["ID", "Name", "Address", "Operator", "Contacts"])
        self.cinemas_table.setSelectionBehavior(QTableWidget.SelectRows)
        
        cinema_buttons_layout = QHBoxLayout()
        
        add_cinema_button = QPushButton("Add Cinema")
        add_cinema_button.clicked.connect(self.show_add_cinema_dialog)
        
        edit_cinema_button = QPushButton("Edit Cinema")
        edit_cinema_button.clicked.connect(self.show_edit_cinema_dialog)
        
        delete_cinema_button = QPushButton("Delete Cinema")
        delete_cinema_button.clicked.connect(self.delete_cinema)
        
        cinema_buttons_layout.addWidget(add_cinema_button)
        cinema_buttons_layout.addWidget(edit_cinema_button)
        cinema_buttons_layout.addWidget(delete_cinema_button)
        
        cinemas_layout.addWidget(self.cinemas_table)
        cinemas_layout.addLayout(cinema_buttons_layout)
        cinemas_tab.setLayout(cinemas_layout)
        
        # Data Management Tab
        data_tab = QWidget()
        data_layout = QVBoxLayout()
        
        export_button = QPushButton("Export Movies to JSON")
        export_button.clicked.connect(self.export_movies)
        
        import_button = QPushButton("Import Movies from JSON")
        import_button.clicked.connect(self.import_movies)
        
        data_layout.addWidget(export_button)
        data_layout.addWidget(import_button)
        data_layout.addStretch()
        data_tab.setLayout(data_layout)
        
        self.tabs.addTab(users_tab, "User Management")
        self.tabs.addTab(cinemas_tab, "Cinema Management")  # Новая вкладка
        self.tabs.addTab(data_tab, "Data Management")
        
        layout.addWidget(self.tabs)
        central_widget.setLayout(layout)
        self.setCentralWidget(central_widget)
    
    def load_cinemas(self):
        """Загрузка списка кинотеатров из базы данных"""
        cinemas = self.db.fetch_all("""
            SELECT k.ID_кинотеатра, k.Название, k.Адрес, 
                   o.Фамилия + ' ' + o.Имя AS Оператор, k.Контакты
            FROM Кинотеатр k
            JOIN Оператор o ON k.ID_оператора = o.ID_оператора
        """)
        
        if cinemas:
            self.cinemas_table.setRowCount(len(cinemas))
            for row_idx, cinema in enumerate(cinemas):
                for col_idx in range(5):
                    self.cinemas_table.setItem(row_idx, col_idx, QTableWidgetItem(str(cinema[col_idx])))
    
    def show_add_cinema_dialog(self):
        """Диалог добавления нового кинотеатра"""
        dialog = QDialog(self)
        dialog.setWindowTitle("Add New Cinema")
        layout = QFormLayout()
        
        name_edit = QLineEdit()
        address_edit = QTextEdit()
        contacts_edit = QLineEdit()
        
        # Выбор оператора из списка
        operator_combo = QComboBox()
        operators = self.db.fetch_all("SELECT ID_оператора, Фамилия + ' ' + Имя FROM Оператор")
        if operators:
            for operator in operators:
                operator_combo.addItem(operator[1], operator[0])
        
        save_button = QPushButton("Save")
        
        def save_cinema():
            name = name_edit.text()
            address = address_edit.toPlainText()
            contacts = contacts_edit.text()
            operator_id = operator_combo.currentData()
            
            if not all([name, address, operator_id]):
                QMessageBox.warning(dialog, "Warning", "Please fill all required fields!")
                return
            
            result = self.db.execute_query("""
                INSERT INTO Кинотеатр (Название, Адрес, Контакты, ID_оператора)
                VALUES (?, ?, ?, ?)
            """, [name, address, contacts, operator_id])
            
            if result:
                QMessageBox.information(dialog, "Success", "Cinema added successfully!")
                dialog.accept()
                self.load_cinemas()
            else:
                QMessageBox.warning(dialog, "Error", "Failed to add cinema!")
        
        save_button.clicked.connect(save_cinema)
        
        layout.addRow(QLabel("Name*:"), name_edit)
        layout.addRow(QLabel("Address*:"), address_edit)
        layout.addRow(QLabel("Contacts:"), contacts_edit)
        layout.addRow(QLabel("Operator*:"), operator_combo)
        layout.addRow(save_button)
        
        dialog.setLayout(layout)
        dialog.exec_()
    
    def show_edit_cinema_dialog(self):
        """Диалог редактирования кинотеатра"""
        selected_row = self.cinemas_table.currentRow()
        if selected_row == -1:
            QMessageBox.warning(self, "Warning", "Please select a cinema first!")
            return
        
        cinema_id = int(self.cinemas_table.item(selected_row, 0).text())
        
        dialog = QDialog(self)
        dialog.setWindowTitle("Edit Cinema")
        layout = QFormLayout()
        
        # Получаем текущие данные кинотеатра
        cinema = self.db.fetch_all("""
            SELECT Название, Адрес, Контакты, ID_оператора 
            FROM Кинотеатр 
            WHERE ID_кинотеатра = ?
        """, [cinema_id])[0]
        
        name_edit = QLineEdit(cinema[0])
        address_edit = QTextEdit(cinema[1])
        contacts_edit = QLineEdit(cinema[2] if cinema[2] else "")
        
        # Выбор оператора из списка
        operator_combo = QComboBox()
        operators = self.db.fetch_all("SELECT ID_оператора, Фамилия + ' ' + Имя FROM Оператор")
        if operators:
            for operator in operators:
                operator_combo.addItem(operator[1], operator[0])
                if operator[0] == cinema[3]:
                    operator_combo.setCurrentIndex(operator_combo.count() - 1)
        
        save_button = QPushButton("Save")
        
        def save_cinema():
            result = self.db.execute_query("""
                UPDATE Кинотеатр 
                SET Название=?, Адрес=?, Контакты=?, ID_оператора=?
                WHERE ID_кинотеатра=?
            """, [
                name_edit.text(),
                address_edit.toPlainText(),
                contacts_edit.text(),
                operator_combo.currentData(),
                cinema_id
            ])
            
            if result:
                QMessageBox.information(dialog, "Success", "Cinema updated successfully!")
                dialog.accept()
                self.load_cinemas()
            else:
                QMessageBox.warning(dialog, "Error", "Failed to update cinema!")
        
        save_button.clicked.connect(save_cinema)
        
        layout.addRow(QLabel("Name:"), name_edit)
        layout.addRow(QLabel("Address:"), address_edit)
        layout.addRow(QLabel("Contacts:"), contacts_edit)
        layout.addRow(QLabel("Operator:"), operator_combo)
        layout.addRow(save_button)
        
        dialog.setLayout(layout)
        dialog.exec_()
    
    def delete_cinema(self):
        """Удаление кинотеатра"""
        selected_row = self.cinemas_table.currentRow()
        if selected_row == -1:
            QMessageBox.warning(self, "Warning", "Please select a cinema first!")
            return
        
        cinema_id = int(self.cinemas_table.item(selected_row, 0).text())
        
        reply = QMessageBox.question(
            self, 
            "Confirm Delete", 
            "Are you sure you want to delete this cinema? This will also delete all associated halls!",
            QMessageBox.Yes | QMessageBox.No
        )
        
        if reply == QMessageBox.Yes:
            result = self.db.execute_query("DELETE FROM Кинотеатр WHERE ID_кинотеатра = ?", [cinema_id])
            if result:
                QMessageBox.information(self, "Success", "Cinema deleted successfully!")
                self.load_cinemas()
            else:
                QMessageBox.warning(self, "Error", "Failed to delete cinema!")
    
    def load_users(self):
        # Combine clients, operators and support staff for admin view
        clients = self.db.fetch_all("""
            SELECT ID_клиента, Фамилия + ' ' + Имя, Email, 'Client' AS Role 
            FROM Клиент
        """)
        
        operators = self.db.fetch_all("""
            SELECT ID_оператора, Фамилия + ' ' + Имя, Email, 'Operator' AS Role 
            FROM Оператор
        """)
        
        support = self.db.fetch_all("""
            SELECT ID_сотрудника, Фамилия + ' ' + Имя, Email, 'Support Staff' AS Role 
            FROM Служба_поддержки
        """)
        
        all_users = []
        if clients:
            all_users.extend(clients)
        if operators:
            all_users.extend(operators)
        if support:
            all_users.extend(support)
        
        if all_users:
            self.users_table.setRowCount(len(all_users))
            for row_idx, user in enumerate(all_users):
                for col_idx in range(4):
                    self.users_table.setItem(row_idx, col_idx, QTableWidgetItem(str(user[col_idx])))
    
    def show_add_user_dialog(self):
        dialog = QDialog(self)
        dialog.setWindowTitle("Add New User")
        layout = QFormLayout()
        
        role_combo = QComboBox()
        role_combo.addItems(["Client", "Operator", "Support Staff"])
        
        first_name_edit = QLineEdit()
        last_name_edit = QLineEdit()
        email_edit = QLineEdit()
        phone_edit = QLineEdit()
        password_edit = QLineEdit()
        password_edit.setEchoMode(QLineEdit.Password)
        
        save_button = QPushButton("Save")
        
        def save_user():
            role = role_combo.currentText()
            first_name = first_name_edit.text()
            last_name = last_name_edit.text()
            email = email_edit.text()
            phone = phone_edit.text()
            password = password_edit.text()
            
            if not all([first_name, last_name, email, phone, password]):
                QMessageBox.warning(dialog, "Warning", "Please fill all fields!")
                return
            
            if role == "Client":
                result = self.db.execute_query("""
                    INSERT INTO Клиент (Фамилия, Имя, Email, Телефон, Пароль)
                    VALUES (?, ?, ?, ?, ?)
                """, [last_name, first_name, email, phone, password])
            elif role == "Operator":
                result = self.db.execute_query("""
                    INSERT INTO Оператор (Фамилия, Имя, Email, Телефон, Пароль)
                    VALUES (?, ?, ?, ?, ?)
                """, [last_name, first_name, email, phone, password])
            elif role == "Support Staff":
                result = self.db.execute_query("""
                    INSERT INTO Служба_поддержки (Фамилия, Имя, Email, Телефон, Пароль)
                    VALUES (?, ?, ?, ?, ?)
                """, [last_name, first_name, email, phone, password])
            
            if result:
                QMessageBox.information(dialog, "Success", "User added successfully!")
                dialog.accept()
                self.load_users()
            else:
                QMessageBox.warning(dialog, "Error", "Failed to add user!")
        
        save_button.clicked.connect(save_user)
        
        layout.addRow(QLabel("Role:"), role_combo)
        layout.addRow(QLabel("First Name:"), first_name_edit)
        layout.addRow(QLabel("Last Name:"), last_name_edit)
        layout.addRow(QLabel("Email:"), email_edit)
        layout.addRow(QLabel("Phone:"), phone_edit)
        layout.addRow(QLabel("Password:"), password_edit)
        layout.addRow(save_button)
        
        dialog.setLayout(layout)
        dialog.exec_()
    
    def show_edit_user_dialog(self):
        selected_row = self.users_table.currentRow()
        if selected_row == -1:
            QMessageBox.warning(self, "Warning", "Please select a user first!")
            return
        
        user_id = int(self.users_table.item(selected_row, 0).text())
        role = self.users_table.item(selected_row, 3).text()
        
        dialog = QDialog(self)
        dialog.setWindowTitle("Edit User")
        layout = QFormLayout()
        
        # Get current user data
        if role == "Client":
            user = self.db.fetch_all("SELECT * FROM Клиент WHERE ID_клиента = ?", [user_id])[0]
        elif role == "Operator":
            user = self.db.fetch_all("SELECT * FROM Оператор WHERE ID_оператора = ?", [user_id])[0]
        elif role == "Support Staff":
            user = self.db.fetch_all("SELECT * FROM Служба_поддержки WHERE ID_сотрудника = ?", [user_id])[0]
        
        first_name_edit = QLineEdit(user[2])
        last_name_edit = QLineEdit(user[1])
        email_edit = QLineEdit(user[4])
        phone_edit = QLineEdit(user[5])
        
        save_button = QPushButton("Save")
        
        def save_user():
            result = False
            if role == "Client":
                result = self.db.execute_query("""
                    UPDATE Клиент 
                    SET Фамилия=?, Имя=?, Email=?, Телефон=?
                    WHERE ID_клиента=?
                """, [
                    last_name_edit.text(),
                    first_name_edit.text(),
                    email_edit.text(),
                    phone_edit.text(),
                    user_id
                ])
            elif role == "Operator":
                result = self.db.execute_query("""
                    UPDATE Оператор 
                    SET Фамилия=?, Имя=?, Email=?, Телефон=?
                    WHERE ID_оператора=?
                """, [
                    last_name_edit.text(),
                    first_name_edit.text(),
                    email_edit.text(),
                    phone_edit.text(),
                    user_id
                ])
            elif role == "Support Staff":
                result = self.db.execute_query("""
                    UPDATE Служба_поддержки 
                    SET Фамилия=?, Имя=?, Email=?, Телефон=?
                    WHERE ID_сотрудника=?
                """, [
                    last_name_edit.text(),
                    first_name_edit.text(),
                    email_edit.text(),
                    phone_edit.text(),
                    user_id
                ])
            
            if result:
                QMessageBox.information(dialog, "Success", "User updated successfully!")
                dialog.accept()
                self.load_users()
            else:
                QMessageBox.warning(dialog, "Error", "Failed to update user!")
        
        save_button.clicked.connect(save_user)
        
        layout.addRow(QLabel("First Name:"), first_name_edit)
        layout.addRow(QLabel("Last Name:"), last_name_edit)
        layout.addRow(QLabel("Email:"), email_edit)
        layout.addRow(QLabel("Phone:"), phone_edit)
        layout.addRow(save_button)
        
        dialog.setLayout(layout)
        dialog.exec_()
    
    def delete_user(self):
        selected_row = self.users_table.currentRow()
        if selected_row == -1:
            QMessageBox.warning(self, "Warning", "Please select a user first!")
            return
        
        user_id = int(self.users_table.item(selected_row, 0).text())
        role = self.users_table.item(selected_row, 3).text()
        
        reply = QMessageBox.question(self, "Confirm Delete", 
                                    "Are you sure you want to delete this user?",
                                    QMessageBox.Yes | QMessageBox.No)
        
        if reply == QMessageBox.Yes:
            result = False
            if role == "Client":
                result = self.db.execute_query("DELETE FROM Клиент WHERE ID_клиента = ?", [user_id])
            elif role == "Operator":
                result = self.db.execute_query("DELETE FROM Оператор WHERE ID_оператора = ?", [user_id])
            elif role == "Support Staff":
                result = self.db.execute_query("DELETE FROM Служба_поддержки WHERE ID_сотрудника = ?", [user_id])
            
            if result:
                QMessageBox.information(self, "Success", "User deleted successfully!")
                self.load_users()
            else:
                QMessageBox.warning(self, "Error", "Failed to delete user!")
    
    def export_movies(self):
        result = self.db.execute_query("EXEC dbo.ExportFilmsToJSON")
        if result:
            QMessageBox.information(self, "Success", "Movies exported successfully!")
        else:
            QMessageBox.warning(self, "Error", "Failed to export movies!")
    
    def import_movies(self):
        result = self.db.execute_query("EXEC dbo.ImportFilmsFromJSON")
        if result:
            QMessageBox.information(self, "Success", "Movies imported successfully!")
        else:
            QMessageBox.warning(self, "Error", "Failed to import movies!")

if __name__ == "__main__":
    app = QApplication(sys.argv)
    login_window = LoginWindow()
    login_window.show()
    sys.exit(app.exec_())