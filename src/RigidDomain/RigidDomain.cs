using LanguageExt;

using Vogen;

namespace RigidDomain;

public record InsuranceApplication {
  public required Applicant MainApplicant { get; init; }
  public required Option<Applicant> Partner { get; init; }
  public required PolicyDetails PolicyDetails { get; init; }
  public required Option<AcceptanceProblem> Refusal { get; init; }
}

public record Applicant(FirstName FirstName, LastName LastName, DateOfBirth DateOfBirth);

public record PolicyDetails {
  public required StartDate StartDate { get; init; }
}

public record AcceptanceProblem {
  public required RefusalYear Year { get; init; }
  public required RefusalReason Reason { get; init; }
}

[ValueObject<DateOnly>]
public partial struct StartDate;

[ValueObject<DateOnly>]
public partial struct DateOfBirth;

[ValueObject<int>]
public partial struct RefusalYear;

[ValueObject<string>]
public partial struct RefusalReason;

[ValueObject<string>]
public partial struct FirstName;

[ValueObject<string>]
public partial struct LastName;


