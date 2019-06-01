using UnityEngine;
using System.Collections;

public class CreateRoofTile : ProcBase {

	void Start ()
	{
		StartCoroutine("Build");
	}

	IEnumerator Build()
	{

			HouseVariables hV;
			hV= GetComponent<HouseVariables>();
			
		
			BodyVariables house =hV.body;
			TileVariables tiles = hV.tiles;
		house.Width = (hV.bricks.brickAmtX * hV.bricks.brickSizeX);
		house.Length = (hV.bricks.brickAmtZ * hV.bricks.brickSizeZ) ;
		//house.Height = 	hV.bricks.brickAmtY * hV.bricks.brickSizeY;
		//house.Height *=2;
			//Vector3 upDir = house.lookRot * Vector3.up * house.Height ;
			Vector3 upDir = Vector3.up*(hV.bricks.brickAmtY*hV.bricks.brickSizeY);
			upDir*=2;//dont know why//because of roof peak


			//upDir+= (hV.bricks.brickSizeY*Vector3.up)*0.5;
//			Vector3 rightDir = house.lookRot * Vector3.right * house.Width ;
//			rightDir = (hVScript.body.Length/hVScript.body.bricksX) * Vector3.right;
//			rightDir  *= hVScript.body.bricksZ;
//			rightDir  += hVScript.body.Length/hVScript.body.bricksX * Vector3.right;

			Vector3 rightDir = Vector3.right*(hV.bricks.brickAmtZ*hV.bricks.brickSizeZ);

//			Vector3 forwardDir = house.lookRot * Vector3.forward * house.Length;
			Vector3 forwardDir = Vector3.forward * (hV.bricks.brickAmtX*hV.bricks.brickSizeX) + (hV.bricks.brickSizeZ*Vector3.forward);

			Vector3 nearCorner = Vector3.zero;
			Vector3 farCorner = upDir + rightDir + forwardDir;
			
			Vector3 pivotOffset = (rightDir)*0.5f + (forwardDir)*0.5f ;// +(hV.bricks.brickSizeZ*Vector3.forward)*0.5f;
			pivotOffset -= (hV.bricks.brickSizeX * Vector3.right) *0.25f; // thios line adjust it slightly so the peak is exactly in the middle of the top brick 
			//pivotOffset = Vector3.up;
			farCorner -= pivotOffset;
			nearCorner -= pivotOffset;
			
			Vector3 roofPeak = house.lookRot * ((Vector3.up * hV.bricks.brickSizeY*hV.bricks.brickAmtZ));
			roofPeak += Vector3.up*(hV.bricks.brickAmtY * hV.bricks.brickSizeY) * 2;
			roofPeak += rightDir * 0.5f - pivotOffset;
			//roofPeak -= Vector3.forward * hV.bricks.brickSizeX;
			//roofPeak *=0.75f;
			
		Vector3 wallTopLeft = upDir - rightDir*0.5f -forwardDir*0.5f + ((hV.bricks.brickSizeX * Vector3.right)*0.25f);
		//wallTopLeft *= 0.5f;
		Vector3 wallTopRight = upDir + rightDir*0.5f - forwardDir*0.5f + ((hV.bricks.brickSizeX * Vector3.right)*0.25f);

			//move the whoel roof back half a brick to create overhang
			wallTopRight -= Vector3.forward * hV.bricks.brickSizeX *0.5f;
			wallTopLeft -= Vector3.forward * hV.bricks.brickSizeX *0.5f;
			roofPeak -= Vector3.forward * hV.bricks.brickSizeX *0.5f;

			//lengthen the roof so it overhangs at other side too

			forwardDir += Vector3.forward * hV.bricks.brickSizeX *1f;

			Vector3 dirFromPeakLeft = wallTopLeft - roofPeak;
			Vector3 dirFromPeakRight = wallTopRight - roofPeak;
			Vector3 tileOffsetXF = roofPeak -(forwardDir/tiles.tilesX);
			Vector3 tileOffsetXF2 = roofPeak -(forwardDir/tiles.tilesX);
			Vector3 tileOffsetXB = roofPeak -(forwardDir/tiles.tilesX);
			Vector3 tileOffsetXB2 = roofPeak -(forwardDir/tiles.tilesX);

			dirFromPeakLeft += dirFromPeakLeft.normalized * house.RoofOverhangSide;
			dirFromPeakRight += dirFromPeakRight.normalized * house.RoofOverhangSide;
			
			roofPeak -= house.lookRot * Vector3.forward * house.RoofOverhangFront;
			forwardDir += house.lookRot * Vector3.forward * house.RoofOverhangFront * 2f;
			
			roofPeak += Vector3.up * house.RoofBias;
			
			house.lookRot = Quaternion.identity;


			//house.Height = (hV.bricks.brickSizeY+ hV.bricks.brickAmtY)*0.5f;
			//house.Width = (hV.bricks.brickSizeZ+ hV.bricks.brickAmtZ);
			//house.Length = hV.bricks.brickSizeX+ hV.bricks.brickAmtX;
			//BuildRoofAsOne(meshBuilder,house.Height,house.Width,house.Length,house.RoofHeight,house.RoofOverhangSide,
			               //house.RoofOverhangFront,house.RoofBias,tiles.tilesX,tiles.tilesY,house.lookRot);



			for (int i = 0; i < tiles.tilesX ; i++)
			{

				tileOffsetXF+=forwardDir/tiles.tilesX;
			
				for (int j = 0; j <= (tiles.tilesY/2); j++)
				{
					Vector3 tileOffsetYF=((dirFromPeakLeft/tiles.tilesY)*j)*2;

					MeshBuilder meshBuilder = new MeshBuilder(); 


					//LookRot*Random breakDown - x is how is spins when it faces you, Y is up and down, and z is how much it flaps up and down)
			/*		BuildTile(meshBuilder,
					        (hV.bricks.brickSizeY + hV.bricks.brickAmtY*0.5f),
					          house.Width,
					          house.Length,
					          (hV.bricks.brickSizeY + hV.bricks.brickAmtY*0.5f),//roofheight
					          house.RoofOverhangSide,
					          house.RoofOverhangFront,
					          house.RoofBias,
					          tiles.tilesX,
					          tiles.tilesY,
					          tileOffsetXF,
					          tileOffsetYF,
					          tiles.slant,
					          house.lookRot*Quaternion.Euler(Random.Range(-tiles.XSpin,tiles.XSpin),Random.Range(-tiles.YSpin,tiles.YSpin),Random.Range(0,tiles.ZSpin))
					          );


*/
						BuildTileHV(meshBuilder,hV,tileOffsetXF,tileOffsetYF,
				            house.lookRot*Quaternion.Euler(Random.Range(-tiles.XSpin,tiles.XSpin),Random.Range(-tiles.YSpin,tiles.YSpin),Random.Range(0,tiles.ZSpin))
				            );


					GameObject child = new GameObject();
					child.name = "Tile";
					child.transform.position = this.gameObject.transform.position;
					child.transform.parent = this.gameObject.transform;

			//		MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
			//		meshCollider.convex = true;

//					child.gameObject.AddComponent<Rigidbody>();
//					FixedJoint joint = child.gameObject.AddComponent<FixedJoint>();
//					joint.connectedBody = gameObject.GetComponent<Rigidbody>();
//					joint.enableCollision = true;
//					child.gameObject.GetComponent<FixedJoint>().breakForce = 1000;

					//child.gameObject.GetComponent<FixedJoint>().connectedAnchor = gameObject.GetComponent<Rigidbody>().position;
					
					MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
					MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();					
					Mesh mesh = meshBuilder.CreateMesh();
				//	meshCollider.sharedMesh = mesh;
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
				yield return new WaitForFixedUpdate();

				}
			}

			for (int i = 0; i <= tiles.tilesX; i++)
			{				
				tileOffsetXF2+=(forwardDir/tiles.tilesX);
				
				for (int j = 0; j <= (tiles.tilesY/2); j++)
				{
					Vector3 tileOffsetYF=(((dirFromPeakLeft/tiles.tilesY)*j)*2)+(dirFromPeakLeft/tiles.tilesY);
					MeshBuilder meshBuilder = new MeshBuilder();
				
					if (i == 0)
					{
				/*			BuildTile (meshBuilder,
					        house.Height,house.Width,
					        house.Length,
						    (hV.bricks.brickSizeY + hV.bricks.brickAmtY*0.5f),
					        house.RoofOverhangSide,
					        house.RoofOverhangFront,
					        house.RoofBias,
					        tiles.tilesX*2,
					        tiles.tilesY,
					        tileOffsetXF2,
					        tileOffsetYF,
						    tiles.slant,
						    house.lookRot*Quaternion.Euler(Random.Range(-tiles.XSpin,tiles.XSpin),Random.Range(-tiles.YSpin,tiles.YSpin),Random.Range(0,tiles.ZSpin))
						      );
				*/						
				//	BuildTileHV(meshBuilder,hV,tileOffsetXF2,tileOffsetYF,
				//	            house.lookRot*Quaternion.Euler(Random.Range(-tiles.XSpin,tiles.XSpin),Random.Range(-tiles.YSpin,tiles.YSpin),Random.Range(0,tiles.ZSpin))
				//	            );
					}
		
				else if (i == tiles.tilesX){

					/*
					BuildTile (meshBuilder,
					           house.Height,house.Width,
					           house.Length,
						           (hV.bricks.brickSizeY + hV.bricks.brickAmtY*0.5f),
					           house.RoofOverhangSide,
					           house.RoofOverhangFront,
					           house.RoofBias,
					           tiles.tilesX*2,
					           tiles.tilesY,
					           ((tileOffsetXF2)- (forwardDir/tiles.tilesX)*0.5f),
					           tileOffsetYF,
						       tiles.slant,
						           house.lookRot*Quaternion.Euler(Random.Range(-tiles.XSpin,tiles.XSpin),Random.Range(-tiles.YSpin,tiles.YSpin),Random.Range(0,tiles.ZSpin))
						        );

					*/

				//	BuildTileHV(meshBuilder,hV,(tileOffsetXF2)- ((forwardDir/tiles.tilesX)*0.5f),tileOffsetYF,
				//	            house.lookRot*Quaternion.Euler(Random.Range(-tiles.XSpin,tiles.XSpin),Random.Range(-tiles.YSpin,tiles.YSpin),Random.Range(0,tiles.ZSpin))
				//	            );
					}

					else if (i != 0 || i != tiles.tilesX)
					{
					/*
								BuildTile(meshBuilder,
						        house.Height,house.Width,
						        house.Length,
						          (hV.bricks.brickSizeY + hV.bricks.brickAmtY*0.5f),
						        house.RoofOverhangSide,
						        house.RoofOverhangFront,
						        house.RoofBias,
						        tiles.tilesX,
						        tiles.tilesY,
						        (tileOffsetXF2 - (forwardDir/tiles.tilesX)*0.5f),
						        tileOffsetYF,
						           tiles.slant,
						           house.lookRot*Quaternion.Euler(Random.Range(-tiles.XSpin,tiles.XSpin),Random.Range(-tiles.YSpin,tiles.YSpin),Random.Range(0,tiles.ZSpin))
						        );
*/
					BuildTileHV(meshBuilder,hV,(tileOffsetXF2 - (forwardDir/tiles.tilesX)*0.5f),tileOffsetYF,
					            	            house.lookRot*Quaternion.Euler(Random.Range(-tiles.XSpin,tiles.XSpin),Random.Range(-tiles.YSpin,tiles.YSpin),Random.Range(0,tiles.ZSpin))
					            	            );

					}

					GameObject child = new GameObject();
					child.name = "Tile";
					child.transform.position = this.gameObject.transform.position;
					child.transform.parent = this.gameObject.transform;

			//		MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
			//		meshCollider.convex = true;
					
					//child.gameObject.AddComponent<Rigidbody>();
					//FixedJoint joint = child.gameObject.AddComponent<FixedJoint>();
					//joint.connectedBody = gameObject.GetComponent<Rigidbody>();
					//child.gameObject.GetComponent<FixedJoint>().breakForce = 1000;
					//joint.enableCollision = true;

					MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
					MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();					
					Mesh mesh = meshBuilder.CreateMesh();
			//		meshCollider.sharedMesh = mesh;
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
				yield return new WaitForFixedUpdate();
				}
			}
			for (int i = 0; i < tiles.tilesX; i++)
			{
				
				tileOffsetXB+=forwardDir/tiles.tilesX;
				
				for (int j = 0; j <= (tiles.tilesY/2); j++)
				{
					Vector3 tileOffsetYB=(((dirFromPeakRight/tiles.tilesY)*j)*2);//+(dirFromPeakRight/tiles.tilesY);
					MeshBuilder meshBuilder = new MeshBuilder();
					
				/*	BuildTileBack(meshBuilder,
					              house.Height,
					              house.Width,
					              house.Length,
					              (hV.bricks.brickSizeY + hV.bricks.brickAmtY*0.5f),
					              house.RoofOverhangSide,
					              house.RoofOverhangFront,
					              house.RoofBias,
					              tiles.tilesX,
					              tiles.tilesY,
					              tileOffsetXB,
					              tileOffsetYB,
					              tiles.slant,
					              house.lookRot*Quaternion.Euler(Random.Range(-tiles.XSpin,tiles.XSpin),Random.Range(-tiles.YSpin,tiles.YSpin),Random.Range(0,tiles.ZSpin)));
				*/	


					GameObject child = new GameObject();
					child.name = "Tile";
					child.transform.position = this.gameObject.transform.position;
					child.transform.parent = this.gameObject.transform;
			//		MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
			//		meshCollider.convex = true;
					
					child.gameObject.AddComponent<Rigidbody>();
					FixedJoint joint = child.gameObject.AddComponent<FixedJoint>();
					joint.connectedBody = gameObject.GetComponent<Rigidbody>();
					child.gameObject.GetComponent<FixedJoint>().breakForce = 1000;
					joint.enableCollision = true;
				
					MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
					MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();					
					Mesh mesh = meshBuilder.CreateMesh();
				//	meshCollider.sharedMesh = mesh;
					meshFilter.sharedMesh = mesh;					

					Material randomMat = new Material(Shader.Find("Standard"));
					Color newColor = new Color( Random.Range(-0.1f,0.1f),Random.Range(-0.1f,0.1f), Random.Range(-0.1f,0.1f),1.0f);
					meshRenderer.material = randomMat;
					Material newMat = Resources.Load("Grey",typeof(Material)) as Material;
					Material comboColour = new Material(Shader.Find("Standard"));
					comboColour.color = newMat.color * newColor;
					meshRenderer.material =comboColour;
					meshRenderer.sharedMaterial =  Resources.Load("Grey",typeof(Material)) as Material;

					meshRenderer.shadowCastingMode = 0;
					meshRenderer.receiveShadows = false;
					//child.layer = 8;
				yield return new WaitForFixedUpdate();
				}
			}


			for (int i = 0; i <= tiles.tilesX; i++)
			{
				
				tileOffsetXB2+=forwardDir/tiles.tilesX;
				
				for (int j = 0; j <= (tiles.tilesY); j++)
				{
					Vector3 tileOffsetYB=(((dirFromPeakRight/tiles.tilesY)*j)*2)+(dirFromPeakRight/tiles.tilesY);
					MeshBuilder meshBuilder = new MeshBuilder();
				/*
					if (i == 0)
					{
					BuildTileBack (meshBuilder,
						           house.Height,
					               house.Width,
						           house.Length,
						            (hV.bricks.brickSizeY + hV.bricks.brickAmtY*0.5f),
						           house.RoofOverhangSide,
						           house.RoofOverhangFront,
						           house.RoofBias,
						           tiles.tilesX*2,
						           tiles.tilesY,
						           tileOffsetXB2,
						           tileOffsetYB,
						               tiles.slant,
						               house.lookRot*Quaternion.Euler(Random.Range(-tiles.XSpin,tiles.XSpin),Random.Range(-tiles.YSpin,tiles.YSpin),Random.Range(0,tiles.ZSpin)));
						
						
					}
					else if (i == tiles.tilesX){
						BuildTileBack (meshBuilder,
						           house.Height,house.Width,
						           house.Length,
						           (hV.bricks.brickSizeY + hV.bricks.brickAmtY*0.5f),
						           house.RoofOverhangSide,
						           house.RoofOverhangFront,
						           house.RoofBias,
						           tiles.tilesX*2,
						           tiles.tilesY,
						           ((tileOffsetXB2)-(forwardDir/tiles.tilesX)*0.5f),
						           tileOffsetYB,
						               tiles.slant,
						               house.lookRot*Quaternion.Euler(Random.Range(-tiles.XSpin,tiles.XSpin),Random.Range(-tiles.YSpin,tiles.YSpin),Random.Range(0,tiles.ZSpin)));
					
					}

					else if (i != 0 || i != tiles.tilesX)
					{
						BuildTileBack(meshBuilder,
						              house.Height,
						              house.Width,
						              house.Length,
						              (hV.bricks.brickSizeY + hV.bricks.brickAmtY*0.5f),
						              house.RoofOverhangSide,
						              house.RoofOverhangFront,
						              house.RoofBias,
						              tiles.tilesX,
						              tiles.tilesY,
						              (tileOffsetXB2 - (forwardDir/tiles.tilesX)*0.5f),
						              tileOffsetYB,
						              tiles.slant,
							              house.lookRot*Quaternion.Euler(Random.Range(-tiles.XSpin,tiles.XSpin),Random.Range(-tiles.YSpin,tiles.YSpin),Random.Range(0,tiles.ZSpin)));
						
					}
*/
					GameObject child = new GameObject();
					child.name = "Tile";
					child.transform.position = this.gameObject.transform.position;
					child.transform.parent = this.gameObject.transform;

				//	MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
				//	meshCollider.convex = true;
					
					child.gameObject.AddComponent<Rigidbody>();
					FixedJoint joint = child.gameObject.AddComponent<FixedJoint>();
					joint.connectedBody = gameObject.GetComponent<Rigidbody>();
					child.gameObject.GetComponent<FixedJoint>().breakForce = 1000;
					joint.enableCollision = true;

					MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
					MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();					
					Mesh mesh = meshBuilder.CreateMesh();
				//	meshCollider.sharedMesh = mesh;
					meshFilter.sharedMesh = mesh;					


//					Material randomMat = new Material(Shader.Find("Standard"));
//					Color newColor = new Color( Random.Range(-0.1f,0.1f),Random.Range(-0.1f,0.1f), Random.Range(-0.1f,0.1f),1.0f);
//					meshRenderer.material = randomMat;
//					Material newMat = Resources.Load("Grey",typeof(Material)) as Material;
//					Material comboColour = new Material(Shader.Find("Standard"));
//					comboColour.color = newMat.color * newColor;
//					meshRenderer.material =comboColour;

					meshRenderer.sharedMaterial =  Resources.Load("Grey",typeof(Material)) as Material;
					meshRenderer.shadowCastingMode = 0;
					meshRenderer.receiveShadows = false;
					//child.layer = 8;
				yield return new WaitForFixedUpdate();
										
				}
			}
		gameObject.GetComponent<MeshControlTiles>().enabled = true;
		//return meshBuilder.CreateMesh();
		}
		


}