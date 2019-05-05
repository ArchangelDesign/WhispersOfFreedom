using System;

public class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(
                JsonArrayToItemsObject(json)
            );
        return wrapper.Items;
    }

    public static string JsonArrayToItemsObject(string input)
    {
        return "{\"Items\":" + input + "}";
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
