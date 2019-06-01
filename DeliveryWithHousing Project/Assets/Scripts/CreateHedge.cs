using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateHedge : MonoBehaviour {
	public BezierSpline bezierL;
	public BezierSpline bezierR;
	// Use this for initialization
	public float frequency = 1000;
	public int hedgeWidth =10;
	public int hedgeHeight = 30;
	public Transform quadPrefab;
	public KerbMesh kerbMesh;
	public SplineMesh splineMeshScript;
	void Start() 
	{	
		//bezierL = GameObject.Find("Kerbs").GetComponent<BezierSpline>();
		bezierL = kerbMesh.splineL;
		bezierR = kerbMesh.splineR;
		StartCoroutine("buildHedgeCoLeft");
//		StartCoroutine("buildFieldsCoRight");
	}

	IEnumerator buildHedgeCoLeft()
	{
		
		//Make verge on Left Hand side
		for (float k = 1; k <= frequency/10; k+=frequency/1000)//how many sections there are
		{
			
			for (int i = 1; i <= frequency/1000; i++)//how long each section is
			{
				
				for(int j = 1; j<= hedgeWidth; j++)//how wide each section is
				{

					for(int l = 1; l <= hedgeHeight; l++)
					{
						if ( l==hedgeHeight || j==1 || j== hedgeWidth){//Only build outside edges --put in k == the end of the hedgerow(howlong is that?)

							Transform quad = Instantiate(quadPrefab) as Transform;
							quad.transform.localScale *=0.2f;

							Vector3 point = bezierL.GetPoint((i+k)/(frequency-1));
							Vector3 direction = bezierL.GetDirection ((i+k)/(frequency-1));
							float random = Random.Range(0.5f,1.5f);
							Vector3 upwards = Vector3.up*random*l*quad.localScale.y;
							Vector3 shiftBack = Quaternion.Euler (0, -90, 0) * ((direction)*random*quad.localScale.z); //number here is how much it is pulled back on to road mesh
							Vector3 rot1 = Quaternion.Euler (0, -90, 0) * (direction*j*0.1f);
													
							point += shiftBack*5; // quadprefab is actually referencing the grass verge prefab size ;)
							
							
							quad.position =point+ rot1 + upwards;// + shiftBack;
							Quaternion randomRot = Random.rotation;
							quad.rotation = randomRot;

							quad.parent = this.gameObject.transform;
							
							//Create the back of plane
							Transform quad2 =Instantiate(quadPrefab) as Transform;
							quad2.position = rot1 + point+ upwards;
							quad2.localRotation = randomRot;
							
							//spin therotation to look backwards
							quad2.rotation *= Quaternion.Euler(180f,0f,90f);	//used these numbers so vertices join up(aesthetic niceness)			
							quad2.transform.localScale *=0.2f;
							
							quad2.parent = this.gameObject.transform;

						}
					}
				}				
			}
			StartCoroutine("Combine");
			yield return new WaitForFixedUpdate();
			
		}
	}
	IEnumerator buildHedgeCoRight()
	{
		
		//Make verge on Left Hand side
		for (float k = 0; k <= frequency/10; k+=frequency/1000)//how many sections there are
		{
			
			for (int i = 0; i <= frequency/1000; i++)//how long each section is
			{
				
				for(int j = 1; j<= hedgeWidth; j++)//how wide each section is
				{
					Vector3 point = bezierR.GetPoint((i+k)/(frequency-1));
					Vector3 direction = bezierR.GetDirection ((i+k)/(frequency-1));
					float random = Random.Range(0.5f,1.5f);
					Vector3 shiftBack = Quaternion.Euler (0, 90, 0) * (direction*random*0.2f); //number here is how much it is pulled back on to road mesh
					Vector3 rot1 = Quaternion.Euler (0, 90, 0) * (direction*j*0.1f);
					
					
					point -= shiftBack;
					
					Transform quad = Instantiate(quadPrefab) as Transform;
					quad.position = rot1 + point;
					Quaternion randomRot = Random.rotation;
					quad.rotation = randomRot;
					quad.transform.localScale *=0.2f;
					quad.parent = this.gameObject.transform;
					
					//Create the back of plane
					Transform quad2 =Instantiate(quadPrefab) as Transform;
					quad2.position = rot1 + point;
					quad2.localRotation = randomRot;
					
					//spin therotation to look backwards
					quad2.rotation *= Quaternion.Euler(180f,0f,90f);	//used these numbers so vertices join up(aesthetic niceness)			
					quad2.transform.localScale *=0.2f;
					
					quad2.parent = this.gameObject.transform;
					
				}
				
			}
			StartCoroutine("Combine");
			yield return new WaitForFixedUpdate();
			
		}
	}
	
	IEnumerator Combine()
	{
		//combines meshes and destroys
		{
			Matrix4x4 myTransform = transform.worldToLocalMatrix;
			Dictionary<Material, List<CombineInstance>> combines = new Dictionary<Material, List<CombineInstance>>();
			MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
			foreach (var meshRenderer in meshRenderers)
			{
				foreach (var material in meshRenderer.sharedMaterials)
					if (material != null && !combines.ContainsKey(material))
						combines.Add(material, new List<CombineInstance>());
			}
			
			MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
			foreach(var filter in meshFilters)
			{
				if (filter.sharedMesh == null)
					continue;
				CombineInstance ci = new CombineInstance();
				ci.mesh = filter.sharedMesh;
				ci.transform = myTransform * filter.transform.localToWorldMatrix;
				combines[filter.GetComponent<Renderer>().sharedMaterial].Add(ci);
				filter.GetComponent<Renderer>().enabled = false;
				Destroy(filter.gameObject);
				
			}
			
			foreach(Material m in combines.Keys)
			{
				var go = new GameObject("Combined mesh");
				go.tag = "CombinedMesh";
				//go.transform.parent = transform;
				go.transform.position = transform.position;
				go.transform.localRotation = Quaternion.identity;
				go.transform.localScale = Vector3.one;
				
				var filter = go.AddComponent<MeshFilter>();
				filter.mesh.CombineMeshes(combines[m].ToArray(), true, true);
				
				var renderer = go.AddComponent<MeshRenderer>();
				renderer.material = m;
			}
			
			//this.gameObject.AddComponent<MeshControl>();
		}
		yield break;
	}
}
