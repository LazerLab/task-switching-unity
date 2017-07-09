/*
 * Author(s): Isaiah Mann
 * Description: [to be added]
 * Usage: [no notes]
 */

using UnityEngine;
using UnityEngine.UI;

public class TSGamePiece : TSGameObject 
{	
	const int STIM_1_INDEX = 0;
	const int STIM_2_INDEX = 1;

	public TSPieceID ID
	{
		get;
		private set;
	}

	[SerializeField]
	GameObject allVisuals;

	[SerializeField]
	Text stimuli1TextDisplay;
	[SerializeField]
	Text stimuli2TextDisplay;

	[SerializeField]
	Image stimuli1ImageDisplay;
	[SerializeField]
	Image stimuli2ImageDisplay;

	public void SetPiece(TaskBatch batch)
	{
		if(batch is HybridTaskBatch)
		{
			setPieceHybrid(batch.GetSet() as ImageStimuliSet);
		}
		else if(batch is ImageTaskBatch)
		{
			ImageTaskBatch images = batch as ImageTaskBatch;
			ImageStimuliSet set = images.GetSet() as ImageStimuliSet;
			setPiece(set.Stimuli1Img, set.Stimuli2Img);
		}
		else
		{
			StimuliSet set = batch.GetSet();
			setPiece(set.Stimuli1, set.Stimuli2);
		}
		ToggleVisible(isVisibile:true);
	}

	void setPieceHybrid(ImageStimuliSet set)
	{
		toggleImageMode(set.HasImage1, STIM_1_INDEX);
		toggleImageMode(set.HasImage2, STIM_2_INDEX);
		if(set.HasImage1)
		{
			stimuli1ImageDisplay.sprite = set.Stimuli1Img;
		}
		else
		{
			stimuli1TextDisplay.text = set.Stimuli1;
		}
		if(set.HasImage2)
		{
			stimuli2ImageDisplay.sprite = set.Stimuli2Img;
		}
		else
		{
			stimuli2TextDisplay.text = set.Stimuli2;
		}
		if(set.HasImage1 && set.HasImage2)
		{
			this.ID = new TSPieceID(set.Stimuli1Img, set.Stimuli2Img, isHybrid:true);
		}
		else if(set.HasImage1 && !set.HasImage2)
		{
			this.ID = new TSPieceID(set.Stimuli1Img, set.Stimuli2);
		}
		else if(!set.HasImage1 && set.HasImage2)
		{
			this.ID = new TSPieceID(set.Stimuli1, set.Stimuli2Img);
		}
		else
		{
			this.ID = new TSPieceID(set.Stimuli1, set.Stimuli2, isHybrid:true);
		}
	}

	void setPiece(string stimuli1, string stimuli2)
	{
		toggleImageMode(false);
		this.stimuli1TextDisplay.text = stimuli1;
		this.stimuli2TextDisplay.text = stimuli2;
		this.ID = new TSPieceID(stimuli1, stimuli2);
	}

	void setPiece(Sprite image1, Sprite image2)
	{
		toggleImageMode(true);
		this.stimuli1ImageDisplay.sprite = image1;
		this.stimuli2ImageDisplay.sprite = image2;
		this.ID = new TSPieceID(image1, image2);
	}

	public void ToggleVisible(bool isVisibile)
	{
		allVisuals.SetActive(isVisibile);
	}

	void toggleImageMode(bool enabled, int index)
	{
		if(index == STIM_1_INDEX)
		{
			stimuli1ImageDisplay.enabled = enabled;
			stimuli1TextDisplay.enabled = !enabled;
		}
		else if(index == STIM_2_INDEX)
		{
			stimuli2ImageDisplay.enabled = enabled;
			stimuli2TextDisplay.enabled = !enabled;
		}
	}

	void toggleImageMode(bool enabled)
	{
		toggleImageMode(enabled, STIM_1_INDEX);
		toggleImageMode(enabled, STIM_2_INDEX);
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

	public bool IsHybrid
	{
		get;
		private set;
	}

	public TSPieceID(string stimuli1, Sprite stimuli2)
	{
		this.Stimuli1 = stimuli1;
		this.Stimuli2Image = stimuli2;
		this.IsHybrid = true;
	}

	public TSPieceID(Sprite stimuli1, string stimuli2)
	{
		this.Stimuli1Image = stimuli1;
		this.Stimuli2 = stimuli2;
		this.IsHybrid = true;
	}
		
	public TSPieceID(string stimuli1, string stimuli2)
	{
		this.Stimuli1 = stimuli1;
		this.Stimuli2 = stimuli2;
		this.IsImages = false;
	}

	public TSPieceID(string stimuli1, string stimuli2, bool isHybrid)
	{
		this.Stimuli1 = stimuli1;
		this.Stimuli2 = stimuli2;
		this.IsHybrid = isHybrid;
	}

	public TSPieceID(Sprite stimuli1, Sprite stimuli2)
	{
		this.Stimuli1Image = stimuli1;
		this.Stimuli2Image = stimuli2;
		this.IsImages = true;
	}

	public TSPieceID(Sprite stimuli1, Sprite stimuli2, bool isHybrid)
	{
		this.Stimuli1Image = stimuli1;
		this.Stimuli2Image = stimuli2;
		this.IsHybrid = isHybrid;
	}

}
