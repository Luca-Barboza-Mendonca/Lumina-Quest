using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadInput : MonoBehaviour
{   
    private string input;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReadStringInput(string s){
        input = s;
        int portNumber;
        try
        {
            portNumber = Int32.Parse(s);

        } catch(Exception ex)
        {
            Debug.Log(ex);
        }
    }
}
