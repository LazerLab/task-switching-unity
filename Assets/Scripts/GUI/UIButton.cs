/*
 * Author(s): Isaiah Mann
 * Description: [to be added]
 * Usage: [no notes]
 */

using UnityEngine;

public class UIButton : UIElement 
{	
    public bool IsPressed
    {
        get
        {
            return this.isPressed;
        }
    }

	[SerializeField]
	Color pressedColor = Color.gray;
	[SerializeField]
	Color activeColor = Color.white;
	[SerializeField]
	Color inactiveColor = new Color(0.9f, 0.9f, 0.9f, 0.5f);

	bool isPressed = false;

	public void BeginPress()
	{
		SetImageColorOverlay(pressedColor);
		isPressed = true;
	}

	public void EndPress()
	{
		SetImageColorOverlay(activeColor);
		isPressed = false;
	}

	public void SetInactive()
	{
		SetImageColorOverlay(inactiveColor);
	}

	public void SetActive()
	{
		SetImageColorOverlay(activeColor);
	}

}
