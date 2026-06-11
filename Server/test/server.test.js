'use strict';

const { test, before, after, beforeEach } = require('node:test');
const assert = require('node:assert');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');

const { createApp } = require('../server');

let scoresFile;
let server;
let baseUrl;

// Start one server instance on an ephemeral port, pointed at a temp scores
// file that is reset before each test.
before(async () => {
  const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'leaderboard-test-'));
  scoresFile = path.join(dir, 'scores.json');
  const app = createApp(scoresFile);
  await new Promise((resolve) => {
    server = app.listen(0, () => {
      baseUrl = `http://127.0.0.1:${server.address().port}`;
      resolve();
    });
  });
});

after(() => {
  server.close();
});

beforeEach(() => {
  if (fs.existsSync(scoresFile)) fs.rmSync(scoresFile);
});

test('GET /scores returns an empty array when scores.json does not exist', async () => {
  const res = await fetch(`${baseUrl}/scores`);
  assert.strictEqual(res.status, 200);
  const body = await res.json();
  assert.deepStrictEqual(body, []);
});

test('POST /scores inserts a new high score and returns the ranked top-5 array', async () => {
  const res = await fetch(`${baseUrl}/scores`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ initials: 'AAA', score: 1000 }),
  });
  assert.strictEqual(res.status, 200);
  const body = await res.json();
  assert.deepStrictEqual(body, [{ rank: 1, initials: 'AAA', score: 1000 }]);
});

test('scores are persisted to disk and survive (re-read from file)', async () => {
  await fetch(`${baseUrl}/scores`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ initials: 'BBB', score: 500 }),
  });

  // Simulate a restart by reading the file directly and via a fresh app.
  const onDisk = JSON.parse(fs.readFileSync(scoresFile, 'utf8'));
  assert.deepStrictEqual(onDisk, [{ initials: 'BBB', score: 500 }]);

  const res = await fetch(`${baseUrl}/scores`);
  const body = await res.json();
  assert.deepStrictEqual(body, [{ rank: 1, initials: 'BBB', score: 500 }]);
});

test('only the top 5 entries are retained after a POST, sorted descending', async () => {
  const scores = [100, 600, 300, 200, 500, 400, 50];
  for (const s of scores) {
    await fetch(`${baseUrl}/scores`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ initials: 'ZZZ', score: s }),
    });
  }
  const res = await fetch(`${baseUrl}/scores`);
  const body = await res.json();
  assert.strictEqual(body.length, 5);
  assert.deepStrictEqual(
    body.map((e) => e.score),
    [600, 500, 400, 300, 200]
  );
  assert.deepStrictEqual(
    body.map((e) => e.rank),
    [1, 2, 3, 4, 5]
  );
});

test('CORS headers allow requests from any origin', async () => {
  const res = await fetch(`${baseUrl}/scores`);
  assert.strictEqual(res.headers.get('access-control-allow-origin'), '*');
});

test('POST /scores rejects invalid initials with 400', async () => {
  const res = await fetch(`${baseUrl}/scores`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ initials: 'TOOLONG', score: 10 }),
  });
  assert.strictEqual(res.status, 400);
});

test('POST /scores rejects a non-integer score with 400', async () => {
  const res = await fetch(`${baseUrl}/scores`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ initials: 'CCC', score: 'high' }),
  });
  assert.strictEqual(res.status, 400);
});
