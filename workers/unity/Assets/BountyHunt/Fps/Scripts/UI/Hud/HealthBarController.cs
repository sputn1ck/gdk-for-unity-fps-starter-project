using UnityEngine;
using UnityEngine.UI;

namespace Fps.UI
{
    public class HealthBarController : MonoBehaviour
    {
        public Image HealthFill1;
        public Image HealthFill2;
        public TMPro.TextMeshProUGUI healthText;


        public void SetHealthBar(float healthFraction)
        {
            HealthFill1.fillAmount = healthFraction;
            HealthFill2.fillAmount = healthFraction;
            healthText.text = Mathf.Ceil(healthFraction * 100).ToString();
        }
    }
}
