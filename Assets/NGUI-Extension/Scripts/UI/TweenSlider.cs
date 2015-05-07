//-----------------------------------------------------------------------------
//  TweenSlider.cs
//
//  (c) 2015 Kim, Hyoun Woo
//-----------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

/// <summary>
/// Tween UISlider's value with the given range between from and to.
/// </summary>
[RequireComponent (typeof(UISlider))]
public class TweenSlider : UITweener
{
    [Range(0f, 1f)]
    public float from = 0f;
    [Range(0f, 1f)]
    public float to = 0f;

    private UISlider _slider;

    public UISlider slider { get  {return this._slider; } private set {this._slider = value;} }
    public float Value { get { return this._slider.value; } set { this._slider.value = value; } }


    protected override void OnUpdate(float factor, bool isFinished)
    {
        if (this._slider == null)
            this._slider = this.GetComponent<UISlider>();

        this._slider.value = Mathf.Lerp(from, to, factor);
    }

    static public TweenSlider Begin(UISlider slider, float duration, float value)
    {
        TweenSlider comp = UITweener.Begin<TweenSlider>(slider.gameObject, duration);
        if (comp != null)
        {
            comp.from = comp.Value;
            comp.to = value;
            comp.slider = slider;
            if (duration <= 0.0f)
            {
                comp.Sample(1.0f, true);
                comp.enabled = false;
            }
        }
        return comp;
    }

    static public TweenSlider Begin(UISlider slider, float value, float speed, float maxDuration)
    {
        TweenSlider comp = UITweener.Begin<TweenSlider>(slider.gameObject, 1f);
        if (comp != null)
        {
            comp.from = comp.Value;
            comp.to = value;
            comp.slider = slider;

            comp.duration = Mathf.Max(0.1f, Mathf.Min(Mathf.Abs(comp.to = comp.from) / speed, maxDuration));

            if (speed < 0.0f)
            {
                comp.Sample(1.0f, true);
                comp.enabled = false;
            }
        }
        return comp;
    }

    private void Awake()
    {
        this.slider = this.GetComponent<UISlider>();
    }

    void OnDisable()
    {
        Sample(1.0f, true);
        enabled = false;
    }

}
