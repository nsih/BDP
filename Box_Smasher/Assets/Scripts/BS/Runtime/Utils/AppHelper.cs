﻿using UnityEngine;
namespace BS.Utils{
    public static class AppHelper{
        public static void Quit(){
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
