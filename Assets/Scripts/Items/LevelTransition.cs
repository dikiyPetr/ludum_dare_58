using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Компонент для перехода между уровнями при клике на объект с Outline
/// </summary>
public class LevelTransition : MonoBehaviour
{
    [Header("Настройки перехода")] [SerializeField]
    private int targetSceneIndex = -1;

    /// <summary>
    /// Выполняет переход на указанный уровень
    /// </summary>
    public void TransitionToLevel()
    {
        if (targetSceneIndex >= 0)
        {
            SceneManager.LoadScene(targetSceneIndex);
        }
    }
    
}