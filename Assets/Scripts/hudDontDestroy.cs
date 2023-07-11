using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hudDontDestroy : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
