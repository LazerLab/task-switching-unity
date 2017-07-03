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

	[SerializeField]
	Image stimuli1ImageDisplay;
	[SerializeField]
	Image stimuli2ImageDisplay;

	public void SetPiece(TaskBatch batch)
	{
		if(batch is ImageTaskBatch)
		{
			ImageTaskBatch images = batch as ImageTaskBatch;
			ImageStimuliSet set = images.GetSet();
			SetPiece(set.Stimuli1, set.Stimuli2);
		}
		else
		{
			StimuliSet set = batch.GetSet();
			SetPiece(set.Stimuli1, set.Stimuli2);
		}
	}

	public void SetPiece(string stimuli1, string stimuli2)
	{
		toggleImageMode(false);
		this.letterField.text = stimuli1;
		this.numberField.text = stimuli2;
		this.ID = new TSPieceID(stimuli1, stimuli2);
		ToggleVisible(isVisibile:true);
	}

	public void SetPiece(Sprite image1, Sprite image2)
	{
		toggleImageMode(true);
		this.stimuli1ImageDisplay.sprite = image1;
		this.stimuli2ImageDisplay.sprite = image2;
		this.ID = new TSPieceID(image1, image2);
		ToggleVisible(isVisibile:true);
	}

	public void ToggleVisible(bool isVisibile)
	{
		allVisuals.SetActive(isVisibile);
	}

	void toggleImageMode(bool enabled)
	{
		stimuli1ImageDisplay.enabled = enabled;
		stimuli2ImageDisplay.enabled = enabled;
		letterField.enabled = !enabled;
		numberField.enabled = !enabled;
	}

}

public class TSPieceID
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

	public Sprite Stimuli1Image
	{
		get;
		private set;
	}

	public Sprite Stimuli2Image
	{
		get;
		private set;
	}

	public bool IsImages
	{
		get;
		private set;
	}

	public TSPieceID(string stimuli1, string stimuli2)
	{
		this.Stimuli1 = stimuli1;
		this.Stimuli2 = stimuli2;
		this.IsImages = false;
	}

	public TSPieceID(Sprite stimuli1, Sprite stimuli2)
	{
		this.Stimuli1Image = stimuli1;
		this.Stimuli2Image = stimuli2;
		this.IsImages = true;
	}

}
	