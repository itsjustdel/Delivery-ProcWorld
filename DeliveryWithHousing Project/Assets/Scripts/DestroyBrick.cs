using UnityEngine;
using System.Collections;

public class DestroyBrick : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay(Collider collision) {
		if (collision.gameObject.name == "Brick")
		{
			//Debug.Log("has hit window");
			Destroy(collision.gameObject);

		}
		//GetComponent<MeshCollider>().isTrigger = false;
		//GetComponent<MeshCollider>().convex = false;
		Destroy(GetComponent<DestroyBrick>());
	}

}
