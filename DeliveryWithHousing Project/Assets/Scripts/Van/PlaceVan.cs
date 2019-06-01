using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlaceVan : MonoBehaviour {

	public GameObject vanPrefab;
	//public CurveInjector curveInjector;
	//public GameObject aRoad;
	private bool spawned = false;
	public TJunctionBuildings tJunctionBuildings;
	// Use this for initialization
	void Start () {

		vanPrefab = Resources.Load("Prefabs/Van", typeof (GameObject)) as GameObject;

	}
	
	// Update is called once per frame
	void Update () {	

		
		if (Input.GetButtonDown("Start"))
		{

		}

		if (!spawned){

			if (Input.GetButtonDown("Start") || Input.GetKeyDown("return"))
			{
                SpawnVan();
			}

		}

		if (spawned && Input.GetButtonDown("Start")){

			//Vector3 pos = curveInjector.list[curveInjector.list.Count -50];
			//GameObject.Find("ARoad");


//	/		Vector3 pos = tJunctionBuildings.middle;
	//		pos.y +=5;
	//		van.transform.position = pos;
	//		van.transform.rotation = Quaternion.identity;
		}
	}
    public void SpawnVan()
    {
        GameObject road  = GameObject.FindGameObjectWithTag("Road");

        int max = road.GetComponent<MeshFilter>().mesh.vertexCount;

        int random = Random.Range(0, 1); //spawning near post office atm, was "max"

        Vector3 pos = road.GetComponent<MeshFilter>().mesh.vertices[random];
        pos.y += 1;


        GameObject van = Instantiate(vanPrefab, pos, Quaternion.identity) as GameObject;

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraBehaviour>().van = van;
        //tell LOD script this too
        gameObject.GetComponent<LODSwitcher>().player = van;

        spawned = true;

        //tell the post office this is us
        //post office made up of smaller cells and a combined cell. Script attached to combined cell
        GameObject[] postOfficeCells = GameObject.FindGameObjectsWithTag("PostOffice");
        foreach (GameObject go in postOfficeCells)
        {
            if (go.name == "Combined mesh")
                go.GetComponent<PostOffice>().van = van;

        }
        //this.enabled = false;
    }
}
