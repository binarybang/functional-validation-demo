using ApiContract;

using LanguageExt.Common;

using LaxDomain;

namespace ValidationApproach.Functional;

public interface IRelaxedFunctionalApplyForInsuranceRequestMapper {
  Validation<Error, InsuranceApplication> MapToDomainModel(ApplyForInsuranceRequest source);
}