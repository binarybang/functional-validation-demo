using ApiContract;

using ErrorsWithPath.Errors;

using RigidDomain;

namespace ValidationApproach.Functional.ErrorsWithPath.SeqInsteadOfError;

public interface IRigidWithTypedErrorSeqMapper {
  Validation<Seq<ValidationError>, InsuranceApplication> MapToDomainModel(ApplyForInsuranceRequest source);
}