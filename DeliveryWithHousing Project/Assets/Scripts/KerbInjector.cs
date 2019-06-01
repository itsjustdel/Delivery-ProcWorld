using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class KerbInjector : MonoBehaviour {
	public CreateKerb kerbScript;
	public CurveInjector roadDresserCurve;

	public SplineMesh splineMeshScript;

	public BezierSpline bezier;

	public Vector3 addV3;
	public List<Vector3> list;
	public List<Vector3> listL;
	public List<Vector3> listR;
	//public Transform target;
	//public Transform home;
	private bool meshed;

	public float frequency = 100f;
	public MeshCollider terrain;
	//	public Transform target2;
	//	public Transform target3;
	//	public Transform target4;
	//	public Transform target5;
	//	public Transform target6;
	//	public Transform target7;
	//	public Transform target8;
	
	public bool add;
	// Use this for initialization
	void Start () {

		frequency = splineMeshScript.frequency;
		//list = kerbScript.listL;
	//	list = roadDresserCurve.list;
	//	Array.Resize(ref bezier.points, list.Count);
	
	//	Array.Resize(ref bezier.modes, list.Count);
	//	bezier.points = list.ToArray();
		float stepSize = frequency;

		stepSize =2f / (stepSize -1);
		//Debug.Log(stepSize);

		//lifts the curve up away from terrain
		for (int i = 0; i<bezier.points.Length; i++)
			
		{
			//bezier.points[i].y += raiseAmt;
			
		}
		for (float f = 0; f < frequency; f++) {
			Vector3 position = bezier.GetPoint(f * (stepSize));
			//Vector3 nextPos = bezier.GetPoint((f+1) * (1/frequency));
			//			Transform item = Instantiate(cube) as Transform;
			//			Transform item2 = Instantiate(cube) as Transform;
			//			item.GetComponent<BoxCollider>().enabled = true;
			//			item2.GetComponent<BoxCollider>().enabled = true;
			Vector3 direction = bezier.GetDirection(f*(stepSize));
			Vector3 rot1 = Quaternion.Euler(0,-90,0) * direction; 
			Vector3 rot2 = Quaternion.Euler(0,90,0) * direction; 
			//Vector3 lookUp = Quaternion.Euler(90,0,0) * direction; 
		//	Quaternion lookRot = Quaternion.LookRotation (direction);
			rot1*=splineMeshScript.roadTotalWidth;
			rot2*=splineMeshScript.roadTotalWidth;
			Vector3 offset1 = rot1 +position;
			Vector3 offset2 = rot2+position;

			//check for where the terrain is firing a ray at it, the hit point becomes the new Y co-ordinate
			//Ignores layer 9 - Road
//			RaycastHit hit;
//			//float temp = 0f;;
//			if (Physics.Raycast(offset1, -Vector3.up, out hit, 100.0F,9))
//				hit.point += new Vector3(0f,splineMeshScript.raiseAmt,0f); //C# is annoying//raises the y up a little
//			offset1 = hit.point;

//			if (Physics.Raycast(offset2, -Vector3.up, out hit, 100.0F,9))
//				hit.point += new Vector3(0f,splineMeshScript.raiseAmt,0f); //C# is annoying//raises the y up a little
///				offset2 = hit.point;

			listL.Add(offset1);
			listR.Add(offset2);

		}


		

		//Once finished, start the mesh script
	//	KerbMesh kerbmesh= gameObject.GetComponent<KerbMesh>();
	//	kerbmesh.enabled = true;
	}
}