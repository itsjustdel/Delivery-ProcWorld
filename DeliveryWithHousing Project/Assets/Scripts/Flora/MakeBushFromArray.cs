using UnityEngine;
using System.Collections;

public class MakeBushFromArray : MonoBehaviour {

    public bool gardenTree;
	// Use this for initialization
	void Start () {
        if (!gardenTree)
        {
            BranchArray branchArray = GameObject.FindGameObjectWithTag("Code").GetComponent<BranchArray>();
            branchArray.MakeShrub(transform.gameObject, transform.position,false);
        }
        else
        {
            BranchArray branchArray = GameObject.FindGameObjectWithTag("Code").GetComponent<BranchArray>();
            branchArray.MakeGardenTree(transform.gameObject, transform.position);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
