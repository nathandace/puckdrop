# Puck Drop

A real-time NHL game tracker with webhook support for home automation. Track your favorite team's games and trigger smart home actions on goals, power plays, and other game events.

## Features

- **Live Game Tracking** - Real-time scores, play-by-play, and game statistics
- **Team Dashboard** - Follow your favorite teams with schedules, rosters, and standings
- **Player Stats** - Detailed player statistics and career information
- **Webhook Integration** - Trigger smart home automations on game events
- **Home Assistant Ready** - Built-in support for Home Assistant webhooks with team colors

## Webhook Events

Configure webhooks to trigger on any of these events:

| Event | Description |
|-------|-------------|
| `GoalScored` | When your team scores |
| `PenaltyCommitted` | When a penalty is called |
| `PowerPlayStart` | When your team goes on the power play |
| `PowerPlayEnd` | When the power play ends |
| `GoaliePulled` | When the goalie is pulled |
| `GoalieReturned` | When the goalie returns |
| `PeriodStart` | When a period begins |
| `PeriodEnd` | When a period ends |
| `GameStart` | When the game starts (period 1) |
| `GameEnd` | When the game ends |

## Webhook Payload

Webhooks send JSON payloads with game context and team colors for smart lighting:

```json
{
  "eventType": "GoalScored",
  "timestamp": "2024-12-08T20:30:00Z",
  "gameId": 2024020001,
  "period": 2,
  "timeInPeriod": "15:30",
  "homeTeam": "STL",
  "awayTeam": "CHI",
  "homeScore": 3,
  "awayScore": 1,
  "teamColors": [
    { "r": 0, "g": 47, "b": 135 },
    { "r": 255, "g": 184, "b": 28 },
    { "r": 255, "g": 255, "b": 255 }
  ],
  "details": {
    "team": "STL",
    "player": "Jordan Kyrou",
    "jerseyNumber": 25
  }
}
```

## Supported Payload Formats

- **Generic JSON** - Standard JSON payload for any webhook consumer
- **Discord** - Rich embeds formatted for Discord webhooks
- **Home Assistant** - Optimized for Home Assistant automations

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (for development)
- No runtime required on deployment target (self-contained builds)

### Development

```bash
# Clone the repository
git clone https://github.com/nathandace/nhl.git
cd nhl

# Restore dependencies
dotnet restore

# Run in development mode
dotnet run
```

The app will be available at `https://localhost:5001` or `http://localhost:5000`.

### Configuration

Edit `appsettings.json` to configure:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=puckdrop.db"
  }
}
```

## Deployment

### Raspberry Pi

A deployment script is included for Raspberry Pi:

1. Edit `pi-deploy.sh` and set your Pi's configuration:
   ```bash
   PI_USER="your_username"
   PI_HOST="192.168.1.100"
   PI_PATH="/home/your_username/puckdrop"
   PI_PORT="5050"
   ```

2. Run the deployment:
   ```bash
   ./pi-deploy.sh
   ```

The script will:
- Build a self-contained ARM64 binary (no .NET required on Pi)
- Copy files to your Pi via SCP
- Set up a systemd service for auto-start
- Start the application

### Service Management

```bash
# Check status
sudo systemctl status puckdrop

# View logs
sudo journalctl -u puckdrop -f

# Restart service
sudo systemctl restart puckdrop

# Stop service
sudo systemctl stop puckdrop
```

## Home Assistant Integration

### Setting Up Webhooks

1. In Home Assistant, go to **Settings > Automations & Scenes > Automations**
2. Create a new automation with a **Webhook** trigger
3. Copy the webhook ID
4. In Puck Drop, go to **Webhooks** and create a new rule:
   - Select your team
   - Choose the event type (e.g., GoalScored)
   - Set format to **Home Assistant**
   - Enter your webhook URL: `http://your-ha-ip:8123/api/webhook/YOUR_WEBHOOK_ID`

### Example: Goal Celebration Automation

```yaml
alias: Team Goal Celebration
triggers:
  - trigger: webhook
    webhook_id: "YOUR_WEBHOOK_ID"
    local_only: true
actions:
  # Play goal horn audio
  - action: notify.media_player_YOUR_DEVICE
    data:
      message: "<audio src='https://YOUR_URL/local/audio/goal_song.mp3'/>"
      data:
        type: tts

  # Flash lights in team colors
  - variables:
      colors: "{{ trigger.json.data.team_colors }}"

  - repeat:
      count: 60
      sequence:
        - action: light.turn_on
          target:
            entity_id: light.YOUR_RGB_LIGHT
          data:
            rgb_color:
              - "{{ colors[(repeat.index - 1) % (colors | length)].r }}"
              - "{{ colors[(repeat.index - 1) % (colors | length)].g }}"
              - "{{ colors[(repeat.index - 1) % (colors | length)].b }}"
            brightness: 255
        - delay:
            milliseconds: 500
```

## Tech Stack

- **Framework**: ASP.NET Core 10 with Blazor Server
- **Database**: SQLite with Entity Framework Core
- **API**: NHL Edge API (api-web.nhle.com)

## License

MIT License - See [LICENSE](LICENSE) for details.
