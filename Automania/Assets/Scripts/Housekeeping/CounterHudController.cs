using UnityEngine;

public class CounterHudController : MonoBehaviour
{
    private int counter;

    [SerializeField] private SpriteRenderer[] indicators;
    [SerializeField] private Sprite[] digits;

    public int Counter
    {
        get => counter;
        set
        {
            counter = value;

            foreach (var indicator in indicators)
            {
                indicator.sprite = digits[0];
            }

            var tmp = counter;
            var i = indicators.Length - 1;
            while (tmp > 0)
            {
                var digit = tmp % 10;
                indicators[i--].sprite = digits[digit];
                tmp -= digit;
                tmp /= 10;
            }
        }
    }
}
