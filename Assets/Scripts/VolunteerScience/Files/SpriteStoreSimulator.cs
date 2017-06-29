/*
 * Author(s): Isaiah Mann
 * Description: Used to simulate loading images from Volunteer Science
 * Usage: [no notes]
 */

namespace VolunteerScience
{
	using System;
	using System.Collections.Generic;

	using UnityEngine;

	public class SpriteStoreSimulator : Singleton<SpriteStoreSimulator>
	{
		[SerializeField]
		Sprite[] sprites;

		Dictionary<string, Sprite> spriteLookup;

		public void LoadImage(string fileName, Action<Sprite> callback)
		{
			Sprite sprite;
			if(spriteLookup.TryGetValue(fileName, out sprite))
			{
				callback(sprite);
			}
			else
			{
				Debug.LogErrorFormat("Sprite {0} could not be found", fileName);
			}
		}

		#region Singleton Overrides

		protected override void Awake()
		{
			base.Awake();
			initSpriteLookup();
		}

		#endregion

		void initSpriteLookup()
		{
			spriteLookup = new Dictionary<string, Sprite>();
			foreach(Sprite sprite in sprites)
			{
				spriteLookup[sprite.name] = sprite;
			}
		}

	}

}
