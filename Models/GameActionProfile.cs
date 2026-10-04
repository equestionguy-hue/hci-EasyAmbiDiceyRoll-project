using System;

namespace EasyAmbiDiceyRoll.Models
{
    public enum GameSystem
    {
        Standard,
        DungeonsAndDragons5,
        WorldOfDarkness
    }

    public class GameActionProfile
    {
        // Unique identifier required for editing/deleting profiles later
        public Guid Id { get; set; } = Guid.NewGuid();
        
        // Name of GameActionProfile
        public string Name { get; set; } = string.Empty;
        
        // Determines which specific game UI to display
        public GameSystem SystemMode { get; set; } = GameSystem.Standard;

        // key = die sides. Valuue = quantity of selected die
        public Dictionary<int, int> SelectedDice { get; set; } = new()
        {
            { 4, 0 }, { 6, 0 }, { 8, 0 }, { 10, 0 }, { 12, 0 }, { 20, 0 }, { 100, 0 }
        };

        // The flat value added or subtracted from the result of die roll
        public int ModifierValue { get; set; }

        // --- Game Specific Properties ---
        
        // Dungeons and Dragons 5
        public bool HasAdvantage { get; set; }
        public bool HasDisadvantage { get; set; }

        // World of Darkness
        public int ExplodingDiceThreshold { get; set; } = 0;
    }

}