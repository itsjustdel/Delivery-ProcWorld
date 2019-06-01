using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HouseDetect : MonoBehaviour {
	public bool atThePostOffice = false;
	public bool atBuilding = false;
    public bool haveMail = false;
    public bool haveQuest = false;

	public bool postAvailable = true;
	public bool moving = true;

	public Transform quests;
	public Rigidbody rb;
	public GameObject currentBuilding;
	public PostOffice postOffice;
    public GameObject postOfficeGo;
    public GUIInfo guiInfo;

    public GameObject mailListObject;
    public GameObject mailIcon;
    public GameObject questIcon;

    public List<GameObject> buildingsInTrigger = new List<GameObject>();
    // Use this for initialization
    void Start () {

        mailListObject = transform.parent.Find("Quests").gameObject;

        guiInfo = transform.parent.Find("GUI").GetComponent<GUIInfo>();
        //find parent post office emsh and asign variable
        GameObject[] poArray = GameObject.FindGameObjectsWithTag("PostOffice");
        foreach(GameObject go in poArray)
        {
            if(go.name == "Combined mesh")
            {
                postOffice = go.GetComponent<PostOffice>();
            }
        }
		quests =  gameObject.transform.parent.Find("Quests");
		rb = gameObject.transform.parent.GetComponent<Rigidbody>();
        //		postOffice = GameObject.FindWithTag("PostOffice").GetComponent<PostOffice>(); 

        mailIcon = GameObject.Find("Canvas").transform.Find("MailIcon").gameObject;
        questIcon = GameObject.Find("Canvas").transform.Find("QuestIcon").gameObject;


    }

	void FixedUpdate()
	{

     
        if (rb.velocity.magnitude <= 1f && atThePostOffice == true)
		{
			moving = false;
			//we are at the post office, ask for post to deliver
			//if(postAvailable)
		//	{
				//postOffice.AsignPost();
			//	postAvailable = false;
		//	}
		}


		if (rb.velocity.magnitude <= 1f && atBuilding == true)
		{
			moving = false;
            //check if the van has anymail for the house it is triggering with
            //if(!askedForQuest)
            //{
            //tell GUI to ask player what they want to do
            //quests.GetComponent<PostVan>().checkForPost(currentBuilding);
            guiInfo.currentBuilding = currentBuilding;
            guiInfo.atBuilding = true;
				//askedForQuest = true;
			//}
		}


		if (rb.velocity.magnitude > 1f)
		{
			moving = true;
            //reset gui flag so gui can be swapped again on next stop
            guiInfo.changed = false;
		}
    }
    void Update()
    {
        //work with the closest building to the van/player

        if (buildingsInTrigger.Count > 0)
        {
            float distance = Mathf.Infinity;
            GameObject closest = null;
            for (int i = 0; i < buildingsInTrigger.Count; i++)
            {
                if (closest == null)
                {
                    closest = buildingsInTrigger[i];
                }
                else
                {
                    float temp = Vector3.Distance(closest.GetComponent<MeshRenderer>().bounds.center, buildingsInTrigger[i].GetComponent<MeshRenderer>().bounds.center);
                    if (temp < distance)
                    {
                        distance = temp;
                        closest = buildingsInTrigger[i];
                    }
                }
            }
            //save to public variable
            currentBuilding = closest;

            atBuilding = true;            
            guiInfo.currentBuilding = currentBuilding;

            //check if address has mail
            haveMail = MailForBuilding(currentBuilding);

            if (haveMail)
                mailIcon.SetActive(true);

            //check if a quest owner lives here

            haveQuest = QuestForBuilding(currentBuilding);

            if (haveQuest)
                questIcon.SetActive(true);
        }
        else
        {
            currentBuilding = null;
            atBuilding = false;
            guiInfo.currentBuilding = null;

            //turn off icons when leaving building
            mailIcon.SetActive(false);
            questIcon.SetActive(false);
        }
        


        if (atBuilding && !moving)
        {
            //tell gui we are leaving the post office/house etc
            guiInfo.currentBuilding = currentBuilding;
            guiInfo.atBuilding = true;
        }
        if (atBuilding && moving)
        {
            //tell gui we are at the post office
          
            guiInfo.atBuilding = false;
        }

        
       
    }  
    

    bool MailForBuilding(GameObject building)
    {
        bool mail = false;

        List<GameObject> mailList = mailListObject.GetComponent<PostVan>().mailList;
        for (int i = 0; i < mailList.Count; i++)
        {
            if (mailList[i] == building)
                mail = true;
        }

        return mail;
    }

    bool QuestForBuilding(GameObject building)
    {
        
        
        bool quest = false;

        //if at the psot office, display quest icon
        if (building.tag == "PostOffice")
        {
            quest = true;
            
        }

        else if (building.tag == "HouseCell")
        {
            //check all residents who live here, if they have a quest, return true (even if they are not home)
            List<GameObject> agentsHere = building.GetComponent<Residents>().agentsLivingAtThisCell;
            for (int i = 0; i < agentsHere.Count; i++)
            {
                //look for any agent that has something to do with a quest - dialogue options can reveal more
                if (agentsHere[i].GetComponent<AgentInfo>().fetchQuestTarget == true || agentsHere[i].GetComponent<AgentInfo>().fetchQuestOwner == true)
                    quest = true;
            }

            
        }

        return quest;
    }

	void OnTriggerEnter(Collider other) {

		if(other.tag =="PostOffice")
		{
            if (other.name == "Combined mesh")
            {
                //	Debug.Log("At The Post Office");
                //atBuilding = true;
                //currentBuilding = other.gameObject;
                //guiInfo.currentBuilding = currentBuilding;
                if(!buildingsInTrigger.Contains(other.gameObject))
                    buildingsInTrigger.Add(other.gameObject);
            }
		}

		if(other.tag == ("HouseCell"))
		{
            buildingsInTrigger.Add(other.gameObject);
            if (other.name == "Combined mesh")
            {
                if (!buildingsInTrigger.Contains(other.gameObject))
                    buildingsInTrigger.Add(other.gameObject);
            }
		}
	}
	void OnTriggerExit(Collider other) {
		
		if(other.tag =="PostOffice")
		{
            //	Debug.Log("At The Post Office");
            //	atBuilding = false;
            //    guiInfo.currentBuilding = null;
            buildingsInTrigger.Remove(other.gameObject);
        }

		if(other.tag == ("Building"))
		{
            buildingsInTrigger.Remove(other.gameObject);
        }
	}    
	void OnGUI()
	{
		if (atThePostOffice && moving)
		{
		GUI.Label(new Rect(0,0,100,100),"At the Post Office");
		}
		if (atThePostOffice && !moving )
		{
			GUI.Label(new Rect(0,0,100,100),"Stopped at the Post Office");
		}
	}

}
