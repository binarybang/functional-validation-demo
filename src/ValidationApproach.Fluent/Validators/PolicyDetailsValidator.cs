using ApiContract;

namespace ValidationApproach.Fluent.Validators;

public class PolicyDetailsValidator : AbstractValidator<ContractPolicyDetails> {
  public PolicyDetailsValidator(TimeProvider timeProvider) {
    RuleFor(r => r.StartDate)
      .Must(d => DateOnly.FromDateTime(timeProvider.GetLocalNow().Date) <= d)
      .WithMessage("Start date must not be in the past");
  }
}