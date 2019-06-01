using UnityEngine;
using System.Collections;

public class VanTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.name == "House")
		{
			Debug.Log("House Triggered enter");
			collider.gameObject.GetComponent<MeshControl>().renderIndividuals =true;

		}
	}

	void OnTriggerExit(Collider collider)
	{
		if(collider.gameObject.name == "House")
		{
			Debug.Log("House Triggered exit");
			collider.gameObject.GetComponent<MeshControl>().renderCombined=true;
		}
	}
}
