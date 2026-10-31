using PresentationEngine.Animations;
using PresentationEngine.Assets;
using PresentationEngine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PresentationEngine
{
    public class Presentation
    {
        public bool Editor = false;
        public string Name = "";
        public ushort MaxLayer = 0;

        public List<AnimatedObject> Objects = new List<AnimatedObject>();

        public void Update()
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].Update();
            }
        }
        public void Build()
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].Build();
            }
        }
        public void CheckAll()
        {
            Check();
            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].CheckAll();
            }
        }
        public void Check()
        {

        }
        public void Create()
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].Create();
            }
        }

        public Presentation() { }
        public Presentation(string name)
        {
            Name = name;
            MaxLayer = 1;
            Editor = true;
        }
    }
    public static class ThisPresentation
    {
        public static ushort Layer { get; private set; }
        public static ushort MaxLayer { get; private set; }
        public static float PastTime { get; private set; }
        public static Presentation LoadetPresentation { get; private set; }

        public static void Create(ushort Max)
        {
            Layer = 0;
            MaxLayer = Max;
        }
        public static void Create()
        {
            Layer = 0;
            MaxLayer = 0;
        }
        public static void GoUp()
        {
            if (Layer < MaxLayer)
            {
                Layer++;
            }
        }
        public static void GoDown()
        {
            if (Layer > 0)
            {
                Layer--;
            }
        }
        public static void JumpTo(ushort L)
        {
            if (L <= MaxLayer)
            {
                Layer = L;
            }
        }
        public static void Update()
        {
            PastTime = Time.deltaTime * Time.timeScale;
            LoadetPresentation.Update();
        }
        public static void Load(Presentation P)
        {
            LoadetPresentation = P;
            Create(P.MaxLayer);
        }
        public static void New()
        {
            LoadetPresentation = new Presentation("Presentation");
        }
    }
}
namespace PresentationEngine.Objects
{
    public class AnimatedObject
    {
        public string Name;
        public GameObject Me;

        public void Create()
        {
            Me = new GameObject($"PEObject_{Name}");
            Me.tag = "Object";
        }
        public void CheckAll() { }
        public void Build() { }
        public void Update() { }
    }
    public class Object3D : AnimatedObject
    {
        public Mesh ObjectMesh = new Mesh();
        public Material ObjectMaterial;

        public TransformComponent Transform = new TransformComponent();

        public new void Create()
        {
            base.Create();
            Me.AddComponent<MeshFilter>().mesh = ObjectMesh;
            Me.AddComponent<MeshRenderer>().material = ObjectMaterial;

            if (Transform.UseCollision || Transform.UsePhysics)
            {
                Me.AddComponent<MeshCollider>().sharedMesh = ObjectMesh;
            }
        }
        public new void CheckAll()
        {
            Transform.CheckAll();
        }
        public new void Build()
        {
            Transform.Build();
        }
        public new void Update()
        {
            Transform.Update(Me);
        }
        public Object3D() { }
        public Object3D(string name)
        {
            Name = name;
        }
    }
    public class LightObject : AnimatedObject
    {
        public TransformComponent Transform = new TransformComponent();
        public CustomAnimation<Color> LightColor = new CustomAnimation<Color>(255);
        public CustomAnimation<float> LightIntensity = new CustomAnimation<float>(1);
        public CustomAnimation<float> LightRange = new CustomAnimation<float>(10);
        public float InnerSpotAngle = 20;
        public float OuterSpotAngle = 30;
        public PrimitiveLightType Type = PrimitiveLightType.Spot;
        public Texture2D Cookie = new Texture2D(0, 0);

        public new void Create()
        {
            base.Create();
            Transform.Create(Me);
            Light L = Me.AddComponent<Light>();
            if (Type == PrimitiveLightType.Spot) L.type = LightType.Spot;
            else L.type = LightType.Point;
            L.innerSpotAngle = InnerSpotAngle;
            L.spotAngle = OuterSpotAngle;
            L.shadowResolution = UnityEngine.Rendering.LightShadowResolution.FromQualitySettings;
            L.cookie = Cookie;
        }
        public new void CheckAll()
        {
            Check();
            Transform.CheckAll();
            LightColor.CheckAll();
            LightIntensity.CheckAll();
            LightRange.CheckAll();
        }
        public void Check()
        {
            if (InnerSpotAngle < 0) InnerSpotAngle = 0;
            else if (InnerSpotAngle > 180) InnerSpotAngle = 180;
            if (OuterSpotAngle < 0) OuterSpotAngle = 0;
            else if (OuterSpotAngle > 180) OuterSpotAngle = 180;
            if (InnerSpotAngle > OuterSpotAngle) InnerSpotAngle = OuterSpotAngle;
        }
        public new void Build()
        {
            Check();
            Transform.Build();
            LightColor.Build();
            LightIntensity.Build();
            LightRange.Build();
        }
        public new void Update()
        {
            Transform.Update(Me);
            Light L = Me.GetComponent<Light>();
            L.color = LightColor.GetValue();
            L.intensity = LightIntensity.GetValue();
            L.range = LightRange.GetValue();
        }

        public enum PrimitiveLightType
        {
            Point,
            Spot
        }

        public LightObject() { }
        public LightObject(string name)
        {
            Name = name;
        }
    }
    public class ParticleEmiterObject
    {

    }

    public class TransformComponent
    {
        public CustomAnimation<Vector3> Position = new CustomAnimation<Vector3>(0);
        public CustomAnimation<Vector3> Rotation = new CustomAnimation<Vector3>(0);
        public CustomAnimation<Vector3> Scale = new CustomAnimation<Vector3>(1);

        public bool UseCollision = true;
        public bool UsePhysics = false;
        public float Mass = 1;
        public float Drag = 1;
        public bool UseGravity = false;
        public float StartFriction = 1;
        public float MovingFriction = 1;
        public float Bounciness = 0.05f;

        public GameObject Create(GameObject Me)
        {
            if (UsePhysics)
            {
                Rigidbody RB = Me.AddComponent<Rigidbody>();
                RB.mass = Mass;
                RB.drag = Drag;
                RB.useGravity = UseGravity;

                PhysicMaterial Mat = new PhysicMaterial();
                Mat.staticFriction = StartFriction;
                Mat.dynamicFriction = MovingFriction;
                Mat.bounciness = Bounciness;
                Me.GetComponent<MeshCollider>().material = Mat;
            }
            return Me;
        }
        public void CheckAll()
        {
            Check();
            Position.CheckAll();
            Rotation.CheckAll();
            Scale.CheckAll();
        }
        public void Check()
        {
            if (Bounciness > 1) Bounciness = 1;
            else if (Bounciness < 0) Bounciness = 0;
            if (UsePhysics) UseCollision = true;
            if (Mass < 0) Mass = 0;
            if (StartFriction < 0) StartFriction = 0;
            if (MovingFriction < 0) MovingFriction = 0;
        }
        public void Update(GameObject Me)
        {
            if (UsePhysics)
            {
                Rigidbody RB = Me.GetComponent<Rigidbody>();
                RB.MovePosition(Position.GetValue());
                RB.MoveRotation(Quaternion.Euler(Rotation.GetValue()));
            }
            else
            {
                Me.transform.position = Position.GetValue();
                Me.transform.rotation = Quaternion.Euler(Rotation.GetValue());
            }
            Me.transform.localScale = Scale.GetValue();
        }
        public void Build()
        {
            Check();
            Position.Build();
            Rotation.Build();
            Scale.Build();
        }
    }
}
namespace PresentationEngine.Animations
{
    //Custom
    public class CustomAnimation<T> where T : new()
    {
        private FieldInfo[] Fields;
        public byte Length { get; }
        public AnimationCurve[] CurrentCurves { get; private set; }
        public float Time { get; private set; } = 0;
        private float AnimationStartTime;
        public bool Transition { get; private set; } = false;
        private ushort CurrentLayer = 0;
        private float Default { get; } = 0;

        public CustomAnimationLayer[] Layers { get; private set; }

        //Once
        public void CleanUp()
        {
            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i].CleanUp((ushort)i);
            }
        }
        public void Sort()
        {
            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i].Sort();
            }
        }
        public void Check()
        {
            if (Layers.Length != ThisPresentation.MaxLayer)
            {
                List<CustomAnimationLayer> NewLayers;
                if (Layers.Length < ThisPresentation.MaxLayer)
                {
                    NewLayers = new List<CustomAnimationLayer>();
                    for (int i = 0; i < ThisPresentation.MaxLayer; i++)
                    {
                        NewLayers.Add(Layers[i]);
                    }
                }
                else
                {
                    NewLayers = Layers.ToList();
                    for (int i = Layers.Length; i < ThisPresentation.MaxLayer; i++)
                    {
                        NewLayers.Add(new CustomAnimationLayer());
                    }
                }
                Layers = NewLayers.ToArray();
                Check();
                return;
            }
        }
        public void CheckAll()
        {
            Check();
            CleanUp();
            Sort();
        }
        public void Build()
        {
            CheckAll();

            Time = 0;
            AnimationStartTime = 0;

            CustomKeyframe FirstKey = GetFirstKeyframe();
            CustomKeyframe LastKey = GetLastKeyframe();
            if (FirstKey == null)
            {
                FirstKey = GetDefaultKeyframe(0);
                LastKey = GetDefaultKeyframe(1);
                for (ushort i = 0; i < Layers.Length; i++)
                {
                    Layers[i].ReverseAtEnd = false;
                    Layers[i].Animation.Add(FirstKey);
                    Layers[i].Animation.Add(LastKey);
                    Layers[i].Transition.Add(FirstKey);
                    Layers[i].Transition.Add(LastKey);
                }
            }
            else
            {
                for (ushort i = 0; i < Layers.Length; i++)
                {
                    List<CustomKeyframe> Keys = GetKeys(new AnimationPosition(i, true));

                    Keys = GetKeys(new AnimationPosition(i, true));
                }
            }

            BuildNextLayer();
        }
        private int GetKeyframeCount(AnimationPosition Position)
        {
            if (Position.Transition) return Layers[Position.Layer].Transition.Count;
            else return Layers[Position.Layer].Animation.Count;
        }
        private List<CustomKeyframe> GetKeys(AnimationPosition Position)
        {
            if (Position.Transition) return Layers[Position.Layer].Transition;
            else return Layers[Position.Layer].Animation;
        }
        private CustomKeyframe GetFirstKeyframe(AnimationPosition Position)
        {
            List<CustomKeyframe> Keys = GetKeys(Position);
            if (Keys.Count == 0)
            {
                throw new ArgumentOutOfRangeException("No keyframes found");
            }
            CustomKeyframe Target = new CustomKeyframe(0);
            Target.SetTime(float.MaxValue);
            for (int i = 0; i < Keys.Count; i++)
            {
                if (Target.Time > Keys[i].Time)
                {
                    Target = Keys[i];
                }
            }
            return Target;
        }
        private CustomKeyframe GetLastKeyframe(AnimationPosition Position)
        {
            List<CustomKeyframe> Keys = GetKeys(Position);
            if (Keys.Count == 0)
            {
                throw new ArgumentOutOfRangeException("No keyframes found");
            }
            CustomKeyframe Target = new CustomKeyframe(0);
            Target.SetTime(float.MinValue);
            for (int i = 0; i < Keys.Count; i++)
            {
                if (Target.Time < Keys[i].Time)
                {
                    Target = Keys[i];
                }
            }
            return Target;
        }
        private int GetKeyframeCount()
        {
            int Count = 0;
            for (ushort i = 0; i < Layers.Length; i++)
            {
                Count += GetKeyframeCount(new AnimationPosition(i, false));
                Count += GetKeyframeCount(new AnimationPosition(i, true));
            }
            return Count;
        }
        private List<CustomKeyframe> GetKeys()
        {
            List<CustomKeyframe> Keys = new List<CustomKeyframe>();
            for (ushort i = 0; i < Layers.Length; i++)
            {
                Keys.AddRange(Layers[i].Animation);
                Keys.AddRange(Layers[i].Transition);
            }
            return Keys;
        }
        private CustomKeyframe GetFirstKeyframe()
        {
            for (int i = 0; i < Layers.Length; i++)
            {
                if (Layers[i].Transition.Count > 0)
                {
                    return Layers[i].Transition[0];
                }
                else if (Layers[i].Animation.Count > 0)
                {
                    return Layers[i].Animation[0];
                }
            }
            return null;
        }
        private CustomKeyframe GetLastKeyframe()
        {
            for (int i = Layers.Length - 1; i > 0; i--)
            {
                if (Layers[i].Transition.Count > 0)
                {
                    return Layers[i].Transition[Layers[i].Transition.Count - 1];
                }
                else if (Layers[i].Animation.Count > 0)
                {
                    return Layers[i].Animation[Layers[i].Animation.Count - 1];
                }
            }
            return null;
        }
        private CustomKeyframe GetDefaultKeyframe(float Time)
        {
            CustomKeyframe Key = new CustomKeyframe(Length);
            for (byte i = 0; i < Length; i++)
            {
                Key.Values[i] = new CustomKeyframeValue(new Keyframe(0, Default));
            }
            Key.SetTime(Time);
            return Key;
        }

        //Runtime
        public T GetValue()
        {
            float[] Values = GetValues();
            if (typeof(T) == typeof(bool))
            {
                bool Tmp = Values[0] > 0;
                return (T)(object)Tmp;
            }
            else if (typeof(T) == typeof(Vector2))
            {
                Vector2 Tmp = new Vector2(Values[0], Values[1]);
                return (T)(object)Tmp;
            }
            else if (typeof(T) == typeof(Vector3))
            {
                Vector3 Tmp = new Vector3(Values[0], Values[1], Values[2]);
                return (T)(object)Tmp;
            }
            else if (typeof(T) == typeof(Color))
            {
                Color Tmp = new Color(Values[0], Values[1], Values[2]);
                return (T)(object)Tmp;
            }
            else if (IsPrimitiveNumberType(typeof(T)))
            {
                return (T)Convert.ChangeType(Values[0], typeof(T));
            }
            return new T();
        }
        private float[] GetValues()
        {
            TryChangeLayer();
            CheckTime();
            Time += ThisPresentation.PastTime;
            float[] Output = new float[Length];
            for (int i = 0; i < Length; i++)
            {
                Output[i] = CurrentCurves[i].Evaluate(Time);
            }
            return Output;
        }
        public float[] GetValuesAsync()
        {
            float[] Values = new float[Length];
            for (int i = 0; i < Length; i++)
            {
                Values[i] = CurrentCurves[i].Evaluate(Time);
            }
            return Values;
        }
        private void CheckTime()
        {
            if (Transition && Time >= AnimationStartTime)
            {
                Time = 0;
                Transition = false;
                BuildNextLayer();
            }
        }
        private void BuildNextLayer()
        {
            float[] Values = new float[0];
            if (Transition)
            {
                Values = GetValuesAsync();
            }

            CustomKeyframe[] Keys;
            CurrentCurves = new AnimationCurve[Length];
            WrapMode EndWrapMode = WrapMode.Clamp;
            {
                if (Transition) Keys = Layers[ThisPresentation.Layer].Transition.ToArray();
                else
                {
                    Keys = Layers[ThisPresentation.Layer].Animation.ToArray();
                    if (Layers[ThisPresentation.Layer].ReverseAtEnd) EndWrapMode = WrapMode.PingPong;
                    else EndWrapMode = WrapMode.Loop;
                }
            }

            for (int i = 0; i < Length; i++)
            {
                CurrentCurves[i] = new AnimationCurve();
                CurrentCurves[i].preWrapMode = WrapMode.Clamp;
                CurrentCurves[i].postWrapMode = EndWrapMode;

                for (int i2 = 0; i2 < Keys.Length; i2++)
                {
                    Keyframe Tmp = Keys[i2].GetKeyframe((byte)i);
                    if (Tmp.value != float.NaN)
                    {
                        CurrentCurves[i].AddKey(Tmp);
                    }
                }
            }

            if (Transition)
            {
                AnimationStartTime = GetAnimationStartTime();
                for (int i = 0; i < Length; i++)
                {
                    CurrentCurves[i].AddKey(0, Values[i]);
                }
            }
        }
        private void TryChangeLayer()
        {
            if (CurrentLayer != ThisPresentation.Layer)
            {
                CurrentLayer = ThisPresentation.Layer;
                Transition = true;
                Time = 0;
                BuildNextLayer();
            }
        }
        private float GetAnimationStartTime()
        {
            float LastTime = 0;
            for (int i = 0; i < Layers[ThisPresentation.Layer].Transition.Count; i++)
            {
                if (Layers[ThisPresentation.Layer].Transition[i].Time > LastTime)
                {
                    LastTime = Layers[ThisPresentation.Layer].Transition[i].Time;
                }
            }
            return LastTime;
        }

        //Editor
        public AnimationCurve[] GetCurves(AnimationPosition Position)
        {
            CustomKeyframe[] Keys;
            AnimationCurve[] Output = new AnimationCurve[Length];
            WrapMode EndWrapMode = WrapMode.Clamp;
            {
                CustomAnimation<T> Copy = new CustomAnimation<T>(Default);
                Copy.Layers = Layers;
                Copy.Build();
                if (Position.Transition) Keys = Copy.Layers[Position.Layer].Transition.ToArray();
                else
                {
                    Keys = Copy.Layers[Position.Layer].Animation.ToArray();
                    if (Copy.Layers[Position.Layer].ReverseAtEnd) EndWrapMode = WrapMode.PingPong;
                    else EndWrapMode = WrapMode.Loop;
                }
            }

            for (int i = 0; i < Length; i++)
            {
                Output[i] = new AnimationCurve();
                Output[i].preWrapMode = WrapMode.Clamp;
                Output[i].postWrapMode = EndWrapMode;

                for (int i2 = 0; i2 < Keys.Length; i2++)
                {
                    Keyframe Tmp = Keys[i2].GetKeyframe((byte)i);
                    if (Tmp.value != float.NaN)
                    {
                        Output[i].AddKey(Tmp);
                    }
                }
            }

            return Output;
        }
        public void SetKeyframe(AnimationPosition Position, CustomKeyframe Key)
        {
            int Index = GetKeyframeIndex(Position, Key.Time);
            if (Index == -1)
            {
                if (Position.Transition) Layers[Position.Layer].Transition.Add(Key);
                else Layers[Position.Layer].Animation.Add(Key);
            }
            else
            {
                if (Position.Transition)
                {
                    Layers[Position.Layer].Transition[Index].SetTime(Key.Time);
                    for (byte i = 0; i < Key.Values.Length; i++)
                    {
                        Layers[Position.Layer].Transition[Index].SetKeyframeValue(i, Key.Values[i]);
                    }
                }
            }
        }
        public void SetKeyframe(AnimationPosition Position, T Value, float Time)
        {
            Keyframe[] Keys = new Keyframe[Length];
            for (int i = 0; i < Length; i++)
            {
                Keys[i] = new Keyframe(0, (float)Convert.ChangeType(Fields[i].GetValue(Value), typeof(float)), 0, 0, 0.5f, 0.5f);
            }
            CustomKeyframe Key = new CustomKeyframe(Keys);
            Key.SetTime(Time);
            SetKeyframe(Position, Key);
        }
        public void RemoveKeyframe(AnimationPosition Position, int Index)
        {
            if (Position.Transition) Layers[Position.Layer].Transition.RemoveAt(Index);
            else Layers[Position.Layer].Animation.RemoveAt(Index);
        }
        public void RemoveKeyframe(AnimationPosition Position, int Index, byte KeyIndex)
        {
            if (Position.Transition) Layers[Position.Layer].Transition[Index].RemoveKeyframeValue(KeyIndex);
            else Layers[Position.Layer].Animation[Index].RemoveKeyframeValue(KeyIndex);
        }
        public void SetTime(AnimationPosition Position, int Index, float Time)
        {
            if (Position.Transition) Layers[Position.Layer].Transition[Index].SetTime(Time);
            else Layers[Position.Layer].Animation[Index].SetTime(Time);
        }
        private int GetKeyframeIndex(AnimationPosition Position, float Time)
        {
            List<CustomKeyframe> Keys;
            if (Position.Transition) Keys = Layers[Position.Layer].Transition;
            else Keys = Layers[Position.Layer].Animation;

            for (int i = 0; i < Keys.Count; i++)
            {
                if (Keys[i].Time == Time)
                {
                    return i;
                }
            }
            return -1;
        }
        public void AddLayer()
        {
            Check();
        }
        public void RemoveLayer()
        {
            Check();
        }
        public void RemoveLayer(ushort Layer)
        {
            List<CustomAnimationLayer> TmpLayers = Layers.ToList();
            TmpLayers.RemoveAt(Layer);
            Layers = TmpLayers.ToArray();
            Check();
        }
        public void ReplaceLayer(ushort LayerA, ushort LayerB)
        {
            CustomAnimationLayer TmpB = Layers[LayerA];
            Layers[LayerA] = Layers[LayerB];
            Layers[LayerB] = TmpB;
            Check();
        }
        public void MoveLayer(ushort Layer, ushort NewPosition)
        {
            if (NewPosition != Layer)
            {
                CustomAnimationLayer L = Layers[Layer];
                List<CustomAnimationLayer> NewLayers = Layers.ToList();
                if (NewPosition > Layer)
                {
                    NewLayers.Insert(NewPosition, L);
                    NewLayers.RemoveAt(Layer);
                }
                else
                {
                    NewLayers.RemoveAt(Layer);
                    NewLayers.Insert(NewPosition, L);
                }
                Layers = NewLayers.ToArray();
            }
            Check();
        }

        public CustomAnimation(float DefaultValue)
        {
            if (typeof(T) == typeof(bool))
            {
                Fields = new FieldInfo[0];
                Length = 1;
            }
            else
            {
                Fields = GetFields();
                if (Fields.Length > 255)
                {
                    throw new IndexOutOfRangeException("To many values to handle (Max 255)");
                }
                Length = (byte)Fields.Length;
            }

            Layers = new CustomAnimationLayer[ThisPresentation.MaxLayer];
            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i] = new CustomAnimationLayer();
            }
            Default = DefaultValue;
        }

        private FieldInfo[] GetFields()
        {
            FieldInfo[] Originals = typeof(T).GetFields();
            List<FieldInfo> Output = new List<FieldInfo>();
            if (typeof(T) == typeof(Vector3) ||
                typeof(T) == typeof(Vector2) ||
                typeof(T) == typeof(Vector3Int) ||
                typeof(T) == typeof(Vector2Int))
            {
                for (int i = 0; i < Originals.Length; i++)
                {
                    if (Originals[i].Name == "x" || Originals[i].Name == "y" || Originals[i].Name == "z")
                    {
                        Output.Add(Originals[i]);
                    }
                }
            }
            else if (typeof(T) == typeof(Color) ||
                     typeof(T) == typeof(Color32))
            {
                for (int i = 0; i < Originals.Length; i++)
                {
                    if (Originals[i].Name == "r" || Originals[i].Name == "g" || Originals[i].Name == "b")
                    {
                        Output.Add(Originals[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < Originals.Length; i++)
                {
                    if (IsPrimitiveNumberType(Originals[i].FieldType))
                    {
                        Output.Add(Originals[i]);
                    }
                }
            }
            return Output.ToArray();
        }
        private bool IsPrimitiveNumberType(Type T)
        {
            return T == typeof(float) ||
                T == typeof(double) ||
                T == typeof(ushort) ||
                T == typeof(short) ||
                T == typeof(uint) ||
                T == typeof(int) ||
                T == typeof(ulong) ||
                T == typeof(long) ||
                T == typeof(byte);
        }
    }
    public class CustomAnimationLayer
    {
        public bool ReverseAtEnd = true;
        public List<CustomKeyframe> Animation = new List<CustomKeyframe>();
        public List<CustomKeyframe> Transition = new List<CustomKeyframe>();

        public void Sort()
        {
            Animation = Sort(Animation);
            Transition = Sort(Transition);
        }
        private static List<CustomKeyframe> Sort(List<CustomKeyframe> Keys)
        {
            List<CustomKeyframe> Output = new List<CustomKeyframe>();
            int Count = Keys.Count;
            int SmallestIndex;
            for (int i = 0; i < Count; i++)
            {
                SmallestIndex = 0;
                for (int i2 = 0; i2 < Keys.Count; i2++)
                {
                    if (Keys[SmallestIndex].Time > Keys[i2].Time)
                    {
                        SmallestIndex = i2;
                    }
                }
                Output.Add(Keys[SmallestIndex]);
                Keys.RemoveAt(SmallestIndex);
            }
            return Output;
        }
        public void CleanUp(ushort Layer)
        {
            if (Layer == 0)
            {
                Transition.Clear();
            }
            Animation = CleanUp(Animation);
            Transition = CleanUp(Transition);
            while (true)
            {
                bool AllChecked = true;
                for (int i = 0; i < Transition.Count; i++)
                {
                    if (Transition[i].Time == 0)
                    {
                        Transition.RemoveAt(i);
                        AllChecked = false;
                        break;
                    }
                }

                if (AllChecked)
                {
                    break;
                }
            }
        }
        private static List<CustomKeyframe> CleanUp(List<CustomKeyframe> Keys)
        {
            List<CustomKeyframe> Output = new List<CustomKeyframe>();
            List<float> Times = new List<float>();
            for (int i = 0; i < Keys.Count; i++)
            {
                if (Keys[i].Time > 0)
                {
                    bool MoreThanOne = false;
                    for (int i2 = 0; i2 < Times.Count; i2++)
                    {
                        if (Keys[i].Time == Times[i2])
                        {
                            MoreThanOne = true;
                            break;
                        }
                    }

                    if (!MoreThanOne)
                    {
                        Times.Add(Keys[i].Time);
                        bool Empty = true;
                        for (int i2 = 0; i2 < Keys[i].Values.Length; i2++)
                        {
                            if (Keys[i].Values[i2].Value != float.NaN)
                            {
                                Empty = false;
                                break;
                            }
                        }
                        if (!Empty)
                        {
                            Output.Add(Keys[i]);
                        }
                    }
                }
            }
            return Output;
        }
    }
    public class CustomKeyframe
    {
        public float Time { get; private set; }
        public CustomKeyframeValue[] Values { get; private set; }

        public CustomKeyframe(byte Lenght)
        {
            Values = new CustomKeyframeValue[Lenght];
            for (int i = 0; i < Lenght; i++)
            {
                Values[i] = new CustomKeyframeValue();
            }
        }
        public CustomKeyframe(byte Lenght, float time)
        {
            Values = new CustomKeyframeValue[Lenght];
            for (int i = 0; i < Lenght; i++)
            {
                Values[i] = new CustomKeyframeValue();
            }
            Time = time;
        }

        public CustomKeyframe(Keyframe[] Keys)
        {
            Values = new CustomKeyframeValue[Keys.Length];
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] = new CustomKeyframeValue(Keys[i]);
            }
        }
        public CustomKeyframe(Keyframe[] Keys, float time)
        {
            Values = new CustomKeyframeValue[Keys.Length];
            for (int i = 0; i < Keys.Length; i++)
            {
                Values[i] = new CustomKeyframeValue(Keys[i]);
            }
            Time = time;
        }

        public void SetKeyframeValue(byte Index, CustomKeyframeValue Key)
        {
            Values[Index].Overwrite(Key);
        }
        public Keyframe GetKeyframe(byte Index)
        {
            return new Keyframe(
                Time,
                Values[Index].Value,
                Values[Index].InTangent,
                Values[Index].OutTangent,
                Values[Index].InWeight,
                Values[Index].OutWeight
                );
        }
        public void RemoveKeyframeValue(byte Index)
        {
            Values[Index] = new CustomKeyframeValue();
        }
        public void SetTime(float time)
        {
            Time = time;
        }
    }
    public class CustomKeyframeValue
    {
        public float Value;
        public float InTangent;
        public float OutTangent;
        public float InWeight;
        public float OutWeight;

        public CustomKeyframeValue()
        {
            Value = float.NaN;
            InTangent = 0;
            OutTangent = 0;
            InWeight = 0;
            OutWeight = 0;
        }
        public CustomKeyframeValue(Keyframe Key)
        {
            Value = Key.value;
            InTangent = Key.inTangent;
            OutTangent = Key.outTangent;
            InWeight = Key.inWeight;
            OutWeight = Key.outWeight;
        }
        public CustomKeyframeValue(float value, float inTangent, float outTangent, float inWeight, float outWeight)
        {
            Value = value;
            InTangent = inTangent;
            OutTangent = outTangent;
            InWeight = inWeight;
            OutWeight = outWeight;
        }

        public void Overwrite(CustomKeyframeValue Key)
        {
            if (Key.Value != float.NaN)
            {
                Value = Key.Value;
                InTangent = Key.InTangent;
                OutTangent = Key.OutTangent;
                InWeight = Key.InWeight;
                OutWeight = Key.OutWeight;
            }
        }
    }
    public struct AnimationPosition
    {
        public ushort Layer { get; }
        public bool Transition { get; }

        public AnimationPosition(ushort layer, bool transition)
        {
            Layer = layer;
            Transition = transition;
        }
    }
}
namespace PresentationEngine.Converter
{
    public static class PEConverter
    {
        public static byte[] ToBytes(ushort Data)
        {
            return BitConverter.GetBytes(Data);
        }
        public static byte[] ToBytes(short Data)
        {
            return BitConverter.GetBytes(Data);
        }
        public static byte[] ToBytes(uint Data)
        {
            return BitConverter.GetBytes(Data);
        }
        public static byte[] ToBytes(int Data)
        {
            return BitConverter.GetBytes(Data);
        }
        public static byte[] ToBytes(ulong Data)
        {
            return BitConverter.GetBytes(Data);
        }
        public static byte[] ToBytes(long Data)
        {
            return BitConverter.GetBytes(Data);
        }
        public static byte[] ToBytes(float Data)
        {
            return BitConverter.GetBytes(Data);
        }
        public static byte[] ToBytes(double Data)
        {
            return BitConverter.GetBytes(Data);
        }
        public static byte[] ToBytes(byte Data)
        {
            return new byte[] { Data };
        }
        public static byte[] ToBytes(char Data)
        {
            return new byte[] { BitConverter.GetBytes(Data)[0] };
        }
        public static byte[] ToBytes(bool Data)
        {
            if (Data) return new byte[] { 1 };
            else return new byte[] { 0 };
        }
        public static byte[] ToBytes(string Data)
        {
            List<byte> Output = new List<byte>();
            Output.AddRange(ToBytes(Data.Length));
            for (int i = 0; i < Data.Length; i++)
            {
                Output.AddRange(ToBytes(Data[i]));
            }
            return Output.ToArray();
        }
        public static byte[] ToBytes(CustomKeyframeValue Data)
        {
            List<byte> Output = new List<byte>();
            Output.AddRange(ToBytes(Data.Value));
            Output.AddRange(ToBytes(Data.InTangent));
            Output.AddRange(ToBytes(Data.OutTangent));
            Output.AddRange(ToBytes(Data.InWeight));
            Output.AddRange(ToBytes(Data.OutWeight));
            return Output.ToArray();
        }
        public static byte[] ToBytes(CustomKeyframe Data)
        {
            List<byte> Output = new List<byte>();
            Output.AddRange(ToBytes(Data.Time));
            for (int i = 0; i < Data.Values.Length; i++)
            {
                Output.AddRange(ToBytes(Data.Values[i]));
            }
            return Output.ToArray();
        }
        public static byte[] ToBytes(CustomAnimationLayer Data)
        {
            Data.Sort();
            List<byte> Output = new List<byte>();
            Output.AddRange(ToBytes(Data.ReverseAtEnd));
            Output.AddRange(ToBytes(Data.Animation.Count));
            for (int i = 0; i < Data.Animation.Count; i++)
            {
                Output.AddRange(ToBytes(Data.Animation[i]));
            }

            Output.AddRange(ToBytes(Data.Transition.Count));
            for (int i = 0; i < Data.Animation.Count; i++)
            {
                Output.AddRange(ToBytes(Data.Transition[i]));
            }
            return Output.ToArray();
        }
        public static byte[] ToBytes<T>(CustomAnimation<T> Data) where T : new()
        {
            Data.Check();
            Data.CleanUp();
            Data.Sort();
            List<byte> Output = new List<byte>();
            for (int i = 0; i < Data.Layers.Length; i++)
            {
                Output.AddRange(ToBytes(Data.Layers[i]));
            }
            return Output.ToArray();
        }

        public static ushort ToUShort(byte[] Data)
        {
            return BitConverter.ToUInt16(Data);
        }
        public static short ToShort(byte[] Data)
        {
            return BitConverter.ToInt16(Data);
        }
        public static uint ToUInt(byte[] Data)
        {
            return BitConverter.ToUInt32(Data);
        }
        public static int ToInt(byte[] Data)
        {
            return BitConverter.ToInt32(Data);
        }
        public static ulong ToULong(byte[] Data)
        {
            return BitConverter.ToUInt64(Data);
        }
        public static long ToLong(byte[] Data)
        {
            return BitConverter.ToInt64(Data);
        }
        public static float ToFloat(byte[] Data)
        {
            return BitConverter.ToSingle(Data);
        }
        public static double ToDouble(byte[] Data)
        {
            return BitConverter.ToDouble(Data);
        }
        public static byte ToByte(byte[] Data)
        {
            return Data[0];
        }
        public static char ToChar(byte[] Data)
        {
            return BitConverter.ToChar(Data);
        }
        public static bool ToBool(byte[] Data)
        {
            if (Data[0] == 1) return true;
            else if (Data[0] == 0) return false;
            else throw new ArgumentException();
        }
        public static string ToString(byte[] Data)
        {
            string Output = "";
            for (int i = 4; i < Data.Length; i++)
            {
                Output += ToChar(new byte[] { Data[i] });
            }
            return Output;
        }
        public static CustomKeyframeValue ToCustomKeyframeValue(byte[] Data)
        {
            CustomKeyframeValue Value = new CustomKeyframeValue();
            Value.Value = ToFloat(GetBytes(Data, 0, 4));
            Value.InTangent = ToFloat(GetBytes(Data, 4, 4));
            Value.OutTangent = ToFloat(GetBytes(Data, 8, 4));
            Value.InWeight = ToFloat(GetBytes(Data, 12, 4));
            Value.OutWeight = ToFloat(GetBytes(Data, 16, 4));
            return Value;
        }
        public static CustomKeyframe ToCustomKeyframe(byte[] Data, byte KeyLenght)
        {
            CustomKeyframe Key = new CustomKeyframe(KeyLenght);
            Key.SetTime(ToFloat(Data));
            Data.Trim(4);
            for (int i = 0; i < KeyLenght; i++)
            {
                Key.Values[i].Value = ToFloat(Data);
                Data.Trim(4);
                Key.Values[i].InTangent = ToFloat(Data);
                Data.Trim(4);
                Key.Values[i].OutTangent = ToFloat(Data);
                Data.Trim(4);
                Key.Values[i].InWeight = ToFloat(Data);
                Data.Trim(4);
                Key.Values[i].OutWeight = ToFloat(Data);
                Data.Trim(4);
            }
            return Key;
        }
        public static CustomAnimationLayer ToCustomAnimationLayer(byte[] Data, byte Lenght)
        {
            CustomAnimationLayer Layer = new CustomAnimationLayer();
            Layer.ReverseAtEnd = ToBool(Data);
            Data.Trim(1);
            int ListLenght = ToInt(Data);
            Data.Trim(4);
            for (int i = 0; i < ListLenght; i++)
            {
                Layer.Animation.Add(ToCustomKeyframe(Data, Lenght));
                Data.Trim(Lenght * 20);
            }

            ListLenght = ToInt(Data);
            Data.Trim(4);
            for (int i = 0; i < ListLenght; i++)
            {
                Layer.Transition.Add(ToCustomKeyframe(Data, Lenght));
                Data.Trim(Lenght * 20);
            }
            return Layer;
        }
        public static CustomAnimation<T> ToCustomAnimation<T>(byte[] Data, float DefaultValue) where T : new()
        {
            CustomAnimation<T> Animation = new CustomAnimation<T>(DefaultValue);
            for (int i = 0; i < ThisPresentation.MaxLayer; i++)
            {
                Animation.Layers[i] = ToCustomAnimationLayer(Data, Animation.Length);
                Data.Trim((Animation.Layers[i].Animation.Count + Animation.Layers[i].Animation.Count) * (4 + Animation.Length * 20));
            }
            return Animation;
        }

        public static byte[] GetBytes(byte[] Input, int Ofset, int Lenght)
        {
            byte[] Output = new byte[Lenght];
            int Position = 0;
            for (int i = Ofset; i < Ofset + Lenght; i++)
            {
                Output[Position] = Input[i];
                Position++;
            }
            return Output;
        }
        public static void Trim<T>(this T[] Input, int Lenght)
        {
            if (Lenght > Input.Length)
            {
                List<T> Data = Input.ToList();
                for (int i = 0; i < Lenght; i++)
                {
                    Data.RemoveAt(0);
                }
                Input = Data.ToArray();
            }
            else Input = new T[0];
        }
    }
}
namespace PresentationEngine.Assets
{
    public class SkyboxAsset
    {

    }
}