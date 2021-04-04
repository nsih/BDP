using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctw_Bullet_behavior : MonoBehaviour
{
	Transform BulletTransform;
	Rigidbody2D BulletRigid2D;
	SpriteRenderer BulletSprite;
	
	ctw_Player_behavior PlayerScript;
	
	public Sprite spriteBullet;
	public Sprite spriteEffect;
	
	public bool OnWork = true;
	public Vector3 Vel = new Vector3(0,0,0);
	public float Timer = 0f;
	public float Roll = 0f;
	
	public bool Pop;
	
	public float Alpha = 1f;
	
	ctw_Eraser_behavior Eraser1;
	ctw_Eraser_behavior Eraser2;
	
	void Start(){
		
		Pop = false;
		
		BulletTransform = GetComponent<Transform>();
		BulletRigid2D = GetComponent<Rigidbody2D>();
		BulletSprite = GetComponent<SpriteRenderer>();
		
		PlayerScript = GameObject.Find("BS_Player").GetComponent<ctw_Player_behavior>();
		
		Eraser1 = GameObject.Find("BS_Eraser_Player").GetComponent<ctw_Eraser_behavior>();
		Eraser2 = GameObject.Find("BS_Eraser_Boss").GetComponent<ctw_Eraser_behavior>();
		
		BulletTransform.localScale = new Vector2(1f/3f , 0.5f/3f);
    }
	
	float Math_Force(Vector3 Velocity){
		
		return Mathf.Sqrt(Mathf.Pow(Velocity.x,2)+Mathf.Pow(Velocity.y,2));
	}
	
	Vector3 Get_Vector3_Direction(float angle){
		
		Vector3 Pos = new Vector3(Mathf.Cos(angle*Mathf.Deg2Rad), Mathf.Sin(angle*Mathf.Deg2Rad), 0); 
		
		Vector3 VectorDirection = new Vector3(Pos.x,Pos.y,0);
		
		return VectorDirection;
	}
	
	void StrikeWall(){
		
		Alpha = 1f;
		OnWork = false;
		Pop = false;
	}
	
	void WallChecking(){
		if ( (Mathf.Abs(BulletTransform.position.x)>=42)||(Mathf.Abs(BulletTransform.position.y)>=23) ){
			StrikeWall();
		}
	}
	
	void Timing(){
		if (Timer > 0){
			Timer -= 1;
		}
		else{
			Timer = 0;
		}
	}
	
	void Rendering(){
		
		if ( (Eraser1.Alpha != 0f)||(Eraser2.Alpha != 0f) ) OnWork = false;
		
		BulletRigid2D.rotation = BulletRigid2D.rotation + Roll;
		
		float Force = Math_Force(Vel);
		float Angle = BulletRigid2D.rotation;
		
		float R = BulletSprite.color.r;
		float G = BulletSprite.color.g;
		float B = BulletSprite.color.b;
		
		switch(OnWork) {
			
			case true:
				if (Alpha < 1f)
					Alpha += 0.02f;
				else
					Alpha = 1f;
				BulletSprite.sprite = spriteBullet;
				BulletTransform.localScale = new Vector2(1f/3f , 0.5f/3f);
				BulletSprite.color = new Color(R, G, B, Alpha);
				
				if (Timer == 0){
					
					BulletRigid2D.velocity = Get_Vector3_Direction(Angle) * Force;
				}
			break;
			
			case false:
				BulletRigid2D.velocity = new Vector3(0,0,0);

				if (Pop == true){
					
					if (Alpha > 0f){
						Alpha -= 0.05f;
					}
					
					if (Alpha <= 0f){
						Alpha = 0f;
					}
					
					BulletSprite.sprite = spriteEffect;
					BulletSprite.color = new Color(R, G, B, Alpha);
					BulletTransform.localScale = new Vector2( (0.5f - 0.5f*Alpha), (0.5f - 0.5f*Alpha) );
				}
				
				else{
					
					BulletSprite.color = new Color(R, G, B, 0f);
					BulletTransform.localScale = new Vector2( (0.5f - 0.5f*Alpha), (0.5f - 0.5f*Alpha) );
				}
			break;
		}
	}
	
    void Update(){
		
		Timing();
        Rendering();
		WallChecking();
    }
}
