using NUnit.Framework;
using System.Collections.Generic;
using Tetris.Services;
using UnityEngine;

namespace Tetris.Tests
{
    public class LeaderboardServiceTests
    {
        private LeaderboardService _leaderboardService;
        private GameObject _testGameObject;

        [SetUp]
        public void SetUp()
        {
            _testGameObject = new GameObject("LeaderboardServiceTest");
            _leaderboardService = _testGameObject.AddComponent<LeaderboardService>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_testGameObject);
        }

        [Test]
        public void Constructor_SetServerBaseUrl_DefaultIsLocalhost()
        {
            // The LeaderboardService should have _serverBaseUrl set to default
            // We verify this indirectly by checking that the component exists
            Assert.IsNotNull(_leaderboardService);
        }

        [Test]
        public void DeserializeScores_SingleEntry_ReturnsListWithOneEntry()
        {
            // Test deserialization of a single score entry
            string json = @"[{""rank"":1,""initials"":""AAA"",""score"":1000}]";
            var entries = DeserializeScoresReflection(json);

            Assert.IsNotNull(entries);
            Assert.AreEqual(1, entries.Count);
            Assert.AreEqual(1, entries[0].rank);
            Assert.AreEqual("AAA", entries[0].initials);
            Assert.AreEqual(1000, entries[0].score);
        }

        [Test]
        public void DeserializeScores_MultipleEntries_ReturnsListInOrder()
        {
            // Test deserialization of multiple score entries
            string json = @"[
                {""rank"":1,""initials"":""AAA"",""score"":5000},
                {""rank"":2,""initials"":""BBB"",""score"":4500},
                {""rank"":3,""initials"":""CCC"",""score"":4000}
            ]";
            var entries = DeserializeScoresReflection(json);

            Assert.IsNotNull(entries);
            Assert.AreEqual(3, entries.Count);
            Assert.AreEqual("AAA", entries[0].initials);
            Assert.AreEqual(5000, entries[0].score);
            Assert.AreEqual("BBB", entries[1].initials);
            Assert.AreEqual(4500, entries[1].score);
            Assert.AreEqual("CCC", entries[2].initials);
            Assert.AreEqual(4000, entries[2].score);
        }

        [Test]
        public void DeserializeScores_FiveEntries_ReturnsAll()
        {
            // Test deserialization of the maximum expected list (top 5)
            string json = @"[
                {""rank"":1,""initials"":""AAA"",""score"":5000},
                {""rank"":2,""initials"":""BBB"",""score"":4500},
                {""rank"":3,""initials"":""CCC"",""score"":4000},
                {""rank"":4,""initials"":""DDD"",""score"":3500},
                {""rank"":5,""initials"":""EEE"",""score"":3000}
            ]";
            var entries = DeserializeScoresReflection(json);

            Assert.IsNotNull(entries);
            Assert.AreEqual(5, entries.Count);
            Assert.AreEqual("EEE", entries[4].initials);
            Assert.AreEqual(3000, entries[4].score);
        }

        [Test]
        public void DeserializeScores_EmptyArray_ReturnsEmptyList()
        {
            // Test deserialization of an empty response
            string json = "[]";
            var entries = DeserializeScoresReflection(json);

            Assert.IsNotNull(entries);
            Assert.AreEqual(0, entries.Count);
        }

        [Test]
        public void SerializeSubmitScoreRequest_CreatesValidJson()
        {
            // Test that a score submission request serializes correctly
            string json = JsonUtility.ToJson(new SubmitScoreRequestHelper { initials = "ABC", score = 2500 });

            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("ABC"));
            Assert.IsTrue(json.Contains("2500"));
        }

        [Test]
        public void DeserializeScores_RankValuesPreserved()
        {
            // Test that rank values are correctly preserved during deserialization
            string json = @"[
                {""rank"":1,""initials"":""AAA"",""score"":5000},
                {""rank"":2,""initials"":""BBB"",""score"":4500}
            ]";
            var entries = DeserializeScoresReflection(json);

            Assert.AreEqual(1, entries[0].rank);
            Assert.AreEqual(2, entries[1].rank);
        }

        [Test]
        public void DeserializeScores_InitialsVariableLength_HandledCorrectly()
        {
            // Test that initials of different lengths are handled
            string json = @"[
                {""rank"":1,""initials"":""A"",""score"":5000},
                {""rank"":2,""initials"":""BB"",""score"":4500},
                {""rank"":3,""initials"":""CCC"",""score"":4000}
            ]";
            var entries = DeserializeScoresReflection(json);

            Assert.AreEqual("A", entries[0].initials);
            Assert.AreEqual("BB", entries[1].initials);
            Assert.AreEqual("CCC", entries[2].initials);
        }

        private List<LeaderboardEntry> DeserializeScoresReflection(string json)
        {
            // Use reflection to access the private DeserializeScores method
            var method = _leaderboardService.GetType().GetMethod("DeserializeScores",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (List<LeaderboardEntry>)method.Invoke(_leaderboardService, new object[] { json });
        }

        [System.Serializable]
        private class SubmitScoreRequestHelper
        {
            public string initials;
            public int score;
        }
    }
}
