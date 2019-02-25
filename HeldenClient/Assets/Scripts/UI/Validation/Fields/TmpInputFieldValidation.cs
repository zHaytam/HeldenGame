using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.UI.Validation.Fields
{
    /// <summary>
    /// TextMeshPro Input Field Validation.
    /// </summary>
    [ExecuteAlways]
    public class TmpInputFieldValidation : FieldValidation
    {

        #region Fields

        [SerializeField] private TextMeshProUGUI _errorLabel;
        private TMP_InputField _inputField;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
            if (_inputField == null)
            {
#if UNITY_EDITOR
                DestroyImmediate(this);
                EditorUtility.DisplayDialog("Error", "In order to use TmpInputFieldValidation, a TextMeshPro Input Field is required.", "Ok");
                return;
#else
                throw new MissingComponentException("TextMeshPro Input Field missing.");
#endif
            }

            // Only add listener if there is an error label specified
            if (_errorLabel)
                _inputField.onValueChanged.AddListener(InputField_OnValueChanged);
        }

        private void InputField_OnValueChanged(string text)
        {
            if (IsValid())
            {
                _errorLabel.gameObject.SetActive(false);
            }
            else
            {
                _errorLabel.SetText(string.Format(UiValidationMessages.Required, Name));
                _errorLabel.gameObject.SetActive(true);
            }
        }

        #endregion

        #region Public Methods

        public override bool IsValid()
        {
            string input = _inputField.text;
            return UiValidators.All(validator => validator.Validate(input));
        }

        #endregion

    }
}
