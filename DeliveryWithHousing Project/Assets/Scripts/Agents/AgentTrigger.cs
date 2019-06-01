using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentTrigger : MonoBehaviour {
    //Detects if other agent is in its trigger.then lets the waypoint switcher script know

    public List<GameObject> agentsStationary = new List<GameObject>();
    public List<GameObject> agentsMoving= new List<GameObject>();
    public List<GameObject> agentsInTrigger = new List<GameObject>();
    public GameObject cubePrefab;
	//GameObject agent;
	public bool checkForWaypoints;
	public WaypointSwitcher waypointSwitcher;

    public WaypointPathfinder waypointPathfinderMiddle;
    public WaypointPathfinder waypointPathfinderNormal;
    public PathFollowingController pathFollowingController;
    public WaypointPlayer waypointPlayer;
    public OncomingCarChecker oncomingCarChecker;
    public UnitySteer.Behaviors.SteerForPathSimplified steerForPathSimplified;

   public List<Vector3> tempList = new List<Vector3>();
    public List<Vector3> reverseList =new List<Vector3>();
    public bool thisOnLeft;
    public bool thisInMiddle;
    public bool reverseTheCar;
  //  public string sideOfThis;
    public string sideOfOther;
 //   public string previousSide;
    public string normalSide;
    public bool wantingToStart;
    public bool inJunction;
    private bool inMiddle;
    public bool otherWantingToStart;
    
    // Use this for initialization
    void Awake () {
        //	agent = gameObject.transform.parent.gameObject;
        //	waypointSwitcher = agent.GetComponent<WaypointSwitcher>();

        
  //      waypointPlayer = transform.parent.GetComponent<WaypointPlayer>();
        waypointPathfinderNormal = GameObject.Find("Agents").transform.GetComponent<WaypointPathfinder>();
        waypointPathfinderMiddle = GameObject.Find("Agents").transform.Find("MiddleWaypoints").GetComponent<WaypointPathfinder>();
        //    pathFollowingController = transform.parent.gameObject.GetComponent<PathFollowingController>();

        //allocated in inspector - calling a function in a script before enabling this
        //steerForPathSimplified = transform.parent.gameObject.GetComponent<UnitySteer.Behaviors.SteerForPathSimplified>();
        //   normalSide = CheckSidesForGameObject(transform.parent.gameObject);
    //    normalSide = CheckSideFromPath();

    } 
    
    void OnEnable()
    {
        //when van enables this trigger, check our side
        //Agent should never be in the middle when this is called, as this is the only script which enables such behaviour
        //This script should make sure the agent always goes back to it's 'normal' waypoints

        //  normalSide = CheckSidesForGameObject(transform.parent.gameObject);
        if(waypointPlayer.Path.Count != 0)
            normalSide = CheckSideFromPath();
    }

    void Update()
    {
        if (wantingToStart && waypointPlayer.Path.Count != 0)
        {
            bool safe = true;

//            foreach (GameObject agent in oncomingCarChecker.agentsInLargeTrigger)
//            {
//                //if any of the agents in the zone are moving
//                if (agent.GetComponent<WaypointPlayer>().Path.Count != 0)
//                    safe = false;    
//            }


            if (oncomingCarChecker.agentsInLargeTrigger.Count != 0)
            {
                safe = false;
            }
            //no cars are moving
            if (safe)
                CheckAtStart();           

//            wantingToStart = false;
        }
    }
    public void CheckAtStart()
    {
        //look for an agent which is stationary and on our side. the trigger only looks for cars ahead of it due its relative position to the main body of the car
        normalSide = CheckSideFromPath();
        
       //prioritse this? or do not let exit get overwritten?
        foreach (GameObject agent in agentsInTrigger)
        {
            sideOfOther = CheckSidesForGameObject(agent.gameObject);
            //    normalSide = CheckSidesForGameObject(transform.parent.gameObject);
            DetermineActionEnter();
        }
        waypointPlayer.speed = 5;
        wantingToStart = false;
    }

    void CheckSidesForStationary()
    {  //if there is an agent in the list
        if (agentsStationary.Count > 0)
        {
            CheckThisAgentsSide();

            //go through the list of agents
            foreach (GameObject agent in agentsStationary)
            {
                bool agentOnLeft = false;
                //find out what waypoints are around the agent, left or right. Ignore middle and bus stop points
                RaycastHit[] hits = Physics.SphereCastAll(agent.transform.position + (Vector3.up * 5), 1f, Vector3.down, 10f);

                float distance = Mathf.Infinity;
                RaycastHit closestHit = hits[0];

                foreach (RaycastHit hit in hits)
                {
                    //we are only looking for hits which are our 'normal' waypoints
                    if (hit.transform.name != "WaypointNormal")
                        continue;

                    if (hit.transform.parent.name == "Waypoints Left" || hit.transform.parent.name == "Waypoints Right")
                    {
                        if (cubePrefab != null)
                        {
                            GameObject cube = Instantiate(cubePrefab, hit.transform.position, Quaternion.identity) as GameObject;
                        }

                        //find closesthit to agent
                        float diff = Vector3.Distance(agent.transform.position, hit.transform.position);
                        if (diff < distance)
                        {
                            closestHit = hit;
                            distance = diff;
                        }

                        if (closestHit.transform.parent.name == "Waypoints Left")
                        {
                            agentOnLeft = true;
                            //    if (transform.parent.GetComponent<WaypointPlayer>().Path.Count != 0)
                            //      Debug.Log("Agent on Left True");
                        }
                        else if (closestHit.transform.parent.name == "Waypoints Right")
                        {
                            agentOnLeft = false;
                        }
                    }
                }

                //now we know which side of the road our target agent is on,compare it
                if (thisOnLeft == agentOnLeft)
                {
                    //we are on the same side of the road, move to the middle
                    // MoveToTheMiddle();
                    ChangeWaypointsToMiddle();

                }
            }         //end of for each agent   
        }
    } //old

    void CheckSidesForMoving()
    {
 //       if (agentsMoving.Count > 0)
   //     {

            //only check the last car added to the list, the others will have already been checked

            //go through the list of agents
            //   foreach (GameObject agent in agentsMoving)
            //  {
           GameObject agent = agentsMoving[agentsMoving.Count - 1];
                bool agentOnLeft = false;
                bool agentInMiddle = false;
            //create and an expanding target of raycasting rings to grab any waypoints in the area
            List<RaycastHit> hitList = new List<RaycastHit>();

            for (float i = 0f; i < 5f; i += 0.5f)
            {
                RaycastHit[] hitsOwn = Physics.SphereCastAll(transform.position + (Vector3.up * 5), i, Vector3.down, 10f);

                //add these hits to the hitList
                foreach (RaycastHit hit in hitsOwn)
                {
                    if (!hitList.Contains(hit))
                        hitList.Add(hit);
                }
            }

            float distance = Mathf.Infinity;
            RaycastHit closestHit = hitList[0];

            //look for the closest waypoint to our target agent
            foreach (RaycastHit hit in hitList)
                {
                    //we are only looking for hits which are our 'normal' waypoints or middle waypoints
                    if (hit.transform.name == "WaypointNormal" || hit.transform.name == "WaypointMiddle")
                    {

                    //    if (hit.transform.parent.name == "Waypoints Left" || hit.transform.parent.name == "Waypoints Right")
                      //  {
                            if (cubePrefab != null)
                            {
                                GameObject cube = Instantiate(cubePrefab, hit.transform.position, Quaternion.identity) as GameObject;
                    
                            }

                            //find closesthit to agent
                            float diff = Vector3.Distance(agent.transform.position, hit.transform.position);
                            if (diff < distance)
                            {
                                closestHit = hit;
                                distance = diff;
                            }

                            if (closestHit.transform.parent.name == "WaypointsMiddle")
                            {
                                agentInMiddle = true;
                            }

                            else if (closestHit.transform.parent.name == "Waypoints Left")
                            {
                                agentOnLeft = true;
                            }
                            else if (closestHit.transform.parent.name == "Waypoints Right")
                            {
                                agentOnLeft = false;
                            }

                      //  }
                    }

                    if (!thisInMiddle && agentInMiddle)
                    {
                        //we have a problem
                        Debug.Log("This car in normal lane, oncoming car in middle");
                        //stop the car, we should be passive and let the agent through
                     //   Stop();
                   //     Reverse();
                    }

                    if(thisInMiddle && !agentInMiddle)
                    {
                        Debug.Log("This car in middle, oncoming car in normal lane");
                        //we should be let through, slow the car down
                 //       steerForPathSimplified.Weight = 0;

                        //do we need to reverse to let it through?
                        //if there are stationary cars in the trigger, we should reverse
                        //and give the other agent room
                        //Use the list from waypoint player of previous positions
                        //reverse until we are on a 'normal' waypoint
                       

                    }                    

                    if(thisInMiddle && agentInMiddle)
                    {
                        Debug.Log("Both cars in middle!");
                        //stop both cars!!
                    }

                    //now we know which side of the road our target agent is on,compare it
                    if (thisOnLeft == agentOnLeft || !thisOnLeft == !agentOnLeft)
                    {
                        //this means we have caught up with a car going the same way as us
                        //if we are on the same side as the agent we have caught up with, do nothing
                        //the stop trigger can deal with it
                        Debug.Log("Doing nothing");
                        //do nothing
                    }

                    if (thisOnLeft != agentOnLeft)
                    {
                        //we have an oncoming car, but we are safe in our respective lanes
                        //Debug.Log("Oncoming car - SAFE");
                        //don't debug this - fires as many times as hits there are
                    }
                }

        //    }         //end of for each agent   
   //     }
    } //old

    public string CheckSideFromPath()
    {
        //this is called at the start and checks ahead for the first waypoint that is not a 'bus stop' waypoint.
        //the waypoint found determines what side the car is on for the first stretch

     //   Debug.Log("CheckSideFromPath()");
        string target = null;

        LayerMask lm = LayerMask.GetMask("Waypoint");
        //returns the first name that is a 'normal' waypoint
        for (int i = 0; i < waypointPlayer.Path.Count; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(waypointPlayer.Path[i] + (Vector3.up * 5), Vector3.down, out hit, 10f, lm))
            { 
                if (hit.transform.name == "WaypointNormal")
                {
                    target = hit.transform.parent.name;
                    return target;
                }
            }
        }
        //Debug.Log(target + "Target");
        return target;
    }

    public string CheckSideAtJunction()
    {
   //     Debug.Log("Checking at junction");

  //      Transform cubePrefab = Resources.Load("Prefabs/Cube", typeof (Transform)) as Transform;

        LayerMask lm = LayerMask.GetMask("Waypoint","Junction");

        int amount = 50;
        if (waypointPlayer.Path.Count < 50)
            amount = waypointPlayer.Path.Count;

        for (int i = 0; i < amount; i++)
        {
            RaycastHit hit;

            if(Physics.Raycast(waypointPlayer.Path[i] + (Vector3.up * 50), Vector3.down, out hit, 100f, lm, QueryTriggerInteraction.Collide))
            {
                if (hit.transform.name == "WaypointNormal")
                {
               //     Transform cube = Instantiate(cubePrefab, hit.point, Quaternion.identity) as Transform;
                    return hit.transform.parent.name;
                }
            }          
        }
        return null;
    }

   public string CheckSidesForGameObject(GameObject agent)
    {
        //save 

        string side = null;
    //    bool agentOnLeft = false;
      //  bool agentInMiddle = false;
        //create and an expanding target of raycasting rings to grab any waypoints in the area
        List<RaycastHit> hitList = new List<RaycastHit>();

        for (float i = 0f; i < 5f; i += 1f)
        {
            RaycastHit[] hitsOwn = Physics.SphereCastAll(agent.transform.position + (Vector3.up * 5), i, Vector3.down, 10f);

            //add these hits to the hitList
            foreach (RaycastHit hit in hitsOwn)
            {
                if (!hitList.Contains(hit))
                    hitList.Add(hit);
            }
        }

        float distance = Mathf.Infinity;
        RaycastHit closestHit = hitList[0];

        //look for the closest waypoint to our target agent
        foreach (RaycastHit hit in hitList)
        {
            //we are only looking for hits which are our 'normal' waypoints or middle waypoints
            if (hit.transform.name == "WaypointNormal" || hit.transform.name == "WaypointMiddle")
            {

                //    if (hit.transform.parent.name == "Waypoints Left" || hit.transform.parent.name == "Waypoints Right")
                //  {
                if (cubePrefab != null)
                {
                    GameObject cube = Instantiate(cubePrefab, hit.transform.position, Quaternion.identity) as GameObject;
                }

                //find closesthit to agent
                float diff = Vector3.Distance(agent.transform.position, hit.transform.position);
                if (diff < distance)
                {
                    closestHit = hit;
                    distance = diff;
                }

                //if agent is moving
                if (agent.GetComponent<WaypointPlayer>().speed > 0)
                {
                    if (closestHit.transform.parent.name == "WaypointsMiddle")
                    {
                        //    agentInMiddle = true;
                        side = "MiddleMoving";
                    }

                    else if (closestHit.transform.parent.name == "Waypoints Left")
                    {
                        //   agentOnLeft = true;
                        side = "LeftMoving";
                    }
                    else if (closestHit.transform.parent.name == "Waypoints Right")
                    {
                        //   agentOnLeft = false;
                        side = "RightMoving";
                    }
                }
                //if agent is stationary
                else if (agent.GetComponent<WaypointPlayer>().speed == 0)
                {
                    if (closestHit.transform.parent.name == "WaypointsMiddle")
                    {
                        //    agentInMiddle = true;
                        side = "MiddleStationary";
                    }
                    else if (closestHit.transform.parent.name == "Waypoints Left")
                    {
                        //   agentOnLeft = true;
                        side = "LeftStationary";
                    }
                    else if (closestHit.transform.parent.name == "Waypoints Right")
                    {
                        //   agentOnLeft = false;
                        side = "RightStationary";
                    }
                }              
            }
        }

        //let this script know if the other agent is wanting to start
        otherWantingToStart = agent.transform.Find("Trigger").GetComponent<AgentTrigger>().wantingToStart;
        return side;
    }

    /// <summary>
    /// Determine the waypoints to use on Agent Entering Trigger
    /// </summary>
    void DetermineActionEnter()//string sideOfThis, string sideOfOther) -- using public bools atm
    {
       
        //moving actions if our car is middle
     
        if (normalSide == "MiddleMoving" && sideOfOther == "MiddleMoving")
        {          
        //    if(!otherWantingToStart)
                Stop();    //change this to a function in Stop script, to make one reverse?        
        }   
        
        //moving actions if our car is on 'normal' waypoints
                
        if(normalSide == "Waypoints Left" && sideOfOther == "LeftMoving")
        {
           //check if car is wanting to start, if so, it means he is stationary, pass in middle
        //   if(otherWantingToStart)
          //  {
                ChangeWaypointsToMiddle();
           // }

           //else, it means we are following a moving car

        }  

        if(normalSide == "Waypoints Left" && sideOfOther == "RightMoving")
        {
            //this is ok, each car in their own lane
            
        }

        //if there is an oncomgin car in the middle
        if((normalSide == "Waypoints Left" || normalSide == "Waypoints Right") && sideOfOther == "MiddleMoving")
        {
            //oncoming car in middle of road, stop the car and let it through
          //  if (!otherWantingToStart)
                Stop();
        }

        if (normalSide == "Waypoints Right" && sideOfOther == "RightMoving")
        {
            //this is ok, just follow   
        }

        if (normalSide == "Waypoints Right" && sideOfOther == "LeftMoving")
        {
            //this is ok, each car in their own lane
        }

        ///STATIONARY 
        ///
        //pass stationary car if we are on the same side.
        if ((sideOfOther == "LeftStationary" && normalSide == "Waypoints Left") || (sideOfOther == "RightStationary" && normalSide == "Waypoints Right"))
        {
            //if there are no oncoming cars
            if (oncomingCarChecker.agentsInLargeTrigger.Count == 0)
            {                
                ChangeWaypointsToMiddle();
            }
            //if there are oncoming cars // and none of them are wanting to start
            else if (oncomingCarChecker.agentsInLargeTrigger.Count > 0)
            {
                foreach (GameObject agent in oncomingCarChecker.agentsInLargeTrigger)
                {
                    //note what side our target agent is on
                    AgentTrigger agentTrigger = agent.GetComponentInChildren<AgentTrigger>();

                    //if agent is on the same side as us, just move to the middle to pass the stationary car, we can just follow this car we are checking
                    if (normalSide == agentTrigger.normalSide)
                    {
                        ChangeWaypointsToMiddle();
                    }
                    //if it does not equal it, we have an oncoming car, give way, but also plan to go to the middle
                    else if (normalSide != agentTrigger.normalSide)
                    {
                        if (!agentTrigger.wantingToStart)
                        {
                            Stop();
                        }
                        ChangeWaypointsToMiddle();
                    }
                }
            }
        }      
    }

    void DetermineActionExit()
    {
        //was this agent just giving way?
        if(waypointPlayer.speed == 0 && waypointPlayer.Path.Count !=0)
        {
            bool movingCars = false;
            //are there any moving cars in the trigger?
            foreach (GameObject agent in oncomingCarChecker.agentsInLargeTrigger)
            {
                if (agent.GetComponent<WaypointPlayer>().speed == 0)
                    movingCars = true;                    
            }

            //if there are no moving cars, start the car again
            if (!movingCars)
                waypointPlayer.speed = 5;

            return;
        }
        //if passing a stationary car
        //if no cars in the close trigger, it safe to move to the normal
        if(agentsInTrigger.Count == 0)
        {
            ChangeWaypointsToNormal();
            return;
        }

        //if there are cars still in the trigger, continue

    //    bool stationaryCarInTrigger = false;
        //  bool movingCarInTrigger = false;

        //if there are only moving cars in this trigger, go back to normal
        foreach (GameObject agent in agentsInTrigger)
        {
            //look for a stationary car
            if (agent.GetComponent<WaypointPlayer>().speed == 0)
            {
                sideOfOther = CheckSidesForGameObject(agent.gameObject);
                DetermineActionEnter();
             //   stationaryCarInTrigger = true;
            }
        }

        //if no stationary cars, move back to middle - could go further and check for what side? If stationary car on other side, this agent will hog middle
//        if (!stationaryCarInTrigger)
//        {
 //           ChangeWaypointsToNormal();         //doing this with determineaction above   
 //
//       }
        //if there are no stationary cars, but still agents in the trigger, it must mean, we have other moving cars in the trigger
        //we can continue along behind these cars, as if the car was oncoming,it should have given way
    }

    void CheckThisAgentsSide()
    {
        //Find out which side this agent is on

        //create and an expanding target of raycasting rings to grab any waypoints in the area
        List<RaycastHit> hitList = new List<RaycastHit>();

        for (float i = 0f; i < 5f; i+=0.5f)
        {
            RaycastHit[] hitsOwn = Physics.SphereCastAll(transform.position + (Vector3.up * 5), i, Vector3.down, 10f);

            //add these hits to the hitList
            foreach (RaycastHit hit in hitsOwn)
            {
                if(!hitList.Contains(hit))
                    hitList.Add(hit);
            }
        }

        float distanceOwn = Mathf.Infinity;
        RaycastHit closestHitOwn = hitList[0];
        thisOnLeft = false;
        thisInMiddle = false;
        foreach (RaycastHit hit in hitList)
        {
            
            if (hit.transform.name == "WaypointNormal" || hit.transform.name == "WaypointMiddle")
            {
              
                    //find closesthit to agent
                    float diff = Vector3.Distance(transform.position, hit.transform.position);
                    if (diff < distanceOwn)
                    {
                        closestHitOwn = hit;
                        distanceOwn = diff;
                    }                                
            }
        }

        if (closestHitOwn.transform.parent.name == "WaypointsMiddle")
        {
            thisInMiddle = true;
        }

        if (closestHitOwn.transform.parent.name == "Waypoints Left")
        {
            thisOnLeft = true;
        }

        if (closestHitOwn.transform.parent.name == "Waypoints Right")
        {
            thisOnLeft = false;
        }
    }

    void ChangeWaypointsToMiddle()
    {

        inMiddle = true;


        int amount = 50;
        if (waypointPlayer.Path.Count < 50)
            amount = waypointPlayer.Path.Count;

        LayerMask lm = LayerMask.GetMask("Junction", "WaypointMiddle");

        for (int i = 0; i < amount; i++)
        {
            RaycastHit[] hits = Physics.SphereCastAll(waypointPlayer.Path[i] + (Vector3.up * 50), 1f, Vector3.down, 100f, lm, QueryTriggerInteraction.Collide);
            foreach (RaycastHit hit in hits)
            {
                if (!inJunction)
                {
                    bool hitJunction = false;

                    //if we have hit a junction
                    if (hit.transform.name == "JunctionTrigger")
                    {
                        //force stop the the raycasts!!
                        //we do not want to raycast in to the junction and beyond because it can mess up which
                        //side of the road we use on the other side of the junction.
                        hitJunction = true;
                        i = amount;
                        break;
                    }

                    //we are only looking for hits which are our 'normal' waypoints
                    if (hit.transform.name == "WaypointMiddle")
                    {
                        if (!hitJunction)
                        {
                            waypointPlayer.Path[i] = hit.transform.position;

                            if (cubePrefab != null)
                            {
                                GameObject cube = Instantiate(cubePrefab, hit.transform.position, Quaternion.identity) as GameObject;
                            }
                        }
                    }
                }
                else if (inJunction)
                {
                    if (hit.transform.name == "JunctionTrigger")
                    {
                        //do not do anything, just keep on looking for middle waypoints
                    }

                    if (hit.transform.name == "WaypointMiddle")
                    {
                        //we have found our waypoints
                        waypointPlayer.Path[i] = hit.transform.position;
                    }
                }
            }
        }
    }

    void ChangeWaypointsToNormal()
    {
       // inMiddle = false;

   //     Debug.Log("changing to normal");
        //once an agent leaves the trigger, move waypoints back
        //look for middle waypoints and adjust path to match these

        string targetString = normalSide;
        LayerMask lm = LayerMask.GetMask("Junction", "Waypoint");
        int amount = 50;
        if (waypointPlayer.Path.Count < 50)
            amount = waypointPlayer.Path.Count;

        for (int i = 0; i < amount; i++)
        {
            RaycastHit[] hits = Physics.SphereCastAll(waypointPlayer.Path[i] + (Vector3.up * 50), 1f, Vector3.down, 100f, lm, QueryTriggerInteraction.Collide);
          

            foreach (RaycastHit hit in hits)
            {

                bool hitJunction = false;

                //if we have hit a junction
                if (hit.transform.name == "JunctionTrigger")
                {
                    //force stop the the raycasts!!
                    //we do not want to raycast in to the junction and beyond because it can mess up which
                    //side of the road we use on the other side of the junction.
                    hitJunction = true;
                    i = amount;
                    break;
                }

                if (hit.transform.name == "WaypointNormal")
                {
                    if (!hitJunction)
                    {
                        if (hit.transform.parent.transform.name == targetString)
                        {
                            waypointPlayer.Path[i] = hit.transform.position;

                            if (cubePrefab != null)
                            {
                                GameObject cube = Instantiate(cubePrefab, hit.transform.position, Quaternion.identity) as GameObject;
                            }
                        }
                    }
                }
            }
        }

        //now we have adjusted the path, send it off to the steering class
  //      pathFollowingController.pathPoints = waypointPlayer.Path;
  //      pathFollowingController.AssignPath();

    }

    void Stop()
    {
        waypointPlayer.speed = 0;
    }

    void Go()
    {
        waypointPlayer.speed = 10;
    }

   

    void Reverse()
    {
        //reverses the car until at a normal waypoint. Is called when this agent is in the middle of the road
        reverseList = waypointPlayer.previousPositions;
        //grab the previous positions list
        //List<Vector3> reverseList = waypointPlayer.previousPositions;
        //spin it round so the last point is the first
        //reverseList.Reverse(); // :p
        //stop the normal steering behavious
        steerForPathSimplified.enabled = false;

        reverseTheCar = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Agent")
        {

            sideOfOther = CheckSidesForGameObject(other.gameObject);

            //       if (agentsInTrigger.Count == 0)
            //           normalSide = CheckSidesForGameObject(transform.parent.gameObject);

            if (waypointPlayer.Path.Count != 0)
                DetermineActionEnter();

            //add to a list of what is in our trigger
            if (!agentsInTrigger.Contains(other.gameObject))
            {
                agentsInTrigger.Add(other.gameObject);
            }
        }

        if(other.tag == "Van")
        {
           

        }
    }

	void OnTriggerExit(Collider other)
	{
        if (other.tag != "Agent")
            return;

        if(waypointPlayer.Path.Count !=0)
        {
            // Go(); //and being passive?
        }

 //       sideOfOther = CheckSidesForGameObject(other.gameObject);
        //       normalSide = CheckSidesForGameObject(transform.parent.gameObject); //side Of This is determined only after a junction exit, or at the start of a journey, or when our van triggers it
        //  normalSide = previousSide;

        agentsInTrigger.Remove(other.gameObject);

        //if we are not stationary
        if (waypointPlayer.Path.Count != 0)
        {
            if(!inJunction)
                DetermineActionExit();
        }
        //if there are only moving cars in the trigger, move back to middle
              


    }

    //unused below
    void MoveToTheMiddle()
    {
        waypointPlayer.changingLanes = true;
        Debug.Log("called");
        //change map used from normal to middle waypoints and calculate new path

        tempList = waypointPlayer.Path;
        //make the target a few steps up the road, just enough to get get past the parked car
        Vector3 target = transform.parent.GetComponent<AgentBehaviour>().target.transform.position;
        //save the pathfinder list so we dont need to compute it again once we have passed the car
        tempList = waypointPlayer.Path;
        //clear the path previously made //this might get cleared when we change the pathfinder to 'middle' anyway
        waypointPlayer.Path.Clear();

        //point the waypoint player to the pathfinder which contains the middle waypoints
        waypointPlayer.waypointPathFinder = waypointPathfinderMiddle;

        //get the pathfinding class to find its way to the target again
        waypointPlayer.FindPath(transform.parent.position, target);
        //disable while the pathFollwer script gets new path
        pathFollowingController.enabled = false;

        StartCoroutine("StartPathFollower");
    }
   
    void MoveBackToTheSide2()
    {
        waypointPlayer.changingLanes = true;
        Debug.Log("called move back");
        //change map used from normal to middle waypoints and calculate new path

        //
        //make the target a few steps up the road, just enough to get get past the parked car
        Vector3 target = transform.parent.GetComponent<AgentBehaviour>().target.transform.position;

        //clear the path previously made //this might get cleared when we change the pathfinder to 'middle' anyway
        waypointPlayer.Path.Clear();

        //point the waypoint player to the pathfinder which contains the normal waypoints
        waypointPlayer.waypointPathFinder = waypointPathfinderNormal;

        //get the pathfinding class to find its way to the target again
        waypointPlayer.FindPath(transform.parent.position, target);
        //disable while the pathFollwer script gets new path
        pathFollowingController.enabled = false;

        StartCoroutine("StartPathFollower");
    }

    void MoveBackToTheSide()
    {

        waypointPlayer.Path.Clear();

        waypointPlayer.waypointPathFinder = waypointPathfinderNormal;
        waypointPlayer.Path = tempList;
        pathFollowingController.pathPoints = tempList;
        pathFollowingController.AssignPath();
    }

    IEnumerator StartPathFollower()
    {
        //This coroutine waits until the waypoint path finder finds a succesful path and populates its list
        //Once the list is full, it then starts the UnitySteer script by calling AsignPath()

        while (waypointPlayer.Path.Count < 2)
        {
            yield return new WaitForFixedUpdate();
        }

        pathFollowingController.pathPoints = waypointPlayer.Path;
        //   yield return new WaitForEndOfFrame();
        pathFollowingController.AssignPath();
        pathFollowingController.enabled = true;

        yield break;
    }
}
