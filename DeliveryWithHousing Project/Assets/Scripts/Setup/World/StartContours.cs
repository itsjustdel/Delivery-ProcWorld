using UnityEngine;
using System.Collections;

public class StartContours : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if(GetComponent<MeshCollider>().sharedMesh != null)
        {
            GetComponent<ContoursForTerrain>().enabled = true;
            enabled = false;
        }
	
	}
}
