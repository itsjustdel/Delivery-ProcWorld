  //  
  // Author: Kevin Tritz (tritz at yahoo *spamfilter* com)  
  // copyright (c) 2014  
  // license: BSD style  
  // derived from python version: Icosphere.py  
  //  
  //         Author: William G.K. Martin (wgm2111@cu where cu=columbia.edu)  
  //         copyright (c) 2010  
  //         license: BSD style  
  //        https://code.google.com/p/mesh2d-mpl/source/browse/icosphere.py  
  using UnityEngine;  
  using System.Collections;  
  using System.Collections.Generic;  
  public class CreateIco2 : MonoBehaviour  
  {  
	public IcoMesh icoMesh;
    public int num;            // subdivision level of icosphere, max value of 79 to fit within 64k mesh index limit  
                                      // num = 1 is 1 level of subdivision, 80 faces, 42 verticies  
    public Vector3[] vertices;        // Vector3[M] array of verticies, M = 10*(num+1)^2 + 2  
    public int[] triangleIndices;     // int[3*N] flat triangle index list for mesh, N = 20*(num+1)^2  
    int[,] triangles;                // int[N,3] triangle verticies index list, N = 20*(num+1)^2  
    

	void Start()  
      {  
          Icosahedron ico = new Icosahedron();    // initialize base Icosahedron, 20 faces, 12 vertices, radius = 1  
          get_triangulation(num, ico);            // main function to subdivide and triangulate Icosahedron  
		Mesh mesh = new Mesh();                    // mesh initialization and display   
		GetComponent<MeshFilter>().mesh = mesh; // add Mesh Filter component to game object with this script  
		mesh.vertices = vertices;  
		mesh.normals = vertices;  
		mesh.uv = getUV(vertices);                // UV mapping is messed up near poles and longitude boundary  
		// cube-mapping might work better  
		mesh.triangles = triangleIndices;  
		
		//switches on IcoMesh to deform the mesh--this script is already attached to the gameObject
//		icoMesh.enabled = true;
          
      }  





      void get_triangulation(int num, Icosahedron ico)  
      {  
          Dictionary<Vector3, int> vertDict = new Dictionary<Vector3, int>();    // dict lookup to speed up vertex indexing  
          float[,] subdivision = getSubMatrix(num+2);                            // vertex subdivision matrix calculation  
          Vector3 p1, p2, p3;  
          int index = 0;  
          int vertIndex;  
	          int len = subdivision.GetLength(0);  
		          int triNum = (num+1)*(num+1)*20;            // number of triangle faces  
		          vertices = new Vector3[triNum/2 + 2];        // allocate verticies, triangles, etc...  
		          triangleIndices = new int[triNum*3];  
		          triangles = new int[triNum,3];  
		          Vector3[] tempVerts = new Vector3[len];        // temporary structures for subdividing each Icosahedron face  
		          int[] tempIndices = new int[len];  
		          int[,] triIndices = triangulate(num);        // precalculate generic subdivided triangle indices  
		          int triLength = triIndices.GetLength(0);  
		          for(int i=0; i < 20; i++)                    // calculate subdivided vertices and triangles for each face  
		          {  
			              p1 = ico.vertices[ico.triangles[i*3]];    // get 3 original vertex locations for each face  
			              p2 = ico.vertices[ico.triangles[i*3+1]];  
			              p3 = ico.vertices[ico.triangles[i*3+2]];  
			              for(int j=0; j < len; j++)                // calculate new subdivided vertex locations  
			              {  
				                  tempVerts[j].x = subdivision[j,0]*p1.x + subdivision[j,1]*p2.x + subdivision[j,2]*p3.x;  
				                  tempVerts[j].y = subdivision[j,0]*p1.y + subdivision[j,1]*p2.y + subdivision[j,2]*p3.y;  
				                  tempVerts[j].z = subdivision[j,0]*p1.z + subdivision[j,1]*p2.z + subdivision[j,2]*p3.z;  
				                  tempVerts[j].Normalize();  
				                  if(!vertDict.TryGetValue(tempVerts[j], out vertIndex))    // quick lookup to avoid vertex duplication  
				                  {  
					                      vertDict[tempVerts[j]] = index;    // if vertex not in dict, add it to dictionary and final array  
					                      vertIndex = index;  
					                      vertices[index] = tempVerts[j];  
					                      index += 1;  
					                  }  
				                  tempIndices[j] = vertIndex;            // assemble vertex indices for triangle assignment  
				              }  
			              for(int j=0; j < triLength; j++)        // map precalculated subdivided triangle indices to vertex indices  
			              {  
				                  triangles[triLength*i+j,0] = tempIndices[triIndices[j,0]];  
				                  triangles[triLength*i+j,1] = tempIndices[triIndices[j,1]];  
				                  triangles[triLength*i+j,2] = tempIndices[triIndices[j,2]];  
				                  triangleIndices[3*triLength*i + 3*j] = tempIndices[triIndices[j,0]];  
				                  triangleIndices[3*triLength*i + 3*j + 1] = tempIndices[triIndices[j,1]];  
				                  triangleIndices[3*triLength*i + 3*j + 2] = tempIndices[triIndices[j,2]];  
				              }  
			          }  
		     }  
	      int[,] triangulate(int num)    // fuction to precalculate generic triangle indices for subdivided vertices  
	      {  
		          int n = num + 2;  
		          int[,] triangles = new int[(n-1)*(n-1),3];  
		          int shift = 0;  
		          int ind = 0;  
		          for(int row=0; row < n-1; row++)  
		          {  
			              triangles[ind,0] = shift + 1;  
			              triangles[ind,1] = shift + n - row;  
			              triangles[ind,2] = shift;  
			              ind += 1;  
			              for(int col = 1; col < n-1-row; col++)  
			              {  
				                  triangles[ind,0] = shift + col;  
				                  triangles[ind,1] = shift + n - row + col;  
				                  triangles[ind,2] = shift + n - row + col - 1;  
				                  ind += 1;  
				                  triangles[ind,0] = shift + col + 1;  
				                  triangles[ind,1] = shift + n - row + col;  
				                  triangles[ind,2] = shift + col;  
				                  ind += 1;  
				              }  
			              shift += n - row;  
			          }  
		          return triangles;  
		      }  
	      Vector2[] getUV(Vector3[] vertices)    // standard Longitude/Latitude mapping to (0,1)/(0,1)  
	      {  
		          int num = vertices.Length;  
		          float pi = (float)System.Math.PI;  
		          Vector2[] UV = new Vector2[num];  
		          for(int i=0; i < num; i++)  
		          {  
			              UV[i] = cartToLL(vertices[i]);  
			              UV[i].x = (UV[i].x + pi)/(2.0f*pi);  
			              UV[i].y = (UV[i].y + pi/2.0f)/pi;  
			          }  
		          return UV;  
		      }  
	      Vector2 cartToLL(Vector3 point)    // transform 3D cartesion coordinates to longitude, latitude  
	      {  
		          Vector2 coord = new Vector2();  
		          float norm = point.magnitude;  
		          if(point.x != 0.0f || point.y != 0.0f)  
		              coord.x = -(float)System.Math.Atan2(point.y, point.x);  
		          else  
		             coord.x = 0.0f;  
		          if(norm > 0.0f)  
		              coord.y = (float)System.Math.Asin(point.z / norm);  
		          else  
		              coord.y = 0.0f;  
		          return coord;  
		      }  
	      float[,] getSubMatrix(int num)    // vertex subdivision matrix, num=3 subdivides 1 triangle into 4  
	      {  
		          int numrows = num * (num + 1) / 2;  
		          float[,] subdivision = new float[numrows,3];  
		          float[] values = new float[num];  
		          int[] offsets = new int[num];  
		         int[] starts = new int[num];  
		          int[] stops = new int[num];  
		          int index;  
		          for(int i=0;i < num; i++)  
		          {  
			              values[i] = (float)i / (float)(num - 1);  
			              offsets[i] = (num - i);  
			              if(i > 0)  
				                  starts[i] = starts[i-1] + offsets[i-1];  
			              else  
				                  starts[i] = 0;  
			              stops[i] = starts[i] + offsets[i];              
			          }      
		          for(int i=0; i < num; i++)  
		          {  
			              for(int j=0; j < offsets[i]; j++)  
			              {  
				                  index = starts[i] + j;  
				                  subdivision[index,0] = values[offsets[i]-1-j];  
				                  subdivision[index,1] = values[j];  
				                  subdivision[index,2] = values[i];  
				              }  
			          }  
		          return subdivision;  
	      }  
	  }  
  public class Icosahedron  
  {  
	      public Vector3[] vertices;  
	      public int[] triangles;  
     public Icosahedron()  
      {  
          vertices = getPoints();  
          triangles = getTriangles();      
      }  
     Vector3[] getPoints()  
      {  
          Vector3[] vertices = new Vector3[12];  
         // Define the verticies with the golden ratio  
		          float a = (1.0f + (float)System.Math.Sqrt(5))/2.0f;   
		         vertices[0] = new Vector3(a,        0.0f,    1.0f);  
	         vertices[9] = new Vector3(-a,        0.0f,    1.0f);  
		          vertices[11] = new Vector3(-a,        0.0f,    -1.0f);  
		          vertices[1] = new Vector3(a,        0.0f,    -1.0f);  
		          vertices[2] = new Vector3(1.0f,        a,        0.0f);  
		          vertices[5] = new Vector3(1.0f,        -a,        0.0f);  
		          vertices[10] = new Vector3(-1.0f,    -a,        0.0f);  
		          vertices[8] = new Vector3(-1.0f,    a,        0.0f);  
		          vertices[3] = new Vector3(0.0f,        1.0f,    a);  
		          vertices[7] = new Vector3(0.0f,        1.0f,    -a);  
		          vertices[6] = new Vector3(0.0f,        -1.0f,    -a);  
		          vertices[4] = new Vector3(0.0f,        -1.0f,    a);  
		          for(int i = 0; i < 12; i++)  
		          {  
		              vertices[i].Normalize();  
		          }  
		          // rotate top point to the z-axis  
		          float angle = (float)System.Math.Atan(vertices[0].x / vertices[0].z);  
		          float ca = (float)System.Math.Cos (angle);  
		          float sa = (float)System.Math.Sin (angle);  
		          Matrix4x4 rotation = Matrix4x4.identity;  
		          rotation.m00 = ca;  
		          rotation.m02 = -sa;  
		          rotation.m20 = sa;  
		          rotation.m22 = ca;  
		          for(int i = 0; i < 12; i++)  
		          {  
		              vertices[i] = rotation.MultiplyPoint3x4(vertices[i]);  
		          }  
		          return vertices;          
		      }  
		      int[] getTriangles()  
		      {  
		          int[] tris = {  
		              1,2,0,  
		              2,3,0,  
		              3,4,0,  
		              4,5,0,  
		              5,1,0,  
		              6,7,1,  
		              2,1,7,  
		              7,8,2,  
		              2,8,3,  
		              8,9,3,  
		              3,9,4,  
		              9,10,4,  
		              10,5,4,  
		              10,6,5,  
		              6,1,5,  
		              6,11,7,  
		              7,11,8,  
		              8,11,9,  
		              9,11,10,  
		              10,11,6,              
		          };  
		          return tris;  
		      }  
  }  