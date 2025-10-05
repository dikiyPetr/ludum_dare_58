using UnityEngine;

public enum SurfaceTypeEnum
{
    Default,
    Wood,
    Metal,
    Paper,
    Rug
}

public class SurfaceType : MonoBehaviour
{
    [Header("Тип поверхности")]
    public SurfaceTypeEnum surfaceType = SurfaceTypeEnum.Default;

    public string GetSurfaceTypeName()
    {
        return surfaceType.ToString();
    }
}