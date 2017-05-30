/*
 * Author(s): Isaiah Mann
 * Description: Used to fetch variables from Volunteer Science
 * Usage: [no notes]
 * WARNING: The value field of the fetch action is not valid until the action's IsComplete field is set to true
 */

namespace VolunteerScience
{
    using System;

    using UnityEngine;

    public class VariableFetchAction 
    {
        const string RECEIVE_FUNC = "Receive";
        const string FETCH_FUNC = "fetch";

        #region Instance Accessors

        public bool IsComplete
        {
            get;
            private set;
        }

        public object Value
        {
            get;
            private set;
        }

        #endregion

        bool hasCustomFetchCall
        {
            get
            {
                return !string.IsNullOrEmpty(customFetchCall);
            }
        }

        Action<object> callback;
        string key;
        string customFetchCall = string.Empty;
        GameObject callbackObj;

        public VariableFetchAction(string key, Action<object> callback)
        {
            setup(key, callback);
            run();
        }

        public VariableFetchAction(string key, Action<object> callback, string customFetchFunction)
        {
            setup(key, callback);
            this.customFetchCall = customFetchFunction;
            run();
        }

        protected VariableFetchAction(string key)
        {
            setup(key);
        }

        public virtual void RunCallback(object value)
        {
            callback(value);
            this.Value = value;
        }

        public void Complete()
        {
            this.IsComplete = true;
            // Garbage Collection: destroy this now unused object
            UnityEngine.Object.Destroy(callbackObj);
        }

        protected void run()
        {
            // Setup GameObject to receiver
            this.callbackObj = createObject();
            string jsCall = getJSCall(this.key, this.callbackObj);
            Application.ExternalEval(jsCall);

            // In Editor simulation of fetching variables from a dictionary lookup:
            #if UNITY_EDITOR

            if(VariableStoreSimulator.HasInstance)
            {
                VariableStoreSimulator.Get.SimulateVariableFetch(key, callbackObj.name, RECEIVE_FUNC);
            }

            #endif
        }

        void setup(string key)
        {
            this.key = key;
            this.IsComplete = false;
        }

        void setup(string key, Action<object> callback)
        {
            setup(key);
            this.callback = callback;
        }

        /*
         * Should generate a call in the following format
         * SendMessage([GameObject Name], 'Receive', variables['key']);
         */
        string getJSCall(string key, GameObject receiver)
        {
            return string.Format("{0}('{1}', '{2}', '{3}');", 
                FETCH_FUNC,
                key,
                receiver.name,
                RECEIVE_FUNC);
        }
            
        GameObject createObject()
        {
            GameObject receiverObj = new GameObject();
            // Create a random ID for this GO:
            receiverObj.name = Guid.NewGuid().ToString();
            // Hide this so it doesn't crowd the scene
            receiverObj.hideFlags = HideFlags.HideInHierarchy;
            receiverObj.AddComponent<VariableReceiveHandler>().Set(this);
            UnityEngine.Object.DontDestroyOnLoad(receiverObj);
            return receiverObj;
        }

    }

    public class StringFetchAction : VariableFetchAction
    {
        #region Instance Accessors

        public new string Value
        {
            get;
            private set;
        }

        #endregion

        Action<string> callback;

        public StringFetchAction(string key, Action<string> callback) : base(key)
        {
            this.callback = callback;
            run();
        }

        public override void RunCallback(object value)
        {
            string valueStr = value.ToString();
            callback(valueStr);
            this.Value = valueStr;
        }
    }

    public class FloatFetchAction : VariableFetchAction
    {
        #region Instance Accessors

        public new float Value
        {
            get;
            private set;
        }

        #endregion

        Action<float> callback;

        public FloatFetchAction(string key, Action<float> callback) : base(key)
        {
            this.callback = callback;
            run();
        }

        public override void RunCallback(object value)
        {
            try
            {
                float valueF = float.Parse(value.ToString());
                callback(valueF);
                this.Value = valueF;
            }
            catch
            {
                Debug.LogErrorFormat("Unable to parse value {0} to floating point number", value);
            }
        }
    }

    public class IntFetchAction : VariableFetchAction
    {
        #region Instance Accessors

        public new int Value
        {
            get;
            private set;
        }

        #endregion

        Action<int> callback;

        public IntFetchAction(string key, Action<int> callback) : base(key)
        {
            this.callback = callback;
            run();
        }

        public override void RunCallback(object value)
        {
            try
            {
                int valueInt = int.Parse(value.ToString());
                callback(valueInt);
                this.Value = valueInt;
            }
            catch
            {
                Debug.LogErrorFormat("Unable to parse value {0} to integer", value);
            }
        }
    }

    public class BoolFetchAction : VariableFetchAction
    {
        #region Instance Accessors

        public new bool Value
        {
            get;
            private set;
        }

        #endregion

        Action<bool> callback;

        public BoolFetchAction(string key, Action<bool> callback) : base(key)
        {
            this.callback = callback;
            run();
        }

        public override void RunCallback(object value)
        {
            try
            {
                bool valueBool = bool.Parse(value.ToString());
                callback(valueBool);
                this.Value = valueBool;
            }
            catch
            {
                Debug.LogErrorFormat("Unable to parse value {0} to boolean", value);
            }
        }
    }

}
