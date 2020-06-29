using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerContextMenu : MonoBehaviour, ILookAtHandler
{

    [Require] HunterComponentReader hunterComponentReader;
    private void OnEnable()
    {
    }


    public void OnLookAtEnter()
    {
        List<(UnityAction, string)> actions = new List<(UnityAction, string)>();
        //(UnityAction, string) bookmarkAction = (BookmarkUrl, GameText.AdContextMenuBookmarkActionLabel);
        //actions.Add(bookmarkAction);
        string text = string.Format(GameText.PlayerContextMenuText, Utility.SatsToShortString(hunterComponentReader.Data.Bounty, true, UITinter.tintDict[TintColor.Sats]));
        ContextMenuUI.Instance.Set(this, hunterComponentReader.Data.Name, text, actions);
    }


    public void OnLookAtExit()
    {
        ContextMenuUI.Instance.Hide(this);
    }
}
