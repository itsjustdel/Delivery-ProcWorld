    using UnityEngine;
     
    [ExecuteInEditMode]
    public class glareFxStarburts : MonoBehaviour
    {
		
		public float intensity = 5.0f;
		public float threshold = 0.5f;
		public int blurIteration = 2;
		public float haloIntensity = 1.0f;	
		public float starburstIntensity = 1.0f;	
		public Vector3 chromaticDistortion = new Vector3(-0.04f,0.0f,0.04f);
	
		public Texture2D lensDirt;
		public Texture2D starburstTexture;
	
        public Shader shader;
	
        private Material rbMaterial = null;
	
		private Matrix4x4 bias1;
		private Matrix4x4 bias2;
		private Matrix4x4 rot;
		private Matrix4x4 StarMatrix;
     
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
    
	void OnEnable() {
		
			StarMatrix= new Matrix4x4();
			rot= new Matrix4x4();
			bias1= new Matrix4x4();
			bias2= new Matrix4x4();
		
			bias1.SetRow(0,new Vector4(1.6f,0.0f,-0.8f,0.0f));
			bias1.SetRow(1,new Vector4(0.0f,1.6f,-0.8f,0.0f));
			bias1.SetRow(2,new Vector4(0.0f,0.0f,1.0f,0.0f));
			bias1.SetRow(3,new Vector4(0.0f,0.0f,0.0f,0.0f));
		
			bias2.SetRow(0,new Vector4(0.5f,0.0f,0.5f,0.0f));
			bias2.SetRow(1,new Vector4(0.0f,0.5f,0.5f,0.0f));
			bias2.SetRow(2,new Vector4(0.0f,0.0f,1.0f,0.0f));
			bias2.SetRow(3,new Vector4(0.0f,0.0f,0.0f,0.0f));		
		
		
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
			haloIntensity = Mathf.Clamp(haloIntensity,0,10);
			starburstIntensity = Mathf.Clamp(starburstIntensity,0,10);
		
			GetMaterial().SetFloat("_int", intensity);
			GetMaterial().SetFloat("_starint", starburstIntensity);
			GetMaterial().SetVector("_chromatic",chromaticDistortion);
		
			GetMaterial().SetTexture("_OrgTex", source);
			GetMaterial().SetTexture("_lensDirt", lensDirt);
			GetMaterial().SetTexture("_starTex", starburstTexture);
		
			GetMaterial().SetFloat("_threshold", threshold);
			GetMaterial().SetFloat("_haloint", haloIntensity);		
		
			//matrix preparation
			Vector3 camx = Camera.main.cameraToWorldMatrix.GetColumn(0);
			Vector3 camz = Camera.main.cameraToWorldMatrix.GetColumn(1);
			float camrot = Vector3.Dot(camx, new Vector3(0,0,1)) + Vector3.Dot(camz, new Vector3(0,1,0));
		
			//Matrix4x4 rot= new Matrix4x4();
		
			rot.SetRow(0,new Vector4(Mathf.Cos(camrot),-Mathf.Sin(camrot),0.0f,0.0f));
			rot.SetRow(1,new Vector4(Mathf.Sin(camrot),Mathf.Cos(camrot),0.0f,0.0f));
			rot.SetRow(2,new Vector4(0.0f,0.0f,1.0f,0.0f));
			rot.SetRow(3,new Vector4(0.0f,0.0f,0.0f,0.0f));		
		
			StarMatrix = bias2 * rot * bias1;
			
			GetMaterial().SetMatrix("StarMatrix",StarMatrix);
		
			RenderTexture buffer = RenderTexture.GetTemporary(source.width/4, source.height/4, 0);
			RenderTexture buffer2 = RenderTexture.GetTemporary(source.width/4, source.height/4, 0);
		
			//RenderTexture buffer3 = RenderTexture.GetTemporary(source.width, source.height, 0);
		
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