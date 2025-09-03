using UnityEngine;

public static class ComponentTools
{ 
    public static bool AnyChildHasComponent<T>(Transform obj)
    {
        for(int i = 0; i < obj.childCount; i++)
        {
            if (obj.GetChild(i).TryGetComponent<T>(out _)) return true;
            if (AnyChildHasComponent<T>(obj.GetChild(i))) return true;
        }
        //Debug.Log("WHYYYYYYYYYYY");
        return false;
    }
}
