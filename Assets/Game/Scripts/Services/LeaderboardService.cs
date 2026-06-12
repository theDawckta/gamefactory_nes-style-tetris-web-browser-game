using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Tetris.Services
{
    public class LeaderboardService : MonoBehaviour
    {
        [SerializeField]
        private string _serverBaseUrl = "http://localhost:3000";

        public void FetchScores(Action<List<LeaderboardEntry>> onComplete)
        {
            StartCoroutine(FetchScoresCoroutine(onComplete));
        }

        public void SubmitScore(string initials, int score, Action<List<LeaderboardEntry>> onComplete)
        {
            StartCoroutine(SubmitScoreCoroutine(initials, score, onComplete));
        }

        private System.Collections.IEnumerator FetchScoresCoroutine(Action<List<LeaderboardEntry>> onComplete)
        {
            string url = $"{_serverBaseUrl}/scores";
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.timeout = 5;
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        string json = request.downloadHandler.text;
                        List<LeaderboardEntry> entries = DeserializeScores(json);
                        onComplete?.Invoke(entries);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to deserialize leaderboard scores: {ex.Message}");
                        onComplete?.Invoke(null);
                    }
                }
                else
                {
                    Debug.LogError($"Failed to fetch scores from {url}: {request.error}");
                    onComplete?.Invoke(null);
                }
            }
        }

        private System.Collections.IEnumerator SubmitScoreCoroutine(string initials, int score, Action<List<LeaderboardEntry>> onComplete)
        {
            string url = $"{_serverBaseUrl}/scores";
            var submitData = new SubmitScoreRequest { initials = initials, score = score };
            string json = JsonUtility.ToJson(submitData);

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.timeout = 5;

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        string responseJson = request.downloadHandler.text;
                        List<LeaderboardEntry> entries = DeserializeScores(responseJson);
                        onComplete?.Invoke(entries);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to deserialize score submission response: {ex.Message}");
                        onComplete?.Invoke(null);
                    }
                }
                else
                {
                    Debug.LogError($"Failed to submit score to {url}: {request.error}");
                    onComplete?.Invoke(null);
                }
            }
        }

        private List<LeaderboardEntry> DeserializeScores(string json)
        {
            // JsonUtility cannot deserialize top-level arrays, so wrap in a helper object
            ScoresWrapper wrapper = JsonUtility.FromJson<ScoresWrapper>($"{{\"items\":{json}}}");
            return wrapper != null ? new List<LeaderboardEntry>(wrapper.items) : null;
        }

        [System.Serializable]
        private class SubmitScoreRequest
        {
            public string initials;
            public int score;
        }

        [System.Serializable]
        private class ScoresWrapper
        {
            public LeaderboardEntry[] items;
        }
    }
}
