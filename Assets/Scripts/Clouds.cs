using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    public float rotationSpeed;
    private float thisRotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        thisRotation = rotationSpeed * Time.deltaTime;
        this.gameObject.transform.Rotate(thisRotation, 0f, 0f);
    }
}
