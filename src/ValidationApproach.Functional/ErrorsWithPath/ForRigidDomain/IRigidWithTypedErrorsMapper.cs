using ApiContract;

using LanguageExt.Common;

using StrictDomain;

namespace ValidationApproach.Functional.ErrorsWithPath.ForRigidDomain;

public interface IRigidWithTypedErrorsMapper {
  Validation<Error, InsuranceApplication> MapToDomainModel(ApplyForInsuranceRequest source);
}