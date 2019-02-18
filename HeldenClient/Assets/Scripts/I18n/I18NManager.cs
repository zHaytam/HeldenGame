using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.I18n
{
    /// <summary>
    /// Handles loading/changing languages as well as getting
    /// texts/sentences based on the keys/count.
    /// </summary>
    public class I18NManager : UndestroyableSingletonBehaviour<I18NManager>
    {

        #region Fields

        private Dictionary<string, I18NSentence> _currentSentences;

        #endregion

        #region Properties

        public I18NLanguage CurrentLanguage { get; private set; }

        #endregion

        #region Events

        public event Action SentencesReloaded;

        #endregion

        #region Unity Methods

        protected override void OnAwake()
        {
            CurrentLanguage = GetStartingLanguage();
        }

        #endregion

        #region Public Methods

        public void ChangeLanguageTo(I18NLanguage language)
        {
            if (language == CurrentLanguage)
                return;

            CurrentLanguage = language;
            ReloadSentences(SceneManager.GetActiveScene().name);
        }

        public IEnumerator ReloadSentences(string sceneName)
        {
            string filename = $"{sceneName}_{CurrentLanguage}.csv";
            string streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, "I18n", filename);
            return LoadSentencesFromFile(streamingAssetsPath);
        }

        public string GetNormal(string key) => GetSentence(key)?.Normal ?? key;

        public string GetNormalFormat(string key, params object[] args)
        {
            var sentence = GetSentence(key);
            return sentence == null ? key : string.Format(sentence.Normal, args);
        }

        public string GetBasedOnCount(string key, int count)
        {
            var sentence = GetSentence(key);
            if (sentence == null)
                return key;

            switch (count)
            {
                case 0:
                    return sentence.Zero;
                case 1:
                    return sentence.One;
                default:
                    // Assuming count is always >= 0
                    return sentence.Many;
            }
        }

        public string GetBasedOnCountFormat(string key, int count, params object[] args)
        {
            var sentence = GetSentence(key);
            if (sentence == null)
                return key;

            switch (count)
            {
                case 0:
                    return string.Format(sentence.Zero, args);
                case 1:
                    return string.Format(sentence.One, args);
                default:
                    // Assuming count is always >= 0
                    return string.Format(sentence.Many, args);
            }
        }
    
        #endregion

        #region Private Methods

        private I18NSentence GetSentence(string key)
        {
            return _currentSentences?.ContainsKey(key) == true ? _currentSentences[key] : null;
        }

        private static I18NLanguage GetStartingLanguage()
        {
            // Todo: Load from user data
            return I18NLanguage.English;
        }

        private IEnumerator LoadSentencesFromFile(string streamingAssetsPath)
        {
            // Read file depending on the platform
#if UNITY_ANDROID
            var www = UnityWebRequest.Get(streamingAssetsPath);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogWarning($"Failed to get '{streamingAssetsPath}' using UnityWebRequest.");
                _currentSentences = null;
                yield break;
            }

            string content = www.downloadHandler.text;
#else
            string content = File.ReadAllText(streamingAssetsPath);
#endif

            _currentSentences = CsvReader.ParseToDictionary(content, 5, true, row => row[0], row => new I18NSentence
            {
                Normal = row[1],
                Zero = row[2],
                One = row[3],
                Many = row[4]
            });

            SentencesReloaded?.Invoke();
        }

        #endregion

    }
}