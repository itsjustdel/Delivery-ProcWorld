using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class CurveInjector : MonoBehaviour {


	//Container for receiving points from PathFinder

	public BezierSpline bezier;
	public Vector3 addV3;
	public List<Vector3> list;

	private bool meshed;
	public float raiseAmt = 5;


	public bool add;
	// Use this for initialization
	void Start () {

		//Array.Resize(ref bezier.points, list.Count);
		Array.Resize(ref bezier.modes, list.Count);

		bezier.points = list.ToArray();
	

	
		for (int i = 1; i<bezier.points.Length; i++)		
		{
			//bezier.points[i].y += raiseAmt;	
		}
		//Go through points in the array and set Enforce Mode (aligned/mirrored/free
		for (int i = 0; i < bezier.points.Length; i++)
		
		{
			//Debug.Log("do");
			//Debug.Log(bezier.points.Length);

			//this skips the first two and the last two points in the array which mkaes up the curve
			//It can glitch the curve if we set the control points to "aligned" and it has nothing to align to
			if (i==0 || i == 1|| i == bezier.points.Length-1 || i ==bezier.points.Length-2)
			{
				//Debug.Log("continue");
				continue; //skips
			}
			else	

				bezier.SetControlPointMode(i,BezierControlPointMode.Aligned);
		}

		//Once finished, set the road mesh script enabled (better way of triggering this?)
		gameObject.GetComponent<SplineMesh>().enabled = true;
		//GameObject.Find("Kerbs").GetComponent<KerbInjector>().enabled = true;
	}
}