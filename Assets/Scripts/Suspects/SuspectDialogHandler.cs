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
        if (option.nextNodeId.StartsWith("_map_result_is_stels"))
        {
            if (option.nextNodeId == "_map_result_is_stels:false_0")
            {
                /// поимка охраны
                OutsideActionManager.Instance.SetPendingCutscene("cutscene_is_stels:false_0");
            }
            else if (option.nextNodeId == "map_result_is_stels:false_0")
            {
                /// осмотр морга
                OutsideActionManager.Instance.SetPendingCutscene("cutscene_is_stels:true_0");
            }
        }
    }
}