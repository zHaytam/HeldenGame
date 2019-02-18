using System.Collections;
using Assets.Scripts.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Elements
{
    /// <summary>
    /// A ProgressBar UI Element.
    /// Can either be continuous or undetermined.
    /// </summary>
    public class ProgressBarUiElement : UiElement
    {

        #region Fields

        [Header("Needed elements")]
        [SerializeField] private Image _fillImage;
        [SerializeField] private Image _blockImage;
        [SerializeField] private TextMeshProUGUI _innerText;

        [Header("Options")]
        [SerializeField] private bool _undetermined;
        [SerializeField] private float _minValue;
        [SerializeField] private float _maxValue;
        [SerializeField] private float _value;
        [SerializeField] private ProgressBarTextVisibility _textVisibilityVisibility;

        [Header("Undetermined Animation")]
        [SerializeField] private float _undeterminedStep = 5f;
        [SerializeField] private float _undeterminedStepTime = 0.1f;

        private float _totalWidth;
        private float _blockWidth;
        private Coroutine _undeterminedCoroutine;

        #endregion

        #region Properties

        /// <summary>
        /// Wheither the progress is undetermined or not.
        /// </summary>
        public bool Undetermined
        {
            get => _undetermined;
            set
            {
                if (_undetermined == value)
                    return;

                _undetermined = value;
                OnOptionChanged();
            }
        }

        /// <summary>
        /// The minimum vlaue.
        /// </summary>
        public float MinValue
        {
            get => _minValue;
            set
            {
                if (Mathf.Approximately(_minValue, value) || value >= _maxValue)
                    return;

                _minValue = value;
                if (_value < _minValue)
                    _value = _minValue;

                RefreshValues();
            }
        }

        /// <summary>
        /// The maximum value.
        /// </summary>
        public float MaxValue
        {
            get => _maxValue;
            set
            {
                if (Mathf.Approximately(_maxValue, value) || value <= _minValue)
                    return;

                _maxValue = value;
                if (_value > _maxValue)
                    _value = _maxValue;

                RefreshValues();
            }
        }

        /// <summary>
        /// The current value.
        /// </summary>
        public float Value
        {
            get => _value;
            set
            {
                if (Mathf.Approximately(_value, value))
                    return;

                _value = (value < _minValue ? _minValue : (value > _maxValue ? _maxValue : value));
                RefreshValues();
            }
        }

        /// <summary>
        /// Wheither to show the text (current value / max value) or not.
        /// </summary>
        public ProgressBarTextVisibility TextVisibility
        {
            get => _textVisibilityVisibility;
            set
            {
                if (_textVisibilityVisibility == value)
                    return;

                _textVisibilityVisibility = value;
                OnOptionChanged();
            }
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _totalWidth = _fillImage.rectTransform.rect.width;
            _blockWidth = _blockImage.rectTransform.rect.width;

            OnOptionChanged();
        }

        #endregion

        #region Private Methods

        private void OnOptionChanged()
        {
            _fillImage.gameObject.SetActive(!Undetermined);
            _blockImage.gameObject.SetActive(Undetermined);
            _innerText.gameObject.SetActive(!Undetermined && TextVisibility != ProgressBarTextVisibility.None);

            if (Undetermined)
            {
                // Logically, the fill image should be stretched so we can use it.
                _blockImage.transform.localPosition =
                    _blockImage.transform.localPosition.ChangeTo(_blockWidth / 2 - _totalWidth / 2);
                _undeterminedCoroutine = StartCoroutine(UndeterminedAnimation());
            }
            else if (_undeterminedCoroutine != null)
            {
                StopCoroutine(_undeterminedCoroutine);
            }

            RefreshValues();
        }

        private void RefreshValues()
        {
            if (!Undetermined)
            {
                _fillImage.fillAmount = Value / MaxValue;
                if (TextVisibility != ProgressBarTextVisibility.None)
                {
                    _innerText.SetText($"{Value} / {MaxValue}");
                }

                switch (TextVisibility)
                {
                    case ProgressBarTextVisibility.ValueOverMax:
                        _innerText.SetText($"{Value} / {MaxValue}");
                        break;
                    case ProgressBarTextVisibility.Percentage:
                        _innerText.SetText($"{(Value / MaxValue) * 100}%");
                        break;
                }
            }
        }

        private IEnumerator UndeterminedAnimation()
        {
            float movex = _undeterminedStep;

            while (true)
            {
                yield return new WaitForSeconds(_undeterminedStepTime);

                // Check if we need to switch to moving right:
                if (Mathf.Approximately(_blockImage.transform.localPosition.x, _totalWidth / 2 - _blockWidth / 2))
                {
                    movex *= -1;
                }
                // Check if we need to swith to moving left:
                else if (Mathf.Approximately(_blockImage.transform.localPosition.x, _blockWidth / 2 - _totalWidth / 2))
                {
                    movex = _undeterminedStep;
                }

                _blockImage.transform.localPosition = _blockImage.transform.localPosition.Increment(movex);
            }
        }

        #endregion

    }

    public enum ProgressBarTextVisibility
    {
        None,
        ValueOverMax,
        Percentage
    }
}
