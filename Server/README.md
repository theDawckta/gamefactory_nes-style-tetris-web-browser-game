# Tetris Leaderboard Server

A minimal Node.js/Express companion server that maintains the global top-5
leaderboard for the NES-style Tetris web browser game. Scores are persisted to a
JSON file on disk. CORS is enabled so the Unity WebGL build (served from a
different origin) can call the endpoints.

## Endpoints

### `GET /scores`

Returns the current leaderboard as a JSON array of up to 5 entries, sorted
descending by score. Returns `200` with an empty array if no scores exist yet.

Each entry has the shape:

```json
{ "rank": 1, "initials": "AAA", "score": 1000 }
```

### `POST /scores`

Accepts a JSON body:

```json
{ "initials": "AAA", "score": 1000 }
```

- `initials` -- string, exactly 3 characters
- `score` -- integer

The server inserts the entry, sorts descending by score, trims to the top 5,
persists the result to disk, and returns the updated ranked top-5 array.

Invalid bodies (initials not a 3-character string, or score not an integer)
return `400`.

## Environment Variables

| Variable      | Default          | Description                                   |
| ------------- | ---------------- | --------------------------------------------- |
| `PORT`        | `3000`           | TCP port the HTTP server listens on.          |
| `SCORES_FILE` | `./scores.json`  | Path to the JSON file used to persist scores. |

## Running Locally

```bash
cd Server
npm install
npm start
```

The server listens on `http://localhost:3000` by default.

To override the port or scores file:

```bash
PORT=8080 SCORES_FILE=/var/data/tetris-scores.json npm start
```

On Windows PowerShell:

```powershell
$env:PORT = "8080"; $env:SCORES_FILE = "C:\data\tetris-scores.json"; npm start
```

## Tests

```bash
npm test
```

Runs the test suite using the built-in Node test runner (`node --test`). Tests
cover the empty-leaderboard case, insertion and ranking, persistence across
re-reads, top-5 trimming, CORS headers, and input validation.

## Deployment

The server is a single `server.js` file with two dependencies (`express`,
`cors`). It can run on any Node.js 18+ host (the test suite uses the global
`fetch`, which requires Node 18 or newer).

1. Copy the `Server/` directory to the host.
2. Run `npm install --omit=dev` to install runtime dependencies.
3. Set `PORT` and `SCORES_FILE` as needed (`SCORES_FILE` should point at a path
   on persistent storage so the leaderboard survives restarts and redeploys).
4. Start with `npm start`, or under a process manager such as `pm2` or a
   `systemd` service for production use.
5. Ensure the chosen port is reachable from where the WebGL build is served.
   CORS is already enabled for all origins.
