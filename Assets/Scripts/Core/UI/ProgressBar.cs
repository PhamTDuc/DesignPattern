using UnityEngine;
using UnityEngine.UI;

namespace Guinea.Core.UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] int m_minimumValue;
        [SerializeField] int m_maximumValue;
        [SerializeField] Slider m_slider;
        [SerializeField] Image m_fillImage;

        public void ChangeValue(int value)
        {
            m_slider.value = (float)(value - m_minimumValue) / (m_maximumValue - m_minimumValue);
        }

        public void ChangeColor(Color color)
        {
            m_fillImage.color = color;
        }

        public int GetValue() => (int)m_slider.value * (m_maximumValue - m_minimumValue) + m_minimumValue;
    }
}
