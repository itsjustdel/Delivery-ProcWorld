using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BuildingUseAsign : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        //not using atm
    }

    void Asign()
    { 
		//put all buildings in an array
		GameObject[] buildingsAll = GameObject.FindGameObjectsWithTag("HouseCell");
        //house cells made up of smaller cells, remove the little ones
        List<GameObject> buildings = new List<GameObject>();
        for(int i = 0; i < buildingsAll.Length; i++)
        {
            if(buildingsAll[i].name == ("Combined mesh"))
            {
                buildings.Add(buildingsAll[i]);
            }
        }
       // Debug.Log(buildings.Count);

		//grab one at random
		//int random = Random.Range(0,buildings.Count);


        //plots are decided in Fields script now. yeah...
        /*
		//set this to be the shop
		buildings[random].tag = "Shop";
		buildings[random].GetComponent<BuildingUse>().use = "Shop";
		Material yellow = Resources.Load("Yellow",typeof(Material)) as Material;
		buildings[random].GetComponent<MeshRenderer>().sharedMaterial = yellow;
        */
		//search again

		//buildings = GameObject.FindGameObjectsWithTag("Building");
		//grab one at random
		//random = Random.Range(0,buildings.Count);
		//set this to be the shop
        /*
		buildings[random].tag = "PostOffice";
        buildings[random].name = "PostOffice";
        buildings[random].GetComponent<BuildingUse>().use = "PostOffice";
		Material white = Resources.Load("White",typeof(Material)) as Material;
		buildings[random].GetComponent<MeshRenderer>().sharedMaterial =white;
		//add its own script
		buildings[random].AddComponent<PostOffice>();
        */
		//now asign what is left to be a normal house
		//buildings = GameObject.FindGameObjectsWithTag("Building");
		foreach (GameObject building in buildings)
		{
			building.AddComponent<BuildingUse>().use = "House";
			//building.AddComponent<Residents>().enabled = false;
		}
	}

}
