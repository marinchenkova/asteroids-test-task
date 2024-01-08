# Asteroids 1979 Unity Test Task

Данный проект является примером выполнения тестового задания "Разработка 2D клона оригинальной игры Asteroids" на движке Unity 2022.3 LTS. 

<p align="center">
  <img width="500" alt="image" align="center" src="https://github.com/marinchenkova/asteroids-test-task/assets/22106355/bf51f5bf-a2a0-484c-ac82-45d3c102227d">
</p>

### Содержание

- [Демонстрация игрового процесса](https://github.com/marinchenkova/asteroids-test-task/blob/master/README.md#демонстрация-игрового-процесса)
- [Техническое задание](https://github.com/marinchenkova/asteroids-test-task/blob/master/README.md#техническое-задание)
- [Архитектура проекта](https://github.com/marinchenkova/asteroids-test-task/blob/master/README.md#архитектура-проекта)
- [Пайплайн запуска приложения](https://github.com/marinchenkova/asteroids-test-task/blob/master/README.md#пайплайн-запуска-приложения)

### Демонстрация игрового процесса

https://github.com/marinchenkova/asteroids-test-task/assets/22106355/753bfe9c-f2ac-4d7e-bc8d-f155d144406b

### Техническое задание

Цель игры – получить как можно больше очков, расстреливая астероиды и
летающие тарелки, избегая при этом столкновения с ними.

Игрок управляет космическим кораблём, который может крутиться влево и вправо,
двигаться только вперед и стрелять. Движение корабля должно быть с ускорением и
инерцией. Экран не ограничивает передвижения, а является порталом, т.е. если вы
упираетесь в верхнюю границу, то появитесь с нижней.

У корабля есть два вида оружия:

- Пули при попадании в астероид разбивают его на обломки меньшего размера,
обладающие большейскоростью; попадание пуль в обломки или летающую
тарелку приводит к их уничтожению.
- лазер уничтожает все объекты, которые пересекает. Игрок имеет
ограниченное количество выстрелов лазером. Выстрелы пополняются со
временем.

При столкновении космического корабля с астероидом, обломком или летающей
тарелкойвыводится сообщение о проигрыше со счетом и приглашением начать игру
заново.

После старта игры периодически появляются астероиды и летающие тарелки.
Астероиды двигаются в случайном направлении, а летающие тарелки преследуют
игрока. Астероиды и летающие тарелки между собойне сталкиваются.

Необходимо добавить UI, на котором будут отображаться показатели корабля:
- координаты
- угол поворота
- мгновенная скорость
- число зарядов лазера
- время отката лазера

Детали:
- язык программирования: C#
- разделить логику игры и представление
- классы с логикой не должны наследоваться от MonoBehaviour
- необходимо использовать Assembly Definitions
- для управления использовать Input System
- ассеты для интерфейса и графики можно использовать любые, их качество не
учитывается
- нельзя использовать: сторонние фреймворки, Singleton, preview или
experimental версии Unity пакетов, физику Unity для передвижения объектов.

### Архитектура проекта

В качестве базы для построения игровой логики используется архитектура Entity + Components, реализованная в виде пакета [Assets/Scripts/Entities](https://github.com/marinchenkova/asteroids-test-task/tree/master/Assets/Scripts/Entities) 

- Сущность `Entity` представляет собой readonly-структуру, указатель на список прикрепленных к ней компонентов-классов

```csharp
public readonly struct Entity : IEquatable<Entity> {

    public readonly World world;

    private readonly long _id;

    // ...
}
```

- Компоненты должны быть классами и реализовать интерфейс `IEntityComponent`
- Компоненты принимают сигналы при прикреплении/откреплении, уничтожении сущности для запуска логики 

```csharp
public interface IEntityComponent {

    void OnAttach(Entity entity) {}

    void OnDetach(Entity entity) {}

    void OnDestroy(Entity entity) {}
}
```

- Сущности и компоненты хранятся в созданном мире `World`
- При создании мира в конструктор класса `World` в виде интерфейсов передаются ссылки на сервисы, такие как источник апдейтов, хранилище сущностей, хранилище и фабрика компонентов и т.п. 
- Компоненты через взаимодействие с миром могут подписываться на вызовы Update, создавать вью, добавлять и удалять другие компоненты и сущности

```csharp
public sealed class World {

    public World(
        IEntityIdProvider entityIdProvider,
        IEntityStorage entityStorage,
        IEntityComponentStorage entityComponentStorage,
        IEntityComponentFactory entityComponentFactory,
        IEntityViewProvider entityViewProvider,
        ITickSource tickSource
    ) {
        _entityIdProvider = entityIdProvider;
        _entityStorage = entityStorage;
        _entityComponentStorage = entityComponentStorage;
        _entityComponentFactory = entityComponentFactory;
        _entityViewProvider = entityViewProvider;
        _tickSource = tickSource;
    }

    // ...
}
```

```csharp
public static class EntityExtensions {

    public static T GetComponent<T>(this Entity entity) where T : class, IEntityComponent {
        return entity.world?.GetComponent<T>(entity);
    }

    public static void SetComponent<T>(this Entity entity, T component) where T : class, IEntityComponent {
        entity.world?.SetComponent<T>(entity, component);
    }

    public static void RemoveComponent<T>(this Entity entity) where T : class, IEntityComponent {
        entity.world?.RemoveComponent<T>(entity);
    }

    // ...
}
```

- Для создания сущностей с набором компонентов из редактора был добавлен ScriptableObject класс `EntityPrefab` 
- Компоненты в префабе хранятся с помощью атрибута `[SerializeReference]`, для их редактирования реализован простой инспектор с браузером компонентов

```csharp
public sealed class EntityPrefab : ScriptableObject {

    [SerializeReference] private IEntityComponent[] _components;

    // ...
}
```

<img width="655" alt="image" src="https://github.com/marinchenkova/asteroids-test-task/assets/22106355/c4e345c6-c803-46b7-b6f9-52bd9a02458c">

Для клонирования компонентов, указанных в префабе, используется `UnityEngine.JsonUtility`. Новый экземпляр компонента с нужными данными воссоздается из json-строки компонента в префабе.
Такой подход приводит к образованию мусора в виде json-строк, однако в рамках данного тестового задания было принято решение не тратить много времени на некоторые частные аспекты,
так как техническое задание не предполагает дальнейшего расширения кодовой базы или ее использования в условиях с высокой нагрузкой на ресурсы. 

- Создание вью для сущностей производится через `World`, который вызывает сервис для создания сущностей, использующий пул 

> В проекте не реализованы сервисы для сохранения состояния игры между сессиями. В качестве заглушки используется ScriptableObject, в котором хранится информация о последней сессии.

### Пайплайн запуска приложения 

##### 1. Сцена `Scene_Bootstrap`

Данная сцена является точкой входа в приложение, содержит инициализацию глобальных сервисов и логику запуска стартовой сцены игры.

##### 2. MonoBehaviour `BootstrapLauncher`

При запуске сцены включается `BootstrapLauncher`, который производит настройку и запускает стартовую сцену `Scene_MainMenu`.    

##### 3. Сцена `Scene_MainMenu`

Здесь запускается меню игры, которое управляется с помощью MonoBehaviour `MainMenuUIContoller`.

##### 4. MonoBehaviour `MainMenuUIContoller`

Контроллер запускает геймплейную сцену `Scene_Gameplay` по нажатию кнопки Play.

##### 5. Сцена `Scene_Gameplay`

Здесь запускается игровой процесс, управляемый с помощью MonoBehaviour `GameSessionLauncher`.

##### 6. MonoBehaviour `GameSessionLauncher`

Лаунчер содержит логику для управления игровым процессом для старта/окончания игры, создания мира и наполнения его сущностями, 
а также держит ссылку на сущность с данными по текущему состоянию игры `GameState`, которое хранится в компоненте `GameStateComponent`.

`GameSessionLauncher` внутри создает мир и подписывается на события создания новой сущности, 
что позволяет добавить "предустановленные" компоненты с динамическими даннымм, которые не получится записать заранее.  

##### 7. IEntityComponent `GameStateComponent`

Данный компонент управляет сущностью игрока при старте/окончании игры, содержит логику для записи очков за уничтожение врагов.

Любая сущность получает при создании компонент `GameStateReferenceComponent` со ссылкой на сущность с состоянием игры.
Таким образом обеспечивается работа некоторых компонентов, в частности, для добавления очков: сущность врага при уничтожении вызывает компонент, 
который ожидает что на сущности уже лежит компонент `GameStateReferenceComponent`, и через него получает ссылку на `GameStateComponent`.

За игровым процессом следит MonoBehaviour `GameplayUIController`.

##### 8. MonoBehaviour `GameplayUIController`

Контроллер подписывется на состояние игры через ссылку на `GameSessionLauncher`, который в свою очередь хранит ссылку на сущность с состоянием.
Когда корабль игрока уничтожен, контроллер показывает сообщение "Game Over" и кнопки "Restart" и "Menu".

При нажатии на "Restart" вызывается метод `GameSessionLauncher.RestartGameSession()`, который вызывает очищение мира от старых сущностей и пересоздание игрока и остальных игровых сущностей.
