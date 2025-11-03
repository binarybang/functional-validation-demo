using ApiContract;

using LanguageExt.Common;

using StrictDomain;

namespace ValidationApproach.Functional.ErrorsWithPath.ForStrictDomain;

public interface IStrictWithTypedErrorsMapper {
  Validation<Error, InsuranceApplication> MapToDomainModel(ApplyForInsuranceRequest source);
}