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
	const string TASKS_PER_BATCH = "tasksPerBatch";

	const int START_BATCH = 1;
	const int BATCH_COUNT = 3;

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
	int numTasksPerBatch = 10;

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

	public bool AllBatchesProcessed()
	{
		return batchCount >= BATCH_COUNT;
	}

	public int GetStimuli1Index(string stimuli)
	{
		return CurrentBatch.GetStimuli1Index(stimuli);
	}

	public int GetStimuli2Index(string stimuli)
	{
		return CurrentBatch.GetStimuli2Index(stimuli);
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
		batches = new TaskBatch[BATCH_COUNT];
		for(int i = START_BATCH; i <= BATCH_COUNT; i++)
		{
			batches[i - 1] = new TaskBatch(string.Format("{0}{1}", BATCH_KEY, i), processBatch);
		}
		randomBatch = UnityEngine.Random.Range(0, BATCH_COUNT);
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

    TSGameState getNewGame()
    {
        TSGameState game = new TSGameState();
        // Sets the current mode to the first mode in the enum
        game.CurrentBatchIndex = 0;
        game.CurrentTaskIndex = 0;
        game.CompletedTasks = new List<TSTaskDescriptor>();
        return game;
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
		return BATCH_COUNT + 1;
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