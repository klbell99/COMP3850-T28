using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject blinkObj;
    public Light fireLight;

    // "dim" - point where fire dims down, sky goes darker
    public float dimTime;   //in minutes
    private bool dimFlag;

    // "dark" - point where fire goes out completely
    public float darkTime;  //in minutes
    private bool darkFlag;

    // "end" - point where timed experience ends
    public float endTime;   //in minutes

    private float elapsedTime;  //in seconds

    private float sbRotation;
    public float sbSpeed;

    // Start is called before the first frame update
    void Start()
    {
        elapsedTime = 0;
        dimTime = dimTime * 60;//change to seconds
        darkTime = darkTime * 60;
        endTime = endTime * 60;

        dimFlag = false;
        darkFlag = false;

        sbRotation = 0;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= dimTime && !dimFlag) {
            dimFlag = true;
            DimLight();
            Debug.Log("dimmed");
        }
        //if (elapsedTime >= endTime) {
            //Application.Quit();
        //}
        //sbRotation = sbRotation + (Time.deltaTime * sbSpeed);
        //RenderSettings.skybox.SetFloat("_Rotation", sbRotation);
    }

    private void DimLight() {
        fireLight.intensity = 0.5f;
    }
}
