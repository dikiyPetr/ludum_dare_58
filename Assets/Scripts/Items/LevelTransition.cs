using UnityEngine;

/// <summary>
/// Компонент для телепортации игрока в указанную позицию
/// </summary>
public class LevelTransition : MonoBehaviour, IOutlineInteractable
{
    [Header("Настройки телепортации")] [SerializeField]
    private Transform targetTransform;

    /// <summary>
    /// Телепортирует игрока в указанную позицию
    /// </summary>
    public void TransitionToLevel()
    {
        if (targetTransform != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                SimpleCharacterController controller = player.GetComponentInChildren<SimpleCharacterController>();
                controller.Teleport(targetTransform.position, targetTransform.rotation);
            }
        }
    }

    public void ShowOverlayInfo(OverlayInfoManager overlayInfo)
    {
        overlayInfo.HideOverlay();
    }

    public bool OnClick()
    {
        TransitionToLevel();
        return true;
    }
}