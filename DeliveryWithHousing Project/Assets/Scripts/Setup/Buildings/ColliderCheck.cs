using UnityEngine;
using System.Collections;
/// <summary>
/// Destroys Any Building in its collider
/// </summary>
public class ColliderCheck : MonoBehaviour {

	public int ind;
	public GameObject cell;
	public Mesh mesh;
	public bool townHouse = true;
    public bool giveToVoronoi = false;
	// Use this for initialization

     

	void Start () {
	
		StartCoroutine("FeedVoronoi");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay(Collider col)
	{
		if (col.gameObject.tag == "Building" || col.gameObject.tag == "Road")            
		{
		    Destroy(transform.parent.gameObject);
		//	Debug.Log("Destroyed Building From Collider Check");
		}

		//TODO find out timing to remove these scripts for performance reasons
	//	Destroy(gameObject.GetComponent<ColliderCheck>());
	//	Destroy(gameObject.GetComponent<Rigidbody>());

		//if valid building (hasn't been destroyed) then add building to list for voronoi

		//give this value and the mesh it belongs to the voronoi script so that it can be aware of where buildings are for when it
		//is injecting road points. This stop too many "cells" or poitns being too close to each other, giving a higher chance for the
		//yards to build around the houses correctly
	}

	IEnumerator FeedVoronoi()
	{
		yield return new WaitForSeconds(5);
		//Mesh and vertices is a custom class to hold just as it says
		AddToVoronoi.MeshAndVertice listComponent = new AddToVoronoi.MeshAndVertice();
//		Mesh startMesh = transform.parent.parent.GetComponent<TJunctionBuildings>().startMesh;
//		Mesh mesh = 
		listComponent.mesh = mesh;
		listComponent.ind = ind;
		listComponent.cell = cell;

        if (giveToVoronoi)
        {
            //insert directly in to list
            GameObject.FindWithTag("VoronoiMesh").GetComponent<AddToVoronoi>().meshAndVerticeList.Add(listComponent);
        }
		if(!townHouse)//same as above? ///this was written when voronoi was influenced differently
			{
			//add points adjacent to the road to influence voronoi shape
			Vector3 direction = mesh.vertices[ind +1] - mesh.vertices[ind];
			direction.Normalize();
			direction*=7;
			Vector3 middle = Vector3.Lerp(mesh.vertices[ind],mesh.vertices[ind+1],0.5f);
			Vector3 position = middle + direction;


			GameObject.FindWithTag("VoronoiMesh").GetComponent<AddToVoronoi>().pointsForVoronoi.Add(position);
		}

	//	Debug.Log("feeding Voronoi");

	//	Transform house = Resources.Load("Cube", typeof (Transform)) as Transform;
	//	Transform item = Instantiate(house) as Transform;
	//	item.name = "Test Cube";
	//	item.position = position;
	//	item.localScale*=50;
		
		this.enabled = false; //or destroy?
		Destroy(this);

        yield break;
	}
}
