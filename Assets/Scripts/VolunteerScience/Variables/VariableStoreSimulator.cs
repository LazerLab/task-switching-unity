/*
 * Author(s): Isaiah Mann
 * Description: Used as a testing tool within Unity Editor to simulate returning variables from Volunteer Science
 * Usage: Only to be used within the Unity Editor, not within builds
 */

namespace VolunteerScience
{
    using System.Collections.Generic;

    using UnityEngine;

    public class VariableStoreSimulator : Singleton<VariableStoreSimulator>
    {
        [SerializeField]
        VSVariable[] variables;
        Dictionary<string, VSVariable> variableLookup;

        #region Single Overrides

        protected override void Awake()
        {
            base.Awake();
            generateVariableLookup();
        }

        #endregion

        public void SimulateVariableFetch(string key, string objectName, string receiveMethod)
        {
            VSVariable variable;
            if(variableLookup.TryGetValue(key, out variable))
            {
                // GameObject.Find has poor performance, only use in cases like this (for testing purposes)
                GameObject.Find(objectName).SendMessage(receiveMethod, variable.value);
            }
        }

        void generateVariableLookup()
        {
            this.variableLookup = new Dictionary<string, VSVariable>();
            foreach(VSVariable variable in this.variables)
            {
                // Clobbers repeat variables with most recently seen:
                this.variableLookup[variable.key] = variable;
            }
        }

    }

    [System.Serializable]
    public class VSVariable
    {
        public string key;
        public string value;
    }

}
