# Система диалогов

Модульная система диалогов для Unity с поддержкой ветвления, условий, подсветки текста и интерактивных тултипов.

## Компоненты

### Менеджеры
- **DialogManager** - загрузка диалогов из JSON (StreamingAssets), управление воспроизведением
- **HighlightManager** - фильтрация подсветок по условиям
- **TooltipManager** - позиционирование и отображение тултипов
- **ConditionEvaluator** - статический класс для проверки условий

### UI
- **DialogUIController** - рендер текста и кнопок вариантов ответа
- **DialogUIActivator** - показ/скрытие панели диалога, блокировка управления персонажем
- **TooltipUI** - визуальный компонент тултипа с VerticalLayoutGroup
- **TextHighlighter** - подсветка слов через TMP Rich Text (`<link>`, `<color>`, `<u>`)

### Взаимодействие
- **DialogInteractable** - запуск диалога по триггеру (E / Input System)

### Отладка
- **DialogDebugger** - горячие клавиши F1-F8, Context Menu методы

## Структуры данных

**Dialog:** `id`, `startNodeId`, `speaker`, `nodes[]`  
**DialogNode:** `id`, `text`, `options[]`, `notebookEntries[]`, `highlights[]`  
**DialogOption:** `text`, `nextNodeId`, `condition`  
**DialogHighlight:** `word`, `tooltips[]`  
**Tooltip:** `condition`, `text`  
**ClueConnection:** `id`, `description`, `clueId1`, `clueId2` - связь между уликами

## Условия

### HasEvidence
Проверяет `ClueManager.Instance.HasClue(id)`
```json
{ "type": "HasEvidence", "id": "fingerprints" }
```

### HasConnection
Проверяет `ClueManager.Instance.IsConnectionDiscoveredById(id)` - обнаружена ли связь между уликами
```json
{ "type": "HasConnection", "id": "knife_fingerprints_link" }
```

### MultiCondition
Логика AND/OR
```json
{
  "type": "MultiCondition",
  "logicType": "AND",
  "conditions": [...]
}
```

### NotCondition
Инверсия условия
```json
{
  "type": "NotCondition",
  "condition": { "type": "HasEvidence", "id": "knife" }
}
```

## API

### DialogManager
```csharp
// Управление
StartDialog(string dialogId)
SelectOption(int optionIndex)
EndDialog()

// Получение данных
GetAvailableOptions() → List<DialogOption>
GetDialog(string dialogId) → Dialog
IsDialogLoaded(string dialogId) → bool
LoadDialogs() // перезагрузка JSON

// Свойства
IsInDialog → bool
CurrentDialog → Dialog
CurrentNode → DialogNode

// События
OnDialogStarted(Dialog)
OnDialogEnded(Dialog)
OnNodePlayed(DialogNode)
OnHighlightsUpdated(DialogNode)
OnOptionSelected(Dialog, DialogNode, DialogOption)
```

### HighlightManager
```csharp
RequestTooltips(string word)
HideTooltips()
GetTooltipsForWord(string word) → List<Tooltip>
HasHighlight(string word) → bool
GetHighlightedWords() → List<string>
RefreshHighlights()

// События
OnHighlightsUpdated(DialogNode)
OnTooltipRequested(string, List<Tooltip>)
OnTooltipsHidden()
```

### ConditionEvaluator
```csharp
Evaluate(Condition) → bool
EvaluateAll(List<Condition>) → bool  // AND
EvaluateAny(List<Condition>) → bool  // OR
```

## Настройка сцены

**Минимальная конфигурация:**
1. GameObject с `DialogManager`
2. GameObject с `HighlightManager`
3. Canvas с панелью диалога:
   - `DialogUIController` на панели
   - `TextHighlighter` на TMP_Text (включить Raycast Target!)
   - `DialogUIActivator` с ссылкой на панель
4. GameObject с `TooltipManager` в Canvas
5. `DialogInteractable` на объекте с триггером
6. JSON файл в `StreamingAssets/dialogs.json`

## Формат JSON

```json
{
  "dialogs": [
    {
      "id": "dialog_id",
      "startNodeId": "n1",
      "speaker": "Имя",
      "nodes": [
        {
          "id": "n1",
          "text": "Текст реплики",
          "notebookEntries": [
            { "text": "Запись в блокнот" }
          ],
          "highlights": [
            {
              "word": "нож",
              "tooltips": [
                {
                  "condition": { "type": "HasEvidence", "id": "knife_found" },
                  "text": "Текст тултипа"
                }
              ]
            }
          ],
          "options": [
            {
              "text": "Вариант ответа",
              "nextNodeId": "n2",
              "condition": { "type": "HasEvidence", "id": "clue_id" }
            },
            {
              "text": "Завершить",
              "nextNodeId": null
            }
          ]
        }
      ]
    }
  ]
}
```

## Архитектура

```
DialogManager → (OnDialogStarted) → DialogUIActivator → показать панель
              → (OnNodePlayed) → DialogUIController → отрисовать текст/кнопки
              → (OnNodePlayed) → TextHighlighter → подсветить слова
              → (OnHighlightsUpdated) → HighlightManager → фильтровать тултипы
              
TextHighlighter → (hover) → HighlightManager.RequestTooltips()
                          → (OnTooltipRequested) → TooltipManager → показать TooltipUI
                          
ConditionEvaluator ← HasEvidence → ClueManager.HasClue()
DialogInteractable → (E key) → DialogManager.StartDialog()
```

## Использование

```csharp
// Запуск диалога
DialogManager.Instance.StartDialog("interview_witness");

// Выбор варианта
DialogManager.Instance.SelectOption(0);

// Подписка на события
DialogManager.OnNodePlayed += (node) => {
    foreach (var entry in node.notebookEntries) {
        NotebookSystem.AddEntry(entry.text);
    }
};

DialogManager.OnDialogStarted += (dialog) => {
    Debug.Log($"Начат: {dialog.id}");
};

DialogManager.OnOptionSelected += (dialog, node, option) => {
    Debug.Log($"Выбрана опция: '{option.text}' в диалоге {dialog.id}");
    
    // Пример: выполнить действие в зависимости от выбора
    if (dialog.id == "exit_basement" && option.text == "Да") {
        SceneManager.LoadScene("MainLevel");
    }
};

HighlightManager.OnTooltipRequested += (word, tooltips) => {
    Debug.Log($"Тултип для '{word}'");
};
```

## Интеграция

- **ClueManager** - `HasEvidence` и `HasConnection` условия
- **SimpleCharacterController** - блокировка управления через `DialogUIActivator`
- **Input System** - поддержка в `DialogInteractable`
- **Notebook System** - добавление записей через `OnNodePlayed`

## Примеры использования HasConnection

### Tooltip с условием связи
```json
{
  "word": "нож",
  "tooltips": [
    {
      "condition": { "type": "HasEvidence", "id": "knife" },
      "text": "Нож был найден на месте преступления"
    },
    {
      "condition": { "type": "HasConnection", "id": "knife_fingerprints_link" },
      "text": "На ноже обнаружены отпечатки подозреваемого!"
    }
  ]
}
```

### Вариант ответа с условием связи
```json
{
  "text": "А что насчёт отпечатков на ноже?",
  "nextNodeId": "n5",
  "condition": { "type": "HasConnection", "id": "knife_fingerprints_link" }
}
```

### Составное условие с HasConnection
```json
{
  "type": "MultiCondition",
  "logicType": "AND",
  "conditions": [
    { "type": "HasEvidence", "id": "knife" },
    { "type": "HasEvidence", "id": "fingerprints" },
    { "type": "HasConnection", "id": "knife_fingerprints_link" }
  ]
}
```

## Отладка (DialogDebugger)

**Горячие клавиши:**
- F1 - начать тестовый диалог
- F2/F3 - выбрать вариант 0/1
- F4 - завершить диалог
- F5 - показать все диалоги
- F6 - показать подсветки
- F7 - тултип для "нож"
- F8 - скрыть тултипы
