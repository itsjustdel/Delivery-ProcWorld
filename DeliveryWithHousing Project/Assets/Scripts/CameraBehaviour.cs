using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour {
	public GameObject van;


	[Range(-50.0f, 50.0f)]	
	public float xOffset;

	[Range(-50.0f, 50.0f)]	
	public float yOffset;

	[Range(-50.0f, 50.0f)]	
	public float zOffset;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (van != null){

			transform.position = van.transform.position;
			float x = transform.position.x + xOffset;
			float y = transform.position.y + yOffset;
			float z = transform.position.z + zOffset;
			transform.position = new Vector3(x,y,z);
		}
	}
}
