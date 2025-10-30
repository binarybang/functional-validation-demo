using ApiContract;

using LanguageExt.Common;

using LaxDomain;

using ErrorsWithPath.Errors;

namespace ValidationApproach.Functional.ErrorsWithPath.ForLaxDomain;

public class LaxWithTypedErrorsMapper : ILaxWithTypedErrorsMapper {
  private readonly TimeProvider _timeProvider;

  public LaxWithTypedErrorsMapper(TimeProvider timeProvider) {
    _timeProvider = timeProvider;
  }

  public Validation<Error, InsuranceApplication> MapToDomainModel(ApplyForInsuranceRequest source) {
    var root = ValuePath.Root;
    return (
      MapMainApplicant(source.MainApplicant, root.Combine(nameof(source.MainApplicant))),
      MapApplicant(source.Partner, root.Combine(nameof(source.Partner))),
      MapPolicyDetails(source.PolicyDetails, root.Combine(nameof(source.PolicyDetails))),
      MapRefusal(source, root)
    ).Apply((ma, p, pd, refusal) => new InsuranceApplication {
      MainApplicant = ma,
      Partner = p.Match(a => a, (Applicant?)null),
      PolicyDetails = pd,
      Refusal = refusal.Match(a => a, (AcceptanceProblem?)null)
    }).As();
  }

  private Validation<Error, Option<AcceptanceProblem>> MapRefusal(ApplyForInsuranceRequest source, ValuePath valuePath) {
    if (!source.HasRefusalProblem) {
      return Option<AcceptanceProblem>.None;
    }

    return (
        MapRefusalYear(source.RefusalYear, valuePath.Combine(nameof(source.RefusalYear))),
        MapRefusalReason(source.RefusalReason, valuePath.Combine(nameof(source.RefusalReason))))
      .Apply((year, reason) => Some(new AcceptanceProblem {
        Year = year,
        Reason = reason
      })).As();
  }

  private Validation<Error, string> MapRefusalReason(string? reason, ValuePath valuePath) {
    if (reason == null) {
      return new FieldIsRequired(valuePath, "Refusal reason");
    }

    if (reason.Length is < 1 or > 200) {
      return new LengthOutOfBounds(valuePath, 1, 200);
    }

    return reason;
  }

  private Validation<Error, int> MapRefusalYear(int? year, ValuePath valuePath) {
    if (!year.HasValue) {
      return new FieldIsRequired(valuePath, Option<string>.None);
    }
    
    var currentYear = _timeProvider.GetLocalNow().Year;
    var minAllowedYear = currentYear - 10;
    return minAllowedYear <= year.Value && year.Value <= currentYear
      ? year.Value
      : new YearOutOfBounds(valuePath, minAllowedYear, currentYear, year.Value);
  }
  
  private Validation<Error, Applicant> MapMainApplicant(ContractApplicant? applicant, ValuePath valuePath) {
    return MapApplicant(applicant, valuePath)
      .Bind(app => app.Match(
        Success<Error, Applicant>, 
        new FieldIsRequired(valuePath, "Main applicant")));
  }

  private Validation<Error, Option<Applicant>> MapApplicant(ContractApplicant? applicant, ValuePath valuePath) {
    var possibleValidatedApplicant =
      from presentApplicant in Optional(applicant)
      let validParts = (
        MapFirstName(presentApplicant.FirstName, valuePath.Combine(nameof(presentApplicant.FirstName))),
        MapLastName(presentApplicant.LastName, valuePath.Combine(nameof(presentApplicant.LastName))),
        MapDateOfBirth(presentApplicant.DateOfBirth, valuePath.Combine(nameof(presentApplicant.DateOfBirth))))
      select validParts
        .Apply((fn, ln, dob) => Some(new Applicant(fn, ln, dob)))
        .As();
    
    return possibleValidatedApplicant.IfNone(Option<Applicant>.None);
  }

  private Validation<Error, DateOnly> MapDateOfBirth(DateOnly applicantDateOfBirth, ValuePath valuePath) {
    var currentDate = DateOnly.FromDateTime(_timeProvider.GetLocalNow().Date);
    var minDate = currentDate.AddYears(-100);
    var maxDate = currentDate.AddYears(-18);
    
    return minDate <= applicantDateOfBirth && applicantDateOfBirth <= maxDate
      ? applicantDateOfBirth
      : new DateOutOfBounds(valuePath, minDate, maxDate, applicantDateOfBirth);
  }

  private Validation<Error, string> MapFirstName(string firstName, ValuePath valuePath) {
    return firstName.Length is > 1 and < 50 
      ? firstName 
      : new LengthOutOfBounds(valuePath, 1, 50); 
  }
  
  private Validation<Error, string> MapLastName(string lastName, ValuePath valuePath) {
    return lastName.Length is > 1 and < 50 
      ? lastName 
      : new LengthOutOfBounds(valuePath, 1, 50); 
  }

  private Validation<Error, PolicyDetails> MapPolicyDetails(ContractPolicyDetails source, ValuePath valuePath) {
    var currentDate = DateOnly.FromDateTime(_timeProvider.GetLocalNow().Date);
    return currentDate <= source.StartDate
      ? new PolicyDetails {
        StartDate = source.StartDate
      }
      : new StartDateInThePast(valuePath.Combine(nameof(source.StartDate)), source.StartDate);
  }
}