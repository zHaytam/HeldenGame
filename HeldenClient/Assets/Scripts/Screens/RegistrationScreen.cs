using Assets.Scripts.UI.Validation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Screens
{
    public class RegistrationScreen : MonoBehaviour
    {

        #region Fields

        [SerializeField] private TMP_InputField _emailInput;
        [SerializeField] private TMP_InputField _confirmEmailInput;
        [SerializeField] private TMP_InputField _passwordInput;
        [SerializeField] private TMP_InputField _confirmPasswordInput;
        [SerializeField] private Button _registerBtn;
        [SerializeField] private GameObject _errorLabelPrefab;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            Debug.Log("OnAwake");
            _registerBtn.onClick.AddListener(RegisterBtnClicked);
        }

        private void RegisterBtnClicked()
        {
            // Debug.Log(_emailInput.text);
        }

        private void OnDestroy()
        {
            Debug.Log("OnDestroy");
            _registerBtn.onClick.RemoveListener(RegisterBtnClicked);
        }

        #endregion

    }
}
