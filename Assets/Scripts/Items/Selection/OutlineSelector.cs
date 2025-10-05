using UnityEngine;

public class OutlineSelector : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float rayDistance = 100f;
    [SerializeField] private OverlayInfoManager overlayInfo;

    private Outline currentOutline;

    void Update()
    {
        // Получаем центр экрана
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        // Создаем луч из центра экрана
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        // Выполняем райкаст
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            // Пытаемся получить компонент Outline
            Outline outline = hit.collider.GetComponent<Outline>();

            if (outline != null)
            {
                Select(outline);
                return;
            }
        }

        // Если луч ничего не попал, отключаем обводку у предыдущего объекта
        Unselect();
    }

    void Select(Outline outline)
    {
        if (currentOutline != outline)
        {
            if (currentOutline != null)
            {
                currentOutline.enabled = false;
            }

            outline.enabled = true;
            currentOutline = outline;

            CabinetSlot slot = outline.GetComponentInParent<CabinetSlot>();
            if (slot != null)
            {
                overlayInfo.ShowOverlay(slot.GetClue());
            }
            else
            {
                overlayInfo.ShowOverlay(null);
            }
        }
    }

    void Unselect()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
            overlayInfo.ShowOverlay(null);
        }
    }

    public Outline GetCurrentOutline()
    {
        return currentOutline;
    }
}