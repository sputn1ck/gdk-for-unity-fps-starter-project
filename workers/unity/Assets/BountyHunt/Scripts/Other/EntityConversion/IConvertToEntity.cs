using Improbable.Gdk.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConvertToEntity
{
    void Convert(WorldCommandSender worldCommandSender, Map map);
}
