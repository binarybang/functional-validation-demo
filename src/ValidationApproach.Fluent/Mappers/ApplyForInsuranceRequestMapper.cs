using System.Diagnostics.CodeAnalysis;

using ApiContract;

using LaxDomain;

namespace ValidationApproach.Fluent.Mappers;

public static class ApplyForInsuranceRequestMapper {
  public static InsuranceApplication MapToDomainModel(ApplyForInsuranceRequest source) {
    return new InsuranceApplication {
      MainApplicant = BuildApplicant(source.MainApplicant),
      Partner = BuildApplicant(source.Partner),
      PolicyDetails = new PolicyDetails {
        StartDate = source.PolicyDetails.StartDate
      },
      Refusal = source.HasRefusalProblem
        ? new AcceptanceProblem {
          /* We have to use ! to explicitly ignore the potential null value.
           * There's no constraint here for that, we just _know_ that it _should not_ be there
           * since we call a validator before this mapper.
           *
           * If we for whatever reason skip the validator call, static analysis won't pick it up.
           * And unless you have proper integration tests that perform both validation and mapping for one test,
           * it won't be seen in test runs either!
           */
          Year = source.RefusalYear!.Value,
          Reason = source.RefusalReason!
        }
        : null
    };
  }

  [return: NotNullIfNotNull(nameof(source))]
  private static Applicant? BuildApplicant(ContractApplicant? source) {
    return source is null 
      ? null 
      : new Applicant(source.FirstName, source.LastName, source.DateOfBirth);
  }
}