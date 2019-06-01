using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo {
	public WheelCollider leftWheel;
	public WheelCollider rightWheel;
	public bool motor;
	public bool steering;
	public bool brake;
}
[System.Serializable]
public class GearTorques{
	
	public float firstGear;
	public float secondGear;
}

public class SimpleCarController : MonoBehaviour {
	
	
	public List<AxleInfo> axleInfos; 
	public GearTorques gearTorques;
	public float totalMotorTorque;
	public float maxBrakeTorque;
	public float maxSteeringAngle;
	public float currentGear;
	public AnimationCurve torqueCurve = new AnimationCurve();
	public AnimationCurve gearCurve = new AnimationCurve();
	public float finalDriveRatio = 3.5f;
	public float motorRPM;
	public float minRPM = 700f;
	public float revs;
	public float wheelRPM;
	public float torque;
	public float clutchAmt = 2f;
	public float clutchSpeed = 0.5f;
	public GameObject centerOfMassObject;
	public float powerAppliedToWheels;
	public float clutch;
	public float fwdFriction;
	public float engineInertia;
	// finds the corresponding visual wheel
	// correctly applies the transform
	public void ApplyLocalPositionToVisuals(WheelCollider collider)
	{
		if (collider.transform.childCount == 0) {
			return;
		}
		
		Transform visualWheel = collider.transform.GetChild(0);
		
		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose(out position, out rotation);
		
		visualWheel.transform.position = position;
		visualWheel.transform.rotation = rotation * Quaternion.Euler(0,0,90);
	}
	
	public void Gears ()
	{
		//bool gearUp = Input.GetButtonDown("A");
		//bool gearDown = Input.GetButtonDown("X");
		bool gearUp = Input.GetKeyDown("r");
		bool gearDown = Input.GetKeyDown("f");
		
		if (gearUp && currentGear <=3 ){
			currentGear ++;
		//	Debug.Log(currentGear);
		}
		
		if (gearDown && currentGear >=0){
			currentGear--;
		//	Debug.Log(currentGear);
		}
	}
	
	public void Start()
	{
		currentGear = 1;
	}
	
	public void Update()
	{
		Gears();
		
		//Debug.Log(GetComponent<Rigidbody>().velocity.magnitude);
	}
	
	public void FixedUpdate()
	{
		
		//float brake = maxBrakeTorque * Input.GetAxis ("LTrigger");
		//float maxRPM = 4500;
	//	float accel =  Input.GetAxis("RTrigger");
		//float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

		float accel = 0;
			if(Input.GetKey("w"))
				accel  = 1;

		float brake = 0;
		if(Input.GetKey("s"))
			brake = maxBrakeTorque * 1;

		float steering = 0;
		if(Input.GetKey("a"))
			steering = maxSteeringAngle * -.5f;
		if(Input.GetKey("d"))
			steering = maxSteeringAngle * .5f;

		float targetMinRPM;
		
		
		clutch = Input.GetAxis("Vertical2");
		//Debug.Log(brake);
		
		foreach (AxleInfo axleInfo in axleInfos) {
			
			
			if (axleInfo.motor){
				wheelRPM = (axleInfo.leftWheel.rpm);// + axleInfo.rightWheel.rpm) / 2;					
			}
			if (!axleInfo.motor){
				//Debug.Log(axleInfo.leftWheel.rpm-wheelRPM);//wheel slip//maybe
			}
			
		}
		
		
		if ( Input.GetAxis ("LTrigger") == 1){ //brake
			
			targetMinRPM = accel * 4500;
			
		}else
		{ 
			targetMinRPM = 0;
		}
		
		
		
		minRPM = Mathf.Lerp(minRPM,targetMinRPM,clutchSpeed);
		minRPM = Mathf.Clamp(minRPM,700,4500);
		
		float targetMotorRPM = minRPM + (wheelRPM * finalDriveRatio * gearCurve.Evaluate(currentGear));	
		motorRPM = Mathf.Lerp(motorRPM,targetMotorRPM,engineInertia);
		
		motorRPM = Mathf.Clamp(motorRPM,700,4500);
		totalMotorTorque = torqueCurve.Evaluate(motorRPM) * gearCurve.Evaluate(currentGear) * finalDriveRatio * accel * (1 - clutch);
		
		//if (clutch != 0){
		powerAppliedToWheels = totalMotorTorque;
		//}else if(clutch==0){
		//powerAppliedToWheels = 0;
		//}
		
		
		
		foreach (AxleInfo axleInfo in axleInfos) {
			if (axleInfo.steering) {
				axleInfo.leftWheel.steerAngle = steering;
				axleInfo.rightWheel.steerAngle = steering;
			}
			if (axleInfo.motor ) {
				WheelHit hit;
				WheelCollider wheel = axleInfo.leftWheel;
				if( wheel.GetGroundHit( out hit ) ) {
					fwdFriction = hit.forwardSlip;
				}
				axleInfo.leftWheel.motorTorque = powerAppliedToWheels/2;
				axleInfo.rightWheel.motorTorque = powerAppliedToWheels/2;
				
			}else{
				
			}
			if (axleInfo.brake) {
				axleInfo.leftWheel.brakeTorque = brake;
				axleInfo.rightWheel.brakeTorque = brake;
				//Debug.Log("braking");
			}
			
			ApplyLocalPositionToVisuals(axleInfo.leftWheel);
			ApplyLocalPositionToVisuals(axleInfo.rightWheel);
		}
		
		GetComponent<Rigidbody>().centerOfMass = centerOfMassObject.transform.localPosition;
	}
}