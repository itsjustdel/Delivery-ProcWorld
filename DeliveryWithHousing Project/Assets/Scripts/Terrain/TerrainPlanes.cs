using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TerrainPlanes : MonoBehaviour {
	public bool isJoiningRoad;
	public int xSize;
	public int ySize;
	public float scaleAmt = 0.1f;
	public float planeFrequency = 100;
	
	public LayerMask myLayerMask;
	public LayerMask myLayerMask2;
	public LayerMask myLayerMask3;
	private BezierSpline roadCurve;

	public bool scriptFinished = false;
	public float frequencyMultiplier= 0.5f;

	private int mapSize = 1000;

	// Use this for initialization
	void Start () {

		roadCurve = gameObject.AddComponent<BezierSpline>();

		if(!isJoiningRoad)
		{
			roadCurve = GameObject.FindWithTag("Road Dresser").GetComponent<SplineMesh>().spline;
			planeFrequency = GameObject.FindWithTag("Road Dresser").GetComponent<SplineMesh>().frequency*frequencyMultiplier;
		}

		if(isJoiningRoad)
		{
			roadCurve = transform.GetComponent<BezierSpline>();
			planeFrequency = transform.GetComponent<BRoadMesher>().frequency *0.25f;
		}

		//StartCoroutine("BuildMeshAtRoadPointsV2");
		StartCoroutine("PlaceGameObjects");


	}

	IEnumerator PlaceGameObjects()
	{
		float stepSize = planeFrequency;
		stepSize =1f/(planeFrequency-1);
		
		
		
		for (int j = 0; j < planeFrequency;j+=10)
		{
			Vector3 pPos= roadCurve.GetPoint( j * stepSize);
			Vector3 pDir= roadCurve.GetDirection( j*stepSize);
			pDir.Normalize();
			Vector3 pDir2 = Quaternion.Euler (0, -90, 0)*pDir;
			
			Vector3 modifier = -(pDir2)*10;  //not sure why ten is correct at the moment//maybe because scale amount is 0.1 in inspector
			pPos = pPos  + modifier - transform.position;		

			GameObject terrainPlane = new GameObject();
			terrainPlane.transform.parent = this.gameObject.transform;
			terrainPlane.name = "Terrain Plane L";
			terrainPlane.layer = 12;
			
			//add distance enablerscript to the object
			//DistanceEnable distanceEnable =	terrainPlane.AddComponent<DistanceEnable>();

			SingleTerrainPlane singleTerrainPlane = terrainPlane.AddComponent<SingleTerrainPlane>();
			singleTerrainPlane.enabled = false;
			//add components to this object
			singleTerrainPlane.gameObject.AddComponent<MeshFilter>();
			singleTerrainPlane.gameObject.AddComponent<MeshRenderer>();
			singleTerrainPlane.gameObject.AddComponent<MeshCollider>();

			//change tag to "TerrainPlane"
			singleTerrainPlane.gameObject.tag = "TerrainPlane";

			//singleTerrainPlane.enabled = false;
			singleTerrainPlane.roadCurve = roadCurve;
			singleTerrainPlane.pPos = pPos;
			singleTerrainPlane.pDir = pDir;
			singleTerrainPlane.pDir2 = pDir2;


			//add box collider for activating

			BoxCollider boxCollider = singleTerrainPlane.gameObject.AddComponent<BoxCollider>();
			boxCollider.center = pPos;
			boxCollider.isTrigger = true;
			//yield return new WaitForFixedUpdate();
		}

			
			//yield return new WaitForFixedUpdate();
	
	

		scriptFinished = true;

		yield break;

	}

	IEnumerator BuildMeshAtRoadPointsV2()
	{
		float stepSize = planeFrequency;
		stepSize =1f/(planeFrequency-1);

	

		for (int j = 0; j < planeFrequency;j+=10)
		{
			Vector3 pPos= roadCurve.GetPoint( j * stepSize);
			Vector3 pDir= roadCurve.GetDirection( j*stepSize);
			pDir.Normalize();
			Vector3 pDir2 = Quaternion.Euler (0, -90, 0)*pDir;
	
			Vector3 modifier = -(pDir2)*10;  //not sure why ten is correct at the moment//maybe because scale amount is 0.1 in inspector
			Vector3 planePos = pPos  + modifier - transform.position;
			StartCoroutine(BuildPlane(planePos,pDir,pDir2));
			//BuildPlane(planePos,pDir,pDir2);
			yield return new WaitForFixedUpdate();
		}

		scriptFinished = true;

	}
	

	IEnumerator BuildMeshInGrid()
	{
	
		for (float j = 0; j < mapSize;j+=200*scaleAmt)
		{
			for (float k = 0; k < mapSize;k+=200*scaleAmt)
			{
				Vector3 planePos = new Vector3(j,0f,k);
				StartCoroutine("BuildPlane",(planePos));
				yield return new WaitForSeconds(0.1f);
			}
		}
		


	}

	IEnumerator BuildMeshAtRoadPoints() 
	{
		float stepSize = planeFrequency;
		stepSize =1f/(planeFrequency-1);

		for (int j = 50; j < planeFrequency;j+=50)
		{
//			Vector3 pPos= roadCurve.GetPoint( j * stepSize);
			Vector3 pDir= roadCurve.GetDirection( j*stepSize);
			pDir = Quaternion.Euler (0, -90, 0)*pDir;
			pDir *=50;
//			Vector3 planePos = pPos+pDir;
//			StartCoroutine(BuildPlane(planePos,pDir));
			//yield return new WaitForFixedUpdate();
		}

		for (int j = 50; j < planeFrequency;j+=50)
		{
			Vector3 pPos= roadCurve.GetPoint( j * stepSize);
			Vector3 pDir= roadCurve.GetDirection( j*stepSize);
			pDir = Quaternion.Euler (0, 90, 0)*pDir;
			pDir *=100;
			Vector3 planePos = pPos+pDir;
			StartCoroutine("BuildPlane",(planePos));
			//yield return new WaitForFixedUpdate();
		}

		yield break;
	}

		IEnumerator BuildPlane(Vector3 planePos, Vector3 direction, Vector3 direction2)
		{
			Mesh mesh = new Mesh();
			List<Vector3> tempList = new List<Vector3>();
			
			//Vector3 yAdd = new Vector3(0f,500f,0f);
			
			for (int r = 0; r <=  xSize ;r++)//how deep each section is
			{
				for (int i = 0; i <=ySize;i++)//how long each section is// Change += to change the density level
				{
					
					//Vector3 position = bezier.GetPoint((i)/(frequency-1));
					//Vector3 direction = bezier.GetDirection (i/(frequency-1));
					//float random = Random.Range(0.5f,1.5f);
//					Vector3 pAndR = Vector3.zero;
//					Vector3 rot1 = Vector3.zero;

					//build grid x and y
					//Vector3 x = Vector3.right * i * scaleAmt;
					//Vector3 y = Vector3.forward * r * scaleAmt;
					
					Vector3 x = direction * i * scaleAmt ;
					Vector3 y = direction2 * r * scaleAmt ;
					

					Vector3 position = planePos + x + y;				
					
					
					RaycastHit hit;

					//this should look for road first, if not hit on road, look for roadSkirt, if no hit, terrainBase
					
					if (Physics.Raycast(position + transform.position + (Vector3.up*500), -Vector3.up, out hit, 1000.0f,myLayerMask))
					{
						
					//	if (hit.collider.gameObject.tag == "Road")
					//	{
							tempList.Add(hit.point);// + Vector3.down*10);
							//	Debug.Log("road");
					//	}	


					}

					else if (Physics.Raycast(position + transform.position + (Vector3.up*500), -Vector3.up, out hit, 1000.0f,myLayerMask2))
					{
					//	if (hit.collider.gameObject.tag == ("TerrainSecondLayer"))
					//	{
							tempList.Add(hit.point);// + Vector3.down*10);
							//	Debug.Log("t2ndLayer");
					//yield return new WaitForFixedUpdate();	
					//	}
					}
					
					else if (Physics.Raycast(position + transform.position + (Vector3.up*500), -Vector3.up, out hit, 1000.0F,myLayerMask3))
					{
						
						tempList.Add(hit.point);
						//Debug.Log("TBaseLayer");
						
					//yield return new WaitForFixedUpdate();
						
					}


				else 
					tempList.Add(position);

					//yield return new WaitForFixedUpdate();

					//Debug.Log("never hit");


				}
			yield return new WaitForFixedUpdate();
			}
			
		Debug.Log("got there");
			
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
			
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			;
			
			GameObject terrainPlane = new GameObject();
			terrainPlane.transform.parent = this.gameObject.transform;
			terrainPlane.name = "Terrain Plane";
			terrainPlane.layer = 12;

			//add distance enablerscript to the object
			
			terrainPlane.AddComponent<DistanceEnable>();
			
			MeshFilter meshFilterInstance = terrainPlane.gameObject.AddComponent<MeshFilter>();
			meshFilterInstance.mesh = mesh;
			MeshRenderer meshRenderer = terrainPlane.gameObject.AddComponent<MeshRenderer>();
			meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
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
		//yield return new WaitForSeconds(0.1f);

		yield break;
		}
		
}
