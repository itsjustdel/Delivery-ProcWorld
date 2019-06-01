using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CurveMesh : ProcBase {

	public int xSize;
	public BezierCurve spline;
	
	public int frequency;
	
	public bool lookForward;
	
	public Transform[] items;
	public Transform[] items2;
	private List<Vector3> vertices = new List<Vector3>();
	private List<int> indices = new List<int>();
	//private List<Vector3> normals = new List<Vector3>();
	
	
	private void Start() {



		
		//MeshBuilder meshBuilder = new MeshBuilder();
		
		if (frequency <= 2){// || items == null || items.Length == 0) {
			return;
		}
		float stepSize = frequency;// * items.Length;
		//if (spline.Loop || stepSize == 1) {
		//	
		//	stepSize = 1f / stepSize;
		//}
		//else {
			
			stepSize =2f / (stepSize );
		//}
		for (int f = 0; f <= frequency; f++) {
			//	for (int i = 0; i < items.Length; i++, p++) {
			//Transform item = Instantiate(items[i]) as Transform;
			//Transform item2 = Instantiate(items[i]) as Transform;
			
			Vector3 position = spline.GetPoint(f * stepSize);
			Vector3 direction = spline.GetDirection(f*stepSize);
			Vector3 rot1 = Quaternion.Euler(0,-90,0) * direction; 
			Vector3 rot2 = Quaternion.Euler(0,90,0) * direction; 
			
			Vector3 offset1 = rot1+position;
			Vector3 offset2 = rot2+position;
			
			
			
			
			//	item.transform.localPosition = offset1;
			//	item2.transform.localPosition = offset2;
			
			vertices.Add(offset1 - transform.position);
			vertices.Add(offset2 - transform.position);
			
			if (lookForward) {
				//	item.transform.LookAt(position + spline.GetDirection(p * stepSize));
			}
			//	item.transform.parent = transform;
			//	item2.transform.parent = transform;
			//	}
		}
		
		GameObject child = new GameObject();
		child.name = "Road";
		child.transform.position = this.gameObject.transform.position;
		child.transform.parent = this.gameObject.transform;
		
		MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
		MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();
		
		Mesh mesh = new Mesh();	
		mesh.vertices = vertices.ToArray();
		
		for ( int i = 0; i <= frequency; i+=2)
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
		meshCollider.sharedMesh = mesh;
		mesh.RecalculateNormals();
		meshFilter.sharedMesh = mesh;
		
		Material newMat = Resources.Load("Brown",typeof(Material)) as Material;		
		meshRenderer.material = newMat;
		
		
		
	}
}