using Cutscenes;
using System.Collections;
using UnityEngine;

public class SuspectEliminationTrigger : MonoBehaviour, IOutlineInteractable
{
    [SerializeField] private AudioClip eliminationSound;
    [SerializeField] private AudioSource audioSource;

    public void EliminateSuspect()
    {
        // Проверяем, есть ли пойманный подозреваемый
        SuspectState caughtSuspect = SuspectManager.Instance.GetCaughtSuspect();
        if (caughtSuspect == null)
        {
            Debug.LogWarning("Нет пойманного подозреваемого для устранения!");
            return;
        }

        CutsceneManager.Instance.StartCutscene("eliminate");

        // Стартуем звук
        if (eliminationSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(eliminationSound);
            StartCoroutine(WaitForSoundAndEndCutscene(caughtSuspect.id));
        }
        else
        {
            // Устраняем подозреваемого
            SuspectManager.Instance.EliminateSuspect(caughtSuspect.id);
            CutsceneManager.Instance.EndCutscene();
        }
    }

    private IEnumerator WaitForSoundAndEndCutscene(string suspectId)
    {
        // Ждем длительность звука
        yield return new WaitForSeconds(eliminationSound.length);

        // Устраняем подозреваемого
        SuspectManager.Instance.EliminateSuspect(suspectId);

        // Завершаем катсцену
        CutsceneManager.Instance.EndCutscene();
    }

    public void ShowOverlayInfo(OverlayInfoManager overlayInfo)
    {
        // Показываем информацию только если есть пойманный подозреваемый
        SuspectState caughtSuspect = SuspectManager.Instance.GetCaughtSuspect();
        if (caughtSuspect != null)
        {
            overlayInfo.ShowInfo("Устранить подозреваемого");
        }
        else
        {
            overlayInfo.ShowInfo("Пока что мне это не нужно");
        }
    }

    public bool OnClick()
    {
        // Проверяем, есть ли пойманный подозреваемый перед устранением
        SuspectState caughtSuspect = SuspectManager.Instance.GetCaughtSuspect();
        if (caughtSuspect == null)
        {
            return false;
        }

        EliminateSuspect();
        return true;
    }
}