# Лабораторная работа № 6 — Monitoring
**Вариант 2 — Кинотеатр**

Проект продолжает лабораторную работу № 5. Сохранены Maven, JUnit, Checkstyle, Logging и Jenkins Pipeline. Добавлены Micrometer, Prometheus и Grafana.

## 1. Требования

- Java 21;
- Maven;
- Docker Desktop с Docker Compose;
- Git (для отправки изменений и запуска Jenkins).

## 2. Сборка и проверка проекта

В корне проекта выполните:

```powershell
mvn clean compile
mvn checkstyle:check
mvn test
mvn verify -DskipTests
```

## 3. Запуск приложения

```powershell
mvn exec:java '-Dexec.mainClass=cinema.Main'
```

Приложение запускает HTTP-сервер на порту 8080 и предоставляет:

- `http://localhost:8080/metrics` — метрики для Prometheus;
- `http://localhost:8080/api/status` — состояние приложения и количество свободных мест;
- `http://localhost:8080/api/sell?seat=5&buyer=John` — покупка билета.

Для успешной проверки откройте ссылку покупки с доступным местом. Повторите запрос с тем же местом — операция завершится ошибкой, а счётчик ошибок увеличится. Для нового успешного сценария используйте другое свободное место.

Примеры:

```text
http://localhost:8080/api/sell?seat=5&buyer=John
http://localhost:8080/api/sell?seat=5&buyer=Peter
http://localhost:8080/api/sell?seat=6&buyer=Anna
```

В демонстрационном приложении данные хранятся в памяти и сбрасываются после перезапуска.

## 4. Метрики приложения

Micrometer регистрирует следующие показатели:

| Метрика Micrometer | Метрика Prometheus | Назначение |
|---|---|---|
| `cinema.ticket.operations` | `cinema_ticket_operations_total` | Все попытки покупки |
| `cinema.ticket.operations.success` | `cinema_ticket_operations_success_total` | Успешные покупки |
| `cinema.ticket.operations.error` | `cinema_ticket_operations_error_total` | Неуспешные покупки |
| `cinema.tickets.sold` | `cinema_tickets_sold_total` | Количество проданных билетов |
| `cinema.ticket.purchase.duration` | `cinema_ticket_purchase_duration_seconds` | Продолжительность покупки |

Счётчики увеличиваются непосредственно при выполнении операции `sellTicket`, а не имеют заранее заданные значения.

## 5. Запуск Prometheus и Grafana

Сначала запустите Java-приложение. Не закрывайте его терминал.

В другом терминале из корня проекта выполните:

```powershell
docker compose up -d
```

Конфигурация Prometheus находится в `prometheus/prometheus.yml`. Prometheus опрашивает `host.docker.internal:8080/metrics` каждые 5 секунд.

Интерфейсы:

- Prometheus: `http://localhost:9090`;
- Grafana: `http://localhost:3000`.

Данные для входа в Grafana:

```text
Логин: admin
Пароль: admin
```

Источник Prometheus и Dashboard приложения добавляются автоматически из файлов конфигурации проекта.

## 6. Проверка Prometheus

Откройте `http://localhost:9090/targets`. Для `cinema-application` должен отображаться статус **UP**.

В разделе Graph выполните запросы:

```promql
cinema_ticket_operations_total
cinema_ticket_operations_success_total
cinema_ticket_operations_error_total
cinema_tickets_sold_total
```

Для просмотра частоты операций:

```promql
rate(cinema_ticket_operations_total[1m])
```

Для средней продолжительности покупки:

```promql
rate(cinema_ticket_purchase_duration_seconds_sum[5m])
/
rate(cinema_ticket_purchase_duration_seconds_count[5m])
```

## 7. Проверка Grafana

Откройте Dashboard `Cinema Application Monitoring`. В нём настроены панели:

- All purchase attempts;
- Successful purchases;
- Purchase errors;
- Tickets sold;
- Purchase operations over time;
- Average purchase duration.

После покупки билета и повторной попытки купить занятое место обновите Dashboard. Значения счётчиков должны измениться.

## 8. Остановка сервисов

В каталоге проекта выполните:

```powershell
docker compose down
```

Чтобы удалить также сохранённые данные Grafana:

```powershell
docker compose down -v
```

## 9. Git и Jenkins

В репозитории должны находиться исходный код, `pom.xml`, `Jenkinsfile`, конфигурации Prometheus и Grafana, `docker-compose.yml` и документация.

```powershell
git add .
git commit -m "Add Prometheus and Grafana monitoring"
git push
```

После отправки изменений проверьте Jenkins Pipeline. Должны успешно выполниться этапы Build, Static Analysis, Test и Package.

## 10. Сравнение Logging и Monitoring

При попытке купить уже занятое место Logging записывает предупреждение с деталями операции. Monitoring увеличивает числовой счётчик `cinema_ticket_operations_error_total`. Журнал помогает выяснить причину конкретной ошибки, а метрика показывает общее количество таких случаев.
