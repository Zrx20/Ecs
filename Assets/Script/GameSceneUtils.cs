using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

static public class GameSceneUtils 
{
    static public void LoadSceneAsync(string sceneName,Action call)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        ao.completed += (_ao) =>
        {
            call?.Invoke();
        };
    }
}
