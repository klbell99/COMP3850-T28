using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prototype : MonoBehaviour
{
    public GameObject mallow;
    private float toastTime = 0;
    private bool toasted = false;
    private bool burned = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MallowAppear() {
        toastTime = 0;
        toasted = false;
        burned = false;
        mallow.GetComponent<Renderer>().material.color = Color.white;
        mallow.SetActive(true);
    }

    public void MallowDisappear() {
        mallow.SetActive(false);
    }

    public void MallowToast() {
        toastTime += Time.deltaTime;
        if (toastTime > 10 && !toasted) {
            toasted = true;
            mallow.GetComponent<Renderer>().material.color = Color.yellow;
        }
        if (toastTime > 20 && !burned) {
            burned = true;
            MallowBurn();
        }
    }

    void MallowBurn() {
        mallow.GetComponent<Renderer>().material.color = Color.black;
    }
}
