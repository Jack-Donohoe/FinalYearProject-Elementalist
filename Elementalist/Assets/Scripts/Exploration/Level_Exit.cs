using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Exit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.LoadNextLevel();
        }
    }
}
