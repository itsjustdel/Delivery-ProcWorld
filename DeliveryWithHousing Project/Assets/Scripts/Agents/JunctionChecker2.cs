using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JunctionChecker2 : MonoBehaviour {

	public List<GameObject> agentsInTrigger = new List<GameObject>();
	// Use this for initialization
	void Update ()
    {
       
	    for(int i = 0; i < agentsInTrigger.Count; i++)
        {
            if (agentsInTrigger[i].GetComponent<AgentBehaviour>().readyForNewJourney == true)
            {
                agentsInTrigger.RemoveAt(i);                    
            }
        }

        

	}

	void OnTriggerEnter(Collider other)
	{
		//if agent is in this trigger
		if(other.tag == "Agent")
		{

            AgentTrigger agentTrigger = other.transform.Find("Trigger").GetComponent<AgentTrigger>();
            agentTrigger.inJunction = true;

            if (!agentsInTrigger.Contains(other.gameObject))
			{
				agentsInTrigger.Add(other.gameObject);
			}

            if (agentsInTrigger.Count > 1)
            {
                //other.gameObject.GetComponent<UnitySteer.Behaviors.SteerForPathSimplified>().Weight = 0; //for ontriggerENTER
                other.GetComponent<WaypointPlayer>().speed = 0;
                //as agent goes ion to junciton force it do drive on the correct side of the road, not middlewayppints basically
                //other.gameObject.transform.FindChild("Trigger").GetComponent<AgentTrigger>().CheckSidesForGameObject(other.gameObject);

            }

            //make the car check the side of the road it will be pulling on to
            agentTrigger.normalSide = agentTrigger.CheckSideAtJunction();

            agentTrigger.inJunction = true;
        }

	}

	void OnTriggerExit(Collider other)
	{
		if(other.tag == "Agent")
		{
            AgentTrigger agentTrigger = other.transform.Find("Trigger").GetComponent<AgentTrigger>();
            agentTrigger.inJunction = false;
            //	if (!agentsInTrigger.Contains(other.gameObject))
            //	{
            agentsInTrigger.Remove(other.gameObject);
            //	}
            if (agentsInTrigger.Count > 0)
            {
                //	agentsInTrigger[0].gameObject.GetComponent<UnitySteer.Behaviors.SteerForPathSimplified>().Weight = 10;
                agentsInTrigger[0].gameObject.GetComponent<WaypointPlayer>().speed = 5;
             //   other.gameObject.transform.FindChild("Trigger").GetComponent<AgentTrigger>().CheckSidesForGameObject(other.gameObject);
            }

            agentTrigger.inJunction = false;

        }
	}
}
