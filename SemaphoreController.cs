using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class TrafficLightData
{
    //info received from jsons
    public Vector2 position; 
    public bool isGreen;     
}

[System.Serializable]
public class TrafficLightsResponse
{
    public List<TrafficLightData> trafficLights; 
}

public class SemaphoreController : MonoBehaviour
{
    public Light redLight;
    public Light greenLight;

    private bool isGreen; 

   
    [SerializeField] private Vector2 semaphorePosition;

    private void Start()
    {
        
        StartCoroutine(RequestTrafficLightData());
    }

    private IEnumerator RequestTrafficLightData()
    {
        string url = "url"; 
        while (true)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Error al acceder al endpoint: {www.error}");
                }
                else
                {
                    string jsonResponse = www.downloadHandler.text;
                    Debug.Log($"Respuesta del JSON:\n{jsonResponse}");

                    TrafficLightsResponse response = JsonUtility.FromJson<TrafficLightsResponse>(jsonResponse);
                    if (response != null)
                    {
                        UpdateTrafficLightState(response);
                    }
                    else
                    {
                        Debug.LogError("Error al deserializar el JSON.");
                    }
                }
            }

            
            yield return new WaitForSeconds(2f);
        }
    }

    private void UpdateTrafficLightState(TrafficLightsResponse response)
    {
        
        foreach (var lightData in response.trafficLights)
        {
            if (lightData.position == semaphorePosition) 
            {
                
                isGreen = lightData.isGreen;
                SetLightState(isGreen);
                break;
            }
        }
    }

    private void SetLightState(bool green)
    {
        
        greenLight.enabled = green;
        redLight.enabled = !green;
    }
}
