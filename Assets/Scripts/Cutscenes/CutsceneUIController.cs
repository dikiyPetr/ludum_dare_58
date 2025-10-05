using UnityEngine;
using UnityEngine.UI;

namespace Cutscenes
{
    /// <summary>
    /// Контроллер UI для отображения катсцены
    /// </summary>
    public class CutsceneUIController : MonoBehaviour
    {
        [Header("UI ссылки")]
        [SerializeField] private GameObject cutscenePanel;
        [SerializeField] private Image backgroundImage;

        private void Awake()
        {
            if (cutscenePanel != null)
            {
                cutscenePanel.SetActive(false);
            }
        }

        private void OnEnable()
        {
            CutsceneManager.OnCutsceneStarted += HandleCutsceneStarted;
            CutsceneManager.OnCutsceneEnded += HandleCutsceneEnded;
        }

        private void OnDisable()
        {
            CutsceneManager.OnCutsceneStarted -= HandleCutsceneStarted;
            CutsceneManager.OnCutsceneEnded -= HandleCutsceneEnded;
        }

        private void HandleCutsceneStarted(CutsceneData cutscene)
        {
            if (cutscenePanel != null)
            {
                cutscenePanel.SetActive(true);
            }

            if (backgroundImage != null && cutscene.backgroundImage != null)
            {
                backgroundImage.sprite = cutscene.backgroundImage;
                backgroundImage.enabled = true;
            }
        }

        private void HandleCutsceneEnded(CutsceneData cutscene)
        {
            if (cutscenePanel != null)
            {
                cutscenePanel.SetActive(false);
            }

            if (backgroundImage != null)
            {
                backgroundImage.sprite = null;
                backgroundImage.enabled = false;
            }
        }
    }
}

