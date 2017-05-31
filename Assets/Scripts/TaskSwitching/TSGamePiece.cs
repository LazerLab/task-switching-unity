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

	public void SetPiece(string stimuli1, string stimuli2)
	{
		this.letterField.text = stimuli1;
		this.numberField.text = stimuli2;
		this.ID = new TSPieceID(stimuli1, stimuli2);
		ToggleVisible(isVisibile:true);
	}

	public void ToggleVisible(bool isVisibile)
	{
		allVisuals.SetActive(isVisibile);
	}
		
}

public struct TSPieceID
{
	public string Stimuli1
	{
		get;
		private set;
	}

	public string Stimuli2
	{
		get;
		private set;
	}

	public TSPieceID(string stimuli1, string stimuli2)
	{
		this.Stimuli1 = stimuli1;
		this.Stimuli2 = stimuli2;
	}

}