using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnyKeyCont : MonoBehaviour
{
    [SerializeField]
    private string nextScene;

    void Update()
    {
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(this.nextScene);
        }
    }
}
