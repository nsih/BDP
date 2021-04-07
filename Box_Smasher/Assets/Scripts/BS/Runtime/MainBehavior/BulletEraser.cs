using System.Collections;
using BS.Enemy.Boss;
using UnityEngine;

public class BulletEraser : MonoBehaviour
{
	public SpriteRenderer _sprite;
	CircleCollider2D _collider;

	// collider 원래 반지름
	float _originRadius;

	
    void Awake(){
		_collider = GetComponent<CircleCollider2D>() as CircleCollider2D;
		_originRadius = _collider.radius;
		_sprite.enabled = false;
    }

	public void EraserWave(float t = 0.3f, float speed = 2.5f){
		StartCoroutine(StartEraserWave(t, speed));
	}

	protected IEnumerator StartEraserWave(float t, float speed){
		_sprite.enabled = true;

		float duration = t;

		while(duration > 0){
			duration -= Time.deltaTime;

			this.transform.localScale += (speed * Vector3.one);
			_collider.radius += (_originRadius * speed);
			yield return new WaitForSeconds(Time.deltaTime);
		}

		this.transform.localScale = Vector3.one;
		_collider.radius = _originRadius;
		_sprite.enabled = false;
		yield return null;
	}
}
