using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RingRaycast : MonoBehaviour {
	/// <summary>
	/// Shoots rays in a circle and asigns material and tag	
	/// </summary>

	public List<Vector3> countrysideList = new List<Vector3>();
	List<Vector3> suburbanList = new List<Vector3>();
	List<Vector3> cityList = new List<Vector3>();

	public void Start () {


        countrysideList.Clear();

		//TODO //Put thes in for loops and expose variables

		DrawCirclePoints(10,100,new Vector2(0,0),"countryside");
        
		DrawCirclePoints(10,200,new Vector2(0,0),"countryside");
        
		DrawCirclePoints(20,300,new Vector2(0,0),"countryside");
        /*
		DrawCirclePoints(20,400,new Vector2(0,0),"countryside");
		DrawCirclePoints(20,500,new Vector2(0,0),"countryside");
		DrawCirclePoints(20,600,new Vector2(0,0),"countryside");
		DrawCirclePoints(20,700,new Vector2(0,0),"countryside");
		DrawCirclePoints(20,800,new Vector2(0,0),"countryside");
		DrawCirclePoints(20,900,new Vector2(0,0),"countryside");
		DrawCirclePoints(20,1000,new Vector2(0,0),"countryside");
        */

        //		DrawCirclePoints(40,700,new Vector2(0,0),"suburban");
        //		DrawCirclePoints(50,800,new Vector2(0,0),"suburban");

        //	DrawCirclePoints(60,600,new Vector2(0,0),"city");
        //	DrawCirclePoints(70,700,new Vector2(0,0),"city");
        //	DrawCirclePoints(80,800,new Vector2(0,0),"city");
        //		DrawCirclePoints(90,900,new Vector2(0,0),"city");
        //		DrawCirclePoints(100,1000,new Vector2(0,0),"city");


        Rays("countryside");
		Rays("suburban");
		Rays("city");

        StartCoroutine("CellManager");
    
	}

    IEnumerator CellManager()
    {
        yield return new WaitForEndOfFrame();
        GetComponent<CellManager>().Start();
        yield break;
    }

	void DrawCirclePoints(int points, double radius, Vector2 center, string type)
	{
		
	//map out pints in a circle and add to one of the lists 
		
		float slice = 2 * Mathf.PI / points;
		for (int i = 0; i < points; i++)
		{
			float angle = slice * i;
			int newX = (int)(center.x + radius * Mathf.Cos(angle));
			int newY = (int)(center.y + radius * Mathf.Sin(angle));
			Vector2 p = new Vector2(newX, newY);

			//zero the y co ordinate and make a vector 3
			Vector3 p3 = new Vector3(p.x,0f,p.y);
			if (type == "countryside")
				countrysideList.Add(p3);

			if (type == "suburban")
				suburbanList.Add(p3);

			if (type == "city")
				cityList.Add(p3);
		}
	}

	void Rays(string type)
	{
		RaycastHit hit;
		Material countrysideMaterial = Resources.Load("Green") as Material;
		Material suburbanMaterial = Resources.Load("RosePink") as Material;
		Material cityMaterial = Resources.Load("Grey") as Material;
		LayerMask lM = LayerMask.GetMask("WorldPlan");
		//iterate through 'list' to see what is beneath the points inserted by the circle function

		if(type == "countryside")
		{
			for(int i = 0; i < countrysideList.Count; i++)
			{
				if (Physics.Raycast(countrysideList[i] + Vector3.up*1000, Vector3.down, out hit, 2000f,lM))
				{
					hit.transform.tag = "Countryside";
					hit.transform.GetComponent<MeshRenderer>().sharedMaterial = countrysideMaterial;

				}

				else
					Debug.Log("WorldPlan Raycast Missed (Countryside)");

			}
		}

		if(type == "suburban")
		{
			for(int i = 0; i < suburbanList.Count; i++)
			{
				if (Physics.Raycast(suburbanList[i] + Vector3.up, Vector3.down, out hit, 2f,lM))
				{
					hit.transform.tag = "Suburban";
					hit.transform.GetComponent<MeshRenderer>().sharedMaterial = suburbanMaterial;
					
				}
				
				else
					Debug.Log("WorldPlan Raycast Missed (Suburban)");
				
			}
		}

		if(type == "city")
		{
			for(int i = 0; i < cityList.Count; i++)
			{
				if (Physics.Raycast(cityList[i] + Vector3.up, Vector3.down, out hit, 2f,lM))
				{
					hit.transform.tag = "City";
					hit.transform.GetComponent<MeshRenderer>().sharedMaterial = cityMaterial;
					
				}
				
				else
					Debug.Log("WorldPlan Raycast Missed (City)");
				
			}
		}
	}

}
