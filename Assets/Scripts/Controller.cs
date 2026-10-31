using PresentationEngine;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public ushort Layer = 0;
    public ushort MaxLayer = 10;

    public bool Up = false;
    public bool Down = false;

    void Awake()
    {
        Layer = 0;
        ThisPresentation.Create(MaxLayer);
    }

    void Update()
    {
        ThisPresentation.Update();
        
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Space) || Up)
        {
            ThisPresentation.GoUp();
            Up = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Down)
        {
            ThisPresentation.GoDown();
            Down = false;
        }

        Layer = ThisPresentation.Layer;
        MaxLayer = ThisPresentation.MaxLayer;
    }
}
