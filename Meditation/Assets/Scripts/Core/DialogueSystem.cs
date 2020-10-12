using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public void Say(string speech, string speaker="", bool additive = false){
        StopSpeaking();

        if(additive)
            speechText.text = targetSpeech;



        speaking = StartCoroutine(Speaking(speech, additive, speaker));

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
    public TextArchitect currentArchitect {get{return textArchitect;}}
    IEnumerator Speaking(string speech, bool additive, string speaker=""){
        speechPanel.SetActive(true);
        string additiveSpeech = additive ? speechText.text : "";
        targetSpeech = additiveSpeech + speech;

        textArchitect = new TextArchitect(speechText, speech, additiveSpeech);

        speakerNameText.text = DetermineSpeaker(speaker);
        speakerNamePane.SetActive(speakerNameText.text != "" ? true : false);

        isWaitingForUserInput = false;

        while (textArchitect.isConstructing)
        {
            if(Input.GetKey(KeyCode.Space))
                textArchitect.skip = true;


            
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
        public GameObject speakerNamePane;
        public TextMeshProUGUI speakerNameText;
        public TextMeshProUGUI speechText;
    }

    public GameObject speechPanel {get {return eLEMENTS.speechPanel;}}
    public TextMeshProUGUI speakerNameText {get {return eLEMENTS.speakerNameText;}}
    public TextMeshProUGUI speechText {get {return eLEMENTS.speechText;}}
    public GameObject speakerNamePane {get {return eLEMENTS.speakerNamePane;}}
}
