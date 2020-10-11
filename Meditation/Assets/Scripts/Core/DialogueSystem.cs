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
        if (textArchitect != null && textArchitect.isConstructing)
        {
            textArchitect.Stop();
        }
        speaking = null;
    }

    public bool isSpeaking {get{return speaking!=null;}}
    [HideInInspector] public bool isWaitingForUserInput = false;

    string targetSpeech = "";
    Coroutine speaking = null;
    TextArchitect textArchitect = null;
    IEnumerator Speaking(string speech, bool additive, string speaker=""){
        speechPanel.SetActive(true);
        string additiveSpeech = additive ? speechText.text : "";
        targetSpeech = additiveSpeech + speech;

        textArchitect = new TextArchitect(speech, additiveSpeech);

        speakerNameText.text = DetermineSpeaker(speaker);
        isWaitingForUserInput = false;

        while (textArchitect.isConstructing)
        {
            if(Input.GetKey(KeyCode.Space))
                textArchitect.skip = true;

            speechText.text = textArchitect.currentText;

            
            yield return new WaitForEndOfFrame();
        }
        speechText.text = textArchitect.currentText;

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
