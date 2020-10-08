using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterTesting : MonoBehaviour
{
    public Character boy;
    // Start is called before the first frame update
    void Start()
    {
        boy = CharacterManager.instance.GetCharacter("Boy", enableCharacOnStart:false);
    }

    public string[] speech;
    int i = 0;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (i < speech.Length){
                boy.Say(speech[i]);
            }
            else
                DialogueSystem.instance.Close();

            i++;
        }
    }
}
