using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SingleTerrainPlane : MonoBehaviour {
	public bool isJoiningRoad;
	public int xSize = 20;
	public int ySize = 20;
	public float scaleAmt = 1f;
	public float planeFrequency = 1000f;

	//these values are stuffed from the TerrainPlanes script when Instantiated
	public BezierSpline roadCurve;
	public Vector3 pPos;
	public Vector3 pDir;
	public Vector3 pDir2;

	public LayerMask myLayerMask; 
	public LayerMask myLayerMask2;
	public LayerMask myLayerMask3;	
	public bool scriptFinished = false;
	
//	private int mapSize = 1000;



	private GameObject player;
	public bool doDebug;
	public int range = 200;

	private MeshRenderer meshRenderer;
	public bool planeBuilt;
	// Use this for initialization


	void Start () {

		//Sort out LayerMasks here

		myLayerMask = LayerMask.GetMask("Road");
		myLayerMask2 = LayerMask.GetMask("RoadSkirt");
		myLayerMask3 = LayerMask.GetMask("TerrainBase");

		meshRenderer = gameObject.GetComponent<MeshRenderer>();



		//directions are passed from TerrainPlanes script when instantiating this object
		StartCoroutine(BuildPlane(pPos,pDir,pDir2));

	//	Destroy(GetComponent<BoxCollider>());

	}

	IEnumerator BuildPlane(Vector3 planePos, Vector3 direction, Vector3 direction2)
	{
		Mesh collisionMesh = new Mesh();
		List<Vector3> tempList = new List<Vector3>();
		
		//Vector3 yAdd = new Vector3(0f,500f,0f);

		//collision mesh and visual mesh are not lined up at the moment


		//create points for collision mesh
		for (int r = 0; r <=  xSize ;r++)//how deep each section is
		{
			for (int i = 0; i <=ySize;i++)//how long each section is// Change += to change the density level
			{
	
//				Vector3 pAndR = Vector3.zero;
//				Vector3 rot1 = Vector3.zero;		
				Vector3 x = direction * i * scaleAmt ;
				Vector3 y = direction2 * r * scaleAmt ;			
				Vector3 position = planePos + x + y;		
				
				RaycastHit hit;
				
				//this should look for road first, if not hit on road, look for roadSkirt, if no hit, terrainBase

				if (Physics.Raycast(position + transform.position + (Vector3.up*500), -Vector3.up, out hit, 1000.0f,myLayerMask))
				{			
					tempList.Add(hit.point);// + Vector3.down*10);					
				}
				
				else if (Physics.Raycast(position + transform.position + (Vector3.up*500), -Vector3.up, out hit, 1000.0f,myLayerMask2))
				{
				
					tempList.Add(hit.point);// + Vector3.down*10);
				
				}
				
				else if (Physics.Raycast(position + transform.position + (Vector3.up*500), -Vector3.up, out hit, 1000.0F,myLayerMask3))
				{					
					tempList.Add(hit.point);
				}				
				else
					tempList.Add(position);
			
			}
			yield return new WaitForFixedUpdate();
		}
				
		int[] triangles = new int[(xSize * ySize) * 6];
			
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
			for (int x = 0; x < xSize; x++, ti += 6, vi++) {
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
			}				
		}


		
		
		collisionMesh.vertices = tempList.ToArray();
		//collisionMesh.normals = normals;
		collisionMesh.triangles = triangles;
		collisionMesh.name = "Collision Mesh";
		
		collisionMesh.RecalculateBounds();
		collisionMesh.RecalculateNormals();
		;


		//create visual mesh

		Mesh visualMesh = new Mesh();

		xSize = 200;
		ySize = 200;
		scaleAmt = 0.1f;
		tempList.Clear();

		for (int r = 0; r <=  xSize ;r++)//how deep each section is
		{
			for (int i = 0; i <=ySize;i++)//how long each section is// Change += to change the density level
			{
				
//				Vector3 pAndR = Vector3.zero;
//				Vector3 rot1 = Vector3.zero;		
				Vector3 x = direction * i * scaleAmt ;
				Vector3 y = direction2 * r * scaleAmt ;			
				Vector3 position = planePos + x + y;		
				
				RaycastHit hit;
				
				//this should look for road first, if not hit on road, look for roadSkirt, if no hit, terrainBase
				
				if (Physics.Raycast(position + transform.position + (Vector3.up*500), -Vector3.up, out hit, 1000.0f,myLayerMask))
				{			
					tempList.Add(hit.point);// + Vector3.down*10);					
				}
				
				else if (Physics.Raycast(position + transform.position + (Vector3.up*500), -Vector3.up, out hit, 1000.0f,myLayerMask2))
				{
					
					tempList.Add(hit.point);// + Vector3.down*10);
					
				}
				
				else if (Physics.Raycast(position + transform.position + (Vector3.up*500), -Vector3.up, out hit, 1000.0F,myLayerMask3))
				{					
					tempList.Add(hit.point);
				}				
				else
					tempList.Add(position);
				
			}
			yield return new WaitForFixedUpdate();
		}
		
		 triangles = new int[(xSize * ySize) * 6];
		
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
			for (int x = 0; x < xSize; x++, ti += 6, vi++) {
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
			}				
		}	
		
		visualMesh.vertices = tempList.ToArray();
		//mesh.normals = normals;
		visualMesh.triangles = triangles;
		
		visualMesh.RecalculateBounds();
		visualMesh.RecalculateNormals();
		;
		visualMesh.name = "Visual Mesh";
	//	GameObject terrainPlane = new GameObject();
	//	terrainPlane.transform.parent = this.gameObject.transform;
	//	terrainPlane.name = "Terrain Plane";
	//	terrainPlane.layer = 12;
		
		//add distance enablerscript to the object
		
		//terrainPlane.AddComponent<DistanceEnable>();
		
		MeshFilter meshFilterInstance = gameObject.GetComponent<MeshFilter>();
		meshFilterInstance.mesh = visualMesh;
		meshRenderer = gameObject.GetComponent<MeshRenderer>();
		meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
		meshCollider.sharedMesh = collisionMesh;
		//int randomMatint = Random.Range(0,4);
		//Material[] materials = GameObject.FindGameObjectWithTag("Materials").GetComponent<Materials>().myMaterials;
		Material newMat = Resources.Load("Green4",typeof(Material)) as Material;		
		meshRenderer.sharedMaterial = newMat;
		
		//	StartCoroutine("LinkMeshToTerrain",bezier);
		//StartCoroutine("CombineVertices",mesh);
		//AutoWeld(mesh,10f,100f);
		
		//StartCoroutine("Cubes");
		//yield return new WaitForSeconds(0.1f);

		//adjust height to remove "Z fighting"
		//This can also be done with shaders

		transform.position += Vector3.down *0.1f;

		//planeBuilt = true;

		//add Trigger Script in a gameobject to gather objects
		GameObject boxGo = new GameObject();
		boxGo.name = "boxGo";
		boxGo.transform.parent = this.transform;
		
		BoxCollider boxCollider = boxGo.AddComponent<BoxCollider>();
		boxCollider.isTrigger = true;
		boxCollider.size *= 20;	
		
		TerrainPlaneActivator tpa = boxGo.AddComponent<TerrainPlaneActivator>();
		tpa.pDir = pDir;
		tpa.pPos = meshCollider.bounds.center;


		
		yield break;
	}
}

