using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [ExecuteAlways]
    public class HealthSlider : Slider
    {
        protected override void Start()
        {
            base.Start();
            if (Application.isPlaying)
                fillRect.GetComponent<Image>().material = new Material(fillRect.GetComponent<Image>().material.shader);
        }

        protected override void Update()
        {
            base.Update();
            fillRect.GetComponent<Image>().material.SetFloat("_Value", value);
        }
    }
}