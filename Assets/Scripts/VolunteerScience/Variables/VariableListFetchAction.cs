/*
 * Author(s): Isaiah Mann
 * Description: Used to fetch lists of variables from Volunteer Science
 * Usage: Assumes commas are always used to divide values, trims out white space between values
 * WARNING: The value field of the fetch action is not valid until the action's IsComplete field is set to true
 */

namespace VolunteerScience
{
    using System;

    using UnityEngine;

    // In order to parse a list of variables, we need to convert to a string, so assume these value should already be strings
    public class VariableListFetchAction : VariableFetchAction
    {
		// Expects commas to separate items in a list
        const char DIVIDER_CHAR = ',';

        #region Instance Accessors

        public new string[] Value
        {
            get;
            private set;
        }

        #endregion

        Action<string[]> callback;

        public VariableListFetchAction(string key, Action<string[]> callback) : base(key)
        {
            this.callback = callback;
            run();
        }

        // Empty constructor for subclasses
        protected VariableListFetchAction(string key) : base(key){}

		// The callback returns a single string which needs to be converted into a list
        public override void RunCallback(object value)
        {
            string[] valueList = parseValueList(value);
            callback(valueList);
            this.Value = valueList;
        }

        protected string[] parseValueList(object value)
        {
            string valueStr = value.ToString();
            string[] values = valueStr.Split(DIVIDER_CHAR);
            for(int i = 0; i < values.Length; i++)
            {
				// Removes an spaces between each list item
                values[i] = values[i].Trim();
            }
            return values;
        }

    }

    public class FloatListFetchAction : VariableListFetchAction
    {
        #region Instance Accessors

        public new float[] Value
        {
            get;
            private set;
        }

        #endregion

        Action<float[]> callback;

        public FloatListFetchAction(string key, Action<float[]> callback) : base(key)
        {
            this.callback = callback;
            run();
        }

        public override void RunCallback(object value)
        {
            string[] valueList = parseValueList(value);
            float[] parsedValues = new float[valueList.Length];
            bool success = true;
            for(int i = 0; i < valueList.Length; i++)
            {
                try
                {
					// Parse each string value to a float
                    parsedValues[i] = float.Parse(valueList[i]);
                }
                catch
                {
                    success = false;
                    Debug.LogErrorFormat("Unable to parse {0} to floating point number", valueList[i]);
                }
            }
            if(success)
            {
                callback(parsedValues);
                this.Value = parsedValues;
            }
        }
            
    }

    public class IntListFetchAction : VariableListFetchAction
    {
        #region Instance Accessors

        public new int[] Value
        {
            get;
            private set;
        }

        #endregion

        Action<int[]> callback;

        public IntListFetchAction(string key, Action<int[]> callback) : base(key)
        {
            this.callback = callback;
            run();
        }

        public override void RunCallback(object value)
        {
            string[] valueList = parseValueList(value);
            int[] parsedValues = new int[valueList.Length];
            bool success = true;
            for(int i = 0; i < valueList.Length; i++)
            {
                try
                {
					// Parse each string value to an integer
                    parsedValues[i] = int.Parse(valueList[i]);
                }
                catch
                {
                    success = false;
                    Debug.LogErrorFormat("Unable to parse {0} to integer", valueList[i]);
                }
            }
            if(success)
            {
                callback(parsedValues);
                this.Value = parsedValues;
            }
        }
    }

    public class BoolListFetchAction : VariableListFetchAction
    {
        #region Instance Accessors

        public new bool[] Value
        {
            get;
            private set;
        }

        #endregion

        Action<bool[]> callback;

        public BoolListFetchAction(string key, Action<bool[]> callback) : base(key)
        {
            this.callback = callback;
            run();
        }

        public override void RunCallback(object value)
        {
            string[] valueList = parseValueList(value);
            bool[] parsedValues = new bool[valueList.Length];
            bool success = true;
            for(int i = 0; i < valueList.Length; i++)
            {
                try
                {
					// Parse each string value to a boolean
                    parsedValues[i] = bool.Parse(valueList[i]);
                }
                catch
                {
                    success = false;
                    Debug.LogErrorFormat("Unable to parse {0} to boolean", valueList[i]);
                }
            }
            if(success)
            {
                callback(parsedValues);
                this.Value = parsedValues;
            }
        }
    }

}
