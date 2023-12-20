using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUnit : MonoBehaviour
{
    #region Stats
    public string symbol;
    public byte level;
    public short damage;
    public short health;
    #endregion

    #region Graphics
    public TMP_Text symbolText;
    public TMP_Text damageText;
    public TMP_Text healthText;
    public Sprite sprite;
    public Image image;
    #endregion

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
