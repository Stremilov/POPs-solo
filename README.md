# POPs — учёт контингента студентов

Веб-приложение для работы со студентами, учебными группами и курсами. Администратор ведёт данные и формирует отчёты, студент может посмотреть и изменить свои персональные данные.

[Видеообзор приложения](https://github.com/Stremilov/POPs-solo/raw/main/docs/Obzor-uchet-kontingenta.mp4)

## Возможности

- Поиск, добавление, редактирование и удаление студентов, отчисление с указанием причины.
- Создание групп, назначение кураторов и зачисление студентов.
- Управление курсами, привязка групп и перевод на следующий курс.
- Сводка по контингенту, статистика и отчёт по выбранной группе.
- Отдельные экраны для администратора и студента.

## Стек

- .NET 8, ASP.NET Core Web API
- Blazor WebAssembly, Bootstrap
- Entity Framework Core, SQLite
- BCrypt для хэширования паролей
- Swagger для просмотра и проверки API

## Структура проекта

Решение состоит из пяти проектов:

### `src/POPs.Shared`

Общие модели и типы данных, которыми пользуются сервер и веб-клиент.

- `Models/` — `Administrator`, `Student`, `Group`, `Course` и связи между ними.
- `Dtos/` — запросы и ответы для входа, студентов, групп, курсов и отчётов.

### `src/POPs.Application`

Сценарии работы приложения и контракты доступа к данным.

- `Services/AuthService.cs` — вход администратора и студента.
- `Services/StudentService.cs` — операции со студентами и персональными данными.
- `Services/GroupService.cs` — группы, кураторы и зачисление.
- `Services/CourseService.cs` — курсы, группы на курсах и перевод.
- `Services/ReportService.cs` — сводка, статистика и отчёты.
- `Repositories/I*Repository.cs` — интерфейсы репозиториев для сущностей.
- `Repositories/IUnitOfWork.cs` — контракт сохранения изменений.

### `src/POPs.Infrastructure`

Работа с SQLite через Entity Framework Core.

- `Data/AppDbContext.cs` — таблицы, связи и ограничения базы данных.
- `Data/DbSeeder.cs` — создание базы и начальных данных.
- `Repositories/*Repository.cs` — реализация запросов к базе данных.
- `Repositories/UnitOfWork.cs` — сохранение изменений в общем контексте.

### `src/POPs.Api`

HTTP API приложения.

- `Controllers/` — маршруты для входа, студентов, групп, курсов и отчётов.
- `Program.cs` — настройка сервисов, базы данных, CORS и Swagger.
- `appsettings.json` — строка подключения к SQLite.
- `Properties/launchSettings.json` — профиль локального запуска.

### `src/POPs.Web`

Клиентское приложение на Blazor WebAssembly.

- `Pages/` — вход, главная, студенты, группы, курсы, отчёты и профиль студента.
- `Layout/` — общая компоновка, меню и стили экранов.
- `Services/ApiClient.cs` — запросы к HTTP API.
- `Services/AuthState.cs` — данные текущего пользователя в памяти браузера.
- `wwwroot/css/app.css` — стили интерфейса; `wwwroot/appsettings.json` — адрес API.

В корне находятся `POPs.sln`, скрипты запуска `start.sh`, `start.ps1`, `start.cmd` и видеообзор в `docs/`. Сценарий записи видео лежит в `tools/demo-video/`.

## Как это работает

Веб-клиент отправляет HTTP-запросы в API. Контроллеры вызывают сервисы из `Application`, сервисы обращаются к интерфейсам репозиториев, а реализации из `Infrastructure` работают с SQLite. Зависимости регистрируются в `POPs.Api/Program.cs`. Данные между клиентом и сервером передаются через DTO из `Shared`.

Репозитории отделяют запросы к базе от бизнес-операций. `UnitOfWork` сохраняет изменения через общий `AppDbContext`.

Роли управляют доступными экранами на клиенте. Это демонстрационная авторизация: серверные контроллеры пока не проверяют права пользователя при каждом запросе.

## Запуск

Скрипты при необходимости устанавливают .NET 8, восстанавливают пакеты и запускают API и сайт.

### macOS и Linux

```bash
git clone https://github.com/Stremilov/POPs-solo.git
cd POPs-solo
chmod +x start.sh
./start.sh
```

### Windows

```powershell
git clone https://github.com/Stremilov/POPs-solo.git
cd POPs-solo
.\start.cmd
```

Можно также запустить `start.ps1` из PowerShell.

После запуска сайт доступен на <http://localhost:5081>, Swagger — на <http://localhost:5080/swagger>.

### Демо-доступ

| Роль | Логин | Пароль |
| --- | --- | --- |
| Администратор | `admin` | `admin` |
| Студент | `student` | `student` |
