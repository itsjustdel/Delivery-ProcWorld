using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BuildLTree : MonoBehaviour {

	// need to change to combine meshes

//	private int t = 0;
	// good initial values
	public float initial_length = 3.54f;
	public float initial_radius = 1.15f;
	public BuildLTree rootNode;
	//public static PrimitiveType limbType = PrimitiveType.Cube;
	public static float yScale = 1f;
	public GameObject cubePrefab;
	// this is coming from bush placer
	public Vector3 position;
	
	private List<BuildLTree> branches;
	private GameObject contents;
	private GameObject appearance;
	private BuildLTree lParent;
	
	public float length_decay = 0.8f;
	public float radius_decay = 0.7f;
	public float angle_deviation = 0.3f;
	public float minimum_branches = 1;
	public float maximum_branches = 3;
	public float minimum_radius = 0.1f;

    /*
    void createChildren()
    {
        float new_radius = appearance.transform.localScale.x * radius_decay;
        float new_length = appearance.transform.localScale.y * length_decay;
        if (new_radius < minimum_radius)
            return;
        branches = new List<BuildLTree>();

        GameObject progenitor = new GameObject();
        progenitor.name = "Root for children";
        progenitor.transform.parent = contents.transform;
        progenitor.transform.localPosition = new Vector3(0, 0, 0);
        progenitor.transform.localEulerAngles = new Vector3(0, 0, 0);
        // Debug.Log("the offset is "+appearance.transform.localPosition.y);
        progenitor.transform.Translate(0, 2 * appearance.transform.localPosition.y, 0);

        int num_children = (int)(Random.value * (maximum_branches - minimum_branches) + minimum_branches);
        for (int i = 0; i < num_children; i++)
        {
            // this giving the can't instantiate with new error
            // BuildLTree child = new BuildLTree();
            new GameObject("Branch Child").AddComponent<BuildLTree>().InitializeChild(this);

            branches.Add(child);

            child.construct(progenitor, new_length, new_radius);
        }

    }

    private void InitializeChild (BuildLTree parent)
    {


    }
    */


    // old creatChildren which was working
    
	void createChildren()
	{
		float new_radius = appearance.transform.localScale.x * radius_decay;
		float new_length = appearance.transform.localScale.y * length_decay;
		if(new_radius<minimum_radius) 
			return;
		branches = new List<BuildLTree>();
		
		GameObject progenitor = new GameObject();
		progenitor.name = "Root for children";
		progenitor.transform.parent = contents.transform; 
		progenitor.transform.localPosition = new Vector3(0,0,0);
		progenitor.transform.localEulerAngles= new Vector3(0,0,0);
		// Debug.Log("the offset is "+appearance.transform.localPosition.y);
		progenitor.transform.Translate(0,2*appearance.transform.localPosition.y,0);
		
		int num_children= (int)(Random.value * (maximum_branches - minimum_branches) + minimum_branches);
		for(int i =0; i < num_children; i++)
		{
			// this giving the can't instantiate with new error
			// BuildLTree child = new BuildLTree();
			BuildLTree child = gameObject.AddComponent<BuildLTree>();
			branches.Add(child);

			child.construct(progenitor,new_length,new_radius);
		}
		
	}
    

    public void pivot()
	{
		
		contents.transform.Rotate(0,.1f,0);
	}
	
	public void do_rotate(float amt)
	{
		contents.transform.Rotate(0,0,amt);
		if(branches==null) return;
		for(int i=0;i<branches.Count;i++)
		{
			branches[i].do_rotate(amt);
		}
	}

	public void reset(float l,float r)
	{
		GameObject.Destroy(contents);
		branches = new List<BuildLTree>();
		construct(null, l, r);
	}
	
	public void construct(GameObject parentTree, float length, float radius)
	{
		contents = new GameObject();


		GameObject cube =  gameObject.GetComponent<PlaceLTree>().cubePrefabGO;
		appearance = Instantiate(cube) as GameObject;

        // if != null then this must be a branch
        // I'm sending null to act as parent first go round
		if (parentTree != null)  
			contents.transform.parent = parentTree.transform;

		// old was set to 0
		// contents.transform.localPosition = new Vector3(0,0,0);
		
		// this is the root position!!!
		// need to get this from the road!!!
		contents.transform.localPosition = position;

		contents.transform.localEulerAngles= new Vector3(0,0,0);
		appearance.transform.parent = contents.transform;
		appearance.transform.localPosition = new Vector3(0,0,0);
		appearance.transform.localEulerAngles= new Vector3(0,0,0);
		contents.name = "Contents";
		contents.transform.Rotate(Random.value*100-50,0,Random.value*100-50);
		appearance.name = "Appearance";
		Vector3 scaleVector = new Vector3(radius,length, radius);
		appearance.transform.localScale = scaleVector;
		appearance.transform.Translate(0,0.5f*BuildLTree.yScale*length,0);



		createChildren();
	}


}
