using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Shops;
using TMPro;
using System;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class ShopUI : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI shopName;
        [SerializeField]
        Transform listRoot;
        [SerializeField]
        RowUI rowPrefab;
        [SerializeField]
        TextMeshProUGUI totalField;
        [SerializeField]
        Button confirmButton;
        [SerializeField] 
        Button switchButton;

        Color originalTotalTextColor;

        Shopper shopper = null;
        Shop currentShop = null;

        void Start() 
        {
            originalTotalTextColor = totalField.color;

            shopper = GameObject.FindGameObjectWithTag("Player").GetComponent<Shopper>();
            if (shopper == null) return;

            shopper.activeShopChange += ShopChanged;
            confirmButton.onClick.AddListener(ConfirmTransaction);
            switchButton.onClick.AddListener(SwitchMode);

            ShopChanged();
        }
        private void ShopChanged()
        {
            if(currentShop != null)
            {
                currentShop.onChange -= RefreshUI;
            }

            currentShop = shopper.GetActiveShop();
            gameObject.SetActive(currentShop != null);

            foreach (FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.SetShop(currentShop);
            }

            if(currentShop == null) return;
            shopName.text = currentShop.GetShopName();

            currentShop.onChange += RefreshUI;

            RefreshUI();
        }

        private void RefreshUI()
        {
            foreach(Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }
            foreach(ShopItem item in currentShop.GetFilteredItems())
            {
                RowUI row = Instantiate<RowUI>(rowPrefab, listRoot);
                row.Setup(currentShop, item);
            }

            totalField.text = $"Total : ${currentShop.TransactionTotal():N2}";
            totalField.color = currentShop.HasSufficientFunds()? originalTotalTextColor : Color.red;
            if(!currentShop.HasInventorySpace())
            {
                totalField.text = "NOT ENOUGH SPACE";
                totalField.color = Color.red;
            }
            confirmButton.interactable = currentShop.CanTransact();
            TextMeshProUGUI switchText = switchButton.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI confirmText = confirmButton.GetComponentInChildren<TextMeshProUGUI>();
            if (currentShop.IsBuyingMode())
            {
                switchText.text = "Switch To Selling";
                confirmText.text = "Buy";
            }
            else
            {
                switchText.text = "Switch To Buying";
                confirmText.text = "Sell";
            }
            foreach (FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.RefreshUI();
            }
        }

        public void Close()
        {
            shopper.SetActiveShop(null);
        }

        public void ConfirmTransaction()
        {
            currentShop.ConfirmTransaction();
        }
        public void SwitchMode()
        {
            currentShop.SelectMode(!currentShop.IsBuyingMode());
        }
    }
}
