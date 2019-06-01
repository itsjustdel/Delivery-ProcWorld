using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TestPlane : MonoBehaviour {

	public int xSize;
	public int ySize;
	public float scaleAmt =1 ;

	public LayerMask myLayerMask;
	public LayerMask myLayerMask2;
	public LayerMask myLayerMask3;
	public Verge verge;
	public Transform vergeObject;
	public Transform cube;
	public bool doCubes;
	public MeshFilter planeMesh;
	// Use this for initialization
	void Start () {

	
		StartCoroutine("BuildMesh");
	}

	void Update()
	{
		if (doCubes)
		{
			//StartCoroutine("Cubes");
			StartCoroutine("MoveMesh");
			doCubes =false;
		}
	}
	
	IEnumerator BuildMesh() 
	{
		//meshFilter = plane.gameObject.GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		List<Vector3> tempList = new List<Vector3>();

		//ySize = 100;

//		Vector3 yAdd = new Vector3(0f,500f,0f);
		
		for (int r = 0; r <=  xSize ;r++)//how deep each section is
		{
			for (int i = 0; i <=ySize;i++)//how long each section is// Change += to change the density level
			{

				//Vector3 position = bezier.GetPoint((i)/(frequency-1));
				//Vector3 direction = bezier.GetDirection (i/(frequency-1));
				//float random = Random.Range(0.5f,1.5f);
//				Vector3 pAndR = Vector3.zero;
//				Vector3 rot1 = Vector3.zero;
				//Check what side of the road it has been passed and adjust directional vector accordingly

				// Ignore first row of mesh, as this is attached to the kerbside, move all other mesh heights to match terrain
				Vector3 x = Vector3.right * i * scaleAmt;
				Vector3 y = Vector3.forward * r * scaleAmt;
				Vector3 position = x+y;



					//Transform item = Instantiate(cube) as Transform;
					//item.position = position;
					//tempList.Add(position);




				RaycastHit hit;

				if (Physics.Raycast(position + transform.position, -Vector3.up, out hit, 1000.0f,myLayerMask))
				{
					if (hit.collider.CompareTag("TerrainSecondLayer"))
					{
						tempList.Add(hit.point);// + Vector3.down);
					//	Debug.Log("t2ndLayer");
			
					}
					if (hit.collider.gameObject.tag == "Road")
					{
						tempList.Add(hit.point);// + Vector3.down);
					//	Debug.Log("road");
					}				
				}

				else if (Physics.Raycast(position + transform.position, -Vector3.up, out hit, 1000.0F,myLayerMask2))
					{
						
							tempList.Add(hit.point);
							//Debug.Log("TBaseLayer");
						
					
					
					}
					//Debug.Log("never hit");


			}

		}
			

		
		int[] triangles = new int[(xSize * ySize) * 6];
		//int[] triangles = new int[(tempList.Count)*6]; //cant get this to work
		
			for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
				for (int x = 0; x < xSize; x++, ti += 6, vi++) {
					triangles[ti] = vi;
					triangles[ti + 3] = triangles[ti + 2] = vi + 1;
					triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
					triangles[ti + 5] = vi + xSize + 2;
				}				
			}


		mesh.vertices = tempList.ToArray();
		//mesh.normals = normals;
		mesh.triangles = triangles;
		
		
		mesh.RecalculateNormals();
		;
		
		GameObject terrainPlane = new GameObject();
		terrainPlane.transform.parent = this.gameObject.transform;
		terrainPlane.name = "Terrain Plane";
		terrainPlane.layer = 12;
		
		MeshFilter meshFilterInstance = terrainPlane.gameObject.AddComponent<MeshFilter>();
		meshFilterInstance.mesh = mesh;
		MeshRenderer meshRenderer = terrainPlane.gameObject.AddComponent<MeshRenderer>();
		//MeshCollider meshCollider = 
		terrainPlane.gameObject.AddComponent<MeshCollider>();

		//int randomMatint = Random.Range(0,4);
		//Material[] materials = GameObject.FindGameObjectWithTag("Materials").GetComponent<Materials>().myMaterials;
		Material newMat = Resources.Load("GreenTerrainBase",typeof(Material)) as Material;		
		meshRenderer.sharedMaterial = newMat;
	
			//	StartCoroutine("LinkMeshToTerrain",bezier);
			//StartCoroutine("CombineVertices",mesh);
			//AutoWeld(mesh,10f,100f);

		//StartCoroutine("Cubes");
		yield break;
	}

	IEnumerator Cubes()
	{
		Mesh mesh = vergeObject.GetComponent<MeshFilter>().mesh;
		Vector3[] tempArray;
		tempArray = mesh.vertices;
		for (int i = 0; i <= verge.xSize; i++)
		{
			//Transform item = Instantiate(cube) as Transform;
			//item.position = mesh.vertices[ (mesh.vertices.Length-1-verge.xSize)+ i ];

			Vector3 position = mesh.vertices[ (mesh.vertices.Length-verge.xSize)+ i -1 ];
			RaycastHit hit;
			if (Physics.Raycast(position, -Vector3.up, out hit, 1000.0f,myLayerMask3)) //layermask for testPlane?
			{
				//move themesh point to where it hits the Terrain Plane underneath
				tempArray[(mesh.vertices.Length-verge.xSize)+ i -1] = hit.point;
				//Debug.Log("hit");
				Transform item2 = Instantiate(cube) as Transform;
				item2.position = hit.point;
				yield return new WaitForFixedUpdate();
			}

		}


		mesh.vertices = tempArray;

	}
		IEnumerator MoveMesh()
		{
		//Deform Terrain under where road is placed// put this in coroutine
		//Mesh mesh = GetComponent<MeshFilter>().mesh;
		Mesh vergeMesh = vergeObject.GetComponent<MeshFilter>().mesh;
		Vector3[] tempArray;
		Vector3[] normalsTemp;
		tempArray = planeMesh.mesh.vertices;
		normalsTemp = planeMesh.mesh.normals;
		Debug.Log(tempArray.Length);
		Debug.Log(normalsTemp.Length);
		Debug.Log (planeMesh.mesh.triangles.Length);

			for (int i = 0; i <= verge.xSize; i++)
			{
				RaycastHit hit;
				Vector3 position = vergeMesh.vertices[ (vergeMesh.vertices.Length-verge.xSize)+ i -1 ];
				if (Physics.Raycast(position, -Vector3.up, out hit, 1000.0f,myLayerMask3))
				{
				Debug.Log("hit");

				Mesh mesh = planeMesh.mesh;
				Vector3[] vertices = mesh.vertices;
				int[] triangles = mesh.triangles;


				Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
				Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
				Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];

				p0 = position;
				p1 = position;
				p2 = position;

				tempArray[triangles[hit.triangleIndex * 3 + 0]] = p0;
				tempArray[triangles[hit.triangleIndex * 3 + 1]] = p1;
				tempArray[triangles[hit.triangleIndex * 3 + 2]] = p2;

				//normals
//				Vector3[] normals = mesh.normals;

				//Vector3 n0 = normals[triangles[hit.triangleIndex * 3 + 0]];
				//Vector3 n1 = normals[triangles[hit.triangleIndex * 3 + 1]];
				//Vector3 n2 = normals[triangles[hit.triangleIndex * 3 + 2]];

				//n0 = Vector3.up;

				//normalsTemp[triangles[hit.triangleIndex * 3 + 0]] = n0;
				//normalsTemp[triangles[hit.triangleIndex * 3 + 1]] = n1;
				//normalsTemp[triangles[hit.triangleIndex * 3 + 2]] = n2;
				//normalsTemp[triangles[hit.triangleIndex * 3 + 0]] = vergeMesh.normals[triangles[hit.triangleIndex * 3 + 0]];
				//normalsTemp[triangles[hit.triangleIndex * 3 + 1]] = vergeMesh.normals[triangles[hit.triangleIndex * 3 + 1]];
				//normalsTemp[triangles[hit.triangleIndex * 3 + 2]] = vergeMesh.normals[triangles[hit.triangleIndex * 3 + 2]];
			}
			}
		yield return new WaitForFixedUpdate();

		planeMesh.mesh.vertices = tempArray;
		//planeMesh.mesh.normals = normalsTemp;
		planeMesh.mesh.RecalculateNormals();
		}


}
