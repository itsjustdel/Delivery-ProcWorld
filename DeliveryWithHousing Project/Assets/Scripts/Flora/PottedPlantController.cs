using UnityEngine;
using System.Collections;

public class PottedPlantController : MonoBehaviour {

    public int stems = 2;
    public bool roses;
    public bool tulips;
    
    // Use this for initialization
    void Start()
    {
        stems = Random.Range(1, 10);

        PlantPot pot = GetComponent<PlantPot>();
       // pot.enabled = true;

        

        for (int i = 0; i < stems; i++)
        {
            ProceduralPlant pp = gameObject.AddComponent<ProceduralPlant>();

            if (roses)
            {
                pp.MaxNumVertices = 1024;
                pp.NumberOfSides = 3;
                pp.BaseRadius = 0.4f;
                pp.RadiusStep = 0.8f;   //how wild it looks
                pp.MinimumRadius = 0.02f; //controls how large the plant is
                pp.BranchRoundness = 1f;
                pp.SegmentLength = 2f;
                pp.Twisting = 40f;
                pp.leavesUpStem = true;
                pp.enabled = true;
            }

            if (tulips)
            {
                pp.MaxNumVertices = 1024;
                pp.NumberOfSides = 3;
                pp.BranchProbability = 0f;
                pp.BaseRadius = 0.2f;
                pp.RadiusStep = 0.8f;   //how wild it looks
                pp.MinimumRadius = 0.02f; //controls how large the plant is
                pp.Twisting = 10f;
                pp.BranchRoundness = 1f;
                pp.SegmentLength = 2f;
                pp.leavesUpStem = false;
                pp.enabled = true;
            }
        }
        GetComponent<Leaves>().enabled = true;


    }


}
