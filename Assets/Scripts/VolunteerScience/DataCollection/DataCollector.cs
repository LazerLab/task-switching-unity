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
        const string SUBMIT_FUNC = "submit";
        const string ACTIVE_EXPERIMENT_NOT_SET = "Active experiment not set";

        bool hasActiveExperiment
        {
            get
            {
                return activeExperiment != null;
            }
        }

		Dictionary<string, Experiment> instances = new Dictionary<string, Experiment>();
        Experiment activeExperiment;

		public Experiment TrackExperiment(string instanceName)
		{
			Experiment exp = new Experiment(instanceName);
			instances[exp.GetName] = exp;
			return exp;
		}

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

        public void SetActiveExperiment(string name)
        {
            this.activeExperiment = GetExperiment(name);
        }

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

        void submitData(string dataAsString)
        {
            string jsMessage = string.Format("{0}('{1}');", SUBMIT_FUNC, dataAsString);
            Application.ExternalEval(jsMessage);
        }

	}

	[System.Serializable]
	public class Experiment
	{
        const string DATA_SEPARATOR = ",";

		public string GetName
		{
			get
			{
				return this.experimentName;
			}
		}
			
		string experimentName;
        List<object[]> dataRows;
        Dictionary<string, DateTime> activeTimers;

		public Experiment(string name)
		{
			this.experimentName = name;
            this.dataRows = new List<object[]>();
            this.activeTimers = new Dictionary<string, DateTime>();
		}

        public void AddDataRow(params object[] data)
        {
            this.dataRows.Add(data);
        }

        public string LastRowToString()
        {
            return RowToString(dataRows.Count - 1);
        }

        public string RowToString(int rowIndex)
        {
            string dataAsString = string.Empty;
            try
            {   
                object[] data = dataRows[rowIndex];
                for(int i = 0; i < data.Length - 1; i++)
                {
                    dataAsString += data[i].ToString() + DATA_SEPARATOR;
                }
                dataAsString += data[data.Length - 1].ToString();
                return dataAsString;
            }
            catch
            {
                return string.Empty;
            }
        }

        public void TimeEvent(string key)
        {
            this.activeTimers[key] = DateTime.Now;
        }

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
