using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DualGraph2d;

public class Generator : MonoBehaviour {

	public Vector3 volume= new Vector3(20.0f,0.0f,20.0f);
	public float rootTriMultiplier=1.0f;
	public int cellNumber= 20;
	public bool drawDeluany=false;
	public bool drawRoots=false;
	public bool drawVoronoi=false;
	public bool drawGhostEdges=false;
	public bool drawPartialGhostEdge=false;
	public bool drawCircumspheres=false;

	public bool useSortedGeneration=true;


	public Color sphereColor= Color.cyan;

	private DualGraph dualGraph;
	private float totalTime;
	private float computeTime;

	void Start () {
		totalTime= Time.realtimeSinceStartup;
		dualGraph = new DualGraph(volume);

		if (cellNumber<1)
			cellNumber=1;

		Vector3[] points= new Vector3[cellNumber];

		if(useSortedGeneration)
			GenSortedRandCells(ref points);
		else
			GenRandCells(ref points);
			

		dualGraph.DefineCells(points, rootTriMultiplier);
		computeTime= Time.realtimeSinceStartup;

		if(useSortedGeneration)
			dualGraph.ComputeForAllSortedCells();
		else
			dualGraph.ComputeForAllCells();

		dualGraph.PrepareCellsForMesh();

		computeTime= Time.realtimeSinceStartup- computeTime;
		totalTime= Time.realtimeSinceStartup-totalTime;
		Debug.Log("generation time: "+ (totalTime-computeTime)+"; compute time: "+computeTime+"; total time: "+ totalTime);
		Debug.Log("Cells:"+dualGraph.cells.Count+"; Spheres: "+dualGraph.spheres.Count+"; volume: "+DualGraph.volume);
	}

	/// <summary>
	/// Generates random cells.
	/// </summary>
	/// <param name="p">P.</param>
	private void GenRandCells(ref Vector3[] p){		
		for(int i=0; i<p.Length; i++){
			p[i]= new Vector3(Random.Range(-volume.x,volume.x),0.0f,Random.Range(-volume.z,volume.z));
		}
	}
	/// <summary>
	/// Generates random cells, sorted by x value.
	/// </summary>
	/// <param name="points">Points.</param>
	//Note about sorting: using a sorted list requires the x values to always be different
	private void GenSortedRandCells(ref Vector3[] points){
		SortedList<float, Vector3> p= new SortedList<float,Vector3>();

		for(int i=0; i<cellNumber; i++){
			Vector3 v = new Vector3(Random.Range(-volume.x,volume.x),0.0f,Random.Range(-volume.z,volume.z));
	//		if (i < curvePoints.Count){
	//		//Add curve points in here
	//			foreach (Vector3 v3 in curvePoints){	//Del's snippet
	//				p.Add(v3.x,v3); 
	//			}
	//		}
	//		else 
			try{
			p.Add(v.x, v);
			}
			catch(System.ArgumentException){
				i--;
				Debug.Log("sort conflict");
			}
		}
		p.Values.CopyTo(points,0);
	}


	void OnDrawGizmos(){
		if(dualGraph!=null){
			foreach(Cell c in dualGraph.cells){
				if(c.root){
					Gizmos.color=Color.red;
				}
				else{
					Gizmos.color= Color.blue;
				}
				Gizmos.DrawCube(c.point,Vector3.one*0.2f);
			}

			if (drawDeluany){
				foreach(Cell c in dualGraph.cells){
					foreach(VoronoiEdge e in c.edges){
						if (e.cellPair.root || c.root){
							if (drawRoots){
								Gizmos.color= Color.gray;
								Gizmos.DrawLine(c.point, e.cellPair.point);
							}
						}
						else{
							Gizmos.color= Color.green;
							Gizmos.DrawLine(c.point, e.cellPair.point);
						}
					}
				}
			}
			if (drawVoronoi){
				foreach(Cell c in dualGraph.cells){
					foreach (VoronoiEdge e in c.edges){
						if(e.isConnected){
							Gizmos.color=Color.black;
							if(e.ghostStatus==Ghosting.none){
								Gizmos.DrawLine(e.Sphere.Circumcenter, e.SpherePair.Circumcenter);
							}
							else if(drawGhostEdges){
								Gizmos.DrawLine(e.Sphere.Circumcenter, e.SpherePair.Circumcenter);
							}
							else if(drawPartialGhostEdge&& e.ghostStatus== Ghosting.partial){
								Gizmos.DrawLine(e.Sphere.Circumcenter, e.SpherePair.Circumcenter);
							}
						}
					}
				}
			}
			if (drawCircumspheres){
				Gizmos.color= sphereColor;
				foreach(Circumcircle c in dualGraph.spheres){
					Gizmos.DrawSphere(c.Circumcenter, c.circumradius);
				}
			}

		}
	}
	public void GenerateMesh(){

	}
}
