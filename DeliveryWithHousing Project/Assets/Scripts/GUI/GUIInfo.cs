using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUIInfo : MonoBehaviour {

    //handles entry points to conversation trees. Uses Conversations.cs to change on screen text
    //receives info from House detect to let it know it is at a building, therefore needing to bring text on screen

    public PostOffice postOffice;
    public GameObject currentBuilding;
    public GameObject resident;
    //game objects for gui
    private GameObject canvas;//parent object for gui elements
    public GameObject panel;//background panel
    public GameObject building;//background panel
    public GameObject textObject;//text object which dsiplays on the panel
    private Text textPanel;//component holding text

    //buttons
    public GameObject button1;
    private GameObject button2;
    private GameObject button3;
    private GameObject knockButton;
    private GameObject leaveButton;

    private GameObject quests;
    private List<GameObject> mailList;
    public PostVan postvan;
    
    //flags for house detect script to change   
    public bool atBuilding;
    public bool moving;
    public bool changed;
    public bool inConversation = false;

    //conversatoin script attached to this object
    private Conversation conversation;
    private HouseDetect houseDetect;

   
    
    // Use this for initialization
    void Start ()
    {
        
        //asign game objects
        canvas = GameObject.Find("Canvas");
        textObject = canvas.transform.Find("Text").gameObject;
        panel = canvas.transform.Find("Panel").gameObject;
        building = canvas.transform.Find("Building").gameObject;
        textPanel = textObject.GetComponent<Text>();

        //convo object
        conversation = GetComponent<Conversation>();
        houseDetect = transform.parent.Find("HouseDetect").GetComponent<HouseDetect>();
    }

    void Update()
    {
        if (!atBuilding)
        {
            building.SetActive(false);
        }
        else if (atBuilding)
        {
            building.SetActive(true);
            building.GetComponent<Text>().text = currentBuilding.tag + " Press E to interact";

            if (Input.GetKeyDown("e") && inConversation == false)
            {
                if (currentBuilding.tag == "PostOffice")
                {
                    inConversation = true;
                    conversation.AtPostOffice();
                    //  changed = true;
                }

                else if (currentBuilding.tag == "HouseCell")
                {
                    inConversation = true;

                    conversation.currentBuilding = currentBuilding;

                    conversation.AtHouse();
                   
                    // changed = true;
                }
            }
        }
        
        
    }


  
    
}
