using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class PixelationRenderPass : ScriptableRenderPass
{
    private static readonly int pixelSizeId = Shader.PropertyToID("_PixelSize");
    private static readonly int colorDepthId = Shader.PropertyToID("_ColorDepth");
    private static readonly int ditherStrengthId = Shader.PropertyToID("_DitherStrength");
    private static readonly int timeId = Shader.PropertyToID("_TimeValue");
    
    private const string k_PassName = "PixelationPass";

    private PixelationSettings defaultSettings;
    private Material material;

    public PixelationRenderPass(Material material, PixelationSettings defaultSettings)
    {
        this.material = material;
        this.defaultSettings = defaultSettings;
    }

    private void UpdateSettings()
    {
        if (material == null) return;

        var volumeComponent = VolumeManager.instance.stack.GetComponent<PixelationVolumeComponent>();
        
        int pixelSize = volumeComponent.pixelSize.overrideState ?
            volumeComponent.pixelSize.value : defaultSettings.pixelSize;
        int colorDepth = volumeComponent.colorDepth.overrideState ?
            volumeComponent.colorDepth.value : defaultSettings.colorDepth;
        float ditherStrength = volumeComponent.ditherStrength.overrideState ?
            volumeComponent.ditherStrength.value : defaultSettings.ditherStrength;
            
        material.SetInt(pixelSizeId, pixelSize);
        material.SetInt(colorDepthId, colorDepth);
        material.SetFloat(ditherStrengthId, ditherStrength);
        material.SetFloat(timeId, Time.time);
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

        if (resourceData.isActiveTargetBackBuffer)
            return;

        TextureHandle srcCamColor = resourceData.activeColorTexture;
        
        var descriptor = srcCamColor.GetDescriptor(renderGraph);
        descriptor.depthBufferBits = 0;
        descriptor.name = "_PixelationTemp";
        
        var tempTexture = renderGraph.CreateTexture(descriptor);

        UpdateSettings();

        if (!srcCamColor.IsValid() || !tempTexture.IsValid())
            return;
        
        // Применяем пикселизацию и палитризацию
        RenderGraphUtils.BlitMaterialParameters mainParams = 
            new(srcCamColor, tempTexture, material, 0);
        renderGraph.AddBlitPass(mainParams, k_PassName);
        
        // Копируем обратно
        RenderGraphUtils.BlitMaterialParameters copyParams = 
            new(tempTexture, srcCamColor, material, 1);
        renderGraph.AddBlitPass(copyParams, "CopyBack");
    }
}