using UnityEngine;

/// <summary>
/// Компонент для телепортации игрока в указанную позицию
/// </summary>
public class LevelTransition : MonoBehaviour
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
                player.transform.position = targetTransform.position;
                player.transform.rotation = targetTransform.rotation;
            }
        }
    }
}