using UnityEngine;

namespace Cutscenes
{
    /// <summary>
    /// Данные одной катсцены
    /// </summary>
    [System.Serializable]
    public class CutsceneData
    {
        public string id;
        public Sprite backgroundImage;
        public string dialogId;
    }
}

