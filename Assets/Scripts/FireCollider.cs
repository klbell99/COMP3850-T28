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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
