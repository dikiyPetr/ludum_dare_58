using UnityEngine;

namespace Cutscenes
{
    /// <summary>
    /// Отладочный компонент для тестирования катсцен
    /// </summary>
    public class CutsceneDebugger : MonoBehaviour
    {
        [Header("Горячие клавиши")]
        [SerializeField] private KeyCode testKey1 = KeyCode.F9;
        [SerializeField] private KeyCode testKey2 = KeyCode.F10;

        [Header("Тестовые катсцены")]
        [SerializeField] private string testCutsceneId1 = "";
        [SerializeField] private string testCutsceneId2 = "";

        private void Update()
        {
            // if (Input.GetKeyDown(testKey1) && !string.IsNullOrEmpty(testCutsceneId1))
            // {
            //     StartTestCutscene(testCutsceneId1);
            // }

            // if (Input.GetKeyDown(testKey2) && !string.IsNullOrEmpty(testCutsceneId2))
            // {
            //     StartTestCutscene(testCutsceneId2);
            // }
        }

        private void StartTestCutscene(string id)
        {
            if (CutsceneManager.Instance != null)
            {
                Debug.Log($"[CutsceneDebugger] Запуск катсцены: {id}");
                CutsceneManager.Instance.StartCutscene(id);
            }
            else
            {
                Debug.LogError("[CutsceneDebugger] CutsceneManager не найден!");
            }
        }

        [ContextMenu("Start Test Cutscene 1")]
        public void StartTest1()
        {
            StartTestCutscene(testCutsceneId1);
        }

        [ContextMenu("Start Test Cutscene 2")]
        public void StartTest2()
        {
            StartTestCutscene(testCutsceneId2);
        }

        [ContextMenu("End Current Cutscene")]
        public void EndCurrent()
        {
            if (CutsceneManager.Instance != null)
            {
                CutsceneManager.Instance.EndCutscene();
            }
        }
    }
}

