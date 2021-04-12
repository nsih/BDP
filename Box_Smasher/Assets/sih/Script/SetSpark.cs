using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpark : MonoBehaviour
{
    public float sparkTime;

    float timerTime;
    bool isCool;


    void OnEnable()
    {
        timerTime = sparkTime;
        this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100, 100), Random.Range(100, 500)));
    }

    void Update()
    {
        CoolTimer();
    }


    void CoolTimer()
    {
        if (timerTime <= 0)
            this.gameObject.SetActive(false);

        else
            timerTime -= Time.deltaTime;
    }
}
