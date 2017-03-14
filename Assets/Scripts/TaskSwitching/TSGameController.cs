/*
 * Author(s): Isaiah Mann
 * Description: [to be added]
 * Usage: [no notes]
 */

using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class TSGameController : SingletonController<TSGameController>
{
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

	bool spawningActive = true;
	TSGamePiece[] boardPieces;
	TSGameTile[] boardTiles;
	TSGameTile activeTile;

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
	}

	void initBoardTiles(TSGameTile[] boardTiles)
	{
		for(int i = 0; i < boardTiles.Length; i++)
		{
			boardTiles[i].Init(i);
			trackTile(boardTiles[i]);
		}
	}

	TSGamePiece[] spawnPieces(bool startActive)
	{
		TSGamePiece[] boardPieces = new TSGamePiece[maxPiecesOnBoard];
		for(int i = 0; i < boardPieces.Length; i++)
		{
			boardPieces[i] = Instantiate(piecePrefab);
			boardPieces[i].transform.SetParent(boardParent);
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
		return piece;
	}

	TSGamePiece choosePieceToSpawn()
	{
		return boardPieces[Random.Range(0, boardPieces.Length)];
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
		if(hasActiveTile)
		{
			bool keyPressed = false;
			bool successfulPlacement = false;
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

}

public enum TSMatchCondition
{
	EvenNumber,
	OddNumber,
	VowelLetter,
	ConsonantLetter,
}

public enum TSMatchType
{
	Number,
	Letter,
}
