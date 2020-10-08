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

    public Vector2 anchorPadding{get {return root.anchorMax - root.anchorMin;}}

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

    Vector2 targetPos;
    Coroutine moving;
    bool isMoving{get {return moving != null;}}
    public void MoveTo(Vector2 Target, float speed, bool smooth = true)
    {
        StopMoving();
        moving = CharacterManager.instance.StartCoroutine(Moving(Target, speed, smooth));
    }


    public void StopMoving(bool movTargetPosInstantly = false)
    {
        if(isMoving)
        {
            CharacterManager.instance.StopCoroutine(moving);
            if(movTargetPosInstantly)
            {
            SetPosition(targetPos);
            }
        }
        moving = null;
    }

    public void SetPosition(Vector2 target)
    {
        Vector2 padding = anchorPadding;
        float maxX = 1f - padding.x;
        float maxY = 1f - padding.y;

        Vector2 minAnchorTarget = new Vector2(maxX * targetPos.x, maxY * targetPos.y);

        root.anchorMin =  minAnchorTarget;
        root.anchorMax = root.anchorMin + padding;
        StopMoving();
    }

    IEnumerator Moving(Vector2 target, float speed, bool smooth)
    {
        targetPos = target;
        Vector2 padding = anchorPadding;
        float maxX = 1f - padding.x;
        float maxY = 1f - padding.y;

        Vector2 minAnchorTarget = new Vector2(maxX * targetPos.x, maxY * targetPos.y);
        speed *= Time.deltaTime;
        while(root.anchorMin != minAnchorTarget)
        {
            root.anchorMin = (!smooth) ? Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed) : Vector2.Lerp(root.anchorMin, minAnchorTarget, speed);
            root.anchorMax = root.anchorMin + padding;
            yield return new WaitForEndOfFrame();
        }

        StopMoving();

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
