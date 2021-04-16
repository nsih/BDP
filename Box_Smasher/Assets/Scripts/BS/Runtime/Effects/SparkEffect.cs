using System.Collections;
using System.Collections.Generic;
using BS.Player;
using UnityEngine;


public class SparkEffect : MonoBehaviour
{
    public GameObject player;

    public GameObject spark0;
    public GameObject spark1;
    List<GameObject> sparkList = new List<GameObject>();

    public float sparkCoolTime;
    float timerTime;
    bool isCool;

    private void Start()
    {
        InitSpark();
    }

    void Update()
    {
        CoolTimer();

        if (player.GetComponent<PlayerController>().OnAir == 0 &&
            player.GetComponent<PlayerController>().ismoving == 1 )
        {
            if (isCool == false)
                Spark();
        }
    }



    void InitSpark()
    {
        for (int i = 0; i < 50; i++)
        {
            if ( i % 2 ==0 )
            {
                GameObject temp = Instantiate(spark0) as GameObject;
                temp.gameObject.SetActive(false);

                sparkList.Add(temp);
                sparkList[i].transform.SetParent(this.transform);
            }

            else
            {
                GameObject temp = Instantiate(spark1) as GameObject;
                temp.gameObject.SetActive(false);

                sparkList.Add(temp);
                sparkList[i].transform.SetParent(this.transform);
            }
        }
    }

    void Spark()
    {
        isCool = true;
        timerTime = sparkCoolTime;

        for (int i = 0; i <= sparkList.Count; i++)
        {
            int act = 0;
            foreach(var item  in sparkList)
            {
                if(item.active == true)
                {
                    act++;
                }
            }

            if( act == sparkList.Count) //버그방지
            {
                Debug.Log("spark list full");
                break;
            }

            else if(sparkList[i].active == false)
            {
                Vector2 sparkSpot 
                    = new Vector2(Random.Range(player.transform.position.x-1.0f, player.transform.position.x+1.0f) 
                                , player.transform.position.y - 0.3f);


                sparkList[i].transform.position = sparkSpot;
                sparkList[i].SetActive(true);

                break;
            }
        }   
    }


    void CoolTimer()
    {
        if (timerTime <= 0)
            isCool = false;

        else
            timerTime -= Time.deltaTime;
    }
}
