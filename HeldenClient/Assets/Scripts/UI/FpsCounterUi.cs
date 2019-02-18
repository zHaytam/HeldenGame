using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Display an FPS counter for debugging.
    /// </summary>
    public class FpsCounterUi : UndestroyableSingletonBehaviour<MonoBehaviour>
    {

        #region Fields

        [SerializeField] private TextMeshProUGUI _text;
        private float _deltaTime = 0.0f;

        #endregion

        #region Unity Methods

        private void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
            float msec = _deltaTime * 1000f;
            float fps = 1f / _deltaTime;
            _text.SetText($"{msec:0.0} ms ({fps:0.} fps)");
        }

        #endregion

    }
}
