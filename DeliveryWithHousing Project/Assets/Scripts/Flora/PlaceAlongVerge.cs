using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceAlongVerge : MonoBehaviour {
	public float HouseDensity = 300f;
	public float treeFrequency = 20f;
	public float bushFrequency = 3f;
	private BezierSpline bezierL;
	private BezierSpline bezierR;
	// Use this for initialization
	public float frequency = 1000;
	public KerbMesh kerbMesh;
	//public SplineMesh splineMeshScript;
	public bool placeBush = false;
	public bool placeTree = false;
	public bool placeHouse = true;
	public Transform bushPrefab;
	public Transform treePrefab;
	public Transform housePrefab;
	public bool isConnectingRoad;
	public float frequencyMultiplier= 0.5f;

	private MeshGenerator meshGenerator;
	void Start() 
	{	

		meshGenerator = GameObject.FindWithTag("VoronoiMesh").GetComponent<MeshGenerator>();

		housePrefab = Resources.Load("Prefabs/Setup/House No Road", typeof (Transform)) as Transform;

		List<Vector3> tempListLeft = new List<Vector3>();
		List<Vector3> tempListRight = new List<Vector3>();

		//grab points from last row of the verge mesh

//		if (!isConnectingRoad)
//		{
//			tempListLeft = GameObject.FindWithTag("Verge").GetComponent<Verge>().lastRowListLeft;
//			tempListRight = GameObject.FindWithTag("Verge").GetComponent<Verge>().lastRowListRight;
		//
		//	frequency = GameObject.FindWithTag("Road Dresser").GetComponent<SplineMesh>().frequency*frequencyMultiplier;

//		} else if (isConnectingRoad)
//		{
			tempListLeft = gameObject.GetComponent<Verge>().lastRowListLeft;
			tempListRight = gameObject.GetComponent<Verge>().lastRowListRight;

			tempListLeft.RemoveRange(0,tempListLeft.Count/10);
			tempListRight.RemoveRange(0,tempListRight.Count/10);

			//frequency = transform.GetComponent<BRoadMesher>().frequency *0.25f;
		frequency = 100;
//		}


		bezierL = gameObject.AddComponent<BezierSpline>();
		bezierL.points = tempListLeft.ToArray();
		bezierR = gameObject.AddComponent<BezierSpline>();
		bezierR.points = tempListRight.ToArray();

		//bezierR = kerbMesh.splineR;

		if (placeBush)
		{
	//		StartCoroutine("PlaceBushLeft");
	//		StartCoroutine("PlaceBushRight");
		}

		if (placeTree)
		{
	//		StartCoroutine("PlaceTreeLeft");
	//		StartCoroutine("PlaceTreeRight");
		}

		if (placeHouse)
		{
			StartCoroutine("PlaceHousesLeft");
			StartCoroutine("PlaceHousesRight");
		}

	}

	IEnumerator PlaceBushLeft ()
	{
				
		for (float i = 0; i <=frequency;i+= Random.Range(1f,bushFrequency))
		{
			Vector3 point = bezierL.GetPoint(i/(frequency-1));
			Vector3 direction = bezierL.GetDirection (i/(frequency-1));
			Vector3 rotationDir =  Quaternion.Euler (0, -90, 0) * (direction); 
			float random = Random.Range(0.2f,1f);

				
			LayerMask myLayerMask = LayerMask.GetMask("Road","Building","TerrainPlane");
			RaycastHit hit;
			if (Physics.Raycast(point + (Vector3.up*500),Vector3.down,out hit,1000f,myLayerMask))
			{
				if (hit.collider.tag == "Road" || hit.collider.tag == "Building" || hit.collider.tag == "Pavement")
				{
				Debug.Log("Bush Hit Road");//if it hits the road skip 
					continue;
				}
				point = hit.point;
			}

			Transform bush = Instantiate(bushPrefab) as Transform;

			bush.position = point + (rotationDir*random) - transform.position;
			bush.position += Vector3.up*0.5f;

			Rigidbody rb = bush.gameObject.AddComponent<Rigidbody>();
			rb.constraints = RigidbodyConstraints.FreezeAll;


				//put in asign to colour array

				//colliders?
			yield return new WaitForFixedUpdate();
		}

	}

	IEnumerator PlaceBushRight ()
	{
		
		for (float i = 0; i <=frequency;i+= Random.Range(1f,bushFrequency))
		{
			Vector3 point = bezierR.GetPoint(i/(frequency-1));
			Vector3 direction = bezierR.GetDirection (i/(frequency-1));
			Vector3 rotationDir =  Quaternion.Euler (0, 90, 0) * (direction);
			float random = Random.Range(0.2f,1f);
			LayerMask myLayerMask = LayerMask.GetMask("Road","Building","TerrainPlane");
			RaycastHit hit;
			if (Physics.Raycast(point + (Vector3.up*500),Vector3.down,out hit,1000f,myLayerMask))		
			{
				if (hit.collider.tag == "Road" || hit.collider.tag == "Building" || hit.collider.tag == "Pavement")
				{
					Debug.Log("Bush Hit Road");
					continue;
				}
				else if (hit.collider.tag == "TerrainBase")
				{
					point = hit.point;
				//	Debug.Log("Bush Hit Terrain Base");
				}
			}
			
			
			Transform bush = Instantiate(bushPrefab) as Transform;
			bush.position = point + (rotationDir*random) - transform.position;
			bush.position += Vector3.up*0.5f;
			Rigidbody rb = bush.gameObject.AddComponent<Rigidbody>();
			rb.constraints = RigidbodyConstraints.FreezeAll;
			//rb.constraints = RigidbodyConstraints.FreezeRotation;
			//put in asign to colour array
			
			//colliders?
			yield return new WaitForFixedUpdate();
		}
		
	}

	IEnumerator PlaceTreeLeft ()
	{
		
		for (float i = 0; i <=frequency;i+= Random.Range(1f,treeFrequency))
		{
			Vector3 point = bezierL.GetPoint(i/(frequency-1));
			Vector3 direction = bezierL.GetDirection (i/(frequency-1));
			Vector3 rotationDir =  Quaternion.Euler (0, -90, 0) * (direction);
			float random = Random.Range(0.5f,3f);


			LayerMask myLayerMask = LayerMask.GetMask("Road","Building");//,"TerrainBase");
			RaycastHit hit;
			if (Physics.Raycast(point + (Vector3.up*500),Vector3.down,out hit,1000f,myLayerMask))		
			{
				if (hit.collider.tag == "Road" || hit.collider.tag == "Building" || hit.collider.tag == "Pavement")
				{
				//	Debug.Log("Tree Hit Road");
					continue;
				}
				else if (hit.collider.tag == "TerrainBase")
				{
					point = hit.point;
				//	Debug.Log("Tree Hit Terrain Base");
				}
			}
			point.y -= 0.1f; //half the collider size
			Transform tree = Instantiate(treePrefab) as Transform;
			tree.position = point + (rotationDir*random) - transform.position;
			Rigidbody rb = tree.gameObject.AddComponent<Rigidbody>();
			rb.constraints = RigidbodyConstraints.FreezeAll;
			yield return new WaitForFixedUpdate();
		}		
	
	}

	IEnumerator PlaceTreeRight ()
	{
		
		for (float i = 0; i <=frequency;i+= Random.Range(1f,treeFrequency))
		{
			Vector3 point = bezierR.GetPoint(i/(frequency-1));
			Vector3 direction = bezierR.GetDirection (i/(frequency-1));
			Vector3 rotationDir =  Quaternion.Euler (0, 90, 0) * (direction);
			float random = Random.Range(0.5f,3f);

			
			LayerMask myLayerMask = LayerMask.GetMask("Road","Building");//,"TerrainBase");
			RaycastHit hit;
			if (Physics.Raycast(point + (Vector3.up*500),Vector3.down,out hit,1000f,myLayerMask))		
			{
				if (hit.collider.tag == "Road" || hit.collider.tag == "Building" || hit.collider.tag == "Pavement")
				{
				//	Debug.Log("Tree Hit Road");
					continue;
				}
				else if (hit.collider.tag == "TerrainBase")
				{
					point = hit.point;
				//	Debug.Log("Tree Hit Terrain Base");
				}
			}
			point.y -= 0.1f; //half the collider size
			Transform tree = Instantiate(treePrefab) as Transform;
			tree.position = point + (rotationDir*random) - transform.position;
			Rigidbody rb = tree.gameObject.AddComponent<Rigidbody>();
			rb.constraints = RigidbodyConstraints.FreezeAll;
			yield return new WaitForFixedUpdate();
		}		
		
	}

	IEnumerator PlaceHousesRight ()
	{
		for (float i = 0; i <=frequency;i+= Random.Range(1f,HouseDensity))
		{
			Vector3 point = bezierR.GetPoint(i/(frequency-1));
			Vector3 direction = bezierR.GetDirection (i/(frequency-1));
			Vector3 rotationDir =  Quaternion.Euler (0, 90, 0) * (direction);
			float random = Random.Range(0.5f,5f);

			LayerMask myLayerMask = LayerMask.GetMask("Road","Building");//,"TerrainBase");
			RaycastHit hit;
			if (Physics.Raycast(point + (Vector3.up*500),Vector3.down,out hit,1000f,myLayerMask))		
			{
				if (hit.collider.tag == "Road" || hit.collider.tag == "Building")
				{
					//	Debug.Log("Tree Hit Road");
					continue;
				}
				else if (hit.collider.tag == "TerrainBase")
				{
					point = hit.point;
					//	Debug.Log("Tree Hit Terrain Base");
				}
			}
			point.y -= 0.1f; //half the collider size

			//Instantiate house if ray never hitroad or building
			Transform house = Instantiate(housePrefab) as Transform;
			house.position = point + (rotationDir*random) - transform.position;

			//spin the house to look at road
			Quaternion lookRot = Quaternion.LookRotation(direction);
			lookRot*= Quaternion.Euler(0f,90f,0f);
			house.rotation = lookRot;

			//Add point to the voronoi mesh
			//The building point will be added when AddToVoronoi script checks for all buildings
			//This is neeeded due to the way the backyards are entered in to the voronoi (from closest Ind on TJunction)
			//nothing to do with curve

			//all points added to voronoi need to be y=0, mesh fails otherwise
			//y adjustment happens after the cells are built

			//add points to create yard
			point += rotationDir*50;
			point -= transform.position;
			point.y = 0;
			//meshGenerator.yardPoints.Add(point);

			//tell this script this mesh indice position so that if the building does not get destroyed on setup
			//it can pass its position (mesh and vertice position) to the voronoi mesh script
			//rotation
		//	house.FindChild("Cube").GetComponent<ColliderCheck>().ind = newInd;
		//	houseFindChild("Cube").GetComponent<ColliderCheck>().cell = ihouse.gameObject;

			
		}

		yield return new WaitForFixedUpdate();
	}

	IEnumerator PlaceHousesLeft ()
	{
		for (float i = 0; i <=frequency;i+= Random.Range(1f,HouseDensity))
		{
			Vector3 point = bezierL.GetPoint(i/(frequency-1));
			Vector3 direction = bezierL.GetDirection (i/(frequency-1));
			Vector3 rotationDir =  Quaternion.Euler (0, -90, 0) * (direction);
			float random = Random.Range(0.5f,5f);

			LayerMask myLayerMask = LayerMask.GetMask("Road","Building");//,"TerrainBase");
			RaycastHit hit;
			if (Physics.Raycast(point + (Vector3.up*500),Vector3.down,out hit,1000f,myLayerMask))		
			{
				if (hit.collider.tag == "Road" || hit.collider.tag == "Building")
				{
					//	Debug.Log("Tree Hit Road");
					continue;
				}
				else if (hit.collider.tag == "TerrainBase")
				{
					point = hit.point;
					//	Debug.Log("Tree Hit Terrain Base");
				}
			}
			point.y -= 0.1f; //half the collider size

			//Instantiate house if ray never hitroad or building
			Transform house = Instantiate(housePrefab) as Transform;
			house.position = point + (rotationDir*random) - transform.position;
			
			//spin the house to look at road
			Quaternion lookRot = Quaternion.LookRotation(direction);
			lookRot*= Quaternion.Euler(0f,90f,0f);
			house.rotation = lookRot;
			
			//Add point to the voronoi mesh
			//The building point will be added when AddToVoronoi script checks for all buildings
			//This is neeeded due to the way the backyards are entered in to the voronoi (from closest Ind on TJunction)
			//nothing to do with curve
			
			//all points added to voronoi need to be y=0, mesh fails otherwise
			//y adjustment happens after the cells are built
			
			//add points to create yard
			point += rotationDir*50;
			point -= transform.position;
			point.y = 0;
			meshGenerator.yardPoints.Add(point);
		}
		
		yield return new WaitForFixedUpdate();
	}
}