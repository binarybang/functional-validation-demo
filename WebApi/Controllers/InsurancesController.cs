using ApiContract;

using ErrorsWithPath.Errors;

using LanguageExt;
using LanguageExt.Common;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using ValidationApproach.Fluent.Mappers;
using ValidationApproach.Functional.ErrorsWithPath.ForLaxDomain;
using ValidationApproach.Functional.ErrorsWithPath.ForRigidDomain;
using ValidationApproach.Functional.ErrorsWithPath.SeqInsteadOfError;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class InsurancesController : ControllerBase {
  private readonly ILaxWithTypedErrorsMapper _laxFunctionalMapper;
  private readonly IRigidWithTypedErrorsMapper _rigidFunctionalMapper;
  private readonly IRigidWithTypedErrorSeqMapper _rigidFunctionalSeqMapper; 

  public InsurancesController(
    ILaxWithTypedErrorsMapper laxFunctionalMapper, 
    IRigidWithTypedErrorsMapper rigidFunctionalMapper, 
    IRigidWithTypedErrorSeqMapper rigidFunctionalSeqMapper) {
    _laxFunctionalMapper = laxFunctionalMapper;
    _rigidFunctionalMapper = rigidFunctionalMapper;
    _rigidFunctionalSeqMapper = rigidFunctionalSeqMapper;
  }

  [HttpPost("fluent")]
  public Task<string> ApplyForInsuranceFluent(ApplyForInsuranceRequest request) {
    var insuranceApplication = ApplyForInsuranceRequestMapper.MapToDomainModel(request);
    return Task.FromResult("Application is ready for processing: " + insuranceApplication);
  }
  
  [HttpPost("functional-lax")]
  public IActionResult ApplyForInsuranceFunctionalLax(ApplyForInsuranceRequest request) {
    var validatedApplication = _laxFunctionalMapper.MapToDomainModel(request);
    return validatedApplication
      .Match(
        app => Ok("Application is ready for processing:" + app),
        err => ValidationProblem(ConvertErrorToValidationProblemDetails(err))
      );
  }
  
  [HttpPost("functional-rigid")]
  public IActionResult ApplyForInsuranceFunctionalRigid(ApplyForInsuranceRequest request) {
    var validatedApplication = _rigidFunctionalMapper.MapToDomainModel(request);
    return validatedApplication
      .Match(
        app => Ok("Application is ready for processing:" + app),
        err => ValidationProblem(ConvertErrorToValidationProblemDetails(err))
      );
  }
  
  [HttpPost("functional-rigid-seq")]
  public IActionResult ApplyForInsuranceFunctionalRigidSeq(ApplyForInsuranceRequest request) {
    var validatedApplication = _rigidFunctionalSeqMapper.MapToDomainModel(request);
    return validatedApplication
      .Match(
        app => Ok("Application is ready for processing:" + app),
        err => ValidationProblem(ConvertErrorSeqToValidationProblemDetails(err))
      );
  }

  private static ModelStateDictionary ConvertErrorToValidationProblemDetails(Error errors) {
    return errors
      .Filter<ValidationError>()
      .AsIterable()
      .Cast<ValidationError>()
      .Fold(
        new ModelStateDictionary(),
        (msd, e) => {
          msd.AddModelError(e.Path.FullPath(), e.Message);
          return msd;
        });
  }
  
  private static ModelStateDictionary ConvertErrorSeqToValidationProblemDetails(Seq<ValidationError> errors) {
    return errors
      .Fold(
        new ModelStateDictionary(),
        (msd, e) => {
          msd.AddModelError(e.Path.FullPath(), e.Message);
          return msd;
        });
  }
}