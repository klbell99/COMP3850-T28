using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour
{
    // GameObject references of marshmallows and marshmallow materials
    public GameObject mallow;           // default marshmallow with no bites
    public GameObject mallowBiteOne;    // marshmallow with one bite
    public GameObject mallowBiteTwo;    // marshmallow with two bites
    // Due to odd sizing on the model imports, mallow's default scale is 0.75f and mallowBiteOne/Two are both 2.02f
    public Material toastMat;           // material for toasting texture, alpha value increases over time
    public Material burnMat;            // material for burnt texture, swap to this when marshmallow burns

    // Particle system references for smoke and fire that appears over marshmallow when it is burnt
    public ParticleSystem smoke;
    public ParticleSystem smallFire;

    // time variables
    public float toastThreshold;    // time it takes for marshmallow to go from raw to cooked [set to 0 due to mechanic ending up unused]
    public float burnThreshold;     // time it takes for marshmallow to burn
    public float meltThreshold;     // time it takes for marshmallow to melt since it got burnt
    private float toastTime;        // current time spent toasting a marshmallow
    private bool cooking;           // set to true if marshmallow is currently being toasted over the fire
    private float toastIncrement;   // how much the alpha value of the toast texture should increase by each second
    private float toastAlpha;       // current toast texture alpha value
    private Color toastColor;       // current colour applied to toastMat using toastAlpha
    private int bites;              // current number of bites taken in animation (0 - no bites, 1 - 1 bite, 2 - 2 bites)
    private float mallowScale;      // size of marshmallow as it shrinks/melts

    // Enum variable keeping track of the current state of the marshmallow
    private MarshmallowState mallowState;
    public enum MarshmallowState
    {
        None,
        Raw,
        Cooked,
        Burnt,
        Melted
    }
    // method to let other scripts (fireCollider) see state of marshmallow without editing it
    public MarshmallowState _CurrentState{
        get { return mallowState; }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialise status of marshmallow and variables
        toastTime = 0;
        mallowState = MarshmallowState.None;
        cooking = false;
        toastIncrement = (burnThreshold+toastThreshold)/255f;
        toastColor = toastMat.color;
    }

    // Update is called once per frame
    void Update()
    {
        // Don't cook marshmallow if it is burnt or there is none
        if (cooking && mallowState != MarshmallowState.None && mallowState != MarshmallowState.Burnt) {
            MallowToast();
        }

        // Melt marshmallow down
        if (mallowState == MarshmallowState.Burnt) {
            toastTime += Time.deltaTime;
            // Find marshmallow model currently active and scale it down
            if (mallow.activeInHierarchy) {
                mallowScale = Mathf.Lerp(0.75f, 0f, toastTime/meltThreshold);
                mallow.transform.localScale = new Vector3(mallowScale, mallowScale, mallowScale);
            }
            if (mallowBiteOne.activeInHierarchy) {
                mallowScale = Mathf.Lerp(2.02f, 0f, toastTime/meltThreshold);
                mallowBiteOne.transform.localScale = new Vector3(mallowScale, mallowScale, mallowScale);
            }
            if (mallowBiteTwo.activeInHierarchy) {
                mallowScale = Mathf.Lerp(2.02f, 0f, toastTime/meltThreshold);
                mallowBiteTwo.transform.localScale = new Vector3(mallowScale, mallowScale, mallowScale);
            }
            // End melting process
            if (toastTime >= meltThreshold) {
                smallFire.Stop();
                FireCollider.Instance.StopSound();
                MallowDisappear();
            }
        }
    }
    
    public void MallowAppear() {
        // Reset all variables for new marshmallow
        toastTime = 0;
        mallowState = MarshmallowState.Raw;
        // Set initial material for all marshmallows
        mallow.GetComponent<Renderer>().material = toastMat;
        mallowBiteOne.GetComponent<Renderer>().material = toastMat;
        mallowBiteTwo.GetComponent<Renderer>().material = toastMat;
        toastAlpha = 0f;
        toastColor.a = 0f;
        toastMat.color = toastColor;
        // Make sure all marshmallows are scaled correctly
        mallow.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        mallowBiteOne.transform.localScale = new Vector3(2.02f, 2.02f, 2.02f);
        mallowBiteTwo.transform.localScale = new Vector3(2.02f, 2.02f, 2.02f);
        // Set default, unbitten marshmallow as active
        bites = 0;
        mallow.SetActive(true);
        // Play marshmallow equipping sound
        GetComponent<AudioSource>().Play();
    }

    public void MallowDisappear() {
        // Ensure all marshmallows are gone
        mallowState = MarshmallowState.None;
        mallow.SetActive(false);
        mallowBiteOne.SetActive(false);
        mallowBiteTwo.SetActive(false);
    }

    // Changes whether marshmallow is cooking or not, called by FireCollider
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
                // Set all marshmallows to correct material
                mallow.GetComponent<Renderer>().material = burnMat;
                mallowBiteOne.GetComponent<Renderer>().material = burnMat;
                mallowBiteTwo.GetComponent<Renderer>().material = burnMat;
                // Reset toastTime for melting
                toastTime = 0;
                mallowState = MarshmallowState.Burnt;
            } else {
                toastAlpha = Mathf.Lerp(0, 1, toastTime/burnThreshold);
                toastColor.a = toastAlpha;
                toastMat.color = toastColor;
            }
        }
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
                // marshmallow completely eaten
                mallowBiteTwo.SetActive(false);
                mallowState = MarshmallowState.None;
            }
        }
    }
}
