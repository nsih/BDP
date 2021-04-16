using System.Collections;
using System.Collections.Generic;
using BS.Enemy.Boss;
using BS.Manager.Cameras;
using UnityEngine;

namespace BS.Player{
	public class PlayerController : MonoBehaviour
	{
		public Animator _bodyAnimator;
		public Animator _faceAnimator;
		protected PlayerAnimController _animController;

		public Rigidbody2D _rigidBody;
		public float _maxPower = 2000; /// 최대 차지 파워
		public float _currentPower = 0; /// 현재 차지 파워
		Collider2D _collider;
		
		public BulletEraser _eraser;
		
		public PhysicsMaterial2D Normal;
		public PhysicsMaterial2D Bouncy;
		
		Camera _mainCamera;
		
		public float _moveDirection;
		public bool DOWN = false;
		public int OnHit = 0;
		public int HP = 3;
		public bool Invincible = false;
		public bool IsCharging = false;
		public bool IsFalling = false;
		public bool Dead = false;
		public bool AttackSuccess = false;
		
		public int OnAir = 0;
		ctw_Effector_behavior Effect;
		
		void Awake(){
			_animController = this.transform.gameObject.AddComponent<PlayerAnimController>();
			_animController.Init(this, _bodyAnimator, _faceAnimator);
			_rigidBody = GetComponent<Rigidbody2D>();
			_collider = GetComponent<CapsuleCollider2D>() as Collider2D;
			
			if(_eraser == null){
				Debug.LogError("eraser가 할당되어 있지 않습니다.");
			}
			
			_mainCamera = CameraManager.Instance.MainCamera;
			Effect = GameObject.Find("BS_Effector").GetComponent<ctw_Effector_behavior>();
		}
		
		// Maths
		
		float Math_2D_Force(float x, float y){
			return Mathf.Sqrt(Mathf.Pow(x,2)+Mathf.Pow(y,2));
		}
		
		float Math_Boss_Damage(){
			return (500f + Mathf.Abs((_currentPower * Math_2D_Force(_rigidBody.velocity.x, _rigidBody.velocity.y) / 50f)) );
		}
		
		
		
		// Timers
		void TimerAttackReset(){
			OnHit = 0;
		}
		
		void TimerInvincibleReset(){
			Invincible = false;
		}
		
		// Gets
		Vector2 GetForceDirection(){
			
			Vector3 PlayerPos = this.transform.position;
			Vector3 MouseStaticPos = new Vector3(_mainCamera.ScreenToWorldPoint(Input.mousePosition).x,_mainCamera.ScreenToWorldPoint(Input.mousePosition).y,0);
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
				Invincible = true;
				_eraser.EraserWave(0.05f);
				_animController.OnHit();
				Invoke("TimerInvincibleReset",1.0f);
				CameraManager.Instance.ShakeCamera(0.5f, 0.05f);
			}
			else if (HP == 1){
				HP = 0;
				Dead = true;
				_animController.Dead();
				CameraManager.Instance.ShakeCamera(1f, 0.1f);
			}
		}
		
		// Effects
		void GenEffect(float angle, float F, float time, int num){
			Vector3 pos = this.transform.position;
			Effect.Effect_Run(time, pos, Get_Force_byAngle(angle)*F, num);
		}
		
		
		
		// Inputs
		
		void InputMove(){
			_moveDirection = 0;
			
			if (OnHit == 0){
				
				if (Input.GetKey(KeyCode.A))
					_moveDirection--;
				if (Input.GetKey(KeyCode.D))
					_moveDirection++;
				
				if (Mathf.Abs(_rigidBody.velocity.x) < 15)
				{
					_rigidBody.velocity = new Vector2(_rigidBody.velocity.x+1*_moveDirection,_rigidBody.velocity.y);
				}
				
				if (Input.GetKey(KeyCode.S)){
					DOWN = true;
				}
				else{
					DOWN = false;
				}
				
				if ((Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.Space))&&(OnAir == 0)){
					_rigidBody.velocity = new Vector2(_rigidBody.velocity.x,40);
					GenEffect(0f, 30f, 1f, 4);
					GenEffect(180f, 30f, 1f, 4);
					OnAir = 1;
				}
			}
		}
		
		float AngleBetweenTwoPoints(Vector3 a, Vector3 b){
			return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
		}

		void InputAttack(){

			if(Input.GetMouseButtonDown(0) && (OnAir == 1) && (OnHit != 2)){
				_currentPower = 0;
				IsCharging = true;
				OnHit = 1;
			}
			
			if ((Input.GetMouseButton(0)) && (OnAir == 1) && (OnHit != 2)){
				Vector2 pos = this.transform.position;
				Vector2 mouseOnWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

				float angle = AngleBetweenTwoPoints(pos, mouseOnWorld);

				transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

				float amount = _maxPower * 0.015f;
				float direction = (mouseOnWorld - pos).x <= 0 ? 1 : -1;

				_currentPower += amount * direction;
			}

			if ((Input.GetMouseButtonUp(0))&&(OnHit == 1)){
				if (Mathf.Abs(_currentPower) >= _maxPower){
					_currentPower = (_currentPower > 0 ? 1 : -1) * _maxPower;
				}

				_rigidBody.velocity = GetForceDirection() * Mathf.Abs(_currentPower) / 40;
				_collider.sharedMaterial = Bouncy;

				OnHit = 2;
				IsCharging = false;
			}
			
			if (OnHit == 1){
				_rigidBody.angularDrag = 0.1f;
				_rigidBody.drag = 2.5f;
				_rigidBody.gravityScale = 0.5f;
			}
			
			else{
				_rigidBody.angularDrag = 0.2f;
				_rigidBody.drag = 0.2f;
				_rigidBody.gravityScale = 9.8f;
			}
			
			if (OnHit != 2)
				_collider.sharedMaterial = Normal;
		}
		
		
		
		// Running
		
		void Caring(){
			
			if ((OnAir == 1)&&(OnHit != 1)){
				_rigidBody.gravityScale = 9.8f;
			}
			
			if (OnAir == 0){
				_rigidBody.gravityScale = 4.9f;
			}
		}
		
		void OnTriggerStay2D(Collider2D other){
			
			switch (other.tag){
				case "Platform":
					ctw_Platform_behavior PlatformScript = other.GetComponent<ctw_Platform_behavior>();
					
					if ((PlatformScript.Trigger == false)&&(_rigidBody.velocity.y <= 0)){
						if (OnAir == 1){
							GenEffect(0f, 15f, 1f, 3);
							GenEffect(180f, 15f, 1f, 3);
						}
						OnAir = 0;
						IsFalling = false;
						AttackSuccess = false;
						IsCharging = false;
					}
					break;
				case "Bullet":
					if (!Invincible){
						ctw_Bullet_Collider_Script BulletScript = other.GetComponent<ctw_Bullet_Collider_Script>();
						if ((!Dead)&&(BulletScript.OnWork == true)) {
							GenEffect(Get_Angle_byPosition(this.transform.position, other.GetComponent<Transform>().position)+35f, 15f, 1f, 3);
							GenEffect(Get_Angle_byPosition(this.transform.position, other.GetComponent<Transform>().position)-35f, 15f, 1f, 3);
							OnDamage();
						}
						BulletScript.Hitted();
					}
					break;
				case "Ground":
					if ((OnAir == 1)&&(_rigidBody.velocity.y <= -1f)){
						GenEffect(0f, 15f, 1f, 3);
						GenEffect(180f, 15f, 1f, 3);
					}
					OnAir = 0;
					IsFalling = false;
					AttackSuccess = false;
					IsCharging = false;
					break;
			}
		}
		
		void OnTriggerExit2D(Collider2D other){
			switch (other.tag){
				case "Platform":
					OnAir = 1;
					IsFalling = true;
					break;
				case "Ground":
					OnAir = 1;
					IsFalling = true;
					break;
			}
		}
		
		void OnCollisionEnter2D(Collision2D other){
			if ( (OnHit != 2) && (other.collider.name == "BS_Boss") && (!Invincible) ){
				if (!Dead) {
					GenEffect(Get_Angle_byPosition(this.transform.position, other.collider.GetComponent<Transform>().position), 25f, 2f, 10);
					OnDamage();
				}
			}
			if ( (OnHit == 2) && (other.collider.name == "BS_Boss") ){
				GenEffect(Get_Angle_byPosition(other.collider.GetComponent<Transform>().position, this.transform.position)+60f, 30f, 3f, 8);
				GenEffect(Get_Angle_byPosition(other.collider.GetComponent<Transform>().position, this.transform.position)-60f, 30f, 3f, 8);

				BossBehavior bossScript = other.collider.GetComponent<BossBehavior>();
				bossScript.OnDamaged(Math_Boss_Damage());
				AttackSuccess = true;
				_animController.AttackSuccess();
			}
			
			if (other.collider.tag != "Wall"){
				TimerAttackReset();
			}
		}
		
		void Update(){
			if (!Dead){
				InputAttack();
				InputMove();
			}
			else {
				OnHit = 0;
				_rigidBody.angularDrag = 0.2f;
				_rigidBody.drag = 0.2f;
				_rigidBody.gravityScale = 9.8f;
				_collider.sharedMaterial = Normal;
			}
			
			Caring();
			if(_animController != null){
				_animController.Render();
			}
		}
	}
}

