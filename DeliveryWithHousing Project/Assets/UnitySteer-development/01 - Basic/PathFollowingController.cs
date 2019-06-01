using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitySteer;
using UnitySteer.Behaviors;

[RequireComponent(typeof(SteerForPathSimplified))]
public class PathFollowingController : MonoBehaviour 
{

	public SteerForPathSimplified _steering;

	[SerializeField] Transform _pathRoot;

	[SerializeField] bool _followAsSpline;

	public List<Vector3> pathPoints;
	// Use this for initialization
	void Start() 
	{
		_steering = GetComponent<SteerForPathSimplified>();
		//AssignPath();
	}
/*
	void Update()
	{
		if (Vector3.Distance(transform.position, pathPoints[0]) < 2F)
		{		
			pathPoints.RemoveAt(0);
		}
	}
*/

	public void AssignPath()
	{
		/* ORIGINAL
		// Get a list of points to follow;
		var pathPoints = PathFromRoot(_pathRoot);
		_steering.Path = _followAsSpline ? new SplinePathway(pathPoints, 1) : new Vector3Pathway(pathPoints, 1);
		*/
		//get the waypoint list calculated by the waypoint pathfinder
//		pathPoints = gameObject.GetComponent<WaypointPlayer>().Path;
//		Debug.Log(gameObject.GetComponent<WaypointPlayer>().Path.Count);
		_steering.Path = _followAsSpline ? new SplinePathway(pathPoints, 1) : new Vector3Pathway(pathPoints, 1);
	}

	static List<Vector3> PathFromRoot(Transform root)
	{
		var children = new List<Transform>();
		foreach (Transform t in root)
		{
			if (t != null)
			{
				children.Add(t);
			}
		}
		return children.OrderBy(t => t.gameObject.name).Select(t => t.position).ToList();
	}


}
