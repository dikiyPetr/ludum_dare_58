using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SurfaceSound
{
    public SurfaceTypeEnum surfaceTypeName;
    public AudioClip[] footstepSounds;
}

public class FootstepSoundManager : MonoBehaviour
{
    private const string DefaultSurfaceType = "Default";

    [Header("Звуки для разных поверхностей")]
    [SerializeField] private SurfaceSound[] surfaceSounds;

    private Dictionary<string, AudioClip[]> soundDictionary;

    void Awake()
    {
        InitializeSoundDictionary();
    }

    void InitializeSoundDictionary()
    {
        soundDictionary = new Dictionary<string, AudioClip[]>();
        foreach (var surfaceSound in surfaceSounds)
        {
            if (!soundDictionary.ContainsKey(surfaceSound.surfaceTypeName.ToString()))
            {
                soundDictionary.Add(surfaceSound.surfaceTypeName.ToString(), surfaceSound.footstepSounds);
            }
        }
    }

    public AudioClip GetFootstepSound(string surfaceType)
    {
        if (soundDictionary == null) InitializeSoundDictionary();

        if (soundDictionary.ContainsKey(surfaceType))
        {
            AudioClip[] clips = soundDictionary[surfaceType];
            if (clips != null && clips.Length > 0)
            {
                return clips[Random.Range(0, clips.Length)];
            }
        }

        // Если звук не найден, пытаемся вернуть звук по умолчанию
        if (soundDictionary.ContainsKey(DefaultSurfaceType))
        {
            AudioClip[] clips = soundDictionary[DefaultSurfaceType];
            if (clips != null && clips.Length > 0)
            {
                return clips[Random.Range(0, clips.Length)];
            }
        }

        return null;
    }
}