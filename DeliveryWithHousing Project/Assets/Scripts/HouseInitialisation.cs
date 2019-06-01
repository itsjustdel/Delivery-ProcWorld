using UnityEngine;
using System.Collections;

public class HouseInitialisation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		MeshControl mC =  this.gameObject.AddComponent<MeshControl>();
		mC.enabled = false;

		MeshControlTiles mCt =  this.gameObject.AddComponent<MeshControlTiles>();
		mCt.enabled = false;
		this.gameObject.AddComponent<CombineChildren>();
	}

}
