using UnityEngine;
using System.Collections;

public class PlaceFlora : MonoBehaviour {
	public Transform ico;
	public Transform leafPrefab;
	public bool isTree;
	//public Transform leavesParent;

	public int Seed; // Random seed on which the generation is based
	[Range(1024, 65000)]
	public int MaxNumVertices = 1024; // Maximum number of vertices for the tree mesh
	
	[Range(3, 32)]
	public int NumberOfSides = 4; // Number of sides for tree
	
	[Range(0.25f, 4f)]
	public float BaseRadius = 1f; // Base radius in meters
	
	[Range(0.75f, 0.95f)]
	public float RadiusStep = 0.9f; // Controls how quickly radius decreases
	
	[Range(0.01f, 0.2f)]
	public float MinimumRadius = 0.02f; // Minimum radius for the tree's smallest branches
	
	[Range(0f, 1f)]
	public float BranchRoundness = 0.8f; // Controls how round branches are
	
	[Range(0.1f, 2f)]
	public float SegmentLength = 0.5f; // Length of branch segments
	
	[Range(0f, 40f)]
	public float Twisting = 1f; // How much branches twist
	
	[Range(0f, 0.25f)]
	public float BranchProbability = 0.1f; // Branch probability
	// Use this for initialization
	void Start () {
		//Add the procedural tree component to the game object
		ProceduralRoseBush procTree = gameObject.AddComponent<ProceduralRoseBush>();
		procTree.isTree = isTree;
		
		//Add the renderer to the game object. This was done in the treescript but has been commented out, line 129
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		//meshRenderer.enabled = false;


		meshRenderer.sharedMaterial =  Resources.Load("Brown",typeof(Material)) as Material;

	//	meshRenderer.enabled = false;

		
		// set the script variable to the prefab
		procTree.rosePrefab = ico;
		procTree.leafPrefab = leafPrefab;
		//procTree.leavesParent = leavesParent;
		
		// randomise the seed
		
		procTree.Seed = Random.Range(1024, 65000);
		procTree.NumberOfSides = NumberOfSides;
		procTree.BaseRadius = BaseRadius;
		procTree.RadiusStep = RadiusStep;
		procTree.MinimumRadius = MinimumRadius;
		procTree.BranchRoundness = BranchRoundness;
		procTree.SegmentLength = SegmentLength;
		procTree.Twisting = Twisting;
		procTree.BranchProbability = BranchProbability;
		
		/*

 		public int Seed; // Random seed on which the generation is based
        [Range(1024, 65000)]
        public int MaxNumVertices = 65000; // Maximum number of vertices for the tree mesh
        
        [Range(3, 32)]
        public int NumberOfSides = 16; // Number of sides for tree
        
        [Range(0.25f, 4f)]
        public float BaseRadius = 2f; // Base radius in meters
        
        [Range(0.75f, 0.95f)]
        public float RadiusStep = 0.9f; // Controls how quickly radius decreases
        
        [Range(0.01f, 0.2f)]
        public float MinimumRadius = 0.02f; // Minimum radius for the tree's smallest branches
        
        [Range(0f, 1f)]
        public float BranchRoundness = 0.8f; // Controls how round branches are
        
        [Range(0.1f, 2f)]
        public float SegmentLength = 0.5f; // Length of branch segments
        
        [Range(0f, 40f)]
        public float Twisting = 20f; // How much branches twist
        
        [Range(0f, 0.25f)]
        public float BranchProbability = 0.1f; // Branch probability
        
		*/
	}
	

}
