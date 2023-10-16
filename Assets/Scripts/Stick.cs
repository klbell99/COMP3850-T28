using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour
{
    public GameObject mallow;
    public ParticleSystem smoke;
    public ParticleSystem smallFire;
    public Material toastMat;
    public Material burnMat;
    public float toastThreshold;
    public float burnThreshold;
    public float meltThreshold;
    public float timeToMelt;        // seconds it takes for marshmallow to melt
    private float toastTime;
    private bool cooking;
    private float toastIncrement;   // how much the alpha value of the toast texture should increase by each second
    private float toastAlpha;       // current toast texture alpha value

    private MarshmallowState mallowState;
    public enum MarshmallowState
    {
        None,
        Raw,
        Cooked,
        Burnt,
        Melted
    }
    public MarshmallowState _CurrentState{
        get { return mallowState; }
    }
    // Start is called before the first frame update
    void Start()
    {
        toastTime = 0;
        mallowState = MarshmallowState.None;
        cooking = false;
        toastIncrement = (burnThreshold+toastThreshold)/255f;
    }

    // Update is called once per frame
    void Update()
    {
        if (cooking && mallowState != MarshmallowState.None) {
            MallowToast();
        }
    }
    
    public void MallowAppear() {
        toastTime = 0;
        mallowState = MarshmallowState.Raw;
        mallow.GetComponent<Renderer>().material = toastMat;
        Color thisColor = toastMat.color;
        thisColor.a = 0f;
        toastMat.color = thisColor;
        toastAlpha = 0f;
        mallow.SetActive(true);
        GetComponent<AudioSource>().Play();
    }

    public void MallowDisappear() {
        if (mallowState != MarshmallowState.Raw && mallowState != MarshmallowState.None) {
            mallowState = MarshmallowState.None;
            mallow.SetActive(false);
        }
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
                smoke.Play();
                mallow.GetComponent<Renderer>().material = burnMat;
                mallowState = MarshmallowState.Burnt;
            } else {
                if (toastAlpha < 254) {
                    toastAlpha += (toastIncrement * Time.deltaTime);
                    Color thisColor = toastMat.color;
                    thisColor.a = toastAlpha;
                    toastMat.color = thisColor;
                }
            }
        }
        if (toastTime > meltThreshold && mallowState == MarshmallowState.Burnt) {
            smallFire.Play();
            mallowState = MarshmallowState.Melted;
        }
        if (toastTime > (meltThreshold + timeToMelt) && mallowState == MarshmallowState.Melted) {
            FireCollider.Instance.StopSound();
            MallowDisappear();
        }
    }
}
