using UnityEngine;
using System.Collections;

public class FieldManager : MonoBehaviour {

    public string fieldUse;
    public float fenceIndentation = 1f;//biome governed?
    public float bushesIndentation = 0.5f;
    // Use this for initialization
    void Start () {

        

        fieldUse = FieldUse();    

	
	}

    /// <summary>
    /// Decide field use depending on size and random factors
    /// </summary>
    /// <returns></returns>
    string FieldUse()
    {
        string use = null;

        //get size of field. doesn't need to too accurate, use mesh bounds
        float sizeX = GetComponent<MeshRenderer>().bounds.size.x;
       // Debug.Log(sizeX);
        float sizeZ = GetComponent<MeshRenderer>().bounds.size.z;
       // Debug.Log(sizeZ);

        if(sizeX > 200f || sizeZ > 200f)
        {
            //a large field
            use = "Trees";
        }
        else
        {
            //smaller field
            use = "Cabbages";
        }


        //TEST 
        use = ("Cabbages");

        return use;
    }
	
}
