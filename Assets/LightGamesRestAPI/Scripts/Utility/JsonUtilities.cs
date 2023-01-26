namespace LightGames.Utility
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using UnityEngine;

    public static class JsonUtilities
    {
        private const char _objectJsonFirstChar = '{';
        private const char _objectJsonLastChar = '}';
        private const char _arrayJsonFirstChar = '[';
        private const char _arrayJsonLastChar = ']';

        public static bool IsValidJson(string testingString)
        {
            // https://youtu.be/j4YAY36xjwE?t=2370
            // best not to use String.StartWith/EndWith
            int jsonLastIndex = testingString.Length - 1;

            char first = testingString[0];
            char last = testingString[jsonLastIndex];

            if ((first != _objectJsonFirstChar || last != _objectJsonLastChar) && (first != _arrayJsonFirstChar || last != _arrayJsonLastChar))
                return false;

            try
            {
                JToken.Parse(testingString);
                return true;
            }
            catch (JsonReaderException jex)
            {
                Debug.LogError(jex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }
        }
    }
}

