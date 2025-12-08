using PuckDrop.Models.Api;

namespace PuckDrop.Services;

/// <summary>
/// Service for generating mock NHL data for development and testing.
/// </summary>
public class MockDataService : IMockDataService
{
    private readonly Dictionary<int, MockGameState> _gameStates = new();
    private readonly Random _random = new();
    private readonly object _lock = new();

    private const int MockGameId = 2024020001;

    /// <summary>
    /// Mock player data for home team.
    /// </summary>
    private static readonly List<MockPlayer> HomeTeamPlayers = new()
    {
        new MockPlayer(87, "Sidney", "Crosby", "C"),
        new MockPlayer(71, "Evgeni", "Malkin", "C"),
        new MockPlayer(59, "Jake", "Guentzel", "LW"),
        new MockPlayer(17, "Bryan", "Rust", "RW"),
        new MockPlayer(12, "Kris", "Letang", "D"),
        new MockPlayer(6, "John", "Marino", "D"),
        new MockPlayer(58, "Mike", "Matheson", "D"),
        new MockPlayer(28, "Marcus", "Pettersson", "D"),
        new MockPlayer(77, "Jeff", "Carter", "C"),
        new MockPlayer(19, "Reilly", "Smith", "RW"),
        new MockPlayer(14, "Teddy", "Blueger", "C"),
        new MockPlayer(24, "Ryan", "Shea", "D")
    };

    /// <summary>
    /// Mock player data for away team.
    /// </summary>
    private static readonly List<MockPlayer> AwayTeamPlayers = new()
    {
        new MockPlayer(8, "Alex", "Ovechkin", "LW"),
        new MockPlayer(19, "Nicklas", "Backstrom", "C"),
        new MockPlayer(74, "John", "Carlson", "D"),
        new MockPlayer(77, "T.J.", "Oshie", "RW"),
        new MockPlayer(90, "Marcus", "Johansson", "LW"),
        new MockPlayer(43, "Tom", "Wilson", "RW"),
        new MockPlayer(92, "Evgeny", "Kuznetsov", "C"),
        new MockPlayer(3, "Nick", "Jensen", "D"),
        new MockPlayer(42, "Martin", "Fehervary", "D"),
        new MockPlayer(52, "Matt", "Irwin", "D"),
        new MockPlayer(21, "Garnet", "Hathaway", "RW"),
        new MockPlayer(26, "Nic", "Dowd", "C")
    };

    /// <inheritdoc />
    public GameLandingResponse GetMockGameLanding(int gameId)
    {
        var state = GetOrCreateGameState(gameId);

        return new GameLandingResponse
        {
            Id = gameId,
            Season = 20242025,
            GameType = 2,
            GameDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            Venue = new VenueName { Default = "PPG Paints Arena" },
            StartTimeUtc = DateTime.UtcNow.AddHours(-1),
            EasternUtcOffset = "-05:00",
            GameState = state.IsActive ? "LIVE" : "OFF",
            PeriodDescriptor = new PeriodDescriptor
            {
                Number = state.Period,
                PeriodType = "REG"
            },
            Clock = new GameClock
            {
                TimeRemaining = state.TimeRemaining,
                SecondsRemaining = state.SecondsRemaining,
                Running = state.IsActive && state.SecondsRemaining > 0,
                InIntermission = state.InIntermission
            },
            HomeTeam = new LandingTeam
            {
                Id = 5,
                Abbrev = "PIT",
                Name = new TeamName { Default = "Penguins" },
                Score = state.HomeScore,
                Sog = state.HomeShotsOnGoal,
                Logo = "https://assets.nhle.com/logos/nhl/svg/PIT_light.svg"
            },
            AwayTeam = new LandingTeam
            {
                Id = 15,
                Abbrev = "WSH",
                Name = new TeamName { Default = "Capitals" },
                Score = state.AwayScore,
                Sog = state.AwayShotsOnGoal,
                Logo = "https://assets.nhle.com/logos/nhl/svg/WSH_light.svg"
            },
            GameOutcome = state.IsActive ? null : new GameOutcome
            {
                LastPeriodType = "REG"
            },
            Summary = new GameSummary
            {
                Scoring = GenerateScoringPlays(state),
                Shootout = null
            }
        };
    }

    /// <inheritdoc />
    public PlayByPlayResponse GetMockPlayByPlay(int gameId)
    {
        var state = GetOrCreateGameState(gameId);

        return new PlayByPlayResponse
        {
            Id = gameId,
            Season = 20242025,
            GameType = 2,
            GameDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            Venue = new VenueName { Default = "PPG Paints Arena" },
            StartTimeUtc = DateTime.UtcNow.AddHours(-1),
            GameState = state.IsActive ? "LIVE" : "OFF",
            PeriodDescriptor = new PeriodDescriptor
            {
                Number = state.Period,
                PeriodType = "REG"
            },
            Clock = new GameClock
            {
                TimeRemaining = state.TimeRemaining,
                SecondsRemaining = state.SecondsRemaining,
                Running = state.IsActive
            },
            HomeTeam = new PlayByPlayTeam
            {
                Id = 5,
                Abbrev = "PIT",
                Name = new TeamName { Default = "Penguins" },
                Score = state.HomeScore
            },
            AwayTeam = new PlayByPlayTeam
            {
                Id = 15,
                Abbrev = "WSH",
                Name = new TeamName { Default = "Capitals" },
                Score = state.AwayScore
            },
            Plays = GeneratePlays(state),
            RosterSpots = GenerateRosterSpots()
        };
    }

    /// <inheritdoc />
    public BoxscoreResponse GetMockBoxscore(int gameId)
    {
        var state = GetOrCreateGameState(gameId);

        return new BoxscoreResponse
        {
            Id = gameId,
            Season = 20242025,
            GameType = 2,
            GameDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            Venue = new VenueName { Default = "PPG Paints Arena" },
            StartTimeUtc = DateTime.UtcNow.AddHours(-1),
            GameState = state.IsActive ? "LIVE" : "OFF",
            PeriodDescriptor = new PeriodDescriptor
            {
                Number = state.Period,
                PeriodType = "REG"
            },
            HomeTeam = new BoxscoreTeam
            {
                Id = 5,
                Abbrev = "PIT",
                Name = new TeamName { Default = "Penguins" },
                Score = state.HomeScore,
                Sog = state.HomeShotsOnGoal,
                Logo = "https://assets.nhle.com/logos/nhl/svg/PIT_light.svg"
            },
            AwayTeam = new BoxscoreTeam
            {
                Id = 15,
                Abbrev = "WSH",
                Name = new TeamName { Default = "Capitals" },
                Score = state.AwayScore,
                Sog = state.AwayShotsOnGoal,
                Logo = "https://assets.nhle.com/logos/nhl/svg/WSH_light.svg"
            },
            PlayerByGameStats = new PlayerByGameStats
            {
                HomeTeam = new TeamPlayerStats
                {
                    Forwards = GenerateForwardStats(HomeTeamPlayers, true, state),
                    Defense = GenerateDefenseStats(HomeTeamPlayers, true, state),
                    Goalies = GenerateGoalieStats(true, state)
                },
                AwayTeam = new TeamPlayerStats
                {
                    Forwards = GenerateForwardStats(AwayTeamPlayers, false, state),
                    Defense = GenerateDefenseStats(AwayTeamPlayers, false, state),
                    Goalies = GenerateGoalieStats(false, state)
                }
            },
            GameOutcome = state.IsActive ? null : new GameOutcome { LastPeriodType = "REG" }
        };
    }

    /// <inheritdoc />
    public ShiftChartResponse GetMockShiftChart(int gameId)
    {
        var state = GetOrCreateGameState(gameId);

        return new ShiftChartResponse
        {
            Id = gameId,
            HomeTeam = GenerateShiftChartTeam(5, "PIT", HomeTeamPlayers, true, state),
            AwayTeam = GenerateShiftChartTeam(15, "WSH", AwayTeamPlayers, false, state)
        };
    }

    /// <inheritdoc />
    public ScheduleResponse GetMockSchedule(string teamAbbrev)
    {
        var games = new List<ScheduleGame>();
        var today = DateTime.UtcNow.Date;

        for (int i = -10; i <= 20; i++)
        {
            var gameDate = today.AddDays(i);
            if (gameDate.DayOfWeek == DayOfWeek.Monday)
            {
                continue;
            }

            var isHome = i % 2 == 0;
            var opponent = GetRandomOpponent(teamAbbrev);

            games.Add(new ScheduleGame
            {
                Id = MockGameId + i + 10,
                Season = 20242025,
                GameType = 2,
                GameDate = gameDate.ToString("yyyy-MM-dd"),
                Venue = new VenueName { Default = isHome ? "PPG Paints Arena" : $"{opponent} Arena" },
                StartTimeUtc = gameDate.AddHours(19),
                GameState = i < 0 ? "OFF" : (i == 0 ? "LIVE" : "FUT"),
                HomeTeam = new ScheduleTeam
                {
                    Id = isHome ? 5 : _random.Next(1, 33),
                    Abbrev = isHome ? teamAbbrev : opponent,
                    PlaceName = new TeamName { Default = isHome ? "Pittsburgh" : "Opponent" },
                    Score = i < 0 ? _random.Next(1, 6) : 0,
                    Logo = $"https://assets.nhle.com/logos/nhl/svg/{(isHome ? teamAbbrev : opponent)}_light.svg"
                },
                AwayTeam = new ScheduleTeam
                {
                    Id = isHome ? _random.Next(1, 33) : 5,
                    Abbrev = isHome ? opponent : teamAbbrev,
                    PlaceName = new TeamName { Default = isHome ? "Opponent" : "Pittsburgh" },
                    Score = i < 0 ? _random.Next(1, 6) : 0,
                    Logo = $"https://assets.nhle.com/logos/nhl/svg/{(isHome ? opponent : teamAbbrev)}_light.svg"
                },
                GameOutcome = i < 0 ? new GameOutcome { LastPeriodType = "REG" } : null
            });
        }

        return new ScheduleResponse
        {
            Games = games
        };
    }

    /// <inheritdoc />
    public void AdvanceGameState(int gameId)
    {
        lock (_lock)
        {
            if (!_gameStates.TryGetValue(gameId, out var state))
            {
                return;
            }

            if (!state.IsActive)
            {
                return;
            }

            state.SecondsRemaining -= _random.Next(5, 20);

            if (state.SecondsRemaining <= 0)
            {
                if (state.Period < 3)
                {
                    state.Period++;
                    state.SecondsRemaining = 1200;
                    state.InIntermission = false;
                }
                else
                {
                    state.IsActive = false;
                    state.SecondsRemaining = 0;
                }
            }

            state.TimeRemaining = FormatTime(state.SecondsRemaining);

            if (_random.Next(100) < 15)
            {
                if (_random.Next(2) == 0)
                {
                    state.HomeShotsOnGoal++;
                    if (_random.Next(100) < 10)
                    {
                        state.HomeScore++;
                        state.Events.Add(new MockEvent
                        {
                            Type = "goal",
                            Period = state.Period,
                            Time = state.TimeRemaining,
                            Team = "PIT",
                            Description = $"{HomeTeamPlayers[_random.Next(HomeTeamPlayers.Count)].LastName} scores!"
                        });
                    }
                }
                else
                {
                    state.AwayShotsOnGoal++;
                    if (_random.Next(100) < 10)
                    {
                        state.AwayScore++;
                        state.Events.Add(new MockEvent
                        {
                            Type = "goal",
                            Period = state.Period,
                            Time = state.TimeRemaining,
                            Team = "WSH",
                            Description = $"{AwayTeamPlayers[_random.Next(AwayTeamPlayers.Count)].LastName} scores!"
                        });
                    }
                }
            }

            if (_random.Next(100) < 3)
            {
                var isHome = _random.Next(2) == 0;
                var players = isHome ? HomeTeamPlayers : AwayTeamPlayers;
                state.Events.Add(new MockEvent
                {
                    Type = "penalty",
                    Period = state.Period,
                    Time = state.TimeRemaining,
                    Team = isHome ? "PIT" : "WSH",
                    Description = $"{players[_random.Next(players.Count)].LastName} - 2 min Hooking"
                });
            }
        }
    }

    /// <inheritdoc />
    public void ResetGame(int gameId)
    {
        lock (_lock)
        {
            _gameStates[gameId] = CreateInitialGameState();
        }
    }

    /// <inheritdoc />
    public bool IsGameActive(int gameId)
    {
        lock (_lock)
        {
            return _gameStates.TryGetValue(gameId, out var state) && state.IsActive;
        }
    }

    /// <summary>
    /// Gets or creates a game state for the specified game ID.
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <returns>The game state.</returns>
    private MockGameState GetOrCreateGameState(int gameId)
    {
        lock (_lock)
        {
            if (!_gameStates.TryGetValue(gameId, out var state))
            {
                state = CreateInitialGameState();
                _gameStates[gameId] = state;
            }
            return state;
        }
    }

    /// <summary>
    /// Creates an initial game state.
    /// </summary>
    /// <returns>New game state.</returns>
    private MockGameState CreateInitialGameState()
    {
        return new MockGameState
        {
            Period = 2,
            SecondsRemaining = 847,
            TimeRemaining = "14:07",
            HomeScore = 2,
            AwayScore = 1,
            HomeShotsOnGoal = 18,
            AwayShotsOnGoal = 14,
            IsActive = true,
            InIntermission = false,
            Events = new List<MockEvent>
            {
                new() { Type = "goal", Period = 1, Time = "15:23", Team = "PIT", Description = "Crosby scores! (Malkin, Letang)" },
                new() { Type = "penalty", Period = 1, Time = "8:45", Team = "WSH", Description = "Wilson - 2 min Roughing" },
                new() { Type = "goal", Period = 1, Time = "5:12", Team = "WSH", Description = "Ovechkin scores! (Backstrom)" },
                new() { Type = "goal", Period = 2, Time = "18:30", Team = "PIT", Description = "Guentzel scores! (Crosby, Rust)" }
            }
        };
    }

    /// <summary>
    /// Generates scoring plays from game state events.
    /// </summary>
    /// <param name="state">The game state.</param>
    /// <returns>List of scoring plays by period.</returns>
    private static List<PeriodScoring> GenerateScoringPlays(MockGameState state)
    {
        var periods = new Dictionary<int, List<GoalInfo>>();

        foreach (var evt in state.Events.Where(e => e.Type == "goal"))
        {
            if (!periods.ContainsKey(evt.Period))
            {
                periods[evt.Period] = new List<GoalInfo>();
            }

            periods[evt.Period].Add(new GoalInfo
            {
                PlayerId = 1,
                FirstName = new PlayerName { Default = "Mock" },
                LastName = new PlayerName { Default = "Player" },
                TimeInPeriod = evt.Time,
                TeamAbbrev = new TeamName { Default = evt.Team },
                HomeScore = state.HomeScore,
                AwayScore = state.AwayScore,
                Assists = new List<AssistInfo>()
            });
        }

        return periods.Select(p => new PeriodScoring
        {
            PeriodDescriptor = new PeriodDescriptor { Number = p.Key, PeriodType = "REG" },
            Goals = p.Value
        }).OrderBy(p => p.PeriodDescriptor.Number).ToList();
    }

    /// <summary>
    /// Generates play-by-play events.
    /// </summary>
    /// <param name="state">The game state.</param>
    /// <returns>List of plays.</returns>
    private static List<Play> GeneratePlays(MockGameState state)
    {
        var plays = new List<Play>();
        var eventId = 1;

        foreach (var evt in state.Events.OrderByDescending(e => e.Period).ThenBy(e => e.Time))
        {
            plays.Add(new Play
            {
                EventId = eventId++,
                PeriodDescriptor = new PeriodDescriptor { Number = evt.Period, PeriodType = "REG" },
                TimeInPeriod = evt.Time,
                TypeCode = evt.Type switch
                {
                    "goal" => 505,
                    "penalty" => 509,
                    "shot" => 506,
                    _ => 520
                },
                TypeDescKey = evt.Type,
                SortOrder = eventId,
                Details = new PlayDetails
                {
                    EventOwnerTeamId = evt.Team == "PIT" ? 5 : 15,
                    Reason = evt.Type == "penalty" ? "Hooking" : null
                }
            });
        }

        return plays;
    }

    /// <summary>
    /// Generates roster spots.
    /// </summary>
    /// <returns>List of roster spots.</returns>
    private static List<RosterSpot> GenerateRosterSpots()
    {
        var spots = new List<RosterSpot>();

        foreach (var player in HomeTeamPlayers)
        {
            spots.Add(new RosterSpot
            {
                TeamId = 5,
                PlayerId = player.Number,
                FirstName = new PlayerName { Default = player.FirstName },
                LastName = new PlayerName { Default = player.LastName },
                SweaterNumber = player.Number,
                PositionCode = player.Position
            });
        }

        foreach (var player in AwayTeamPlayers)
        {
            spots.Add(new RosterSpot
            {
                TeamId = 15,
                PlayerId = player.Number + 100,
                FirstName = new PlayerName { Default = player.FirstName },
                LastName = new PlayerName { Default = player.LastName },
                SweaterNumber = player.Number,
                PositionCode = player.Position
            });
        }

        return spots;
    }

    /// <summary>
    /// Generates forward player stats.
    /// </summary>
    /// <param name="players">The team players.</param>
    /// <param name="isHome">Whether this is the home team.</param>
    /// <param name="state">The game state.</param>
    /// <returns>List of skater stats.</returns>
    private List<SkaterStats> GenerateForwardStats(List<MockPlayer> players, bool isHome, MockGameState state)
    {
        return players.Where(p => p.Position != "D").Select(p => new SkaterStats
        {
            PlayerId = isHome ? p.Number : p.Number + 100,
            SweaterNumber = p.Number,
            Name = new PlayerName { Default = $"{p.FirstName} {p.LastName}" },
            Position = p.Position,
            Goals = _random.Next(0, 2),
            Assists = _random.Next(0, 2),
            Points = 0,
            PlusMinus = _random.Next(-2, 3),
            Pim = _random.Next(0, 5),
            Hits = _random.Next(0, 4),
            Shots = _random.Next(0, 5),
            FaceoffWinningPctg = p.Position == "C" ? Math.Round(_random.NextDouble() * 100, 1) : 0,
            Toi = $"{_random.Next(10, 20)}:{_random.Next(0, 60):D2}"
        }).ToList();
    }

    /// <summary>
    /// Generates defenseman player stats.
    /// </summary>
    /// <param name="players">The team players.</param>
    /// <param name="isHome">Whether this is the home team.</param>
    /// <param name="state">The game state.</param>
    /// <returns>List of skater stats.</returns>
    private List<SkaterStats> GenerateDefenseStats(List<MockPlayer> players, bool isHome, MockGameState state)
    {
        return players.Where(p => p.Position == "D").Select(p => new SkaterStats
        {
            PlayerId = isHome ? p.Number : p.Number + 100,
            SweaterNumber = p.Number,
            Name = new PlayerName { Default = $"{p.FirstName} {p.LastName}" },
            Position = p.Position,
            Goals = _random.Next(0, 1),
            Assists = _random.Next(0, 2),
            Points = 0,
            PlusMinus = _random.Next(-2, 3),
            Pim = _random.Next(0, 3),
            Hits = _random.Next(1, 6),
            Shots = _random.Next(0, 3),
            BlockedShots = _random.Next(0, 4),
            Toi = $"{_random.Next(15, 25)}:{_random.Next(0, 60):D2}"
        }).ToList();
    }

    /// <summary>
    /// Generates goalie stats.
    /// </summary>
    /// <param name="isHome">Whether this is the home team.</param>
    /// <param name="state">The game state.</param>
    /// <returns>List of goalie stats.</returns>
    private List<GoalieStats> GenerateGoalieStats(bool isHome, MockGameState state)
    {
        var shots = isHome ? state.AwayShotsOnGoal : state.HomeShotsOnGoal;
        var goalsAgainst = isHome ? state.AwayScore : state.HomeScore;

        return new List<GoalieStats>
        {
            new()
            {
                PlayerId = isHome ? 35 : 135,
                SweaterNumber = 35,
                Name = new PlayerName { Default = isHome ? "Tristan Jarry" : "Charlie Lindgren" },
                Position = "G",
                GoalsAgainst = goalsAgainst,
                SavePctg = shots > 0 ? Math.Round((double)(shots - goalsAgainst) / shots, 3) : 1.0,
                SaveShotsAgainst = $"{shots - goalsAgainst}/{shots}",
                Toi = $"{(state.Period - 1) * 20 + (1200 - state.SecondsRemaining) / 60}:{(1200 - state.SecondsRemaining) % 60:D2}"
            }
        };
    }

    /// <summary>
    /// Generates shift chart team data.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="abbrev">The team abbreviation.</param>
    /// <param name="players">The team players.</param>
    /// <param name="isHome">Whether this is the home team.</param>
    /// <param name="state">The game state.</param>
    /// <returns>Shift chart team data.</returns>
    private ShiftChartTeam GenerateShiftChartTeam(int teamId, string abbrev, List<MockPlayer> players, bool isHome, MockGameState state)
    {
        var currentTimeSeconds = (state.Period - 1) * 1200 + (1200 - state.SecondsRemaining);
        var playerShiftData = new List<PlayerShiftData>();

        foreach (var player in players)
        {
            var shifts = new List<Shift>();
            var onIce = _random.Next(100) < 30;

            if (onIce)
            {
                shifts.Add(new Shift
                {
                    ShiftNumber = _random.Next(5, 20),
                    Period = state.Period,
                    StartTime = FormatTime(Math.Max(0, currentTimeSeconds - _random.Next(10, 45))),
                    EndTime = null,
                    Duration = null,
                    TypeCode = "R"
                });
            }

            playerShiftData.Add(new PlayerShiftData
            {
                PlayerId = isHome ? player.Number : player.Number + 100,
                FirstName = new PlayerName { Default = player.FirstName },
                LastName = new PlayerName { Default = player.LastName },
                SweaterNumber = player.Number,
                PositionCode = player.Position,
                Shifts = shifts
            });
        }

        return new ShiftChartTeam
        {
            Id = teamId,
            Abbrev = abbrev,
            Logo = $"https://assets.nhle.com/logos/nhl/svg/{abbrev}_light.svg",
            Players = playerShiftData
        };
    }

    /// <summary>
    /// Gets a random opponent team abbreviation.
    /// </summary>
    /// <param name="excludeTeam">Team to exclude.</param>
    /// <returns>Random team abbreviation.</returns>
    private string GetRandomOpponent(string excludeTeam)
    {
        var teams = new[]
        {
            "ANA", "ARI", "BOS", "BUF", "CGY", "CAR", "CHI", "COL", "CBJ", "DAL",
            "DET", "EDM", "FLA", "LAK", "MIN", "MTL", "NSH", "NJD", "NYI", "NYR",
            "OTT", "PHI", "PIT", "SJS", "SEA", "STL", "TBL", "TOR", "VAN", "VGK", "WSH", "WPG"
        };

        string opponent;
        do
        {
            opponent = teams[_random.Next(teams.Length)];
        } while (opponent == excludeTeam);

        return opponent;
    }

    /// <summary>
    /// Formats seconds as MM:SS.
    /// </summary>
    /// <param name="seconds">Seconds to format.</param>
    /// <returns>Formatted time string.</returns>
    private static string FormatTime(int seconds)
    {
        if (seconds < 0)
        {
            seconds = 0;
        }

        var minutes = seconds / 60;
        var secs = seconds % 60;
        return $"{minutes}:{secs:D2}";
    }

    /// <summary>
    /// Internal class for tracking mock game state.
    /// </summary>
    private class MockGameState
    {
        /// <summary>
        /// Gets or sets the current period.
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// Gets or sets the seconds remaining in the period.
        /// </summary>
        public int SecondsRemaining { get; set; }

        /// <summary>
        /// Gets or sets the formatted time remaining.
        /// </summary>
        public string TimeRemaining { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the home team score.
        /// </summary>
        public int HomeScore { get; set; }

        /// <summary>
        /// Gets or sets the away team score.
        /// </summary>
        public int AwayScore { get; set; }

        /// <summary>
        /// Gets or sets the home team shots on goal.
        /// </summary>
        public int HomeShotsOnGoal { get; set; }

        /// <summary>
        /// Gets or sets the away team shots on goal.
        /// </summary>
        public int AwayShotsOnGoal { get; set; }

        /// <summary>
        /// Gets or sets whether the game is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets whether the game is in intermission.
        /// </summary>
        public bool InIntermission { get; set; }

        /// <summary>
        /// Gets or sets the list of game events.
        /// </summary>
        public List<MockEvent> Events { get; set; } = new();
    }

    /// <summary>
    /// Internal class for mock game events.
    /// </summary>
    private class MockEvent
    {
        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the period.
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// Gets or sets the time in period.
        /// </summary>
        public string Time { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the team abbreviation.
        /// </summary>
        public string Team { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the event description.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Internal class for mock player data.
    /// </summary>
    private class MockPlayer
    {
        /// <summary>
        /// Gets the jersey number.
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// Gets the first name.
        /// </summary>
        public string FirstName { get; }

        /// <summary>
        /// Gets the last name.
        /// </summary>
        public string LastName { get; }

        /// <summary>
        /// Gets the position.
        /// </summary>
        public string Position { get; }

        /// <summary>
        /// Initializes a new instance of the MockPlayer class.
        /// </summary>
        /// <param name="number">Jersey number.</param>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="position">Position code.</param>
        public MockPlayer(int number, string firstName, string lastName, string position)
        {
            Number = number;
            FirstName = firstName;
            LastName = lastName;
            Position = position;
        }
    }
}
