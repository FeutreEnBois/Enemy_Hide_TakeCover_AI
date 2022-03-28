using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnValidate()
    {
            Vector3 v = new Vector3(1, 1, 1).normalized;
            Debug.Log(v);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
