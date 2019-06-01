using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshControlTiles : MonoBehaviour {
	
	//	public bool combineMeshes = true;
	public bool doChange = false;
	public bool renderIndividuals =false;
	public bool renderCombined = false;
	public bool setActiveFinised;
	public bool individualsInAction;
	Transform[] ts ;
	void Start () {
		renderCombined = true;
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//combinedBool = false;
		if (Input.GetKeyDown("return")){
			doCombine();
			renderCombined=true;
		}
		
		if (Input.GetKeyDown("space")){
			
		}
		
		doCombine();
		doIndividuals();
		
	}
	
	
	void doCombine(){
		
		if(renderCombined && !individualsInAction){
			Debug.Log("rendering combined");
			
			CombineMeshes();	
			
			renderCombined = false;
			
		}
	}
	
	
	void doIndividuals(){
		
		if(renderIndividuals){
			Debug.Log("rendering individuals");
			
			StartCoroutine(SlowlySetActive());
			setActiveFinised = false;
			renderIndividuals = false;
			
		}
	}
	
	
	
	IEnumerator SlowlySetActive()
	{
		individualsInAction = true;
		Debug.Log("coroutine started");
		
		ts = GetComponentsInChildren<Transform>(true);
		
		for(int i = 0; i < ts.Length; i+=100)
		{
			for (int j = 0; j < 100; j++)
			{
				if (ts[j]!=null && i + j < ts.Length)
				{
					ts[i + j].gameObject.SetActive(true);
				}
			}
			//if(i == ts.Length)
			//{
			
			//}
			yield return 0;
		}
		
		DestroyCombinedMeshes();
		Debug.Log("destroying combined");
		individualsInAction = false;
		
		
	}
	
	void DestroyCombinedMeshes()
	{
		Transform[] combined = GetComponentsInChildren<Transform>();
		
		
		foreach(Transform t in combined)
		{
			if (t.gameObject.name == ("Combined mesh"))
			{
				Destroy(t.gameObject);
			}
			
		}
	}
	
	void CombineMeshes()
	{
		//creates a new gameobject with a combined mesh in it
		
		Matrix4x4 myTransform = transform.worldToLocalMatrix;
		Dictionary<Material, List<CombineInstance>> combines = new Dictionary<Material, List<CombineInstance>>();
		MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
		foreach (var meshRenderer in meshRenderers)
		{
			foreach (var material in meshRenderer.sharedMaterials)
				if (material != null && !combines.ContainsKey(material))
					combines.Add(material, new List<CombineInstance>());
		}
		
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(true);
		foreach(var filter in meshFilters)
		{
			if (filter.sharedMesh == null)
				continue;
			CombineInstance ci = new CombineInstance();
			ci.mesh = filter.sharedMesh;
			ci.transform = myTransform * filter.transform.localToWorldMatrix;
			combines[filter.GetComponent<Renderer>().sharedMaterial].Add(ci);
			//filter.GetComponent<Renderer>().enabled = false;
			
		}
		
		Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>(true);
		foreach(Rigidbody rb in rbs)
		{
			if(rb.gameObject.name==("Tile"))
			{
				rb.gameObject.SetActive(false);
			}
		}
		
		
		
		
		foreach(Material m in combines.Keys)
		{
			var go = new GameObject("Combined mesh");
			go.tag = "CombinedMesh";
			go.transform.parent = transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			
			var filter = go.AddComponent<MeshFilter>();
			filter.mesh.CombineMeshes(combines[m].ToArray(), true, true);
			
			var renderer = go.AddComponent<MeshRenderer>();
			renderer.material = m;
		}
		
		//this.gameObject.AddComponent<MeshControl>();
		
	}
}

