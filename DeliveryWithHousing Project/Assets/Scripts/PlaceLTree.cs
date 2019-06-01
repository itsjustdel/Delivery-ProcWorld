using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceLTree : BuildLTree {

	public BezierSpline spline;

	public float treeOffset = 10;
	
	public int frequency = 5;
	
	public Transform cube;
	public GameObject cubePrefabGO;
	private List<Vector3> treePositions = new List<Vector3>();
	private Vector3 rotation01, rotation02 ;

	// NEXT make a long road
	// experiment with randomly changing the index
	// make better spaced clumps
	// make procedural trees
	// have clumps of random procedural trees / bushes
	// if trees, have spacing really large

	// Move stuff into own coroutine
	// stop it running so slow

	private void Start()
	{
		StartCoroutine (BuildTree ());
	}

	IEnumerator BuildTree()
	{
        float stepSize = frequency;
        
		if (spline.Loop || stepSize == 1) 
		{
			stepSize = 1f / stepSize;
		}
		else 
		{
			stepSize = 2f / (stepSize - 1f); // was 1f
		}
		
		// frequency / 10 - to just do the first part of the road
		for (int i = 0; i < (frequency / 10) ; i++) 
		{
			Vector3 position = spline.GetPoint(i * stepSize);
			Vector3 direction = spline.GetDirection(i * stepSize);
			
			// each side of the road (i.e. left right) original version
			// this would all be better if done in code, so don't have to calculate both?
			// all seems a bit unnecesarry
			Vector3 rotation01 = Quaternion.Euler (0, -90, 0) * direction;
			Vector3 rotation02 = Quaternion.Euler (0, 90, 0) * direction;
			
			// this is just here to spice things up a bit
			// makes it so cubes are not mirrored either side
			// if i is odd one side if even then the other side
			// if (i % 2 != 0)
				// rotation01 = Quaternion.Euler (0, -90, 0) * direction;
			// else
				// rotation02 = Quaternion.Euler (0, 90, 0) * direction;
			
			rotation01 *= treeOffset;
			rotation02 *= treeOffset;
			
			Vector3 offset01 = rotation01 + position;
			Vector3 offset02 = rotation02 + position;
			
			treePositions.Add(offset01 - transform.position);
			treePositions.Add(offset02 - transform.position);
			
			// this is here to randomly increase frequency
			// so cubes are not placed randomly
			// i += Random.Range(1, 5);
		}
		
		// places cubes at each point - should be a function that's called at the end of that for loop instead i think
		for (int i = 0; i < treePositions.Count; i++) 
		{
			/*
			Transform item = Instantiate (cube) as Transform;
			BuildLTree lTree = gameObject.AddComponent<BuildLTree>();
			
			Vector3 smallOffset = new Vector3(Random.Range(0.1f, 0.5f), Random.Range(0.1f, 0.5f), Random.Range(0.1f, 0.5f));
			
			lTree.position = treePositions[i] + smallOffset;
			
			item.transform.position = treePositions[i] + smallOffset;
			lTree.construct(null,initial_length,initial_radius);
			*/

			
			// this is making a clump of 5 cubes at each point
			// at each point, do this for loop 5 times

			for (int j = 0; j < 1; j++)
			{
				Transform item = Instantiate (cube) as Transform;

				BuildLTree tp = gameObject.AddComponent<BuildLTree>();
				
				Vector3 balls = new Vector3(Random.Range(-5f, 5f),0f, Random.Range(-5f, 5f));
				
				tp.position = treePositions[i] + balls;
				
				item.transform.position = treePositions[i] + balls;
				tp.construct(null,initial_length,initial_radius);

				//yield return 0;

			}
			yield return new WaitForFixedUpdate();
			
		}


	}

	/*
	// old and not using coroutines
	private void Start()
	{
		// don't really know what this bit is doing :S
		float stepSize = frequency;
		if (spline.Loop || stepSize == 1) 
		{
			stepSize = 1f / stepSize;
		}
		else 
		{
			stepSize = 2f / (stepSize - 1); // was 1f
		}

		// frequency / 10 - to just do the first part of the road
		for (int i = 0; i < (frequency / 10) ; i++) 
		{
			Vector3 position = spline.GetPoint(i * stepSize);
			Vector3 direction = spline.GetDirection(i * stepSize);
			
			// each side of the road (i.e. left right) original version
			// this would all be better if done in code, so don't have to calculate both?
			// all seems a bit unnecesarry
			// Vector3 rotation01 = Quaternion.Euler (0, -90, 0) * direction;
			// Vector3 rotation02 = Quaternion.Euler (0, 90, 0) * direction;
			
			// this is just here to spice things up a bit
			// makes it so cubes are not mirrored either side
			// if i is odd one side if even then the other side
			if (i % 2 != 0)
				rotation01 = Quaternion.Euler (0, -90, 0) * direction;
			else
				rotation02 = Quaternion.Euler (0, 90, 0) * direction;
			
			rotation01 *= treeOffset;
			rotation02 *= treeOffset;
			
			Vector3 offset01 = rotation01 + position;
			Vector3 offset02 = rotation02 + position;
			
			treePositions.Add(offset01 - transform.position);
			treePositions.Add(offset02 -transform.position);
			
			// this is here to randomly increase frequency
			// so cubes are not placed randomly
			// i += Random.Range(1, 5);
		}

		// places cubes at each point - should be a function that's called at the end of that for loop instead i think
		for (int i = 0; i < treePositions.Count; i++) 
		{
			
			// this is making a clump of 5 cubes at each point
			// at each point, do this for loop 5 times
			for (int j = 0; j < 5; j++)
			{
				Transform item = Instantiate (cube) as Transform;
				BuildLTree tp = gameObject.AddComponent<BuildLTree>();
				
				Vector3 balls = new Vector3(Random.Range(0.1f, 0.5f), Random.Range(0.1f, 0.5f), Random.Range(0.1f, 0.5f));
				
				tp.position = treePositions[i] + balls;
				
				item.transform.position = treePositions[i] + balls;
				tp.construct(null,initial_length,initial_radius);
				
			}

		}
	}
	*/
}