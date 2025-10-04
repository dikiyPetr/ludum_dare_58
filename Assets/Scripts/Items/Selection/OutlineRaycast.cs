using UnityEngine;
using UnityEngine.InputSystem;

public class OutlineRaycast : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float rayDistance = 100f;

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
                // Если это новый объект, отключаем обводку у предыдущего
                if (currentOutline != null && currentOutline != outline)
                {
                    currentOutline.OutlineMode = Outline.Mode.OutlineHidden;
                }

                // Включаем обводку у текущего объекта
                outline.OutlineMode = Outline.Mode.OutlineVisible;
                currentOutline = outline;
                return;
            }
        }
        // Если луч ничего не попал, отключаем обводку у предыдущего объекта
        if (currentOutline != null)
        {
            currentOutline.OutlineMode = Outline.Mode.OutlineHidden;
            currentOutline = null;
        }
    }
}