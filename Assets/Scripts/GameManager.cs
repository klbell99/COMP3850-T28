using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;}
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    // "dim" - point where sky goes darker
    public float dimTime;   //in minutes
    private bool dimFlag;
    public Material dawnSkybox;
    private bool dawnFlag;
    private float skyboxTint;   //value for R, G, and B of skybox tint
    public float skyboxChangeTime;  //time in seconds for one skybox change to happen (eg. night sky -> pitch black)
    private float tintChange;   //amount tint changes by per second
    public Transform sun;
    public Light moonlight;
    private float moonChange;


    // "dark" - point where fire starts going out
    public float darkTime;  //in minutes
    private bool darkFlag;

    private bool morning;   //to stop dawn transition

    // "end" - point where timed experience ends
    public float endTime;   //in minutes

    // Start is called before the first frame update
    void Start()
    {
        // dimTime = dimTime * 60;//change to seconds
        // darkTime = darkTime * 60;
        // endTime = endTime * 60;

        dimFlag = false;
        dawnFlag = false;
        darkFlag = false;
        endTime *= 60;  //change to seconds
        darkTime *= 60;
        dimTime *= 60;
        morning = false;
        skyboxTint = RenderSettings.skybox.GetColor("_Tint").r;
        tintChange = skyboxTint / skyboxChangeTime;
        moonChange = moonlight.intensity / skyboxChangeTime;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Time.time >= dimTime && !dimFlag) {
        //     dimFlag = true;
        //     DimLight();
        //     Debug.Log("dimmed");
        // }
        if (Time.time >= dimTime && !dimFlag) {
            dimFlag = true;
        }
        if (dimFlag && !dawnFlag) {
            if (skyboxTint <= 0.01) {
                skyboxTint = 0;
                Color fullDark = new Color(0f, 0f, 0f, 128f);
                RenderSettings.skybox.SetColor("_Tint", fullDark);
                //moonlight.intensity = 0;
                //sun.gameObject.SetActive(true);
                GetComponent<AudioSource>().Play();
                dawnFlag = true;
                RenderSettings.skybox = dawnSkybox;
            } else {
                skyboxTint -= (tintChange * Time.deltaTime);
                Color thisColor = new Color(skyboxTint, skyboxTint, skyboxTint, 128f);
                RenderSettings.skybox.SetColor("_Tint", thisColor);
                //moonlight.intensity -= (moonChange * Time.deltaTime);
            }
        }
        if (dawnFlag) {
            skyboxTint += ((tintChange * Time.deltaTime)/2f);
            Color thisColor = new Color(skyboxTint, skyboxTint, skyboxTint, 128f);
            RenderSettings.skybox.SetColor("_Tint", thisColor);
            sun.position += new Vector3(0f, Time.deltaTime, 0f) * 7f;
        }
        if (Time.time >= darkTime && !darkFlag) {
            FireCollider.Instance.ShrinkFire();
            darkFlag = true;
        }
        if (Time.time >= endTime) {
            Application.Quit();
        }
    }
}
