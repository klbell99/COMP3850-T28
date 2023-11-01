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
    private Color toastColor;
    //private float elapsedEatTime;
    //public float mallowEatTime;     // time in seconds for mallow eat animation to play
    private int bites;      //current number of bites taken in animation (0 - no bites, 1 - 1 bite, 2 - 2 bites)
    private float mallowScale;

    private MarshmallowState mallowState;
    public enum MarshmallowState
    {
        None,
        Raw,
        Cooked,
        Burnt,
        Melted,
        //Eating
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
        toastColor = toastMat.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (cooking && mallowState != MarshmallowState.None && mallowState != MarshmallowState.Burnt) {
            MallowToast();
        }

        if (mallowState == MarshmallowState.Burnt) {
            toastTime += Time.deltaTime;
            if (mallow.activeInHierarchy) {
                mallowScale = Mathf.Lerp(0.75f, 0f, (toastTime-burnThreshold)/(meltThreshold-burnThreshold));
                mallow.transform.localScale = new Vector3(mallowScale, mallowScale, mallowScale);
            }
            if (mallowBiteOne.activeInHierarchy) {
                mallowScale = Mathf.Lerp(2.02f, 0f, (toastTime-burnThreshold)/(meltThreshold-burnThreshold));
                mallowBiteOne.transform.localScale = new Vector3(mallowScale, mallowScale, mallowScale);
            }
            if (mallowBiteTwo.activeInHierarchy) {
                mallowScale = Mathf.Lerp(2.02f, 0f, (toastTime-burnThreshold)/(meltThreshold-burnThreshold));
                mallowBiteTwo.transform.localScale = new Vector3(mallowScale, mallowScale, mallowScale);
            }
            if (toastTime >= meltThreshold) {
                smallFire.Stop();
                FireCollider.Instance.StopSound();
                MallowDisappear();
            }
        }
    }
    
    public void MallowAppear() {
        toastTime = 0;
        mallowState = MarshmallowState.Raw;
        mallow.GetComponent<Renderer>().material = toastMat;
        mallowBiteOne.GetComponent<Renderer>().material = toastMat;
        mallowBiteTwo.GetComponent<Renderer>().material = toastMat;
        toastAlpha = 0f;
        toastColor.a = 0f;
        toastMat.color = toastColor;
        bites = 0;
        mallow.SetActive(true);
        GetComponent<AudioSource>().Play();
        mallow.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        mallowBiteOne.transform.localScale = new Vector3(2.02f, 2.02f, 2.02f);
        mallowBiteTwo.transform.localScale = new Vector3(2.02f, 2.02f, 2.02f);
    }

    public void MallowDisappear() {
        mallowState = MarshmallowState.None;
        mallow.SetActive(false);
        mallowBiteOne.SetActive(false);
        mallowBiteTwo.SetActive(false);
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
                smallFire.Play();
                mallow.GetComponent<Renderer>().material = burnMat;
                mallowBiteOne.GetComponent<Renderer>().material = burnMat;
                mallowBiteTwo.GetComponent<Renderer>().material = burnMat;
                mallowState = MarshmallowState.Burnt;
            } else {
                // if (toastAlpha < 254) {
                //     toastAlpha += (toastIncrement * Time.deltaTime);
                //     Color thisColor = toastMat.color;
                //     thisColor.a = toastAlpha;
                //     toastMat.color = thisColor;
                // }
                toastAlpha = Mathf.Lerp(0, 1, toastTime/burnThreshold);
                toastColor.a = toastAlpha;
                toastMat.color = toastColor;
            }
        }
        // if (toastTime > meltThreshold && mallowState == MarshmallowState.Burnt) {
        //     smallFire.Stop();
        //     mallowState = MarshmallowState.Melted;
        // }
        // if (toastTime > (meltThreshold + timeToMelt) && mallowState == MarshmallowState.Melted) {
        //     FireCollider.Instance.StopSound();
        //     MallowDisappear();
        // }
    }

    public void EatMallow() {
        if (mallowState != MarshmallowState.None) {
            bites++;
            if (bites == 1) {
                mallowBiteOne.SetActive(true);
                mallow.SetActive(false);
            }
            if (bites == 2) {
                mallowBiteTwo.SetActive(true);
                mallowBiteOne.SetActive(false);
            }
            if (bites == 3) {
                mallowBiteTwo.SetActive(false);
                mallowState = MarshmallowState.None;
            }
        }
    }
}
