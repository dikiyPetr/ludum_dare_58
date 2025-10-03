using UnityEngine;
using UnityEngine.Audio;

public class SoundCondition : MonoBehaviour
{
    [Header("🔊 Связанный AudioSource")] [Tooltip("Перетащите сюда AudioSource, который будет воспроизводить звук")]
    public AudioSource audioSource;

    public AudioResource audioResource;

    [Header("📋 Режим воспроизведения")] [Tooltip("Когда запускать звук")]
    public PlaybackMode playbackMode = PlaybackMode.OnEnter;

    [Header("🔁 Настройки зацикливания")] [Tooltip("Зациклить звук")]
    public bool loop = false;

    [Header("🎯 Ограничения воспроизведения")] [Tooltip("Сработать только один раз за всю игру")]
    public bool triggerOnce = false;

    [Tooltip("Проигрывать только пока объект в зоне")]
    public bool playWhileInZone = false;

    [Tooltip("Время задержки перед воспроизведением (секунды)")] [Range(0f, 10f)]
    public float delayBeforePlay = 0f;

    [Header("⏱️ Cooldown (перезарядка)")]
    [Tooltip("Минимальное время между воспроизведениями (секунды)")]
    [Range(0f, 60f)]
    public float cooldownTime = 0f;

    [Header("🔊 Настройки громкости")] [Tooltip("Плавное нарастание звука при входе")]
    public bool fadeIn = false;

    [Tooltip("Плавное затухание при выходе")]
    public bool fadeOut = false;

    [Tooltip("Время fade эффекта (секунды)")] [Range(0.1f, 5f)]
    public float fadeDuration = 1f;

    [Header("🎲 Случайность")] [Tooltip("Шанс воспроизведения (0-100%)")] [Range(0f, 100f)]
    public float playChance = 100f;

    [Tooltip("Случайная вариация громкости")] [Range(0f, 1f)]
    public float volumeVariation = 0f;

    [Tooltip("Случайная вариация высоты тона")] [Range(0f, 0.5f)]
    public float pitchVariation = 0f;

    [Header("📊 Отладка (только для чтения)")] [SerializeField]
    private bool isTriggered = false;

    [SerializeField] private bool isInCooldown = false;
    [SerializeField] private int triggerCount = 0;

    // Приватные переменные
    private bool hasPlayedOnce = false;
    private float lastPlayTime = -999f;
    private float targetVolume = 1f;
    private float currentFadeVolume = 1f;
    private bool isFading = false;
    private Coroutine fadeCoroutine;

    public enum PlaybackMode
    {
        [Tooltip("Воспроизвести при входе в зону")]
        OnEnter,

        [Tooltip("Воспроизвести при выходе из зоны")]
        OnExit,

        [Tooltip("Воспроизвести и при входе, и при выходе")]
        OnEnterAndExit,

        [Tooltip("Постоянное воспроизведение пока в зоне")]
        WhileInZone
    }

    private void Start()
    {
        ValidateComponents();

        if (audioSource != null)
        {
            targetVolume = audioSource.volume;
            audioSource.loop = loop;

            // Применяем AudioResource если назначен
            if (audioResource != null)
            {
                audioSource.resource = audioResource;
            }
        }
    }

    private void ValidateComponents()
    {
        if (audioSource == null)
        {
            Debug.LogError($"❌ AudioSource не назначен в {gameObject.name}! Перетащите AudioSource в инспекторе.");
        }
        else if (audioResource == null && audioSource.clip == null)
        {
            Debug.LogWarning(
                $"⚠️ AudioSource на {audioSource.gameObject.name} не имеет AudioClip и AudioResource не назначен!");
        }
    }

    // Вызывается триггером при входе
    public void OnTriggerEntered(GameObject enteringObject)
    {
        isTriggered = true;
        triggerCount++;

        if (playbackMode == PlaybackMode.OnEnter || playbackMode == PlaybackMode.OnEnterAndExit)
        {
            TryPlaySound();
        }

        if (playbackMode == PlaybackMode.WhileInZone || playWhileInZone)
        {
            TryPlaySound();
        }
    }

    // Вызывается триггером пока объект в зоне
    public void OnTriggerStaying(GameObject stayingObject)
    {
        if (playbackMode == PlaybackMode.WhileInZone && !audioSource.isPlaying)
        {
            TryPlaySound();
        }
    }

    // Вызывается триггером при выходе
    public void OnTriggerExited(GameObject exitingObject)
    {
        isTriggered = false;

        if (playbackMode == PlaybackMode.OnExit || playbackMode == PlaybackMode.OnEnterAndExit)
        {
            TryPlaySound();
        }

        if (playWhileInZone && audioSource.isPlaying)
        {
            if (fadeOut)
            {
                StartFade(0f);
            }
            else
            {
                audioSource.Stop();
            }
        }
    }

    private void TryPlaySound()
    {
        if (audioSource == null) return;

        // Проверка на triggerOnce
        if (triggerOnce && hasPlayedOnce)
        {
            return;
        }

        // Проверка cooldown
        if (Time.time - lastPlayTime < cooldownTime)
        {
            isInCooldown = true;
            return;
        }

        isInCooldown = false;

        // Проверка шанса воспроизведения
        if (Random.Range(0f, 100f) > playChance)
        {
            return;
        }

        // Применяем вариации
        ApplyRandomVariations();

        // Воспроизведение с задержкой
        if (delayBeforePlay > 0)
        {
            Invoke(nameof(PlaySoundDelayed), delayBeforePlay);
        }
        else
        {
            PlaySoundDelayed();
        }
    }

    private void PlaySoundDelayed()
    {
        if (audioSource == null) return;

        // Fade in эффект
        if (fadeIn)
        {
            currentFadeVolume = 0f;
            audioSource.volume = 0f;
            StartFade(targetVolume);
        }
        else
        {
            audioSource.volume = targetVolume;
        }

        audioSource.Play();

        hasPlayedOnce = true;
        lastPlayTime = Time.time;
    }

    private void ApplyRandomVariations()
    {
        // Вариация громкости
        if (volumeVariation > 0)
        {
            float volumeOffset = Random.Range(-volumeVariation, volumeVariation);
            audioSource.volume = Mathf.Clamp01(targetVolume + volumeOffset);
        }

        // Вариация высоты тона
        if (pitchVariation > 0)
        {
            float pitchOffset = Random.Range(-pitchVariation, pitchVariation);
            audioSource.pitch = Mathf.Clamp(1f + pitchOffset, 0.5f, 2f);
        }
    }

    private void StartFade(float targetVol)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeVolume(targetVol));
    }

    private System.Collections.IEnumerator FadeVolume(float targetVol)
    {
        isFading = true;
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVol, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVol;

        // Если fade out завершен и громкость 0, останавливаем
        if (targetVol == 0f)
        {
            audioSource.Stop();
        }

        isFading = false;
    }

    // Публичные методы для внешнего управления
    public void ResetTrigger()
    {
        hasPlayedOnce = false;
        triggerCount = 0;
    }

    public void ForcePlay()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    public void ForceStop()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}