/*
 * Author(s): Isaiah Mann
 * Description: Singleton Class to store collected data
 * Usage: [no notes]
 */

using System;
using UnityEngine;
using System.Collections.Generic;

namespace VolunteerScience
{	
	public class DataCollector : Singleton<DataCollector>
	{
		// JavaScript to call in the web page of the WebGL build
        const string SUBMIT_FUNC = "submit";

		// Error message:
        const string ACTIVE_EXPERIMENT_NOT_SET = "Active experiment not set";

		// Used internally to ensure an Active Experiment exists
        bool hasActiveExperiment
        {
            get
            {
                return activeExperiment != null;
            }
        }

		// Saves experiments by name
		Dictionary<string, Experiment> instances = new Dictionary<string, Experiment>();
        // Tracks currently active experiment
		Experiment activeExperiment;

		// Creates a new experiment with the specified name
		public Experiment TrackExperiment(string instanceName)
		{
			Experiment exp = new Experiment(instanceName);
			instances[exp.GetName] = exp;
			return exp;
		}

		// Fetches or creates an experiment if it does not already exist
		public Experiment GetExperiment(string name)
		{
			Experiment exp;
			if(instances.TryGetValue(name, out exp))
			{
				return exp;
			}
			else
			{
				return TrackExperiment(name);
			}
		}

		// Returns the active experiment or null
		public Experiment GetActiveExperiment()
		{
			if(hasActiveExperiment)
			{
				return activeExperiment;
			}
			else
			{
				Debug.LogErrorFormat("{0}. Returning null", ACTIVE_EXPERIMENT_NOT_SET);
				return null;
			}
		}

		// Sets the active experiment
        public void SetActiveExperiment(string name)
        {
            this.activeExperiment = GetExperiment(name);
        }

		// Adds a row of data to the current experiment
        public void AddDataRow(params object[] data)
        {
            if(hasActiveExperiment)
            {
                activeExperiment.AddDataRow(data);
            }
            else
            {
                Debug.LogError(ACTIVE_EXPERIMENT_NOT_SET);
            }
        }

		// Starts a timer on the active experiment
        public void TimeEvent(string eventName)
        {
            if(hasActiveExperiment)
            {
                activeExperiment.TimeEvent(eventName);
            }
            else
            {
                Debug.LogError(ACTIVE_EXPERIMENT_NOT_SET);
            }
        }
            
		// Gets how much time has passed since the timer begun
        public double GetEventTimeSeconds(string eventName)
        {
            if(hasActiveExperiment)
            {
                return activeExperiment.GetEventTimeSeconds(eventName);
            }
            else
            {
                Debug.LogError(ACTIVE_EXPERIMENT_NOT_SET);
                return double.NaN;
            }
        }

		// Submits the most recently added row of data for the experiment
        public void SubmitLastDataRow()
        {
            if(hasActiveExperiment)
            {
                submitData(activeExperiment.LastRowToString());
            }
            else
            {
                Debug.LogError(ACTIVE_EXPERIMENT_NOT_SET);
            }
        }

		// Submits a string of data directly to Volunteer Science
        public void SubmitData(string dataAsString)
        {
            submitData(dataAsString);
        }

		// Formats the submit call
        void submitData(string dataAsString)
        {
            string jsMessage = string.Format("{0}('{1}');", SUBMIT_FUNC, dataAsString);
            Application.ExternalEval(jsMessage);
        }

	}

	[System.Serializable]
	public class Experiment
	{
		// Each piece of data in a data row is separated by this character
        const string DATA_SEPARATOR = ",";

		// Accessors for the name of the experiment
		public string GetName
		{
			get
			{
				return this.experimentName;
			}
		}
			
		string experimentName;
		// Stores all of data saved in this experiment
        List<object[]> dataRows;
		// Saves timers in a Dictionary by name
        Dictionary<string, DateTime> activeTimers;

		public Experiment(string name)
		{
			this.experimentName = name;
            this.dataRows = new List<object[]>();
            this.activeTimers = new Dictionary<string, DateTime>();
		}

		// Can take any arguments for the data row
        public void AddDataRow(params object[] data)
        {
            this.dataRows.Add(data);
        }

		// Converts the most recently added row to a string
        public string LastRowToString()
        {
            return RowToString(dataRows.Count - 1);
        }

		// Converts a specified row to a string, inserting commas between each piece of data
        public string RowToString(int rowIndex)
        {
			System.Text.StringBuilder dataString = new System.Text.StringBuilder();
            try
            {   
                object[] data = dataRows[rowIndex];
                for(int i = 0; i < data.Length - 1; i++)
                {
					dataString.Append(data[i].ToString() + DATA_SEPARATOR);
                }
				dataString.Append(data[data.Length - 1].ToString());
				return dataString.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

		// Creates a new timer within the experiment
        public void TimeEvent(string key)
        {
            this.activeTimers[key] = DateTime.Now;
        }

		// Returns how long has passed since the timer started
        public double GetEventTimeSeconds(string key)
        {
            DateTime startTime;
            if(activeTimers.TryGetValue(key, out startTime))
            {
                return (DateTime.Now - startTime).TotalSeconds;
            }
            else
            {
                return 0;
            }
        }

	}

}
