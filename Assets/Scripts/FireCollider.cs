using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCollider : MonoBehaviour
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
        other.GetComponent<Stick>().CookingState(true);
        GetComponent<AudioSource>().Play();
    }

    void OnTriggerExit(Collider other) {
        other.GetComponent<Stick>().CookingState(false);
        GetComponent<AudioSource>().Stop();
    }
}
