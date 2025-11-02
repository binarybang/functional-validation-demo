#import "@preview/touying:0.6.1": *
#import "/themes/theme-setup.typ": *

= Solution v1: App layer models and FluentValidation

== App layer models 

- We leave existing API contracts untouched and focus on providing a consistent data structure for application layer
- API service's responsibilities are:
  - validation of API contract instances
  - mapping those instances to application layer models

#pagebreak()


=== Optional data: nullable type with required non-nullable members
```cs
public class InsuranceApplication {
    public CustomerIssue? CustomerIssue { get; set; }
}

public class CustomerIssue {
    // No nulls
    public required int Year { get; init; }
    // No nulls (if we do things right AND maintain the course)
    public required string Reason { get; init; }   
}

```
#pagebreak()

=== Alternative data: "ADTs" and maintaining requirements in subtypes

```cs
public abstract record Customer {
    public sealed record CategoryOne(decimal Amount): Customer;

    public sealed record CategoryTwo(decimal Estimate, string Justification): Customer;
}
```

#pagebreak()

```cs
public record Purchase {
    public required Customer Customer { get; init; }
}

// Encoded invariants allow us to be certain what the state is
var amountForCalculations = purchase.Customer switch {
    CategoryOne co => co.Amount,
    CategoryTwo ct => ct.Estimate * 0.9
}

var justificationForNotProvidingExactAmount = purchase.Customer switch {
    CategoryOne co => "",
    CategoryTwo ct => ct.Justification
}

```

== FluentValidation

Validation part of API service responsibilities covered by FluentValidation package.

```cs
public class RequestValidator: AbstractValidator<InsuranceApplicationRequest> {
  public RequestValidator() {
    When(r => r.CustomerHasIssue), () => {
        RuleFor(iar => iar.IssueYear).NotNull();
        RuleFor(iar => iar.IssueReason).NotEmpty().Length(1, 200);
    };
  }
}
```

#pagebreak()

```cs
public class RequestValidator: AbstractValidator<PurchaseRequest> {
  public RequestValidator() {
    When(r => r.CustomerCategory == CustomerCategory.CategoryOne), () => {
        RuleFor(iar => iar.CategoryOneAmount).NotEmpty().Max(200_000);
    };

    When(r => r.CustomerCategory == CustomerCategory.CategoryTwo), () => {
        RuleFor(iar => iar.CategoryTwoEstimation).NotEmpty().Max(100_000);
        RuleFor(iar => iar.CategoryTwoJustification).NotEmpty().Length(1, 200);
    };
  }
}
```

== Implicity as a curse
- FluentValidation doesn't affect data in the API contract instances
- After successful validation we need to map the API contract to model
  - Choice 1: we rely on FluentValidation doing its job and map assuming no inconsitencies
  - Choice 2: we check all of the rules again and keep those checks in sync with validator implementations
- Real implementations we have use a somewhat hybrid approach 