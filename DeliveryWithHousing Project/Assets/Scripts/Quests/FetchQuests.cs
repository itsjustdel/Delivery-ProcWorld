using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FetchQuests : MonoBehaviour {

    //holds what fetch quest are active

    public List<FetchQuest> fetchQuestsList = new List<FetchQuest>();
    
    // Use this for initialization
	void Start () {
	
	}

    public class FetchQuest
    {
        public GameObject owner;
        public GameObject target;
        public GameObject item;
        public bool playerHasItem = false;

        public GameObject minimapBlipOwner;
        public GameObject minimapBlipTarget;
    }
	
}
