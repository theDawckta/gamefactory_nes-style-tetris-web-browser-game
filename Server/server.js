'use strict';

const express = require('express');
const cors = require('cors');
const fs = require('fs');

const MAX_ENTRIES = 5;

// Build the Express app against a specific scores file path. Exported as a
// factory so tests can run against a temporary file without touching the
// real leaderboard.
function createApp(scoresFile) {
  const app = express();
  app.use(cors());
  app.use(express.json());

  // Read the persisted entries. Returns an empty array if the file does not
  // exist yet or contains anything other than a JSON array.
  function readScores() {
    let raw;
    try {
      raw = fs.readFileSync(scoresFile, 'utf8');
    } catch (err) {
      if (err.code === 'ENOENT') return [];
      throw err;
    }
    try {
      const parsed = JSON.parse(raw);
      return Array.isArray(parsed) ? parsed : [];
    } catch (err) {
      return [];
    }
  }

  function writeScores(entries) {
    fs.writeFileSync(scoresFile, JSON.stringify(entries, null, 2));
  }

  // Sort descending by score and keep only the top MAX_ENTRIES. Stored shape
  // is {initials, score} -- rank is derived on output, never persisted.
  function trimTop(entries) {
    return entries
      .slice()
      .sort((a, b) => b.score - a.score)
      .slice(0, MAX_ENTRIES)
      .map((e) => ({ initials: e.initials, score: e.score }));
  }

  // Attach a 1-based rank to each entry for the API response.
  function withRank(entries) {
    return entries.map((e, i) => ({ rank: i + 1, initials: e.initials, score: e.score }));
  }

  app.get('/scores', (req, res) => {
    res.json(withRank(trimTop(readScores())));
  });

  app.post('/scores', (req, res) => {
    const body = req.body || {};
    const { initials, score } = body;

    if (typeof initials !== 'string' || initials.length !== 3) {
      return res.status(400).json({ error: 'initials must be a 3-character string' });
    }
    if (!Number.isInteger(score)) {
      return res.status(400).json({ error: 'score must be an integer' });
    }

    const entries = readScores();
    entries.push({ initials, score });
    const top = trimTop(entries);
    writeScores(top);
    res.json(withRank(top));
  });

  return app;
}

const SCORES_FILE = process.env.SCORES_FILE || './scores.json';
const PORT = process.env.PORT || 3000;

if (require.main === module) {
  const app = createApp(SCORES_FILE);
  app.listen(PORT, () => {
    console.log(`Leaderboard server listening on port ${PORT} (scores file: ${SCORES_FILE})`);
  });
}

module.exports = { createApp };
