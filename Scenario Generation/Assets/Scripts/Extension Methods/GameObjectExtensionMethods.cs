using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ExtensionMethods
{
    public static class GameObjectExtensionMethods
    {

        public static List<GameObject> GetAllChildrenWithTag(this GameObject gameObject, string tag)
        {
            // This could be more efficient by making the list during recursive calls
            // but i am lazy and this is simpler
            return gameObject.GetAllChildren().Where(o => o.CompareTag(tag)).ToList();
        }

        public static List<GameObject> GetAllChildren(this GameObject gameObject)
        {
            List<GameObject> children = new List<GameObject>();
            
            //Debug.Log(gameObject.transform.childCount);

            // If the object has no children, return empty list
            if (gameObject.transform.childCount <= 0)
                return children;

            // Recursively search all children
            foreach (Transform child in gameObject.transform)
            {
                children.Add(child.gameObject);
                children = children.Concat(child.gameObject.GetAllChildren()).ToList();
            }

            //Debug.Log($"Child count: {children.Count}");
            
            return children;
        }

        public static bool IsValidRoom(this GameObject room)
        {
            if (!room.CompareTag("Room"))
                return false;
            /*
            if (!room.GetAllChildren().Any(childObject =>
                    childObject.CompareTag("Connector")
                ))
                return false;
            */

            return true;
        }
    }
}
