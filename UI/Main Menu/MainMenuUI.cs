using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.SceneManagment;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        LazyValue<SavingWrapper> savingWrapper;
        [SerializeField] TMP_InputField newGameNameField;
        private void Awake() {
            savingWrapper = new LazyValue<SavingWrapper>(GetSavingWrapper);
        }

        private SavingWrapper GetSavingWrapper()
        {
            return FindObjectOfType<SavingWrapper>();
        }

        public void ContinueGame()
        {
            savingWrapper.value.ContinueGame();
        }
        public void NewGame()
        {
            savingWrapper.value.NewGame(newGameNameField.text);
        }
    }
}