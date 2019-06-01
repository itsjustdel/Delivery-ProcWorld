using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Conversation : MonoBehaviour {

    //attached to player// changes the test on screen and what the buttons do

    public GameObject GUIParent;
    public GameObject panel;
    public GameObject text;    
    public GameObject button1;
    public GameObject button2;
    public GameObject button3;

    private Text mainText;
    private Text  button1Text;
    private Text button2Text;
    private Text button3Text;

    public GameObject currentBuilding;
    public GameObject resident;

    private PostOffice postOffice;
    private GameObject quests;
    private List<GameObject> mailList;
    private PostVan postVan;
    private List<GameObject> agentList;
    private List<FetchQuests.FetchQuest> fetchQuestsList = new List<FetchQuests.FetchQuest>();
    public GUIInfo guiInfo;
    public HouseDetect houseDetect;
    // Use this for initialization

    void OnGUI()
    {

        GUI.Label(new Rect(0, 0, 100, 100), "Mail In Van " + postVan.mailList.Count.ToString());

    }

    
    void Start ()
    {

        guiInfo = GetComponent<GUIInfo>();

        GUIParent= GameObject.Find("Canvas");
        text = GUIParent.transform.Find("Text").gameObject;
        panel = GUIParent.transform.Find("Panel").gameObject;
        button1 = GUIParent.transform.GetChild(0).gameObject;
        button2 = GUIParent.transform.GetChild(1).gameObject;
        button3 = GUIParent.transform.GetChild(2).gameObject;

        mainText = text.GetComponent<Text>();
        button1Text = button1.transform.GetChild(0).GetComponent<Text>();
        button2Text = button2.transform.GetChild(0).GetComponent<Text>();
        button3Text = button3.transform.GetChild(0).GetComponent<Text>();

        //find post office
        GameObject[] poArray = GameObject.FindGameObjectsWithTag("PostOffice");
        quests = transform.parent.Find("Quests").gameObject;

        foreach (GameObject go in poArray)
        {
            if (go.name == "Combined mesh")
            {
                postOffice = go.GetComponent<PostOffice>();
            }
        }

        //list of mail
        mailList = quests.GetComponent<PostVan>().mailList;
        postVan = transform.parent.Find("Quests").GetComponent<PostVan>();

        //all residents list
        agentList = GameObject.Find("Agents").GetComponent<AgentList>().agentList;

        //fetch quest list location
        fetchQuestsList = quests.GetComponent<FetchQuests>().fetchQuestsList;

        //house detect script
        houseDetect = transform.parent.Find("HouseDetect").GetComponent<HouseDetect>();
    }

    #region Entry Points
    public void AtHouse()
    {
        if (houseDetect.haveMail)
        {
            //mainText.text = "You have mail for this house.";
            //text.SetActive(true);

            panel.SetActive(true);

            //buttons
            button1.SetActive(true);
            button1Text.text = "Post";
            button1.GetComponent<Button>().onClick.RemoveAllListeners();
            button1.GetComponent<Button>().onClick.AddListener(Post);

            button2.SetActive(true);
            button2Text.text = "Knock";
            button2.GetComponent<Button>().onClick.RemoveAllListeners();
            button2.GetComponent<Button>().onClick.AddListener(KnockDoor);

            button3.SetActive(true);
            button3Text.text = "Leave";
        }

        if (!houseDetect.haveMail)
        {
            button2.SetActive(true);
            button2Text.text = "Knock";
            button2.GetComponent<Button>().onClick.RemoveAllListeners();
            button2.GetComponent<Button>().onClick.AddListener(KnockDoor);

            button3.SetActive(true);
            button3Text.text = "Leave";
        }
    }
    public void AtHouseNoPost()
    {

    }
    public void AtPostOffice()
    {

        if (postOffice.newMailDay)
        {
            mainText.text = "You have arrived at the Post Office, collect mail?";
            text.SetActive(true);

            //buttons
            button1.SetActive(true);
            button1Text.text = "Yes";
            button1.GetComponent<Button>().onClick.RemoveAllListeners();
            button1.GetComponent<Button>().onClick.AddListener(CollectMail);

            button2.SetActive(true);
            button2Text.text = "No";
            button2.GetComponent<Button>().onClick.RemoveAllListeners();
            button2.GetComponent<Button>().onClick.AddListener(TurnOffGUI);

            button3.SetActive(false);
        }
        else if(!postOffice.newMailDay)
        {
            mainText.text = "You haven't delivered all the mail for today";

            button1.SetActive(true);
            button1Text.text = "Leave";
            button1.GetComponent<Button>().onClick.RemoveAllListeners();
            button1.GetComponent<Button>().onClick.AddListener(TurnOffGUI);

            button2.SetActive(false);
            button3.SetActive(false);
        }

    }

    #endregion

    #region internalChat
    void CollectMail()
    {

        postOffice.AsignPost();

        mainText.text = "You have collected mail, time to deliver it";
        text.SetActive(true);        

        button1Text.text = "Leave the Post Office";
        button1.GetComponent<Button>().onClick.RemoveAllListeners();
        button1.GetComponent<Button>().onClick.AddListener(TurnOffGUI);

        button2.SetActive(false);
        button3.SetActive(false);
    }

    void Post()
    {
        //post and remove mail from bag
        GameObject quests = transform.parent.Find("Quests").gameObject;
        List<GameObject> mailList = quests.GetComponent<PostVan>().mailList;
        mailList.Remove(currentBuilding);

        //if we talked to a resident before posting, improve opinion of player
        if (resident != null)
        {
            //successfully given mail to this house, improve the agents' opinion
            if (resident.GetComponent<AgentInfo>().opinionOfPlayer < 1)
                resident.GetComponent<AgentInfo>().opinionOfPlayer++;
        }

        if(mailList.Count > 0)
            mainText.text = "Mail delivered, continue with your deliveries";
        else
            mainText.text = "All the mail has been delivered, head back to the Post Office";

        button1Text.text = "Leave";
        button1.GetComponent<Button>().onClick.RemoveAllListeners();
        button1.GetComponent<Button>().onClick.AddListener(TurnOffGUI);

        button2.SetActive(false);
        button3.SetActive(false);

        currentBuilding.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Green", typeof(Material)) as Material;
    }


    
    void KnockDoor()
    {
        //find out if anyone is home
        // find out if anyone is in,list of agents for this cell held in Residents script attached to the cell
        List<GameObject> residents = currentBuilding.GetComponent<Residents>().agentsLivingAtThisCell;
        bool someoneIsHome = false;
        //GameObject resident = null;

        //create list of who is home
        List<GameObject> residentsAtHome = new List<GameObject>();
        foreach (GameObject r in residents)
        {
            if (r.GetComponent<AgentBehaviour>().atHome == true)
            {                
                someoneIsHome = true;                
                residentsAtHome.Add(r);
            }
        }

        //check for an empty house
        if (residentsAtHome.Count == 0)
        {
            //nobody is home
            NobodyHome();

            return;

        }

        //reset variable, will be saved from previous house
        resident = null;
        //if any resident has a quest, force them to the door

        for (int i = 0; i < residentsAtHome.Count; i++)
        {
            if(residentsAtHome[i].GetComponent<AgentInfo>().fetchQuestOwner || residentsAtHome[i].GetComponent<AgentInfo>().fetchQuestTarget)
            {
                resident = residentsAtHome[i];
            }
        }

        //if no quest resident, randomly choose a resident from the list of who is at home
        if(resident == null)
            resident = residentsAtHome[Random.Range(0, residentsAtHome.Count)];
        
        int opinionOfPlayer = resident.GetComponent<AgentInfo>().opinionOfPlayer;

        if(someoneIsHome)
        {
            mainText.text = "Hello";
            text.SetActive(true);
            //if we have mail
            if (houseDetect.haveMail)
            {
                if (opinionOfPlayer == 0)
                {
                    mainText.text = resident.name + " - Hello? - opinion 0";

                    button1Text.text = "Hello, I'm the postman, I have your mail - 0";
                    button1.GetComponent<Button>().onClick.AddListener(Introduction1);
                    button1.SetActive(true);

                    button2.SetActive(false);
                    button3.SetActive(false);
                }
                else if (opinionOfPlayer == 1)
                {
                    mainText.text = resident.name + " - Hi posty - 1";

                    button1Text.text = "Hello again - 1";
                    button1.GetComponent<Button>().onClick.AddListener(Introduction2);
                    button1.SetActive(true);

                    button2.SetActive(false);
                    button3.SetActive(false);
                    //start 1 convo
                }
                else if (opinionOfPlayer == 2)
                {
                    mainText.text = resident.name + " - Hi friend - opinion 2";
                    button1Text.text = "Hello, nice to see you again - 2";
                    button1.SetActive(true);
                    button2.SetActive(false);
                    button3.SetActive(false);
                    //start 2 convo
                }
                else if (opinionOfPlayer == 3)
                {
                    mainText.text = resident.name + " - Hi close friend - opinion 3";
                    button1Text.text = "Hello, glad to see you - 3";
                    button1.SetActive(true);

                    button2.SetActive(false);
                    button3.SetActive(false);
                    //start 3 convo
                }
                else if (opinionOfPlayer == 4)
                {
                    mainText.text = resident.name + " - Hi best friend - opinion 4";
                    button1Text.text = "Hello, great to see you - 4";
                    button1.SetActive(true);

                    button2.SetActive(false);
                    button3.SetActive(false);
                    //start 4 convo
                }
            }

            else if(!houseDetect.haveMail)
            {
                if (opinionOfPlayer == 0)
                {
                    mainText.text = resident.name + " - Hello? - opinion 0";

                    button1Text.text = "Hello, no mail today, I was just wanting to say hello - 0";
                    button1.GetComponent<Button>().onClick.AddListener(Introduction1);
                    button1.SetActive(true);

                    button2.SetActive(false);
                    button3.SetActive(false);
                }
                else if (opinionOfPlayer == 1)
                {
                    mainText.text = resident.name + " - Hi posty - 1";

                    button1Text.text = "Hello again - 1";
                    button1.GetComponent<Button>().onClick.AddListener(Introduction2);

                    button1.SetActive(true);

                    button2.SetActive(false);
                    button3.SetActive(false);

                    //start 1 convo
                }
                else if (opinionOfPlayer == 2)
                {
                    mainText.text = resident.name + " - Hi friend - opinion 2";
                    button1Text.text = "Hello, nice to see you again - 2";
                    button1.SetActive(true);

                    button2.SetActive(false);
                    button3.SetActive(false);
                    //start 2 convo
                }
                else if (opinionOfPlayer == 3)
                {
                    mainText.text = resident.name + " - Hi close friend - opinion 3";
                    button1Text.text = "Hello, glad to see you - 3";
                    button1.SetActive(true);

                    button2.SetActive(false);
                    button3.SetActive(false);
                    //start 3 convo
                }
                else if (opinionOfPlayer == 4)
                {
                    mainText.text = resident.name + " - Hi best friend - opinion 4";
                    button1Text.text = "Hello, great to see you - 4";
                    button1.SetActive(true);

                    button2.SetActive(false);
                    button3.SetActive(false);
                    //start 4 convo
                }
            }

            //if this agent has something for us
            if(resident.GetComponent<AgentInfo>().fetchQuestTarget)
            {
                //use button 3 for quests
                button3Text.text = "Fetch Quest, retrieve object from agent";
                button3.GetComponent<Button>().onClick.AddListener(FetchQuestRetrieve);

                button3.SetActive(true);
            }

            //if this agent is waiting for an object we have an object for this agent
            if(resident.GetComponent<AgentInfo>().fetchQuestOwner)
            {
                //find fetch quest attached to this player
                for (int i = 0; i < fetchQuestsList.Count; i++)
                {
                    FetchQuests.FetchQuest fq = fetchQuestsList[i];
                    if(fq.owner == resident)
                    {
                        //we have returned to the correct agent with a quest item

                        //use button 3 for quests
                        button3Text.text = "Fetch Quest, return object to this agent";
                        button3.GetComponent<Button>().onClick.AddListener(FetchQuestReturnObject);

                        button3.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("not this resident");
                    }
                }
            }
        }

        else
        {

        }

        
    //    text.SetActive(true);
    //    panel.SetActive(true);
    //    button1.SetActive(true);
    //    button2.SetActive(true);
    //    button3.SetActive(true);

    } 

    void NobodyHome()
    {
        mainText.text = "Nobody is home";
        text.SetActive(true);

        //buttons
        button1.SetActive(true);
        button1Text.text = "Post";
        button1.GetComponent<Button>().onClick.RemoveAllListeners();
        button1.GetComponent<Button>().onClick.AddListener(Post);

        button2.SetActive(true);
        button2Text.text = "Knock";
        button2.GetComponent<Button>().onClick.RemoveAllListeners();
        button2.GetComponent<Button>().onClick.AddListener(KnockDoor);

        button3.SetActive(true);
        button3Text.text = "Leave";
        button3.GetComponent<Button>().onClick.RemoveAllListeners();
        button3.GetComponent<Button>().onClick.AddListener(TurnOffGUI);
    }

    void Introduction1()
    {
        mainText.text = resident.name + " - Thank you, it's nice having a new postman, the last postman was, a bit, strange";

        button1Text.text = "Nice to meet you";
        button1.GetComponent<Button>().onClick.RemoveAllListeners();
        button1.GetComponent<Button>().onClick.AddListener(Post);

        button1.SetActive(true);
        button2.SetActive(false);
        button3.SetActive(false);   
    }

    void Introduction2()
    {
        mainText.text = resident.name + " - Would you be able to do me a favour?";

        button1Text.text = "What kind of favour?";
        button1.GetComponent<Button>().onClick.RemoveAllListeners();
        button1.GetComponent<Button>().onClick.AddListener(Favour);
        button1.SetActive(true);

        button2Text.text = "No";
        button2.GetComponent<Button>().onClick.RemoveAllListeners();
        button2.GetComponent<Button>().onClick.AddListener(TurnOffGUI);
        button2.GetComponent<Button>().onClick.AddListener(LowerOpinion);

        button2.SetActive(true);

        button3.SetActive(false);
    }

    void LowerOpinion()
    {
        //opinion lowers        
        resident.GetComponent<AgentInfo>().opinionOfPlayer--;
    }

    void Favour()
    {
        //choose a favour -- perhaps this can be determined from character traits/stats. e.g - mobility low, fetch quest -- mob includes speed of movement, driving, walking
        //                                                                                     intelligence low, puzzle? -- what they notice? (in social?)
        //                                                                                     social low, messenger/feelings for someone. social is how nice they are to be around, including looks, humour
        
        //if mobility quest
        mainText.text = resident.name + " - I need you to do me get me something, these bones aren't what the used to be";

        button1Text.text = "No problem, I can get that on my way back";
        button1.GetComponent<Button>().onClick.RemoveAllListeners();
        button1.GetComponent<Button>().onClick.AddListener(FetchQuest);
        button1.SetActive(true);

        button2Text.text = "No";
        button2.GetComponent<Button>().onClick.RemoveAllListeners();
        button2.GetComponent<Button>().onClick.AddListener(TurnOffGUI);
        button2.GetComponent<Button>().onClick.AddListener(LowerOpinion);
        button2.SetActive(true); 

        button3.SetActive(false);
    }

    void FetchQuest()
    {        
        //choose where the object is -- shop, person etc

        //person
        //choose an agent at random
        int random = Random.Range(0, agentList.Count);
        //do not visit ourselves,
        while (agentList[random] == resident)
        {
            random = Random.Range(0, agentList.Count);
        }
        // or others in this house
        List<GameObject> agentsAtThisCell = currentBuilding.GetComponent<Residents>().agentsLivingAtThisCell;
        for (int i = 0; i < agentsAtThisCell.Count; i++)
        {
            //force a change if we meet ourselves or others at this address - we do not need to get items from the same address
            while (agentList[random] == agentsAtThisCell[i])
            {
                random = Random.Range(0, agentList.Count);
            }
        }

        //choose what the object is -- get a list of possible objects from location
        //location is person/agent
        //choose item from agent item list
        GameObject item = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        item.name = "item";
        //move to home
        item.transform.position = agentList[random].GetComponent<AgentInfo>().home.GetComponent<MeshRenderer>().bounds.center;
        item.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("RosePink") as Material;

        mainText.text = resident.name + "Please get " + item.name.ToString() + " from" + agentList[random].name.ToString();

        //add map item
        GameObject mapBlipTarget = GameObject.CreatePrimitive(PrimitiveType.Quad);
        mapBlipTarget.transform.position = item.transform.position;
        mapBlipTarget.transform.rotation = Quaternion.Euler(90, 0, 0);
        mapBlipTarget.transform.localScale *= 20;
        mapBlipTarget.layer = 15; //MiniMapBlips
        mapBlipTarget.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;

        GameObject mapBlipOwner = GameObject.CreatePrimitive(PrimitiveType.Quad);
        mapBlipOwner.transform.position = resident.transform.position;
        mapBlipOwner.transform.rotation = Quaternion.Euler(90, 0, 0);
        mapBlipOwner.transform.localScale *= 20;
        mapBlipOwner.layer = 15; //MiniMapBlips
        mapBlipOwner.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;

        //send this info to the fetch quests script on van/quests
        FetchQuests.FetchQuest fq = new FetchQuests.FetchQuest();
        fq.owner = resident;
        fq.target = agentList[random];
        fq.item = item;
        fq.minimapBlipTarget = mapBlipTarget;
        fq.minimapBlipOwner = mapBlipOwner;
        fetchQuestsList.Add(fq);

        //set flag on agent to show they own or are a target for a quest item        
        resident.GetComponent<AgentInfo>().fetchQuestOwner = true;
        agentList[random].GetComponent<AgentInfo>().fetchQuestTarget = true;

        //set button
        button1Text.text = "Sure, I'll see what I can do";
        button1.GetComponent<Button>().onClick.RemoveAllListeners();
        button1.GetComponent<Button>().onClick.AddListener(TurnOffGUI);
        button1.SetActive(true);


        button2.SetActive(false);
        button3.SetActive(false);


    }

    void FetchQuestRetrieve()
    {
        //locate object. It is stored in fetch quest list

        //search fetch quest list for a match on agent
        for (int i = 0; i < fetchQuestsList.Count; i++)
        {
            FetchQuests.FetchQuest fq = fetchQuestsList[i];
            //if target matches the resident we are talking to -- need support for when mulitple people are living here
            if(fq.target == resident)
            {
                fq.item.transform.position = postVan.transform.position;
                fq.item.transform.parent = postVan.transform;   //once player is in, need to transport from agent to van

                //flag that we have this item
                fq.playerHasItem = true;

                //reset quest flag on this agent, player is taking over responsibilty for this object
                resident.GetComponent<AgentInfo>().fetchQuestTarget = false;

                //so we can remove the minimap blip for this agent
                Destroy(fq.minimapBlipTarget);
            }
        }

        TurnOffGUI();

    }
    void FetchQuestReturnObject()
    {
        //locate object. It is stored in fetch quest list

        //search fetch quest list for a match on agent
        for (int i = 0; i < fetchQuestsList.Count; i++)
        {
            FetchQuests.FetchQuest fq = fetchQuestsList[i];
            //if target matches the resident we are talking to -- need support for when mulitple people are living here
            if (fq.owner == resident)
            {
                fq.item.transform.position = resident.transform.position;
                //parent it to the house? -- what to do with these quest objects? destroying atm
                Destroy(fq.item);

                //can now remove fetchquest from list
                fetchQuestsList.Remove(fetchQuestsList[i]);

                //reset flags on agents
                resident.GetComponent<AgentInfo>().fetchQuestOwner = false;

                //destroy minimap blips
                Destroy(fq.minimapBlipOwner);

                //improve relationship with this agent
                resident.GetComponent<AgentInfo>().opinionOfPlayer++;
                
                //removing from list while iterating through is bad - force stop loop
                break;
            }
        }

        TurnOffGUI();
    }
    #endregion

    #region exits

    void TurnOffGUI()
    {
        text.SetActive(false);
        panel.SetActive(false);
        button1.SetActive(false);
        button2.SetActive(false);
        button3.SetActive(false);

        guiInfo.inConversation = false;
    }
    #endregion

}
