using System.Collections.Generic;
using UnityEngine;

namespace PresentationEngine.Demo
{
    public class DemoRGBColorAnimation
    {
        public DemoFloatAnimation R;
        public DemoFloatAnimation G;
        public DemoFloatAnimation B;

        public Color GetValue(float Time, ushort Layer)
        {
            return new Color(R.GetValue(Time, Layer), G.GetValue(Time, Layer), B.GetValue(Time, Layer));
        }

        public DemoRGBColorAnimation(List<Color> Colors)
        {
            List<float> RValues = new List<float>();
            List<float> GValues = new List<float>();
            List<float> BValues = new List<float>();

            for (int i = 0; i < Colors.Count; i++)
            {
                RValues.Add(Colors[i].r);
                GValues.Add(Colors[i].g);
                BValues.Add(Colors[i].b);
            }

            R = new DemoFloatAnimation(RValues, 255);
            G = new DemoFloatAnimation(GValues, 255);
            B = new DemoFloatAnimation(BValues, 255);
        }
    }
    public class DemoVector3Animation
    {
        public DemoFloatAnimation X;
        public DemoFloatAnimation Y;
        public DemoFloatAnimation Z;

        public Vector3 GetValue(float Time, ushort Layer)
        {
            return new Vector3(X.GetValue(Time, Layer), Y.GetValue(Time, Layer), Z.GetValue(Time, Layer));
        }

        public DemoVector3Animation(List<Vector3> Vectors, float Start)
        {
            List<float> XVectors = new List<float>();
            List<float> YVectors = new List<float>();
            List<float> ZVectors = new List<float>();

            for (int i = 0; i < Vectors.Count; i++)
            {
                XVectors.Add(Vectors[i].x);
                YVectors.Add(Vectors[i].y);
                ZVectors.Add(Vectors[i].z);
            }

            X = new DemoFloatAnimation(XVectors, Start);
            Y = new DemoFloatAnimation(YVectors, Start);
            Z = new DemoFloatAnimation(ZVectors, Start);
        }
    }
    public class DemoFloatAnimation
    {
        public List<float> Targets { get; }
        private AnimationCurve CurrentCurve;
        private float CurrentValue;
        private ushort LastLayer = ushort.MaxValue;

        public float GetValue(float Time, ushort Layer)
        {
            if (Layer != LastLayer)
            {
                ChangeLayer(Layer);
                LastLayer = Layer;
            }
            CurrentValue = CurrentCurve.Evaluate(Time);
            return CurrentValue;
        }
        private void ChangeLayer(ushort Layer)
        {
            CurrentCurve = new AnimationCurve();
            float Target;
            if (Targets.Count > Layer)
            {
                Target = Targets[Layer];
            }
            else
            {
                Target = Targets[Targets.Count - 1];
            }
            CurrentCurve = AnimationCurve.EaseInOut(0, CurrentValue, 1, Target);
        }

        public DemoFloatAnimation(List<float> targets, float Start)
        {
            Targets = targets;
            if (Targets.Count == 0)
            {
                Targets.Add(Start);
            }
            CurrentCurve = new AnimationCurve();
            CurrentCurve.AddKey(0, Targets[0]);
            CurrentValue = Start;
        }
    }
}