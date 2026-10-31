using PresentationEngine.Demo;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class AnimatedTransform : MonoBehaviour
{
    public List<Vector3> Position = new List<Vector3>();
    public List<Vector3> Rotation = new List<Vector3>();
    public List<Vector3> Scale = new List<Vector3>();

    public bool Apply = false;

    private DemoVector3Animation Pos;
    private DemoVector3Animation Rot;
    private DemoVector3Animation Sca;

    private Main M;

    void Start()
    {
        M = GameObject.Find("Main").GetComponent<Main>();

        Pos = new DemoVector3Animation(Position, 0);
        Rot = new DemoVector3Animation(Rotation, 0);
        Sca = new DemoVector3Animation(Scale, 1);
    }

    void Update()
    {
        if (Apply)
        {
            Start();
            Apply = false;
        }
        transform.position = Pos.GetValue(M.AnimationTime, M.Layer);
        transform.rotation = Quaternion.Euler(Rot.GetValue(M.AnimationTime, M.Layer));
        transform.localScale = Sca.GetValue(M.AnimationTime, M.Layer);
    }
}
