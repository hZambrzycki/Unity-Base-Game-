using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;
using UnityEngine.UI;


namespace RPG.Attributes
{
    public class ManaDisplay : MonoBehaviour
    {
        Mana mana;

        private void Awake()
        {
            mana = GameObject.FindWithTag("Player").GetComponent<Mana>();
        }

        private void Update()
        {
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", mana.GetMana(), mana.GetMaxMana());
        }
    }
}