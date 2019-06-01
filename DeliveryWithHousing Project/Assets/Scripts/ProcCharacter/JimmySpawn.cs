using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JimmySpawn : MonoBehaviour {
    public int amount = 1;
	// Use this for initialization
	void Start ()
    {
        //get all houses
        //global list?
        GameObject[] housePlots = GameObject.FindGameObjectsWithTag("HouseCell");
        //filter out unecessary cells which make up the larger hosue cell
        List<GameObject> filtered = new List<GameObject>();
        for (int i = 0; i < housePlots.Length; i++)
        {
            if (housePlots[i].name == "Combined mesh")
                filtered.Add(housePlots[i]);
        }
        //choose
        GameObject spawnHouse = filtered[Random.Range(0, filtered.Count)];

        //make character
        GameObject g = new GameObject();
        BindPoseExample bpe = g.AddComponent<BindPoseExample>();
        ProceduralAnimator pA = g.AddComponent<ProceduralAnimator>();
        CharacterControllerProc ccp = g.AddComponent<CharacterControllerProc>();
        ccp.enabled = false;
        ccp.pA = pA;
        ccp.bPE = bpe;
        bpe.spawnPoint = spawnHouse.GetComponent<MeshRenderer>().bounds.center;
	}
	
}
