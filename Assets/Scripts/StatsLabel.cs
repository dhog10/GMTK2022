using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StatsLabel : MonoBehaviour
{
    public Stat Stat { get; set; }

    private TextMeshProUGUI _text;

    private void Start()
    {
        _text = this.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        var player = CharacterControl.Instance;
        if (player == null)
        {
            return;
        }

        var stat = player.Stats.FirstOrDefault(s => s.Name == this.Stat.Name);
        _text.text = stat == null ? "???" : $"{stat.Value} - {stat.Name}";
    }
}
