using BS.Projectile;
using BS.Enemy.Boss;
using UnityEngine;

/// 콜리더만 있는 Bullet을 한 개 생성해 플레이어와 가장 가까이에 있고 (Onwork == true)인 총알을 따라다니며 피격처리를 함

public class ctw_Bullet_Collider_Script : MonoBehaviour
{
	Transform BulletTransform;
	Collider2D BulletCollider;
	Rigidbody2D BulletRigid2D;
	SpriteRenderer BulletSprite;
	
	public GameObject TargetBullet;
	
	public bool OnWork = true;
	
	BulletEraser Eraser1;
	BulletEraser Eraser2;
	
	Transform PlayerTransform;
	BossBehavior Boss;
	
	void Start(){
		
		BulletTransform = GetComponent<Transform>();
        BulletCollider = GetComponent<BoxCollider2D>() as Collider2D;
		BulletRigid2D = GetComponent<Rigidbody2D>();
		BulletSprite = GetComponent<SpriteRenderer>();
		
		Eraser1 = GameObject.Find("BS_Eraser_Player").GetComponent<BulletEraser>();
		Eraser2 = GameObject.Find("BS_Eraser_Boss").GetComponent<BulletEraser>();
		
		PlayerTransform = GameObject.Find("BS_Player").GetComponent<Transform>();
		Boss = GameObject.Find("BS_Boss").GetComponent<BossBehavior>();
		
		BulletSprite.color = new Color(0f, 0f, 0f, 0f);
    }
	
	public void Hitted(){
		TargetBullet.GetComponent<Bullet>().OnWork = false;
	}
	
	float Math_2D_Range(Vector3 Pos1, Vector3 Pos2){
		
		return Mathf.Sqrt(Mathf.Pow((Pos1.x-Pos2.x),2)+Mathf.Pow((Pos1.y-Pos2.y),2));
	}
	
	void Picking(){
		
		if (Boss.BulletPool != 0){
			Vector3 Player = PlayerTransform.position;
			float Last = 500;
			for(int i = 0;i<Boss.BulletPool; i++){
				if (Boss.Bullet[i].GetComponent<Bullet>().OnWork == true){
					Vector3 Bullets = Boss.Bullet[i].GetComponent<Transform>().position;
					if (Last > Math_2D_Range(Bullets,Player)){
					Last = Math_2D_Range(Bullets,Player);
					TargetBullet = Boss.Bullet[i];
					}
				}
			}
		}
		
	}
	
	void Rendering(){
		
		if ( (Eraser1._sprite.enabled != false)||(Eraser2._sprite.enabled != false) ) OnWork = false;
		else OnWork = true;
		
		if (Boss.DEAD){
			OnWork = false;
		}
		
		if (TargetBullet != null){
			BulletTransform.position = TargetBullet.GetComponent<Transform>().position;
			BulletRigid2D.rotation = TargetBullet.GetComponent<Rigidbody2D>().rotation;
		}
	}
	
    void Update(){
		
		Picking();
        Rendering();
    }
}