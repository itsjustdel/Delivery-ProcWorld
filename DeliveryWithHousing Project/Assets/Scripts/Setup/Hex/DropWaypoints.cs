using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropWaypoints : MonoBehaviour {

	public float rayWidth = 10;
	public float heightLimit = 1f;
	public MeshFilter meshFilter;
	public HexGridRectangle hexGridRectangle;
	public bool findNeighbours;
	// Use this for initialization
	void Start () {

		rayWidth = hexGridRectangle.hexSize + 1f;

		StartCoroutine("CreateNodeObjects");
	//	FindNeighbours();

	}

	void Update()
	{
		if(findNeighbours)
		{
			StartCoroutine("FindNeighbours");
			findNeighbours = false;
		}

	}

	IEnumerator CreateNodeObjects()
	{
		Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
//		if(mesh.name == "AutoWeldedMesh")
//			Debug.Log("welded mesh found!");
		LayerMask lm = LayerMask.GetMask("TerrainBase");

		for (int i = 0; i < mesh.vertexCount; i++)
		{
			GameObject node = new GameObject();

			node.name = "Node " + i.ToString();
			node.tag = "HexWaypoint";
			node.layer = 20;
			//hex grid builds on wrong axis, so its transform is spun 90 degrees, multiply it by it transform's rotation
			node.transform.position = meshFilter.transform.rotation*mesh.vertices[i];
			node.transform.position += this.transform.position;
		//	node.transform.position -= transform.parent.position;
			//now raycast for y position on terrain

			RaycastHit hit;
			if(Physics.Raycast(node.transform.position + (Vector3.up*1000),Vector3.down,out hit,2000f,lm))
			{

			node.transform.position = hit.point;
			}
		//	else
			//	Debug.Log("not hit");

			WaypointNode wpn = node.AddComponent<WaypointNode>();
			wpn.position = node.transform.position;
			wpn.isActive = true;

			BoxCollider box = node.AddComponent<BoxCollider>();
			Vector3 size = new Vector3(0.1f,0.1f,0.1f);
			box.size = size;

			node.transform.parent = this.transform;
			//yield return new WaitForFixedUpdate();
		}
		yield return new WaitForEndOfFrame();

		//once completed
		StartCoroutine("FindNeighbours");
		yield break;
	}

	public IEnumerator FindNeighbours()
	{
		WaypointNode[] nodes = transform.GetComponentsInChildren<WaypointNode>();
	//	Debug.Log(nodes.Length);
		LayerMask lm = LayerMask.GetMask("HexWaypoint");

		foreach(WaypointNode node in nodes)
		{
			
			//casts a thick ray of width 'rayWidth' downwards,looking for other waypoints
			RaycastHit[] results = Physics.SphereCastAll(node.transform.position + (heightLimit*Vector3.down*0.5f),
				rayWidth,
				Vector3.down,
				heightLimit,
				lm);

//			Debug.Log(results.Length);

			for (int j = 0; j < results.Length; j++)
			{

				if(results[j].transform != node.transform)
				{
						
						//check for height
					float heightDiff =  results[j].transform.position.y - node.transform.position.y;
					heightDiff = Mathf.Abs(heightDiff);
//					Debug.Log(heightDiff);
					if(heightDiff < heightLimit)
					{
					
						//add the 'hits' to the neighbours of this waypoint node	
						if(!results[j].transform.GetComponent<WaypointNode>().neighbors.Contains(node))	
							node.neighbors.Add(results[j].transform.GetComponent<WaypointNode>());
						//Now we must also add this waypoint we are working on to the neighbours of the waypoints we just hit
						//waypoints need a two way neighbour relationship, each on referencing the other
						//ony do this if the neighbour isn not in the list already
						if(!results[j].transform.GetComponent<WaypointNode>().neighbors.Contains(node))						
							results[j].transform.GetComponent<WaypointNode>().neighbors.Add(node);
					
					}
				}
			//	yield return new WaitForFixedUpdate();

			}

//			Debug.Log(results.Length);
		}

		yield break;
	}
}
