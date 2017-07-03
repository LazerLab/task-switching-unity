/*
 * Author(s): Isaiah Mann
 * Description: Loading files
 * Usage: [no notes]
 */

namespace VolunteerScience
{
	using System;
	using System.Collections;

	using UnityEngine;

	public class FileLoader : Singleton<FileLoader>
	{
		const string FILE_URL_KEY = "vs_file_url";

		public void LoadImage(string fileName, Action<Sprite> callback)
		{
			#if UNITY_EDITOR

			if(SpriteStoreSimulator.HasInstance)
			{
				SpriteStoreSimulator.Get.LoadImage(fileName, callback);
			}

			#else

			GetFileURL(fileName, delegate(string fileURL)
				{
					StartCoroutine(loadTexture(fileURL, callback));
				});

			#endif
		}

		public StringFetchAction GetFileURL(string fileName, Action<string> callback)
		{
			return VariableFetcher.Get.GetString(getFileURLFetchCallback(fileName), 
				delegate(string fileURL)
				{
					// The VS getFile method returns a relative URL, need to add the Volunteer Science domain
					string urlWithVolunteerScienceDomain = replaceURLHostName(fileURL);
					callback(urlWithVolunteerScienceDomain);
				}
			);
		}
			
		IEnumerator loadTexture(string url, Action<Sprite> callback)
		{
			Texture2D texture;
			texture = new Texture2D(4, 4, TextureFormat.DXT1, false);
			WWW webRequest = new WWW(url);
			yield return webRequest;
			webRequest.LoadImageIntoTexture(texture);
			Rect textureRect = new Rect(0, 0, texture.width, texture.height);
			Sprite sprite = Sprite.Create(texture, textureRect, textureRect.center);
			callback(sprite);
		}

		string getFileURLFetchCallback(string fileName)
		{
			return VariableFetcher.Get.FormatFetchCall(FILE_URL_KEY, fileName);
		}

		// Adapated from: https://stackoverflow.com/questions/479799/replace-host-in-uri
		string replaceURLHostName(string originalURL)
		{
			return string.Format("{0}{1}", Global.WEB_DOMAIN, originalURL);
		}

	}

}
