# Система улик (Clue System)

Модульная система сбора улик, обнаружения связей между ними и отображения в шкафу.

## Компоненты

### Менеджеры
- **ClueManager** - управление уликами, проверка наличия, обнаружение связей
- **ClueCabinet** - отображение собранных улик в шкафу
- **ClueDebugger** - отладочные инструменты для тестирования системы

### Визуализация
- **CabinetSlot** - слот для размещения улики в шкафу
- **ClueConnectionRenderer** - рендеринг линий между связанными уликами

## Структуры данных

**ClueData (ScriptableObject):**
- `id` - уникальный идентификатор улики
- `title` - название улики
- `description` - описание улики
- `cluePrefab` - префаб 3D модели улики
- `cabinetSlotIndex` - индекс слота в шкафу

**ClueConnection:**
- `id` - уникальный идентификатор связи (используется в условиях диалогов)
- `description` - описание связи
- `clueId1` - ID первой улики
- `clueId2` - ID второй улики

**ClueReferencesData (ScriptableObject):**
- `allConnections` - список всех возможных связей между уликами

## API ClueManager

### Управление уликами
```csharp
// Добавить улику
AddClue(string clueId)

// Проверить наличие улики
HasClue(string clueId) → bool

// Проверить наличие всех улик
HasAllClues(params string[] clueIds) → bool

// Проверить наличие хотя бы одной улики
HasAnyClue(params string[] clueIds) → bool

// Получить состояние улики
GetClueState(string clueId) → ClueState

// Получить все собранные улики
GetCollectedClues() → List<ClueState>
```

### Управление связями
```csharp
// Попытка обнаружить связь между уликами
TryDiscoverConnection(string clueId1, string clueId2) → bool

// Попытка обнаружить связь по ID
TryDiscoverConnectionById(string connectionId) → bool

// Проверить, обнаружена ли связь между уликами
IsConnectionDiscovered(string clueId1, string clueId2) → bool

// Проверить, обнаружена ли связь по ID
IsConnectionDiscoveredById(string connectionId) → bool

// Получить все обнаруженные связи
GetDiscoveredConnections() → List<ClueConnection>
```

### События
```csharp
OnClueCollected(string clueId)
OnConnectionDiscovered(string clueId1, string clueId2)
```

## API ClueCabinet

```csharp
// Отобразить улику в шкафу
DisplayClue(ClueData clueData)

// Получить слот по индексу
GetSlotByIndex(int index) → CabinetSlot

// Обновить отображение всех улик
RefreshAllClues()

// Обновить отображение всех связей
RefreshAllConnections()

// Очистить все слоты
ClearAllSlots()
```

## API ClueDebugger

### Методы
```csharp
// Выдать все стартовые улики
GiveAllStartingClues()

// Выдать конкретную улику
GiveClue(ClueData clue)
GiveClueById(string clueId)

// Очистить все улики
ClearAllClues()

// Вывести список собранных улик
PrintCollectedClues()

// Проверить наличие улики
CheckClue(string clueId)

// Обнаружить связь
DiscoverConnection(string clueId1, string clueId2)
DiscoverConnectionById(string connectionId)

// Проверить статус связи
CheckConnectionStatus(string clueId1, string clueId2)
CheckConnectionStatusById(string connectionId)

// Вывести все обнаруженные связи
PrintDiscoveredConnections()

// Обнаружить все связи из ClueReferencesData
DiscoverAllConnectionsFromReferences()
```

### Горячие клавиши (по умолчанию)
- **F1** - выдать все стартовые улики
- **F2** - очистить все улики
- **F3** - вывести список собранных улик
- **F4** - обнаружить все связи из ClueReferencesData

## Настройка в Unity

### 1. ClueManager
1. Создать GameObject с компонентом `ClueManager`
2. Создать ScriptableObjects типа `ClueData` для каждой улики
3. Создать ScriptableObject типа `ClueReferencesData` со всеми связями
4. Назначить ссылки в инспекторе ClueManager

### 2. ClueCabinet
1. Создать GameObject с компонентом `ClueCabinet`
2. Создать дочерние GameObject со слотами (`CabinetSlot`)
3. Создать GameObject с компонентом `ClueConnectionRenderer`
4. Назначить ссылки в инспекторе ClueCabinet

### 3. ClueDebugger (опционально)
1. Создать GameObject с компонентом `ClueDebugger`
2. Добавить стартовые улики для автовыдачи
3. Настроить горячие клавиши

## Пример использования

### Создание улики (ClueData)
```
Создать: Assets → Create → Game → Clue

Заполнить:
- id: "knife"
- title: "Нож"
- description: "Окровавленный нож с места преступления"
- cluePrefab: [префаб 3D модели]
- cabinetSlotIndex: 0
```

### Создание связей (ClueReferencesData)
```
Создать: Assets → Create → Game → Clue References

Добавить связи:
- id: "knife_fingerprints_link"
  description: "Отпечатки на ноже совпадают с подозреваемым"
  clueId1: "knife"
  clueId2: "fingerprints"

- id: "suspect_motive_link"
  description: "У подозреваемого был мотив"
  clueId1: "suspect_profile"
  clueId2: "victim_diary"
```

### Код
```csharp
// Добавить улику
ClueManager.Instance.AddClue("knife");

// Проверить наличие
if (ClueManager.Instance.HasClue("knife"))
{
    Debug.Log("Нож найден!");
}

// Обнаружить связь
bool discovered = ClueManager.Instance.TryDiscoverConnection("knife", "fingerprints");

// Обнаружить связь по ID
bool discovered = ClueManager.Instance.TryDiscoverConnectionById("knife_fingerprints_link");

// Подписка на события
ClueManager.Instance.OnClueCollected += (clueId) => {
    Debug.Log($"Собрана улика: {clueId}");
};

ClueManager.Instance.OnConnectionDiscovered += (id1, id2) => {
    Debug.Log($"Обнаружена связь: {id1} <-> {id2}");
};
```

## Интеграция с системой диалогов

Система улик полностью интегрирована с системой диалогов через условия:

### HasEvidence - проверка наличия улики
```json
{
  "type": "HasEvidence",
  "id": "knife"
}
```

### HasConnection - проверка обнаруженной связи
```json
{
  "type": "HasConnection",
  "id": "knife_fingerprints_link"
}
```

### Пример в диалоге
```json
{
  "id": "n1",
  "text": "Что вы можете сказать о ноже?",
  "highlights": [
    {
      "word": "ноже",
      "tooltips": [
        {
          "condition": { "type": "HasEvidence", "id": "knife" },
          "text": "Нож найден на месте преступления"
        },
        {
          "condition": { "type": "HasConnection", "id": "knife_fingerprints_link" },
          "text": "На ноже обнаружены отпечатки подозреваемого!"
        }
      ]
    }
  ],
  "options": [
    {
      "text": "Показать результаты экспертизы",
      "nextNodeId": "n2",
      "condition": { "type": "HasConnection", "id": "knife_fingerprints_link" }
    }
  ]
}
```

## Архитектура

```
ClueManager (Singleton, DontDestroyOnLoad)
  ├─ Хранит состояние улик (собрана/не собрана)
  ├─ Хранит обнаруженные связи
  ├─ Ссылается на ClueReferencesData (все возможные связи)
  └─ События: OnClueCollected, OnConnectionDiscovered

ClueCabinet (Singleton)
  ├─ Слушает события ClueManager
  ├─ Управляет слотами (CabinetSlot)
  ├─ Управляет визуализацией связей (ClueConnectionRenderer)
  └─ Обновляет отображение при сборе улик/обнаружении связей

CabinetSlot
  ├─ Хранит текущую улику
  └─ Инстанцирует префаб улики

ClueConnectionRenderer
  ├─ Создаёт LineRenderer для связей
  └─ Управляет отображением линий между уликами

ClueDebugger
  └─ Инструменты для тестирования и отладки
```

## Поток данных

```
1. Игрок собирает улику
   → ClueManager.AddClue(id)
   → OnClueCollected(id)
   → ClueCabinet.DisplayClue(clueData)

2. Игрок обнаруживает связь
   → ClueManager.TryDiscoverConnection(id1, id2)
   или ClueManager.TryDiscoverConnectionById(connectionId)
   → OnConnectionDiscovered(id1, id2)
   → ClueCabinet создаёт линию через ClueConnectionRenderer

3. Проверка в диалоге
   → HasConnection.Evaluate()
   → ClueManager.IsConnectionDiscoveredById(id)
   → Показ тултипа или варианта ответа
```

## Отладка

### Включение отладочных сообщений
Все основные действия логируются в консоль:
- Сбор улик (зелёный)
- Обнаружение связей (зелёный)
- Ошибки (красный)
- Предупреждения (жёлтый)

### Использование ClueDebugger
1. Добавьте компонент на сцену
2. Заполните список `startingClues` для автовыдачи
3. Используйте горячие клавиши или Context Menu
4. Проверяйте результаты в консоли

### Типичные проблемы
- **Улика не добавляется**: проверьте, что ID совпадает в ClueData и коде
- **Связь не обнаруживается**: проверьте, что обе улики собраны и связь существует в ClueReferencesData
- **Линия не отображается**: проверьте, что ClueConnectionRenderer назначен в ClueCabinet
- **HasConnection всегда false**: убедитесь, что `id` связи совпадает в ClueReferencesData и JSON диалога

