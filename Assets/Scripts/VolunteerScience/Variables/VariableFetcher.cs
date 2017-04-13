/*
 * Author(s): Isaiah Mann
 * Description: Fetches variables from the Volunteer Science environment via external JavaScript calls
 * Usage: [no notes]
 */

namespace VolunteerScience
{
    using UnityEngine;

    using System;
    using System.Collections.Generic;

    public class VariableFetcher : Singleton<VariableFetcher>
    {
        public VariableFetchAction GetValue(string key, Action<object> callback)
        {
            return new VariableFetchAction(key, callback);
        }

    }

    public class VariableFetchAction 
    {
        const string VARIABLES_LOOKUP = "variables";
        const string SEND_TO_UNITY_FUNC = "SendMessage";
        const string RECEIVE_FUNC = "Receive";

        Action<object> callback;
        string key;
        GameObject callbackObj;

        internal VariableFetchAction(string key, Action<object> callback)
        {
            this.key = key;
            this.callback = callback;
            run();
        }
            
        public void RunCallback(object value)
        {
            callback(value);
        }

        void run()
        {
            // Setup GameObject to receiver
            this.callbackObj = createObject();
            string jsCall = getJSCall(this.key, this.callbackObj);
            Application.ExternalEval(jsCall);
        }

        /*
         * Should generate a call in the following format
         * SendMessage([GameObject Name], 'Receive', variables['key']);
         */
        string getJSCall(string key, GameObject receiver)
        {
            return string.Format("{0}('{1}', '{2}', {3}['{4}']);",
                SEND_TO_UNITY_FUNC,
                receiver.name,
                RECEIVE_FUNC,
                VARIABLES_LOOKUP,
                key);
        }

        GameObject createObject()
        {
            GameObject receiverObj = new GameObject();
            // Create a random ID for this GO:
            receiverObj.name = Guid.NewGuid().ToString();
            // Hide this so it doesn't crowd the scene
            receiverObj.hideFlags = HideFlags.HideInHierarchy;
            receiverObj.AddComponent<VariableReceiveHandler>().Set(this);
            return receiverObj;
        }

    }

    public class VariableReceiveHandler : MonoBehaviour
    {
        VariableFetchAction fetcher;

        public void Set(VariableFetchAction fetcher)
        {
            this.fetcher = fetcher;
        }

        public void Receive(object value)
        {
            fetcher.RunCallback(value);   
        }

    }

}
