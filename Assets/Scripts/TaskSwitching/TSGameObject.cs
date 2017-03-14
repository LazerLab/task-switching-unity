/*
 * Author(s): Isaiah Mann
 * Description: [to be added]
 * Usage: [no notes]
 */

public class TSGameObject : MonoBehaviourExtended 
{	
	public int Index
	{
		get;
		private set;
	}

	public void Init(int index)
	{
		this.Index = index;
	}

}
