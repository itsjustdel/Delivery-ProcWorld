using UnityEngine;
using System.Collections;

public class QuickTree : MonoBehaviour {

    //makes a tree usign branch array at gamobject position

    
	// Use this for initialization
	void Start ()
    {
        BranchArray bA = GameObject.FindGameObjectWithTag("Code").GetComponent<BranchArray>();
        StartCoroutine(bA.MakeGardenTree(gameObject, transform.position));
        //GameObject.FindGameObjectWithTag("Code").GetComponent<BranchArray>().MakeShrub(gameObject, transform.position,false);
    }

    public void BuildTree()
    {

        BranchArray bA = GameObject.FindGameObjectWithTag("Code").GetComponent<BranchArray>();
        StartCoroutine(bA.MakeGardenTree(gameObject, transform.position));
    }
	
	
}
