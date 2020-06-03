using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "BBH/Skills/Dictionary", order = 0)]
public class SkillDictionary : ScriptableObject
{

    public static SkillDictionary Instance;

    [SerializeField] public PlayerSkill[] skills;
    // Start is called before the first frame update

    public static PlayerSkill Get(int index)
    {
        if (Instance == null)
        {
            Debug.LogError("The Gun Dictionary has not been set.");
            return null;
        }

        if (index < 0 || index >= Count)
        {
            //Debug.LogErrorFormat("The index {0} is outside of the dictionary's range (size {1}).", index, Count);
            return null;
        }

        return Instance.skills[index];
    }

    public static int Count => Instance.skills.Length;
}
