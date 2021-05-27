using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracing : MonoBehaviour
{
    static private CameraTracing instance;

    public GameObject Player;

    public int f_speed; //카메라가 따라가는 속도


    void Awake()
    {
        Screen.SetResolution(1920, 1280, true);

        if (instance != null)
        {
            Destroy(this.gameObject);
        }

        else
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
    }

    void FixedUpdate()
    {
        follow();
    }

    void follow()
    {
        Vector2 Circle_Pos = Player.transform.position;

        Vector2 Camera_Pos = this.transform.position;

        this.gameObject.transform.Translate((Circle_Pos - Camera_Pos) * f_speed * Time.deltaTime);
    }
}
