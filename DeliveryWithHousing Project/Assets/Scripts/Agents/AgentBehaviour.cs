using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentBehaviour : MonoBehaviour {
	public GameObject home;
	public GameObject shop;
	public GameObject target;
	public GameObject[] houses;
    //	public bool finishedCurrentPath = true;
    //	public bool atTarget;
    public bool readyForNewJourney = true;
    public bool forceJourneyToFriend;

    public bool pathfinderFoundRoute;
//	public bool travelling;
//	public bool outward;
//	public bool back;
	public bool atHome = true; 
	public bool awaitingVisitor = false;
	public bool atOther;

    public bool returningHome = false;
    public bool visitingFriend = false;
    public bool hangout = false;
	public bool goToShop;
	public bool goHome;

    public AgentBehaviour agentBeingVisited;

	public PathFollowingController pfc;
//	public WaypointPathfinder wppf;
	public WaypointPlayer wpp;
    public AgentTrigger agentTrigger;
    public OncomingCarChecker oncomingCarChecker;
	public Vector3 lastPoint;
	private RaycastHit[] hits;
	public List<WaypointNode> nodeList = new List<WaypointNode>();

	public bool copyPath =false;

    private float walkSpeed = 0.01f;
	// Use this for initialization

    void Awake()
    {
        //enabled = false;

        //check if this agent has a car, disable this script if they have a car -- TODO //walking quests for these agents?
        //use partners car?

        if (GetComponent<AgentInfo>().haveOwnCar == false)
            enabled = false;
    }

	void Start () {
       // AsignHouse(); //old
        StartCoroutine("RemoveFromJunction");

		pfc = gameObject.GetComponent<AgentInfo>().car.GetComponent<PathFollowingController>();
		wpp = gameObject.GetComponent<AgentInfo>().car.GetComponent<WaypointPlayer>();
        agentTrigger = gameObject.GetComponent<AgentInfo>().car.transform.Find("Trigger").GetComponent<AgentTrigger>();
        oncomingCarChecker = gameObject.GetComponent<AgentInfo>().car.transform.Find("LargeTrigger").GetComponent<OncomingCarChecker>();
        //wppf = gameObject.GetComponent<WaypointPathfinder>();
    }

    IEnumerator RemoveFromJunction()
    {
        yield return new WaitForSeconds(1);

        //do a ray check from agetn's position looking for any junction triggers
        //if found, grab the list of agents in the junction and remove this agent
        //check to see if agent is inside a junction trigger
        RaycastHit[] hits = Physics.RaycastAll(transform.position + (Vector3.up * 50), Vector3.down, 100f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Collide);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.name == "JunctionTrigger")
            {
                //if the collider/trigger we have hit is a junction trigger, add this gameobjeft to the list of agents in the junction
                hit.transform.GetComponent<JunctionChecker2>().agentsInTrigger.Remove(gameObject);
                Debug.Log("inside Junction Removal");
            }
        }
    }
    void AsignHouse()
    {
        //choose a random house to be a home
        houses = GameObject.FindGameObjectsWithTag("Building");
        shop = GameObject.FindGameObjectWithTag("Shop");
        home = gameObject.GetComponent<AgentInfo>().home;
        atHome = true;

        //place agent at house
        //grab the first waypoint which is childed to the home game object (one up from home- is Cube) - could be randomised? //car parking?
        Vector3 pos = home.transform.parent.transform.Find("Waypoints").GetChild(5).transform.position;
        gameObject.GetComponent<AgentInfo>().space1taken = true;
        //grab the rotation
        Quaternion rot = home.transform.parent.transform.Find("Waypoints").rotation;
        //the house is facing towards the road, so we need to spin it 90 degrees to the get the agent facing along the road
        rot *= Quaternion.Euler(new Vector3(0, -90, 0));
        transform.position = pos;
        transform.rotation = rot;
    } //not being used. house asigned earlier in SpawnAgents

	IEnumerator OutwardJourneyToFriend()
	{
        
        //disable while the pathFollower script gets new path
        pfc.enabled = false;
        wpp.enabled = false;

        

        //agent is not in the car. Move towards car, once in car, start engine.
        GameObject car = GetComponent<AgentInfo>().car;
        while (Vector3.Distance(transform.position, car.transform.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, car.transform.position, 0.05f);
          //  Debug.Log("returning");
            yield return new WaitForEndOfFrame();
        }

        if (Vector3.Distance(transform.position, car.transform.position) <= 0.01f)
        {
            transform.parent = car.transform;
        }

        
        //set the target to the visitor's space in front of the house
        GameObject targetsHome = agentBeingVisited.GetComponent<AgentInfo>().home;
        //get the parking space for visitors
        //just using space 2 atm
        Vector3 targetSpace = targetsHome.transform.parent.Find("Waypoints Parent").GetChild(1).transform.position;

        //tell our waypoint player to find a path from current position to the target space
        wpp.FindPath(car.transform.position, targetSpace);
            
        //start the coroutine which waits for the path to be filled before calling the move function
        StartCoroutine("StartPathFollower");

     
        yield break;
    }
    IEnumerator ReturnHome()
    {
        //disable while the pathFollower script gets new path
        pfc.enabled = false;
        wpp.enabled = false;

        //called from waypoint player script when a journey to a friend/shop has been completed
        returningHome = true;

        //get to car
        GameObject car = GetComponent<AgentInfo>().car;
        while (Vector3.Distance(transform.position, car.transform.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, car.transform.position, walkSpeed);
            yield return new WaitForEndOfFrame();
        }

        if (Vector3.Distance(transform.position, car.transform.position) <= 0.01f)
        {
            transform.parent = car.transform;
        }

        

        //tell our waypointPlayer to find a path home
        Vector3 targetSpace = GetComponent<AgentInfo>().home.transform.parent.Find("Waypoints Parent").GetChild(0).transform.position;
        wpp.FindPath(GetComponent<AgentInfo>().car.transform.position, targetSpace);
        //start the coroutine which waits for the pathfinder to have found a path
        StartCoroutine("StartPathFollower");

        yield break;
    }
    public IEnumerator GoInsideHome()
    {
        //make agent get out car and go close to agent they are visiting
        transform.parent = null;
        Vector3 home = GetComponent<AgentInfo>().home.GetComponent<MeshRenderer>().bounds.center;
        while (Vector3.Distance(transform.position, home) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, home, walkSpeed);
            yield return new WaitForEndOfFrame();
        }
        //reset flags
        returningHome = false;
        atHome = true;
        readyForNewJourney = true;

        yield break;
    }
    IEnumerator StartPathFollower()
	{
		//This coroutine waits until the waypoint path finder finds a succesful path and populates its list
		//Once the list is full, it then starts the UnitySteer script by calling AsignPath()
        
		yield return new WaitForSeconds(1); // find a good solution, only asign path once pathfinder returns a path

        pfc.pathPoints = wpp.Path;
        pfc.AssignPath();

        wpp.enabled = true;
        pfc.enabled = true;
    
		yield break;
	}

    public IEnumerator StartHangout()
    {
        

        //make agent get out car and go close to agent they are visiting
        transform.parent = null;
        while (Vector3.Distance(transform.position, agentBeingVisited.transform.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, agentBeingVisited.transform.position, walkSpeed);
            yield return new WaitForEndOfFrame();
        }
        hangout = true;
        agentBeingVisited.hangout = true;

        //hang out at friend's for a random amount of time
        yield return new WaitForSeconds(Random.Range(10, 20));

        hangout = false;
        agentBeingVisited.hangout = false;
        //let the agent we were visiting know we are leaving so they can leave their house
        agentBeingVisited.awaitingVisitor = false;
        //we are onthe way home now - no longer visiting friend
        visitingFriend = false;
        

        //return home after this time
        StartCoroutine("ReturnHome");

        
        yield break;
    }

   

    IEnumerator VisitShop()
	{	
		yield break;
	}

    Vector3 AsignSpace()
    {
        List<GameObject> agentList = GameObject.Find("Agents").GetComponent<AgentList>().agentList;
        int random = Random.Range(0, agentList.Count);
        AgentInfo agentInfo = agentList[random].GetComponent<AgentInfo>();
        //asign a free car parking space. Spaces are asigned at the start of each journey
        Vector3 space = Vector3.zero;
        if (!agentInfo.space1taken)
        {
            space = home.transform.parent.transform.Find("Waypoints").GetChild(0).transform.position;
        }
        else if (!agentInfo.space2taken)
        {
            space = home.transform.parent.transform.Find("Waypoints").GetChild(10).transform.position;
        }
        else if (agentInfo.space3taken)
        {
            space = home.transform.parent.transform.Find("Waypoints").GetChild(20).transform.position;
        }

        if (space == Vector3.zero)
        {
            //reset 
            readyForNewJourney = true;

            Debug.Log("car park full");
            return space;
        }

        return space;
    }
    // Update is called once per frame
    void Decide()
    {
        if (readyForNewJourney && !awaitingVisitor)
        {
            int random = Random.Range(0, 5000);

            if (random == 0 || forceJourneyToFriend == true)
            {

                //find the agentlist game object in the scene
                List<GameObject> agentList = GameObject.Find("Agents").GetComponent<AgentList>().agentList;

                //look for an agent who is at home
                int randomAgent = Random.Range(0, agentList.Count);
                agentBeingVisited = agentList[randomAgent].GetComponent<AgentBehaviour>();
                //make sure we do not visit ourselves or vist someone who is already waiting on a visitor or is on their way to see someone
                if (agentList[randomAgent] == gameObject || agentList[randomAgent].GetComponent<AgentBehaviour>().awaitingVisitor || agentList[randomAgent].GetComponent<AgentBehaviour>().visitingFriend) //could change at home but waiting on visitor
                {
                    Debug.Log("agent has visitor or is ourselves");
                    //wait for next frame and start decision making process again
                    return;
                   
                }
                //stop the agent we are visiting from moving, this agent presumably telephoned them and told them we were visiting!
                agentBeingVisited.awaitingVisitor = true;

                //set flags so we do not try and create a new journey // could alternatively disable the script?
                readyForNewJourney = false;
                forceJourneyToFriend = false;
                //we are now planning a journey, so we are not home
                atHome = false;
                //this agent is now off to visit a friend
                visitingFriend = true;

                StartCoroutine("OutwardJourneyToFriend");

                //check to see if agent is inside a junction trigger
                RaycastHit[] hits = Physics.RaycastAll(transform.position + (Vector3.up * 50), Vector3.down, 100f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Collide);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.name == "JunctionTrigger")
                    {
                        //if the collider/trigger we have hit is a junction trigger, add this gameobjeft to the list of agents in the junction
                        hit.transform.GetComponent<JunctionChecker2>().agentsInTrigger.Add(gameObject);
                        Debug.Log("inside Junction");
                    }
                }


            }
        }
    }

	void Update ()
    {

        Decide();
	
		

	//		if (random ==1)
	//			goToShop = true;
	//		if (random ==2)
	//			goHome = true;


			if(goToShop)
			{
				StartCoroutine("VisitShop");
				goToShop = false;
			}

			if(goHome)
			{

			}

			
		

	}

}
