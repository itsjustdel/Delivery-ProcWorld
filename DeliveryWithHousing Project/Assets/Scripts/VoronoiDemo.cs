using UnityEngine;
using System.Collections.Generic;
using Delaunay;
using Delaunay.Geo;
using Delaunay.Utils;

public class VoronoiDemo : MonoBehaviour
{
	public Transform cube;
	public bool dumpCubes;
	public bool dumpCubesOnCurve;
	public SplineMesh splineMesh;
	public BezierSpline bezier;

	//public BezierSpline bezier3;
	public float frequency;
	[SerializeField]
//	private int	m_pointCount = 300;

	private List<Vector2> m_points;
	private float m_mapWidth = 1000;
	private float m_mapHeight = 1000;
	private List<LineSegment> m_edges = null;
//	private List<LineSegment> m_spanningTree;
	private List<LineSegment> m_delaunayTriangulation;

	void Start ()
	{
		bezier = splineMesh.spline;

//		Demo ();
	}

	void Update ()
	{
	//	if (Input.anyKeyDown) {
	//		Demo ();
	//	}
		if (dumpCubes){
			DumpCubes();
			dumpCubes = false;
		}
		if (dumpCubesOnCurve){
			DumpCubesOnCurve();
			dumpCubesOnCurve = false;
		}
	}
/*
	private void Demo ()
	{

				
		List<uint> colors = new List<uint> ();
		m_points = new List<Vector2> ();
			

		for (int i = 1; i <= m_pointCount; i++) {
			colors.Add (0);
			float stepSize = frequency;
			stepSize =2f / (stepSize -1);
			if (i<frequency)
			{
				//m_points.Add (new Vector2 ( ////AddCurve points here 
				//	i * 1f * Random.Range(0.99f,1.01f),
				//	i));
				Vector3 position = bezier.GetPoint(i*stepSize);
				m_points.Add ( new Vector2 (
					position.x,position.z));

//				Transform item = Instantiate(cube) as Transform;
//				item.position = position;
//				item.name = "left";
//
			}


			if (i>=frequency)
			{ 
				m_points.Add (new Vector2 (
				UnityEngine.Random.Range (0, m_mapWidth),
				UnityEngine.Random.Range (0, m_mapHeight))
				);


			}
		}
		Delaunay.Voronoi v = new Delaunay.Voronoi (m_points, colors, new Rect (0, 0, m_mapWidth, m_mapHeight));

		m_edges = v.VoronoiDiagram ();
		m_spanningTree = v.SpanningTree (KruskalType.MINIMUM);
		m_delaunayTriangulation = v.DelaunayTriangulation ();

	}
*/
	void DumpCubes()
	{

	//	for (int i = 0; i< m_edges.Count; i++) {
	//		Vector2 left = (Vector2)m_edges [i].p0;
//			Vector2 right = (Vector2)m_edges [i].p1;
	//		Transform item = Instantiate(cube) as Transform;
	//		cube.position = (Vector3)left;

			//cube.position = (Vector3)right;
	//	}
	}
	void DumpCubesOnCurve()
	{
//		float stepSize = frequency;
//		stepSize =2f / (stepSize -1);

//		for (int i = 1; i <= frequency; i++)
//		{		
//				Vector3 position = bezier.GetPoint(i * stepSize);
//				Transform item = Instantiate(cube) as Transform;
//				cube.position = position;
//				cube.name = "CubeOnCurve";
//		}
	}
	void OnDrawGizmos ()
	{
		Gizmos.color = Color.red;
		if (m_points != null) {
			for (int i = 0; i < m_points.Count; i++) {
				Gizmos.DrawSphere (m_points [i], 0.2f);
			}
		}
	

		if (m_edges != null) {
			Gizmos.color = Color.white;
			for (int i = 0; i< m_edges.Count; i++) {
				Vector2 left = (Vector2)m_edges [i].p0;
				Vector2 right = (Vector2)m_edges [i].p1;
				Vector3 v3left = new Vector3 (left.x,0,left.y);
				Vector3 v3right = new Vector3 (right.x,0,right.y);
				//Gizmos.DrawLine ((Vector3)left, (Vector3)right);
				Gizmos.DrawLine (v3left, v3right);


			}
		}
/*
		Gizmos.color = Color.magenta;
		if (m_delaunayTriangulation != null) {
			for (int i = 0; i< m_delaunayTriangulation.Count; i++) {
				Vector2 left = (Vector2)m_delaunayTriangulation [i].p0;
				Vector2 right = (Vector2)m_delaunayTriangulation [i].p1;
				Gizmos.DrawLine ((Vector3)left, (Vector3)right);
			}
		}

		if (m_spanningTree != null) {
			Gizmos.color = Color.green;
			for (int i = 0; i< m_spanningTree.Count; i++) {
				LineSegment seg = m_spanningTree [i];				
				Vector2 left = (Vector2)seg.p0;
				Vector2 right = (Vector2)seg.p1;
				Gizmos.DrawLine ((Vector3)left, (Vector3)right);
			}
		}
*/
		Gizmos.color = Color.yellow;
		//Gizmos.DrawLine (new Vector2 (0, 0), new Vector2 (0, m_mapHeight));
		Gizmos.DrawLine (new Vector3 (0, 0, 0), new Vector3 (0,0, m_mapHeight));

		//Gizmos.DrawLine (new Vector2 (0, 0), new Vector2 (m_mapWidth, 0));
		Gizmos.DrawLine (new Vector3 (0,0,0), new Vector3 (m_mapWidth,0,0));

		//Gizmos.DrawLine (new Vector2 (m_mapWidth, 0), new Vector2 (m_mapWidth, m_mapHeight));
		Gizmos.DrawLine (new Vector3 (m_mapWidth,0 ,0), new Vector3 (m_mapWidth,0, m_mapHeight));

		//Gizmos.DrawLine (new Vector2 (0, m_mapHeight), new Vector2 (m_mapWidth, m_mapHeight));
		Gizmos.DrawLine (new Vector3 (0,0, m_mapHeight), new Vector3 (m_mapWidth,0, m_mapHeight));
	}
}