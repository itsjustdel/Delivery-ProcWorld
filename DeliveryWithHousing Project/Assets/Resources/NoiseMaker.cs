using UnityEngine;
using System.Collections;

public class NoiseMaker : MonoBehaviour
{
    public int pixWidth;
    public int pixHeight;
    public float xOrg;
    public float yOrg;
    public float scale = 1.0F;
    private Texture2D noiseTex;
    private Texture2D normalTex;
    private Color[] pix;
    private Renderer rend;
    void Start()
    {
        rend = GetComponent<Renderer>();
        noiseTex = new Texture2D(pixWidth, pixHeight);
        pix = new Color[noiseTex.width * noiseTex.height];
        rend.material.mainTexture = noiseTex;

        CalcNoise();
        normalTex = NormalMap(noiseTex, 0.5f);

        Material noise = new Material(Resources.Load("Blue") as Material);
        noise.name = "Noise";
        //noise.color = Color.blue;
        noise.SetTexture("_MainTex", noiseTex);
        noise.SetTexture("_BumpMap", normalTex);
        noise.SetTexture("_HeightMap", noiseTex);
        rend.material = noise;
        
        //noise.SetFloat("Height", 0.1234f);

        
    }
    void CalcNoise()
    {
        float y = 0.0F;
        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {
                float xCoord = xOrg + x / noiseTex.width * scale;
                float yCoord = yOrg + y / noiseTex.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[(int)(y * noiseTex.width + x)] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }
        noiseTex.SetPixels(pix);
        noiseTex.Apply();

        
        
        
    }
    void Update()
    {

    }

    private Texture2D NormalMap(Texture2D source, float strength)
    {
        strength = Mathf.Clamp(strength, 0.0F, 1.0F);

        Texture2D normalTexture;
        float xLeft;
        float xRight;
        float yUp;
        float yDown;
        float yDelta;
        float xDelta;

        normalTexture = new Texture2D(source.width, source.height, TextureFormat.ARGB32, true);

        for (int y = 0; y < normalTexture.height; y++)
        {
            for (int x = 0; x < normalTexture.width; x++)
            {
                xLeft = source.GetPixel(x - 1, y).grayscale * strength;
                xRight = source.GetPixel(x + 1, y).grayscale * strength;
                yUp = source.GetPixel(x, y - 1).grayscale * strength;
                yDown = source.GetPixel(x, y + 1).grayscale * strength;
                xDelta = ((xLeft - xRight) + 1) * 0.5f;
                yDelta = ((yUp - yDown) + 1) * 0.5f;
                normalTexture.SetPixel(x, y, new Color(xDelta, yDelta, 1.0f, yDelta));
            }
        }
        normalTexture.Apply();

        //Code for exporting the image to assets folder
        System.IO.File.WriteAllBytes("Assets/NormalMap.png", normalTexture.EncodeToPNG());

        return normalTexture;
    }
}