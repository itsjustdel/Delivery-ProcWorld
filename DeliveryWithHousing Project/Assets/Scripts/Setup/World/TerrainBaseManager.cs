using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBaseManager : MonoBehaviour {

    public SubdivideMesh sdm;
    public PerlinNoisePlane pnp1;
    public PerlinNoisePlane pnp2;
    public ContoursForVoronoi cfv;

    // Use this for initialization
    void Start ()
    {
        StartCoroutine("Builder");
	}
	
	IEnumerator Builder()
    {
        sdm.enabled = true;

        yield return new WaitForEndOfFrame();

        pnp1.enabled = true;

        yield return new WaitForEndOfFrame();

        pnp2.enabled = true;
        
        yield return new WaitForEndOfFrame();

        pnp2.enabled = true;

        yield return new WaitForEndOfFrame();

        cfv.enabled = true;

        yield break;
    }
}
