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
    private float fireMainVol;
    private float dimTime;
    public Material charcoal;
    private Color coalCol;
    private float halfDimTime;
    public float coalDimRate;

    // Collider variables related to shrinking size of collider
    private BoxCollider colliderRef;
    private Vector3 colliderSize;
    private float centerY;  // Y value of the collider's center - needs to be half the collider's size y to keep position consistent
    private Vector3 currentCollSize;
    
    // Start is called before the first frame update
    void Start()
    {
        //fireDimRate = 1f/fireDimRate;//change to the unit amount the scale should decrease by each second
        fireDimming = false;
        colourMod = fireParticles.GetComponent<ParticleSystem>().colorOverLifetime;
        fireGrad = colourMod.color.gradient;
        fireAlpha = fireGrad.alphaKeys[0].alpha;
        dimTime = 0;
        halfDimTime = coalDimRate/2;
        colliderRef = this.GetComponent<BoxCollider>();
        colliderSize = colliderRef.size;
        centerY = colliderSize.y/2f;
        colliderRef.center = new Vector3(0, centerY, 0);
        fireMainVol = fireMainSound.volume;
    }

    // Update is called once per frame
    void Update()
    {
        if (fireDimming) {
            dimTime += Time.deltaTime;
            // Fully end dimming process
            if (dimTime/fireDimRate >= 1) {
                dimTime = fireDimRate;
                fireDimming = false;
                GetComponent<BoxCollider>().enabled = false;
                StopSound();
                fireMainSound.Stop();
                emberParticles.Stop();
            }
            // Scaling of fire particle system
            float thisScale = Mathf.Lerp(1, 0, dimTime/fireDimRate);
            fireScale = new Vector3(thisScale, thisScale, thisScale);
            fireParticles.transform.localScale = fireScale;
            float thisVolume = Mathf.Lerp(fireMainVol, 0, dimTime/fireDimRate);
            fireMainSound.volume = thisVolume;
            // Lower opacity of fire particles
            fireAlpha = Mathf.Lerp(0.62f, 0f, dimTime/fireDimRate);
            fireGrad.SetKeys(
                fireGrad.colorKeys,
                new GradientAlphaKey[] { 
                    new GradientAlphaKey(fireAlpha, 0.0f), new GradientAlphaKey(fireAlpha, 1.0f) 
                }
            );
            colourMod.color = fireGrad;
            // Decrease size of collider in proportion with particle system shrinking
            currentCollSize = Vector3.Lerp(colliderSize, new Vector3(0, 0, 0), dimTime/fireDimRate);
            colliderRef.size = currentCollSize;
            centerY = currentCollSize.y/2f;
            colliderRef.center = new Vector3(0, centerY, 0);
            // Lower emission of charcoal texture - should happen halfway through dimming process
            if (dimTime >= halfDimTime) {
                coalCol = Color.Lerp(Color.white, Color.black, (dimTime-halfDimTime)/halfDimTime);
                charcoal.SetColor("_EmissionColor", coalCol);
            }
            // Scale down intensity of surrounding light
            fireLight.intensity = thisScale;
        }
        if (dimTime >= halfDimTime && !fireDimming && dimTime <= coalDimRate) {
            dimTime += Time.deltaTime;
            coalCol = Color.Lerp(Color.white, Color.black, (dimTime-halfDimTime)/halfDimTime);
            charcoal.SetColor("_EmissionColor", coalCol);
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

    // Public function to immediately stop sizzling sound, called by Stick script when marshmallow melts
    public void StopSound() {
        GetComponent<AudioSource>().Stop();
    }

    public void ShrinkFire() {
        fireDimming = true;
    }
}
