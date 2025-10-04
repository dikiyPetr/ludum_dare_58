using UnityEngine;
using TMPro;

public class OverlayInfoManager : MonoBehaviour
{
    [Header("UI Components")] [SerializeField]
    private TextMeshProUGUI textComponent;

    [SerializeField] private GameObject overlayInfoObject;

    private void Start()
    {
        // Скрываем overlay при старте
        if (overlayInfoObject != null)
        {
            overlayInfoObject.SetActive(false);
        }
    }

    /// <summary>
    /// Показывает overlay с информацией об объекте или скрывает его
    /// </summary>
    /// <param name="obj">Объект для отображения. Если null - overlay скрывается</param>
    private void ToggleOverlay(object obj)
    {
        if (overlayInfoObject == null)
        {
            Debug.LogWarning("Overlay object is not assigned!");
            return;
        }

        if (obj == null)
        {
            // Скрываем overlay
            overlayInfoObject.SetActive(false);
            return;
        }

        // Показываем overlay
        overlayInfoObject.SetActive(true);
    }

    public void ShowOverlay(ClueData clue)
    {
        ToggleOverlay(clue);
        // Обновляем текст
        if (textComponent != null && clue !=null)
        {
            textComponent.text = clue.title + "\n" + clue.description;
        }
    }
}