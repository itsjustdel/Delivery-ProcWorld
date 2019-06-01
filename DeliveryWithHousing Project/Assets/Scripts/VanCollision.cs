using UnityEngine;
using System.Collections;


//This script enables and disables the renderer on objects within the trigger/collider of the van

public class VanCollision : MonoBehaviour {



	public MeshControl combinedMeshParent;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collision)
	{
		if ( collision.gameObject.name == ("Brick"))
		{
//			Debug.Log("hit");
			GameObject combinedMesh = collision.gameObject;
			combinedMeshParent = combinedMesh.GetComponentInParent<MeshControl>();
			if(!combinedMeshParent.renderIndividuals)
			{
				//combinedMeshParent.renderIndividuals = true;
			}
		}
	}
}
