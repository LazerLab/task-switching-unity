/*
 * Author(s): Isaiah Mann
 * Description: Fetches variables from the Volunteer Science environment via external JavaScript calls
 * Usage: [no notes]
 */

namespace VolunteerScience
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    public class VariableFetcher : Singleton<VariableFetcher>
    {
		const string CONSUMABLE_KEY = "vs_consumables";
		const string MATRIX_KEY = "vs_matrix";
		const string SET_CONSUMABLES_FUNC = "setConsumables";

		const char JOIN_CHAR = ':';

        public VariableFetchAction GetValue(string key, Action<object> callback)
        {
            return new VariableFetchAction(key, callback);
        }

        public StringFetchAction GetString(string key, Action<string> callback)
        {
            return new StringFetchAction(key, callback);
        }

        public FloatFetchAction GetFloat(string key, Action<float> callback)
        {
            return new FloatFetchAction(key, callback);
        }

        public IntFetchAction GetInt(string key, Action<int> callback)
        {
            return new IntFetchAction(key, callback);
        }

        public BoolFetchAction GetBool(string key, Action<bool> callback)
        {
            return new BoolFetchAction(key, callback);
        }

        public VariableListFetchAction GetValueList(string key, Action<string[]> callback)
        {
            return new VariableListFetchAction(key, callback);
        }

		public VariableListFetchAction GetConsumables(string consumableClass, string consumableSet, int amount, Action<string[]> callback)
		{
			return new VariableListFetchAction(getConsumablesKey(consumableClass, consumableSet, amount), callback);
		}

		public void SetConsumables(string consumableClass, string consumableSet, string usedConsumable)
		{
			Application.ExternalCall(SET_CONSUMABLES_FUNC, consumableClass, consumableSet, usedConsumable);
		}

		// Row and column values are zero-indexed
		public StringFetchAction GetMatrixValue(string matrix, int row, int column, Action<string> callback)
		{
			return new StringFetchAction(FormatFetchCall(MATRIX_KEY, matrix, row.ToString(), column.ToString()), callback);
		}

        public FloatListFetchAction GetFloatList(string key, Action<float[]> callback)
        {
            return new FloatListFetchAction(key, callback);
        }

        public IntListFetchAction GetIntList(string key, Action<int[]> callback)
        {
            return new IntListFetchAction(key, callback);
        }

        public BoolListFetchAction GetBoolList(string key, Action<bool[]> callback)
        {
            return new BoolListFetchAction(key, callback);
        }

		public string FormatFetchCall(params string[] subKeys)
		{
			return string.Join(JOIN_CHAR.ToString(), subKeys);
		}

		string getConsumablesKey(string consumableClass, string consumableSet, int amount)
		{
			return string.Format("{1}{0}{2}{0}{3}{0}{4}", JOIN_CHAR, CONSUMABLE_KEY, consumableClass, consumableSet, amount);
		}

    }
        
}
