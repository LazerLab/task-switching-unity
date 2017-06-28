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

	public ImageTaskBatch(string batchKey, Action onStimuliFetched) : base(batchKey, onStimuliFetched)
	{
		sprites = new Sprite[][][]
		{
			new Sprite[2][],
			new Sprite[2][],
		};
	}

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

	#region TaskBatch Overrides

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
			sprites[0][0] != null &&
			sprites[0][1] != null &&
			sprites[1][0] != null &&
			sprites[1][1] != null;
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
		sprites[stimuliIndex][stimuliIndex] = new Sprite[spriteNames.Length];
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
				checkToRunCallback();
			});
	}

	Sprite randomStimuli1Sprite()
	{
		return firstStimuliOptions[UnityEngine.Random.Range(0, firstStimuliOptions.Length)];
	}

	Sprite randomStimuli2Sprite()
	{
		return secondStimuliOptions[UnityEngine.Random.Range(0, firstStimuliOptions.Length)];
	}

	bool sameSprite(Sprite s1, Sprite s2)
	{
		return s1 == s2;
	}

}

public struct ImageStimuliSet
{
	public Sprite Stimuli1;
	public Sprite Stimuli2;

	public ImageStimuliSet(Sprite stimuli1, Sprite stimuli2)
	{
		this.Stimuli1 = stimuli1;
		this.Stimuli2 = stimuli2;
	}

}