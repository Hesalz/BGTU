# Лабораторная работа № 4 — Git + Jenkins
**Вариант 2 — Кинотеатр**

## 1. Описание проекта
Проект основан на лабораторной работе № 3. Добавлен JUnit-тест `MovieTest.shouldReturnCorrectMovieInformation`, проверяющий данные фильма и его текстовое представление.
В проекте используются `pom.xml`, `Jenkinsfile` и `.gitignore`. Последний исключает каталог `target/` и служебные файлы IntelliJ IDEA.

## 2. Проверка проекта
В PowerShell из корневого каталога проекта выполните:

```powershell
mvn clean
mvn compile
mvn test
```

Убедитесь, что тесты проходят успешно.

## 3. Настройка Git
Если репозиторий ещё не создан:

```powershell
git init
git branch -M main
git add .
git commit -m "Initial project version"
git remote add origin https://github.com/USERNAME/REPOSITORY.git
git push -u origin main
```

Замените `USERNAME/REPOSITORY` адресом своего репозитория.
Если origin ещё не настроен, Откройте свой репозиторий на GitHub, нажмите Code → Local → HTTPS и скопируйте ссылку.
Если `origin` уже настроен, повторно добавлять его не нужно. Для лабораторной необходимо минимум два коммита.

## 4. Настройка Jenkins (http://localhost:8080)
1. Создайте задание типа **Pipeline** или откройте существующее.
2. В разделе **Pipeline** выберите `Pipeline script from SCM`.
3. В поле **SCM** выберите `Git`.
4. Укажите URL репозитория, ветку `*/main` и путь к файлу `Jenkinsfile`.
5. Сохраните настройки и нажмите **Build Now** для первой проверки.

Для приватного репозитория добавьте Credentials в Jenkins: **Manage Jenkins → Credentials → Add Credentials**. Выберите `Username with password`, укажите имя пользователя GitHub и Personal Access Token вместо пароля, который можно создать тут https://github.com/settings/tokens. Затем выберите эти Credentials в настройках Pipeline. Для публичного репозитория доступ на чтение обычно возможен без авторизации.

## 5. Автоматический запуск
В настройках задания найдите раздел **Triggers** и включите **«Опрашивать SCM об изменениях» (Poll SCM)**.
В поле расписания укажите:

```text
* * * * *
```

Jenkins будет проверять изменения каждую минуту и запускать сборку при обнаружении нового коммита. 
Webhook для этого способа не требуется.
Чтобы проверить работу триггера, измените файл проекта и выполните:

```powershell
git add .
git commit -m "Update"
git push
```

Не нажимайте **Build Now** после отправки изменений. Дождитесь автоматического запуска и проверьте результат в **Console Output**.

## 6. Проверка ошибки и восстановление
Для демонстрации ошибки временно измените ожидаемое значение в `MovieTest.java`, чтобы тест завершился неуспешно.

```powershell
git add .
git commit -m "Demonstrate test failure"
git push
```

Дождитесь автоматической сборки. Этап `Test` должен завершиться ошибкой, а `Package` — быть пропущен, если это предусмотрено `Jenkinsfile`.
Затем восстановите правильное значение и отправьте исправление:

```powershell
git add .
git commit -m "Fix movie test"
git push
```

Убедитесь, что следующая автоматическая сборка завершилась со статусом `Finished: SUCCESS`.