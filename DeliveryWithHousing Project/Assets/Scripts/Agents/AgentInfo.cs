using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentInfo : MonoBehaviour {
	//contains information for agents homes// needs to be class, structs are not serialisable

	public GameObject agent;
	public GameObject home;
    public GameObject car;

    public int opinionOfPlayer = 0;

    public bool haveOwnCar;

    public bool fetchQuestTarget;
    public bool fetchQuestOwner;    

    public bool space1taken;
    public bool space2taken;
    public bool space3taken;
}


