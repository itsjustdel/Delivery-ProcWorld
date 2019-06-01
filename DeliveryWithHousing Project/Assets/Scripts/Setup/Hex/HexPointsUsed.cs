using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//This script keeps a list of which hex points are used by the pathfinders which create the roads.
//Its primary purpose is to stop the more than one road using any partuclar point, thus stopping any overlaps


public class HexPointsUsed : MonoBehaviour {

	public static List<Vector3> hexPointsUsed = new List<Vector3>();

}
