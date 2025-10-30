using ApiContract;

namespace ValidationApproach.Fluent.Validators;

/// <summary>
/// </summary>
/// <remarks>
/// The validator here is specific to the way FluentValidation works.
/// You as a developer need to know how these validators are hooked up to the processing,
/// how the rules need to be defined (in the constructor here) and how to make complex
/// combinations of rules (nested validation, conditional validation).
/// This is a unique approach for this exact stage/use case which you usually won't get to reuse elsewhere.
/// </remarks>
public class ApplyForInsuranceRequestValidator : AbstractValidator<ApplyForInsuranceRequest> {
  public ApplyForInsuranceRequestValidator(TimeProvider timeProvider) {
    // Applicant is marked as non-nullable, but it's not a type constraint,
    // just a compiler hint. We have to check it for deserialized data.
    RuleFor(r => r.MainApplicant)
      .NotNull()
      .SetValidator(new ApplicantValidator(timeProvider));

    // We use these DSL-like constructs which are essentially reinvented control flow
    When(r => r.Partner != null, () => {
      RuleFor(r => r.Partner!)
        .SetValidator(new ApplicantValidator(timeProvider));
    });

    RuleFor(r => r.PolicyDetails)
      .SetValidator(new PolicyDetailsValidator(timeProvider));

    When(r => r.HasRefusalProblem, () => {
      RuleFor(r => r.RefusalReason)
        .NotNull()
        .Length(1, 200);

      RuleFor(r => r.RefusalYear)
        .Must(y => {
          var currentYear = timeProvider.GetLocalNow().Date.Year;
          var minAllowedYear = currentYear - 10;
          return minAllowedYear <= y && y <= currentYear;
        });
    });
  }
}