using ApiContract;

using FluentValidation;

using ValidationApproach.Fluent.Validators;
using ValidationApproach.Functional;
using ValidationApproach.Functional.ErrorsWithPath.ForLaxDomain;
using ValidationApproach.Functional.ErrorsWithPath.ForStrictDomain;
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
      .AddScoped<IValidator<ApplyForInsuranceRequest>, ApplyForInsuranceRequestValidator>()
      .AddTransient<IRelaxedFunctionalApplyForInsuranceRequestMapper,
        RelaxedFunctionalApplyForInsuranceRequestMapper>()
      .AddTransient<ILaxWithTypedErrorsMapper, LaxWithTypedErrorsMapper>()
      .AddTransient<IStrictWithTypedErrorsMapper, StrictWithTypedErrorsMapper>()
      .AddTransient<IStrictWithTypedErrorSeqMapper, StrictWithTypedErrorSeqMapper>();
  }
}