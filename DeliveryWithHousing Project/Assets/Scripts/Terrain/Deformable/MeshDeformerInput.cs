using UnityEngine;

public class MeshDeformerInput : MonoBehaviour {
	
	public float force = 10f;
	public float forceOffset = 0.1f;
    public bool mouse;
    public bool van;
    void Update () {
        if (mouse)
        {
            if (Input.GetMouseButton(0))
            {
                HandleInput();
            }
        }
        if(van)
        {
            VanInput();
        }
	}
    void VanInput()
    {
        //Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(transform.position,Vector3.down, out hit,2f))
        {
            MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
            if (deformer)
            {
                Vector3 point = hit.point;
                point += hit.normal * forceOffset;
                deformer.AddDeformingForce(point, force);
            }
        }
    }

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if (Physics.Raycast(inputRay, out hit)) {
			MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
			if (deformer) {
				Vector3 point = hit.point;
				point += hit.normal * forceOffset;
				deformer.AddDeformingForce(point, force);
			}
		}
	}
}