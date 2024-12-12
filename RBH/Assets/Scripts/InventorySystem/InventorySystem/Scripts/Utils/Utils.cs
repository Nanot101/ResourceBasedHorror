using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;
namespace InventorySystem
{
    public static class Utils
    {
        public static bool IsAlive(this object aObj)
        {
            UnityEngine.Object unityObject = aObj as UnityEngine.Object;
            return unityObject != null;
        }

        public static void ResetTransform(this Transform aTransform)
        {
            aTransform.localPosition = Vector3.zero;
            aTransform.localRotation = Quaternion.identity;
            aTransform.localScale = Vector3.one;
        }

        public static T GetRandom<T>(this T[] sequence)
        {
            return sequence.Length > 0 ? sequence[UnityEngine.Random.Range(0, sequence.Length)] : default;
        }
        //Finalizar GetRandomAmmount
        public static T[] GetRandomAmmount<T>(this T[] sequence, int ammount, bool allowDuplicates = false)
        {
            if (sequence == null && ammount <= 0)
            {
                return Array.Empty<T>();
            }
            Random random = new Random();
            List<T> shuffledList = sequence.OrderBy(x => random.Next()).ToList();
            if (!allowDuplicates && shuffledList.Count <= ammount)
            {
                Debug.LogWarning("Getting Random With Reduced Ammount");
                ammount = shuffledList.Count;
            }
            HashSet<T> selectedSet = new HashSet<T>();
            List<T> selectedItems = new List<T>();
            while (selectedItems.Count < ammount)
            {
                int index = random.Next(shuffledList.Count);
                T selectedItem = shuffledList[index];

                if (allowDuplicates || selectedSet.Add(selectedItem))
                {
                    selectedItems.Add(selectedItem);
                }
            }
            return selectedItems.ToArray();
        }

        public static void DestroyAllChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}