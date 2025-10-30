namespace LaxDomain;

public record InsuranceApplication {
  public required Applicant MainApplicant { get; init; }
  public required Applicant? Partner { get; init; }
  public required PolicyDetails PolicyDetails { get; init; }
  public required AcceptanceProblem? Refusal { get; init; }
}

public record Applicant(string FirstName, string LastName, DateOnly DateOfBirth);

public record PolicyDetails {
  public required DateOnly StartDate { get; init; }
}

public record AcceptanceProblem {
  public required int Year { get; init; }
  public required string Reason { get; init; }
}