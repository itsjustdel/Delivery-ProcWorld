using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewWorld : MonoBehaviour {
    public GameObject terrainBase;
    public GameObject hexGrid;
    public GameObject worldPlan;
    public GameObject voronoiMesh;
    public GameObject gardenCentre;
    // Use this for initialization
    void Start () {
      //  DeleteOldWorld();
      //  StartCoroutine("GenerateNewWorld");
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("n"))
        {
            DeleteOldWorld();
            StartCoroutine("GenerateNewWorld");
        }
        if(Input.GetKeyDown("p"))
        {
            //place garden centre back in
            GameObject newSite = GameObject.Find("GardenCentreBuildingPlot").transform.Find("ProcHouse(Clone)").Find("Meshes").gameObject;

        }
	}

    void DeleteOldWorld()
    {
        //save important buildings
        //gardenCentre = GameObject.Find("GardenCentreBuildingPlot").transform.FindChild("ProcHouse(Clone)").gameObject;
       // gardenCentre = GameObject.FindWithTag("GardenCentreBuilding");
        //parents all get deleted
        gardenCentre.transform.parent = null;


        //destroy is deffered so it is ok to destroy whislt iterating through the loop
        foreach (Transform child in worldPlan.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in voronoiMesh.transform)
        {
            Destroy(child.gameObject);
        }

        //clear all lists that were used
        //doing here instead of initialisation because, many scripts populate the lists at different points, it is easier to reset here rather than at initialisation at script execution - bad design from me

        //save vars from previous
        Vector3 volume = worldPlan.GetComponent<MeshGenerator>().volume;
        int cellNumber = worldPlan.GetComponent<MeshGenerator>().cellNumber;
        Destroy(worldPlan.GetComponent<MeshGenerator>());

        MeshGenerator mg = worldPlan.AddComponent<MeshGenerator>();
        mg.enabled = false;
        mg.volume = volume;
        mg.cellNumber = cellNumber;
        mg.fillWithRandom = true;
        mg.worldPlan = true;
        mg.addMeshCollider = true;        
        mg.ringRaycast = true;
        mg.useSortedGeneration = true;

        Destroy(voronoiMesh.GetComponent<MeshGenerator>());

        MeshGenerator vmg = voronoiMesh.AddComponent<MeshGenerator>();
        vmg.enabled = false;
        vmg.volume = volume;
        vmg.fillWithPoints = true;
        vmg.addMeshCollider = true;
        vmg.addCellYAdjust = true;
        vmg.addPolygonTester = true;
        vmg.renderCells = true;
        vmg.fields = true;
        vmg.useSortedGeneration = false;
       
        voronoiMesh.GetComponent<AddToVoronoi>().pointsForVoronoi.Clear();// = new List<Vector3>();

        

    }

    IEnumerator GenerateNewWorld()
    {
        yield return new WaitForEndOfFrame();

        worldPlan.GetComponent<MeshGenerator>().Start();

        yield break;
    }

  
}
