using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Screens
{
    public class RegistrationScreen : MonoBehaviour
    {

        #region Fields

        [SerializeField] private TMP_InputField _emailInput;
        [SerializeField] private Button _registerBtn;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            Debug.Log("OnAwake");
            _registerBtn.onClick.AddListener(RegisterBtnClicked);
        }

        private void RegisterBtnClicked()
        {
            Debug.Log(_emailInput.text);
        }

        private void OnDestroy()
        {
            Debug.Log("OnDestroy");
            _registerBtn.onClick.RemoveListener(RegisterBtnClicked);
        }

        #endregion

    }
}
