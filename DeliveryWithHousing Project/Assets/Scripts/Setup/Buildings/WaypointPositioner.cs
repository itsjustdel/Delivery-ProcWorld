using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointPositioner : MonoBehaviour {

    public bool move;
    public Transform cubePrefab;
	// Use this for initialization
	void Start () {

        StartCoroutine("MoveToNearestVertice");        
		
	}

    void Update()
    {
        if (move)
        {
            MoveToNearestVertice();
            move = false;
        }
    }
    IEnumerator MoveToNearestVertice()
    {
        List<RaycastHit> hits = new List<RaycastHit>();

        for (float i = 0; i < 7f; i += 0.5f)
        { 
            //Scan the road mesh and move waypoint to nearest point hit on the road
            RaycastHit[] results = Physics.SphereCastAll(transform.position + (Vector3.up * 10), i, Vector3.down);
            foreach(RaycastHit hit in results)
            {
                if (hit.transform.name != "RingRoad")
                    continue;

                hits.Add(hit);
            }
            yield return new WaitForEndOfFrame();
        }

        Vector3 closestPoint = Vector3.zero;
        float distance = Mathf.Infinity;

        foreach (RaycastHit hit in hits)
        {
            //          Transform cube = Instantiate(cubePrefab, hit.point, Quaternion.identity) as Transform;
           
            float diff = Vector3.Distance(transform.position, hit.point);
            if (diff < distance)
            {
                closestPoint = hit.point;
                distance = diff;
            }
            yield return new WaitForEndOfFrame();
        }

        //move this transform's position half way towards the road. This means the cars park slightly off the road

        transform.position = Vector3.Lerp(transform.parent.transform.position, closestPoint, 0.75f);

        SetNodePosition();
    }

    void SetNodePosition()
    {
        //Each node has a variable called Position. Set this to be the same as the gameobject it is attached to
        WaypointNode waypointNode = gameObject.GetComponent<WaypointNode>();
        waypointNode.position = transform.position;
    }
	

}
