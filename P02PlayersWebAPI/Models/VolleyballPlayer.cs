using System;
using System.Collections.Generic;

namespace P02PlayersWebAPI.Models;

public partial class VolleyballPlayer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Position { get; set; } = null!;

    public int Number { get; set; }

    public int MatchesPlayed { get; set; }

    public int PointsScored { get; set; }

    public int MedalsWon { get; set; }
}
