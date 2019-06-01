using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class BRoadPlotter : Pathfinding  {
	public BRoadMesher bRoadMesher;
	//public BezierSpline bezier;
	public Vector3 target ;
	public Mesh mesh;
	public Transform cube;
	public bool placeCubes ;
	public bool findPoint;
	public bool mainRoad;
	// Use this for initialization
	void Start () {

		if (mainRoad){
		//find closest point on aroad mesh to p1
		Mesh mesh = transform.parent.transform.Find("RingRoad").GetComponent<MeshFilter>().mesh;
		Vector3 p1 = this.transform.parent.GetComponent<FindFurthestCorner>().p1;

		//set the transform position and thus the start of the road plotter to the closest point on the mesh 
		//this has already been worked out for us in FindFurthestCorner
		int closest = transform.parent.GetComponent<FindFurthestCorner>().meshIndex1;
		transform.position = mesh.vertices[closest];

		int closest2 = transform.parent.GetComponent<FindFurthestCorner>().meshIndex2;
		target = mesh.vertices[closest2];	

			FindPoint();
		}

		if(!mainRoad)
		{
			FindPoint();
		}


		Debug.Log("go");

	}

	void Update(){
		if (placeCubes){
			PlaceCubesOnCurve();
			placeCubes =false;
			Transform item = Instantiate(cube) as Transform;
			item.position = transform.position;
			item.name = "p1";
		}
		if (findPoint){
			FindPoint();
			findPoint = false;
		}
	}

	public static int FindClosestPoint(int targetIndex, Mesh mesh, List<Vector3> list)
	{
		
		int closest = 0;
		float minDistanceSqr = Mathf.Infinity;
		//could search the A Road Plotter's array instead
		for (int i = 0; i < mesh.vertices.Length; i++)
		{
			
			Vector3 midPoint = list[targetIndex];
			
			Vector3 diff = midPoint-mesh.vertices[i];
			float distSqr = diff.sqrMagnitude;
			
			if (distSqr < minDistanceSqr)
			{
				minDistanceSqr = distSqr;
				
				closest = i ;
				
			}			
		}		
		return closest;		
	}

	private void FindPoint()
	{
		FindPath(transform.position,target);
	}


	void PlaceCubesOnCurve()
	{
		Debug.Log(Path.Count);
	
		for (int i = 0; i<Path.Count; i++)	
		{
			Transform item = Instantiate(cube) as Transform;
			item.position = Path[i];

		}
	}

}
