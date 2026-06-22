namespace Application.Services.MetrajCalculation;

public interface IMetrajCalculationService
{
  Task<MetrajCalculationResultDto> CalculateAsync(
    MetrajCalculationRequest request,
    CancellationToken cancellationToken = default
  );
}
