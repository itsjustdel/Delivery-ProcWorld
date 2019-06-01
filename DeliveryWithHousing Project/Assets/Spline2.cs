using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spline2 : MonoBehaviour {

	public CRSpline spline;
	public Vector3 point;
	public bool add;
	public bool cubeIt;
	public float t;
	public Transform c;
	public float stepSize;
	// Use this for initialization
	void Start () {

	
	}
	
	// Update is called once per frame
	void Update () {
		if (add)
		{
			List<Vector3> list = new List<Vector3>(spline.pts);
			list.Add(point);
			spline.pts = list.ToArray();
			add = false;

		}

		if (cubeIt)
		{
			Transform item = Instantiate(c) as Transform;
			Vector3 position = spline.Interp(t * stepSize);
			item.transform.localPosition = position;
			item.transform.parent = transform;
			cubeIt = false;
		}
	}


}
