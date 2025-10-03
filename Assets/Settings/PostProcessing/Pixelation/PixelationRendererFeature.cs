using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PixelationRendererFeature : ScriptableRendererFeature
{
    [SerializeField] private PixelationSettings settings;
    [SerializeField] private Shader shader;
    private Material material;
    private PixelationRenderPass renderPass;

    public override void Create()
    {
        if (shader == null)
        {
            return;
        }

        material = new Material(shader);
        renderPass = new PixelationRenderPass(material, settings);

        // Применяем ПОСЛЕ стандартной постобработки Unity
        renderPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        if (renderPass == null)
        {
            return;
        }

        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderer.EnqueuePass(renderPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (Application.isPlaying)
        {
            Destroy(material);
        }
        else
        {
            DestroyImmediate(material);
        }
    }
}

[Serializable]
public class PixelationSettings
{
    [Header("Pixelation")] [Range(1, 8)] public int pixelSize = 4;

    [Header("Color Reduction")] [Range(2, 256)]
    public int colorDepth = 16;

    [Range(0, 1)] public float ditherStrength = 0.5f;
}