using UnityEngine;
using System.Collections;

public class CheckForCellUnderRoad : MonoBehaviour {
	public LayerMask myLayerMask;
	public float cellCheckFrequency = 100f;
	public Transform  cube;
	// Use this for initialization
	void Start () {
		ChangeMaterialOfCell();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void ChangeMaterialOfCell()
	{
//		BezierSpline roadCurve = GameObject.FindWithTag("Road Dresser").GetComponent<SplineMesh>().spline;
		Mesh roadMesh = GameObject.FindGameObjectWithTag("Road").GetComponent<MeshFilter>().mesh;
		myLayerMask = LayerMask.GetMask("TerrainCell");
		float stepSize = cellCheckFrequency;
		stepSize = 1/(stepSize-1);
		
		RaycastHit hit;
		
		for (int i = 0; i < roadMesh.vertexCount; i++)
		{
			//Vector3 position = roadCurve.GetPoint( i * stepSize);
			Vector3 position = roadMesh.vertices[i];

			if (Physics.Raycast(position + (Vector3.up*500) , -Vector3.up, out hit, 1000.0f, myLayerMask))
			{
				//Debug.Log(" change materialhit");
				
				Transform item = Instantiate(cube) as Transform;
				item.position= hit.point;
				Material newMat = Resources.Load("Brown",typeof(Material)) as Material;		
				hit.collider.GetComponent<MeshRenderer>().sharedMaterial = newMat;
			}
			
		}
		
	}
}
