using PresentationEngine.Demo;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class AnimatedCamera : MonoBehaviour
{
    public List<float> FieldOfView = new List<float>();
    public List<Color> BackgroundColor = new List<Color>();

    public bool Apply = false;

    private DemoFloatAnimation FOV;
    private DemoRGBColorAnimation BG;

    private Camera Cam;
    private Main M;

    void Start()
    {
        M = GameObject.Find("Main").GetComponent<Main>();

        BG = new DemoRGBColorAnimation(BackgroundColor);
        FOV = new DemoFloatAnimation(FieldOfView, 60);
        Cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Apply)
        {
            Start();
            Apply = false;
        }

        Cam.fieldOfView = FOV.GetValue(M.AnimationTime, M.Layer);
        Cam.backgroundColor = BG.GetValue(M.AnimationTime, M.Layer);
    }
}
