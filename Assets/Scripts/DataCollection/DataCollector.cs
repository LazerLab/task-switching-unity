/*
 * Author(s): Isaiah Mann
 * Description: Singleton Class to store collected data
 * Usage: [no notes]
 */

using System;
using UnityEngine;
using System.Collections.Generic;

namespace VSDataCollector
{	
	public class DataCollector : Singleton<DataCollector>
	{
		Dictionary<string, Experiment> instances = new Dictionary<string, Experiment>();

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
