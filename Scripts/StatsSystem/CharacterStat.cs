using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using UnityEngine;  // for ReadOnlyCollection

namespace Bisic.CharacterStats {

    [Serializable]
    public class CharacterStat {
        public int BaseValue; // float ?
        public List<StatModifier> statModifiers; // private readonly
        public readonly ReadOnlyCollection<StatModifier> StatModifiers; // reference to statModifiers

        public CharacterStat() {
            statModifiers = new List<StatModifier>();
            StatModifiers = statModifiers.AsReadOnly();
        }

        public CharacterStat(int baseValue) {
            BaseValue = baseValue;
            statModifiers = new List<StatModifier>();
            StatModifiers = statModifiers.AsReadOnly();
        }

        private bool isDirty = true;
        private int _value;
        private int lastBaseValue = int.MinValue;
        public int Value {
            get {
                if (isDirty || lastBaseValue != BaseValue) {
                    lastBaseValue = BaseValue;
                    _value = CalculateFinalValue();
                    isDirty = false;
                }
                return _value;
            }
        }
        public void AddModifier(StatModifier mod) {
            isDirty = true;
            statModifiers.Add(mod);
            statModifiers.Sort(CompareModifierOrder);            
        }
        private int CompareModifierOrder(StatModifier a, StatModifier b) {
            if (a.Order < b.Order)
                return -1;
            else if (a.Order > b.Order)
                return 1;
            return 0; // if (a.Order == b.Order)
        }

        public bool RemoveModifier(StatModifier mod) {
            if (statModifiers.Remove(mod)) {
                isDirty = true;
                return true;
            }
            return false;
        }

        public bool RemoveAllModifiersFromSource(object source) {
            bool didRemove = false;

            for (int i = statModifiers.Count - 1; i >= 0; i--) {
                if (statModifiers[i].Source == source) {
                    isDirty = true;
                    didRemove = true;
                    statModifiers.RemoveAt(i);
                }
            }
            return didRemove;
        }

        private int CalculateFinalValue() {
            int finalValue = BaseValue;
            int sumPercentAdd = 0; // This will hold the sum of "PercentAdd" modifiers

            for (int i = 0; i < statModifiers.Count; i++) {
                StatModifier mod = statModifiers[i];

                if (mod.Type == StatModType.Flat) {
                    finalValue += mod.Value;
                }
                else if (mod.Type == StatModType.PercentAdd) // When we encounter a "PercentAdd" modifier
                {
                    sumPercentAdd += mod.Value; // Start adding together all modifiers of this type
                    
                    if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModType.PercentAdd) {
                        finalValue *= 1 + sumPercentAdd; // Multiply the sum with the "finalValue", like "PercentMult" modifiers
                        sumPercentAdd = 0; // Reset the sum back to 0
                    }
                }
                else if (mod.Type == StatModType.PercentMult) // Percent renamed to PercentMult
                {
                    finalValue *= 1 + mod.Value;
                }
            }
           
           return finalValue;
        }
    }
}