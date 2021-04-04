using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctw_Effects_OnCam : MonoBehaviour
{
	public Vector3 SelfPos;
	public float Zpos;
	
	Transform CameraTransform;
	Transform SelfTransform;
	
    void Start(){
		
        CameraTransform = GameObject.Find("BS_Main Camera").GetComponent<Transform>();
		SelfTransform = GetComponent<Transform>();
    }

	void FollowCamera(){
		
		Vector3 CamPos = CameraTransform.position;
		
		Vector3 Pos = CamPos + SelfPos;
		Pos = new Vector3(Pos.x, Pos.y, Zpos);
		
		SelfTransform.position = Pos;
	}

    void Update(){
		
        FollowCamera();
    }
}
