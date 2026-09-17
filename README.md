# NEST

**NEST** - личное приложение для хранения заметок, задач, рецептов и учёта финансов в одном месте.

Я делаю его как pet-project, чтобы на практике разобраться с **C#, ASP.NET Core, Entity Framework Core и Angular**, а также попробовать DDD, CQRS, Clean Architecture и микросервисный подход.

## Что будет в NEST

* **Dashboard** - главная страница с последними изменениями и настраиваемыми блоками.
* **Notes** - заметки.
* **Tasks** - Kanban-доски с задачами, комментариями и файлами.
* **Recipes** - рецепты и ингредиенты.
* **Finance** - расходы, покупки, зарплата, накопления и статистика.
* **Settings** - настройки пользователя, языка и темы.

---

## Tasks

Основной раздел - Kanban-доски с динамическими колонками.

Например:

```text
TODO → В процессе → Review → Done
```

Пользователь сам создаёт и называет колонки.

Основные возможности:

* создание и редактирование досок;
* создание и изменение порядка колонок;
* создание и редактирование задач;
* Drag & Drop;
* приоритеты и дедлайны;
* комментарии;
* вложения;
* удаление и архивирование;
* копирование задач между досками.

При удалении колонки или доски пользователь сам решает, что делать с находящимися внутри задачами.

---

## Архитектура

На первом этапе NEST будет **модульным монолитом**.

Основные Bounded Contexts:

```text
NEST
├── Tasks
├── Notes
├── Recipes
├── Finance
├── Dashboard
└── Identity
```

Для проектирования используются:

* **DDD**
* **Clean Architecture**
* **SOLID**
* **CQRS**

В дальнейшем отдельные Contexts можно будет вынести в микросервисы.

---

## Backend

Стек:

```text
C#
.NET
ASP.NET Core
Entity Framework Core
PostgreSQL
```

Основные слои:

```text
NEST
├── Domain
├── Application
├── Infrastructure
└── API
```

### Domain

Здесь находятся Entity, Aggregate, Value Objects и бизнес-правила.

### Application

Commands, Queries, Handlers, DTO и интерфейсы.

### Infrastructure

EF Core, PostgreSQL, файловое хранилище, RabbitMQ и другие технические детали.

### API

ASP.NET Core Controllers, HTTP, Authentication и Authorization.

---

## CQRS

Изменение данных и получение данных разделены.

**Commands:**

```text
CreateTask
UpdateTask
MoveTask
DeleteTask
AddComment
```

**Queries:**

```text
GetTask
GetTasks
GetBoard
GetDashboard
```

Например:

```text
POST /tasks
    ↓
CreateTaskCommand
    ↓
Handler
    ↓
Domain
    ↓
PostgreSQL
```

и:

```text
GET /tasks/1
    ↓
GetTaskQuery
    ↓
Handler
    ↓
DTO
    ↓
Angular
```

---

## Хранение файлов

Файлы задач не будут храниться непосредственно в PostgreSQL.

В базе будут храниться их метаданные:

```text
FileName
ContentType
Size
StorageKey
```

А сами файлы - во внешнем Object Storage.

Для локальной разработки планируется использовать **MinIO**.

---

## RabbitMQ

RabbitMQ планируется использовать для асинхронного взаимодействия между Contexts и будущими микросервисами.

Например:

```text
Task Completed
      ↓
Event
      ↓
RabbitMQ
      ↓
Другой Context
```

При этом RabbitMQ не будет использоваться там, где обычный синхронный вызов проще.

---

## Frontend

Frontend будет написан на **Angular + TypeScript**.

Планируются:

* Drag & Drop;
* редактируемый Dashboard;
* Authentication;
* Dark / Light Theme;
* RU / EN;
* адаптивный интерфейс.

### Локализация

Поддерживаются:

```text
ru-RU
en-EN
```

Например:

```text
HomePage.Greetings
```

```text
ru-RU → Добрый день, {username}!
en-EN → Good afternoon, {username}!
```

### Темы

Пользователь сможет переключаться между:

```text
Light
Dark
```

Выбранная тема будет сохраняться.

---

## База данных

Основная БД проекта:

**PostgreSQL**

Работа с ней будет выполняться через **Entity Framework Core**.

Планируется использовать:

* Fluent API;
* migrations;
* индексы;
* связи между Entity;
* транзакции;
* оптимизацию запросов;
* проекции в DTO.

---

## Тестирование

Будут использоваться:

* Unit Tests;
* Integration Tests.

В первую очередь тестироваться будут Domain и Application, а затем реальные API-сценарии вместе с базой данных.

---

## Docker

Для локальной разработки инфраструктура будет запускаться через Docker.

Планируемые сервисы:

```text
PostgreSQL
MinIO
RabbitMQ
```

---

## Структура проекта

Предварительно:

```text
NEST
│
├── src
│   ├── NEST.Domain
│   ├── NEST.Application
│   ├── NEST.Infrastructure
│   └── NEST.API
│
├── tests
│   ├── NEST.Domain.Tests
│   ├── NEST.Application.Tests
│   └── NEST.IntegrationTests
│
├── frontend
│   └── nest-client
│
├── docker-compose.yml
└── README.md
```

Структура будет изменяться по мере разработки.

---

## План разработки

```text
1. Архитектура
       ↓
2. Tasks Context
       ↓
3. Backend Tasks
       ↓
4. Angular Tasks
       ↓
5. Notes Context
       ↓
6. Recipes Context
       ↓
7. Finance Context
       ↓
8. Dashboard
       ↓
9. Identity
       ↓
10. RabbitMQ
       ↓
11. Docker
       ↓
12. Микросервисная архитектура
```

Каждый Context сначала проектируется отдельно, затем реализуется.

---

## Цель проекта

NEST - это не просто приложение с заметками и задачами.

Главное для меня - понимать, **почему архитектура устроена именно так**, а не просто собрать проект из готовых решений.
