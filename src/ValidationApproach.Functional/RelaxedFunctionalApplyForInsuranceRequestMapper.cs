using ApiContract;

using LanguageExt.Common;

using LaxDomain;

namespace ValidationApproach.Functional;

public class RelaxedFunctionalApplyForInsuranceRequestMapper : IRelaxedFunctionalApplyForInsuranceRequestMapper {
  private readonly TimeProvider _timeProvider;

  public RelaxedFunctionalApplyForInsuranceRequestMapper(TimeProvider timeProvider) {
    _timeProvider = timeProvider;
  }

  public Validation<Error, InsuranceApplication> MapToDomainModel(ApplyForInsuranceRequest source) {
    return (
      MapMainApplicant(source.MainApplicant),
      MapApplicant(source.Partner),
      MapPolicyDetails(source.PolicyDetails),
      MapRefusal(source)
    ).Apply((ma, p, pd, refusal) => new InsuranceApplication {
      MainApplicant = ma,
      Partner = p.Match(a => a, (Applicant?)null),
      PolicyDetails = pd,
      Refusal = refusal.Match(a => a, (AcceptanceProblem?)null)
    }).As();
  }

  private Validation<Error, Option<AcceptanceProblem>> MapRefusal(ApplyForInsuranceRequest source) {
    if (!source.HasRefusalProblem) {
      return Option<AcceptanceProblem>.None;
    }

    return (
        MapRefusalYear(source.RefusalYear),
        MapRefusalReason(source.RefusalReason))
      .Apply((year, reason) => Some(new AcceptanceProblem {
        Year = year,
        Reason = reason
      })).As();
  }

  private Validation<Error, string> MapRefusalReason(string? reason) {
    if (reason == null) {
      return Error.New("Refusal reason needs to be specified");
    }

    if (reason.Length is < 1 or > 200) {
      return Error.New("Refusal reason must be between 1 and 200 characters");
    }

    return reason;
  }

  private Validation<Error, int> MapRefusalYear(int? year) {
    if (!year.HasValue) {
      return Error.New("Refusal year needs to be specified");
    }
    
    var currentYear = _timeProvider.GetLocalNow().Year;
    var minAllowedYear = currentYear - 10;
    return minAllowedYear <= year.Value && year.Value <= currentYear
      ? year.Value
      : Error.New($"Refusal year must be between {minAllowedYear} and {currentYear}");
  }
  
  private Validation<Error, Applicant> MapMainApplicant(ContractApplicant? applicant) {
    return MapApplicant(applicant)
      .Bind(app => app.Match(
        Success<Error, Applicant>, 
        Fail<Error, Applicant>(Error.New("Applicant has to be present"))));
  }

  private Validation<Error, Option<Applicant>> MapApplicant(ContractApplicant? applicant) {
    var possibleValidatedApplicant =
      from presentApplicant in Optional(applicant)
      let validParts = (
        MapFirstName(presentApplicant.FirstName),
        MapLastName(presentApplicant.LastName),
        MapDateOfBirth(presentApplicant.DateOfBirth))
      select validParts
        .Apply((fn, ln, dob) => Some(new Applicant(fn, ln, dob)))
        .As();
    
    return possibleValidatedApplicant.IfNone(Option<Applicant>.None);
  }

  private Validation<Error, DateOnly> MapDateOfBirth(DateOnly applicantDateOfBirth) {
    var currentDate = DateOnly.FromDateTime(_timeProvider.GetLocalNow().Date);
    var minDate = currentDate.AddYears(-100);
    var maxDate = currentDate.AddYears(-18);
    
    return minDate <= applicantDateOfBirth && applicantDateOfBirth <= maxDate
      ? applicantDateOfBirth
      : Error.New("Age should be between 18 and 100");
  }

  private Validation<Error, string> MapFirstName(string firstName) {
    return firstName.Length is > 1 and < 50 
      ? firstName 
      : Error.New("First name must be between 1 and 50 characters"); 
  }
  
  private Validation<Error, string> MapLastName(string lastName) {
    return lastName.Length is > 1 and < 50 
      ? lastName 
      : Error.New("Last name must be between 1 and 50 characters"); 
  }

  private Validation<Error, PolicyDetails> MapPolicyDetails(ContractPolicyDetails source) {
    var currentDate = DateOnly.FromDateTime(_timeProvider.GetLocalNow().Date);
    return currentDate <= source.StartDate
      ? new PolicyDetails {
        StartDate = source.StartDate
      }
      : Error.New("Policy start date must not be in the past");
  }
}