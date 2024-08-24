using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bandwidth_Displayer : MonoBehaviour
{
    [System.Serializable]
    public enum Bandwidth_Type { TCP = 1, UDP = 2}


    public Bandwidth_Type bwtype;
    public TMP_Text bandwidthText; // Referencja do obiektu Text w UI do wyœwietlania przepustowoœci
    public float updateInterval = 1.0f; // Interwa³ aktualizacji przepustowoœci (w sekundach)

    private float bytesReceived;
    private float bandwidth;
    private float lastUpdateTime;

    private void Start()
    {
        if (bwtype == Bandwidth_Type.TCP)
        {
            Net_ConnectionHandler.Instance.OnBandwidthRecivedGS += AddBytesReceived;
        }
        else
        {
            Net_ConnectionHandler.Instance.OnBandwidthRecivedZS += AddBytesReceived;
        }

        lastUpdateTime = Time.time;
        StartCoroutine(UpdateBandwidthDisplay());
    }

    public void AddBytesReceived(int bytes)
    {
        bytesReceived += bytes;
    }

    private IEnumerator UpdateBandwidthDisplay()
    {
        while (true)
        {
            // Oblicz przepustowoœæ w b/s
            bandwidth = bytesReceived / updateInterval;
            bytesReceived = 0;

            // Aktualizuj tekst w UI
            bandwidthText.text = $"Bandwidth: {bandwidth.ToString("F2")} bytes/s";

            // Resetuj licznik i czas ostatniej aktualizacji
            lastUpdateTime = Time.time;
            yield return new WaitForSeconds(updateInterval);
        }
    }
}
