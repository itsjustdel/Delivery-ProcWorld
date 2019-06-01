using UnityEngine;
using System.Collections;

public class TriggerEnable : MonoBehaviour {
    /// <summary>
    /// Waits for a frame before enabling the trigger used to find agents
    /// Seems to have a problem instantiating the agents, with triggers enabled and using them straight off
    /// </summary>



	// Use this for initialization
	void Start () {
        StartCoroutine("Enable");
	}



    IEnumerator Enable()
    {
        yield return new WaitForEndOfFrame();
        transform.Find("Trigger").gameObject.SetActive(true);
        transform.Find("LargeTrigger").gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
    //    transform.FindChild("Trigger").gameObject.SetActive(false);

        yield break;
    }
	
	
}
