using UnityEngine;
using System.Collections;

public class DistanceEnable : MonoBehaviour {
	public bool doDebug;

	public int range = 200;
	private GameObject player;
	private MeshRenderer meshRenderer;
	private SingleTerrainPlane singleTerrainPlane;
	// Use this for initialization
	void Start () {
		meshRenderer = gameObject.GetComponent<MeshRenderer>();
		singleTerrainPlane = gameObject.GetComponent<SingleTerrainPlane>();
	}
	
	// Update is called once per frame



	void Update () {

		if (GameObject.FindWithTag("Player")!= null)
		{
			player = GameObject.FindWithTag("Player");
		

		float distance = Vector3.Distance(player.transform.position, meshRenderer.bounds.center); 

			if(doDebug)
			Debug.Log(distance);
		
			if (distance >= range){
			meshRenderer.enabled = false; //deactivates the gameobject mesh renderer
			
			//if it is within range
			}else{
				meshRenderer.enabled = true;

				if(!singleTerrainPlane.enabled)
				{
					//create mesh if not already done
					singleTerrainPlane.enabled = true;
				}
			}
		}
	}
}


