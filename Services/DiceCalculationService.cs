using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using EasyAmbiDiceyRoll.Models;

namespace EasyAmbiDiceyRoll.Services
{
    public class DiceCalculationService
    {
        public RollResult CalculateRoll(GameActionProfile profile)
        {
            var result = new RollResult
            {
                ModifierValue = profile.ModifierValue
            };

            // Dungeons & Dragons 5 advantage/disadvantage logic
            if (profile.SystemMode == GameSystem.DungeonsAndDragons5 && (profile.HasAdvantage || profile.HasDisadvantage))
            {
                // Roll requested pool two times
                var poolA = GenerateStandardPool(profile.SelectedDice);
                var poolB = GenerateStandardPool(profile.SelectedDice);

                int totalA = poolA.Sum();
                int totalB = poolB.Sum();

                // Determine which pool to keep based on toggle
                bool keepPoolA = profile.HasAdvantage ? (totalA >= totalB) : (totalA <= totalB);

                if (keepPoolA)
                {
                    result.StandardRolls = poolA;
                    result.DiscardedRolls = poolB;
                    result.BaseRollTotal = totalA;
                }
                else
                {
                    result.StandardRolls = poolB;
                    result.DiscardedRolls = poolA;
                    result.BaseRollTotal = totalB;
                }
                return result; // Exit here to prevent exploding dice logic from applying
            }

            // Standard dice and World of Darkness logic
            foreach (var kvp in profile.SelectedDice)
            {
                int sides = kvp.Key;
                int count = kvp.Value;

                for (int i = 0; i < count; i++)
                {
                    int baseRoll = Random.Shared.Next(1, sides + 1);
                    result.StandardRolls.Add(baseRoll);
                    result.BaseRollTotal += baseRoll;


                    // Handle exploding dice for WoD
                    if (profile.SystemMode == GameSystem.WorldOfDarkness && profile.ExplodingDiceThreshold > 0)
                    {
                        int currentRoll = baseRoll;

                        // While-loop to ensure dice keep exploding infinitely as long as bonus rolls hit the threshold
                        while (currentRoll >= profile.ExplodingDiceThreshold)
                        {
                            currentRoll = Random.Shared.Next(1, sides + 1);
                            result.ExplodedRolls.Add(currentRoll);
                            result.BaseRollTotal += currentRoll;
                        }
                    }
                }
            }
            // Evaluate successes and failures for WoD
            if (profile.SystemMode == GameSystem.WorldOfDarkness)
            {
                // Combine both pools to check for successes and 1s
                var allRolls = new List<int>();
                allRolls.AddRange(result.StandardRolls);
                allRolls.AddRange(result.ExplodedRolls);

                // Count dice that rolled 8 or higher
                result.Successes = allRolls.Count(r => r >= 8);

                // Evaluate failure states
                if (result.Successes == 0)
                {
                    result.IsFailure = true;
                    // Become a critical failure if there is at least one dice with a value of 1 in pool
                    result.IsCriticalFailure = allRolls.Any(r => r == 1);
                }
            }

            return result;
        }

        // Helper method to generate flat list of rolls for a given dice pool
        private List<int> GenerateStandardPool(Dictionary<int, int> selectedDice)
        {
            var rolls = new List<int>();
            foreach( var kvp in selectedDice)
            {
                int sides = kvp.Key;
                int count = kvp.Value;
                for (int i = 0; i < count; i++)
                {
                    rolls.Add(Random.Shared.Next(1, sides + 1));
                }
            }
            return rolls;
        }
    }
}