using UnityEngine;

// UI компонент для отображения подозреваемого
public class SuspectUI : MonoBehaviour
{
    [Header("Gender Objects")]
    [Tooltip("Интерактивный объект подозреваемого-мужчины")]
    [SerializeField] private InteractableSuspect maleObject;

    [Tooltip("Интерактивный объект подозреваемого-женщины")]
    [SerializeField] private InteractableSuspect femaleObject;

    private void Start()
    {
        UpdateVisibility();

        // Подписываемся на события
        if (SuspectManager.Instance != null)
        {
            SuspectManager.Instance.OnSuspectCaught += OnSuspectCaught;
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от событий
        if (SuspectManager.Instance != null)
        {
            SuspectManager.Instance.OnSuspectCaught -= OnSuspectCaught;
        }
    }

    // Обработчик события поимки подозреваемого
    private void OnSuspectCaught(string caughtSuspectId)
    {
        UpdateVisibility();
    }

    // Обновить видимость объектов
    private void UpdateVisibility()
    {
        if (SuspectManager.Instance == null)
        {
            // Если менеджера нет, скрываем оба объекта
            if (maleObject != null) maleObject.gameObject.SetActive(false);
            if (femaleObject != null) femaleObject.gameObject.SetActive(false);
            return;
        }

        // Получаем первого пойманного подозреваемого
        var caughtSuspects = SuspectManager.Instance.GetCaughtSuspects();

        if (caughtSuspects == null || caughtSuspects.Count == 0)
        {
            // Если нет пойманных подозреваемых, скрываем оба объекта
            if (maleObject != null) maleObject.gameObject.SetActive(false);
            if (femaleObject != null) femaleObject.gameObject.SetActive(false);
            return;
        }

        SuspectState caughtSuspect = caughtSuspects[0];

        if (caughtSuspect.data == null)
        {
            if (maleObject != null) maleObject.gameObject.SetActive(false);
            if (femaleObject != null) femaleObject.gameObject.SetActive(false);
            return;
        }

        // Показываем и настраиваем нужный объект в зависимости от пола
        if (maleObject != null)
        {
            if (caughtSuspect.data.isMale)
            {
                maleObject.SetSuspectData(caughtSuspect.data);
                maleObject.gameObject.SetActive(true);
            }
            else
            {
                maleObject.gameObject.SetActive(false);
            }
        }

        if (femaleObject != null)
        {
            if (!caughtSuspect.data.isMale)
            {
                femaleObject.SetSuspectData(caughtSuspect.data);
                femaleObject.gameObject.SetActive(true);
            }
            else
            {
                femaleObject.gameObject.SetActive(false);
            }
        }
    }

    // Обновить отображение (для вызова извне)
    public void Refresh()
    {
        UpdateVisibility();
    }
}