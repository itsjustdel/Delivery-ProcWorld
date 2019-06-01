using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainPlaneActivator : MonoBehaviour {
	public Vector3 pPos;
	public Vector3 pDir;
	private bool waited; 
	public List<GameObject> objectList = new List<GameObject>();
	// Use this for initialization
	void Start () {
		transform.position = pPos;
		transform.rotation = Quaternion.LookRotation(pDir);
		StartCoroutine("Timer");
	}
	


	//This gathers all objects within the box collider and enters them in to a list for activation
	void OnTriggerStay(Collider item)
	{

		objectList.Add(item.gameObject);
		
		if (item.tag == "Flora")
			item.GetComponent<PlaceFlora>().enabled = true;

	
		
	}

	IEnumerator Timer()
	{
		if (!waited)
		{
			yield return new WaitForSeconds(1);
			waited = true;
		}
	 	
		if (waited)
		{
			Debug.Log("turning off trigger");
			this.enabled = false;
			Destroy(this.gameObject);
		}
	
	}
}
