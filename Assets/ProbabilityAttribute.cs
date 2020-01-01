using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.Utilities;

#if UNITY_EDITOR

using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

#endif


public sealed class ProbabilityAttribute : Attribute
{


    public string ColorName;
    public string DataMember;


    public ProbabilityAttribute(string dataMember, string colors = "Fall")
    {
        this.DataMember = dataMember;
        this.ColorName = colors;
    }
}
#if UNITY_EDITOR


public sealed class ProbabilityAttributeDrawer : OdinAttributeDrawer<ProbabilityAttribute, float[]>
{
    string errorMessage;
    InspectorPropertyValueGetter<object[]> baseItemDataGetter;

    State state;
    Vector2 selectSzie = new Vector2(30, 30);
    //設定點 Output用
    List<float> selectValues = new List<float>();

    List<Rect> ranges = new List<Rect>();

    //選擇第幾個Select
    int selectId = -1;
    Color[] colors;


    float[] SetData<T>(T[] datas, ref List<float> select, float[] value)
    {

        if (datas == null || datas.Length == 0)
        {
            select.Clear();
            ranges.Clear();
            return new float[0];

        }

        if (datas.Length == 1)
        {
            select.Clear();
            ranges.Clear();
            for (int i = 0; i < datas.Length; i++)
            {
                ranges.Add(new Rect());
            }
            return new float[0];

        }

        //datas改變大小
        if (datas.Length - 1 != value.Length)
        {
            select.Clear();
            for (int i = 0; i < datas.Length; i++)
            {
                if (i != datas.Length - 1)
                {
                    select.Add(1 / (float)datas.Length * (i + 1));
                }
            }
            ranges.Clear();
            for (int i = 0; i < datas.Length; i++)
            {
                ranges.Add(new Rect());
            }


        }
        else if (datas.Length - 1 != select.Count)
        {
            //重新算區塊
            select.Clear();
            for (int i = 0; i < datas.Length; i++)
            {
                if (i != datas.Length - 1)
                {
                    select.Add(value[i]);
                }
            }
            ranges.Clear();
            for (int i = 0; i < datas.Length; i++)
            {
                ranges.Add(new Rect());
            }

        }
        return select.ToArray();
    }

    protected override void Initialize()
    {
        if (this.Attribute.DataMember != null)
        {
            this.baseItemDataGetter = new InspectorPropertyValueGetter<object[]>(this.Property, this.Attribute.DataMember);
            if (this.errorMessage != null)
            {
                this.errorMessage = this.baseItemDataGetter.ErrorMessage;
            }
        }

        if (baseItemDataGetter.GetValue() != null && baseItemDataGetter.GetValue().Length != 0)
        {
            this.ValueEntry.SmartValue = SetData(baseItemDataGetter.GetValue(), ref selectValues, ValueEntry.SmartValue);
        }


        for (int i = 0; i < ColorPaletteManager.Instance.ColorPalettes.Count; i++)
        {
            if (ColorPaletteManager.Instance.ColorPalettes[i].Name == Attribute.ColorName)
            {
                colors = ColorPaletteManager.Instance.ColorPalettes[i].Colors.ToArray();
                break;
            }
        }
    }


    protected override void DrawPropertyLayout(GUIContent label)
    {
        DrawAll(baseItemDataGetter);
    }


    void DrawAll<T>(InspectorPropertyValueGetter<T[]> inspectorPropertyValueGetter)
    {
        if (inspectorPropertyValueGetter.GetValue() != null && inspectorPropertyValueGetter.GetValue().Length != 0)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 40);
            DrawRange(rect, baseItemDataGetter);
            DrawSelect(rect, baseItemDataGetter);
        }
    }

    void DrawRange<T>(Rect rect, InspectorPropertyValueGetter<T[]> inspectorPropertyValueGetter)
    {

        if (inspectorPropertyValueGetter.GetValue() != null && inspectorPropertyValueGetter.GetValue().Length != 0)
        {
            SirenixEditorGUI.DrawSolidRect(rect, new Color(0.7f, 0.7f, 0.7f, 1f));
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.UpperCenter;
            GUIStyle percentageStyle = new GUIStyle();
            percentageStyle.alignment = TextAnchor.LowerCenter;

            this.ValueEntry.SmartValue = SetData(inspectorPropertyValueGetter.GetValue(), ref selectValues, ValueEntry.SmartValue);

            T[] datas = inspectorPropertyValueGetter.GetValue();
            GameObject[] go = datas as GameObject[];


            for (int i = 0; i < ranges.Count; i++)
            {
                if (ranges.Count == 1)
                {
                    ranges[i] = rect.SetXMin(rect.xMin).SetXMax(rect.xMax);
                    SirenixEditorGUI.DrawSolidRect(ranges[i], colors[i]);
                }
                else
                {
                    if (i == 0)
                    {
                        ranges[i] = rect.SetXMin(rect.xMin).SetXMax(rect.width * selectValues[i] + (selectSzie.x / 2));
                        SirenixEditorGUI.DrawSolidRect(ranges[i], colors[i]);
                    }
                    else if (i == ranges.Count - 1)
                    {
                        ranges[i] = rect.SetXMin(rect.width * selectValues[i - 1] + (selectSzie.x / 2));
                        SirenixEditorGUI.DrawSolidRect(ranges[i], colors[i]);
                    }
                    else
                    {
                        ranges[i] = rect.SetXMin(rect.width * selectValues[i - 1] + (selectSzie.x / 2)).SetXMax(rect.width * selectValues[i] + (selectSzie.x / 2));
                        SirenixEditorGUI.DrawSolidRect(ranges[i], colors[i]);
                    }
                    SirenixEditorGUI.DrawSolidRect(ranges[i], colors[i]);
                }

                GUIHelper.PushColor(Color.black);
                if (inspectorPropertyValueGetter.GetValue()[i] != null)
                {
                    if (go != null)
                    {
                        if (go[i] != null)
                        {
                            GUI.Label(ranges[i].AlignCenterX(ranges[i].width / 2), go[i].name, style);
                        }
                    }
                    else {
                        GUI.Label(ranges[i].AlignCenterX(ranges[i].width / 2), inspectorPropertyValueGetter.GetValue()[i].ToString(), style);
                    }
                }
                float percentage = (ranges[i].width / rect.width) * 100;

                GUI.Label(ranges[i].AlignCenterX(ranges[i].width / 2), Mathf.Round(percentage) + "%", percentageStyle);
                GUIHelper.PopColor();
            }


        }
    }
    void DrawSelect<T>(Rect rect, InspectorPropertyValueGetter<T[]> inspectorPropertyValueGetter)
    {
        if (inspectorPropertyValueGetter.GetValue() != null && inspectorPropertyValueGetter.GetValue().Length != 0)
        {
            Event currentEvent = Event.current;
            Rect[] selectPoint = new Rect[selectValues.Count];
            for (int i = 0; i < selectPoint.Length; i++)
            {
                selectPoint[i] = new Rect(rect.width * selectValues[i], rect.y, selectSzie.x, selectSzie.y);
                GUI.Label(selectPoint[i], EditorIcons.Eject.Raw);
            }
            //判定點到第幾個Select
            for (int i = 0; i < selectPoint.Length; i++)
            {
                if (selectPoint[i].Contains(currentEvent.mousePosition))
                {
                    if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                    {

                        ChangeState(State.Down);
                        selectId = i;
                    }
                }

            }
            if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
            {
                ChangeState(State.Up);
                selectId = -1;
            }
            ///////////////////Mouse Event/////////////////////
            switch (state)
            {
                case State.Down:
                    if (currentEvent.type == EventType.MouseDrag && currentEvent.button == 0)
                    {
                        ChangeState(State.Drag);
                    }
                    break;
                case State.Up:
                    break;
                case State.Drag:

                    if (Event.current.type != EventType.Repaint) return;
                    float value = currentEvent.mousePosition.x;
                    value = Mathf.Clamp(value, rect.xMin, rect.xMax);
                    value = (value - rect.xMin) / (rect.xMax - rect.xMin);

                    if (selectValues.Count == 1)
                    {
                        selectValues[selectId] = value;
                    }
                    else
                    {
                        if (selectId == 0)
                        {
                            value = Mathf.Clamp(value, 0, selectValues[selectId + 1]);
                            selectValues[selectId] = value;
                        }
                        else if (selectId == selectPoint.Length - 1)
                        {
                            value = Mathf.Clamp(value, selectValues[selectId - 1], 1);
                            selectValues[selectId] = value;
                        }
                        else
                        {
                            value = Mathf.Clamp(value, selectValues[selectId - 1], selectValues[selectId + 1]);
                            selectValues[selectId] = value;
                        }
                    }
                    break;
                case State.Out:
                    break;
                default:
                    break;
            }

        }
    }

    public enum State { None, Down, Up, Drag, Out }
    void ChangeState(State state)
    {
        this.state = state;
    }
}

#endif

