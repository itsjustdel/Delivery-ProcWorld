using UnityEngine;
using System.Collections;

public class RandomiseBrickWall : MonoBehaviour {

	// Use this for initialization
	void Start () {
	//	Mesh mesh = GetComponent<MeshFilter>().mesh;
		//mesh.RecalculateBounds();

		//GetComponent<Transform>().rotation =  Quaternion.Euler(Random.Range(-10f,10f),
		  //                                                           Random.Range(-10f,10f),
		    //                                                         Random.Range(-10f,10f));
		transform.RotateAround(gameObject.transform.localPosition,Vector3.up,10);
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(gameObject.transform.localPosition,Vector3.up,10*Time.deltaTime);
	}
}
