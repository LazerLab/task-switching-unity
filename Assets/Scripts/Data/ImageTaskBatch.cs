/*
 * Author(s): Isaiah Mann
 * Description: Describes a task batch where the stimuli are images
 * Usage: [no notes]
 */

using System;

using UnityEngine;

using VolunteerScience;

public class ImageTaskBatch : TaskBatch
{
	Sprite[] firstStimuliOptions;
	Sprite[] secondStimuliOptions;

	Sprite[][][] sprites;

	public ImageTaskBatch(string batchKey, Action onStimuliFetched) : base(batchKey, onStimuliFetched){}

	public new ImageStimuliSet GetSet()
	{
		return new ImageStimuliSet(randomStimuli1Sprite(), randomStimuli2Sprite());
	}

	public bool IsValidStimuli1Category1(Sprite stimuli)
	{
		return ArrayUtil.Contains(sprites[0][0], stimuli, sameSprite);
	}

	public bool IsValidStimuli1Category2(Sprite stimuli)
	{
		return ArrayUtil.Contains(sprites[0][1], stimuli, sameSprite);
	}

	public bool IsValidStimuli2Category1(Sprite stimuli)
	{
		return ArrayUtil.Contains(sprites[1][0], stimuli, sameSprite);
	}

	public bool IsValidStimuli2Category2(Sprite stimuli)
	{
		return ArrayUtil.Contains(sprites[1][1], stimuli, sameSprite);
	}

	public int GetStimuli1Index(Sprite stimuli)
	{
		return ArrayUtil.IndexOf(firstStimuliOptions, stimuli, sameSprite);
	}

	public int GetStimuli2Index(Sprite stimuli)
	{
		return ArrayUtil.IndexOf(secondStimuliOptions, stimuli, sameSprite);
	}


	#region TaskBatch Overrides

	protected override void earlyInit()
	{
		base.earlyInit();
		sprites = new Sprite[][][]
		{
			new Sprite[2][],
			new Sprite[2][],
		};
	}

	protected override void getFirstStimuliCategory1Options(string[] options)
	{
		base.getFirstStimuliCategory1Options(options);
		loadSpriteList(options, 0, 0);
	}

	protected override void getFirstStimuliCategory2Options(string[] options)
	{
		base.getFirstStimuliCategory2Options(options);
		loadSpriteList(options, 0, 1);
	}

	protected override void getSecondStimuliCategory1Options(string[] options)
	{
		base.getSecondStimuliCategory1Options(options);
		loadSpriteList(options, 1, 0);
	}

	protected override void getSecondStimuliCategory2Options(string[] options)
	{
		base.getSecondStimuliCategory2Options(options);
		loadSpriteList(options, 1, 1);
	}
		
	protected override bool shouldRunCallback()
	{
		return base.shouldRunCallback() &&
			arrayReady(sprites[0][0]) &&
			arrayReady(sprites[0][1]) &&
			arrayReady(sprites[1][0]) &&
			arrayReady(sprites[1][1]);
	}

	protected override void runCallback()
	{
		base.runCallback();
		firstStimuliOptions = ArrayUtil.Concat(sprites[0][0], sprites[0][1]);
		secondStimuliOptions = ArrayUtil.Concat(sprites[1][0], sprites[1][1]);
	}

	#endregion

	Sprite[] getSpriteArr(int stimuliIndex, int categoryIndex)
	{
		return sprites[stimuliIndex][categoryIndex];
	}

	void loadSpriteList(string[] spriteNames, int stimuliIndex, int categoryIndex)
	{
		sprites[stimuliIndex][categoryIndex] = new Sprite[spriteNames.Length];
		for(int i = 0; i < spriteNames.Length; i++)
		{
			loadSpriteIntoArr(spriteNames[i], stimuliIndex, categoryIndex, i);
		}
	}

	void loadSpriteIntoArr(string spriteName, int stimuliIndex, int categoryIndex, int spriteIndex)
	{
		FileLoader.Get.LoadImage(spriteName, delegate(Sprite sprite)
			{
				getSpriteArr(stimuliIndex, categoryIndex)[spriteIndex] = sprite;
				if(sprite == null)
				{
					Debug.LogError(spriteName);
				}
				checkToRunCallback();
			});
	}

	Sprite randomStimuli1Sprite()
	{
		return firstStimuliOptions[UnityEngine.Random.Range(0, firstStimuliOptions.Length)];
	}

	Sprite randomStimuli2Sprite()
	{
		return secondStimuliOptions[UnityEngine.Random.Range(0, secondStimuliOptions.Length)];
	}

	bool sameSprite(Sprite s1, Sprite s2)
	{
		return s1 == s2;
	}

	bool arrayReady(Sprite[] sprites)
	{
		if(sprites == null)
		{
			return false;
		}
		for(int i = 0; i < sprites.Length; i++)
		{
			if(sprites[i] == null)
			{
				return false;
			}
		}
		return true;
	}

}

public class ImageStimuliSet : StimuliSet
{
	public new Sprite Stimuli1;
	public new Sprite Stimuli2;

	public ImageStimuliSet(Sprite stimuli1, Sprite stimuli2) : base()
	{
		this.Stimuli1 = stimuli1;
		this.Stimuli2 = stimuli2;
	}

}