using ApiContract;

using ErrorsWithPath.Errors;

using Microsoft.Extensions.Time.Testing;

using Shouldly;

using ValidationApproach.Functional.ErrorsWithPath.Errors;
using ValidationApproach.Functional.ErrorsWithPath.SeqInsteadOfError;

namespace ValidationApproach.Functional.Tests;

public class StrictWithTypedErrorSeqMapperTests {
  private readonly StrictWithTypedErrorSeqMapper _sut;
  private readonly TimeProvider _timeProvider;

  public StrictWithTypedErrorSeqMapperTests() {
    _timeProvider = new FakeTimeProvider(new DateTime(2025, 11, 1));
    
    
    _sut = new StrictWithTypedErrorSeqMapper(_timeProvider);
  }
  
  [Fact]
  public void MapToDomainModel_Passes_WhenRequestIsValid() {
    var request = new ApplyForInsuranceRequest {
      MainApplicant = new ContractApplicant {
        DateOfBirth = new DateOnly(2000, 1, 1),
        FirstName = "John",
        LastName = "Doe",
      },
      HasRefusalProblem = false,
      PolicyDetails = new ContractPolicyDetails {
        StartDate = new DateOnly(2025, 11, 2)
      }
    };
    
    var validatedApplication = _sut.MapToDomainModel(request);
    
    validatedApplication.IsSuccess.ShouldBeTrue();
  }
  
  
  [Fact]
  public void MapToDomainModel_Fails_WhenApplicantIsTooYoung() {
    var request = new ApplyForInsuranceRequest {
      MainApplicant = new ContractApplicant {
        DateOfBirth = new DateOnly(2020, 1, 1),
        FirstName = "John",
        LastName = "Doe",
      },
      HasRefusalProblem = false,
      PolicyDetails = new ContractPolicyDetails {
        StartDate = new DateOnly(2025, 11, 2)
      }
    };
    
    var validatedApplication = _sut.MapToDomainModel(request);

    var expectedError = new DateOutOfBounds(
      ValuePath.FromString("MainApplicant.DateOfBirth"),
      new DateOnly(1925, 11, 1),
      new DateOnly(2007, 11, 1),
      new DateOnly(2020, 1, 1));
    
    validatedApplication.IsFail.ShouldBeTrue();
    validatedApplication.IfFail(d => {
      d.ShouldContain(expectedError);
    });
  }
}