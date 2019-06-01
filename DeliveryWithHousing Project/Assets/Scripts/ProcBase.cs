using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for procedural meshes. Contains generic initialisation code and shared methods such as BuildQuad() and BuildRing()
/// </summary>
public abstract class ProcBase : MonoBehaviour
{
	/// <summary>
	/// Method for building a mesh. Called in Start()
	/// </summary>
	/// <returns>The completed mesh</returns>
	//public abstract Mesh BuildMesh();

	/// <summary>
	/// Initialisation. Build the mesh and assigns it to the object's MeshFilter
	/// </summary>
	//private void Start()
//	{
//		//Build the mesh:
//		Mesh mesh = BuildMesh();
	//
	//	//Look for a MeshFilter component attached to this GameObject:
	//	MeshFilter filter = GetComponent<MeshFilter>();
	//
		//If the MeshFilter exists, attach the new mesh to it.
		//Assuming the GameObject also has a renderer attached, our new mesh will now be visible in the scene.
	//	if (filter != null)
	//	{
	//		filter.sharedMesh = mesh;
	//	}
	//}

	#region "BuildQuad() methods"

	/// <summary>
	/// Builds a single quad in the XZ plane, facing up the Y axis.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="offset">A position offset for the quad.</param>
	/// <param name="width">The width of the quad.</param>
	/// <param name="length">The length of the quad.</param>
	protected void BuildQuad(MeshBuilder meshBuilder, Vector3 offset, float width, float length)
	{
		meshBuilder.Vertices.Add(new Vector3(0.0f, 0.0f, 0.0f) + offset);
		meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
		meshBuilder.Normals.Add(Vector3.up);

		meshBuilder.Vertices.Add(new Vector3(0.0f, 0.0f, length) + offset);
		meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
		meshBuilder.Normals.Add(Vector3.up);

		meshBuilder.Vertices.Add(new Vector3(width, 0.0f, length) + offset);
		meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
		meshBuilder.Normals.Add(Vector3.up);

		meshBuilder.Vertices.Add(new Vector3(width, 0.0f, 0.0f) + offset);
		meshBuilder.UVs.Add(new Vector2(1.0f, 0.0f));
		meshBuilder.Normals.Add(Vector3.up);

		//we don't know how many verts the meshBuilder is up to, but we only care about the four we just added:
		int baseIndex = meshBuilder.Vertices.Count - 4;

		meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
		meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);
	}

	/// <summary>
	/// Builds a single quad based on a position offset and width and length vectors.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="offset">A position offset for the quad.</param>
	/// <param name="widthDir">The width vector of the quad.</param>
	/// <param name="lengthDir">The length vector of the quad.</param>
	protected void BuildQuad(MeshBuilder meshBuilder, Vector3 offset, Vector3 widthDir, Vector3 lengthDir)
	{
		Vector3 normal = Vector3.Cross(lengthDir, widthDir).normalized;

		meshBuilder.Vertices.Add(offset);
		meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
		meshBuilder.Normals.Add(normal);

		meshBuilder.Vertices.Add(offset + lengthDir);
		meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
		meshBuilder.Normals.Add(normal);

		meshBuilder.Vertices.Add(offset + lengthDir + widthDir);
		meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
		meshBuilder.Normals.Add(normal);

		meshBuilder.Vertices.Add(offset + widthDir);
		meshBuilder.UVs.Add(new Vector2(1.0f, 0.0f));
		meshBuilder.Normals.Add(normal);

		//we don't know how many verts the meshBuilder is up to, but we only care about the four we just added:
		int baseIndex = meshBuilder.Vertices.Count - 4;

		meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
		meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);
			
	}


	#endregion

	#region "BuildQuadForGrid() methods"

	/// <summary>
	/// Builds a single quad as part of a mesh grid.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="position">A position offset for the quad. Specifically the position of the corner vertex of the quad.</param>
	/// <param name="uv">The UV coordinates of the quad's corner vertex.</param>
	/// <param name="buildTriangles">Should triangles be built for this quad? This value should be false if this is the first quad in any row or collumn.</param>
	/// <param name="vertsPerRow">The number of vertices per row in this grid.</param>
	protected void BuildQuadForGrid(MeshBuilder meshBuilder, Vector3 position, Vector2 uv, bool buildTriangles, int vertsPerRow)
	{
		meshBuilder.Vertices.Add(position);
		meshBuilder.UVs.Add(uv);

		if (buildTriangles)
		{
			int baseIndex = meshBuilder.Vertices.Count - 1;

			int index0 = baseIndex;
			int index1 = baseIndex - 1;
			int index2 = baseIndex - vertsPerRow;
			int index3 = baseIndex - vertsPerRow - 1;

			meshBuilder.AddTriangle(index0, index2, index1);
			meshBuilder.AddTriangle(index2, index3, index1);
		}
	}

	/// <summary>
	/// Builds a single quad as part of a mesh grid.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="position">A position offset for the quad. Specifically the position of the corner vertex of the quad.</param>
	/// <param name="uv">The UV coordinates of the quad's corner vertex.</param>
	/// <param name="buildTriangles">Should triangles be built for this quad? This value should be false if this is the first quad in any row or collumn.</param>
	/// <param name="vertsPerRow">The number of vertices per row in this grid.</param>
	/// <param name="normal">The normal of the quad's corner vertex.</param>
	protected void BuildQuadForGrid(MeshBuilder meshBuilder, Vector3 position, Vector2 uv, bool buildTriangles, int vertsPerRow, Vector3 normal)
	{
		meshBuilder.Vertices.Add(position);
		meshBuilder.UVs.Add(uv);
		meshBuilder.Normals.Add(normal);

		if (buildTriangles)
		{
			int baseIndex = meshBuilder.Vertices.Count - 1;

			int index0 = baseIndex;
			int index1 = baseIndex - 1;
			int index2 = baseIndex - vertsPerRow;
			int index3 = baseIndex - vertsPerRow - 1;

			meshBuilder.AddTriangle(index0, index2, index1);
			meshBuilder.AddTriangle(index2, index3, index1);
		}
	}

	protected void BuildQuadForGridNoUV(MeshBuilder meshBuilder, Vector3 position, bool buildTriangles, int vertsPerRow, Vector3 normal)
	{
		meshBuilder.Vertices.Add(position);
		//meshBuilder.UVs.Add(uv);
		meshBuilder.Normals.Add(normal);
		
		if (buildTriangles)
		{
			int baseIndex = meshBuilder.Vertices.Count - 1;
			
			int index0 = baseIndex;
			int index1 = baseIndex - 1;
			int index2 = baseIndex - vertsPerRow;
			int index3 = baseIndex - vertsPerRow - 1;
			
			meshBuilder.AddTriangle(index0, index2, index1);
			meshBuilder.AddTriangle(index2, index3, index1);
		}
	}

	#endregion

	#region "BuildRing() methods"

	/// <summary>
	/// Builds a ring as part of a cylinder.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="segmentCount">The number of segments in this ring.</param>
	/// <param name="centre">The position at the centre of the ring.</param>
	/// <param name="radius">The radius of the ring.</param>
	/// <param name="v">The V coordinate for this ring.</param>
	/// <param name="buildTriangles">Should triangles be built for this ring? This value should be false if this is the first ring in the cylinder.</param>
	protected void BuildRing(MeshBuilder meshBuilder, int segmentCount, Vector3 centre, float radius, float v, bool buildTriangles)
	{
		float angleInc = (Mathf.PI * 2.0f) / segmentCount;

		for (int i = 0; i <= segmentCount; i++)
		{
			float angle = angleInc * i;

			Vector3 unitPosition = Vector3.zero;
			unitPosition.x = Mathf.Cos(angle);
			unitPosition.z = Mathf.Sin(angle);

			meshBuilder.Vertices.Add(centre + unitPosition * radius);
			meshBuilder.Normals.Add(unitPosition);
			meshBuilder.UVs.Add(new Vector2((float)i / segmentCount, v));

			if (i > 0 && buildTriangles)
			{
				int baseIndex = meshBuilder.Vertices.Count - 1;

				int vertsPerRow = segmentCount + 1;

				int index0 = baseIndex;
				int index1 = baseIndex - 1;
				int index2 = baseIndex - vertsPerRow;
				int index3 = baseIndex - vertsPerRow - 1;

				meshBuilder.AddTriangle(index0, index2, index1);
				meshBuilder.AddTriangle(index2, index3, index1);
			}
		}
	}

	/// <summary>
	/// Builds a ring as part of a cylinder.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="segmentCount">The number of segments in this ring.</param>
	/// <param name="centre">The position at the centre of the ring.</param>
	/// <param name="radius">The radius of the ring.</param>
	/// <param name="v">The V coordinate for this ring.</param>
	/// <param name="buildTriangles">Should triangles be built for this ring? This value should be false if this is the first ring in the cylinder.</param>
	/// <param name="rotation">A rotation value to be applied to the whole ring.</param>
	protected void BuildRing(MeshBuilder meshBuilder, int segmentCount, Vector3 centre, float radius, float v, bool buildTriangles, Quaternion rotation)
	{
		float angleInc = (Mathf.PI * 2.0f) / segmentCount;

		for (int i = 0; i <= segmentCount; i++)
		{
			float angle = angleInc * i;

			Vector3 unitPosition = Vector3.zero;
			unitPosition.x = Mathf.Cos(angle);
			unitPosition.z = Mathf.Sin(angle);

			unitPosition = rotation * unitPosition;

			meshBuilder.Vertices.Add(centre + unitPosition * radius);
			meshBuilder.Normals.Add(unitPosition);
			meshBuilder.UVs.Add(new Vector2((float)i / segmentCount, v));

			if (i > 0 && buildTriangles)
			{
				int baseIndex = meshBuilder.Vertices.Count - 1;

				int vertsPerRow = segmentCount + 1;

				int index0 = baseIndex;
				int index1 = baseIndex - 1;
				int index2 = baseIndex - vertsPerRow;
				int index3 = baseIndex - vertsPerRow - 1;

				meshBuilder.AddTriangle(index0, index2, index1);
				meshBuilder.AddTriangle(index2, index3, index1);
			}
		}
	}

	/// <summary>
	/// Builds a ring as part of a cylinder.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="segmentCount">The number of segments in this ring.</param>
	/// <param name="centre">The position at the centre of the ring.</param>
	/// <param name="radius">The radius of the ring.</param>
	/// <param name="v">The V coordinate for this ring.</param>
	/// <param name="buildTriangles">Should triangles be built for this ring? This value should be false if this is the first ring in the cylinder.</param>
	/// <param name="rotation">A rotation value to be applied to the whole ring.</param>
	/// <param name="slope">The normalised slope (rise and run) of the cylinder at this height.</param>
	protected void BuildRing(MeshBuilder meshBuilder, int segmentCount, Vector3 centre, float radius, float v, bool buildTriangles, Quaternion rotation, Vector2 slope)
	{
		float angleInc = (Mathf.PI * 2.0f) / segmentCount;

		for (int i = 0; i <= segmentCount; i++)
		{
			float angle = angleInc * i;

			Vector3 unitPosition = Vector3.zero;
			unitPosition.x = Mathf.Cos(angle);
			unitPosition.z = Mathf.Sin(angle);

			float normalVertical = -slope.x;
			float normalHorizontal = slope.y;

			Vector3 normal = unitPosition * normalHorizontal;
			normal.y = normalVertical;

			normal = rotation * normal;

			unitPosition = rotation * unitPosition;

			meshBuilder.Vertices.Add(centre + unitPosition * radius);
			meshBuilder.Normals.Add(normal);
			meshBuilder.UVs.Add(new Vector2((float)i / segmentCount, v));

			if (i > 0 && buildTriangles)
			{
				int baseIndex = meshBuilder.Vertices.Count - 1;

				int vertsPerRow = segmentCount + 1;

				int index0 = baseIndex;
				int index1 = baseIndex - 1;
				int index2 = baseIndex - vertsPerRow;
				int index3 = baseIndex - vertsPerRow - 1;

				meshBuilder.AddTriangle(index0, index2, index1);
				meshBuilder.AddTriangle(index2, index3, index1);
			}
		}
	}

	#endregion

	/// <summary>
	/// Builds a single triangle.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="corner0">The vertex position at index 0 of the triangle.</param>
	/// <param name="corner1">The vertex position at index 1 of the triangle.</param>
	/// <param name="corner2">The vertex position at index 2 of the triangle.</param>
	protected void BuildTriangle(MeshBuilder meshBuilder, Vector3 corner0, Vector3 corner1, Vector3 corner2)
	{
		Vector3 normal = Vector3.Cross((corner1 - corner0), (corner2 - corner0)).normalized;

		meshBuilder.Vertices.Add(corner0);
		meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
		meshBuilder.Normals.Add(normal);

		meshBuilder.Vertices.Add(corner1);
		meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
		meshBuilder.Normals.Add(normal);

		meshBuilder.Vertices.Add(corner2);
		meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
		meshBuilder.Normals.Add(normal);

		int baseIndex = meshBuilder.Vertices.Count - 3;

		meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
	}

	/// <summary>
	/// Builds a ring as part of a sphere. Normals are calculated as directions from the sphere's centre.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="segmentCount">The number of segments in this ring.</param>
	/// <param name="centre">The position at the centre of the ring.</param>
	/// <param name="radius">The radius of the ring.</param>
	/// <param name="v">The V coordinate for this ring.</param>
	/// <param name="buildTriangles">Should triangles be built for this ring? This value should be false if this is the first ring in the cylinder.</param>
	protected void BuildRingForSphere(MeshBuilder meshBuilder, int segmentCount, Vector3 centre, float radius, float v, bool buildTriangles)
	{
		float angleInc = (Mathf.PI * 2.0f) / segmentCount;

		for (int i = 0; i <= segmentCount; i++)
		{
			float angle = angleInc * i;

			Vector3 unitPosition = Vector3.zero;
			unitPosition.x = Mathf.Cos(angle);
			unitPosition.z = Mathf.Sin(angle);

			Vector3 vertexPosition = centre + unitPosition * radius;

			meshBuilder.Vertices.Add(vertexPosition);
			meshBuilder.Normals.Add(vertexPosition.normalized);
			meshBuilder.UVs.Add(new Vector2((float)i / segmentCount, v));

			if (i > 0 && buildTriangles)
			{
				int baseIndex = meshBuilder.Vertices.Count - 1;

				int vertsPerRow = segmentCount + 1;

				int index0 = baseIndex;
				int index1 = baseIndex - 1;
				int index2 = baseIndex - vertsPerRow;
				int index3 = baseIndex - vertsPerRow - 1;

				meshBuilder.AddTriangle(index0, index2, index1);
				meshBuilder.AddTriangle(index2, index3, index1);
			}
		}
	}



	protected void BuildWindows(MeshBuilder meshBuilder, float windowSize,float strutWidth, float strutDepth,
	                            int amountOfPanesX, int amountOfPanesY,Vector3 position,Quaternion lookRot)
	{
		Vector3 offset = lookRot*((position));
		Vector3 YOffset;// = lookRot*Vector3.zero + (position);
		Vector3 upDir = lookRot*Vector3.up * windowSize;
		Vector3 rightDir =  lookRot*Vector3.right * windowSize;
		Vector3 width = lookRot*Vector3.right * strutWidth;
		Vector3 height = lookRot*Vector3.up *strutWidth;
		Vector3 depth = lookRot*Vector3.forward * strutDepth;
		

		
		//offset += pivotOffset;

		for (int i = 0; i < amountOfPanesX; i++)
		{
			Vector3 mod = ((width+rightDir)*amountOfPanesX)*0.5f;
			offset+= (width) + rightDir;

			//for (int j = 0; j <amountOfPanesY; j++)
			//{
				YOffset = (upDir);
				//Y duplicate

				BuildQuad(meshBuilder,offset + YOffset - mod - width + height,upDir,width);//upwards strut
				BuildQuad(meshBuilder,offset + YOffset - mod - width + upDir,-rightDir,height);
				BuildQuad(meshBuilder,offset + YOffset- mod - width + upDir-rightDir,width,-upDir);
				BuildQuad(meshBuilder,offset + YOffset- mod - width + width,-rightDir,height);
				//Add depth
				
				//insides
				BuildQuad(meshBuilder,offset + YOffset - mod - width+ upDir, -depth,-rightDir + width);
				BuildQuad(meshBuilder,offset + YOffset - mod - width+ height,-rightDir + width,-depth);
				BuildQuad(meshBuilder,offset + YOffset - mod - width- rightDir + width+height,upDir - height,-depth);
				BuildQuad(meshBuilder,offset + YOffset - mod - width+ height,-depth,upDir - height);
				
				//outsides
				
				BuildQuad(meshBuilder,offset + YOffset - mod - width+ upDir + height - rightDir,-depth,rightDir + width);
				BuildQuad(meshBuilder,offset + YOffset - mod - width- rightDir, rightDir + width,-depth);
				BuildQuad(meshBuilder,offset + YOffset - mod - width- rightDir,-depth,upDir + height);
				BuildQuad(meshBuilder,offset + YOffset - mod - width+ width, upDir + height,-depth);
			//}
		
		}
	}

	protected void BuildWindow(MeshBuilder meshBuilder,
	                           float houseH,
	                           float houseW,
	                           float houseL,
	                           float windowSize,
	                           float strutWidth,
	                           float strutDepth,
	                           int amountOfPanesX,
	                           int amountOfPanesY,
	                           Vector3 pos,
	                           Quaternion lookRot,
	                           HouseVariables hV)
	{
//			Vector3 houseFwd = houseL*Vector3.forward;
			Vector3 houseRight = lookRot * Vector3.right * houseW;
			Vector3 houseHeight = houseH*Vector3.up;

			Vector3 YOffset;// = lookRot*Vector3.zero + (position);
			Vector3 upDir = lookRot*Vector3.up * windowSize;
			Vector3 rightDir =  lookRot*Vector3.right * windowSize;
			Vector3 width = lookRot*Vector3.right * strutWidth;
			Vector3 height = lookRot*Vector3.up *strutWidth;
			Vector3 depth = lookRot*Vector3.forward * strutDepth;

			Vector3 position= new Vector3();
			position.x = houseRight.x *hV.window.windowOffset;
			position.y = houseHeight.y*0.5f;
			position.z = pos.z;

		Vector3 offset = lookRot*((pos));
		offset -= houseRight*0.5f + (lookRot*Vector3.right *windowSize);
			//offset = rightDir*amountOfPanesX;

			for (int i = 0; i < amountOfPanesX; i++)
			{
				offset +=  rightDir/amountOfPanesX;

				for (int j = 0; j < amountOfPanesY; j++)
				{
					YOffset = (-upDir/amountOfPanesX)*j;
					Vector3 farCorner = offset - (rightDir/amountOfPanesX) + (upDir/amountOfPanesX);// + width;

					BuildQuad(meshBuilder,offset + YOffset,upDir/amountOfPanesX,width);//upwards strut
					BuildQuad(meshBuilder,offset + YOffset ,-rightDir/amountOfPanesX,height);	

					BuildQuad(meshBuilder,farCorner + YOffset+ width,height,rightDir/amountOfPanesX);
					BuildQuad(meshBuilder,farCorner + YOffset+ height,width,-upDir/amountOfPanesX);

					//Add depth
					
					//insides
					BuildQuad(meshBuilder,offset + YOffset,(-rightDir/amountOfPanesX)+width,-depth);
					BuildQuad(meshBuilder,offset + YOffset,-depth ,(upDir/amountOfPanesX) );
					BuildQuad(meshBuilder,farCorner + YOffset + width,-depth,-upDir/amountOfPanesX);
					BuildQuad(meshBuilder,farCorner + YOffset + width,(rightDir/amountOfPanesX)-width,-depth);
			


				}
			}
		//outsides

			BuildQuad(meshBuilder,offset + width - (upDir/amountOfPanesX)*amountOfPanesY +upDir/amountOfPanesX,-depth,-rightDir-width);
			BuildQuad(meshBuilder,offset + width - (upDir/amountOfPanesX)*amountOfPanesY +upDir/amountOfPanesX,((upDir/amountOfPanesX)*amountOfPanesY)+height,-depth);
			BuildQuad(meshBuilder,offset -rightDir + (upDir/amountOfPanesX) + height -depth ,rightDir+width,depth);
			BuildQuad(meshBuilder,offset -rightDir + (upDir/amountOfPanesX) + height -depth ,depth,((-upDir/amountOfPanesX)*amountOfPanesY)-height);
		

		
	}

	protected void BuildWindow2(MeshBuilder meshBuilder,
	                           float houseH,
	                           float houseW,
	                           float houseL,
	                           float windowSize,
	                           float strutWidth,
	                           float strutDepth,
	                           int amountOfPanesX,
	                           int amountOfPanesY,
	                           Vector3 pos,
	                           Quaternion lookRot,
	                           HouseVariables hV)
		
		
	{
		

//			Vector3 houseFwd = houseL* Vector3.forward;
			Vector3 houseRight = lookRot * Vector3.right * houseW;
			Vector3 houseHeight = houseH*Vector3.up;
			
			Vector3 YOffset;// = lookRot*Vector3.zero + (position);
			Vector3 upDir = lookRot*Vector3.up * windowSize;
			Vector3 rightDir =  lookRot*Vector3.right * windowSize;
			Vector3 width = lookRot*Vector3.right * strutWidth;
			Vector3 height = lookRot*Vector3.up *strutWidth;
			Vector3 depth = lookRot*Vector3.forward * strutDepth;
			
			Vector3 position= new Vector3();
			position.x = houseRight.x *hV.window.windowOffset;
			position.y = houseHeight.y*0.5f;
			position.z = pos.z;
			
			Vector3 offset = lookRot*((pos));
			offset -= houseRight*0.5f + (lookRot*Vector3.right *windowSize);
			
			for (int i = 0; i < amountOfPanesX; i++)
			{
				offset +=  rightDir/amountOfPanesX;
				
				for (int j = 0; j < amountOfPanesY; j++)
				{
					YOffset = (-upDir/amountOfPanesX)*j;
					Vector3 farCorner = offset - (rightDir/amountOfPanesX) + (upDir/amountOfPanesX);// + width;
					
					BuildQuad(meshBuilder,offset + YOffset,upDir/amountOfPanesX,width);//upwards strut
					BuildQuad(meshBuilder,offset + YOffset ,-rightDir/amountOfPanesX,height);	
					
					BuildQuad(meshBuilder,farCorner + YOffset+ width,height,rightDir/amountOfPanesX);
					BuildQuad(meshBuilder,farCorner + YOffset+ height,width,-upDir/amountOfPanesX);
					
					//Add depth
					
					//insides
					BuildQuad(meshBuilder,offset + YOffset,(-rightDir/amountOfPanesX)+width,-depth);
					BuildQuad(meshBuilder,offset + YOffset,-depth ,(upDir/amountOfPanesX) );
					BuildQuad(meshBuilder,farCorner + YOffset + width,-depth,-upDir/amountOfPanesX);
					BuildQuad(meshBuilder,farCorner + YOffset + width,(rightDir/amountOfPanesX)-width,-depth);
					
					
					
				}
			}
			//outsides
			
			BuildQuad(meshBuilder,offset + width - (upDir/amountOfPanesX)*amountOfPanesY +upDir/amountOfPanesX,-depth,-rightDir-width);
			BuildQuad(meshBuilder,offset + width - (upDir/amountOfPanesX)*amountOfPanesY +upDir/amountOfPanesX,((upDir/amountOfPanesX)*amountOfPanesY)+height,-depth);
			BuildQuad(meshBuilder,offset -rightDir + (upDir/amountOfPanesX) + height -depth ,rightDir+width,depth);
			BuildQuad(meshBuilder,offset -rightDir + (upDir/amountOfPanesX) + height -depth ,depth,((-upDir/amountOfPanesX)*amountOfPanesY)-height);
			

		
	}

	protected void BuildTestWall (MeshBuilder meshBuilder,
	                         float houseH,
	                         float houseW,
	                         float houseL,
	                         float windowSize,
	                         float strutWidth,
	                         float strutDepth,
	                         int amountOfPanesX,
	                         int amountOfPanesY,
	                         Vector3 pos,
	                         Quaternion lookRot,
	                         HouseVariables hV)
		
	{
		//build quad under window


//		int windowsAmount = hV.body.windowAmount;
//		int doorsAmount = hV.body.doorsAmount;

		Vector3 upDir =  lookRot * Vector3.up * houseH ;//lookRot *
		Vector3 rightDir =lookRot*  Vector3.right * houseW; //lookRot *
		Vector3 forwardDir =lookRot* Vector3.forward * houseL;// lookRot *

		Vector3 nearCorner = Vector3.zero;
		Vector3 farCorner = upDir + rightDir + forwardDir;
		
		//shift pivot to centre-bottom
		
		Vector3 pivotOffset = (rightDir + forwardDir) *0.5f;
		farCorner -= pivotOffset;
		nearCorner -= pivotOffset;

		Vector3 houseHeight =  lookRot * Vector3.up * houseH ;
		//Vector3 windowHeight = lookRot * Vector3.up * (windowSize/amountOfPanesY) ;

		//Find centre of the height of the house
		Vector3 houseCentre = houseHeight*0.5f;
		//Find one size of a window pane
		Vector3 paneSize = lookRot*Vector3.up* (windowSize/amountOfPanesX);

		//find how far the bottom of the window is from the centre
		Vector3 windowBottom = houseCentre - (paneSize*amountOfPanesY) ;





		//if (windowsAmount == 1 && doorsAmount ==0){

			//draw quad from bottom of house to bottom of window
			BuildQuad(meshBuilder,nearCorner,windowBottom + paneSize,forwardDir);
			
			//Find the origin of the window relatively to the house
			Vector3 windowOriginX = houseL*Vector3.forward * hV.window.windowOffset;///////////////*****************?//////////////
			
			//draw quad from bottom of window to top of window on the right hand side
			BuildQuad(meshBuilder,nearCorner+windowBottom + paneSize,
			          (paneSize*amountOfPanesY) + strutWidth*Vector3.up,
			          windowOriginX);

			//draw quad from bottom of window to top of window on the left hand side

			BuildQuad(meshBuilder,nearCorner + windowOriginX + (windowSize*Vector3.forward + strutWidth*Vector3.forward)+ (windowBottom + paneSize),
			          (paneSize*amountOfPanesY) + strutWidth*Vector3.up,
			          forwardDir -  windowOriginX -(windowSize*Vector3.forward + strutWidth*Vector3.forward));

			//draw quad above window

			BuildQuad(meshBuilder,nearCorner + houseCentre + paneSize + strutWidth*Vector3.up,
			          upDir - (houseCentre + paneSize + strutWidth*Vector3.up),
			          forwardDir);
		//}

	
		          

		
	}

	protected void BuildTestWall2 (MeshBuilder meshBuilder,
	                              float houseH,
	                              float houseW,
	                              float houseL,
	                              float windowSize,
	                              float strutWidth,
	                              float strutDepth,
	                              int amountOfPanesX,
	                              int amountOfPanesY,
	                              Vector3 pos,
	                              Quaternion lookRot,
	                              HouseVariables hV)
		
	{
		//build quad under window
		
		
//		int windowsAmount = hV.body.windowAmount;
//		int doorsAmount = hV.body.doorsAmount;
		
		Vector3 upDir =  lookRot * Vector3.up * houseH ;//lookRot *
		Vector3 rightDir = lookRot * -Vector3.right * houseW; //lookRot *
		Vector3 forwardDir = lookRot * Vector3.forward * houseL;// lookRot *
		
		Vector3 nearCorner = Vector3.zero;
		Vector3 farCorner = upDir + rightDir + forwardDir;
		
		//shift pivot to centre-bottom
		
		Vector3 pivotOffset = (rightDir + forwardDir) *0.5f;
		farCorner -= pivotOffset;
		nearCorner -= pivotOffset;
		
		Vector3 houseHeight =  lookRot * Vector3.up * houseH ;
		//Vector3 windowHeight = lookRot * Vector3.up * (windowSize/amountOfPanesY) ;
		
		//Find centre of the height of the house
		Vector3 houseCentre = houseHeight*0.5f;
		//Find one size of a window pane
		Vector3 paneSize = lookRot*Vector3.up* (windowSize/amountOfPanesX);
		
		//find how far the bottom of the window is from the centre
		Vector3 windowBottom = houseCentre - (paneSize*amountOfPanesY);
		
		//if (windowsAmount == 1 && doorsAmount ==0){
			
			//draw quad from bottom of house to bottom of window
			BuildQuad(meshBuilder,nearCorner,forwardDir,windowBottom + paneSize);
			
			//Find the origin of the window relatively to the house
		//Vector3 windowOriginX = (houseL*Vector3.forward * hV.window.windowOffset) - (windowSize*Vector3.forward) - (strutWidth*Vector3.forward);
		Vector3 windowOriginX = houseL*Vector3.forward -(houseL*Vector3.forward * hV.window.windowOffset);///////////////*****************?//////////////	
		windowOriginX -= (lookRot * Vector3.forward * windowSize) ;
		windowOriginX -= (lookRot * Vector3.forward * strutWidth) ;
		//draw quad from bottom of window to top of window on the right hand side
			BuildQuad(meshBuilder,nearCorner+windowBottom + paneSize,
			          windowOriginX,
			          (paneSize*amountOfPanesY) + strutWidth*Vector3.up
			         );
			
			//draw quad from bottom of window to top of window on the left hand side
			
			BuildQuad(meshBuilder,nearCorner + windowOriginX + (windowSize*Vector3.forward + strutWidth*Vector3.forward)+ (windowBottom + paneSize),
			          forwardDir -  windowOriginX -(windowSize*Vector3.forward + strutWidth*Vector3.forward),
			          (paneSize*amountOfPanesY) + strutWidth*Vector3.up
			         );
			
			//draw quad above window
			
			BuildQuad(meshBuilder,nearCorner + houseCentre + paneSize + strutWidth*Vector3.up,
			          forwardDir,
			          upDir - (houseCentre + paneSize + strutWidth*Vector3.up)
			          );
		//}
		
		
		
	}

	protected void WallNoWindows(MeshBuilder meshBuilder,
	                             float houseH,
	                             float houseW,
	                             float houseL,
	                             float windowSize,
	                             float strutWidth,
	                             float strutDepth,
	                             int amountOfPanesX,
	                             int amountOfPanesY,
	                             Vector3 pos,
	                             Quaternion lookRot,
	                             HouseVariables hV)
	{

		Vector3 upDir =  lookRot * Vector3.up * houseH ;//lookRot *
//		Vector3 rightDir = lookRot * -Vector3.right * houseW; //lookRot *
		Vector3 forwardDir = lookRot * Vector3.forward * houseL;// lookRot *
		
		Vector3 nearCorner = Vector3.zero;
//		Vector3 farCorner = upDir + rightDir + forwardDir;
			//BuildQuad(meshBuilder,nearCorner,rightDir,upDir);
			BuildQuad(meshBuilder,nearCorner,upDir,forwardDir);		
			//BuildQuad(meshBuilder, farCorner, -upDir, -rightDir);
			//BuildQuad(meshBuilder, farCorner, -forwardDir, -upDir);
		}


	protected void BuildRoofAsOne(MeshBuilder meshBuilder,
	                   		 float m_Height,
	                         float m_Width,
	                         float m_Length,
	                         float m_RoofHeight,
	                         float m_RoofOverhangSide,
	                         float m_RoofOverhangFront,
	                         float m_RoofBias,
	                         int tilesX,
	                         int tilesY,
	                         Quaternion lookRot
	                          )
	{
		//roof
		float slant = 50f;
		Vector3 upDir = lookRot * Vector3.up * m_Height ;
		Vector3 rightDir = lookRot * Vector3.right * m_Width ;
		Vector3 forwardDir = lookRot * Vector3.forward * m_Length;
		
		Vector3 nearCorner = Vector3.zero;
		Vector3 farCorner = upDir + rightDir + forwardDir;

		Vector3 pivotOffset = (rightDir + forwardDir) *0.5f;
		farCorner -= pivotOffset;
		nearCorner -= pivotOffset;
		
		Vector3 roofPeak = lookRot * Vector3.up* (m_Height+m_RoofHeight) + rightDir * 0.5f - pivotOffset;
		
		Vector3 wallTopLeft = upDir - pivotOffset;
		Vector3 wallTopRight = upDir + rightDir - pivotOffset;
		
		Vector3 dirFromPeakLeft = wallTopLeft - roofPeak;
		Vector3 dirFromPeakRight = wallTopRight - roofPeak;
		
		dirFromPeakLeft += dirFromPeakLeft.normalized * m_RoofOverhangSide;
		dirFromPeakRight += dirFromPeakRight.normalized * m_RoofOverhangSide;
		
		roofPeak -= lookRot * Vector3.forward * m_RoofOverhangFront;
		forwardDir += lookRot * Vector3.forward * m_RoofOverhangFront * 2f;
		
		roofPeak += Vector3.up * m_RoofBias;
		
		//build roof faces
		
		BuildQuad(meshBuilder,roofPeak,forwardDir,dirFromPeakLeft);
		BuildQuad(meshBuilder,roofPeak,dirFromPeakRight,forwardDir);
		
		//Tiles
		Vector3 tileOffsetX = roofPeak -(forwardDir/tilesX);
		

		//First Row, Third Row, Fifth Row etc, front and back

		for (int i = 0; i <tilesX; i++)
		{
			tileOffsetX+=forwardDir/tilesX;
			
			for (int j = 0; j <(tilesY/2); j++)
			{
				Vector3 tileOffsetYB=((dirFromPeakLeft/tilesY)*j)*2;
				
				BuildQuad(meshBuilder,(tileOffsetX + tileOffsetYB),
				          (forwardDir/tilesX)-(forwardDir/(tilesX*slant)),
				          (dirFromPeakLeft/tilesY)-(rightDir/slant)
				          );
				
				Vector3 tileOffsetYF = ((dirFromPeakRight/tilesY)*j)*2;
				
				BuildQuad(meshBuilder,tileOffsetX + tileOffsetYF,
				          (dirFromPeakRight/tilesY)+(rightDir/slant),
				          (forwardDir/tilesX)-(forwardDir/(tilesX*slant))
				         );
				
			}
		}

		//Second Row, Fourth, etc.

		for (int i = 0; i <tilesX; i++)
		{
			tileOffsetX-=(forwardDir/tilesX);
			
			for (int j = 0; j <(tilesY/2); j++)
			{
				Vector3 tileOffsetYB=(((dirFromPeakLeft/tilesY)*j)*2)+(dirFromPeakLeft/tilesY);
				
				BuildQuad(meshBuilder,(tileOffsetX + tileOffsetYB) +(forwardDir/tilesX)*0.5f,
				          (forwardDir/tilesX)-(forwardDir/(tilesX*slant)),
				          (dirFromPeakLeft/tilesY)-(rightDir/slant)
				          );
				
				Vector3 tileOffsetYF = ((dirFromPeakRight/tilesY)*j)*2+(dirFromPeakRight/tilesY);
				
				BuildQuad(meshBuilder,(tileOffsetX + tileOffsetYF) +(forwardDir/tilesX)*0.5f,
				          (dirFromPeakRight/tilesY)+(rightDir/slant),
				          (forwardDir/tilesX)-(forwardDir/(tilesX*slant))
				        );
				
			}
		}

				//backfaces
		BuildQuad(meshBuilder,roofPeak,dirFromPeakLeft,forwardDir);
		BuildQuad(meshBuilder,roofPeak,forwardDir,dirFromPeakRight);
	}

	protected void BuildTileHV(MeshBuilder meshBuilder,
	                         HouseVariables hV,
	                          Vector3 tileOffsetX,
	                           Vector3 tileOffsetY,
	                         Quaternion lookRot)
		
	{
		
		Vector3 upDir = lookRot*Vector3.up *hV.bricks.brickAmtY * hV.bricks.brickSizeY;
		//upDir*=0.5f;
		Vector3 rightDir = lookRot* Vector3.right * hV.bricks.brickAmtZ * hV.bricks.brickSizeZ;

		Vector3 forwardDir = lookRot*Vector3.forward *hV.bricks.brickAmtX * hV.bricks.brickSizeX;
		
		//	Vector3 nearCorner = Vector3.zero;
		Vector3 farCorner = upDir + rightDir + forwardDir;
		
		Vector3 pivotOffset = ((rightDir) + forwardDir);
		//farCorner -= pivotOffset;
		//nearCorner -= pivotOffset;
		
		Vector3 roofPeak =  lookRot*Vector3.up* (hV.bricks.brickAmtY * hV.bricks.brickSizeY) + (rightDir * 0.5f) - pivotOffset;//*rot
		
		Vector3 wallTopLeft = (upDir) - pivotOffset;

		Vector3 wallTopRight = (upDir) + rightDir - pivotOffset;		
		
		Vector3 dirFromPeakLeft = wallTopLeft - roofPeak;

		Vector3 dirFromPeakRight = wallTopRight - roofPeak;

		dirFromPeakLeft += dirFromPeakLeft.normalized * hV.body.RoofOverhangSide;
		dirFromPeakRight += dirFromPeakRight.normalized * hV.body.RoofOverhangSide;		
		
		forwardDir +=  Vector3.forward * hV.body.RoofOverhangFront ;//*rot

		int tilesX = hV.tiles.tilesX;
		int tilesY = hV.tiles.tilesY;
		int slant = hV.tiles.slant;
		
		Vector3 offset = tileOffsetX + tileOffsetY;// + (upDir/100);
		
		BuildQuad(meshBuilder,offset,
		          (forwardDir/tilesX)-(forwardDir/(tilesX*slant)),
		          (dirFromPeakLeft/tilesY)-(rightDir/slant)
		          );
		BuildQuad(meshBuilder,offset,
		          (dirFromPeakLeft/tilesY)-(rightDir/slant),
		          -upDir/100);//using this line makes the tilepoint straight downwards/ add offset to make proper cube ;)
		BuildQuad(meshBuilder,offset ,
		          -(upDir/100),
		          (forwardDir/tilesX)-(forwardDir/(tilesX*slant))
		          );
		
		//length plus width plus depth of tile is farCorner
		
		farCorner = (forwardDir/tilesX)-(forwardDir/(tilesX*slant)) +  (dirFromPeakLeft/tilesY)-(rightDir/slant) + (-upDir/100);
		
		BuildQuad(meshBuilder,offset + farCorner,
		          -((dirFromPeakLeft/tilesY)-(rightDir/slant)),
		          -(forwardDir/tilesX)+(forwardDir/(tilesX*slant))
		          );
		
		BuildQuad(meshBuilder,offset + farCorner,
		          upDir/100,
		          -((dirFromPeakLeft/tilesY)-(rightDir/slant))
		          );
		BuildQuad(meshBuilder,offset + farCorner,
		          -((forwardDir/tilesX)-(forwardDir/(tilesX*slant))),
		          upDir/100
		          );
		
		
		
	}

	protected void BuildTile(MeshBuilder meshBuilder,
	                         float m_Height,
	                         float m_Width,
	                         float m_Length,
	                         float m_RoofHeight,
	                         float m_RoofOverhangSide,
	                         float m_RoofOverhangFront,
	                         float m_RoofBias,
	                         int tilesX,
	                         int tilesY,
	                         Vector3 tileOffsetX,
	                         Vector3 tileOffsetY,
	                         float slant,
	                         Quaternion lookRot)

	{

		Vector3 upDir = lookRot*Vector3.up * m_Height ;//*rot
		Vector3 rightDir =lookRot* Vector3.right * m_Width ;//*rot
		Vector3 forwardDir = lookRot*Vector3.forward * m_Length;//*rot
		
	//	Vector3 nearCorner = Vector3.zero;
		Vector3 farCorner = upDir + rightDir + forwardDir;
		
		Vector3 pivotOffset = (rightDir + forwardDir) *0.5f;
		//farCorner -= pivotOffset;
		//nearCorner -= pivotOffset;
		
		Vector3 roofPeak =  lookRot*Vector3.up* (m_Height+m_RoofHeight) + rightDir * 0.5f - pivotOffset;//*rot
		
		Vector3 wallTopLeft = upDir - pivotOffset;
		Vector3 wallTopRight = upDir + rightDir - pivotOffset;
		
		
		
		Vector3 dirFromPeakLeft = wallTopLeft - roofPeak;
		Vector3 dirFromPeakRight = wallTopRight - roofPeak;
		
		dirFromPeakLeft += dirFromPeakLeft.normalized * m_RoofOverhangSide;
		dirFromPeakRight += dirFromPeakRight.normalized * m_RoofOverhangSide;
		

		forwardDir +=  Vector3.forward * m_RoofOverhangFront * 2f;//*rot
		

				
		Vector3 offset = tileOffsetX + tileOffsetY + (upDir/100);

		BuildQuad(meshBuilder,offset,
		          (forwardDir/tilesX)-(forwardDir/(tilesX*slant)),
		          (dirFromPeakLeft/tilesY)-(rightDir/slant)
		          );
		BuildQuad(meshBuilder,offset,
		          (dirFromPeakLeft/tilesY)-(rightDir/slant),
		          -upDir/100);//using this line makes the tilepoint straight downwards/ add offset to make proper cube ;)
		BuildQuad(meshBuilder,offset ,
		          -(upDir/100),
		          (forwardDir/tilesX)-(forwardDir/(tilesX*slant))
		          );

		//length plus width plus depth of tile is farCorner

		farCorner = (forwardDir/tilesX)-(forwardDir/(tilesX*slant)) +  (dirFromPeakLeft/tilesY)-(rightDir/slant) + (-upDir/100);

		BuildQuad(meshBuilder,offset + farCorner,
		          -((dirFromPeakLeft/tilesY)-(rightDir/slant)),
		          -(forwardDir/tilesX)+(forwardDir/(tilesX*slant))
		          );

		BuildQuad(meshBuilder,offset + farCorner,
		          upDir/100,
				  -((dirFromPeakLeft/tilesY)-(rightDir/slant))
					);
		BuildQuad(meshBuilder,offset + farCorner,
		          -((forwardDir/tilesX)-(forwardDir/(tilesX*slant))),
		          upDir/100
		          );

		
		
	}


		

	
	protected void BuildTileBack(MeshBuilder meshBuilder,
	                         float m_Height,
	                         float m_Width,
	                         float m_Length,
	                         float m_RoofHeight,
	                         float m_RoofOverhangSide,
	                         float m_RoofOverhangFront,
	                         float m_RoofBias,
	                         int tilesX,
	                         int tilesY,
	                         Vector3 tileOffsetX,
	                         Vector3 tileOffsetY,
	                         float slant,
	                         Quaternion lookRot)
		
	{
		
		Vector3 upDir = lookRot* Vector3.up * m_Height ;//*rot
		Vector3 rightDir = lookRot*Vector3.right * m_Width ;//
		Vector3 forwardDir = lookRot*Vector3.forward * m_Length;//*rot
		
		Vector3 nearCorner = Vector3.zero;
		Vector3 farCorner = upDir + rightDir + forwardDir;
		
		Vector3 pivotOffset = (rightDir + forwardDir) *0.5f;
		farCorner -= pivotOffset;
		nearCorner -= pivotOffset;
		
		Vector3 roofPeak = lookRot* Vector3.up* (m_Height+m_RoofHeight) + rightDir * 0.5f - pivotOffset;//*rot


		Vector3 wallTopLeft = upDir - pivotOffset;
		Vector3 wallTopRight = upDir + rightDir - pivotOffset;
		
		
		
		Vector3 dirFromPeakLeft = wallTopLeft - roofPeak;
		Vector3 dirFromPeakRight = wallTopRight - roofPeak;
		
		dirFromPeakLeft += dirFromPeakLeft.normalized * m_RoofOverhangSide;
		dirFromPeakRight += dirFromPeakRight.normalized * m_RoofOverhangSide;
		
		//roofPeak -=  Vector3.forward * m_RoofOverhangFront;//*rot
		forwardDir +=  Vector3.forward * m_RoofOverhangFront * 2f;//*rot
		
		//roofPeak += (Vector3.up * m_RoofBias) + (upDir);
		

		Vector3 widthDir = (dirFromPeakRight/tilesY)+(rightDir/slant);
		Vector3 lengthDir = (forwardDir/tilesX)-(forwardDir/(tilesX*slant));
		Vector3 offset = (tileOffsetX +tileOffsetY)+ (upDir/100);

		BuildQuad(meshBuilder,offset,widthDir,lengthDir);
		BuildQuad(meshBuilder,offset, lengthDir,-upDir/100);
		BuildQuad(meshBuilder,offset,-upDir/100,widthDir);

		farCorner = forwardDir/tilesX-(forwardDir/(tilesX*slant)) +  (dirFromPeakRight/tilesY)+(rightDir/slant) + (-upDir/100);

		BuildQuad(meshBuilder,offset+farCorner,-lengthDir,-widthDir);
		BuildQuad(meshBuilder,offset+farCorner,-widthDir,upDir/100);
		BuildQuad(meshBuilder,offset+farCorner,upDir/100,-lengthDir);

	

		
		
	}
	protected void BuildBody(MeshBuilder meshBuilder,
	                   		 float m_Height,
	                         float m_Width,
	                         float m_Length,
	                         float m_RoofHeight,
	                         float m_RoofOverhangSide,
	                         float m_RoofOverhangFront,
	                         float m_RoofBias,
	                         int tilesX,
	                         int tilesY,
	                         Quaternion lookRot,
	                         HouseVariables hV )
	{
	//	Vector3 upDir =  lookRot * Vector3.up * m_Height ;//lookRot *
	//	Vector3 rightDir =lookRot*  Vector3.right * m_Width; //lookRot *
	//	Vector3 forwardDir =lookRot* Vector3.forward * m_Length;// lookRot *

		Vector3 forwardDir =  lookRot * Vector3.forward * (hV.bricks.brickSizeX * hV.bricks.brickAmtX);// +  (hV.bricks.brickSizeZ*Vector3.forward);
		Vector3 rightDir =  lookRot * Vector3.right * (hV.bricks.brickSizeX * hV.bricks.brickAmtZ) + hV.bricks.brickSizeZ*Vector3.right;  
		Vector3 upDir =  lookRot * Vector3.up * (hV.bricks.brickSizeY * hV.bricks.brickAmtY) + (hV.bricks.brickSizeY*Vector3.up);
		
		Vector3 nearCorner = Vector3.zero;// - (hV.bricks.brickSizeZ*Vector3.forward);
		Vector3 farCorner = upDir + rightDir + forwardDir;
		
		//shift pivot to centre-bottom
		
		Vector3 pivotOffset = (rightDir *0.5f) - (hV.bricks.brickSizeZ*Vector3.right*0.5f);
		farCorner -= pivotOffset;
		nearCorner -= pivotOffset;

		Vector3 roofPeak = lookRot* Vector3.up* (m_Height+m_RoofHeight) + rightDir * 0.5f - pivotOffset;//lookRot *
		Vector3 wallTopLeft = upDir - pivotOffset;
		Vector3 wallTopRight = upDir + rightDir - pivotOffset;

		BuildTriangle(meshBuilder,wallTopLeft,roofPeak,wallTopRight);
		BuildTriangle(meshBuilder,wallTopLeft+forwardDir,wallTopRight+forwardDir,roofPeak+forwardDir);

		//walls
		
	//	BuildQuad(meshBuilder,nearCorner,rightDir,upDir);//peakside
	//	BuildQuad(meshBuilder,nearCorner,upDir,forwardDir);		
	//	BuildQuad(meshBuilder, farCorner, -upDir, -rightDir);//peakside
	//	BuildQuad(meshBuilder, farCorner, -forwardDir, -upDir);
		
		//roof face
	//	Vector3 dirFromPeakLeft = wallTopLeft - roofPeak;
	//	Vector3 dirFromPeakRight = wallTopRight - roofPeak;

	//	BuildQuad(meshBuilder,roofPeak,forwardDir,dirFromPeakLeft);
	//	BuildQuad(meshBuilder,roofPeak,dirFromPeakRight,forwardDir);



		
	}

	protected void BuildBrickWall(MeshBuilder meshbuilder,
	                              HouseVariables hV,Vector3 offset,Vector3 yOffset)
	{
		//hV.body.lookRot = Quaternion.Euler(Random.Range(-hV.tiles.XSpin,hV.tiles.XSpin),
		  //                                  Random.Range(-hV.tiles.YSpin,hV.tiles.YSpin),
		    //                                Random.Range(-hV.tiles.ZSpin,hV.tiles.ZSpin));

		Vector3 upDir = (hV.body.lookRot* Vector3.up) * hV.body.Height;
		Vector3 rightDir = (hV.body.lookRot* Vector3.right) * hV.body.Width;
		Vector3 forwardDir = (hV.body.lookRot* Vector3.forward) * hV.body.Length;

		Vector3 brickLengthX = -forwardDir/hV.body.bricksX;
		Vector3 brickHeightY = upDir/hV.body.bricksY;
		Vector3 brickDepthZ = rightDir*hV.body.bricksDepth;

		Vector3 newOffset = (offset + yOffset) - brickDepthZ;// + (hV.body.brickSeperation*Vector3.right) + (hV.body.brickSeperation*Vector3.up);


		BuildQuad(meshbuilder,newOffset,brickLengthX + (hV.body.brickSeperation*Vector3.forward),brickHeightY - hV.body.brickSeperation*Vector3.up);
		BuildQuad(meshbuilder,newOffset,brickHeightY- hV.body.brickSeperation*Vector3.up,brickDepthZ);
		BuildQuad(meshbuilder,newOffset,brickDepthZ,brickLengthX+ (hV.body.brickSeperation*Vector3.forward));

		Vector3 farCorner = newOffset + brickLengthX + brickHeightY + brickDepthZ ;
		farCorner += (hV.body.brickSeperation*Vector3.forward) - hV.body.brickSeperation*Vector3.up;

		BuildQuad(meshbuilder, farCorner,-brickHeightY + ( hV.body.brickSeperation*Vector3.up),-brickLengthX - (hV.body.brickSeperation*Vector3.forward));
		BuildQuad(meshbuilder,farCorner,-brickLengthX- (hV.body.brickSeperation*Vector3.forward),-brickDepthZ);
		BuildQuad(meshbuilder,farCorner,-brickDepthZ,-brickHeightY+ ( hV.body.brickSeperation*Vector3.up));

	}

	protected void BuildBrickWall2(MeshBuilder meshbuilder,
	                              HouseVariables hV,Vector3 offset,Vector3 yOffset)
	{

		Vector3 brickX = hV.bricks.brickSizeX * (hV.body.lookRot*-Vector3.forward);
		Vector3 brickY = hV.bricks.brickSizeY * (hV.body.lookRot*Vector3.up);
		Vector3 brickZ = hV.bricks.brickSizeZ * (hV.body.lookRot* Vector3.right);

		brickX -= (hV.body.brickSeperation* (hV.body.lookRot*-Vector3.forward));
		brickY -= hV.body.brickSeperation*(hV.body.lookRot*Vector3.up);
		//brickZ -= hV.body.brickSeperation*Vector3.right;

		Vector3 newOffset = (offset + yOffset);
		BuildQuad(meshbuilder,newOffset,brickX,brickY);
		BuildQuad(meshbuilder,newOffset,brickY,brickZ);
		BuildQuad(meshbuilder,newOffset,brickZ,brickX);

		Vector3 farCorner = newOffset+brickX +brickY+brickZ;		
		BuildQuad(meshbuilder, farCorner,-brickY,-brickX);
		BuildQuad(meshbuilder, farCorner,-brickX,-brickZ);
		BuildQuad(meshbuilder, farCorner,-brickZ,-brickY);
	
		
	}

	protected void BuildBrickWallSide(MeshBuilder meshbuilder,
	                              HouseVariables hV,Vector3 offset,Vector3 yOffset)
	{
		//hV.body.lookRot = Quaternion.Euler(Random.Range(-hV.tiles.XSpin,hV.tiles.XSpin),
		//                                  Random.Range(-hV.tiles.YSpin,hV.tiles.YSpin),
		//                                Random.Range(-hV.tiles.ZSpin,hV.tiles.ZSpin));

//		Quaternion spin = hV.body.lookRot;

		//spin *= Quaternion.Euler(0,90,0);

	//	Vector3 upDir = (spin* Vector3.up) * hV.body.Height;
		//upDir *= Quaternion.Euler(0,90,0);
	//	Vector3 rightDir = (spin* Vector3.forward) * hV.body.Width;
		//rightDir *= Quaternion.Euler(0,90,0);
//		Vector3 forwardDir = (spin* -Vector3.right) * hV.body.Length;
		
		Vector3 brickLengthX = Vector3.right * hV.bricks.brickSizeX;
		Vector3 brickHeightY = Vector3.up * hV.bricks.brickSizeY;
		Vector3 brickDepthZ = Vector3.forward * hV.bricks.brickSizeZ;
		
		Vector3 newOffset = (offset + yOffset) - brickDepthZ;// + (hV.body.brickSeperation*Vector3.right) + (hV.body.brickSeperation*Vector3.up);
		
		
		BuildQuad(meshbuilder,newOffset,brickLengthX + (hV.body.brickSeperation*Vector3.forward),brickHeightY - hV.body.brickSeperation*Vector3.up);
		BuildQuad(meshbuilder,newOffset,brickHeightY- hV.body.brickSeperation*Vector3.up,brickDepthZ);
		BuildQuad(meshbuilder,newOffset,brickDepthZ,brickLengthX+ (hV.body.brickSeperation*Vector3.forward));
		
		Vector3 farCorner = newOffset + brickLengthX + brickHeightY + brickDepthZ ;
		farCorner += (hV.body.brickSeperation*Vector3.forward) - hV.body.brickSeperation*Vector3.up;
		
		BuildQuad(meshbuilder, farCorner,-brickHeightY + ( hV.body.brickSeperation*Vector3.up),-brickLengthX - (hV.body.brickSeperation*Vector3.forward));
		BuildQuad(meshbuilder,farCorner,-brickLengthX- (hV.body.brickSeperation*Vector3.forward),-brickDepthZ);
		BuildQuad(meshbuilder,farCorner,-brickDepthZ,-brickHeightY+ ( hV.body.brickSeperation*Vector3.up));
		
	}


	//DrawGizmos()
	//{
	//    Gizmos.DrawCube(transform.position, Vector3.one);
	//}
}
