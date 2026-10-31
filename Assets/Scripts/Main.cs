using UnityEngine;

[ExecuteAlways]
public class Main : MonoBehaviour
{
    public ushort Layer = 0;
    public ushort MaxLayer = 10;
    public float TimeSpeed = 0.25f;
    [HideInInspector()]
    public float AnimationTime = 0;

    public bool LayerUp;
    public bool LayerDown;

    void Awake()
    {
        Layer = 0;
        AnimationTime = 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Space))
        {
            LayerUp = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LayerDown = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Space) || LayerUp)
        {
            if (Layer < MaxLayer)
            {
                Layer++;
                AnimationTime = 0;
            }
            LayerUp = false;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || LayerDown)
        {
            if (Layer > 0)
            {
                Layer--;
                AnimationTime = 0;
            }
            LayerDown = false;
        }

        if (Layer > MaxLayer)
        {
            Layer = MaxLayer;
        }

        if (AnimationTime != 1)
        {
            if (AnimationTime < 1)
            {
                AnimationTime += Time.deltaTime * Time.timeScale * TimeSpeed;
            }
            else
            {
                AnimationTime = 1;
            }
        }
    }
}
