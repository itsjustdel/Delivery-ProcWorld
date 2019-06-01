    using UnityEngine;
     
    [ExecuteInEditMode]
    public class glareFxCheapSM20 : MonoBehaviour
    {
		
		public float intensity = 5.0f;
		public float threshold = 0.5f;
		public int blurIteration = 2;
	
		public Texture2D lensDirt;
	
        public Shader shader;
	
        private Material rbMaterial = null;
     
        private Material GetMaterial()
        {
            if (rbMaterial == null)
            {
                rbMaterial = new Material(shader);
                rbMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return rbMaterial;
        }
	
    public Shader blurShader;
    Material m_BlurMaterial = null;
	protected Material blurMaterial {
		get {
			if (m_BlurMaterial == null) {
                m_BlurMaterial = new Material(blurShader);
                m_BlurMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_BlurMaterial;
		} 
	}	
	
	protected void OnDisable()
	{
		if( m_BlurMaterial ) 
			DestroyImmediate( m_BlurMaterial );

	}	
     
        void Start()
        {
            if (shader == null)
            {
                Debug.LogError("No glare shader assigned!", this);
				enabled = false;
            }
			if( blurShader == null )
			{
				Debug.LogError ("No blur shader assigned!", this);
				enabled = false;
			}		
		
        }
	
	// Performs one blur iteration.
	public void FourTapCone (RenderTexture source, RenderTexture dest)
	{
		float off = 0.75f;
		Graphics.BlitMultiTap (source, dest, blurMaterial,
			new Vector2(-off, -off),
			new Vector2(-off,  off),
			new Vector2( off,  off),
			new Vector2( off, -off)
		);
	}
	
	// Downsamples the texture to a quarter resolution.
	private void DownSample4x (RenderTexture source, RenderTexture dest)
	{
		float off = 1.0f;
		Graphics.BlitMultiTap (source, dest, blurMaterial,
			new Vector2(-off, -off),
			new Vector2(-off,  off),
			new Vector2( off,  off),
			new Vector2( off, -off)
		);
	}
     
        void OnRenderImage(RenderTexture source, RenderTexture dest)
        {
		
			threshold = Mathf.Clamp01(threshold);
			intensity = Mathf.Clamp(intensity,0,10);
		
			GetMaterial().SetFloat("_int", intensity);
			GetMaterial().SetTexture("_OrgTex", source);
			GetMaterial().SetTexture("_lensDirt", lensDirt);
			GetMaterial().SetFloat("_threshold", threshold);
		
			RenderTexture buffer = RenderTexture.GetTemporary(source.width/4, source.height/4, 0);
			RenderTexture buffer2 = RenderTexture.GetTemporary(source.width/4, source.height/4, 0);
		
			DownSample4x (source, buffer);
		
       bool oddEven = true;
       for(int i = 0; i < blurIteration; i++)
       {
         if( oddEven )
          FourTapCone (buffer, buffer2);
         else
          FourTapCone (buffer2, buffer);
         oddEven = !oddEven;
       }
       if( oddEven )
         Graphics.Blit(buffer, dest,GetMaterial());
       else
         Graphics.Blit(buffer2, dest,GetMaterial());	
		
		RenderTexture.ReleaseTemporary(buffer);
		RenderTexture.ReleaseTemporary(buffer2);

        }
    }