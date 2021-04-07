using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All Codes worked well on 2020-07-06

public class ctw_Platform_behavior : MonoBehaviour
{
	
	public bool Trigger = false;
	
	int DownCool = 0;
	
	Transform PlatformTransform;
	BoxCollider2D PlatformCollider;
	
	Transform PlayerTransform;
	
    void Start(){
		
        PlatformTransform = GetComponent<Transform>();
		PlatformCollider = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
		PlatformCollider.offset = new Vector2(0,-2f);
		PlatformCollider.size = new Vector2(1,4);
		
		PlayerTransform = GameObject.Find("BS_Player").GetComponent<Transform>();
    }
	
	void Control(){
		
		PlayerController CallScript = GameObject.Find("BS_Player").GetComponent<PlayerController>();
		
		if ((PlayerTransform.position.y >= PlatformTransform.position.y + 0.49f)&&(DownCool == 0)){
			PlatformCollider.isTrigger = false;
			
			if ((CallScript.DOWN == true)&&(PlayerTransform.position.y - PlatformTransform.position.y <= 2f)){
				PlatformCollider.isTrigger = true;
				DownCool = 1;
				Invoke("Cooler",0.5f);
			}
		}
		if ((PlayerTransform.position.y < PlatformTransform.position.y -1f)&&(DownCool == 0)){
			PlatformCollider.isTrigger = true;
		}
	}
	
	void Cooler(){
		DownCool = 0;
	}
	
    void Update(){
		
		Control();
		
		Trigger = PlatformCollider.isTrigger;
    }
}
