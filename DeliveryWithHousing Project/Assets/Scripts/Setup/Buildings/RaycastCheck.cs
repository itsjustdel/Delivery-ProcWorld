using UnityEngine;
using System.Collections;

public class RaycastCheck : MonoBehaviour {

	public Transform box;

	// Use this for initialization
	void Start () {

		LayerMask lM = LayerMask.GetMask("Road");
		
		RaycastHit hit;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1000.0F,lM))
		{
			
			if ((hit.collider.name == "Road"))
			   
			{
				Destroy(transform.parent.gameObject);
				Debug.Log("Building Destroyed On Setup, hit Road" + hit.collider);
			}
			
		}

		
		//StartCoroutine("changeBoxLayer");


	}
	
	IEnumerator changeBoxLayer()
	{
		yield return new WaitForFixedUpdate();
		box.gameObject.layer = 0;
		yield break;
	}
}
