using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using TMPro;

public class MovieGenre : MonoBehaviour
{
    public TMP_Text genreText;
    public float influenceRadius = 300f;
    public string apiKey = "REDACTED";
    public string apiEndpoint = "REDACTED";

    private Dictionary<string, Vector3> zoneCenters = new Dictionary<string, Vector3>
    {
        { "Drama",     new Vector3(165, 40, 128) },
        { "Adventure", new Vector3(720, 25, 128) },
        { "Sci-Fi",    new Vector3(720, 25, 1000) },
        { "Action",    new Vector3(180, 40, 49) },
        { "Horror",    new Vector3(320, 215, 612) }
    };

    void Start()
    {
        Dictionary<string, float> distances = new Dictionary<string, float>();
        foreach (var zone in zoneCenters)
        {
            float dist = Vector3.Distance(transform.position, zone.Value);
            distances[zone.Key] = Mathf.Clamp01(1f - dist / influenceRadius);
        }

        float total = distances.Values.Sum();
        if (total == 0f) total = 1f;

        Dictionary<string, int> genreScores = distances.ToDictionary(
            pair => pair.Key,
            pair => Mathf.RoundToInt((pair.Value / total) * 10)
        );

        string prompt = BuildPrompt(genreScores);
        StartCoroutine(SendPrompt(prompt));
    }

    string BuildPrompt(Dictionary<string, int> genreScores)
    {
        string genreList = string.Join(", ", genreScores.OrderByDescending(g => g.Value)
            .Select(g => $"{g.Key}: {g.Value}/10"));

        return $"You are a movie generator. Given the genre weights {genreList}, return only a short movie title and description under 30 words that matches the distribution. Respond with nothing else.";
    }

    IEnumerator SendPrompt(string prompt)
    {
        string json = JsonUtility.ToJson(new
        {
            model = "REDACTED",
            messages = new[] {
                new { role = "user", content = prompt }
            }
        });

        using (UnityWebRequest request = new UnityWebRequest(apiEndpoint, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                if (genreText != null) genreText.text = "API Error";
            }
            else
            {
                string response = request.downloadHandler.text;
                string content = ExtractContent(response);
                if (genreText != null) genreText.text = content;
            }
        }
    }

    string ExtractContent(string json)
    {
        var wrapper = JsonUtility.FromJson<ResponseWrapper>(json);
        return wrapper.choices[0].message.content.Trim();
    }

    [System.Serializable]
    public class ResponseWrapper
    {
        public Choice[] choices;

        [System.Serializable]
        public class Choice
        {
            public Message message;

            [System.Serializable]
            public class Message
            {
                public string content;
            }
        }
    }
}