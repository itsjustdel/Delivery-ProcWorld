using UnityEngine;
using System.Collections;

public class ActivateObjects : MonoBehaviour {

    public BuildList buildList;
    public BranchArray branchArray;
    public bool houses = false;
    public bool flora;
    public bool bushes;
    public bool fences;
    public bool fieldPatterns;
	// Use this for initialization
	void Start () {
        buildList = GameObject.FindGameObjectWithTag("Code").GetComponent<BuildList>();
        branchArray = GameObject.FindGameObjectWithTag("Code").GetComponent<BranchArray>();
	}

	//Check for Flora entering the trigger attached to the van gameobject
	void OnTriggerEnter(Collider other) {
       

        //activate an agent's trigger so it can avoid other cars
        if(other.tag == "Agent")
        {
            other.transform.Find("Trigger").gameObject.SetActive(true);
            other.transform.Find("Stop").gameObject.SetActive(true);
        }
        if (other.tag == "HouseCell")            
        {
            if (houses)
            {
                //old
                // HouseCellInfo hci = other.gameObject.AddComponent<HouseCellInfo>();
                //hci.enabled = false;
                //buildList.components.Add(hci);
                //enable procedural house script
                //other.GetComponent<HouseV2>().enabled = true;
                //make sure it hasn't  already been built
                if(other.gameObject.GetComponent<HouseV2>().reportedBack == false)
                    if(!buildList.components.Contains(other.gameObject.GetComponent<HouseV2>()))
                        buildList.components.Add(other.gameObject.GetComponent<HouseV2>());

                //other.GetComponent<HouseCellInfo>().enabled = true;
            }
        }

        if(other.tag == "Field")
        {
            if (fences)
            {
                if (other.gameObject.GetComponent<FenceAroundCell>() == null)
                {
                    FenceAroundCell fenceAroundCell = other.gameObject.AddComponent<FenceAroundCell>();
                    fenceAroundCell.enabled = false;
                    buildList.components.Add(fenceAroundCell);
                }
            }

            if(bushes)
            {
                
                if (other.gameObject.GetComponent<BushesForCell>() == null)
                {
                    // if(other.gameObject.GetComponent<FieldPattern>().planted == false)
                    // other.gameObject.GetComponent<FieldPattern>().StartPlanting();
                    BushesForCell bushesForCell = other.gameObject.AddComponent<BushesForCell>();
                    bushesForCell.enabled = false;
                    buildList.components.Add(bushesForCell);
                }
                
            }

            if(fieldPatterns)
            {

                if (other.gameObject.GetComponent<FieldGridPlanter>() != null)
                {
                    //start
                    //if we ahvent worked this out, add it to the list of building
                    if(  other.gameObject.GetComponent<FieldGridPlanter>().enabled != true && !buildList.components.Contains(other.gameObject.GetComponent<FieldGridPlanter>()))//and isnt in list?//beter way,. waiting bool?
                        buildList.components.Add(other.gameObject.GetComponent<FieldGridPlanter>());
                }

                if (other.gameObject.GetComponent<FieldGrids>() != null)
                {
                    if (other.gameObject.GetComponent<FieldGrids>().enabled != true && !buildList.components.Contains(other.gameObject.GetComponent<FieldGrids>()))//and isnt in list?
                    {
                        FieldGrids fg = other.gameObject.GetComponent<FieldGrids>();
                        //fg.enabled = false;//already dia
                        buildList.components.Add(fg);
                    }
                }

                //if we find a field grid, start the process to make it check whether is in the polygon of the field
                
                /*
                if (other.gameObject.GetComponent<FieldPattern>() == null)
                {
                   // if(other.gameObject.GetComponent<FieldPattern>().planted == false)
                     // other.gameObject.GetComponent<FieldPattern>().StartPlanting();
                    FieldPattern fieldPattern = other.gameObject.AddComponent<FieldPattern>();
                    fieldPattern.enabled = false;
                    buildList.components.Add(fieldPattern);
                }

            */
            }
        }
        if (flora)
        {
            if (other.tag == "Flora")
            {
                //return;
                if (other.name == "Shrub")
                {
                    //use branch array function to create a shrub at its position and parent this new shrub to the trigger we just hit's parent too
                    branchArray.MakeShrub(other.transform.gameObject, other.transform.position, true);
                }
                if (other.name == "Batch Parent")
                {
                    // buildList.components.Add(other.GetComponent<BatchPlanter>());//planting in grids directly now

                }
            }
        }



        if (other.tag == "Cell")
        {
            if (other.gameObject.GetComponent<PolygonTester>())
            {
                PolygonTester pt = other.gameObject.GetComponent<PolygonTester>();
            
                if (pt.hitRoad)
                {
                    other.gameObject.GetComponent<AddToHitRoadCell>().enabled = true;
                }
            }            
        }
    }

    void OnTriggerExit(Collider other)
    {
        //disable everthing in field
        if(other.tag == "Field")
        {
            for (int i = 0; i < other.transform.childCount; i++)
            {
                //other.transform.GetChild(i).gameObject.SetActive(false);
              // Destroy(other.transform.GetChild(i).gameObject);
            }
        }
    }
   
    /*
	IEnumerator EnableFlora(GameObject other)
	{
		PlaceFlora[] flora = other.GetComponentsInChildren<PlaceFlora>();
		//yield return new WaitForSeconds(1);
		for(int i = 0; i < flora.Length; i++)
		{
			flora[i].GetComponent<PlaceFlora>().enabled = true;
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}
    /*
	//Check for Flora leaving the van area// Taken out jsut now to check if any performance gain
    /*
	void OnTriggerExit(Collider other) {
		
		if (other.tag == "Flora")
		{
			//Find all renderers and disable them
			MeshRenderer[] array = other.GetComponentsInChildren<MeshRenderer>();

			foreach (MeshRenderer m in array)
			{
				//if(other.name == "CombineMesh")
				m.enabled =false;
			}
		}

        if (other.tag == "Agent")
        {
            other.transform.FindChild("Trigger").gameObject.SetActive(false);
            other.transform.FindChild("Stop").gameObject.SetActive(false);

        //    AgentTrigger agentTrigger = other.transform.FindChild("Trigger").GetComponent<AgentTrigger>();
          //  other.transform.FindChild("Trigger").GetComponent<AgentTrigger>().sideOfThis = agentTrigger.CheckSidesForGameObject(other.gameObject);
        }
	}
    */

}
