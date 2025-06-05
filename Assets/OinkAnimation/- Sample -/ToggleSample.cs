using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Oink.UIAnimation;

namespace Oink.UIAnimation
{
    [RequireComponent(typeof(OinkAnimations))]
    public class ToggleSample : MonoBehaviour
    {
        [SerializeField]
        private OinkAnimations m_ToggleBg;

        [SerializeField]
        private OinkAnimations m_ToggleBar;

        void Start()
        {
            m_ToggleBg = GetComponent<OinkAnimations>();
        }

        public void Toggle()
        {
            if (!m_ToggleBg.IsAnimationRunning())
            {
                m_ToggleBg.PlayNext();
            }
            if (!m_ToggleBar.IsAnimationRunning())
            {
                m_ToggleBar.PlayNext();
            }
        }
    }
}
