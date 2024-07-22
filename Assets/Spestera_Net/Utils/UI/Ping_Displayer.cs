using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Ping_Displayer : MonoBehaviour
{
    public TMP_Text pingdDisplayer;
    private int _ping;
    private int _totalPing;

    public int _tickRate = 1;
    private int _currentTickrate;

    private void Start()
    {
        Net_HeartbeatHandler.Instance.OnUpdatePing_event += AddPing;
        StartCoroutine(UpdatePingDisplay());
    }

    private void AddPing()
    {
        _tickRate++;
    }

    private IEnumerator UpdatePingDisplay()
    {
        while (true)
        {
            _currentTickrate = _tickRate;
            if (_tickRate > 0)
            {
                _ping = _totalPing / _tickRate;
                _totalPing = 0;
                _tickRate = 0;
            }

            pingdDisplayer.text = $"tickrate: {_currentTickrate}/s";
            yield return new WaitForSeconds(1);
        }
    }
}
