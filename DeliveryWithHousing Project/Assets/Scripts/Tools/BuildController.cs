using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildController : MonoBehaviour {

	public List<GameObject> gOList = new List<GameObject>();

	//private GameObject van;

	// Use this for initialization
	void Start () {
	
		StartCoroutine("Enable");

	//	van = GameObject.FindGameObjectWithTag("Player");
	}
	
	IEnumerator Enable()
	{
		for (int i = 0; i < gOList.Count; i++)
		{
			if(gOList[i].GetComponent<CellYAdjust>() != null)
			{
				gOList[i].GetComponent<CellYAdjust>().enabled = true;
				yield return new WaitForEndOfFrame();
			}
		}
	}
}
