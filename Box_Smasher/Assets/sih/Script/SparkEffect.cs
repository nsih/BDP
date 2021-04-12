using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkEffect : MonoBehaviour
{
    public GameObject Player;

    public GameObject Spark0;
    public GameObject Spark1;
    List<GameObject> SparkList = new List<GameObject>();

    bool isMoving = false;


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
        MoveCheck();

        if ( Player.GetComponent<PlayerController>().OnAir == 0 && isMoving == true )
        {
            if (isCool == false)
                Spark();
        }
    }



    void InitSpark()
    {
        for (int i = 0; i < 50; i++)
        {
            if (i%2 ==0)
            {
                GameObject temp = Instantiate(Spark0) as GameObject;
                temp.gameObject.SetActive(false);

                SparkList.Add(temp);
                SparkList[i].transform.SetParent(this.transform);
            }

            else
            {
                GameObject temp = Instantiate(Spark1) as GameObject;
                temp.gameObject.SetActive(false);

                SparkList.Add(temp);
                SparkList[i].transform.SetParent(this.transform);
            }
        }
    }

    void Spark()
    {
        isCool = true;
        timerTime = sparkCoolTime;

        for (int i = 0; i <= SparkList.Count; i++)
        {
            int act = 0;
            foreach(var item  in SparkList)
            {
                if(item.active == true)
                {
                    act++;
                }
            }

            if( act == SparkList.Count) //버그방지
            {
                Debug.Log("???");
                break;
            }

            else if(SparkList[i].active == false)
            {
                Vector2 sparkSpot 
                    = new Vector2(Random.Range(Player.transform.position.x-1.0f, Player.transform.position.x+1.0f) 
                                , Player.transform.position.y - 0.3f);


                SparkList[i].transform.position = sparkSpot;
                SparkList[i].SetActive(true);

                break;
            }
        }   
    }

    void MoveCheck()
    {
        if (Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.F))
            isMoving = true;

        else
            isMoving = false;
    }


    void CoolTimer()
    {
        if (timerTime <= 0)
            isCool = false;

        else
            timerTime -= Time.deltaTime;
    }
}
