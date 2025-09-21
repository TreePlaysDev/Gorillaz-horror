using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class joindiscord : GorillaPressableButton
{
    [Header("credits : quantum.ix")]
    public string url = "https://discord.gg/quantumsunityhub";

    public override void ButtonActivation()
    {
        Application.OpenURL(url);
    }
}
