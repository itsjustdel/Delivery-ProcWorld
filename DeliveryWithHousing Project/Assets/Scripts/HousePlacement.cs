using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// House placement. Also finds the nearst point on existing road to create road to
/// </summary>
public class HousePlacement : MonoBehaviour {

	//public List<Vector3> aRoad;

	public Transform cube;
	public Pathfinder pathfinder;
	public Mesh mesh;
	public Vector3 closestPoint = Vector3.zero;
	public List<Vector3> path = new List<Vector3>();
	void Start()
	{
		PlaceHouse();
		FindClosestPathFinderPoint();
		//FindClosestPointInMesh();

	}

	void PlaceHouse()
	{
		RaycastHit hit;
		if (Physics.Raycast(transform.position + Vector3.up*500, -Vector3.up, out hit, 1000.0F))
			transform.position = hit.point;

		//Transform item = Instantiate(cube) as Transform;
		//item.position = hit.point;
		//item.name = "housePos";
	}

	void FindClosestPathFinderPoint()
	{
		//Find A Road's plotter

		path = GameObject.FindGameObjectWithTag("Road Dresser").GetComponent<CurveInjector>().list;

		//search for the closest point

		float minDistanceSqr = Mathf.Infinity;
	
		
		for (int i = 0; i <path.Count; i++)
		{
			Vector3 diff = transform.position-path[i];
			float distSqr = diff.sqrMagnitude;
			
			if (distSqr < minDistanceSqr)
			{
				minDistanceSqr = distSqr;
				closestPoint = path[i];
				
			}			
		}		
		// Finds the road plotter script and gives it the target vector3
		gameObject.GetComponent<BRoadPlotter>().target = closestPoint;

	}

	void FindClosestPointInMesh()
	{
		mesh = GameObject.Find("ARoad").GetComponent<MeshFilter>().mesh;
		
		
		//Scan for terrain and put house on top using only the Y coordinate

		
		//find nearest point on A road
		float minDistanceSqr = Mathf.Infinity;

		//		Vector3 nextVertex = Vector3.zero;
		//		Vector3 previousVertex = Vector3.zero;
		//		BRoadMesher br = gameObject.GetComponent<BRoadMesher> ();
		// scan all vertices to find nearest ==slow if large frequency of road (coroutine?)
		
		for (int i = 0; i < mesh.vertices.Length; i++)
		{
			Vector3 diff = transform.position-mesh.vertices[i];
			float distSqr = diff.sqrMagnitude;
			
			if (distSqr < minDistanceSqr)
			{
				minDistanceSqr = distSqr;
				closestPoint = mesh.vertices[i];
				
			}
			
		}

		
		// Finds the road plotter script and gives it the target vector3
		gameObject.GetComponent<BRoadPlotter>().target = closestPoint;

		
		
		this.enabled = false;
	}


}
