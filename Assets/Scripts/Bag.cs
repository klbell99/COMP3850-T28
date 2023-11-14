using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour
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
        other.GetComponent<Stick>().MallowAppear();
        //GetComponent<AudioSource>().Play();   // bag rustling sound effect currently unused
    }
}
