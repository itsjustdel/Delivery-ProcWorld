using UnityEngine;
using System.Collections;

public class DetachFromParent : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.parent = null;	
	}

}
