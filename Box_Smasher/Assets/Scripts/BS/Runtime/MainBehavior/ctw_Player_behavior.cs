using System.Collections;
using System.Collections.Generic;
using BS.Enemy.Boss;
using BS.Manager.Cameras;
using UnityEngine;

public class ctw_Player_behavior : MonoBehaviour
{
	Transform PlayerTransform;
	Rigidbody2D PlayerRigid2D;
	Collider2D PlayerCollider;
	SpriteRenderer PlayerSprite;
	
	ctw_Eraser_behavior Eraser;
	
	public PhysicsMaterial2D Normal;
	public PhysicsMaterial2D Bouncy;
	
	Camera MainCamera;
	
	public bool DOWN = false;
	public int OnAttack = 0;
	public int HP = 3;
	public int Invincible = 0;
	public int DEAD = 0;
	
	int OnAir = 0;
	float AlphaInvincible = 0;
	
	ctw_Effector_behavior Effect;
	
    void Start(){
		
        PlayerTransform = GetComponent<Transform>();
		PlayerRigid2D = GetComponent<Rigidbody2D>();
		PlayerCollider = GetComponent<PolygonCollider2D>() as Collider2D;
		PlayerSprite = GetComponent<SpriteRenderer>();
		
		Eraser = GameObject.Find("BS_Eraser_Player").GetComponent<ctw_Eraser_behavior>();
		
		MainCamera = GameObject.Find("BS_Main Camera").GetComponent<Camera>();
		
		Effect = GameObject.Find("BS_Effector").GetComponent<ctw_Effector_behavior>();
    }
	
	
	
	// Maths
	
	float Math_2D_Force(float x, float y){
		
		return Mathf.Sqrt(Mathf.Pow(x,2)+Mathf.Pow(y,2));
	}
	
	float Math_Boss_Damage(){
		
		return (500f + Mathf.Abs((PlayerRigid2D.angularVelocity * Math_2D_Force(PlayerRigid2D.velocity.x, PlayerRigid2D.velocity.y) / 50f)) );
	}
	
	
	
	// Timers
	
	void TimerAttackReset(){
		
		OnAttack = 0;
	}
	
	void TimerInvincibleReset(){
		
		Invincible = 0;
	}
	
	
	
	// Gets
	
	Vector2 GetForceDirection(){
		
		Vector3 PlayerPos = PlayerTransform.position;
		Vector3 MouseStaticPos = new Vector3(MainCamera.ScreenToWorldPoint(Input.mousePosition).x,MainCamera.ScreenToWorldPoint(Input.mousePosition).y,0);
		Vector3 MousePrivatePos = MouseStaticPos - PlayerPos;
		
		float RangeKey = Math_2D_Force(MousePrivatePos.x,MousePrivatePos.y);
		
		MousePrivatePos = MousePrivatePos/RangeKey;
		
		Vector2 ForceDirection = new Vector2(MousePrivatePos.x,MousePrivatePos.y);
		
		return ForceDirection;
	}
	
	float Get_Angle_byPosition(Vector3 Target, Vector3 Pos){
		
		return ( Mathf.Atan2(Target.y-Pos.y, Target.x-Pos.x) * Mathf.Rad2Deg );
	}
	
	Vector2 Get_Force_byAngle(float angle){
		
		return new Vector2( Mathf.Cos(angle*Mathf.Deg2Rad), Mathf.Sin(angle*Mathf.Deg2Rad) ); 
	}
	
	// Checks
	
	void OnDamage(){
		
		if (HP > 1){
			HP -= 1;
			AlphaInvincible = 0f;
			Invincible = 1;
			Eraser.Alpha = 1f;
			Invoke("TimerInvincibleReset",3.0f);
			CameraManager.Instance.ShakeCamera(0.5f, 0.5f);
		}
		else if (HP == 1){
			HP = 0;
			DEAD = 1;
			CameraManager.Instance.ShakeCamera(1f, 1f);
		}
	}
	
	void OnInvincible(){
		if (Invincible == 1)
			AlphaInvincible += 0.1f;
		else if (DEAD == 1)
			AlphaInvincible = 1.2f;
		else
			AlphaInvincible = 0f;
	}
	
	
	// Effects
	
	void GenEffect(float angle, float F, float time, int num){
		
		Vector3 pos = PlayerTransform.position;
		Effect.Effect_Run(time, pos, Get_Force_byAngle(angle)*F, num);
	}
	
	
	
	// Inputs
	
	void InputMove(){
		
		float XMoveCount = 0;
		
		if (OnAttack == 0){
			
			if (Input.GetKey(KeyCode.A))
				XMoveCount--;
			if (Input.GetKey(KeyCode.D))
				XMoveCount++;
			
			if (Mathf.Abs(PlayerRigid2D.velocity.x) < 15)
			{
				PlayerRigid2D.velocity = new Vector2(PlayerRigid2D.velocity.x+1*XMoveCount,PlayerRigid2D.velocity.y);
			}
			
			/// 플레이어의 각속도가 0에 가까울때만 쉐이더가 적용되도록함
			if(Mathf.Abs(PlayerRigid2D.angularVelocity) <= 0.01f)
			{
				this.transform.localRotation = Quaternion.Euler(0, 0, 0);
				GetComponent<Renderer>().material.SetVector("_Direction", PlayerRigid2D.velocity * 0.05f);
			}
			else if(Mathf.Abs(PlayerRigid2D.angularVelocity) > 0.01f)
			{
				GetComponent<Renderer>().material.SetVector("_Direction", Vector4.zero);
			}
			
			if (Input.GetKey(KeyCode.S)){
				DOWN = true;
			}
			else{
				DOWN = false;
			}
			
			if ((Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.Space))&&(OnAir == 0)){
				PlayerRigid2D.velocity = new Vector2(PlayerRigid2D.velocity.x,40);
				GenEffect(0f, 30f, 1f, 4);
				GenEffect(180f, 30f, 1f, 4);
				OnAir = 1;
			}
		}
	}
	
	void InputAttack(){
		
		if ((Input.GetMouseButton(0))&&(OnAir == 1)&&(OnAttack != 2)){
			if ((PlayerRigid2D.angularVelocity <= 2000)&&(MainCamera.ScreenToWorldPoint(Input.mousePosition).x - PlayerTransform.position.x <= 0)){
				PlayerRigid2D.angularVelocity = PlayerRigid2D.angularVelocity + 30;
			}
			if ((PlayerRigid2D.angularVelocity >= -2000)&&(MainCamera.ScreenToWorldPoint(Input.mousePosition).x - PlayerTransform.position.x > 0)){
				PlayerRigid2D.angularVelocity = PlayerRigid2D.angularVelocity - 30;
			}
			if (Mathf.Abs(PlayerRigid2D.angularVelocity) > 2000){
				PlayerRigid2D.angularVelocity = PlayerRigid2D.angularVelocity/Mathf.Abs(PlayerRigid2D.angularVelocity)*2000;
			}
			OnAttack = 1;
		}
		else if ((Input.GetMouseButtonUp(0))&&(OnAttack == 1)){
			PlayerRigid2D.velocity = GetForceDirection()*Mathf.Abs(PlayerRigid2D.angularVelocity)/40;
			OnAttack = 2;
			PlayerCollider.sharedMaterial = Bouncy;
		}
		
		if (OnAttack == 1){
			PlayerRigid2D.angularDrag = 0.1f;
			PlayerRigid2D.drag = 2.5f;
			PlayerRigid2D.gravityScale = 0.5f;
		}
		
		else{
			PlayerRigid2D.angularDrag = 0.2f;
			PlayerRigid2D.drag = 0.2f;
			PlayerRigid2D.gravityScale = 9.8f;
		}
		
		if (OnAttack != 2)
			PlayerCollider.sharedMaterial = Normal;
	}
	
	
	
	// Running
	
	void Caring(){
		
		if ((OnAir == 1)&&(OnAttack != 1)){
			
			PlayerRigid2D.gravityScale = 9.8f;
		}
		
		if (OnAir == 0){
			
			PlayerRigid2D.gravityScale = 4.9f;
		}
	}
	
	void Rendering(){
		
		float R = PlayerSprite.color.r;
		float G = PlayerSprite.color.g;
		float B = PlayerSprite.color.b;
		
		PlayerSprite.color = new Color(R, G, B, Mathf.Abs((Mathf.Cos(AlphaInvincible))) );
	}
	
	void OnTriggerStay2D(Collider2D other){
		
		switch (other.tag){
			
			case "Platform":
			
				ctw_Platform_behavior PlatformScript = other.GetComponent<ctw_Platform_behavior>();
				
				if ((PlatformScript.Trigger == false)&&(PlayerRigid2D.velocity.y <= 0)){
					if (OnAir == 1){
						GenEffect(0f, 15f, 1f, 3);
						GenEffect(180f, 15f, 1f, 3);
					}
					OnAir = 0;
				}
			break;
			
			case "Bullet":
				if (Invincible == 0){
					ctw_Bullet_Collider_Script BulletScript = other.GetComponent<ctw_Bullet_Collider_Script>();
					if ((DEAD != 1)&&(BulletScript.OnWork == true)) {
						GenEffect(Get_Angle_byPosition(PlayerTransform.position, other.GetComponent<Transform>().position)+35f, 15f, 1f, 3);
						GenEffect(Get_Angle_byPosition(PlayerTransform.position, other.GetComponent<Transform>().position)-35f, 15f, 1f, 3);
						OnDamage();
					}
					BulletScript.Hitted();
				}
			break;
			
			case "Ground":
				if ((OnAir == 1)&&(PlayerRigid2D.velocity.y <= -1f)){
					GenEffect(0f, 15f, 1f, 3);
					GenEffect(180f, 15f, 1f, 3);
				}
				OnAir = 0;
			break;
		}
	}
	
	void OnTriggerExit2D(Collider2D other){
		
		switch (other.tag){
			
			case "Platform":
				OnAir = 1;
			break;
			
			case "Ground":
				OnAir = 1;
			break;
		}
	}
	
	void OnCollisionEnter2D(Collision2D other){
		
		if ((OnAttack != 2)&&(other.collider.name == "BS_Boss")&&(Invincible == 0)){
			if (DEAD != 1) {
				GenEffect(Get_Angle_byPosition(PlayerTransform.position, other.collider.GetComponent<Transform>().position), 25f, 2f, 10);
				OnDamage();
			}
		}
		if ((OnAttack == 2)&&(other.collider.name == "BS_Boss")){
			
			GenEffect(Get_Angle_byPosition(other.collider.GetComponent<Transform>().position, PlayerTransform.position)+60f, 30f, 3f, 8);
			GenEffect(Get_Angle_byPosition(other.collider.GetComponent<Transform>().position, PlayerTransform.position)-60f, 30f, 3f, 8);

			BossBehavior bossScript = other.collider.GetComponent<BossBehavior>();
			bossScript.OnDamaged(Math_Boss_Damage());
		}
		
		TimerAttackReset();
	}
	
    void Update(){
		if (DEAD != 1){
			InputAttack();
			InputMove();
		}
		else {
			OnAttack = 0;
			PlayerRigid2D.angularDrag = 0.2f;
			PlayerRigid2D.drag = 0.2f;
			PlayerRigid2D.gravityScale = 9.8f;
			PlayerCollider.sharedMaterial = Normal;
		}
			
		
		OnInvincible();
		
		Caring();
		Rendering();
    }
}
