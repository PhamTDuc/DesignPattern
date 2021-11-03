using UnityEngine;
using UnityEngine.UI;

namespace Guinea.Core.UI
{
    public class RadialBar : MonoBehaviour
    {
        [SerializeField] int m_minimumValue;
        [SerializeField] int m_maximumValue;
        [SerializeField] Image m_fillImage;

        public void ChangeValue(int value)
        {
            m_fillImage.fillAmount = (float)(value - m_minimumValue) / (m_maximumValue - m_minimumValue);
        }

        public void ChangeColor(Color color)
        {
            m_fillImage.color = color;
        }

        public int GetValue() => (int)m_fillImage.fillAmount * (m_maximumValue - m_minimumValue) + m_minimumValue;
    }
}