using BS.Enemy.Boss;
using UnityEngine;

public class ctw_Bullet_Collider_Script : MonoBehaviour
{
	Transform BulletTransform;
	Collider2D BulletCollider;
	Rigidbody2D BulletRigid2D;
	SpriteRenderer BulletSprite;
	
	public GameObject TargetBullet;
	
	public bool OnWork = true;
	
	ctw_Eraser_behavior Eraser1;
	ctw_Eraser_behavior Eraser2;
	
	Transform PlayerTransform;
	BossBehavior Boss;
	
	void Start(){
		
		BulletTransform = GetComponent<Transform>();
        BulletCollider = GetComponent<BoxCollider2D>() as Collider2D;
		BulletRigid2D = GetComponent<Rigidbody2D>();
		BulletSprite = GetComponent<SpriteRenderer>();
		
		Eraser1 = GameObject.Find("BS_Eraser_Player").GetComponent<ctw_Eraser_behavior>();
		Eraser2 = GameObject.Find("BS_Eraser_Boss").GetComponent<ctw_Eraser_behavior>();
		
		PlayerTransform = GameObject.Find("BS_Player").GetComponent<Transform>();
		Boss = GameObject.Find("BS_Boss").GetComponent<BossBehavior>();
		
		BulletSprite.color = new Color(0f, 0f, 0f, 0f);
    }
	
	public void Hitted(){
		TargetBullet.GetComponent<ctw_Bullet_behavior>().OnWork = false;
	}
	
	float Math_2D_Range(Vector3 Pos1, Vector3 Pos2){
		
		return Mathf.Sqrt(Mathf.Pow((Pos1.x-Pos2.x),2)+Mathf.Pow((Pos1.y-Pos2.y),2));
	}
	
	void Picking(){
		
		if (Boss.BulletPool != 0){
			Vector3 Player = PlayerTransform.position;
			float Last = 500;
			for(int i = 0;i<Boss.BulletPool; i++){
				if (Boss.Bullet[i].GetComponent<ctw_Bullet_behavior>().OnWork == true){
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
		
		if ( (Eraser1.Alpha != 0f)||(Eraser2.Alpha != 0f) ) OnWork = false;
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