using System;
using System.Collections;

using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Slider = UnityEngine.UI.Slider;

namespace Menu
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;
        
        public Slider progressBar;
        public GameObject transitionsContainer;
        
        private SceneTransition[] transitions;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            transitions = transitionsContainer.GetComponentsInChildren<SceneTransition>();
        }

        public void LoadScene(string sceneName, string transitionName)
        {
            StartCoroutine(LoadSceneAsync(sceneName, transitionName));
        }

        private IEnumerator LoadSceneAsync(string sceneName, string transitionName)
        {
            SceneTransition transition = transitions.First(t => t.name == transitionName);
            
            AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
            scene.allowSceneActivation = false;

            yield return transition.AnimateTransitionIn();
            
            scene.allowSceneActivation = true;

            
            yield return transition.AnimateTransitionOut();
        }
    }
}