using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour
{
    public GameObject mallow;
    public GameObject mallowBiteOne;
    public GameObject mallowBiteTwo;

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
    private float elapsedEatTime;
    public float mallowEatTime;     // time in seconds for mallow eat animation to play
    private int bites;      //current number of bites taken in animation (0 - no bites, 1 - 1 bite, 2 - 2 bites)

    private MarshmallowState mallowState;
    public enum MarshmallowState
    {
        None,
        Raw,
        Cooked,
        Burnt,
        Melted,
        Eating
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

        if (mallowState == MarshmallowState.Eating) {
            elapsedEatTime += Time.deltaTime;
            if ((elapsedEatTime >= (mallowEatTime/3)) && bites == 0) {
                mallowBiteOne.SetActive(true);
                mallow.SetActive(false);
                bites = 1;
            }
            if ((elapsedEatTime >= 2*(mallowEatTime/3)) && bites == 1) {
                mallowBiteTwo.SetActive(true);
                mallowBiteOne.SetActive(false);
                bites = 2;
            }
            if (elapsedEatTime >= mallowEatTime) {
                mallowBiteTwo.SetActive(false);
                mallowState = MarshmallowState.None;
            }
        }
    }
    
    public void MallowAppear() {
        toastTime = 0;
        mallowState = MarshmallowState.Raw;
        mallow.GetComponent<Renderer>().material = toastMat;
        mallowBiteOne.GetComponent<Renderer>().material = toastMat;
        mallowBiteTwo.GetComponent<Renderer>().material = toastMat;
        Color thisColor = toastMat.color;
        thisColor.a = 0f;
        toastMat.color = thisColor;
        toastAlpha = 0f;
        bites = 0;
        mallow.SetActive(true);
        GetComponent<AudioSource>().Play();
    }

    public void MallowDisappear() {
        mallowState = MarshmallowState.None;
        mallow.SetActive(false);
        mallowBiteOne.SetActive(false);
        mallowBiteTwo.SetActive(false);
    }

    public void CookingState(bool state) {
        cooking = state;
        if (state) {
            smallFire.Play();
        } else {
            smallFire.Stop();
        }
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
                mallowBiteOne.GetComponent<Renderer>().material = burnMat;
                mallowBiteTwo.GetComponent<Renderer>().material = burnMat;
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
            mallowState = MarshmallowState.Melted;
        }
        if (toastTime > (meltThreshold + timeToMelt) && mallowState == MarshmallowState.Melted) {
            FireCollider.Instance.StopSound();
            MallowDisappear();
        }
    }

    public void EatMallow() {
        if (mallowState != MarshmallowState.None && mallowState != MarshmallowState.Eating) {
            mallowState = MarshmallowState.Eating;
            elapsedEatTime = 0;
        }
    }
}
