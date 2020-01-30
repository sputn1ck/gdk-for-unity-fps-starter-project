using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
public class AnnouncementMessageUI : MonoBehaviour
{

    public TextMeshProUGUI text;
    public Animator animator;


    public void play(string text, Color color, float duration)
    {
        if (duration == 0) return;
        animator.SetTrigger("play");
        animator.SetFloat("speed", 1/duration) ;
        this.text.text = text;
        this.text.color = color;
    }


}
