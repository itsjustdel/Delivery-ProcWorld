using UnityEngine;
using System.Collections;

public class WaypointAdder : MonoBehaviour {

	// Use this for initialization
	public void AddWaypoints () {

		WaypointNode waypointNode = gameObject.GetComponent<WaypointNode>();
		//spherecast looking for other waypoints
		LayerMask lm = LayerMask.GetMask("WaypointL","WaypointR");
		RaycastHit[] hits = Physics.SphereCastAll(transform.position + (Vector3.up *10),1f, Vector3.down,20f,lm);
		//add this node to each hit waypoint as neighbour
		foreach(RaycastHit hit in hits)
		{
			waypointNode.neighbors.Add(hit.transform.GetComponent<WaypointNode>());
		}
	}
	

}
