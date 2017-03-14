/*
 * Author: Isaiah Mann
 * Description: Abstract data class
 */

using k = Global;

[System.Serializable]
public abstract class SerializableData 
{
	protected const string TIME_STAMP = k.TIME_STAMP;

    protected const int NONE_VALUE = k.NONE_VALUE;

	const float FULL_PERCENT = k.FULL_PERCENT_F;

	protected float percentToDecimal(int percent)
	{
		return percentToDecimal((float) percent);
	}

	protected float percentToDecimal(float percent)
	{
		return percent / FULL_PERCENT;
	}

}
