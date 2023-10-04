using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMallow : MonoBehaviour
{

    public bool isHead;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        if (isHead) {
            other.GetComponent<Prototype>().MallowDisappear();
        } else {
            other.GetComponent<Prototype>().MallowAppear();
        }
    }
}
