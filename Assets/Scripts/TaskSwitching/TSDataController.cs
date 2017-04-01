/*
 * Author(s): Isaiah Mann
 * Description: [to be added]
 * Usage: [no notes]
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSDataCollector;

public class TSDataController : SingletonController<TSDataController> 
{
    public TSMode CurrentMode
    {
        get
        {
            return game.CurrentMode;
        }
    }

    int currentModeIndex
    {
        get
        {
            return (int) CurrentMode;
        }
    }
 
    int currentTaskIndexInMode
    {
        get
        {
            return game.CurrentTaskIndex % numTasksPerMode;
        }
    }

    #region Tunable Variables

    [SerializeField]
    int numTasksPerMode = 40;

    [SerializeField]
    string experimentName = "Task Switching";

    [SerializeField]
    bool verboseMode = true;

    #endregion

    TSGameState game;
    MonoAction onGameEnd;
    DataCollector data;
    Experiment currentExperiment;

    protected override void setReferences()
    {
        base.setReferences();
        game = getNewGame();
        data = DataCollector.Get;
        currentExperiment = data.TrackExperiment(experimentName);
    }

    TSGameState getNewGame()
    {
        TSGameState game = new TSGameState();
        // Sets the current mode to the first mode in the enum
        game.CurrentMode = (TSMode) 0;
        game.CurrentTaskIndex = 0;
        game.CompletedTasks = new List<TSTaskDescriptor>();
        return game;
    }

    public void StartTimer(string key)
    {
        currentExperiment.TimeEvent(key);
    }

    public double GetEventTime(string key)
    {
        return currentExperiment.GetEventTimeSeconds(key);
    }

    public bool ShouldSwitchMode()
    {
        return currentTaskIndexInMode >= numTasksPerMode - 1;
    }

    public bool IsLastMode()
    {
        return currentModeIndex == getNumModes() - 1;
    }

    // Wrap functionality
    public void NextMode()
    {
        int modeIndex = currentModeIndex;
        modeIndex++;
        modeIndex %= getNumModes();
        game.CurrentMode = (TSMode) modeIndex;
    }

    public void CompleteTask(TSTaskDescriptor task)
    {
        game.CompletedTasks.Add(task);
        currentExperiment.AddDataRow(task.GetDataRow());
        if(verboseMode)
        {
            Debug.Log(currentExperiment.LastRowToString());
        }
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
            }
        }
        game.CurrentTaskIndex++;
    }

    int getNumModes()
    {
        return Enum.GetValues(typeof(TSMode)).Length;
    }

}

[System.Serializable]
public struct TSGameState
{
    public TSMode CurrentMode;
    public int CurrentTaskIndex;
    public List<TSTaskDescriptor> CompletedTasks;
}

[System.Serializable]
public struct TSTaskDescriptor
{
    public string BlockName;
    public int StimulusPosition; // 1,2,3,4 (top left, top right, bottom right, bottom left
    public int TaskType; // 1 for letter, 2 for number
    public int LetterStimulusIndex; // index of letter in list
    public int NumberStimulusIndex; // number value
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
            LetterStimulusIndex,
            NumberStimulusIndex,
            TypeOfBlock,
            IsNewTaskSwitch,
            ResponseStatus,
            ResponseTime,
            TotalTaskTime
        };
    }
}

public enum TSMode
{
    LeterRow,
    NumberRow,
    MixedRows
}
