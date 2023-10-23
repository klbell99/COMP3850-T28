//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PrototypeCollider : MonoBehaviour
//{

//    public bool isFire;
//    public bool isHead;
//    // Start is called before the first frame update
//    void Start()
//    {
//        if (isFire) {
//            this.gameObject.GetComponent<Renderer>().material.color = Color.red;
//        }
//        if (!isHead && !isFire) {
//            this.gameObject.GetComponent<Renderer>().material.color = Color.blue;
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }

//    void OnTriggerEnter(Collider other) {
//        if (!isFire && !isHead) {
//            other.GetComponent<Prototype>().MallowAppear();
//        }
//        if (isHead) {
//            other.GetComponent<Prototype>().MallowDisappear();
//        }
//    }

//    void OnTriggerStay(Collider other) {
//        if (isFire) {
//            other.GetComponent<Prototype>().MallowToast();
//        }
//    }
//}
