using UnityEngine;
using Dialogs;

public class SuspectDialogHandler : MonoBehaviour
{
    public static SuspectDialogHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Подписываемся на событие выбора опции в диалоге
        DialogManager.OnOptionSelected += HandleSuspectDialogs;
    }

    private void OnDestroy()
    {
        // Отписываемся от события
        DialogManager.OnOptionSelected -= HandleSuspectDialogs;
    }

    /// <summary>
    /// Обработчик кастомных диалогов, связанных с подозреваемыми
    /// </summary>
    private void HandleSuspectDialogs(Dialog dialog, DialogNode node, DialogOption option)
    {
        if (SuspectManager.Instance == null) return;
        if (option.nextNodeId == null) return;
        if (option.nextNodeId.StartsWith("_"))
        {
            if (option.nextNodeId.StartsWith("_map_result_is_stels"))
            {
                switch (option.nextNodeId)
                {
                    case "_map_result_is_stels:false_0":
                        /// поимка охраны
                        OutsideActionManager.Instance.SetPendingCutscene("cutscene_is_stels:false_0");
                        break;
                    case "_map_result_is_stels:true_0":
                        /// осмотр морга
                        OutsideActionManager.Instance.SetPendingCutscene("cutscene_is_stels:true_0");
                        break;
                }
            }

            if (option.nextNodeId.StartsWith("_outside_result"))
            {
                switch (option.nextNodeId)
                {
                    case "_outside_result_is_stels:false_0":
                        /// поимка заведующего моргом
                        SuspectManager.Instance.CatchSuspect("0");
                        break;
                    case "_outside_result_is_stels:true_0":
                        /// осмотр морга
                        ClueManager.Instance.AddClue("2");
                        break;
                }
            }

            if (option.nextNodeId.StartsWith("_dialogue"))
            {
                switch (option.nextNodeId)
                {
                    case "_dialogue_0:clue_3":
                        /// информация о компании x
                        ClueManager.Instance.AddClue("3");
                        break;
                    case "_dialogue_0:release":
                        /// отпустил зав морга
                        SuspectManager.Instance.ReleaseSuspect("0");
                        break;
                    case "_dialogue_1:release":
                        /// отпустил зам компании
                        SuspectManager.Instance.ReleaseSuspect("1");
                        break;
                    case "_dialogue_1:clue_5":
                        /// павел z
                        ClueManager.Instance.AddClue("5");
                        break;
                    case "_dialogue_2:release":
                        /// отпустил глав врача
                        SuspectManager.Instance.ReleaseSuspect("2");
                        break;
                    case "_dialogue_2:clue_8":
                        /// тел больше нет
                        ClueManager.Instance.AddClue("8");
                        break;
                }
            }

            if (option.nextNodeId.StartsWith("_news_"))
            {
                switch (option.nextNodeId)
                {
                    case "_news_1_end":
                        /// новости первого дня. не выдают тело
                        ClueManager.Instance.AddClue("1");
                        break;
                    case "_news_2_end":
                        /// новости второго дня. зам директор компани и x
                        ClueManager.Instance.AddClue("4");
                        break;
                    case "_news_3_end":
                        /// Знакомый голос из рекламы
                        ClueManager.Instance.AddClue("7");
                        break;
                }
            }

            switch (option.nextNodeId)
            {
                case "_sleep":
                    /// пропуск дня
                    DayManager.Instance.SkipDay();
                    break;
            }
        }
    }
}