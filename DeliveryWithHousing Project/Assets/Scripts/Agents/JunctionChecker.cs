using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JunctionChecker : MonoBehaviour {
	//This script is attcehd to each agent

	public UnitySteer.Behaviors.SteerForPathSimplified steerForPath;
	private LayerMask lm;
	public GameObject lastJunction;
	public int amountOfAgents;
	public int currentAmount;
	// Use this for initialization
	void Start () {
		lm = LayerMask.GetMask("Road");
		//steerForPath = gameObject.transform.parent.GetComponent<UnitySteer.Behaviors.SteerForPathSimplified>();
	}

	void OnTriggerEnter(Collider other)
	{
		//if we enter a junction area
		if (other.tag == ("JunctionTrigger"))
		{
			lastJunction = other.GetComponent<Collider>().gameObject;
			JunctionCheck();		
		}
	}

	void JunctionCheck()
	{

			Debug.Log("entering junction");
			//get the lsit of agents in this trigger at this point in time
			List<GameObject> agentsInTrigger = lastJunction.GetComponent<JunctionChecker2>().agentsInTrigger;
			amountOfAgents = agentsInTrigger.Count;
			//check what road we are on
			RaycastHit roadCheck;
			Physics.Raycast(transform.position+Vector3.up,Vector3.down,out roadCheck, 5f,lm);
			
			bool atEnd = false;
			Mesh mesh =  roadCheck.transform.GetComponent<MeshFilter>().mesh;
			int triangleCount = mesh.triangles.Length;
			//	Debug.Log("At End" + roadCheck.triangleIndex);
			//	Debug.Log("of " + triangleCount);
			if( roadCheck.triangleIndex*3 < 100 || roadCheck.triangleIndex*3 > triangleCount - 300)//TODO still not right//
			{
				steerForPath.Weight = 0;
				atEnd =true;
			//	Debug.Log("At End" + roadCheck.triangleIndex*3);
			//	Debug.Log("of " + triangleCount);
			}
			
			foreach(GameObject agent in agentsInTrigger)
			{
				//get the position of all agents in junction area (what road they are on)
				RaycastHit[] hits = Physics.RaycastAll(agent.transform.position + Vector3.up,Vector3.down,5f,lm);
				//Debug.Log("hits length "+ hits.Length);
				
				foreach(RaycastHit hit in hits)
				{
					//if other agent is on a different road
					if(hit.collider.gameObject != roadCheck.collider.gameObject)
					{
						if(atEnd)
						{
							//if at the start or end of a mesh, stop the car
							steerForPath.Weight = 0;
							StartCoroutine("WaitAndCheck");
						}
					}
				}
			}

	}

	IEnumerator WaitAndCheck()
	{

		 currentAmount = lastJunction.GetComponent<JunctionChecker2>().agentsInTrigger.Count;

		while (currentAmount > 1)
		{
			yield return new WaitForSeconds(1f);
		}

		steerForPath.Weight = 10;
	}


}







	/*
	void OnTriggerStay(Collider other)
	{

		if(other.tag == "Agent")
		{
			RaycastHit[] hits =	Physics.RaycastAll(transform.parent.position + Vector3.up*2,Vector3.down,10f,lm);

			if (hits.Length > 1)
				return;

			foreach(RaycastHit hit in hits)
			{
				Mesh mesh =  hit.transform.GetComponent<MeshFilter>().mesh;
				int triangleCount = mesh.triangles.Length;	
				//Debug.Log(hit.triangleIndex);
				//Debug.Log("agent inside");
				//check where the hit is on the mesh using the mesh's triangle index
				if (hit.triangleIndex*3 < 10 || hit.triangleIndex*3 > triangleCount - 10)
				{
					//check that we are not at the start of the road we have just turned on to by scanning ahead and checking
					//if the the road ahead is the same
				
					if(hit.transform.parent.gameObject.name!="FirstRingRoad")
					{
						target = hit.transform.gameObject;
						steerForPath.Weight = 0;
					}
				}
			}

		}
	}

	void OnTriggerExit(Collider other)
	{
		
		if(other.tag == "Agent")
		{
			steerForPath.Weight = 10;
		}
	}
}
*/