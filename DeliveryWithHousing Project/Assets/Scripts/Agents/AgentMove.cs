using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AgentMove : MonoBehaviour {

	public GridPlayer gp;
	public List<Vector3> pathList;
	public Transform agentPrefab;
	// Use this for initialization
	void Start () {
	
		gp = GetComponent<GridPlayer>();
		pathList = gp.Path;

		Transform agent = Instantiate(agentPrefab,transform.position,Quaternion.identity) as Transform;
		agent.parent = this.transform;
	}


	public int i = 0;
	// Update is called once per frame
	void Update () 
	{
		Vector3 currentWaypoint = gp.Path[i];
		Vector3 nextWaypoint = gp.Path[i+1];
		float distance = Vector3.Distance(transform.position, currentWaypoint);
		Debug.Log(distance);
		if(distance < 10 && i < gp.Path.Count)
		{
			i += 1;
		}

		transform.position = Vector3.MoveTowards(transform.position,nextWaypoint,0.1f);
	}
}
