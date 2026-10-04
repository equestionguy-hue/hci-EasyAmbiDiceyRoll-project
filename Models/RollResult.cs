using System;

namespace EasyAmbiDiceyRoll.Models
{
    public class RollResult
    {
        public int TotalValue => BaseRollTotal + ModifierValue;
        public int BaseRollTotal { get; set; }
        public int ModifierValue { get; set; }

        // Tracks the standard dice rolled
        public List<int> StandardRolls { get; set; } = new();

        // --- Game specific properties ---

        // Dungeons and Dragons 5
        public List<int> DiscardedRolls { get; set; } = new();

        // World of Darkness
        public int Successes { get; set; }
        public bool IsFailure { get; set; }
        public bool IsCriticalFailure { get; set; }
        public List<int> ExplodedRolls { get; set; } = new();
    }
}
