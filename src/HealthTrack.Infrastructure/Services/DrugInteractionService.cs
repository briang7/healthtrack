using HealthTrack.Application.Common.Interfaces;

namespace HealthTrack.Infrastructure.Services;

public class DrugInteractionService : IDrugInteractionService
{
    private static readonly List<(string Drug1, string Drug2, string Severity, string Description)> KnownInteractions =
    [
        ("Warfarin", "Aspirin", "High",
            "Increased risk of bleeding. Aspirin inhibits platelet aggregation and may enhance the anticoagulant effect of Warfarin."),
        ("Warfarin", "Ibuprofen", "High",
            "NSAIDs increase the risk of gastrointestinal bleeding and may enhance the anticoagulant effect of Warfarin."),
        ("Lisinopril", "Potassium", "Moderate",
            "ACE inhibitors can increase potassium levels. Concurrent use with potassium supplements may cause hyperkalemia."),
        ("Lisinopril", "Spironolactone", "Moderate",
            "Both agents can increase serum potassium. Monitor potassium levels closely."),
        ("Metformin", "Alcohol", "Moderate",
            "Alcohol potentiates the effect of Metformin on lactate metabolism, increasing the risk of lactic acidosis."),
        ("Simvastatin", "Amiodarone", "High",
            "Amiodarone inhibits CYP3A4 and increases Simvastatin levels, raising the risk of rhabdomyolysis."),
        ("Simvastatin", "Grapefruit", "Moderate",
            "Grapefruit juice inhibits CYP3A4 metabolism and may increase Simvastatin blood levels."),
        ("Ciprofloxacin", "Antacids", "Moderate",
            "Antacids containing aluminum or magnesium reduce the absorption of Ciprofloxacin."),
        ("Methotrexate", "NSAIDs", "High",
            "NSAIDs may reduce renal clearance of Methotrexate, increasing toxicity risk."),
        ("Fluoxetine", "Tramadol", "High",
            "Concurrent use increases the risk of serotonin syndrome and may lower the seizure threshold."),
        ("Amoxicillin", "Methotrexate", "Moderate",
            "Amoxicillin may reduce renal clearance of Methotrexate, potentially increasing toxicity."),
        ("Digoxin", "Amiodarone", "High",
            "Amiodarone increases Digoxin levels and may cause toxicity. Reduce Digoxin dose by 50%."),
        ("Clopidogrel", "Omeprazole", "Moderate",
            "Omeprazole inhibits CYP2C19 and may reduce the antiplatelet effect of Clopidogrel."),
    ];

    public Task<List<DrugInteraction>> CheckInteractionsAsync(
        string medication, List<string> currentMedications, CancellationToken ct = default)
    {
        var interactions = new List<DrugInteraction>();
        var medLower = medication.ToLower();

        foreach (var currentMed in currentMedications)
        {
            var currentLower = currentMed.ToLower();

            foreach (var (drug1, drug2, severity, description) in KnownInteractions)
            {
                var d1Lower = drug1.ToLower();
                var d2Lower = drug2.ToLower();

                if ((medLower.Contains(d1Lower) && currentLower.Contains(d2Lower)) ||
                    (medLower.Contains(d2Lower) && currentLower.Contains(d1Lower)))
                {
                    interactions.Add(new DrugInteraction(medication, currentMed, severity, description));
                }
            }
        }

        return Task.FromResult(interactions);
    }
}
