using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointPlayer : Pathfinding {

    public GameObject agent;
    public AgentBehaviour agentBehaviour;
	private CharacterController controller;
	private LineRenderer lineRenderer;
    //public WaypointPathfinder wpp;
    public List<Vector3> previousPositions = new List<Vector3>();

	public float speed = 5;
	public float rotSpeed = 5;
	public LayerMask lm;

	//public bool atEnd;
	//public bool differentRoad;
	public GameObject currentRoad;
	public GameObject nextRoad;
	public bool onConnectingRoad;
	public bool onMainRoad;
	public UnitySteer.Behaviors.SteerForPathSimplified steerForPath;
	public GameObject jC;
	//hex vars
	public bool forHex =false;
    public bool forVoronoi = false;
    public Vector3 target; //is asigned by placeWayPointPlayer script
	public bool doDebug;
	public Transform cubePrefab;
	public bool doCubes;
    public bool changingLanes;
    public Avoidance avoidance;
    public WaypointNode startNode;
    public WaypointNode endNode;
    public List<Vector3> returnedPath = new List<Vector3>();
    
    void Start()
	{
		//cubePrefab = Resources.Load("Cube",typeof (Transform)) as Transform;
		controller = gameObject.GetComponent<CharacterController>();
		lineRenderer = gameObject.GetComponent<LineRenderer>();
        //agentBehaviour = gameObject.GetComponent<AgentBehaviour>();

		
	
		if(!forHex && !forVoronoi) //for agents
		{
            //agent is asigned by spawn agents scripts when attaching this script
            agentBehaviour = agent.GetComponent<AgentBehaviour>();
        }

		else if(forHex)
			waypointPathFinder = GameObject.Find("HexGrid").GetComponent<WaypointPathfinder>();

        else if(forVoronoi)
            waypointPathFinder = GameObject.Find("VoronoiPathfinderMap").GetComponent<WaypointPathfinder>();


        lm = LayerMask.GetMask("Road");

		if(forHex || forVoronoi)
		{
			//if building for setup, find path from this position to the target. Target is given to this script by the place waypointplayers
			//script. This is certainly a round about way of doing it, but the architecture was in place for doing it this way from a previous method.
			//I would like to streamline this all at some point.


        //firing from Placewaypoint players directly - making enabled false when we palce it
            /*
			returnedPath = waypointPathFinder.FindPath(transform.position,target);

            foreach(Vector3 v3 in returnedPath)
            {
                GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.position = v3;
            }
            */
        }

	}


	//private float lerpAmt = 0f;

	void Update()
	{
		//We do not need to keep checking this on setup. We just need the path it finds once.
		if(!forHex)
			MoveMethod();

		if(doCubes)
		{
			foreach(Vector3 v3 in Path)
			{
				Transform newCube2 = Instantiate(cubePrefab,v3,Quaternion.identity) as Transform; 
				newCube2.name = "PathfinderCube";
				newCube2.transform.parent = this.transform;
			}
			doCubes = false;
		}
	}

	private void MoveMethod()
	{

	//	jC.SetActive(false);
	//	steerForPath.Weight = 10;
		//called in Fixed Update
	
		if (Path.Count > 1)
		{

       //     Vector3 moveDirection = Path[0] - transform.position;

            //Vector3 target = (moveDirection.normalized + avoidance.avoidanceVector);
      //      Quaternion rotation = Quaternion.LookRotation(Path[0]- transform.position);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * (rotSpeed*speed));
            //    character.Move(moveDirection.normalized * speed * Time.deltaTime);
            
        //    transform.position +=  moveDirection * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, Path[0]) < 1F)
			{
				Path.RemoveAt(0);

               // previousPositions.Add(Path[0]);                       
               


                    //     transform.position = Vector3.Lerp(transform.position, Path[0], 0.1f);

                //bool atEnd = false;
                //bool differentRoad = false;


                //path.count > 25 because this is how far we check ahead for junctions
                /*
                if(Path.Count > 25)
                {				
                    RaycastHit hit;
                    Physics.Raycast(Path[0] + Vector3.up*2,Vector3.down,out hit,5f,lm);					

                    Mesh mesh =  hit.transform.GetComponent<MeshFilter>().mesh;
                    int triangleCount = mesh.triangles.Length;	



                    //check where the hit is on the mesh using the mesh's triangle index
                    if (hit.triangleIndex*3 < 10 || hit.triangleIndex*3 > triangleCount - 10)
                    {
                        //we are at the end/start of a road if this is true
                        atEnd = true;
                    }

                    //check for first ring road
                    if(hit.transform.parent.transform.name == "FirstRingRoad")
                    {
                        //we are on the first ring road, which has a join so we can never be at the end of it
                        atEnd = false;
                    }

                    //we dont know at this moment if we have just came on to the start of a road or are leaving
                    //the end of the road. To figure this out we need to check if the next section of road is a different road or not
                    //If it is a different road, we must be at the end of our road, if it s the same road, we must be a the beginning
                    //of a new road.

                    //So lets raycast up the path a bit,say 50 waypoints
                    RaycastHit hit2;
                    Physics.Raycast(Path[25] + Vector3.up*2,Vector3.down,out hit2,5f,lm);

                    if(hit2.collider.gameObject != hit.collider.gameObject)
                    {
                        //it is a different road, so it must be the end of the road
                        differentRoad = true;
                    }

                    if(atEnd && differentRoad)
                    {
                        steerForPath.Weight = 0;
                        currentRoad = hit.collider.gameObject;
                        nextRoad = hit2.collider.gameObject;
                    }
                }
                */
                }
            }


        if (Path.Count == 1 && (agentBehaviour.visitingFriend || agentBehaviour.returningHome))// && !changingLanes)
        {
         //   speed = 0;
         //clear this waypointplayer's path
         Path.Clear();

         //tell AgentBehaviour we have finished our journey
         if(agentBehaviour.visitingFriend == true)
            {
                Debug.Log("finished friend journey");
                //agentBehaviour.visitingFriend = false;
                StartCoroutine( agentBehaviour.StartHangout() );
            } 
         if(agentBehaviour.returningHome == true)
            {
                //call, get out the car and go in home
                StartCoroutine(agentBehaviour.GoInsideHome());

                //moving these til after agent is in house
                //agentBehaviour.returningHome = false;
                //agentBehaviour.atHome = true;
                //agentBehaviour.readyForNewJourney = true;
            }

         //remove this agent from any junction list it is in

            if (agent.GetComponent<AgentBehaviour>().readyForNewJourney == false)
            {
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

         //   gameObject.GetComponent<AgentBehaviour>().readyForNewJourney = true;
        }
	}
	
	private void DrawPath()
	{
		if (Path.Count > 0)
		{
			lineRenderer.SetVertexCount(Path.Count);
			
			for (int i = 0; i < Path.Count; i++)
			{
				lineRenderer.SetPosition(i, Path[i] + Vector3.up);
			}
		}
		else
		{
			lineRenderer.SetVertexCount(0);
		}
	}
}
