using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollection : MonoBehaviour {
    private AudioSource audioSource;
    void Start()
    {   
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            audioSource.Play();
            Destroy(gameObject, audioSource.clip.length);
        }
    }

    
}
