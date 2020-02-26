using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KillFeedMessageUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Animator animator;
    [SerializeField] float fadeOutTime;
    [SerializeField] float fadeInTime;

    bool fadingOut;
    public float lifeTime;
    float hideTime;


    void Awake()
    {
        animator.SetFloat("fadeOutSpeed", 1/Mathf.Max(fadeOutTime,0.0001f));
        animator.SetFloat("fadeInSpeed", 1/Mathf.Max(fadeInTime,0.0001f));
    }

    void Update()
    {
        if (!fadingOut)
        {
            if(Time.time >= hideTime)
            {
                Hide();
            }
        }
    }

    public void SetNew(string msg)
    {
        gameObject.SetActive(true);
        text.text = msg;
        animator.SetTrigger("fadeIn");
        transform.SetAsLastSibling();
        hideTime = Time.time + lifeTime;
        fadingOut = false;
    }

    public void Hide()
    {
        if (fadingOut) return;
        fadingOut = true;
        animator.SetTrigger("fadeOut");
        StartCoroutine(RemoveAfterHide());
    }

    IEnumerator RemoveAfterHide()
    {
        KillFeedUI.instance.RemoveMessageFromActive(this);
        yield return new WaitForSeconds(fadeOutTime);
        KillFeedUI.instance.AddMessageToInactive(this);

    }

}
