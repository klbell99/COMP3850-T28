using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugObject : MonoBehaviour
{
    public GameObject defaultLight;
    public GameObject fireLight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Oculus_GearVR_RIndexTrigger") || Input.GetButtonDown("Fire1")) {
            if (defaultLight.activeInHierarchy) {
                defaultLight.SetActive(false);
                fireLight.SetActive(true);
            }
            else {
                defaultLight.SetActive(true);
                fireLight.SetActive(false);
            }
        }
    }
}
