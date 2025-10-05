# Система катсцен

Простая система для показа катсцен с фоновым изображением и диалогами.

## Компоненты

### CutsceneManager
Основной менеджер системы катсцен. Singleton, DontDestroyOnLoad.

**Настройки:**
- `cutscenes` - список доступных катсцен (ID, фон, ID диалога)
- `fadeInDuration` - длительность осветления
- `fadeOutDuration` - длительность затемнения

**Методы:**
- `StartCutscene(string id)` - запустить катсцену по ID
- `EndCutscene()` - завершить текущую катсцену
- `GetCutscene(string id)` - получить данные катсцены

**События:**
- `OnCutsceneStarted` - вызывается при начале катсцены
- `OnCutsceneEnded` - вызывается при завершении катсцены

### ScreenFader
Управление затемнением экрана. Singleton, DontDestroyOnLoad.

**Методы:**
- `FadeOut(float duration, Action onComplete)` - затемнить экран
- `FadeIn(float duration, Action onComplete)` - осветлить экран
- `SetAlpha(float alpha)` - установить прозрачность напрямую

### CutsceneUIController
Контроллер UI для отображения катсцены.

**Настройки:**
- `cutscenePanel` - панель катсцены
- `backgroundImage` - Image компонент для фона

Автоматически подписывается на события CutsceneManager и показывает/скрывает UI.

### CutsceneDebugger
Отладочный компонент для тестирования катсцен.

**Горячие клавиши:**
- F9, F10 - запуск тестовых катсцен

**Context Menu:**
- Start Test Cutscene 1/2
- End Current Cutscene

### CutsceneInteractable
Запуск катсцены при взаимодействии с объектом (нажатие ЛКМ).

**Настройки:**
- `cutsceneId` - ID катсцены для запуска
- `playerTag` - тег игрока
- `playOnce` - запускать только один раз

**Методы:**
- `ResetPlayedFlag()` - сбросить флаг проигрывания

## Структура данных

### CutsceneData
```csharp
public class CutsceneData
{
    public string id;              // Уникальный ID катсцены
    public Sprite backgroundImage; // Фоновое изображение
    public string dialogId;        // ID связанного диалога
}
```

## Использование

### Быстрая настройка (минимум)

**Обязательные шаги для работы системы:**

1. **Создать ScreenFader** (для затемнения):
   ```
   GameObject → Create Empty → Назвать "ScreenFader"
   Add Component → ScreenFader
   ```
   Canvas создастся автоматически при первом запуске.

2. **Создать CutsceneManager:**
   ```
   GameObject → Create Empty → Назвать "CutsceneManager"
   Add Component → CutsceneManager
   ```
   В инспекторе:
   - Нажать `+` в списке Cutscenes
   - Задать `Id` (например "test_cutscene")
   - Назначить `Background Image` (Sprite)
   - Задать `Dialog Id` (ID из dialogs.json)

3. **Создать UI для отображения фона:**
   - На существующем Canvas создать Panel
   - Назвать "CutscenePanel"
   - Настроить Image: stretch по всему экрану
   - Добавить компонент `CutsceneUIController`
   - Назначить ссылки: `Cutscene Panel` = сам Panel, `Background Image` = Image компонент

4. **Создать интерактивный объект** (опционально):
   ```
   GameObject → 3D Object → Cube
   Add Component → Box Collider (Is Trigger = true)
   Add Component → CutsceneInteractable
   ```
   - Задать `Cutscene Id` = ID из CutsceneManager
   - `Player Tag` = "Player"

### Полная настройка на сцене

1. **Создать CutsceneManager:**
   - GameObject → Create Empty (назвать "CutsceneManager")
   - Добавить компонент CutsceneManager
   - Настроить список катсцен (ID, фон, диалог)
   - Настроить длительность fade (fadeInDuration, fadeOutDuration)

2. **Создать ScreenFader:**
   - GameObject → Create Empty (назвать "ScreenFader")
   - Добавить компонент ScreenFader
   - Fade Duration: 0.5 (или по желанию)
   - Canvas и Image создадутся автоматически при запуске
   - ScreenFader создаст Canvas с sortingOrder = 9999 (поверх всего)

3. **Создать UI для катсцен:**
   - Canvas → UI → Panel (назвать CutscenePanel)
   - Добавить Image компонент для фона
   - Добавить CutsceneUIController на Canvas или отдельный объект
   - Назначить ссылки на panel и background image

4. **Добавить DialogPanel поверх CutscenePanel:**
   - DialogPanel должен быть выше CutscenePanel в иерархии Canvas

### Запуск катсцены из кода

```csharp
if (CutsceneManager.Instance != null)
{
    CutsceneManager.Instance.StartCutscene("intro_cutscene");
}
```

### Интерактивный объект на сцене

Создать объект, который запускает катсцену при нажатии E:

1. **Создать GameObject:**
   - GameObject → 3D Object → Cube (или любой другой)
   - Добавить компонент BoxCollider, отметить Is Trigger
   - Добавить компонент CutsceneInteractable

2. **Настроить CutsceneInteractable:**
   - `Cutscene Id` - ID катсцены для запуска
   - `Player Tag` - тег игрока (по умолчанию "Player")
   - `Play Once` - запускать только один раз

3. **Игрок:**
   - Убедитесь, что объект игрока имеет тег "Player"

При входе игрока в триггер и нажатии E запустится катсцена.

### Триггер на выход

Катсцена автоматически завершается при окончании связанного диалога.
Также можно принудительно завершить:

```csharp
CutsceneManager.Instance.EndCutscene();
```

## Интеграция с пикселизацией

Фон катсцены автоматически использует тот же фильтр пикселизации, что и основная сцена, так как отображается через стандартный Unity UI Image компонент, на который применяется post-processing.

## Последовательность событий

1. `StartCutscene(id)` вызывается
2. Экран затемняется (FadeOut)
3. `OnCutsceneStarted` - UI показывает фон
4. Запускается связанный диалог
5. Экран осветляется (FadeIn)
6. Пользователь проходит диалог
7. Диалог завершается → `HandleDialogEnded`
8. Экран затемняется (FadeOut)
9. `OnCutsceneEnded` - UI скрывает фон
10. Экран осветляется (FadeIn)

## Примечания

- ScreenFader создаёт Canvas автоматически, если не назначен вручную
- CutsceneManager подписан на `DialogManager.OnDialogEnded` для автоматического завершения
- Все менеджеры используют DontDestroyOnLoad
- Fade эффекты настраиваются через инспектор

