using UnityEngine;
using System.Collections;



[System.Serializable]
public class WindowVariables{
	
	public float windowSize;
	public float strutWidth;
	public float strutDepth;
	public int amountOfPanesX;
	public int amountOfPanesY;
	public Vector3 position;
	public float windowOffset;
	
}

[System.Serializable]
public class BodyVariables{
	public int windowAmount;
	public int doorsAmount;
	public float Height;
	public float Width;
	public float Length;
	public float RoofHeight;
	public float RoofOverhangSide;
	public float RoofOverhangFront;
	public float RoofBias;
	public Quaternion lookRot;
	public float bricksX;
	public float bricksY;
	public float bricksZ;
	public float bricksDepth;
	public float brickSeperation;
	public float brickBreakForce;
}

[System.Serializable]
public class BrickVariables{

	public float brickSizeX;
	public float brickSizeY;
	public float brickSizeZ;
	public float brickAmtX;
	public float brickAmtY;
	public float brickAmtZ;
}

[System.Serializable]
public class TileVariables{
	public int tilesX;
	public int tilesY;
	public int maxTilt;
	public int slant;
	public float XSpin;
	public float YSpin;
	public float ZSpin;
	
}
public class HouseVariables : MonoBehaviour {


	public GameObject target;
	public BodyVariables body;
	public WindowVariables window;
	public TileVariables tiles;
	public BrickVariables bricks;
	// Use this for initialization
	void Update() {
		//transform.rotation = Quaternion.LookRotation( target.transform.position - transform.position);
	}


}
