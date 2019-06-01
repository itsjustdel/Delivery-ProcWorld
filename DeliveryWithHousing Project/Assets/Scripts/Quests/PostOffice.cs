using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PostOffice : MonoBehaviour {

	public GameObject van;
    public bool newMailDay = true;
    public bool expectingMail;
    // Use this for initialization
    void Start () {

	}

	public void AsignPost()
	{

        newMailDay = false;

        van = GameObject.FindWithTag("Player");

		if(van.transform.Find("Quests").GetComponent<PostVan>().mailList.Count > 0)
			return;

		//grab available buildings
		GameObject[] allBuildings = GameObject.FindGameObjectsWithTag("HouseCell");
        //create buildings list with only cells which will be built on. Filter out the cells, which make up the parent house mesh, basically
        List<GameObject> buildings = new List<GameObject>();
        for(int i = 0; i < allBuildings.Length; i++)
        {
            if (allBuildings[i].name == "Combined mesh")
                buildings.Add(allBuildings[i]);
        }

		//decide how much mail there is today
		int mailAmount = Random.Range(0,buildings.Count);

		//go through buildings array and add some buildings randomly to a mail list
		List<GameObject> buildingsList = new List<GameObject>();

		Material pink = Resources.Load("Red",typeof (Material)) as Material;
		for (int i = 0; i < mailAmount; i++)
		{
            
			int random = Random.Range(0,buildings.Count);
			buildingsList.Add(buildings[random]);
			//change colour of building
			buildings[random].GetComponent<HouseQuests>().expectingMail = true;
			buildings[random].GetComponent<MeshRenderer>().sharedMaterial = pink;
		}

		//give this list to the van

		van.transform.Find("Quests").GetComponent<PostVan>().mailList = buildingsList;
	}

}
