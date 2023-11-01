using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
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
    // all colours in 0-255 range, converted to 0-1 later
    public Vector3 duskColour;    // r/g/b for the initial sunset colour
    public Vector3 nightColour;     // r/g/b for the night sky at its brightest
    public Vector3 dawnColour;   // r/g/b for the dawn sky at its brightest
    private Vector3 black = new Vector3(0f, 0f, 0f);

    public float duskRotation;  // rotation of skybox at dusk
    public float dawnRotation;  // rotation of skybox at dawn - ideally should be 180 degrees above the dusk rotation
    private float nightRotation;

    public Text line1;
    public Text line2;
    private Color textCol1 = Color.white;
    private Color textCol2 = Color.white;

    // enum for different states of the night cycle
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

    // "dawn" - sunrise variables
    public float dawnTime;  // in minutes - time that dawn transition starts
    private bool dawnFlag;  // true when dawn transition is complete

    // "dim" - point where fire starts going out
    public float dimTime;  // in minutes
    private bool dimFlag;   // true when ShrinkFire() call has been made

    // "end" - point where timed experience ends
    public float endTime;   // in minutes

    // Start is called before the first frame update
    void Start()
    {
        currentSky = SkyTime.Dusk;
        dimFlag = false;
        dawnFlag = false;
        // change times from minutes to seconds
        endTime *= 60;
        dawnTime *= 60;
        dimTime *= 60;
        elapsedTime = 0;
        Color startColour = new Color(duskColour.x/255f, duskColour.y/255f, duskColour.z/255f, 1f);
        RenderSettings.skybox.SetColor("_Tint", startColour);
        RenderSettings.skybox.SetFloat("_Rotation", duskRotation);
        textCol1.a = 0;
        textCol2.a = 0;
        nightRotation = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= 2 && Time.time < 5.5) {
            if (textCol1.a < 1) {
                textCol1.a += (Time.deltaTime/3);
                line1.color = textCol1;
            } else if (textCol1.a > 1) {
                textCol1.a = 1;
                line1.color = textCol1;
            }
        }
        if (Time.time >= 8 && Time.time < 11.5) {
            if (textCol2.a < 1) {
                textCol2.a += (Time.deltaTime/3);
                line2.color = textCol2;
            } else if (textCol2.a > 1) {
                textCol2.a = 1;
                line2.color = textCol2;
            }
        }
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
                currentSky = SkyTime.NightStart;
                elapsedTime = 0;
                RenderSettings.skybox = nightSkybox;
            }
        }
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
        if (currentSky == SkyTime.Night) {
            nightRotation += (Time.deltaTime*0.05f);
            RenderSettings.skybox.SetFloat("_Rotation", nightRotation);
        }
        if (Time.time >= dawnTime && currentSky == SkyTime.Night) {
            currentSky = SkyTime.NightEnd;
        }
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
                GetComponent<AudioSource>().Play();
            }
        }
        if (currentSky == SkyTime.Dawn && !dawnFlag) {
            elapsedTime += Time.deltaTime;
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
        if (Time.time >= dimTime && !dimFlag) {
            FireCollider.Instance.ShrinkFire();
            dimFlag = true;
        }
        if (Time.time >= endTime) {
            Application.Quit();
        }
    }
}
