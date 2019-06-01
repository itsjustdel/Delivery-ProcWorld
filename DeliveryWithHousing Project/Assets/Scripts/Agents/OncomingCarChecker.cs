using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class OncomingCarChecker : MonoBehaviour {

    public AgentTrigger agentTrigger;
    public List<GameObject> agentsInLargeTrigger = new List<GameObject>();

    public GameObject GoThis;
    public GameObject GoOther;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Agent")
        {
            //if our agent is moving
            if (other.GetComponent<WaypointPlayer>().Path.Count != 0)
            {
                LayerMask lm = LayerMask.GetMask("Road");

                //check to see if car is on the same road as us - not seperated by a junction
                RaycastHit thisHit;
                Physics.Raycast(transform.parent.transform.position + (Vector3.up * 5), Vector3.down, out thisHit, 10f, lm);

                RaycastHit otherHit;
                Physics.SphereCast(other.transform.position + (Vector3.up * 5),2f, Vector3.down, out otherHit, 10f, lm);
                GoThis = thisHit.collider.gameObject;//TODO still misses sometimes
                GoOther = otherHit.collider.gameObject;

                if (thisHit.collider == otherHit.collider)
                {
                    agentsInLargeTrigger.Add(other.gameObject);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Agent")
        {
            if(agentsInLargeTrigger.Contains(other.gameObject))
            agentsInLargeTrigger.Remove(other.gameObject);
        }
    }
}
