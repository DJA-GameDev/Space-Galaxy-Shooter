using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossLifeBar : MonoBehaviour
{
    [SerializeField]
    private Slider _slider;

    // Start is called before the first frame update
    public void UpdateBossLifeBar(int _currentLives)
    { 
        _slider.value = _currentLives;
    }
}
