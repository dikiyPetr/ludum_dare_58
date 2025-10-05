using UnityEngine;
using UnityEngine.InputSystem;
using Dialogs;

public class OutlineSelector : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float rayDistance = 100f;
    [SerializeField] private OverlayInfoManager overlayInfo;
    [SerializeField] private InputActionReference clickAction;

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

                // Проверяем наличие DialogInteractable
                DialogInteractable dialogInteractable = hit.collider.GetComponent<DialogInteractable>();
                if (dialogInteractable == null)
                {
                    dialogInteractable = hit.collider.GetComponentInParent<DialogInteractable>();
                }

                // Проверяем наличие LevelTransition
                LevelTransition levelTransition = hit.collider.GetComponent<LevelTransition>();
                if (levelTransition == null)
                {
                    levelTransition = hit.collider.GetComponentInParent<LevelTransition>();
                }

                // Проверяем наличие InteractableSuspect
                InteractableSuspect suspect = hit.collider.GetComponent<InteractableSuspect>();
                if (suspect == null)
                {
                    suspect = hit.collider.GetComponentInParent<InteractableSuspect>();
                }

                // Проверяем наличие SuspectVisualDisplay
                SuspectVisualDisplay suspectDisplay = hit.collider.GetComponent<SuspectVisualDisplay>();
                if (suspectDisplay == null)
                {
                    suspectDisplay = hit.collider.GetComponentInParent<SuspectVisualDisplay>();
                }

                // Обработка клика
                if (clickAction != null && clickAction.action.WasPressedThisFrame())
                {
                    // Приоритет: сначала LevelTransition, потом InteractableSuspect, потом SuspectVisualDisplay, потом DialogInteractable
                    if (levelTransition != null)
                    {
                        levelTransition.TransitionToLevel();
                    }
                    else if (suspect != null)
                    {
                        SuspectData suspectData = suspect.GetSuspectData();
                        if (suspectData != null && !string.IsNullOrEmpty(suspectData.suspectDialogNodeId))
                        {
                            if (DialogManager.Instance != null && !DialogManager.Instance.IsInDialog)
                            {
                                DialogManager.Instance.StartDialog(suspectData.suspectDialogNodeId);
                            }
                        }
                    }
                    else if (suspectDisplay != null)
                    {
                        SuspectData suspectData = suspectDisplay.GetSuspectData();
                        if (suspectData != null && !string.IsNullOrEmpty(suspectData.mapDialogNodeId))
                        {
                            if (DialogManager.Instance != null && !DialogManager.Instance.IsInDialog)
                            {
                                DialogManager.Instance.StartDialog(suspectData.mapDialogNodeId);
                            }
                        }
                    }
                    else if (dialogInteractable != null)
                    {
                        if (DialogManager.Instance != null && !DialogManager.Instance.IsInDialog)
                        {
                            // Используем рефлексию для вызова приватного метода TryStartDialog
                            var method = dialogInteractable.GetType().GetMethod("TryStartDialog",
                                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            if (method != null)
                            {
                                method.Invoke(dialogInteractable, null);
                            }
                        }
                    }
                }

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

            // Проверяем наличие CabinetSlot
            CabinetSlot slot = outline.GetComponentInParent<CabinetSlot>();
            if (slot != null)
            {
                overlayInfo.ShowClueOverlay(slot.GetClue());
                return;
            }

            // Проверяем наличие SuspectVisualDisplay
            SuspectVisualDisplay suspectDisplay = outline.GetComponent<SuspectVisualDisplay>();
            if (suspectDisplay != null)
            {
                SuspectData suspectData = suspectDisplay.GetSuspectData();
                if (suspectData != null)
                {
                    overlayInfo.ShowSuspectOverlay(suspectData);
                    return;
                }
            }

            // Проверяем наличие InteractableSuspect
            InteractableSuspect interactableSuspect = outline.GetComponent<InteractableSuspect>();
            if (interactableSuspect == null)
            {
                interactableSuspect = outline.GetComponentInParent<InteractableSuspect>();
            }

            if (interactableSuspect != null)
            {
                SuspectData suspectData = interactableSuspect.GetSuspectData();
                if (suspectData != null)
                {
                    overlayInfo.ShowSuspectOverlay(suspectData);
                    return;
                }
            }

            overlayInfo.HideOverlay();
        }
    }

    void Unselect()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
            overlayInfo.HideOverlay();
        }
    }

    public Outline GetCurrentOutline()
    {
        return currentOutline;
    }
}