using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class CellManager : MonoBehaviour {
	public GameObject[] countryArray;
	public List<PositionAndOwner> masterRoadList = new List<PositionAndOwner>();
    public float vergesBuilt;
    public List<JunctionArea> junctions = new List<JunctionArea>();
    public List<Vector3> voronoiPointsUsed = new List<Vector3>();
	// Use this for initialization
	public void Start () {

        //initialise
        masterRoadList = new List<PositionAndOwner>();
        vergesBuilt = 0;
        junctions = new List<JunctionArea>();

        //StartCoroutine("AddPathFinders");
        StartCoroutine("StartBuildOrder");
       // StartCoroutine("BuildIndividualCell");

    }

  
	
	IEnumerator StartBuildOrder()
	{
		
		yield return new WaitForEndOfFrame();
		countryArray = GameObject.FindGameObjectsWithTag("Countryside");

        for (int i = 0; i < countryArray.Length; i++)
        {
            //countryArray[0].name = "FirstRingRoad";

            //now add gridplayers/waypointplayers, the script which figures out the route from point to point
            yield return new WaitForEndOfFrame();
          
            //Give the cell a script which manages how the road makes its way around the cell edge
            countryArray[i].AddComponent<PlaceWaypointPlayer>();   
          
			
		}
        yield return new WaitForEndOfFrame();
        //all roads have been built. now we can attach the junction meshes
        for (int i = 0; i < countryArray.Length; i++)
        {
            //loop connects to itself, we don't need joining meshes
            if (!countryArray[i].GetComponent<PlaceWaypointPlayer>().loopRoad)
            {
                yield return new WaitForEndOfFrame();
                    
              //  RingRoadMesh.JoiningMeshStart(countryArray[i]); //changed function to work with voronoi road builder - cell maanger obselete
              //  RingRoadMesh.JoiningMeshEnd(countryArray[i]);
            }
        }

        //now the roads have been built, build verges
        for (int i = 0; i < countryArray.Length; i++)
        {
            //	yield return new WaitForSeconds(1);
            yield return new WaitForEndOfFrame();
            //add script to create verge along ring road

            Verge verge = countryArray[i].AddComponent<Verge>();
            verge.forRingRoad = true;

            yield return new WaitForEndOfFrame();
            while (vergesBuilt < 2) //two because there are two verges per cell
            {
                yield return new WaitForEndOfFrame();
            }
            //reset for next iteration in for loop
            vergesBuilt = 0;
        }

        

        for (int i = 0; i < countryArray.Length; i++)
        {           

            HousesForVoronoi hFV = countryArray[i].AddComponent<HousesForVoronoi>();
        }



        for (int i = 0; i < countryArray.Length; i++)
        {
         //   countryArray[i].AddComponent<SmoothVergeFullRoad>();      //unnecesary complications?
        }

        for (int i = 0; i < countryArray.Length; i++)
        {
            if (i == 0)
                continue;
            //add to list for enabling later once mesh gen has finished
            JunctionArea ja = countryArray[i].AddComponent<JunctionArea>();
            junctions.Add(ja);
        }

        //Now the roads have finished buling, add the voronoi mesh which lays over the roads and adds detail to
        //the surroundings
        //this script starts a new chain reaction of functions	
        yield return new WaitForEndOfFrame();//improve this?

        /*
        //GameObject.FindGameObjectWithTag("VoronoiMesh").GetComponent<AddToVoronoi>().Start();
        voronoiPointsUsed = voronoiPointsUsed.Distinct().ToList();
        for (int i = 0; i < voronoiPointsUsed.Count; i++)
        {
            voronoiPointsUsed[i] = new Vector3(voronoiPointsUsed[i].x, 0, voronoiPointsUsed[i].z);
        }
        */

        
        GameObject.FindGameObjectWithTag("VoronoiMesh").GetComponent<MeshGenerator>().yardPoints = voronoiPointsUsed;
        GameObject.FindGameObjectWithTag("VoronoiMesh").GetComponent<AddToVoronoi>().Start();

    }
	

    IEnumerator BuildIndividualCell()
    {
        yield break;
    }
	
	public class PositionAndOwner
	{
		public Vector3 position;// = new Vector3();
		public GameObject owner;/// = new GameObject();
	}
}
