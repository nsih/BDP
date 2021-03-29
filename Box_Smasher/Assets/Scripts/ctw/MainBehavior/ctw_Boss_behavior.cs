using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctw_Boss_behavior : MonoBehaviour
{
	public float DamageForce;
	public float MaxHP;
	public float HP;
	public float LastHP;
	public int Invincible = 0;
	public int DEAD = 0;
	
	public GameObject BulletPrefab;
	
	Transform PlayerTransform;
	Rigidbody2D PlayerRigid2D;
	ctw_Player_behavior PlayerScript;
	
	ctw_Camera_behavior CameraScript;
	
	Transform BossTransform;
	SpriteRenderer BossSprite;
	ctw_Eraser_behavior Eraser;
	
	public GameObject[] Bullet = new GameObject[250];
	public int BulletPool = 0;
	
	int ATTACK = 1;
	int AttackType = 0;
	
	float AlphaInvincible = 0;
	
    void Start(){
		
        MaxHP = 10000;
		HP = 10000;
		LastHP = 10000;
		
		PlayerTransform = GameObject.Find("ctw_Player").GetComponent<Transform>();
		PlayerRigid2D = GameObject.Find("ctw_Player").GetComponent<Rigidbody2D>();
		PlayerScript = GameObject.Find("ctw_Player").GetComponent<ctw_Player_behavior>();
		
		CameraScript = GameObject.Find("ctw_Main Camera").GetComponent<ctw_Camera_behavior>();
		
		BossTransform = GetComponent<Transform>();
		BossSprite = GetComponent<SpriteRenderer>();
		Eraser = GameObject.Find("ctw_Eraser_Boss").GetComponent<ctw_Eraser_behavior>();
		
		Invoke("Timer_AttackCool",1f);
    }
	
	
	
	// Timers
	
	void Timer_InvincibleCool(){
		
		Invincible = 0;
	}
	
	void Timer_AttackCool(){
		
		ATTACK = 0;
	}
	
	
	
	// Maths
	
	float Math_2D_Force(float x, float y){
		
		return Mathf.Sqrt(Mathf.Pow(x,2)+Mathf.Pow(y,2));
	}
	
	
	
	// Checks
	
	public void OnDamage(float Damage){
		
		if (Invincible == 0){
			
			if (HP > Damage){
				
				LastHP = HP;
				HP -= Damage;
				AttackType = 0;
				ATTACK = 0;
				AlphaInvincible = 0f;
				Invincible = 1;
				Eraser.Alpha = 1f;
				Invoke("Timer_InvincibleCool",3.0f);
				CancelInvoke("Timer_AttackCool");
				CancelInvoke("PatternUpdate");
				CameraScript.CamShake = 1f;
			}
			
			else if (HP != 0) {
				
				LastHP = HP;
				HP = 0;
				DEAD = 1;
				Eraser.Alpha = 1f;
				CameraScript.CamShake = 2f;
			}
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
	
	
	
	// Gets
	
	float Get_angle_byPosition(Vector3 Pos){
		
		Vector3 BossPos = BossTransform.position;
		
		return ( Mathf.Atan2(Pos.y-BossPos.y, Pos.x-BossPos.x) * Mathf.Rad2Deg );
	}
	
	Vector2 Get_Force_Direction(){
		
		Vector3 PlayerPos = PlayerTransform.position;
		Vector3 BossPos = BossTransform.position;
		Vector3 PlayerPrivatePos = PlayerPos - BossPos;
		
		float RangeKey = Math_2D_Force(PlayerPrivatePos.x,PlayerPrivatePos.y);
		
		PlayerPrivatePos = PlayerPrivatePos/RangeKey;
		
		Vector2 ForceDirection = new Vector2(PlayerPrivatePos.x,PlayerPrivatePos.y);
		
		return ForceDirection;
	}
	
	Vector3 Get_Target_AngleToPos(float angle){
		
		return new Vector3(Mathf.Cos(angle*Mathf.Deg2Rad), Mathf.Sin(angle*Mathf.Deg2Rad), 0); 
	}
	
	
	Vector3 Get_Vector3_Direction(Vector3 Pos){
		
		float RangeKey = Math_2D_Force(Pos.x,Pos.y);
		
		Pos = Pos/RangeKey;
		
		Vector3 VectorDirection = new Vector3(Pos.x,Pos.y,0);
		
		return VectorDirection;
	}
	
	float Get_Vector3_Range(Vector3 Pos1, Vector3 Pos2){
		
		return Mathf.Sqrt(Mathf.Pow((Pos1.x - Pos2.x),2)+Mathf.Pow((Pos1.y - Pos2.y),2));
	}
	
	Quaternion Get_toPlayer_rotation(Vector3 Pos){
		
		Vector3 PlayerPos = PlayerTransform.position;
		
		float angle = Mathf.Atan2(PlayerPos.y-Pos.y, PlayerPos.x-Pos.x) * Mathf.Rad2Deg;
		
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		
		return rotation;
	}
	
	
	
	// Attacks
	
	GameObject Attack_Gen_Bullet(){
		

		BulletPool++;
		
		return Instantiate(BulletPrefab, BossTransform);
	}
	
	GameObject Attack_CheckandReturn(){
		
		int Key = 0;
		int i = 0;
		
		if (BulletPool != 0){
			for(;i<BulletPool; i++){
				if (Bullet[i].GetComponent<ctw_Bullet_behavior>().OnWork == false){
					Key = 1;
					break;
				}
			}
		}
		
		if (Key == 0){
			Bullet[i] = Attack_Gen_Bullet();
		}
		
		return Bullet[i];
	}
	
	void Attack_SetBullet(float Force,Vector3 Target,Quaternion rotation, float Timer_t, float roll){
		
		GameObject Bullet = Attack_CheckandReturn();
		Vector3 BossPos = BossTransform.position;
		
		Transform BulletTransform = Bullet.GetComponent<Transform>();
		ctw_Bullet_behavior BulletScript = Bullet.GetComponent<ctw_Bullet_behavior>();
		
		BulletTransform.position = BossPos;
		BulletTransform.rotation = rotation;
		BulletScript.Vel = Get_Vector3_Direction(Target)*Force;
		BulletScript.OnWork = true;
		BulletScript.Timer = Timer_t;
		BulletScript.Roll = roll;
		BulletScript.Alpha = 0f;
	}
	
	void Attack_TeleportBullet(float Force,Vector3 Target,Quaternion rotation, float Timer_t, float roll, Vector3 TPlocation){
		
		GameObject Bullet = Attack_CheckandReturn();
		
		Transform BulletTransform = Bullet.GetComponent<Transform>();
		ctw_Bullet_behavior BulletScript = Bullet.GetComponent<ctw_Bullet_behavior>();
		
		BulletTransform.position = TPlocation;
		BulletTransform.rotation = rotation;
		BulletScript.Vel = Get_Vector3_Direction(Target)*Force;
		BulletScript.OnWork = true;
		BulletScript.Timer = Timer_t;
		BulletScript.Roll = roll;
		BulletScript.Alpha = 0f;
	}
	
	
	
	// Attack Patterns
	
	void PatternUpdate(){
		
		// /* Turn this off when test new pattern
		if (AttackType < 4) AttackType++;
		else AttackType = 0;
		// */
		 /* Use This code to test new pattern
		AttackType = 4;
		 */
		CancelInvoke("PatternUpdate");
	}
	
	void Attack_Pattern_0(){
		
		Invoke("PatternUpdate",2.4f);
		
		float randomi = Random.Range(0f,20f);
		float randomj = Random.Range(-1.0f,1.0f);
		randomj = randomj/Mathf.Abs(randomj);
		for(float i = -180; i<180; i+=20){
			Attack_SetBullet(25f, Get_Target_AngleToPos(i+randomi), Quaternion.AngleAxis(i+randomi, Vector3.forward), 0f, -0.25f*randomj);
			Attack_SetBullet(18f, Get_Target_AngleToPos(i-10+randomi), Quaternion.AngleAxis(i-10+randomi, Vector3.forward), 0f, 0.2f*randomj);
		}
		ATTACK = 1;
		Invoke("Timer_AttackCool",0.8f);
	}
	
	void Attack_Pattern_1(){
		
		Invoke("PatternUpdate",1.5f);
		float randomi = Random.Range(0f,9f);

		for(float i = 0; i<360; i+=10){
			Attack_SetBullet(25f, Get_Target_AngleToPos(i+randomi), Quaternion.AngleAxis(i+randomi, Vector3.forward), i, 0f);
			Attack_SetBullet(25f, Get_Target_AngleToPos(120+i+randomi), Quaternion.AngleAxis(120+i+randomi, Vector3.forward), i, 0f);
			Attack_SetBullet(25f, Get_Target_AngleToPos(240+i+randomi), Quaternion.AngleAxis(240+i+randomi, Vector3.forward), i, 0f);
		}
		ATTACK = 1;
		Invoke("Timer_AttackCool",1.5f);
	}
	
	void Attack_Pattern_2(){
		
		Invoke("PatternUpdate",3f);
		
		for(float i = 0; i<360; i+=8){
			
			float randomi = Random.Range(0f,50f);
			
			Attack_SetBullet(10f-(i/120f), Get_Target_AngleToPos(i+randomi), Quaternion.AngleAxis(i+randomi, Vector3.forward), i, 0f);
			Attack_SetBullet(10f-(i/120f), Get_Target_AngleToPos(120+i+randomi), Quaternion.AngleAxis(120+i+randomi, Vector3.forward), i, 0f);
			Attack_SetBullet(10f-(i/120f), Get_Target_AngleToPos(240+i+randomi), Quaternion.AngleAxis(240+i+randomi, Vector3.forward), i, 0f);
		}
		
		ATTACK = 1;
		Invoke("Timer_AttackCool",3f);
	}
	
	void Attack_Pattern_3(){
		
		Invoke("PatternUpdate",3.2f);
		
		float Range = Get_Vector3_Range(PlayerTransform.position, BossTransform.position);
		float angle = Get_angle_byPosition(PlayerTransform.position);
		
		Attack_SetBullet( (50f-Range), Get_Target_AngleToPos(angle), Quaternion.AngleAxis(angle, Vector3.forward), 0f, 0f);
		Attack_SetBullet( (50f-Range), Get_Target_AngleToPos(angle+7), Quaternion.AngleAxis(angle+7, Vector3.forward), 0f, 0f);
		Attack_SetBullet( (50f-Range), Get_Target_AngleToPos(angle-7), Quaternion.AngleAxis(angle-7, Vector3.forward), 0f, 0f);
		ATTACK = 1;
		Invoke("Timer_AttackCool",0.8f);
	}
	
	void Attack_Pattern_4(){
		
		Invoke("PatternUpdate",4.8f);
		
		Vector3 PlayerPos = PlayerTransform.position;
		float angle = Get_angle_byPosition(PlayerTransform.position);
		float randomi = Random.Range(40f,50f);
		
		for(float i = 0;i < 360;i += 30){
			Vector3 TargetPos = PlayerPos + Get_Target_AngleToPos(i+randomi)*7f;
			Attack_TeleportBullet(25f, Get_Target_AngleToPos(i+randomi+180), Quaternion.AngleAxis(i+randomi+180, Vector3.forward), 120f, 0f, TargetPos);
		}
		
		ATTACK = 1;
		Invoke("Timer_AttackCool",1.6f);
	}
	
	
	// Running
	
	void Rendering(){
		
		float R = BossSprite.color.r;
		float G = BossSprite.color.g;
		float B = BossSprite.color.b;
		
		BossSprite.color = new Color(R, G, B, Mathf.Abs((Mathf.Cos(AlphaInvincible))) );
	}
	
	void Attacking(){
		if ((ATTACK == 0)&&(Invincible == 0)){
			Invoke("Attack_Pattern_"+AttackType.ToString() , 0f);
		}
	}
	
    void Update(){
		OnInvincible();
		
		if (DEAD == 0) Attacking();
		
		Rendering();
    }
}
