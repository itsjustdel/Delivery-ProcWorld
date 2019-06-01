using UnityEngine;
using System.Collections;

public class CreateRoofAsOne : ProcBase{

	// Use this for initialization
	void Start () {
		HouseVariables hV;
		hV = GetComponent<HouseVariables>();
		BodyVariables house =hV.body;
		TileVariables tiles = hV.tiles;
		MeshBuilder meshBuilder = new MeshBuilder(); 

		BuildRoofAsOne(meshBuilder,house.Height,house.Width,house.Length,house.RoofHeight,house.RoofOverhangSide,
		               house.RoofOverhangFront,house.RoofBias,tiles.tilesX,tiles.tilesY,house.lookRot);

		GameObject child = new GameObject();
		child.name = "Roof";
		child.transform.position = this.gameObject.transform.position;
		child.transform.parent = this.gameObject.transform;
		
		MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
		meshCollider.convex = true;
		
		//					child.gameObject.AddComponent<Rigidbody>();
		//					FixedJoint joint = child.gameObject.AddComponent<FixedJoint>();
		//					joint.connectedBody = gameObject.GetComponent<Rigidbody>();
		//					joint.enableCollision = true;
		//					child.gameObject.GetComponent<FixedJoint>().breakForce = 1000;
		
		//child.gameObject.GetComponent<FixedJoint>().connectedAnchor = gameObject.GetComponent<Rigidbody>().position;
		
		MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();					
		Mesh mesh = meshBuilder.CreateMesh();
		meshCollider.sharedMesh = mesh;
		meshFilter.sharedMesh = mesh;					
		
		Material randomMat = new Material(Shader.Find("Standard"));
		Color newColor = new Color( Random.Range(-0.1f,0.1f),Random.Range(-0.1f,0.1f), Random.Range(-0.1f,0.1f),1.0f);
		meshRenderer.material = randomMat;
		Material newMat = Resources.Load("Grey",typeof(Material)) as Material;
		Material comboColour = new Material(Shader.Find("Standard"));
		comboColour.color = newMat.color * newColor;
		//meshRenderer.material =comboColour;
		meshRenderer.sharedMaterial =  Resources.Load("Grey",typeof(Material)) as Material;
		
		meshRenderer.shadowCastingMode = 0;
		meshRenderer.receiveShadows = false;
		//child.layer = 8;
	
	}
	
	
}
