/*
 * Author(s): Isaiah Mann
 * Description: [to be added]
 * Usage: [no notes]
 */

using UnityEngine;
using UnityEngine.UI;

public class TSGamePiece : TSGameObject 
{	
	public TSPieceID ID
	{
		get;
		private set;
	}

	[SerializeField]
	GameObject allVisuals;

	[SerializeField]
	Text letterField;
	[SerializeField]
	Text numberField;

	public void SetPiece(char letter, int number)
	{
		this.letterField.text = letter.ToString();
		this.numberField.text = number.ToString();
		this.ID = new TSPieceID(letter, number);
		ToggleVisible(isVisibile:true);
	}

	public void ToggleVisible(bool isVisibile)
	{
		allVisuals.SetActive(isVisibile);
	}
		
}

public struct TSPieceID
{
	public char Letter
	{
		get;
		private set;
	}

	public int Number
	{
		get;
		private set;
	}

	public TSPieceID(char letter, int number)
	{
		this.Letter = letter;
		this.Number = number;
	}

}