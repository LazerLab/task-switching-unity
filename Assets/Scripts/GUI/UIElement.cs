/*
 * Author(s): Isaiah Mann
 * Description: [to be added]
 * Usage: [no notes]
 */

using UnityEngine;
using UnityEngine.UI;

public class UIElement : MonoBehaviourExtended 
{	
	protected Text text;
	protected Image image;

	public void SetText(string text)
	{
		if(this.text)
		{
			this.text.text = text;
		}
	}

	public void SetImageColorOverlay(Color color)
	{
		if(image)
		{
			image.color = color;
		}
	}

	protected override void setReferences()
	{
		base.setReferences();
		text = GetComponentInChildren<Text>();
		image = GetComponentInChildren<Image>();
	}

}
