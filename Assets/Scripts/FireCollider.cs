using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCollider : MonoBehaviour
{
    public static FireCollider Instance { get; private set;}
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }
    private Vector3 fireScale;
    private bool fireDimming;
    public GameObject fireParticles;
    public ParticleSystem emberParticles;
    public float fireDimRate;//how long it should take for fire to dim completely in seconds
    private ParticleSystem.ColorOverLifetimeModule colourMod;
    private Gradient fireGrad;
    private float fireAlpha;
    public Light fireLight;
    public AudioSource fireMainSound;
    private float dimTime;
    // Start is called before the first frame update
    void Start()
    {
        //fireDimRate = 1f/fireDimRate;//change to the unit amount the scale should decrease by each second
        fireDimming = false;
        colourMod = fireParticles.GetComponent<ParticleSystem>().colorOverLifetime;
        fireGrad = colourMod.color.gradient;
        fireAlpha = fireGrad.alphaKeys[0].alpha;
        dimTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (fireDimming) {
            dimTime += Time.deltaTime;
            if (dimTime/fireDimRate >= 1) {
                dimTime = fireDimRate;
                fireDimming = false;
                GetComponent<BoxCollider>().enabled = false;
                StopSound();
                fireMainSound.Stop();
                emberParticles.Stop();
            }
            float thisScale = Mathf.Lerp(1, 0, dimTime/fireDimRate);
            //float thisRate = fireDimRate * Time.deltaTime;
            fireScale = new Vector3(thisScale, thisScale, thisScale);
            fireParticles.transform.localScale = fireScale;
            fireAlpha = Mathf.Lerp(0.62f, 0f, dimTime/fireDimRate);
            fireGrad.SetKeys(
                fireGrad.colorKeys,
                new GradientAlphaKey[] { 
                    new GradientAlphaKey(fireAlpha, 0.0f), new GradientAlphaKey(fireAlpha, 1.0f) 
                }
            );
            colourMod.color = fireGrad;
            //float thisIntensity = fireLight.intensity;
            fireLight.intensity = thisScale;//thisIntensity - thisRate;
            // if (fireParticles.transform.localScale.x < 0.1 || fireGrad.alphaKeys[0].alpha <= 0) {
            //     fireDimming = false;
            //     GetComponent<BoxCollider>().enabled = false;
            //     StopSound();
            //     fireMainSound.Stop();
            //     emberParticles.SetActive(false);
            //     fireLight.intensity = 0;
            // }
        }
    }

    void OnTriggerEnter(Collider other) {
        Stick stick = other.GetComponent<Stick>();
        if (stick._CurrentState != Stick.MarshmallowState.None) {
            stick.CookingState(true);
            GetComponent<AudioSource>().Play();
        }
    }

    void OnTriggerExit(Collider other) {
        other.GetComponent<Stick>().CookingState(false);
        GetComponent<AudioSource>().Stop();
    }

    public void StopSound() {
        GetComponent<AudioSource>().Stop();
    }

    public void ShrinkFire() {
        fireDimming = true;
    }
}
