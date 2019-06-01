using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour {

    //attached to van
    public GameObject cam;
	// Use this for initialization
	void Start ()
    {
        cam = GameObject.Find("MiniMapCam");
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown("m"))
        {
            //set minimap to the centre of the closest ring road
            GameObject[] roads = GameObject.FindGameObjectsWithTag("Road");

            float distance = Mathf.Infinity;
            GameObject nearestRoad = null;
            for(int i = 0; i < roads.Length;i++)
            {
                if (nearestRoad == null)
                    nearestRoad = roads[i];
                else
                {
                    float temp = Vector3.Distance(nearestRoad.GetComponent<MeshRenderer>().bounds.center, roads[i].GetComponent<MeshRenderer>().bounds.center);
                    if (temp < distance)
                    {
                        distance = temp;
                        nearestRoad = roads[i];
                    }
                }
            }
            Vector3 camPos = nearestRoad.GetComponent<MeshRenderer>().bounds.center;
            camPos.y = 500f;
            cam.transform.position = camPos;
            cam.GetComponent<Camera>().orthographicSize = nearestRoad.GetComponent<MeshRenderer>().bounds.size.magnitude / 2;
            
        }


        /*
        if (Input.GetKey("m"))
        {
            cam.SetActive(true);
        }
        else
            cam.SetActive(false);
            */
	}
}
