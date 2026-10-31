using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PresentationEngine.Demo;

[ExecuteAlways]
public class AnimatedLight : MonoBehaviour
{
    public List<Color> Color = new List<Color>();
    public List<float> Intensity = new List<float>();
    public List<float> Range = new List<float>();
    public List<float> Angle = new List<float>();

    public bool Apply = false;

    private DemoRGBColorAnimation LColor;
    private DemoFloatAnimation LIntensity;
    private DemoFloatAnimation LRange;
    private DemoFloatAnimation LAngle;

    private Light L;
    private Main M;
    void Start()
    {
        M = GameObject.Find("Main").GetComponent<Main>();
        L = GetComponent<Light>();

        LColor = new DemoRGBColorAnimation(Color);
        LIntensity = new DemoFloatAnimation(Intensity, 1);
        LRange = new DemoFloatAnimation(Range, 10);
        LAngle = new DemoFloatAnimation(Angle, 30);
    }

    void Update()
    {
        if (Apply)
        {
            Start();
            Apply = false;
        }

        L.color = LColor.GetValue(M.AnimationTime, M.Layer);
        L.intensity = LIntensity.GetValue(M.AnimationTime, M.Layer);
        L.range = LRange.GetValue(M.AnimationTime, M.Layer);
        L.spotAngle = LAngle.GetValue(M.AnimationTime, M.Layer) + 0.5f;
        L.innerSpotAngle = LAngle.GetValue(M.AnimationTime, M.Layer) - 0.5f;
    }
}
