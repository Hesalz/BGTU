import telebot
from telebot import types
import cv2
import numpy as np
import io
import json
import os
from datetime import datetime

bot = telebot.TeleBot("your_token_from_telegram_bot")

user_images = {}
DATA_FILE = os.path.join(os.path.dirname(__file__), "bot_data.json")

def load_finance_data():
    try:
        if not os.path.exists(DATA_FILE):
            print(f"Файл {DATA_FILE} не существует, возвращаю пустой словарь")
            return {}
        
        with open(DATA_FILE, 'r') as f:
            data = json.load(f)
            print(f"Загружены данные: {data}")
            return data
    except Exception as e:
        print(f"Ошибка загрузки данных: {e}")
        return {}

def save_finance_data(data):
    try:
        print(f"Сохранение данных: {data}")
        with open(DATA_FILE, 'w') as f:
            json.dump(data, f, indent=2)
        print(f"Данные успешно сохранены в {DATA_FILE}")
    except Exception as e:
        print(f"Ошибка сохранения данных: {e}")


def show_finance_menu(source):
    chat_id = source.message.chat.id if hasattr(source, 'message') else source.chat.id
    markup = types.InlineKeyboardMarkup()
    markup.add(types.InlineKeyboardButton("Добавить доход", callback_data="add_income"))
    markup.add(types.InlineKeyboardButton("Добавить расход", callback_data="add_expense"))
    markup.add(types.InlineKeyboardButton("Показать баланс", callback_data="show_balance"))
    markup.add(types.InlineKeyboardButton("История операций", callback_data="show_history"))
    bot.send_message(chat_id, "💰 Финансовый менеджер:", reply_markup=markup)
    
def handle_finance_callback(call):
    try:
        if call.data == "add_income":
            msg = bot.send_message(call.from_user.id, "Введите сумму дохода:") 
            bot.register_next_step_handler(msg, add_income)
        elif call.data == "add_expense":
            msg = bot.send_message(call.from_user.id, "Введите сумму расхода:")
            bot.register_next_step_handler(msg, add_expense)
        elif call.data == "show_balance":
            show_balance(call) 
        elif call.data == "show_history":
            show_history(call) 
    except:
        bot.send_message(call.from_user.id, "❌ Произошла ошибка")

def add_income(message):
    try:
        amount = float(message.text)
        user_id = str(message.from_user.id)
        data = load_finance_data()
        
        if user_id not in data:
            data[user_id] = {"balance": 0, "transactions": []}
        
        data[user_id]["balance"] += amount
        data[user_id]["transactions"].append({
            "type": "income",
            "amount": amount,
            "date": datetime.now().strftime("%d.%m.%Y %H:%M")
        })
        
        save_finance_data(data) 
        bot.send_message(message.chat.id, f"📈 Доход {amount} добавлен. Баланс: {data[user_id]['balance']}")
    except:
        bot.send_message(message.chat.id, "❌ Ошибка! Введите число, например: 100")

def add_expense(message):
    try:
        amount = float(message.text)
        user_id = str(message.from_user.id)
        data = load_finance_data()
        
        if user_id not in data:
            data[user_id] = {"balance": 0, "transactions": []}
        
        if data[user_id]["balance"] - amount < 0:
            bot.send_message(message.chat.id, "❌ Недостаточно средств на балансе!")
            return

        data[user_id]["balance"] -= amount
        data[user_id]["transactions"].append({
            "type": "expense",
            "amount": amount,
            "date": datetime.now().strftime("%d.%m.%Y %H:%M")
        })
        
        save_finance_data(data)
        bot.send_message(message.chat.id, f"📉 Расход {amount} добавлен. Баланс: {data[user_id]['balance']}")
    except:
        bot.send_message(message.chat.id, "❌ Ошибка! Введите число, например: 50")
        
def show_balance(source):
    try:
        if hasattr(source, 'from_user'):
            user_id = str(source.from_user.id)
            chat_id = source.message.chat.id
        else:
            user_id = str(source.from_user.id)
            chat_id = source.chat.id
            
        print(f"Запрошен баланс для user_id: {user_id}")
        data = load_finance_data()
        
        if user_id in data:
            bot.send_message(chat_id, f"💰 Ваш баланс: {data[user_id]['balance']}")
        else:
            bot.send_message(chat_id, "💡 У вас пока нет операций. Баланс: 0")
    except Exception as e:
        print(f"Ошибка в show_balance: {e}")
        bot.send_message(chat_id, "❌ Ошибка загрузки баланса")

def show_history(source):
    """Показывает историю операций (принимает message или call)"""
    try:
        if hasattr(source, 'from_user'):
            user_id = str(source.from_user.id)
            chat_id = source.message.chat.id
        else:
            user_id = str(source.from_user.id)
            chat_id = source.chat.id
            
        print(f"Запрошена история для user_id: {user_id}")
        data = load_finance_data()
        
        if user_id not in data or not data[user_id]["transactions"]:
            bot.send_message(chat_id, "📭 У вас пока нет операций")
            return
        
        history = "📋 История операций:\n"
        for tx in reversed(data[user_id]["transactions"][-10:]):
            sign = "+" if tx["type"] == "income" else "-"
            history += f"{tx['date']} {sign}{tx['amount']}\n"
        
        history += f"\n💰 Итоговый баланс: {data[user_id]['balance']}"
        bot.send_message(chat_id, history)
    except Exception as e:
        print(f"Ошибка в show_history: {e}")
        bot.send_message(chat_id, "❌ Ошибка загрузки истории")

@bot.message_handler(commands=['start', 'help'])
def send_welcome(message):
    help_text = """
    🤖 *Универсальный бот* (лабораторная работа 6)

    *Основные команды:*
    /help - показать это сообщение
    /menu - открыть интерактивное меню
    
    *Финансовый менеджер:*
    /finance - управление финансами
    
    *Я реагирую на:*
    - Текст "привет" или "пока"
    - Фотографии 📷 (попробуйте отправить фото)
    - Файлы 📁
    - Стикеры 😊 (я отвечу текстом)
    """
    bot.send_message(message.chat.id, help_text, parse_mode="Markdown")

@bot.message_handler(commands=['finance'])
def finance_command(message):
    show_finance_menu(message)

@bot.message_handler(commands=['menu'])
def menu_command(message):
    show_main_menu(message)

@bot.message_handler(func=lambda message: message.text and message.text.lower() in ['привет', 'здравствуйте', 'hi', 'hello'])
def greet(message):
    bot.reply_to(message, f"Привет, {message.from_user.first_name}! 😊")

@bot.message_handler(func=lambda message: message.text and message.text.lower() in ['пока', 'до свидания', 'bye', 'бб', 'чау'])
def farewell(message):
    bot.reply_to(message, "До свидания! Хорошего дня! 👋")

@bot.message_handler(content_types=['photo'])
def handle_photo(message):
    bot.reply_to(message, "📸 Отличное фото! Вы можете обработать его через /menu")

@bot.message_handler(content_types=['document'])
def handle_document(message):
    bot.reply_to(message, "📄 Файл получен! Если это изображение, можете обработать его через /menu")

@bot.message_handler(content_types=['sticker'])
def handle_sticker(message):
    try:
        bot.reply_to(message, "😊 Вижу ваш стикер!")
    except Exception as e:
        print(f"Ошибка при обработке стикера: {e}")
        bot.reply_to(message, "Классный стикер! 👍")

def show_main_menu(message):
    markup = types.InlineKeyboardMarkup()
    btn1 = types.InlineKeyboardButton("Обработать текст", callback_data="process_text")
    btn2 = types.InlineKeyboardButton("Обработать изображение", callback_data="process_image")
    btn3 = types.InlineKeyboardButton("Финансовый менеджер", callback_data="finance_menu")
    markup.add(btn1, btn2, btn3)
    bot.send_message(message.chat.id, "🏠 Главное меню:", reply_markup=markup)

@bot.callback_query_handler(func=lambda call: True)
def callback_inline(call):
    try:
        if call.data == "process_text":
            msg = bot.send_message(call.from_user.id, "Введите текст для обработки:")
            bot.register_next_step_handler(msg, process_text)
        elif call.data == "process_image":
            msg = bot.send_message(call.from_user.id, "Отправьте изображение для обработки:")
            bot.register_next_step_handler(msg, process_image)
        elif call.data == "finance_menu":
            show_finance_menu(call) 
        elif call.data in ["sepia", "edges", "invert"]:
            process_image_choice(call)
        elif call.data in ["add_income", "add_expense", "show_balance", "show_history"]:
            handle_finance_callback(call)
    except Exception as e:
        print(f"Ошибка в callback_inline: {e}")
        bot.send_message(call.from_user.id, "❌ Произошла ошибка. Попробуйте снова.")

def process_text(message):
    try:
        if message.content_type != 'text':
            bot.reply_to(message, "❌ Ожидался текст! Попробуйте снова.")
            return
        
        processed = f"🔠 Ваш текст в верхнем регистре:\n\n{message.text.upper()}"
        bot.reply_to(message, processed)
    except Exception as e:
        print(f"Ошибка в process_text: {e}")
        bot.reply_to(message, "❌ Произошла ошибка при обработке текста.")

def process_image(message):
    try:
        if message.content_type not in ['photo', 'document']:
            bot.reply_to(message, "❌ Ожидалось изображение! Попробуйте снова.")
            return
        
        if message.content_type == 'photo':
            file_info = bot.get_file(message.photo[-1].file_id)
        else:
            if not message.document.mime_type.startswith('image/'):
                bot.reply_to(message, "❌ Файл не является изображением!")
                return
            file_info = bot.get_file(message.document.file_id)
        
        downloaded_file = bot.download_file(file_info.file_path)
        nparr = np.frombuffer(downloaded_file, np.uint8)
        img = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
        user_images[message.chat.id] = img
        
        markup = types.InlineKeyboardMarkup()
        btn1 = types.InlineKeyboardButton("Сепия", callback_data="sepia")
        btn2 = types.InlineKeyboardButton("Контуры", callback_data="edges")
        btn3 = types.InlineKeyboardButton("Инверсия", callback_data="invert")
        markup.add(btn1, btn2, btn3)
        
        bot.send_message(message.chat.id, "Выберите тип обработки:", reply_markup=markup)
    except Exception as e:
        print(f"Ошибка в process_image: {e}")
        bot.reply_to(message, "❌ Произошла ошибка при обработке изображения.")

def process_image_choice(call):
    try:
        chat_id = call.message.chat.id
        if chat_id not in user_images:
            bot.send_message(chat_id, "❌ Изображение не найдено. Попробуйте снова.")
            return
        
        img = user_images[chat_id]
        
        if call.data == "sepia":
            kernel = np.array([[0.272, 0.534, 0.131],
                             [0.349, 0.686, 0.168],
                             [0.393, 0.769, 0.189]])
            processed = cv2.transform(img, kernel)
            caption = "🟤 Эффект сепии"
        elif call.data == "edges":
            gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
            edges = cv2.Canny(gray, 100, 200)
            processed = cv2.cvtColor(edges, cv2.COLOR_GRAY2BGR)
            caption = "🔲 Контуры изображения"
        elif call.data == "invert":
            processed = cv2.bitwise_not(img)
            caption = "⚪ Инверсия цветов"
        
        _, img_encoded = cv2.imencode('.jpg', processed)
        img_bytes = io.BytesIO(img_encoded.tobytes())
        bot.send_photo(chat_id, img_bytes, caption=caption)
    except Exception as e:
        print(f"Ошибка в process_image_choice: {e}")
        bot.send_message(call.message.chat.id, "❌ Произошла ошибка при обработке изображения.")

@bot.message_handler(func=lambda message: True)
def handle_unknown(message):
    if message.text and message.text.startswith('/'):
        bot.reply_to(message, "❌ Неизвестная команда. Введите /help для списка команд")
    else:
        bot.reply_to(message, "❌ Я не понимаю это сообщение. Введите /help для справки")

if __name__ == "__main__":
    print("Бот запущен и готов к работе...")
    try:
        bot.polling(none_stop=True, skip_pending=True)
    except Exception as e:
        print(f"Произошла ошибка: {e}")
        print("Попытка перезапуска через 5 секунд...")
        import time
        time.sleep(5)
        bot.polling(none_stop=True, skip_pending=True)