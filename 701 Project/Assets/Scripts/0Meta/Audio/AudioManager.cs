using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance;    // Probably don't need this, since we don't change scenes

    public GameObject canvasUI;
    private GameObject mainMenuUI;
    private GameObject setupUI;
    private GameObject battleUI;

    void Awake(){

        /*
         * 
         * Q: What is the reason for having acces to
         *     > canvasUI
         *     > mainMenuUI
         *     > setupUI
         *     > battleUI
         * 
         */

        #region Find UI "scenes" in canvas
        mainMenuUI = canvasUI.transform.Find("Main Menu").gameObject;
        setupUI = canvasUI.transform.Find("Setup").gameObject;
        battleUI = canvasUI.transform.Find("Battle").gameObject;
        #endregion

        // If there is no instance of the AudioManager, set it to this instance
        if (instance == null){
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds){
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name){
        // Find the first element in the sounds array that matches with the name we are searching for
        Sound s = Array.Find(sounds, sound => sound.name == name);  
        
        // If the sound is not found, exit the function
        if (s == null){ 
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        // Play the sound
        s.source.Play();
        //Debug.Log("Playing sound: " + name);
    }

        public void Stop(string name){
        // Find the first element in the sounds array that matches with the name we are searching for
        Sound s = Array.Find(sounds, sound => sound.name == name);  
        
        // If the sound is not found, exit the function
        if (s == null){ 
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        // Stop the sound
        s.source.Stop();
        //Debug.Log("Stopped playing sound: " + name);
    }
}
