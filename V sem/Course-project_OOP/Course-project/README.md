# Система бронирования билетов на мероприятия

Desktop приложение на C# для системы бронирования билетов с использованием MVVM паттерна и современных шаблонов проектирования.

## Структура решения

Решение состоит из следующих проектов:

- **TicketBookingSystem.App** - WPF приложение (UI слой)
- **TicketBookingSystem.Models** - Модели данных
- **TicketBookingSystem.ViewModels** - ViewModels для MVVM паттерна
- **TicketBookingSystem.Services** - Бизнес-логика и сервисы
- **TicketBookingSystem.Data** - Слой доступа к данным (Entity Framework)

## Технологии

- .NET 9.0
- WPF для UI
- Entity Framework Core 9.0 с SQLite
- CommunityToolkit.Mvvm для MVVM
- Microsoft.Extensions.DependencyInjection для DI
- BCrypt.Net для хеширования паролей

## Архитектура

### Паттерны проектирования

1. **MVVM (Model-View-ViewModel)** - разделение UI и бизнес-логики
2. **Repository Pattern** - абстракция доступа к данным
3. **Service Pattern** - бизнес-логика в сервисах
4. **Dependency Injection** - инверсия зависимостей

### Модели данных

- **User** - пользователи системы (Admin/Client)
- **Event** - мероприятия
- **Ticket** - билеты
- **Booking** - бронирования

### Основные сервисы

- **IAuthenticationService** - авторизация и регистрация
- **IEventService** - управление мероприятиями
- **IBookingService** - управление бронированиями
- **IAnalyticsService** - аналитика и статистика

## Функционал

### Администратор

- Авторизация в системе
- CRUD операции для мероприятий
- CRUD операции для билетов
- Дашборд аналитики (количество бронирований, популярные мероприятия, выручка)

### Клиент

- Регистрация и авторизация
- Просмотр афиши мероприятий с поиском и фильтрацией
- Бронирование билетов
- История бронирований
- Личный кабинет (смена email, пароля, персональных данных)

## Учетные данные по умолчанию

**Администратор:**
- Email: `admin@gmail.com`
- Пароль: `123456`

**Пользователь:**
- Email: `user@gmail.com`
- Пароль: `123456`

## База данных

База данных SQLite создается автоматически при первом запуске в папке:
`%LocalAppData%\TicketBookingSystem\ticketbooking.db`

## Принципы SOLID

- **S**ingle Responsibility - каждый класс имеет одну ответственность
- **O**pen/Closed - открыт для расширения, закрыт для модификации
- **L**iskov Substitution - интерфейсы и абстракции
- **I**nterface Segregation - разделение интерфейсов
- **D**ependency Inversion - зависимость от абстракций через DI

