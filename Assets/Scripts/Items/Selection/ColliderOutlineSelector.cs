using UnityEngine;
using UnityEngine.InputSystem;
using Dialogs;
using Tutorial;

/// <summary>
/// Компонент для обнаружения объектов с Outline через триггерный коллайдер камеры
/// Заменяет райкаст-систему OutlineSelector на физические коллизии
/// </summary>
public class ColliderOutlineSelector : MonoBehaviour
{
    [SerializeField] private OverlayInfoManager overlayInfo;
    [SerializeField] private InputActionReference clickAction;
    [SerializeField] private LayerMask interactionLayers = -1; // Слои для взаимодействия (по умолчанию все слои)

    private Outline currentOutline;
    private Collider currentCollider;

    private void Update()
    {
        // Обработка клика для текущего объекта
        if (currentOutline != null && clickAction != null && clickAction.action.WasPressedThisFrame())
        {
            IOutlineInteractable interactable = currentOutline.GetComponent<IOutlineInteractable>();
            if (interactable != null)
            {
                interactable.OnClick();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, принадлежит ли коллайдер нужному слою
        if (!IsInLayerMask(other.gameObject.layer))
            return;

        // Проверяем наличие компонента Outline
        Outline outline = other.GetComponent<Outline>();

        if (outline != null)
        {
            Select(outline, other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Проверяем, принадлежит ли коллайдер нужному слою
        if (!IsInLayerMask(other.gameObject.layer))
            return;

        // Проверяем, изменился ли коллайдер
        if (other != currentCollider)
        {
            Outline outline = other.GetComponent<Outline>();

            if (outline != null)
            {
                Select(outline, other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Если вышли из триггера текущего объекта, отключаем обводку
        if (other == currentCollider)
        {
            Unselect();
        }
    }

    void Select(Outline outline, Collider collider)
    {
        if (currentOutline != outline)
        {
            if (currentOutline != null)
            {
                // Проверяем, не является ли текущий объект активным объектом обучения
                if (!IsGuidanceObject(currentOutline))
                {
                    currentOutline.enabled = false;
                }
            }

            // Проверяем, не является ли новый объект активным объектом обучения
            if (!IsGuidanceObject(outline))
            {
                outline.enabled = true;
                currentOutline = outline;
                currentCollider = collider;
            }
            else
            {
                // Если это объект обучения, не меняем currentOutline
                // но показываем overlay если нужно
                currentOutline = null;
                currentCollider = null;
            }

            // Проверяем наличие IOutlineInteractable
            IOutlineInteractable interactable = outline.GetComponent<IOutlineInteractable>();
            if (interactable != null)
            {
                interactable.ShowOverlayInfo(overlayInfo);
                return;
            }

            overlayInfo.HideOverlay();
        }
    }

    void Unselect()
    {
        if (currentOutline != null)
        {
            // Проверяем, не является ли объект активным объектом обучения
            if (!IsGuidanceObject(currentOutline))
            {
                currentOutline.enabled = false;
            }
            currentOutline = null;
            currentCollider = null;
            overlayInfo.HideOverlay();
        }
    }

    public Outline GetCurrentOutline()
    {
        return currentOutline;
    }

    /// <summary>
    /// Проверяет, принадлежит ли слой к маске взаимодействия
    /// </summary>
    private bool IsInLayerMask(int layer)
    {
        return ((1 << layer) & interactionLayers) != 0;
    }

    /// <summary>
    /// Проверяет, является ли объект активным объектом обучения
    /// </summary>
    private bool IsGuidanceObject(Outline outline)
    {
        if (outline == null) return false;

        // Проверяем, есть ли компонент GuidanceHighlight
        GuidanceHighlight guidanceHighlight = outline.GetComponent<GuidanceHighlight>();

        if (guidanceHighlight == null) return false;

        // Проверяем, активна ли система обучения и является ли этот объект текущим
        if (GuidanceManager.Instance != null && GuidanceManager.Instance.IsGuidanceActive())
        {
            return GuidanceManager.Instance.IsCurrentGuidanceObject(guidanceHighlight) && guidanceHighlight.IsHighlighted();
        }

        return false;
    }
}