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

    // "dim" - point where fire dims down, sky goes darker
    //public float dimTime;   //in minutes
    //private bool dimFlag;

    // "dark" - point where fire starts going out
    public float darkTime;  //in minutes
    private bool darkFlag;

    private bool morning;

    // "end" - point where timed experience ends
    public float endTime;   //in minutes

    // Start is called before the first frame update
    void Start()
    {
        // dimTime = dimTime * 60;//change to seconds
        // darkTime = darkTime * 60;
        // endTime = endTime * 60;

        // dimFlag = false;
        darkFlag = false;
        endTime *= 60;  //change to seconds
        darkTime *= 60;
        morning = false;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Time.time >= dimTime && !dimFlag) {
        //     dimFlag = true;
        //     DimLight();
        //     Debug.Log("dimmed");
        // }
        if (Time.time >= darkTime && !darkFlag) {
            FireCollider.Instance.ShrinkFire();
            darkFlag = true;
        }
        if (Time.time >= endTime) {
            Application.Quit();
        }
    }

    public void Sunrise() {
        morning = true;
    }
}
