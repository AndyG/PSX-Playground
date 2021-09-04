using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoFogCamera : MonoBehaviour
{

    private bool doesSceneHaveFog;

    void Start() {
        doesSceneHaveFog = RenderSettings.fog;
    }

    void OnPreRender() {
        RenderSettings.fog = false;
    }

    void OnPostRender() {
        RenderSettings.fog = doesSceneHaveFog;
    }
}
