using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class SkyRecommendationDisplay : MonoBehaviour
{
    public string cloudFunctionUrl = "REDACTED";
    private string userId = "REDACTED";

    [System.Serializable]
    public class RecommendationResponse
    {
        public List<string> recommendations;
    }

    public GameObject[] recommendationOrbs = new GameObject[5];

    void Start()
    {
        StartCoroutine(CallCloudFunction());
    }

    IEnumerator CallCloudFunction()
    {
        string requestUrl = $"{cloudFunctionUrl}?userId={userId}";
        UnityWebRequest request = UnityWebRequest.Get(requestUrl);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            yield break;
        }

        string wrappedJson = "{\"recommendations\":" + request.downloadHandler.text + "}";
        RecommendationResponse response = JsonUtility.FromJson<RecommendationResponse>(wrappedJson);

        for (int i = 0; i < recommendationOrbs.Length && i < response.recommendations.Count; i++)
        {
            TextMesh textMesh = recommendationOrbs[i].GetComponentInChildren<TextMesh>();
            if (textMesh != null)
            {
                textMesh.text = response.recommendations[i];
            }
        }
    }
}