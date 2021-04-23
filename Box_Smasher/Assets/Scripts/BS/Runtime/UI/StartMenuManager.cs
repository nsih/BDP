using System.Collections;
using System.Collections.Generic;
using BS.Utils;
using UnityEngine;

public class StartMenuManager : MonoBehaviour
{
    public void Play(){
        Debug.Log("게임 시작");
    }

    public void Settings(){
        Debug.Log("설정 보기");
    }

    public void Exit(){
        AppHelper.Quit();
    }
}
