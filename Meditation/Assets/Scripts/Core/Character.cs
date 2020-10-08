using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Character
{
    public string characName;
    [HideInInspector] public RectTransform root;

    public bool isMultiLayerCharac{get {return renderers.renderer==null; }}

    public bool enabled {get {return root.gameObject.activeInHierarchy;} set {root.gameObject.SetActive(value);}}

    DialogueSystem dialogueSystem;
    public Character(string _name, bool enableOnStart = true)
    {
        CharacterManager cm = CharacterManager.instance;
        GameObject prefab = Resources.Load("CharacterPrefab/Character["+_name+"]") as GameObject;
        GameObject ob = GameObject.Instantiate(prefab, cm.characterPanel);
        root = ob.GetComponent<RectTransform>();
        characName = _name;

        renderers.renderer = ob.GetComponentInChildren<RawImage>();
        if(isMultiLayerCharac)
        {
            renderers.bodyRenderer = ob.transform.Find("body").GetComponent<Image>();
            renderers.expressionRenderer = ob.transform.Find("expression").GetComponent<Image>();
        }

        dialogueSystem = DialogueSystem.instance;
        enabled = enableOnStart;
    }


    public void Say(string speech, bool add = false){

        if(!enabled)
            enabled = true;

        if(!add)
            dialogueSystem.Say(speech, characName);
        else
            dialogueSystem.SayAdd(speech, characName);

    }

    [System.Serializable]
    public class Renderers
    {
        public RawImage renderer;
        public Image bodyRenderer;
        public Image expressionRenderer;

    }
    public Renderers renderers = new Renderers();
}
