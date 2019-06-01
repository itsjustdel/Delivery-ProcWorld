using UnityEngine;
using System.Collections;

public class CreateTree : MonoBehaviour {
	public Transform ico;
	// Use this for initialization
	void Start () {
		//Add the procedural tree component to the game object
		ProceduralTree procTree = gameObject.AddComponent<ProceduralTree>();

		//Add the renderer to the game object. This was done in the treescript but has been commented out, line 129
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial =  Resources.Load("Brown",typeof(Material)) as Material;


		// set the script variable to the prefab
		procTree.ico = ico;

		// randomise the seed

		procTree.Seed = Random.Range(0, 65536);

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
		public float BranchProbability = 0.1f; // Branch probabilit
		*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
