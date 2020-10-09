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
            renderers.bodyRenderer = ob.transform.Find("bodyLayer").GetComponentInChildren<Image>();
            renderers.expressionRenderer = ob.transform.Find("expressionLayer").GetComponentInChildren<Image>();
            renderers.allBodyRenders.Add(renderers.bodyRenderer);
            renderers.allExpressionRenders.Add(renderers.expressionRenderer);
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

    // Begin Transitioning Images
    public Sprite GetSprite(int index = 0)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Characters/" + characName);
        return sprites[index];
    }

    public void SetBody(int index)
    {
        renderers.bodyRenderer.sprite = GetSprite(index);

    }

    public void SetBody(Sprite sprite)
    {   
        renderers.bodyRenderer.sprite = sprite;
    }


    public void SetExpression(int index)
    {
        renderers.expressionRenderer.sprite = GetSprite(index);

    }

    public void SetExpression(Sprite sprite)
    {   
        renderers.expressionRenderer.sprite = sprite;
    }


    // Body transition
    bool isTransitioningBody {get {return transitioningBody != null;}}
    Coroutine transitioningBody = null;

    public void TranisitionBody(Sprite sprite, float speed, bool smooth)
    {
        if(renderers.bodyRenderer.sprite == sprite)
            return;
        
        StopTransitioningBody();
        transitioningBody = CharacterManager.instance.StartCoroutine(TransitioningBody(sprite, speed, smooth));
    }

    void StopTransitioningBody()
    {
        if(isTransitioningBody)
            CharacterManager.instance.StopCoroutine(transitioningBody);
        transitioningBody = null;
    }

    public IEnumerator TransitioningBody(Sprite sprite, float speed, bool smooth)
    {
        for (int i = 0; i< renderers.allBodyRenders.Count; i++)
        {
            Image image = renderers.allBodyRenders[i];
            if (image.sprite == sprite)
            {
                renderers.bodyRenderer = image;
                break;
            }
        }    

        if(renderers.bodyRenderer.sprite != sprite)
        {
            Image image = GameObject.Instantiate(renderers.bodyRenderer.gameObject, renderers.bodyRenderer.transform.parent).GetComponent<Image>();
            renderers.allBodyRenders.Add(image);
            renderers.bodyRenderer = image;
            image.color = GlobalF.setAlpha(image.color, 0f);
            image.sprite = sprite;
        }

        while(GlobalF.TransitionImages(ref renderers.bodyRenderer, ref renderers.allBodyRenders, speed, smooth))
        {
            yield return new WaitForEndOfFrame();
        }

        Debug.Log ("done");

        StopTransitioningBody();
    }


     // Expression transition
    bool isTransitioningExpression {get {return transitioningExpression != null;}}
    Coroutine transitioningExpression = null;

    public void TranisitionExpression(Sprite sprite, float speed, bool smooth)
    {
        if(renderers.expressionRenderer.sprite == sprite)
            return;
        
        StopTransitioningExpression();
        transitioningExpression = CharacterManager.instance.StartCoroutine(TransitioningExpression(sprite, speed, smooth));
    }

    void StopTransitioningExpression()
    {
        if(isTransitioningExpression)
            CharacterManager.instance.StopCoroutine(transitioningExpression);
        transitioningExpression = null;
    }

    public IEnumerator TransitioningExpression(Sprite sprite, float speed, bool smooth)
    {
        for (int i = 0; i< renderers.allExpressionRenders.Count; i++)
        {
            Image image = renderers.allExpressionRenders[i];
            if (image.sprite == sprite)
            {
                renderers.expressionRenderer = image;
                break;
            }
        }    

        if(renderers.expressionRenderer.sprite != sprite)
        {
            Image image = GameObject.Instantiate(renderers.expressionRenderer.gameObject, renderers.expressionRenderer.transform.parent).GetComponent<Image>();
            renderers.allExpressionRenders.Add(image);
            renderers.expressionRenderer = image;
            image.color = GlobalF.setAlpha(image.color, 0f);
            image.sprite = sprite;
        }

        while(GlobalF.TransitionImages(ref renderers.expressionRenderer, ref renderers.allExpressionRenders, speed, smooth))
        {
            yield return new WaitForEndOfFrame();
        }

        Debug.Log ("done");

        StopTransitioningExpression();
    }
    // End transitioning images



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

        public List<Image> allBodyRenders = new List<Image>();
        public List<Image> allExpressionRenders = new List<Image>();

    }
    public Renderers renderers = new Renderers();
}
