# POPs — Учёт контингента студентов

Монорепозиторий на C#: backend (ASP.NET Core Web API) + frontend (Blazor WebAssembly).

Приложение реализовано по диаграмме классов, вариантов использования и активности из лабораторной работы.

## Видеообзор

Короткий обзор системы (вход, дашборд, студенты, группы, курсы, отчёты, роль студента).

<video controls width="100%">
  <source src="docs/Obzor-uchet-kontingenta.mp4" type="video/mp4">
</video>

[Смотреть / скачать MP4](https://github.com/Stremilov/POPs-solo/raw/main/docs/Obzor-uchet-kontingenta.mp4)

## Структура

Решение состоит из трёх проектов: библиотеки общих типов, API и веб-интерфейса. При работе в браузере страница вызывает `ApiClient`, тот отправляет запрос контроллеру API, контроллер обращается к сервису, а сервис работает с SQLite через `AppDbContext`. Модели и DTO для обмена данными лежат в `POPs.Shared`.

### Корень репозитория

| Путь | Назначение |
| --- | --- |
| `POPs.sln` | Файл решения .NET: объединяет проекты `Shared`, `Api` и `Web`. |
| `start.sh` | Запуск на macOS/Linux: при необходимости устанавливает .NET 8, восстанавливает пакеты, запускает API и сайт. |
| `start.ps1`, `start.cmd` | Аналогичный запуск на Windows; `start.cmd` вызывает PowerShell-скрипт. |
| `.gitignore` | Исключает результаты сборки, локальную БД, скачанный SDK и временные файлы. |
| `docs/Obzor-uchet-kontingenta.mp4` | Готовый видеообзор приложения. |
| `tools/demo-video/package.json`, `record.mjs` | Зависимости и сценарий Playwright для записи демонстрационного видео. |

### `src/POPs.Shared` — общие типы

| Путь | Назначение |
| --- | --- |
| `Models/Administrator.cs`, `Student.cs`, `Group.cs`, `Course.cs` | Сущности предметной области и связи между ними; используются в базе данных. |
| `Dtos/AuthDtos.cs` | Данные запроса на вход и ответа с информацией о пользователе. |
| `Dtos/StudentDtos.cs`, `GroupDtos.cs`, `CourseDtos.cs` | Данные для чтения, создания и изменения студентов, групп и курсов. |
| `Dtos/ReportDtos.cs` | Форматы данных для сводки, статистики и отчётов. |
| `POPs.Shared.csproj` | Настройки проекта с типами, доступными API и веб-приложению. |

### `src/POPs.Api` — сервер и база данных

| Путь | Назначение |
| --- | --- |
| `Program.cs` | Точка входа API: регистрирует контроллеры, сервисы, SQLite, CORS и Swagger; создаёт демо-данные при запуске. |
| `Controllers/AuthController.cs` | HTTP-запрос на вход. |
| `Controllers/StudentsController.cs` | HTTP-запросы для поиска, создания, изменения, удаления и отчисления студентов. |
| `Controllers/GroupsController.cs` | HTTP-запросы для групп, кураторов и зачисления студентов. |
| `Controllers/CoursesController.cs` | HTTP-запросы для курсов, привязки групп и перевода на следующий курс. |
| `Controllers/ReportsController.cs` | HTTP-запросы для сводки, статистики и отчётов по группам и курсам. |
| `Services/AuthService.cs` | Проверка логина и пароля, подготовка данных о пользователе. |
| `Services/StudentService.cs`, `GroupService.cs`, `CourseService.cs` | Правила работы со студентами, группами и курсами. |
| `Services/ReportService.cs` | Выборка и расчёт данных для отчётов. |
| `Data/AppDbContext.cs` | Контекст Entity Framework Core: таблицы и связи SQLite. |
| `Data/DbSeeder.cs` | Создание базы и начальных записей для демонстрации. |
| `appsettings.json` | Строка подключения к SQLite и базовые настройки сервера. |
| `appsettings.Development.json` | Настройки журналирования для режима разработки. |
| `Properties/launchSettings.json` | Профили локального запуска API, включая адрес и порт. |
| `POPs.Api.http` | Шаблон HTTP-запроса из заготовки проекта. Указанного в нём маршрута `/weatherforecast/` в текущем API нет. |
| `POPs.Api.csproj` | Зависимости и настройки серверного проекта. |

### `src/POPs.Web` — интерфейс Blazor WebAssembly

| Путь | Назначение |
| --- | --- |
| `Program.cs` | Точка входа веб-приложения: настраивает `HttpClient`, `ApiClient` и состояние входа. |
| `App.razor` | Маршрутизация между страницами и общая компоновка. |
| `Pages/Login.razor`, `Home.razor` | Вход и главная страница со сводкой. |
| `Pages/Students.razor`, `Groups.razor`, `Courses.razor` | Экраны управления студентами, группами и курсами. |
| `Pages/Reports.razor`, `Profile.razor` | Отчёты администратора и личные данные студента. |
| `Services/ApiClient.cs` | Вызовы REST API и обработка ответов сервера. |
| `Services/AuthState.cs` | Текущий пользователь и его роль в памяти браузера. |
| `Layout/MainLayout.razor`, `LoginLayout.razor`, `NavMenu.razor` | Общие макеты и меню; соседние файлы `.razor.css` задают их стили. |
| `_Imports.razor` | Общие пространства имён для компонентов Razor. |
| `wwwroot/index.html` | HTML-оболочка, загружающая Blazor и стили. |
| `wwwroot/appsettings.json` | Адрес API для веб-клиента. |
| `wwwroot/css/app.css`, `wwwroot/css/bootstrap/` | Собственные стили и локальная копия Bootstrap. |
| `wwwroot/favicon.png`, `wwwroot/icon-192.png` | Иконки сайта. |
| `Properties/launchSettings.json` | Профили локального запуска сайта. |
| `POPs.Web.csproj` | Зависимости и настройки веб-проекта. |

## Что внутри

**Сущности:** `Administrator`, `Student`, `Group`, `Course`

**Сервисы (как на диаграмме классов):**
- `StudentService` — добавить / изменить / удалить / найти / по группе / отчислить
- `GroupService` — создать / изменить / удалить / куратор / зачисление
- `CourseService` — создать / изменить / привязать группу / перевод на следующий курс
- `ReportService` — контингент, статистика, отчёт по группе

**Роли:**
- Администратор — студенты, группы, курсы, отчёты
- Студент — вход и редактирование персональных данных

## Утилиты

- **Entity Framework Core + SQLite** — БД без отдельного сервера
- **Swagger** — документация API (`/swagger`)
- **BCrypt** — хэш паролей
- **Blazor WASM** — фронт на C#

## Запуск одной командой

Скрипт сам скачает .NET 8 (если его нет), восстановит пакеты и поднимет API + сайт.

### macOS / Linux

```bash
git clone https://github.com/Stremilov/POPs-solo.git
cd POPs-solo
chmod +x start.sh
./start.sh
```

Если репозиторий уже скачан:

```bash
./start.sh
```

### Windows (PowerShell или двойной клик по `start.cmd`)

```powershell
git clone https://github.com/Stremilov/POPs-solo.git
cd POPs-solo
.\start.cmd
```

или:

```powershell
.\start.ps1
```

Сайт: http://localhost:5081  
Swagger: http://localhost:5080/swagger

### Демо-логины

| Роль            | Логин     | Пароль   |
|-----------------|-----------|----------|
| Администратор   | `admin`   | `admin`  |
| Студент         | `student` | `student`|

## Git / GitHub

Репозиторий: https://github.com/Stremilov/POPs-solo.git
