using ApiContract;

using LanguageExt.Common;

using RigidDomain;

namespace ValidationApproach.Functional.ErrorsWithPath.ForRigidDomain;

public interface IRigidWithTypedErrorsMapper {
  Validation<Error, InsuranceApplication> MapToDomainModel(ApplyForInsuranceRequest source);
}