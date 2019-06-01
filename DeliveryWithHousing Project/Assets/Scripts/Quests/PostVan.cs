using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PostVan : MonoBehaviour {
    
	public List<GameObject> mailList = new List<GameObject>(); 
	public Material red;
    public GUIInfo guiInfo;
    
	void Start()
	{
		red = Resources.Load("Green", typeof (Material)) as Material;
        guiInfo = transform.parent.Find("GUI").GetComponent<GUIInfo>();
    }
	public void checkForPost(GameObject building)
	{
		if(mailList.Contains(building))
		{
			Debug.Log("removing mail item from list");
			mailList.Remove(building);
			building.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Green", typeof(Material)) as Material;            
        }
	}

    void Update()
    {

    }

}
