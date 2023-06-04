## Стажировка C#-разработчик

#### Данные
База данных проекта Sqlite файл базы \WebBlog\Data\WebBlogDatabase.db 
Модель сознания БД с нуля \WebBlog.DAL\Migrations\20230602094411_InitialCreate.cs

#### Пользователи 
| Пользователь  | Пароль | Роль | Id
| ------------- | ------------- | ------- | ---- |
| adm@a.aa  | 111111  | Administrator | b6cacd9f-641e-493c-bb78-5893dc543de4
| mod@m.mm  | 111111  | Moderator     | 4aac6ba0-19cf-4470-978d-e6807470f3e6
| usr@u.uu  | 111111  | User          | 3384dc49-b1a8-4174-95cf-33dde7da4ddd

#### Роли
| Роль  |   Id  | Описание |
| ------------- | ------------- | -----|
| Administrator  | 489905d4-74cf-4151-ad5d-86d568ff795e | Доступны все функции |
| User  | b13f7f75-85b3-4d4d-8424-926118d9d6ad | Доступно только создание  статей , тегов, коментариев |
| Moderator  | 39716c38-6144-4cfd-9307-25fa92b07391 | не доступно управление пользователями и ролями |




### Задача
 В рамках стажировки на базе шаблона  ASP.NET MVC для ASP.NET Core 7.0 будет разработан блог с следующим функционалом:
 
* Регистрация пользователей
* Создание статьи
* Создание и редактирование статьи
* Теги статей
* Поиск по тегам и тексту
* Комментирование статей

### Этап 1. Проектирование и разработка бэка

#### База данных
В качестве бд выбрана Sqlite. Модель проекта представлена следующими типами сущностей:


| Таблица  | Описание |
| ------------- | ------------- |
| Articles  |  Представляет статью  |
| ArticleTag  | Связывает  статьи с тегами  |
| Tags | Представляет тег |
| Comments | Представляет комментарий |


Сушность пользователя в проекте представлена моделью Identity ASP.NET Core

| Таблица  | Описание |
| ------------- | ------------- |
| AspNetUsers| представляет пользователя|
| AspNetUserRoles | Сущность соединения, которая связывает пользователей и роли |
| AspNetRoleClaims | Представляет утверждение, которое предоставляется всем пользователям в роли|
| AspNetRoles |Представляет роль|
| AspNetUserClaims | Представляет утверждение о том, что пользователь обладает |
| AspNetUserLogins | Связывает пользователя с именем входа |
| AspNetUserTokens | Представляет маркер проверки подлинности для пользователя |

#### Многоуровневая архитектура,  и бизнес-модели
Многоуровневая архитектура в проекте представлена следующими уровнями: 

| Таблица  | Описание |
| ------------- | ------------- |
| Уровень представления| Представлен приложением  .exe|
| Уровень логики | Представлен библиотекой .BLL.dll|
| Уровень данных | Представлен библиотекой .DAL.dll|

#### ORM
 В качестве объектно-ориентированной технологии доступа к данным, выбран Entity Framework Core для ASP.NET Core от Microsoft.

#### Бизнес-модели
| Класс  | Описание |
| ------------- | ------------- |
| Article  |  Представляет статью  |
| ArticleTag  | Связывает  статьи с тегами  |
| Tag | Представляет тег |
| Comment | Представляет комментарий |
| BlogUser| представляет пользователя|

####CRUD-операции для бизнес-моделей
| Класс реализации | Интерфейс | Описание |
| ------------- | ------------- | ----|
| ArticleRespository  | IArticleRespository | CRUD-операции для модели Article |
| TagRespository | ITagRespository | CRUD-операции для модели Tag |
| CommentRespository | ICommentRespository | CRUD-операции для модели Comment |

CRUD-операции для моделеи BlogUser реализованы в классе Microsoft.AspNetCore.Identity.UserManager


