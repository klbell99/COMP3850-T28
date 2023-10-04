using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour
{
    public GameObject mallow;
    public float toastThreshold;
    public float burnThreshold;
    public float meltThreshold;
    private float toastTime;
    private bool cooking;

    private MarshmallowState mallowState;
    private enum MarshmallowState
    {
        None,
        Raw,
        Cooked,
        Burnt
    }
    // Start is called before the first frame update
    void Start()
    {
        toastTime = 0;
        mallowState = MarshmallowState.None;
        cooking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (cooking) {
            MallowToast();
        }
    }
    
    public void MallowAppear() {
        toastTime = 0;
        mallowState = MarshmallowState.Raw;
        mallow.GetComponent<Renderer>().material.color = Color.white;
        mallow.SetActive(true);
    }

    public void MallowDisappear() {
        mallowState = MarshmallowState.None;
        mallow.SetActive(false);
    }

    public void CookingState(bool state) {
        cooking = state;
    }

    private void MallowToast() {
        toastTime += Time.deltaTime;
        if (toastTime > toastThreshold && mallowState == MarshmallowState.Raw) {
            mallowState = MarshmallowState.Cooked;
        }
        if (mallowState == MarshmallowState.Cooked) {
            if (toastTime > burnThreshold) {
                mallowState = MarshmallowState.Burnt;
            }
        }
        if (toastTime > meltThreshold && mallowState == MarshmallowState.Burnt) {
            MallowDisappear();
        }
    }
}
