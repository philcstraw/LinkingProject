using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/FastMobileBloom")]
public class FastMobileBloom : MonoBehaviour
{
    public Material fastBloomMaterial = null;
    public RenderTexture glowTexture;
    public GameObject mainCamera;
    public Camera glowCamera;
    public CombineBloom combineBloom;

    [Range(0.0f, 1.5f)] public float threshold = 0.25f;
	[Range(0.00f, 4.0f)] public float intensity = 1.0f;
	[Range(0.01f, 5.5f)] public float blurSize = 1.0f;
	[Range(1, 4)] public int blurIterations = 2;

    void Awake()
    {
        if (glowCamera.targetTexture != null)
            glowCamera.targetTexture.Release();

        glowCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        glowTexture = glowCamera.targetTexture;

        combineBloom.fastBloomMaterial = fastBloomMaterial;
        combineBloom.glowTexture = glowTexture;

        fastBloomMaterial.SetFloat("_Spread", blurSize);
        fastBloomMaterial.SetVector("_ThresholdParams", new Vector2(1.0f, -threshold));
        fastBloomMaterial.SetFloat("_BloomIntensity", intensity);
    }
   
    void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		int rtW = source.width / 4;
		int rtH = source.height / 4;
        //initial downsample
        RenderTexture rt = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
		rt.DiscardContents();
        
		Graphics.Blit(source, rt, fastBloomMaterial, 0);
        
        //downscale
        RenderTexture rt2 = RenderTexture.GetTemporary(rt.width / 2, rt.height / 2, 0, source.format);
        for (int i = 0; i < blurIterations - 1; i++)
		{
            rt2.DiscardContents();
            Graphics.Blit(rt, rt2, fastBloomMaterial, 1);
			RenderTexture.ReleaseTemporary(rt);
			rt = rt2;
		}

        //upscale
        RenderTexture rt3 = RenderTexture.GetTemporary(rt.width*2, rt.height*2, 0, source.format);
        for (int i = 0; i < blurIterations - 1; i++)
		{
			rt3.DiscardContents();
			Graphics.Blit(rt, rt3, fastBloomMaterial, 2);
			RenderTexture.ReleaseTemporary(rt);
			rt = rt3;
		}

		//fastBloomMaterial.SetFloat("_BloomIntensity", intensity);
		fastBloomMaterial.SetTexture("_BloomTex", rt);
        //destination.height = rtH;
        //destination.width = rtW;
		Graphics.Blit(source, destination, fastBloomMaterial, 3);

		RenderTexture.ReleaseTemporary(rt);
	}
}
