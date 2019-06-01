using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System;

public class RoadLines : ProcBase {
	public float lineWidth;
	public float lineLength;
	public SplineMesh splineMesh;
	//public KerbInjector kerbInjector;
	public BezierSpline splineL;
	public BezierSpline splineR;
	public Transform cube;
	// Use this for initialization
	void Start () {


		LinesFromCurves();
	}

	void LinesFromCurves()
	{


		float frequency = splineMesh.frequency;
		float stepSize = frequency;
		stepSize = 2f / (stepSize -1);
	
	
		for (int k = 1, l =0; k <= frequency; k++, l+=10)
		{
			BezierSpline splineM = gameObject.AddComponent<BezierSpline>();	
			splineM = splineMesh.spline;
			
			splineL = gameObject.AddComponent<BezierSpline>();
			List<Vector3> splineLList = new List<Vector3>();
			List<BezierControlPointMode> splineLControlPointsList = new List<BezierControlPointMode>();
			
			splineR = gameObject.AddComponent<BezierSpline>();
			List<Vector3> splineRList = new List<Vector3>();
			List<BezierControlPointMode> splineRControlPointsList = new List<BezierControlPointMode>();
			
			List<Vector3> vertices = new List<Vector3>();
			List<int> indices = new List<int>();
			for (int f = 1; f <= lineLength; f++) //needs to be one because 0 * stepSize doestn return usable value
			{ 
				
				Vector3 position = splineM.GetPoint((f+k+l) * stepSize);
				Vector3 direction = splineM.GetDirection((f+k+l)*stepSize);
				Vector3 rot1 = Quaternion.Euler(0,-90,0) * direction; 
				Vector3 rot2 = Quaternion.Euler(0,90,0) * direction; 
				rot1*=lineWidth;
				rot2*=lineWidth;
				Vector3 offset1 = rot1+position;
				Vector3 offset2 = rot2+position;
				
				
//				Vector3 ray1 = position+(rot1*lineWidth);
//				Vector3 ray2 = position+(rot2*lineWidth);
				//check for where the terrain is firing a ray at it, the hit point becomes the new Y co-ordinate
				//RaycastHit hit;
				
				//	Transform item = Instantiate(cube) as Transform;
				//	item.position = offset1 - transform.position;
				//	item.name = "splineMesh Cube" + f;
				
				splineLList.Add(offset1 - transform.position);
				splineLControlPointsList.Add(BezierControlPointMode.Mirrored);
				
				splineRList.Add(offset2 - transform.position);
				splineRControlPointsList.Add(BezierControlPointMode.Mirrored);
				
			}
			
			//set the temp list we just filled up to the instantiated beziers
			//also adds the corresponding control point modes to be referenced by the curve
			splineL.points = splineLList.ToArray();
			splineL.modes = splineLControlPointsList.ToArray();
			splineR.points = splineRList.ToArray();
			splineR.modes = splineLControlPointsList.ToArray();
			
			
			//iterate through bezier and add points to mesh vertice temp List "vertices"
			
			for (int i = 1; i <= lineLength; i++)
			{
				Vector3 position = splineL.GetPoint(i * stepSize);
				vertices.Add(position);
				Vector3 position2 = splineR.GetPoint(i * stepSize);
				vertices.Add(position2);
				//	Transform item = Instantiate(cube) as Transform;
				//	item.position = position + transform.position;
			}

			GameObject child = new GameObject();
			child.name = "Lines";
			child.transform.position = this.gameObject.transform.position;
			child.transform.parent = this.gameObject.transform;
			child.layer = 9;
			//child.tag = "Lines";
			
		//	MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
			MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();
			
			Mesh mesh = new Mesh();	
			mesh.vertices = vertices.ToArray();
			
			
			
			for ( int i = 0; i <= lineLength; i+=2)
			{
				indices.Add(i+2);
				indices.Add(i+1);
				indices.Add(i);	
				indices.Add(i+3);
				indices.Add(i+1);
				indices.Add(i+2);
			}
			
			mesh.triangles = indices.ToArray();
			//		mesh.triangles = triangles;
			mesh.RecalculateBounds();
			//meshCollider.sharedMesh = mesh;
			mesh.RecalculateNormals();
			meshFilter.sharedMesh = mesh;
			
			Material newMat = Resources.Load("White",typeof(Material)) as Material;		
			meshRenderer.material = newMat;
		}
	}
}