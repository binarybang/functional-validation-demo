namespace ApiContract;

public record ApplyForInsuranceRequest {
  public required ContractApplicant MainApplicant { get; init; }
  public ContractApplicant? Partner { get; init; }
  public required ContractPolicyDetails PolicyDetails { get; init; }
  public required bool HasRefusalProblem { get; init; }
  public int? RefusalYear { get; init; }
  public string? RefusalReason { get; init; }
}

public record ContractApplicant {
  public required string FirstName { get; init; }
  public required string LastName { get; init; }
  public required DateOnly DateOfBirth { get; init; }
}

public record ContractPolicyDetails {
  public required DateOnly StartDate { get; init; }
}