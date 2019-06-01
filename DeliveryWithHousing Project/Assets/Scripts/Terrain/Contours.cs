using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class Contours : MonoBehaviour {

	public SplineMesh splineMeshScript; //used for frequency
	public BezierSpline roadCurve;
	public BezierSpline bezier;
	public List<Vector3> listL;
	public List<Vector3> listR;
	public List<Vector3> listL2;
	public List<Vector3> listR2;
	public List<Vector3> listL3;
	public List<Vector3> listR3;
	public float frequency;
	public float tileSize = 100;
	public Transform cubePrefab;
	public List<BezierSpline> splineList;
	public List<List<Vector3>> listList;
	public int ySize= 10;
	public bool doCubesList = false;
	public bool doCubesCurves = false;
//	public int xSize = 100;
	// Use this for initialization

	void Start () {

		//frequency = splineMeshScript.frequency;

		splineList = new List<BezierSpline>();
		listList = new List<List<Vector3>>();

	
		float stepSize = frequency;
		
		stepSize =2f / (stepSize -1);
		//Debug.Log(stepSize);
		
		//lifts the curve up away from terrain
		for (int i = 0; i<bezier.points.Length; i++)
			
		{
			//bezier.points[i].y += raiseAmt;
			
		}


		for (int i = 0; i < ySize;i++){

			List<Vector3> tempList = new List<Vector3>();

			for (float f = 0; f < frequency/tileSize; f++) {
				Vector3 position = bezier.GetPoint(f * (stepSize));
			
				Vector3 direction = bezier.GetDirection(f*(stepSize));
				Vector3 rot1 = Quaternion.Euler(0,-90,0) * direction; 
				//Vector3 rot2 = Quaternion.Euler(0,90,0) * direction; 
				rot1 = Vector3.right;

			//	rot1*=splineMeshScript.roadWidth;
			//	rot2*=splineMeshScript.roadWidth;

				rot1*= (i+1) + splineMeshScript.roadTotalWidth;
				//rot2*= (i+1) + splineMeshScript.roadWidth;

				Vector3 offset1 = rot1 +position;
				//Vector3 offset2 = rot2+position;
				
				tempList.Add(offset1);
				//tempList.Add(offset2);
				
			}
			//add this templist to the list List
			listList.Add(tempList);
						
		}

		//Make Curves from the lists inside the listOfLists and add curve to splineList

		for (int i = 0; i < listList.Count; i++)
		{
			BezierSpline bezier = gameObject.AddComponent<BezierSpline>();
			bezier.points = listList[i].ToArray();

			//iterate through the beizer and adjust control points
			splineList.Add(bezier);
		}

/*
		for (float f = 0; f < frequency/tileSize; f++) {
			Vector3 position = bezier.GetPoint(f * (stepSize));
			
			Vector3 direction = bezier.GetDirection(f*(stepSize));
			Vector3 rot1 = Quaternion.Euler(0,-90,0) * direction; 
			Vector3 rot2 = Quaternion.Euler(0,90,0) * direction; 
			rot1*=2;
			rot2*=2;

			rot1*=splineMeshScript.roadWidth;
			rot2*=splineMeshScript.roadWidth;
			Vector3 offset1 = rot1 +position;
			Vector3 offset2 = rot2+position;
			
			
			listL2.Add(offset1);
			listR2.Add(offset2);
			
		}
		for (float f = 0; f < frequency/tileSize; f++) {
			Vector3 position = bezier.GetPoint(f * (stepSize));
			
			Vector3 direction = bezier.GetDirection(f*(stepSize));
			Vector3 rot1 = Quaternion.Euler(0,-90,0) * direction; 
			Vector3 rot2 = Quaternion.Euler(0,90,0) * direction; 
			rot1*=3;
			rot2*=3;
			
			rot1*=splineMeshScript.roadWidth;
			rot2*=splineMeshScript.roadWidth;
			Vector3 offset1 = rot1 +position;
			Vector3 offset2 = rot2+position;
			
			
			listL3.Add(offset1);
			listR3.Add(offset2);
			
		}
*/

	}

	void Update(){
		if (doCubesList)
		{
			CubesL ();
			doCubesList = false;
		}

		if (doCubesCurves)
		{
			CubesC ();
			doCubesCurves = false;
		}
	}

	void CubesC(){

		frequency = frequency/100;
		Debug.Log("hi");

		for (int i = 0; i < splineList.Count; i++)
		{
			for (int j = 0; j < frequency;j++)
			{
				bezier = splineList[i];
				Vector3 position = bezier.GetPoint(j/(frequency-1));
				Transform item = Instantiate(cubePrefab) as Transform;
				item.position = position; 
			}
		}
	}

	void CubesL(){
		Debug.Log("hi");
		for (int j = 0; j < listList.Count;j++){
			for (int i = 0; i < listList[j].Count;i++)
			{
				Transform item = Instantiate(cubePrefab) as Transform;
				item.position= listList[j][i];
			
			}
		}
	}
}