using BS.Enemy.Boss;
using UnityEngine;

public class ctw_Eraser_behavior : MonoBehaviour
{
	public float Alpha = 0f;
	
	SpriteRenderer SelfSprite;
	Transform SelfTransform;
	CircleCollider2D SelfCollider;
	
	Transform PlayerTransform;
	Transform BossTransform;
	
	BossBehavior BossScript;
	
    void Start(){
		
        SelfSprite = GetComponent<SpriteRenderer>();
		SelfTransform = GetComponent<Transform>();
		SelfCollider = GetComponent<CircleCollider2D>() as CircleCollider2D;
		
		PlayerTransform = GameObject.Find("BS_Player").GetComponent<Transform>();
		BossTransform = GameObject.Find("BS_Boss").GetComponent<Transform>();
		
		BossScript = GameObject.Find("BS_Boss").GetComponent<BossBehavior>();
    }

	
    void Update(){
		
		if (SelfTransform.name == "BS_Eraser_Player"){
			
			if (Alpha > 0f){
				Alpha -= 0.05f;
			}
		
			if (Alpha <= 0f){
				Alpha = 0f;
			}
			
			SelfTransform.position = PlayerTransform.position;
			SelfSprite.color = new Color(1,1,1,Alpha);
			SelfTransform.localScale = new Vector2( (50f - 50f*Alpha) , (50f - 50f*Alpha) );
			SelfCollider.radius = 2.34f*(50f - 50f*Alpha);
		}
		
		else {
			
			if (Alpha > 0f){
				Alpha -= 0.005f;
				if (!BossScript.DEAD) Alpha -= 0.015f;
			}
		
			if (Alpha <= 0f){
				Alpha = 0f;
			}
			
			SelfTransform.position = BossTransform.position;
			SelfSprite.color = new Color(1,0.8f,0.8f,Alpha);
			SelfTransform.localScale = new Vector2( (50f - 50f*Alpha) , (50f - 50f*Alpha) );
			SelfCollider.radius = 2.34f*(50f - 50f*Alpha);
		}
    }
}
