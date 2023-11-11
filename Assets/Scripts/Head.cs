using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        Stick stick = other.GetComponent<Stick>();
        if (stick._CurrentState != Stick.MarshmallowState.None && stick._CurrentState != Stick.MarshmallowState.Burnt) {
            other.GetComponent<Stick>().EatMallow();
            GetComponent<AudioSource>().Play();
        }
    }
}
