using UnityEngine;
using System.Collections;

public class AgentNav : MonoBehaviour {

	GameObject[] buildings;
	// Use this for initialization
	void Start () {
	//TODO transform.position should be on the road mesh, not the house, same with target point




		buildings = GameObject.FindGameObjectsWithTag("Building");

		transform.position = buildings[Random.Range(0,buildings.Length)].transform.position;
		GridPlayer gp = gameObject.AddComponent<GridPlayer>();
		Vector3 target = buildings[Random.Range(0,buildings.Length)].transform.position;

		//TODO dont think this is working right, should nt be able to choose a house nearby
		while (target==transform.position)
		{
			target = buildings[Random.Range(0,buildings.Length)].transform.position;

			//if target is to close, force the loop to try again
			float distance = Vector3.Distance(transform.position, target);
			if (distance < 200)
				target = transform.position;
		}

		gp.target = buildings[Random.Range(0,buildings.Length)].transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
