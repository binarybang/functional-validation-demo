#import "@preview/touying:0.6.1": *
#import themes.metropolis: *

= Solution v2: Validation as transformation

== General idea
- API contract instance is validated and transformed at the same time
- Result type contains:
  - a valid transformed app layer model when validation is successful
  - "list" of issues when validation failed
- The described set of alternatives implies all errors are blocking
  - It's possible to implement a variation with non-critical issues
  - Real use-cases predominantly follow error-only pattern

== Sketching the data structure

```cs
public abstract record Validation<E, A> {
	public sealed record Valid(A Value): Validation<E, A>;
	public sealed record Invalid(List<E> Errors): Validation<E, A>;
}

public static class Validator {
  public static Validation<string, InsuranceApplication> ValidateApplication(InsuranceApplicationRequest req) {
    /// implementation here
  }
}
```

== Functional requirements
- `Invalid` should contain all of the errors we could find in the validation
- We can explicitly operate on validated pieces of information (no rechecks)
  - Modern C\# provides some ad-hoc type guards but it's not enough
- Errors should be linked to data that triggered them
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "MainApplicant.DateOfBirth": [
      "Applicant must be at least 18 years old and at most 100 years old"
    ]
  },
  "traceId": "00-4fc2b47a18250f1003cf0646b5803f8d-27e604cb6e0c29db-00"
}
```

== LanguageExt 

- #link("https://github.com/louthy/language-ext")[LanguageExt] is a library which implements sort of "functional BCL" for C\#
- Provides a wide variety of functional data structures and behaviors
- We're primarily interested in `Validation<F, A>` type but a few others are also relevant

== Short detour: `Semigroup`

- `Semigroup` defines an "addition" operation \
```cs
public interface Semigroup<A> where A : Semigroup<A>
{
  A Combine(A rhs);
}
```
- Example: integers and +, strings/lists and concatenation
- `Combine` allows for merging values into one we can return

== Short detour: `Monoid`

- `Monoid` is a `Semigroup` with a neutral/empty element \
```cs
public interface Monoid<A> : Semigroup<A> where A : Monoid<A>
{
  static abstract A Empty { get; }
}
```
- Example: integers and +, strings/lists and concatenation
- Positive integers and + aren't a monoid 
- `Empty` allows to start with "zero" and merge other instances into it i.e. aggregate/reduce

== LanguageExt's Validation

```cs
public abstract record Validation<F, A>() where F : Monoid<F> {
  ...
}
```
- Technically `Semigroup` is enough: we only need to combine errors we return
- LanguageExt adds a `Monoid` constraint to implement filtering for `A`
   - if filtering fails but there were no errors we return `Monoid<F>.Empty`
- LanguageExt provides `Error` type which implements `Monoid<Error>` for this purpose

== Combining successful results
```cs
public class Customer(string FirstName, string LastName);
...
Validation<Error, string> firstName = ValidateFirstName(rawFirstName);
Validation<Error, string> lastName = ValidateLastName(rawLastName);

// How do we extract and use the validated values?

```

== Short detour: `Applicative`
- Classic definition is a bit too abstract to dive into here
- Detailed explanations: #link("https://typelevel.org/cats/typeclasses/applicative.html")[Scala Cats],
 #link("https://paullouth.com/higher-kinds-in-c-with-language-ext-part-4-applicatives/")[Paul Louth's blog]
- Practical application: allows to "map" over multiple "containerized" values at once
```cs
Validation<Error, string> firstName = ValidateFirstName(rawFirstName);
Validation<Error, string> lastName = ValidateLastName(rawLastName);

var customer = (firstName, lastName)
  // fn/ln here are strings
  // new Customer is only called if both tuple items are Valid
  // otherwise a new Invalid is created 
  // Errors are merged by Combine provided by Semigroup/Monoid
  .Apply((fn, ln) => new Customer(fn, ln));
```

== Demo
#link("https://github.com/binarybang/functional-validation-demo")[Demo repository on Github]
- Classic FluentValidation-based implementation
- A variation of the endpoint where validation is not enabled
- Basic `Validation`-based implementation (`LanguageExt.Common.Error` as error monoid, lax domain)
- `Validation`-based implementation with strict domain and `Vogen` library for custom "value object" types
- Same but with `Seq<ValidationError>` as error monoid for a stricter result


