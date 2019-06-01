using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateKerb : ProcBase {

	public float kerbSizeX;
	public float kerbSizeY;
	public float kerbSizeZ;

	public float frequency;
	public BezierSpline spline;
	public BezierSpline splineL;
	public Transform cube;
	public SplineMesh splineMeshScript;
	public List<Vector3> listL;
	public List<Vector3> listR;
	public KerbInjector kerbInj;
	// Use this for initialization
	void Start () {

		for (int i = 0; i <spline.points.Length;i++)

		{

		//	listL.Add(spline.points[i]);
		//	Vector3 direction = spline.GetDirection(f*(1/frequency));
		//	Vector3 rot1 = Quaternion.Euler(0,-90,0) * direction; 
		//	spline.points[i] 
		}
	

		
		for (float f = 0; f <= frequency; f++) {
			Vector3 position = spline.GetPoint(f * (1/frequency));
//			Vector3 nextPos = spline.GetPoint((f+1) * (1/frequency));
//			Transform item = Instantiate(cube) as Transform;
//			Transform item2 = Instantiate(cube) as Transform;
//			item.GetComponent<BoxCollider>().enabled = true;
//			item2.GetComponent<BoxCollider>().enabled = true;
			Vector3 direction = spline.GetDirection(f*(1/frequency));
			Vector3 rot1 = Quaternion.Euler(0,-90,0) * direction; 
			Vector3 rot2 = Quaternion.Euler(0,90,0) * direction; 
//			Vector3 lookUp = Quaternion.Euler(90,0,0) * direction; 
//			Quaternion lookRot = Quaternion.LookRotation (direction);
			rot1*=splineMeshScript.roadTotalWidth;
			rot2*=splineMeshScript.roadTotalWidth;
			Vector3 offset1 = rot1 +position;
			Vector3 offset2 = rot2+position;

			listL.Add(offset1);
			listR.Add(offset2);
		

			//Vector3 posL = splineL.GetPoint(f * (1/frequency));

//			item.transform.position = offset1;
//			item.transform.parent = transform;
//			item.tag = "Kerb";
//			item2.localScale = new Vector3(0.5f,0.5f,3f);
//			
//			item.transform.localRotation *= lookRot;
//			item.transform.position +=direction;

//			item2.transform.position = offset2;
//			item2.transform.parent = transform;
//			item2.tag = "Kerb";
			//item2.localScale = new Vector3(0.5f,0.5f,2.5f);
//			item2.transform.localRotation *= lookRot;
//			item2.transform.position +=direction;

//			MeshBuilder meshBuilder = new MeshBuilder();
		//	Quaternion lookRot = Quaternion.identity;
//			Vector3 up =  lookRot * Vector3.up * kerbSizeY ;
//			Vector3 right = lookRot * Vector3.right * kerbSizeX;
//			Vector3 forward = lookRot *Vector3.forward * kerbSizeZ;

//			GameObject child = new GameObject();
//			MeshRenderer meshRenderer = child.AddComponent<MeshRenderer>();
//			MeshFilter meshFilter = child.AddComponent<MeshFilter>();
//			MeshCollider meshCollider = child.AddComponent<MeshCollider>();
//			child.transform.parent = this.gameObject.transform;

//			Vector3 kerbHeight = kerbSizeY*(Vector3.up);
//			kerbHeight = lookRot*kerbHeight;
//			Vector3 nextPoint = nextPos-position;
//			right*=0.2f;
//			BuildQuad(meshBuilder,offset1,nextPoint,right);//bottom
//			BuildQuad(meshBuilder,offset1,right,kerbHeight);
//			BuildQuad(meshBuilder,offset1,kerbHeight,nextPoint);//far side

//			Vector3 farCorner = kerbHeight+nextPoint + offset1;
//			BuildQuad(meshBuilder,farCorner,-nextPoint,right);//top
//			BuildQuad(meshBuilder,farCorner,right,-kerbHeight);
//			BuildQuad(meshBuilder,farCorner+right,-nextPoint,-kerbHeight);


//			Mesh mesh = meshBuilder.CreateMesh();
//			meshFilter.sharedMesh = mesh;
//			meshCollider.sharedMesh = mesh;

//			meshRenderer.sharedMaterial = Resources.Load("Brick",typeof(Material)) as Material;

		}

	kerbInj.enabled =true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
