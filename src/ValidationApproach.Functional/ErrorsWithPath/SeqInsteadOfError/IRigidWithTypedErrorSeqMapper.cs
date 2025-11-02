using ApiContract;

using ErrorsWithPath.Errors;

using StrictDomain;

namespace ValidationApproach.Functional.ErrorsWithPath.SeqInsteadOfError;

public interface IRigidWithTypedErrorSeqMapper {
  Validation<Seq<ValidationError>, InsuranceApplication> MapToDomainModel(ApplyForInsuranceRequest source);
}