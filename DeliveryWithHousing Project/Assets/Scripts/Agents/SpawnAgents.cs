using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnAgents : MonoBehaviour
{

    //THis script puts people in houses #samaritan
    void Start()
    {

        StartCoroutine("Spawn");

    }

    IEnumerator Spawn()
    {


        //all house plots
        GameObject[] plots = GameObject.FindGameObjectsWithTag("HouseCell");
        for (int p = 0; p < plots.Length; p++)
        {
            //skip mini house cells which make up larger plot
            if (plots[p].name != "Combined mesh")
                continue;

            //how many people live in this house
            int residentsNumber = 2;// Random.Range(0,1);

            //spawn agents
            //GameObject agentPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);


            //spawn this house a car // 1car per house? or 1 for each parent?
            GameObject carPrefab = Resources.Load("Prefabs/Agents/AgentVehicle", typeof(GameObject)) as GameObject;

            GameObject partnersCar = null;

            for (int i = 0; i < residentsNumber; i++)
            {
                //spawn at a waypoint in front of the house, first resident gets 1st space, 2nd gets 2nd
                //grabbing first space
                //sometimes do not give a car to second(etc) partner
                GameObject car = null;
                bool spawnedCarForThisResident = false;
                //always spawn first car
                if(i==0)
                {
                    Vector3 positionCar = plots[p].transform.parent.Find("Waypoints Parent").GetChild(i).transform.position;
                    car = Instantiate(carPrefab, positionCar, Quaternion.identity) as GameObject;

                    spawnedCarForThisResident = true;
                }

                if (i != 0 && Random.Range(0, 2) == 0)
                {
                    Vector3 positionCar = plots[p].transform.parent.Find("Waypoints Parent").GetChild(i).transform.position;
                    car = Instantiate(carPrefab, positionCar, Quaternion.identity) as GameObject;

                    spawnedCarForThisResident = true;
                }

                //save 1st car in case of car share on 2nd resident, or 2nd with 3rd etc
                if(car != null)
                    partnersCar = car;
                //spawn agent at center of house plot
                Vector3 positionAgent = plots[p].GetComponent<MeshRenderer>().bounds.center;
                
                GameObject agent = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), positionAgent, Quaternion.identity) as GameObject;
                agent.name = "Agent" + i;

                //Class/Script which holds all if the agent's details
                AgentInfo agentInfo = agent.AddComponent<AgentInfo>();
                agentInfo.agent = agent;
                agentInfo.home = plots[p];

                //if they have their own car
                if (spawnedCarForThisResident)
                {
                    agentInfo.car = car;
                    agentInfo.haveOwnCar = true;
                }
                //else, asign their partners car to fight over
                else
                {
                    agentInfo.car = partnersCar;
                    agentInfo.haveOwnCar = false;
                }
                //Behaviour script
                agent.AddComponent<AgentBehaviour>();

                //Waypoint script
                WaypointPlayer wpp = null;
                //set pathfinder
                if (spawnedCarForThisResident)
                    wpp = car.GetComponent<WaypointPlayer>();
                else if (!spawnedCarForThisResident)
                    wpp = partnersCar.GetComponent<WaypointPlayer>();

                wpp.agent = agent;
                wpp.waypointPathFinder = GameObject.Find("Agents").GetComponent<WaypointPathfinder>();
                wpp.PathType = PathfinderType.WaypointBased;

                //add this agent to a master list of all the agents in the game
                GetComponent<AgentList>().agentList.Add(agent);

                //add agent to a list of agents who live at this cell
                //check if a residents script has been added
                if (plots[p].GetComponent<Residents>() == null)
                    //add if there is no residents script attached
                    plots[p].AddComponent<Residents>();

                plots[p].GetComponent<Residents>().agentsLivingAtThisCell.Add(agent);

                //stagger spawning
              //  yield return new WaitForSeconds(0.5f);
            }
        }


        //yield return new WaitForSeconds(1);
        SpawnPlayer();

        yield break;
    }

    void SpawnPlayer()
    {
        GameObject.FindGameObjectWithTag("Code").GetComponent<PlaceVan>().SpawnVan();
    }

}
