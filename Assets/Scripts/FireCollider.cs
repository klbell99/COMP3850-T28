using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCollider : MonoBehaviour
{
    // FireCollider should be a singleton, so it can be easily referenced by GameManager
    public static FireCollider Instance { get; private set;}
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    private Vector3 fireScale;  // variable for current scale of fire
    private bool fireDimming;   // true when fire should be dimming, set by GameManager
    public float fireDimTime;   // how long it should take for fire to dim completely in minutes, converted to seconds
    private Gradient fireGrad;  // Gradient of fire colour, repeatedly changed and assigned to colourMod
    private float fireAlpha;    // Alpha value of fire colour, applied to fireGrad
    private float fireMainVol;  // Initial volume of fire ambience (fireMainSound)
    private float elapsedTime;      // Current time spent dimming fire

    // Charcoal dimming variables
    private bool coalDimming;   // true when coal should be dimming
    public float coalDimTime;   // how long in seconds after the fire is dimmed that the coal should be completely dimmed
    public Material charcoal;   // Charcoal material, emmission is lowered to dim the coal
    private Color coalCol;      // Current colour value to assign to charcoal material
    private float elapsedTimeC; // current elapsed time for dimming coal

    // Reference variables to outside objects affected by dimming
    public GameObject fireParticles;    // Fire particle system
    private ParticleSystem.ColorOverLifetimeModule colourMod;   // Colour of fireParticles to reduce alpha values
    public ParticleSystem emberParticles;   // Flying embers particle system
    public Light fireLight;     // Light intensity decreased with dimming
    public AudioSource fireMainSound;   // Volume lowered over time with dimming

    // Collider variables related to shrinking size of collider
    private BoxCollider colliderRef;    // The collider itself
    private Vector3 colliderSize;   // Initial size of collider, used for vector lerp
    private float centerY;  // Y value of the collider's center - needs to be half the collider's size y to keep position consistent
    private Vector3 currentCollSize;    // The new size determined each frame and assigned to colliderRef.size
    
    // Start is called before the first frame update
    void Start()
    {
        // Convert to seconds
        fireDimTime *= 60;
        // coal starts dimming halfway through fire dimming
        // coalDimTime is half of fireDimTime + some seconds set in Inspector
        coalDimTime += (fireDimTime/2);
        // initialise flags as false
        fireDimming = false;
        coalDimming = false;
        // get fireParticles colour and initialise related variables
        colourMod = fireParticles.GetComponent<ParticleSystem>().colorOverLifetime;
        fireGrad = colourMod.color.gradient;
        fireAlpha = fireGrad.alphaKeys[0].alpha;
        // initialise timers to 0
        elapsedTime = 0;
        elapsedTimeC = 0;
        // get reference to collider and set related variables
        colliderRef = this.GetComponent<BoxCollider>();
        colliderSize = colliderRef.size;
        centerY = colliderSize.y/2f;
        colliderRef.center = new Vector3(0, centerY, 0);    // ensure center of collider starts at half of height
        // get initial volume
        fireMainVol = fireMainSound.volume;
    }

    // Update is called once per frame
    void Update()
    {
        if (fireDimming) {
            elapsedTime += Time.deltaTime;
            // Fully end dimming process
            if (elapsedTime/fireDimTime >= 1) {
                elapsedTime = fireDimTime;
                fireDimming = false;
                GetComponent<BoxCollider>().enabled = false;
                StopSound();
                fireMainSound.Stop();
                emberParticles.Stop();
            }
            // Scaling of fire particle system
            float thisScale = Mathf.Lerp(1, 0, elapsedTime/fireDimTime);
            fireScale = new Vector3(thisScale, thisScale, thisScale);
            fireParticles.transform.localScale = fireScale;
            // Lower volume of ambience sounds
            float thisVolume = Mathf.Lerp(fireMainVol, 0, elapsedTime/fireDimTime);
            fireMainSound.volume = thisVolume;
            // Lower opacity of fire particles
            fireAlpha = Mathf.Lerp(0.62f, 0f, elapsedTime/fireDimTime);
            fireGrad.SetKeys(
                fireGrad.colorKeys,
                new GradientAlphaKey[] { 
                    new GradientAlphaKey(fireAlpha, 0.0f), new GradientAlphaKey(fireAlpha, 1.0f) 
                }
            );      // change alpha keys of fireGrad to reduce alpha over time
            colourMod.color = fireGrad;
            // Decrease size of collider in proportion with particle system shrinking
            currentCollSize = Vector3.Lerp(colliderSize, new Vector3(0, 0, 0), elapsedTime/fireDimTime);
            colliderRef.size = currentCollSize;
            centerY = currentCollSize.y/2f;
            colliderRef.center = new Vector3(0, centerY, 0);
            // Halfway point of fire dimming passed, start coalDimming
            if ((elapsedTime >= (fireDimTime/2)) && !coalDimming) {
                coalDimming = true;
            }
            // Scale down intensity of surrounding light
            fireLight.intensity = thisScale;
        }
        if (coalDimming) {
            elapsedTimeC += Time.deltaTime;
            // End coal dimming
            if (elapsedTimeC >= coalDimTime) {
                elapsedTimeC = coalDimTime;
                coalDimming = false;    // code below still executes on this last call
            }
            //Lower emission of charcoal texture
            coalCol = Color.Lerp(Color.white, Color.black, elapsedTimeC/coalDimTime);
            charcoal.SetColor("_EmissionColor", coalCol);
        }
    }

    // Start cooking on Stick and start sizzling sound when stick enters fire
    void OnTriggerEnter(Collider other) {
        Stick stick = other.GetComponent<Stick>();
        if (stick._CurrentState != Stick.MarshmallowState.None) {
            stick.CookingState(true);
            GetComponent<AudioSource>().Play();
        }
    }

    // Stop cooking on Stick and stop sizzling sound when stick exits fire
    void OnTriggerExit(Collider other) {
        other.GetComponent<Stick>().CookingState(false);
        GetComponent<AudioSource>().Stop();
    }

    // Public function to immediately stop sizzling sound, called by Stick script when marshmallow melts
    public void StopSound() {
        GetComponent<AudioSource>().Stop();
    }

    // Public function to start fire dimming process, called by GameManager script based on its dimTime variable
    public void ShrinkFire() {
        fireDimming = true;
    }
}
