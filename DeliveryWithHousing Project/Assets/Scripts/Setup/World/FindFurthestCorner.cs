using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FindFurthestCorner : MonoBehaviour {
	public Transform roadPrefab;
	public Vector3 p1;
	public Vector3 p2;
	private int i1;
	private int i2;
	public int meshIndex1;
	public int meshIndex2;
	public List<int> bRoadPoints = new List<int>();
	private List<FindEdges.Edge> edgeList;
	public Transform cubePrefab;
	public List<StartAndEndOfBRoads> bRoadInfo = new List<StartAndEndOfBRoads>();
	public bool findStarts;
	// Use this for initialization
	void Awake()
	{
		//enabled = false;
	}
	void Start () 
	{
		edgeList = transform.GetComponent<FindEdges>().orderedEdges;
		//FindFurthestPointsWholeMesh();
		FindFurthestPointsFromEdgeList();
		ScanMeshForMainRoad();
		CreateBRoadPoints();
//		FindNearestPointsOnRingRoadMesh();  //Was using this to create B Roads across the polygon// needs hierarchy search change
//		Cubes();

	}
/*
	void Update()
	{
		if(findStarts)
		{
			FindNearestPointsOnRingRoadMesh();
			Cubes();
			findStarts = false;
		}

		if( gameObject.transform.FindChild("Aroad2(Clone)").FindChild("BRoad") !=null)
		{
		
			if(gameObject.transform.FindChild("Aroad2(Clone)").FindChild("BRoad").GetComponent<MeshFilter>().mesh != null)
			{
			FindNearestPointsOnRingRoadMesh();
			Debug.Log("finding starts");		
			}
		}
	}
*/

	void FindFurthestPointsFromEdgeList()
	{
		//this creates points for the main road of the area, which strethes across the polygon

		//Scans the EdgeList from <FindEdges> and finds the two furthest points from each other
	//	FindEdges.Edge[] edgeList = transform.GetComponent<FindEdges>().outline;
		Mesh polygonMesh = gameObject.GetComponent<MeshFilter>().mesh;

		float distance = 0;
		p1 = Vector3.zero;
		p2 = Vector3.zero;
		for (int i = 0; i <edgeList.Count; i++)
		{
			for (int j =0; j < edgeList.Count; j++)
			{
				Vector3 edgeP1 = polygonMesh.vertices[edgeList[i].vertexIndex[0]];
				Vector3 edgeP2 = polygonMesh.vertices[edgeList[j].vertexIndex[0]];
				//float diff = Vector3.Distance(polygonMesh.vertices[i],polygonMesh.vertices[j]);
				float diff = Vector3.Distance(edgeP1,edgeP2);
				if (diff > distance)
				{
					distance = diff;
					//p1 = polygonMesh.vertices[i];
					//p2 = polygonMesh.vertices[j];
					p1 = polygonMesh.vertices[edgeList[i].vertexIndex[0]];
					p2 = polygonMesh.vertices[edgeList[j].vertexIndex[0]];
					//bPoints use these
					i1 = i;
					i2 = j;
				}				
			}
		}
	}
	void ScanMeshForMainRoad()
	{
		//Now find the mesh vertices closest to these points
		
		Mesh ringRoadMesh = transform.Find("RingRoad").GetComponent<MeshFilter>().mesh;
		
		float distance = Mathf.Infinity;
		float distance2 = Mathf.Infinity;
		meshIndex1 = 0;
		meshIndex2 = 0;
		
		for (int i = 0; i < ringRoadMesh.vertexCount; i++)
		{
			float diff = Vector3.Distance(p1,ringRoadMesh.vertices[i]);
			if (diff < distance)
			{
				distance = diff;
				meshIndex1 = i;
			}
			
			float diff2 = Vector3.Distance(p2,ringRoadMesh.vertices[i]);
			if (diff2 < distance2)
			{
				distance2 = diff2;
				meshIndex2 = i;
			}	
		}
		/*
		Transform c1 = Instantiate(cubePrefab) as Transform;
		c1.position = ringRoadMesh.vertices[meshIndex1];
		c1.name ="Position 1 Cube";
		
		Transform c2 = Instantiate(cubePrefab) as Transform;
		c2.position = ringRoadMesh.vertices[meshIndex2];
		c2.name ="Position 2 Cube";
*/
	}

	void CreateBRoadPoints()
	{
		//Finds the points from which to make the smaller roads which join from the edge of the polygon 
		//to the main road of the area. (not the ring road)
	//	bRoadPoints = new List<int>();

		//make a list of points not used by the main road
		for (int i = 0; i < edgeList.Count; i++)
		{
			//if the point in the list does not equal p1 or p2, add to list
			//p1 and p2 are the points for the main road
			if ( edgeList[i].vertexIndex[0] != i1 || edgeList[i].vertexIndex[0] != i2)
				bRoadPoints.Add(edgeList[i].vertexIndex[0]);
		}
		//Now remove points which are too close to each other
		Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
		for (int i = 0; i < bRoadPoints.Count; i++)
		{
			for (int j = 0; j <bRoadPoints.Count; j++)
			{
				//checks one point against all others in list
				float distance =  Vector3.Distance(mesh.vertices[bRoadPoints[i]],mesh.vertices[bRoadPoints[j]]);
				//if it is too close, remove
				if (distance < 50)
					bRoadPoints.RemoveAt(j);
			}
		}

		cubePrefab = Resources.Load("Cube",typeof (Transform)) as Transform;
		foreach(int i in bRoadPoints)
		{
			Instantiate(cubePrefab,gameObject.GetComponent<MeshFilter>().mesh.vertices[i],Quaternion.identity);
		}

	}



	void FindNearestPointsOnRingRoadMesh()
	{

		List<int> nearestRingRoadIndices = new List<int>();
		List<Vector3> bRoadStarts = new List<Vector3>();
		List<Vector3> bRoadEnds = new List<Vector3>();
		Mesh ringRoadMesh = transform.Find("RingRoad").GetComponent<MeshFilter>().mesh;
		Mesh polygonMesh = gameObject.GetComponent<MeshFilter>().mesh;
		//go through the sorted BRoad starting points
		for (int i =0; i < bRoadPoints.Count; i++)
		{
			//look for the closest points on the ring road from the points on
			//the edges of the polygon
			float distance = Mathf.Infinity;
			Vector3 p1 = polygonMesh.vertices[bRoadPoints[i]];
			int closestInd = 0;

			for (int j = 0; j < ringRoadMesh.vertexCount; j++)
			{
				Vector3 p2 = ringRoadMesh.vertices[j];

				//compare the point on the edge to all of the ringroad's vertices
				float diff = Vector3.Distance(p1,p2);
				if (diff < distance)
				{
					distance = diff;
					closestInd = j;
				}
			}

			//add to a list of start positions
			bRoadStarts.Add(ringRoadMesh.vertices[closestInd]);

		}
		//now we have our points on the ring road, scan the main connecting road to
		//find the closest point to join to
		Mesh mainRoadMesh = transform.Find("Aroad2(Clone)").Find("BRoad").GetComponent<MeshFilter>().mesh;
		for (int i = 0; i < bRoadStarts.Count; i++)
		{
			float distance = Mathf.Infinity;
			Vector3 p1 = bRoadStarts[i];
			int closestInd = 0;

			for (int j = 0; j < mainRoadMesh.vertexCount; j++)
			{
				Vector3 p2 = mainRoadMesh.vertices[j];
				float diff = Vector3.Distance(p1,p2);
				if (diff < distance)
				{
					distance = diff;
					closestInd = j;
				}
			}
			Debug.Log(closestInd);
			//add to a list of end positions
			bRoadEnds.Add(mainRoadMesh.vertices[closestInd]);
		}

		//now enter these lists in to the bRoadInfo list
		for (int i = 0; i < bRoadStarts.Count; i++)
		{
			StartAndEndOfBRoads b = new StartAndEndOfBRoads();
			b.start = bRoadStarts[i];
			b.end = bRoadEnds[i];
			bRoadInfo.Add(b);
		}

		enabled = false;
		gameObject.AddComponent<PlaceBRoads>();
	}

	void FindFurthestPointsWholeMesh()
	{
		
		//		roadPrefab = Resources.Load("Prefabs/Setup/Aroad2",typeof (Transform)) as Transform;
		Mesh polygonMesh = gameObject.GetComponent<MeshFilter>().mesh;
		
		//find furthest points from each other
		
		float distance = 0;
		p1 = Vector3.zero;
		p2 = Vector3.zero;
		for (int i = 0; i <polygonMesh.vertexCount; i++)
		{
			for (int j =0; j < polygonMesh.vertexCount; j++)
			{
				float diff = Vector3.Distance(polygonMesh.vertices[i],polygonMesh.vertices[j]);
				
				if (diff > distance)
				{
					distance = diff;
					p1 = polygonMesh.vertices[i];
					p2 = polygonMesh.vertices[j];
				}
				
			}
		}
	}


	void Cubes()
	{
		cubePrefab = Resources.Load("Cube",typeof (Transform)) as Transform;

		for (int i = 0; i < bRoadInfo.Count; i++)
		{
			Transform startCube = 	Instantiate(cubePrefab,bRoadInfo[i].start,Quaternion.identity) as Transform;
			startCube.name = "Start Cube";
			Transform endCube = 	Instantiate(cubePrefab,bRoadInfo[i].end,Quaternion.identity)as Transform;
			endCube.name = "EndCube";
		}
	}

	public class StartAndEndOfBRoads
	{
		public Vector3 start = new Vector3();
		public Vector3 end = new Vector3();
	}

}
