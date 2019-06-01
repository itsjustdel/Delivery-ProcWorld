using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODSwitcher : MonoBehaviour {

    /// <summary>
    /// any object we wish to have an lod is passed to this script and it updates and switches the between meshes
    /// </summary>
    public GameObject player = null;
    
    public List<List<MeshFilter>> meshFilters = new List<List<MeshFilter>>();
    public float lodDepth0 = 100f;
    public float lodDepth1 = 200f;
    public float lodDepth2 = 300f;
    public float lodDepth3 = 400f;

    [Range(0.1f,10f)]
    public float multiplier = 1f;
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (player == null)
            return;


        //check each object's distance to player
        for (int i = 0; i < meshFilters.Count; i++)
        {
            //if player is close enough set mesh to the first in the list - this is the most detailed
            //[0] is attached to the rendering objects, [1] and [2],etc are just objects to hold mehes in
            float distance = Vector3.Distance(meshFilters[i][0].transform.position, player.transform.position);
            if (distance/multiplier < lodDepth0)
            {
                meshFilters[i][0].mesh = meshFilters[i][1].mesh;
            }
            else if(distance < lodDepth1)
                //set to lower poly
                meshFilters[i][0].mesh = meshFilters[i][2].mesh;

            else if(distance < lodDepth1)            
                meshFilters[i][0].mesh = meshFilters[i][3].mesh;
            else
                meshFilters[i][0].mesh = meshFilters[i][4].mesh;


        }		
	}
}
