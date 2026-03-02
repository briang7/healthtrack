namespace HealthTrack.Application.Common.Interfaces;

public record DrugInteraction(string Drug1, string Drug2, string Severity, string Description);

public interface IDrugInteractionService
{
    Task<List<DrugInteraction>> CheckInteractionsAsync(string medication, List<string> currentMedications, CancellationToken ct = default);
}
