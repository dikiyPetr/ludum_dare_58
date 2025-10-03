using System;
using UnityEngine.Rendering;

[Serializable]
public class PixelationVolumeComponent : VolumeComponent
{
    public ClampedIntParameter pixelSize = new ClampedIntParameter(4, 1, 8);
    public ClampedIntParameter colorDepth = new ClampedIntParameter(16, 2, 256);
    public ClampedFloatParameter ditherStrength = new ClampedFloatParameter(0.5f, 0, 1);
}