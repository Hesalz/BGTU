from collections import Counter
from datetime import datetime

#1
def word_counter(text):
    words = text.lower().split()
    word_count = Counter(words)
    return dict(word_count)

# 2
logs = [
    ("192.168.1.1", "200 OK", 1543),
    ("192.168.1.2", "404 Not Found", 234),
    ("192.168.1.1", "500 Internal Server Error", 542),
    ("192.168.1.3", "200 OK", 876),
    ("192.168.1.2", "200 OK", 1324),
]

def logi(logs):
    ip_count = Counter(log[0] for log in logs)
    status_count = Counter(log[1] for log in logs)
    total_data = sum(log[2] for log in logs)
    most_common_status = status_count.most_common(1)[0]
    return ip_count, most_common_status, total_data

# 3
service_a = {"Анна", "Иван", "Мария", "Сергей", "Алексей"}
service_b = {"Мария", "Иван", "Дмитрий", "Ольга", "Светлана"}
service_c = {"Сергей", "Ольга", "Александр", "Иван", "Анна"}

def analyze_users(service_a, service_b, service_c):
    all_three = service_a & service_b & service_c
    only_one = (service_a ^ service_b ^ service_c) - (service_a & service_b) - (service_b & service_c) - (service_a & service_c)
    unique_counts = {
        'Service A': len(service_a - (service_b | service_c)),
        'Service B': len(service_b - (service_a | service_c)),
        'Service C': len(service_c - (service_a | service_b)),
    }
    largest_unique = max(unique_counts, key=unique_counts.get)
    return all_three, only_one, largest_unique

# 4

tasks = {
    "Task 1": "2024-02-10",
    "Task 2": "2024-02-20",
    "Task 3": "2024-02-22",
    "Task 4": "2024-02-25",
    "Task 5": "2024-02-27",
    "Task 6": "2024-02-05",
    "Task 7": "2024-02-18",
    "Task 8": "2024-02-19",
    "Task 9": "2024-02-29",
    "Task 10": "2024-03-01",
}

def tasks_admin(tasks):
    today = datetime.today()
    expired_tasks = [task for task, date in tasks.items() if datetime.strptime(date, "%Y-%m-%d") < today]
    upcoming_tasks = [task for task, date in tasks.items() if 0 <= (datetime.strptime(date, "%Y-%m-%d") - today).days < 3]
    return expired_tasks, upcoming_tasks

def add_task(tasks, name, date):
    try:
        task_date = datetime.strptime(date, "%Y-%m-%d")
        if name in tasks:
            return "Ошибка: задача с таким названием уже существует."
        tasks[name] = date
        return "Задача добавлена."
    except ValueError:
        return "Ошибка: некорректный формат даты."



if __name__ == "__main__":
    text = "Проверка задания задания первого с подсчётом количества количества количества слов слов слов слов"
    print("Частота слов:", word_counter(text))
    
    ip_count, most_common_status, total_data = logi(logs)
    print("Количество запросов от IP:", ip_count)
    print("Самый частый HTTP-статус:", most_common_status)
    print("Общий объем переданных данных:", total_data)
    
    all_three, only_one, largest_unique = analyze_users(service_a, service_b, service_c)
    print("Пользователи во всех трех сервисах:", all_three)
    print("Пользователи только в одном сервисе:", only_one)
    print("Сервис с наибольшей уникальной базой пользователей:", largest_unique)
    
    expired_tasks, upcoming_tasks = tasks_admin(tasks)
    print("Задачи с истекшим дедлайном:", expired_tasks)
    print("Задачи с дедлайном менее 3 дней:", upcoming_tasks)
    print(add_task(tasks, "Task 11", "2024-03-05"))
    print(add_task(tasks, "Task 1", "2024-03-06"))
    print(add_task(tasks, "Task 12", "2024-02-30"))