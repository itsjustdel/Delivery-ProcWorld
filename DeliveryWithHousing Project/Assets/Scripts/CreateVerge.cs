using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateVerge : MonoBehaviour {
	public BezierSpline bezierL;
	public BezierSpline bezierR;
	// Use this for initialization
	public float frequency = 1000;
	public int vergeWidth =10;
	public Transform quadPrefab;
	public KerbMesh kerbMesh;
	public SplineMesh splineMeshScript;
	void Start() 
	{	
		//bezierL = GameObject.Find("Kerbs").GetComponent<BezierSpline>();
		bezierL = kerbMesh.splineL;
		bezierR = kerbMesh.splineR;
		StartCoroutine("buildFieldsCoLeft");
		StartCoroutine("buildFieldsCoRight");
	}



	IEnumerator buildVergeCo()
	{

		bezierR = kerbMesh.splineL;
	//	for (float k = 0; k <= frequency/10; k+=frequency/1000)//how many sections there are
	//	{
			
			for (int i = 0; i <= frequency/1000; i++)//how long each section is
			{
				
				for(int j = 1; j<= vergeWidth; j++)//how wide/deep each section is
				{



					Vector3 point = bezierR.GetPoint((i)/(frequency-1));
					Vector3 direction = bezierR.GetDirection ((i)/(frequency-1));
					float random = Random.Range(0.11f,1.5f);
					Vector3 rot1 = Quaternion.Euler (0, -90, 0) * direction * (j*0.1f*random); 			

					BezierSpline vergeCurve = new BezierSpline();
					List<Vector3> vergeList = new List<Vector3>();
					vergeList.Add(point);
					vergeList.Add(point + Vector3.up*4);
					vergeList.Add(point+direction + (Vector3.up*4));
					vergeList.Add(point+(direction*2) + (Vector3.up*4));

					vergeCurve.points = vergeList.ToArray();
					Vector3 pointV = vergeCurve.GetPoint(j);

					Transform quad = Instantiate(quadPrefab);					
					quad.position = rot1 + pointV;
				//	quad.rotation = Random.rotation;
					quad.transform.localScale *=0.2f;
					quad.parent = this.gameObject.transform;
					
					//Create the back of plane
					Transform quad2 =Instantiate(quad);
					quad2.position = rot1 + pointV;
				//	quad2.rotation = Random.rotation;
					//spin therotation to look backwards
					quad.rotation *= Quaternion.Euler(180f,180f,180f);				
					quad2.transform.localScale *=0.2f;
					quad2.parent = this.gameObject.transform;
				}
			}
			StartCoroutine("Combine");
			//yield return new WaitForFixedUpdate();
		yield break;
		//}
	}

	IEnumerator buildFieldsCoLeft()
	{

		//Make verge on Left Hand side
		for (float k = 0; k <= frequency/10; k+=frequency/1000)//how many sections there are
		{
			
			for (int i = 0; i <= frequency/1000; i++)//how long each section is
			{
				
				for(int j = 1; j<= vergeWidth; j++)//how wide each section is
				{
					Transform quad = Instantiate(quadPrefab) as Transform;
					Vector3 point = bezierL.GetPoint((i+k)/(frequency-1));
					Vector3 direction = bezierL.GetDirection ((i+k)/(frequency-1));
					float random = Random.Range(0.2f,1.8f);
					Vector3 shiftBack = Quaternion.Euler (0, -90, 0) * (direction*random*0.2f); //number here is how much it is pulled back on to road mesh
					Vector3 rot1 = Quaternion.Euler (0, -90, 0) * (direction*j*0.1f);
					Vector3 upwards = Vector3.up*random*quad.localScale.y*0.1f;

					point -= shiftBack;


					quad.position = rot1 + point + upwards;
					Quaternion randomRot = Random.rotation;
					quad.rotation = randomRot;
					quad.transform.localScale *=0.2f;
					quad.parent = this.gameObject.transform;
					
					//Create the back of plane
					Transform quad2 =Instantiate(quadPrefab) as Transform;
					quad2.position = rot1 + point+upwards;
					quad2.localRotation = randomRot;

					//spin therotation to look backwards
					quad2.rotation *= Quaternion.Euler(180f,0f,90f);	//used these numbers so vertices join up(aesthetic niceness)			
					quad2.transform.localScale *=0.2f;

					quad2.parent = this.gameObject.transform;

				}

			}
			//StartCoroutine("Combine");
			yield return new WaitForFixedUpdate();

		}
	}

	IEnumerator buildFieldsCoRight()
	{
		
		//Make verge on Left Hand side
		for (float k = 0; k <= frequency/10; k+=frequency/1000)//how many sections there are
		{
			
			for (int i = 0; i <= frequency/1000; i++)//how long each section is
			{
				
				for(int j = 1; j<= vergeWidth; j++)//how wide each section is
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
