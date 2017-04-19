/*
 * Author(s): Isaiah Mann
 * Description: [to be added]
 * Usage: [no notes]
 */

using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using VolunteerScience;

public class TSGameController : SingletonController<TSGameController>
{
    const string CURRENT_TASK_FORMAT = "Task {0}";
    const string SPAWN_DELAY_KEY = "spawnDelay";

	[Header("Game Logic")]

	[SerializeField]
	int maxPiecesOnBoard = 4;

	[SerializeField]
	float spawnDelay = 2f;

	[SerializeField]
	char[] validVowels;
	[SerializeField]
	char[] validConsonants;

	char[] validLetters
	{
		get
		{
			return ArrayUtil.Concat(validVowels, validConsonants);
		}
	}

    bool hasCurrentTask
    {
        get
        {
            return currentTask != null;
        }
    }

	[SerializeField]
	int[] validEvenNumbers;
	[SerializeField]
	int[] validOddNumbers;

	int[] validNumbers
	{
		get
		{
			return ArrayUtil.Concat(validEvenNumbers, validOddNumbers);
		}
	}

	[Header("Control Scheme")]

	[SerializeField]
	KeyCode leftKey = KeyCode.B;

	[SerializeField]
	KeyCode rightKey = KeyCode.N;

	[Header("Game Components")]
	[SerializeField]
	UIButton leftButton;
	[SerializeField]
	UIButton rightButton;
	[SerializeField]
	TSGamePiece piecePrefab;

	[SerializeField]
	Transform boardParent;

    [Header("Debugging")]
    [SerializeField]
    bool verboseMode = true;

	bool spawningActive = true;
	TSGamePiece[] boardPieces;
	TSGameTile[] boardTiles;
	TSGameTile activeTile;
    TSDataController data;
    TSTaskDescriptor currentTask;
    VariableFetcher fetcher;

    double responseTime = 0;
    double taskTime = 0;

	bool hasActiveTile
	{
		get
		{
			return activeTile != null;
		}
	}

	Dictionary<TSMatchCondition, TSGameTile> tileMatches = new Dictionary<TSMatchCondition, TSGameTile>();

	IEnumerator spawningRoutine;

	public void StartSpawning()
	{
		spawningRoutine = spawnCoroutine();
		StartCoroutine(spawningRoutine);
	}

	public void StopSpawning()
	{
		if(spawningRoutine != null)
		{
			StopCoroutine(spawningRoutine);
		}
	}

	protected override void setReferences()
	{
		base.setReferences();
		this.boardPieces = spawnPieces(startActive:false);
		this.boardTiles = GetComponentsInChildren<TSGameTile>();
		initBoardTiles(boardTiles);
		StartSpawning();
	}

	protected override void fetchReferences()
	{
		base.fetchReferences();
		leftButton.SetText(leftKey.ToString());
		rightButton.SetText(rightKey.ToString());
        data = TSDataController.Instance;
        data.SubscribeToGameEnd(handleGameEnd);
        fetcher = VariableFetcher.Get;
        fetchTunableVariables();
	}
        
    protected override void cleanupReferences()
    {
        base.cleanupReferences();
        data.UnsubscribeFromGameEnd(handleGameEnd);
    }

    void fetchTunableVariables()
    {
        fetchSpawnDelay();
    }

	void initBoardTiles(TSGameTile[] boardTiles)
	{
		for(int i = 0; i < boardTiles.Length; i++)
		{
			boardTiles[i].Init(i);
			trackTile(boardTiles[i]);
		}
	}

    void recordReponseTime()        
    {
        this.responseTime = data.GetEventTime(getTaskKey());
    }

    void recordTaskTime()
    {
        this.taskTime = data.GetEventTime(getTaskKey());
    }

	TSGamePiece[] spawnPieces(bool startActive)
	{
		TSGamePiece[] boardPieces = new TSGamePiece[maxPiecesOnBoard];
		for(int i = 0; i < boardPieces.Length; i++)
		{
			boardPieces[i] = Instantiate(piecePrefab);
			boardPieces[i].transform.SetParent(boardParent);
            boardPieces[i].transform.localScale = Vector3.one;
			boardPieces[i].Init(index:i);
			boardPieces[i].ToggleVisible(startActive);
		}
		return boardPieces;
	}

	void trackTile(TSGameTile tile)
	{
		if(tileMatches.ContainsKey(tile.GetMatchCondition))
		{
			tileMatches[tile.GetMatchCondition] = tile;
		}
		else
		{
			tileMatches.Add(tile.GetMatchCondition, tile);
		}
	}

	TSGamePiece spawnPiece(char pieceLetter, int pieceNumber)
	{
		toggleAllPiecesVisible(isVisible:false);
		TSGamePiece piece = choosePieceToSpawn();
		int pieceIndex = ArrayUtil.IndexOf(boardPieces, piece);
		this.activeTile = boardTiles[pieceIndex];
		piece.SetPiece(pieceLetter, pieceNumber);
		activeTile.SetPiece(piece);
        this.currentTask = trackTask(pieceLetter, pieceNumber, pieceIndex);
		return piece;
	}

    TSTaskDescriptor trackTask(char pieceLetter, int pieceNumber, int stimulusPosition)
    {
        TSTaskDescriptor task = new TSTaskDescriptor();
        task.BlockName = data.CurrentMode.ToString();
        task.StimulusPosition = stimulusPosition;
        task.TaskType = (int) boardTiles[stimulusPosition].GetMatchType + 1;
        task.LetterStimulusIndex = ArrayUtil.IndexOf(validLetters, pieceLetter);
        task.NumberStimulus = pieceNumber;
        task.TypeOfBlock = ((int) data.CurrentMode) + 1;
        TSTaskType taskType =  data.IsTaskSwitch ? TSTaskType.TaskSwitch : TSTaskType.TaskRepeat;
        task.IsNewTaskSwitch = (int) taskType;
        data.StartTimer(getTaskKey());
        return task;
    }

    string getTaskKey()
    {
        return string.Format(CURRENT_TASK_FORMAT, data.CurrentTaskIndex);
    }

    void sendTask(TSTaskDescriptor task, TSResponseStatus response, double responseTime, double taskTime)
    {
        task.ResponseStatus = (int) response;
        task.ResponseTime = responseTime;
        task.TotalTaskTime = taskTime;
        data.CompleteTask(task);
        resetTaskData();
        data.NextTask();
    }

    void resetTaskData()
    {
        this.currentTask = null;
        this.responseTime = 0;
        this.taskTime = 0;
    }

	TSGamePiece choosePieceToSpawn()
	{
        switch(data.CurrentMode)
        {
            case TSMode.LeterRow:
                return boardPieces[Random.Range(0, boardPieces.Length / 2)];
            case TSMode.NumberRow:
                return boardPieces[Random.Range(boardPieces.Length / 2, boardPieces.Length)];
            default:
                return boardPieces[Random.Range(0, boardPieces.Length)];
        }
	}
		
	char chooseLetter()
	{
		return validLetters[Random.Range(0, validLetters.Length)];
	}

	int chooseNumber()
	{ 
		return validNumbers[Random.Range(0, validNumbers.Length)];
	}

	void toggleAllPiecesVisible(bool isVisible)
	{
		foreach(TSGamePiece piece in boardPieces)
		{
			piece.ToggleVisible(isVisible);
		}
	}

	void Update()
	{
        bool successfulPlacement = false;
		if(hasActiveTile)
		{
			bool keyPressed = false;
			TSGameTile targetTile = null;
			if(Input.GetKeyDown(leftKey))
			{
				keyPressed = true;
				successfulPlacement = tryPlacePiece(leftKey, out targetTile);
				leftButton.BeginPress();
			}
			else if(Input.GetKeyDown(rightKey))
			{
				keyPressed = true;
				successfulPlacement = tryPlacePiece(rightKey, out targetTile);
				rightButton.BeginPress();
			}
			if(keyPressed && targetTile != null)
			{
				targetTile.TimedShowIcon(successfulPlacement);
                if(hasCurrentTask)
                {
                    recordReponseTime();
                }
			}
		}
		bool buttonWasUp = false;
		if(Input.GetKeyUp(leftKey))
		{
			leftButton.EndPress();
			buttonWasUp = true;
		}
		else if(Input.GetKeyUp(rightKey))
		{
			rightButton.EndPress();
			buttonWasUp = true;
		}
		if(buttonWasUp)
		{
			rightButton.SetInactive();
			leftButton.SetInactive();
            if(hasCurrentTask) 
            {
                recordTaskTime();
                sendTask(currentTask,
                    successfulPlacement ? TSResponseStatus.Correct : TSResponseStatus.Error,
                    responseTime,
                    taskTime);

            }
		}
	}

	void clearActiveTile()
	{
		activeTile.ClearPiece();
		activeTile = null;
	}

	bool tryPlacePiece(KeyCode key, out TSGameTile targetTile)
	{
		targetTile = getTargetTile(key, activeTile);
		bool valid = isValidPlacement(activeTile, targetTile);
		clearActiveTile();
		return valid;
	}

	bool isValidPlacement(TSGameTile sourceTile, TSGameTile targetTile)
	{
		TSPieceID id = sourceTile.GetPiece.ID;
		switch(targetTile.GetMatchCondition)
		{
			case TSMatchCondition.ConsonantLetter:
				return isValidConsonant(id.Letter);
			case TSMatchCondition.VowelLetter:
				return isValidVowel(id.Letter);
			case TSMatchCondition.EvenNumber:
				return isValidEvenNumber(id.Number);
			case TSMatchCondition.OddNumber:
				return isValidOddNumber(id.Number);
			default:
				return false;
		}

	}

    void handleGameEnd()
    {
        StopCoroutine(spawningRoutine);
    }

	TSGameTile getTargetTile(KeyCode key, TSGameTile occupiedTile)
	{
		TSMatchCondition targetCondition = getMatchCondition(key, occupiedTile.GetMatchType);
		TSGameTile targetTile;
		if(tileMatches.TryGetValue(targetCondition, out targetTile))
		{
			return targetTile;
		}
		else
		{
			Debug.LogError("Could not find specified target tile");
			return null;
		}
	}

	TSMatchCondition getMatchCondition(KeyCode key, TSMatchType type)
	{
		if(key == leftKey)
		{
			if(type == TSMatchType.Letter)
			{
				return TSMatchCondition.ConsonantLetter;
			}
			else if (type == TSMatchType.Number)
			{
				return TSMatchCondition.OddNumber;
			}
		}
		else if(key == rightKey)
		{
			if(type == TSMatchType.Letter)
			{
				return TSMatchCondition.VowelLetter;
			}
			else if (type == TSMatchType.Number)
			{
				return TSMatchCondition.EvenNumber;
			}
		}

		// Default return condition
		return default(TSMatchCondition);
	}

	IEnumerator spawnCoroutine()
	{
		while(this.spawningActive)
		{
			yield return new WaitForSeconds(spawnDelay);
            if(hasCurrentTask)
            {
                sendTask(currentTask, TSResponseStatus.TooSlow, 0, 0);
            }
            rightButton.SetActive();
			leftButton.SetActive();
			toggleAllTileIcons(visible:false);
			spawnPiece(chooseLetter(), chooseNumber());
		}
	}

	void toggleAllTileIcons(bool visible)
	{
		foreach(TSGameTile tile in boardTiles)
		{
			tile.ToggleAllIcons(visible);
		}
	}

	bool isValidVowel(char letter)
	{
		return ArrayUtil.Contains(this.validVowels, letter);
	}

	bool isValidConsonant(char letter)
	{
		return ArrayUtil.Contains(this.validConsonants, letter);
	}

	bool isValidEvenNumber(int number)
	{
		return ArrayUtil.Contains(this.validEvenNumbers, number);
	}

	bool isValidOddNumber(int number)
	{
		return ArrayUtil.Contains(this.validOddNumbers, number);
	}

    void fetchSpawnDelay()
    {
        fetcher.GetValue(SPAWN_DELAY_KEY, setSpawnDelay);
    }

    void setSpawnDelay(object value)
    {
        try
        {
            if(verboseMode)
            {
                Debug.LogFormat("Retrieved value {0} for spawnDelay", value);
            }
            float valueAsFloat = float.Parse(value.ToString());
            this.spawnDelay = valueAsFloat;
        }
        catch
        {
            Debug.LogError("Unable to set spawn delay. Could not cast value to float");
        }
    }

}
