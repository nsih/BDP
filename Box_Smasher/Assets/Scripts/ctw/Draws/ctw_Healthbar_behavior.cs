using UnityEngine;
using BS.Enemy.Boss;
using UnityEngine.UI;

public class ctw_Healthbar_behavior : MonoBehaviour
{
	
	Image SelfImage;
	
	BossBehavior Boss;
	ctw_Eraser_behavior Eraser;
	
    void Start(){
		
		SelfImage = GetComponent<Image>();
		
		Boss = GameObject.Find("ctw_Boss").GetComponent<BossBehavior>();
		Eraser = GameObject.Find("ctw_Eraser_Boss").GetComponent<ctw_Eraser_behavior>();
    }
	
    void Update(){
		
		float CurrentHP = ( Boss.HP / Boss.MaxHP );
		float LastHP = ( Boss.LastHP / Boss.MaxHP );
		
		switch (gameObject.name){
			
			case "ctw_HealthBar_Red":
				SelfImage.fillAmount = CurrentHP;
			break;
			
			case "ctw_HealthBar_Yellow":
				SelfImage.fillAmount = LastHP;
				if (Boss.LastHP > Boss.HP) Boss.LastHP -= (Boss.LastHP - Boss.HP)/50 + 2;
				if (Boss.LastHP <= Boss.HP+25) Boss.LastHP = Boss.HP;
			break;
		}
    }
}
