using Assets.Scripts.UI.Validation.Validators;
using UnityEngine;

namespace Assets.Scripts.UI.Validation.Fields
{
    /// <summary>
    /// Base class for each field validation.
    /// </summary>
    public abstract class FieldValidation : MonoBehaviour
    {

        #region Fields

        [SerializeField] protected string Name;
        [SerializeField] protected UiValidator[] UiValidators;

        #endregion

        #region Public Methods

        public abstract bool IsValid();

        #endregion

    }
}
