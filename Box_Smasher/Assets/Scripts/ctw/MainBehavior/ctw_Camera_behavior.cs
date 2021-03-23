using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctw_Camera_behavior : MonoBehaviour
{
	public float CamShake;
	
	ctw_Boss_behavior BossScript;
	
	Transform SelfTransform;
	
    void Start(){
        
		CamShake = 0f;
		
		BossScript = GameObject.Find("ctw_Boss").GetComponent<ctw_Boss_behavior>();
		
		SelfTransform = GetComponent<Transform>();
    }
	
	Vector3 pick(){
		return new Vector3(Random.Range(-CamShake-0.01f,CamShake+0.01f), Random.Range(-CamShake-0.01f,CamShake+0.01f), -10f);
	}
	
	void Supressing(){
		
		if (CamShake > 0.1f){
			
			CamShake -= 0.005f;
			if (BossScript.DEAD != 1) CamShake -= 0.015f;
		}
		else CamShake = 0f;
	}
	
	void Shaking(){
		
		if (CamShake != 0f) SelfTransform.position = pick();
		else SelfTransform.position = new Vector3(0,0,-10f);
	}
	
    void Update(){
		
		Supressing();
        Shaking();
    }
}
