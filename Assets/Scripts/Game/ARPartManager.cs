using System.Collections.Generic;
using UnityEngine;

namespace Game
{

    public static class ARPartManager
    {
        // Public variables
        public static List<ARPart> parts = new List<ARPart>();

        // Public Methods
        public static void Add(ARPart pPart)
        {
            if (!parts.Contains(pPart))
            {
                parts.Add(pPart);
            }
        }

        public static void ShowAll()
        {
            for (int i=0; i<parts.Count; i++)
            {
                if (parts[i] != null)
                {
                    parts[i].Show();
                }
            }
        }

        public static void HideAll()
        {
            for (int i=0; i<parts.Count; i++)
            {
                if (parts[i] != null)
                {
                    parts[i].Hide();
                }
            }
        }

    }
    

}