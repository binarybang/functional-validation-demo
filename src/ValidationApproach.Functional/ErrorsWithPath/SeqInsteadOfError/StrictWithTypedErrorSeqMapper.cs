using ApiContract;

using ErrorsWithPath.Errors;

using StrictDomain;

using ValidationApproach.Functional.ErrorsWithPath.Errors;

namespace ValidationApproach.Functional.ErrorsWithPath.SeqInsteadOfError;

public class StrictWithTypedErrorSeqMapper : IStrictWithTypedErrorSeqMapper {
  private readonly TimeProvider _timeProvider;

  public StrictWithTypedErrorSeqMapper(TimeProvider timeProvider) {
    _timeProvider = timeProvider;
  }

  public Validation<Seq<ValidationError>, InsuranceApplication> MapToDomainModel(ApplyForInsuranceRequest source) {
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

  private Validation<Seq<ValidationError>, Option<AcceptanceProblem>> MapRefusal(ApplyForInsuranceRequest source, ValuePath valuePath) {
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

  private Validation<Seq<ValidationError>, RefusalReason> MapRefusalReason(string? reason, ValuePath valuePath) {
    if (reason == null) {
      return new FieldIsRequired(valuePath, "Refusal reason").InvalidSeq();
    }

    if (reason.Length is < 1 or > 200) {
      return new LengthOutOfBounds(valuePath, 1, 200).InvalidSeq();
    }

    return RefusalReason.From(reason);
  }

  private Validation<Seq<ValidationError>, RefusalYear> MapRefusalYear(int? year, ValuePath valuePath) {
    if (!year.HasValue) {
      return new FieldIsRequired(valuePath, Option<string>.None).InvalidSeq();
    }
    
    var currentYear = _timeProvider.GetLocalNow().Year;
    var minAllowedYear = currentYear - 10;
    return minAllowedYear <= year.Value && year.Value <= currentYear
      ? RefusalYear.From(year.Value)
      : new YearOutOfBounds(valuePath, minAllowedYear, currentYear, year.Value).InvalidSeq();
  }
  
  private Validation<Seq<ValidationError>, Applicant> MapMainApplicant(ContractApplicant? applicant, ValuePath valuePath) {
    return MapApplicant(applicant, valuePath)
      .Bind(app => app.Match(
        Success<Seq<ValidationError>, Applicant>, 
        new FieldIsRequired(valuePath, "Main applicant").InvalidSeq()));
  }

  private Validation<Seq<ValidationError>, Option<Applicant>> MapApplicant(ContractApplicant? applicant, ValuePath valuePath) {
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

  private Validation<Seq<ValidationError>, DateOfBirth> MapDateOfBirth(DateOnly applicantDateOfBirth, ValuePath valuePath) {
    var currentDate = DateOnly.FromDateTime(_timeProvider.GetLocalNow().Date);
    var minDate = currentDate.AddYears(-100);
    var maxDate = currentDate.AddYears(-18);
    
    return minDate <= applicantDateOfBirth && applicantDateOfBirth <= maxDate
      ? DateOfBirth.From(applicantDateOfBirth)
      : new DateOutOfBounds(valuePath, minDate, maxDate, applicantDateOfBirth).InvalidSeq();
  }

  private Validation<Seq<ValidationError>, FirstName> MapFirstName(string firstName, ValuePath valuePath) {
    return firstName.Length is > 1 and < 50 
      ? FirstName.From(firstName) 
      : new LengthOutOfBounds(valuePath, 1, 50).InvalidSeq();; 
  }
  
  private Validation<Seq<ValidationError>, LastName> MapLastName(string lastName, ValuePath valuePath) {
    return lastName.Length is > 1 and < 50 
      ? LastName.From(lastName) 
      : new LengthOutOfBounds(valuePath, 1, 50).InvalidSeq();; 
  }

  private Validation<Seq<ValidationError>, PolicyDetails> MapPolicyDetails(ContractPolicyDetails source, ValuePath valuePath) {
    var currentDate = DateOnly.FromDateTime(_timeProvider.GetLocalNow().Date);
    return currentDate <= source.StartDate
      ? new PolicyDetails {
        StartDate = StartDate.From(source.StartDate)
      }
      : new StartDateInThePast(valuePath.Combine(nameof(source.StartDate)), source.StartDate).InvalidSeq();;
  }
}