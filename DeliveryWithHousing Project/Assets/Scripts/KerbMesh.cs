using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System;

public class KerbMesh: ProcBase {
	public float kerbSizeX;
	public float kerbSizeY;
	public SplineMesh splineMesh;
	public KerbInjector kerbInjector;
	public BezierSpline splineL;
	public BezierSpline splineR;
	public Transform cube;
	// Use this for initialization
	void Start () {

		//grab the points from array and feed in to the curve splineL so we can use the
		//bezier class to work out the points on the curve
		splineL.points = kerbInjector.listL.ToArray();
		splineR.points = kerbInjector.listR.ToArray();
		//the higher this is,thesmaller thekerbs are -the amount of kerbs
		float frequency = splineMesh.frequency*2;

		//this is a hangover from splineDecorator from catlike, still don't understand why it
		//has to be these numbers
		float stepSize = frequency;
		stepSize = 2f / (stepSize -1);

		// frequency is divided by four because the vertices add to four
		for (int f = 0; f <= frequency/4; f++) { 

			Vector3 position = splineL.GetPoint(f * stepSize);
			Vector3 nextPos = splineL.GetPoint((f+1) * stepSize);
			//nextPoint is used to determine where the Quad is stretched to
			Vector3 nextPoint = nextPos-position;
			nextPoint *=0.98f;//makes the kerbs seperate instead of a big snake

			Vector3 direction = splineL.GetDirection(f*stepSize);
			Quaternion lookRot = Quaternion.LookRotation (direction);
			Vector3 right = lookRot * Vector3.right * kerbSizeX;

			Vector3 kerbHeight = kerbSizeY*(lookRot*Vector3.up);

			MeshBuilder meshBuilder = new MeshBuilder();
			BuildQuad(meshBuilder,position,nextPoint,right);//bottom
			BuildQuad(meshBuilder,position,right,kerbHeight);
			BuildQuad(meshBuilder,position,kerbHeight,nextPoint);//far side
			
			Vector3 farCorner = kerbHeight+nextPoint + position;

			BuildQuad(meshBuilder,farCorner,-nextPoint,right);//top
			BuildQuad(meshBuilder,farCorner,right,-kerbHeight);
			BuildQuad(meshBuilder,farCorner+right,-nextPoint,-kerbHeight);

			GameObject child = new GameObject();
			MeshRenderer meshRenderer = child.AddComponent<MeshRenderer>();
			MeshFilter meshFilter = child.AddComponent<MeshFilter>();
			MeshCollider meshCollider = child.AddComponent<MeshCollider>();
			child.transform.parent = this.gameObject.transform;
			child.layer = 9;

			//creates a rotation for each kerb, so they look wonky. The 2f could be replaces for a variable
			child.transform.RotateAround(position,Vector3.up,UnityEngine.Random.Range(-0.2f,0.2f));
			child.transform.RotateAround(position,Vector3.right,UnityEngine.Random.Range(-0.2f,0.2f));
			child.transform.RotateAround(position,Vector3.forward,UnityEngine.Random.Range(-0.2f,0.2f));

			Mesh mesh = meshBuilder.CreateMesh();
			meshFilter.sharedMesh = mesh;
			meshCollider.sharedMesh = mesh;
			
			meshRenderer.sharedMaterial = Resources.Load("Grey",typeof(Material)) as Material;
		}

		for (int f = 0; f <= frequency/4; f++) { 

			Vector3 position = splineR.GetPoint(f * stepSize);


			Vector3 nextPos = splineR.GetPoint((f+1) * stepSize);
			//nextPoint is used to determine where the Quad is stretched to
			Vector3 nextPoint = nextPos-position;
			nextPoint *=0.98f;//makes the kerbs seperate instead of a big snake
			
			Vector3 direction = splineR.GetDirection(f*stepSize);
			Quaternion lookRot = Quaternion.LookRotation (direction);
			Vector3 right = lookRot * Vector3.right * kerbSizeX;

			//this shifts the offset towards the middle of the road so the kerb builds inside the raod mesh
			position-=right;

			Vector3 kerbHeight = kerbSizeY*(Vector3.up);
			
			MeshBuilder meshBuilder = new MeshBuilder();
			BuildQuad(meshBuilder,position,nextPoint,right);//bottom
			BuildQuad(meshBuilder,position,right,kerbHeight);
			BuildQuad(meshBuilder,position,kerbHeight,nextPoint);//far side
			
			Vector3 farCorner = kerbHeight+nextPoint + position;
			
			BuildQuad(meshBuilder,farCorner,-nextPoint,right);//top
			BuildQuad(meshBuilder,farCorner,right,-kerbHeight);
			BuildQuad(meshBuilder,farCorner+right,-nextPoint,-kerbHeight);
			
			GameObject child = new GameObject();
			MeshRenderer meshRenderer = child.AddComponent<MeshRenderer>();
			MeshFilter meshFilter = child.AddComponent<MeshFilter>();
			MeshCollider meshCollider = child.AddComponent<MeshCollider>();
			child.transform.parent = this.gameObject.transform;
			child.layer = 9;
			//creates a rotation for each kerb, so they look wonky. The 2f could be replaces for a variable
			child.transform.RotateAround(position,Vector3.up,UnityEngine.Random.Range(-0.2f,0.2f));
			child.transform.RotateAround(position,Vector3.right,UnityEngine.Random.Range(-0.2f,0.2f));
			child.transform.RotateAround(position,Vector3.forward,UnityEngine.Random.Range(-0.2f,0.2f));
			
			Mesh mesh = meshBuilder.CreateMesh();
			meshFilter.sharedMesh = mesh;
			meshCollider.sharedMesh = mesh;
			
			meshRenderer.sharedMaterial = Resources.Load("Grey",typeof(Material)) as Material;
		}

	
	}

}
