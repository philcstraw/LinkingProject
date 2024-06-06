using UnityEngine;

//This should be attached to the main camera
//Combine the bloom texture with the normal render texture
public class CombineBloom : MonoBehaviour
{
    internal Material fastBloomMaterial = null;
    internal RenderTexture glowTexture;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        CombineGlow(source,destination);
    }

    void CombineGlow(RenderTexture source, RenderTexture destination)
    {
        RenderTexture rt = glowTexture;
        fastBloomMaterial.SetTexture("_BloomTex", rt);
        Graphics.Blit(source, destination, fastBloomMaterial, 4);
    }
}
