using UnityEngine;

public class CurrentEnemies : MonoBehaviour
{
    [SerializeField] private bool _isView;
    [SerializeField] private TMPro.TMP_Text _enemiesText;
    [SerializeField] private string _text;
    [SerializeField] private AllAgents _allAgents;
    [SerializeField] private float _updateTime = 0.5f;

    private float _timeCurrent = 1;

    private void Start()
    {
        if (!_isView)
            _enemiesText.text = "";
    }

    private void Update()
    {
        if (!_isView) return;

        _timeCurrent += Time.deltaTime;
        if (_timeCurrent >= _updateTime)
        {
            _enemiesText.text = _text + ": " + _allAgents.GetLivingAgents();
            _timeCurrent = 0;
        }
    }
}
