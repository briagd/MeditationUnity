using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterManager : MonoBehaviour
{


public RectTransform characterPanel;
    public static CharacterManager instance;

    public List<Character> characters = new List<Character>();

    public Dictionary <string, int> characterDictionary = new Dictionary<string, int>();


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public Character GetCharacter(string characterName, bool  createCharacterIfNotExist = true, bool enableCharacOnStart = true)
    {
        int index =-1;
        if(characterDictionary.TryGetValue(characterName, out index)){
            return characters[index];
        } else if (createCharacterIfNotExist){
            return CreateCharacter(characterName, enableCharacOnStart);
        }

        return null;
    }

    public Character CreateCharacter(string characterName, bool enableCharacOnStart = true)
    {
        Character newCharacter = new Character(characterName, enableCharacOnStart);
        characterDictionary.Add(characterName, characters.Count);
        characters.Add(newCharacter);
        return newCharacter;

    }
}
