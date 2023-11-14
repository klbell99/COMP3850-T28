using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Keep GameManager as a singleton
    public static GameManager Instance { get; private set;}
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    // Skybox assets
    public Material daySkybox;  // used for both sunset and sunrise with different colours applied
    public Material nightSkybox;    // used for nighttime
    // all colours in 0-255 range for inspector modification, converted to 0-1 later
    public Vector3 duskColour;    // r/g/b for the initial sunset colour
    public Vector3 nightColour;     // r/g/b for the night sky at its brightest
    public Vector3 dawnColour;   // r/g/b for the dawn sky at its brightest
    private Vector3 black = new Vector3(0f, 0f, 0f);

    // skybox rotations
    public float duskRotation;  // rotation of skybox at dusk
    public float dawnRotation;  // rotation of skybox at dawn - ideally should be 180 degrees above the dusk rotation
    private float nightRotation;    // current rotation of night skybox, is turned slowly during the night

    // Variables for starting narration text
    public Text line1;
    public Text line2;
    private Color textCol1 = new Color(1, 1, 1, 0);   // current colour of line1, also used for both lines when dimming them
    private Color textCol2 = new Color(1, 1, 1, 0);   // current colour of line2 as it appears

    // enum for different states of the night cycle
    // Dusk - sky is going from dusk to black
    // NightStart - sky is going from black to night
    // Night - sky does not change
    // NightEnd - sky is going from night to black
    // Dawn - sky is going from black to dawn
    private enum SkyTime
    {
        Dusk,
        NightStart,
        Night,
        NightEnd,
        Dawn
    }
    private SkyTime currentSky;    // variable for current state of the enum
    private float elapsedTime;      // elapsed time of current transition
    public float transitionTime;    // time in seconds for one skybox change to happen (eg. night sky -> pitch black)

    // "dawn" - sunrise variables + sound assets
    public float dawnTime;  // in minutes - time that dawn transition starts
    private bool dawnFlag;  // true when dawn transition is complete
    public AudioSource yawnAudio;   // audio source on tent for yawn clip
    public float yawnTime;  // in seconds - time after SkyTime=Dawn that yawn audio clip plays
    private bool yawnFlag;  // true when yawn has been played to avoid sending the call again

    // "dim" - point where fire starts going out
    public float dimTime;  // in minutes
    private bool dimFlag;   // true when ShrinkFire() call has been made

    // "end" - point where timed experience ends
    public float endTime;   // in minutes
    public SpriteRenderer fadeScreen;   // plain black sprite that is faded in at the end of the experience4
    private Color fadeColor = new Color(1, 1, 1, 0);
    public float timeToFade;    // seconds it takes to fade out the experience before the application quits
    private float fadeTimer;    // current time in seconds spent fading

    // Start is called before the first frame update
    void Start()
    {
        // set initial state of SkyTime enum
        currentSky = SkyTime.Dusk;
        // set flags to false at start
        dimFlag = false;
        dawnFlag = false;
        yawnFlag = false;
        // change times from minutes to seconds
        endTime *= 60;
        dawnTime *= 60;
        dimTime *= 60;
        //set initial colour + rotation of skybox
        Color startColour = new Color(duskColour.x/255f, duskColour.y/255f, duskColour.z/255f, 1f);
        RenderSettings.skybox.SetColor("_Tint", startColour);
        RenderSettings.skybox.SetFloat("_Rotation", duskRotation);
        // set timers + nightRotation to 0
        elapsedTime = 0;
        nightRotation = 0;
        fadeTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // line1 appears
        if (Time.time >= 2 && Time.time < 5.5) {
            if (textCol1.a < 1) {
                textCol1.a += (Time.deltaTime/3);
                line1.color = textCol1;
            } else if (textCol1.a > 1) {
                textCol1.a = 1;
                line1.color = textCol1;
            }
        }
        // line2 appears
        if (Time.time >= 8 && Time.time < 11.5) {
            if (textCol2.a < 1) {
                textCol2.a += (Time.deltaTime/3);
                line2.color = textCol2;
            } else if (textCol2.a > 1) {
                textCol2.a = 1;
                line2.color = textCol2;
            }
        }
        // both text lines disappear
        if (Time.time >= 17 && Time.time < 20.5) {
            if (textCol1.a > 0) {
                textCol1.a -= (Time.deltaTime/3);
                line1.color = textCol1;
                line2.color = textCol1;
            } else if (textCol1.a < 0) {
                textCol1.a = 0;
                line1.color = textCol1;
                line2.color = textCol1;
            }
        }
        // dusk transition
        if (currentSky == SkyTime.Dusk) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > transitionTime) { // avoid overshoot
                elapsedTime = transitionTime;
            }
            float currentInterpolation = elapsedTime / transitionTime;
            // lerp between initial colour for this skybox and final colour
            Vector3 thisVector = Vector3.Lerp(duskColour, black, currentInterpolation);
            // convert vector to colour, with r/g/b being changed from 0-255 to 0-1
            Color thisColour = new Color(thisVector.x/255f, thisVector.y/255f, thisVector.z/255f, 1f);
            RenderSettings.skybox.SetColor("_Tint", thisColour);
            if (elapsedTime == transitionTime) {
                // swap skybox, reset elapsedTime
                currentSky = SkyTime.NightStart;
                elapsedTime = 0;
                RenderSettings.skybox = nightSkybox;
            }
        }
        // night sky appears
        if (currentSky == SkyTime.NightStart) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > transitionTime) { // avoid overshoot
                elapsedTime = transitionTime;
            }
            float currentInterpolation = elapsedTime / transitionTime;
            Vector3 thisVector = Vector3.Lerp(black, nightColour, currentInterpolation);
            Color thisColour = new Color(thisVector.x/255f, thisVector.y/255f, thisVector.z/255f, 1f);
            RenderSettings.skybox.SetColor("_Tint", thisColour);
            nightRotation += (Time.deltaTime*0.05f);
            RenderSettings.skybox.SetFloat("_Rotation", nightRotation);
            if (elapsedTime == transitionTime) {
                currentSky = SkyTime.Night;
                elapsedTime = 0;
            }
        }
        // rotate night sky
        if (currentSky == SkyTime.Night) {
            nightRotation += (Time.deltaTime*0.05f);
            RenderSettings.skybox.SetFloat("_Rotation", nightRotation);
        }
        // at dawnTime, change enum to start dawn transition
        if (Time.time >= dawnTime && currentSky == SkyTime.Night) {
            currentSky = SkyTime.NightEnd;
        }
        // night sky disappears
        if (currentSky == SkyTime.NightEnd) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > transitionTime) { // avoid overshoot
                elapsedTime = transitionTime;
            }
            float currentInterpolation = elapsedTime / transitionTime;
            Vector3 thisVector = Vector3.Lerp(nightColour, black, currentInterpolation);
            Color thisColour = new Color(thisVector.x/255f, thisVector.y/255f, thisVector.z/255f, 1f);
            nightRotation += (Time.deltaTime*0.05f);
            RenderSettings.skybox.SetFloat("_Rotation", nightRotation);
            RenderSettings.skybox.SetColor("_Tint", thisColour);
            if (elapsedTime == transitionTime) {
                currentSky = SkyTime.Dawn;
                elapsedTime = 0;
                RenderSettings.skybox = daySkybox;
                RenderSettings.skybox.SetFloat("_Rotation", dawnRotation);
                // play bird calls as dawn starts
                GetComponent<AudioSource>().Play();
            }
        }
        // dawn transition
        if (currentSky == SkyTime.Dawn && !dawnFlag) {
            elapsedTime += Time.deltaTime;
            // do yawn audio
            if (elapsedTime > yawnTime && !yawnFlag) {
                yawnFlag = true;
                yawnAudio.Play();
            }
            if (elapsedTime > transitionTime) { // avoid overshoot
                elapsedTime = transitionTime;
            }
            float currentInterpolation = elapsedTime / transitionTime;
            Vector3 thisVector = Vector3.Lerp(black, dawnColour, currentInterpolation);
            Color thisColour = new Color(thisVector.x/255f, thisVector.y/255f, thisVector.z/255f, 1f);
            RenderSettings.skybox.SetColor("_Tint", thisColour);
            if (elapsedTime == transitionTime) {
                dawnFlag = true;
            }
        }
        // send call to dim fire
        if (Time.time >= dimTime && !dimFlag) {
            FireCollider.Instance.ShrinkFire();
            dimFlag = true;
        }
        // begin ending fadeout
        if (Time.time >= endTime) {
            fadeTimer += Time.deltaTime;
            float thisAlpha = Mathf.Lerp(0, 1, fadeTimer/timeToFade);
            fadeColor.a = thisAlpha;
            fadeScreen.color = fadeColor;
        }
        // end application when fadeout is complete
        if (Time.time > (endTime + timeToFade)) {
            Application.Quit();
        }
    }
}
