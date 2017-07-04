/*
 * Author(s): Isaiah Mann
 * Description: [to be added]
 * Usage: [no notes]
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolunteerScience;

public class TSDataController : SingletonController<TSDataController> 
{
	const string BATCH_KEY = "batch";
	const string IS_IMAGES_KEY = "IsImages";
	const string TASKS_PER_BATCH = "tasksPerBatch";

	const int START_BATCH = 1;
	const int STANDARD_BATCH_COUNT = 3;
	const int HYBRID_BATCH_COUNT = 1;

    public int CurrentBatchIndex
    {
        get
        {
            return game.CurrentBatchIndex;
        }
    }

    public bool IsTaskSwitch
    {
        get;
        private set;
    }

    public int CurrentTaskIndex
    {
        get
        {
            return game.CurrentTaskIndex;
        }
    }

	public TaskBatch CurrentBatch
	{
		get
		{
			if(CurrentBatchIndex >= batches.Length)
			{
				return batches[randomBatch];
			}
			else
			{
				return batches[CurrentBatchIndex];
			}
		}
	}

    int currentTaskIndexInMode
    {
        get
        {
            return game.CurrentTaskIndex % numTasksPerBatch;
        }
    }

	#region Tunable Variables

    [SerializeField]
    string experimentName = "Task Switching";

    [SerializeField]
    bool verboseMode = true;

    #endregion

	TSUIController ui;
    TSGameState game;
    MonoAction onGameEnd;
    DataCollector data;
	TaskBatch[] batches;
	int batchCount = 0;
	int randomBatch;
	int numTasksPerBatch = 10;

	public bool AllBatchesProcessed()
	{
		return batchCount >= STANDARD_BATCH_COUNT;
	}

    protected override void setReferences()
    {
        base.setReferences();
        game = getNewGame();
        data = DataCollector.Get;
        data.SetActiveExperiment(experimentName);
    }

	protected override void fetchReferences()
	{
		base.fetchReferences();		
		ui = TSUIController.Instance;
		createBatches();
		fetchTunableVariables();
	}

	void fetchTunableVariables()
	{
		VariableFetcher.Get.GetInt(
			TASKS_PER_BATCH,
			delegate(int numTasks)
			{
				this.numTasksPerBatch = numTasks;
			}
		);
	}
		
	void createBatches()
	{
		VariableFetcher fetch = VariableFetcher.Get;
		batches = new TaskBatch[STANDARD_BATCH_COUNT + HYBRID_BATCH_COUNT];
		for(int i = START_BATCH; i <= STANDARD_BATCH_COUNT; i++)
		{
			int batchNum = i;
			int batchIndex = batchNum - 1;
			fetch.GetBool(getImageCheckKey(batchNum), delegate(bool isImages)
				{
					string batchName = string.Format("{0}{1}", BATCH_KEY, batchNum);
					if(isImages)
					{
						batches[batchIndex] = new ImageTaskBatch(batchName, processBatch);
					}
					else
					{
						batches[batchIndex] = new TaskBatch(batchName, processBatch);
					}
					postProcessBatch();
				});
		}
		randomBatch = UnityEngine.Random.Range(0, STANDARD_BATCH_COUNT);
	}

	void processBatch()
	{
		batchCount++;
		if(verboseMode)
		{
			Debug.Log("Batch Processed");
		}
		if(AllBatchesProcessed())
		{
			ui.SetLabels(CurrentBatch);
		
		}
	}

	void postProcessBatch()
	{
		if(AllBatchesProcessed())
		{
			generateHybridBatches();
		}
	}

	void generateHybridBatches()
	{
		for(int i = STANDARD_BATCH_COUNT; i < batches.Length; i++)
		{
			batches[i] = new HybridTaskBatch(randomStandardBatch(), randomStandardBatch());
		}
	}

	TaskBatch randomStandardBatch()
	{
		int batchIndex = UnityEngine.Random.Range(0, STANDARD_BATCH_COUNT);
		return batches[batchIndex];
	}

    TSGameState getNewGame()
    {
        TSGameState game = new TSGameState();
        // Sets the current mode to the first mode in the enum
        game.CurrentBatchIndex = 0;
        game.CurrentTaskIndex = 0;
        game.CompletedTasks = new List<TSTaskDescriptor>();
        return game;
    }

	string getImageCheckKey(int batchIndex)
	{
		return string.Format("{0}{1}{2}", BATCH_KEY, batchIndex, IS_IMAGES_KEY);
	}

	public StimuliSet GetSet()
	{
		return CurrentBatch.GetSet();
	}

    public void StartTimer(string key)
    {
        data.TimeEvent(key);
    }

    public double GetEventTime(string key)
    {
        return data.GetEventTimeSeconds(key);
    }

    public bool ShouldSwitchMode()
    {
        return currentTaskIndexInMode >= numTasksPerBatch - 1;
    }

    public bool IsLastMode()
    {
		return CurrentBatchIndex == getNumModes() - 1;
    }

    // Wrap functionality
    public void NextMode()
    {
		game.CurrentBatchIndex++;
		game.CurrentBatchIndex %= getNumModes();
		ui.SetLabels(CurrentBatch);
    }

    public void CompleteTask(TSTaskDescriptor task)
    {
        game.CompletedTasks.Add(task);
        data.AddDataRow(task.GetDataRow());
        if(verboseMode)
        {
            Debug.Log(data.GetExperiment(experimentName).LastRowToString());
        }
        data.SubmitLastDataRow();
    }

    public void SubscribeToGameEnd(MonoAction handler)
    {
        onGameEnd += handler;
    }

    public void UnsubscribeFromGameEnd(MonoAction handler)
    {
        onGameEnd -= handler;
    }

    void callGameEnd()
    {
        if(onGameEnd != null)
        {
            onGameEnd();
        }
    }

    public void NextTask()
    {
        if(ShouldSwitchMode())
        {
            if(IsLastMode())
            {
                callGameEnd();
            }
            else
            {
                NextMode();
                IsTaskSwitch = true;
            }
        }
        else
        {
            IsTaskSwitch = false;
        }
        game.CurrentTaskIndex++;
    }

    int getNumModes()
    {
		// Extra random batch at end
		return STANDARD_BATCH_COUNT + 1;
    }

}

[System.Serializable]
public struct TSGameState
{
    public int CurrentBatchIndex;
    public int CurrentTaskIndex;
    public List<TSTaskDescriptor> CompletedTasks;
}

[System.Serializable]
public class TSTaskDescriptor
{
    public string BlockName;
    public int StimulusPosition; // 1,2,3,4 (top left, top right, bottom right, bottom left
    public int TaskType; // 1 for letter, 2 for number
    public int Stimuli1Index; // index of letter in list
    public int Stimuli2Index; // number value
    public int TypeOfBlock; // (1=just task 1; 2=just task 2; 0=both tasks mixed)
    public int IsNewTaskSwitch; // 1=task switch , 0=task repeat
    public int ResponseStatus; // (1=correct, 2=error, 3=too slow)
    public double ResponseTime; // In Seconds
    public double TotalTaskTime; // total time (response time + button release time)

    public object[] GetDataRow()
    {
        return new object[]
        {
            BlockName,
            StimulusPosition,
            TaskType,
            Stimuli1Index,
            Stimuli2Index,
            TypeOfBlock,
            IsNewTaskSwitch,
            ResponseStatus,
            ResponseTime,
            TotalTaskTime
        };
    }
}

public enum TSMatchCondition
{
    Stimuli1Category1,
    Stimuli1Category2,
    Stimuli2Category1,
    Stimuli2Category2,
}

public enum TSMatchType
{
    Stimuli1,
    Stimuli2,
}

public enum TSTaskType
{
    TaskSwitch = 1,
    TaskRepeat = 0,
}

public enum TSResponseStatus
{
    Correct = 1,
    Error = 2,
    TooSlow = 3,
}