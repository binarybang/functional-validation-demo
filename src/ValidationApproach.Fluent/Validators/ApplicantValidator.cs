using ApiContract;

namespace ValidationApproach.Fluent.Validators;

public class ApplicantValidator : AbstractValidator<ContractApplicant> {
  public ApplicantValidator(TimeProvider timeProvider) {
    RuleFor(r => r.FirstName)
      .NotEmpty()
      .Length(1, 50);

    RuleFor(r => r.LastName)
      .NotEmpty()
      .Length(1, 60);

    RuleFor(r => r.DateOfBirth)
      .Must(d => {
        var currentDate = DateOnly.FromDateTime(timeProvider.GetLocalNow().Date);
        var maxDate = currentDate.AddYears(-18);
        var minDate = currentDate.AddYears(-100);

        return minDate <= d && d <= maxDate;
      });
  }
}