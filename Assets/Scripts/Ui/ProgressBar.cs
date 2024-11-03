using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class ProgressBar : MonoBehaviour
{

    [SerializeField]
    private Image m_FillImage;

    [SerializeField, Range(0f, 1f)]
    protected float m_Value;

    public virtual float value
    {
        get
        {
            return m_Value;
        }
        set
        {
            Set(value);
        }
    }

    public float normalizedValue
    {
        get
        {
            if (Mathf.Approximately(0, 1))
                return 0;
            return Mathf.InverseLerp(0, 1, value);
        }
        set
        {
            this.value = Mathf.Lerp(0, 1, value);
        }
    }

    float ClampValue(float input)
    {
        float newValue = Mathf.Clamp(input, 0, 1);
        return newValue;
    }

    protected virtual void Set(float input)
    {
        // Clamp the input
        float newValue = ClampValue(input);

        // If the stepped value doesn't match the last one, it's time to update
        if (m_Value == newValue)
            return;

        m_Value = newValue;
        UpdateVisuals();
    }

    // Force-update the slider. Useful if you've changed the properties and want it to update visually.
    private void UpdateVisuals()
    {
        if (m_FillImage != null && m_FillImage.type == Image.Type.Filled)
        {
            m_FillImage.fillAmount = normalizedValue;
        }
    }


    // This method is called automatically by Unity when any serialized field is modified in the inspector.
    private void OnValidate()
    {
        // Update the visuals when the value changes in the editor
        Set(m_Value);
        UpdateVisuals();
    }
}