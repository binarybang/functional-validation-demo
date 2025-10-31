using ValidationApproach.Functional;
using ValidationApproach.Functional.ErrorsWithPath.ForLaxDomain;
using ErrorsWithPath;

using ValidationApproach.Functional.ErrorsWithPath.ForRigidDomain;
using ValidationApproach.Functional.ErrorsWithPath.SeqInsteadOfError;

namespace WebApi.Setup;

public static class ServiceCollectionExtensions {
  public static IServiceCollection AddApplicationServices(this IServiceCollection services) {
    return services
      .AddSingleton(TimeProvider.System)
      .AddValidationServices();
  }
  
  public static IServiceCollection AddValidationServices(this IServiceCollection services) {
    return services
      .AddTransient<IRelaxedFunctionalApplyForInsuranceRequestMapper,
        RelaxedFunctionalApplyForInsuranceRequestMapper>()
      .AddTransient<ILaxWithTypedErrorsMapper, LaxWithTypedErrorsMapper>()
      .AddTransient<IRigidWithTypedErrorsMapper, RigidWithTypedErrorsMapper>()
      .AddTransient<IRigidWithTypedErrorSeqMapper, RigidWithTypedErrorSeqMapper>();
  }
}