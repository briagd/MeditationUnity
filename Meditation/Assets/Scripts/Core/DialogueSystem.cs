using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{   
    public static DialogueSystem instance;
    public ELEMENTS eLEMENTS;


    void Awake(){
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Say(string speech, string speaker=""){
        StopSpeaking();
        speaking = StartCoroutine(Speaking(speech, false, speaker));

    }


    public void SayAdd(string speech, string speaker=""){
        StopSpeaking();
        speechText.text = targetSpeech;
        speaking = StartCoroutine(Speaking(speech, true, speaker));

    }

    public void StopSpeaking(){
         if (isSpeaking){
            StopCoroutine(speaking);
                    }
        speaking = null;
    }

    public bool isSpeaking {get{return speaking!=null;}}
    [HideInInspector] public bool isWaitingForUserInput = false;

    string targetSpeech = "";
    Coroutine speaking = null;

    IEnumerator Speaking(string speech, bool additive, string speaker=""){
        speechPanel.SetActive(true);
        targetSpeech = speech;
        if(!additive)
            speechText.text = "";
        else
            speech = speechText.text + targetSpeech;
        speechText.text = "";
        speakerNameText.text = DetermineSpeaker(speaker);
        isWaitingForUserInput = false;

        while (speechText.text != targetSpeech)
        {
            speechText.text += targetSpeech[speechText.text.Length];
            yield return new WaitForEndOfFrame();
        }

        isWaitingForUserInput = true;
        while(isWaitingForUserInput){
            yield return new WaitForEndOfFrame();
        }
        StopSpeaking();
    }

    string DetermineSpeaker(string s){
        string retVal = speakerNameText.text;
        if(s!=speakerNameText.text && s != "")
            retVal = (s.ToLower().Contains("narrator")) ? "" : s;

        return retVal;
        
    }

    public void Close(){
        StopSpeaking();
        speechPanel.SetActive(false);
    }

    [System.Serializable]
    public class ELEMENTS{
        public GameObject speechPanel;
        public Text speakerNameText;
        public Text speechText;
    }

    public GameObject speechPanel {get {return eLEMENTS.speechPanel;}}
    public Text speakerNameText {get {return eLEMENTS.speakerNameText;}}
    public Text speechText {get {return eLEMENTS.speechText;}}
}
