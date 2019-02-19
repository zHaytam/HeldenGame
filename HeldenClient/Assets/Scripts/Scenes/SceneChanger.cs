using System.Collections;
using Assets.Scripts.I18n;
using Assets.Scripts.UI.Elements;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Scenes
{
    public class SceneChanger : UndestroyableSingletonBehaviour<SceneChanger>
    {

        #region Fields

        private const float SceneLoadedPercentage = 0.9f;
        [SerializeField] private GameObject _panel;
        [SerializeField] private ProgressBarUiElement _pb;

        #endregion

        #region Properties

        public AvailableScene CurrentScene { get; private set; }

        #endregion

        #region Unity Methods

        protected override void OnAwake()
        {
            base.OnAwake();

            SceneManager.sceneLoaded += (scene, mode) =>
            {
                _panel.SetActive(false);
                _pb.Value = 0;
            };
        }

        #endregion

        #region Public Methods

        public void ChangeScene(AvailableScene newScene)
        {
            StartCoroutine(LoadScene(newScene));
        }

        #endregion

        #region Private Methods

        private IEnumerator LoadScene(AvailableScene newScene)
        {
            // Show panel
            _panel.SetActive(true);
            yield return new WaitForSeconds(1f);

            // Load I18N sentences
            yield return StartCoroutine(I18NManager.Instance.ReloadSentences($"{newScene}Scene"));
            Debug.Log("Loaded I18N sentences.");

            // Load scene (de-activated)
            var operation = SceneManager.LoadSceneAsync($"{newScene}Scene");
            operation.allowSceneActivation = false;

            while (operation.progress < SceneLoadedPercentage)
            {
                _pb.Value = operation.progress;
                yield return null;
            }

            // Activate scene
            _pb.Value = 1f;
            yield return new WaitForSeconds(1f);
            operation.allowSceneActivation = true;
            CurrentScene = newScene;
        }

        #endregion

    }
}
