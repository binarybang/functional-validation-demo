using ApiContract;

using LanguageExt.Common;

using LaxDomain;

namespace ValidationApproach.Functional.ErrorsWithPath.ForLaxDomain;

public interface ILaxWithTypedErrorsMapper {
  Validation<Error, InsuranceApplication> MapToDomainModel(ApplyForInsuranceRequest source);
}