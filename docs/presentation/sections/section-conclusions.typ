#import "@preview/touying:0.6.1": *
#import "/themes/theme-setup.typ": *

= Conclusions

== Advantages
- Explicit expectations: you deal with `Validation<E, A>`, not `A` with it quietly failing
- Single implementation for validation and mapping
  - Strict modeling is easier now
  - Models are in app or domain layer, so encoded constraints are usable everywhere
- Unit tests are stricter: type system makes assertions direct

#pagebreak()

- The approach is not in any way tied to WebAPIs
 - _Counterargument_: neither is FluentValidation. I'm yet to see it used anywhere else though, might be because of lack of experience
- LanguageExt doesn't need to spread beyond the API layer
 - On the other hand, it certainly helps apply stricter principles on a larger scale

== So why don't we do this already?

=== Object-oriented habits

- Classic OO practices don't make use of these abstractions
- Understanding functional concepts requires cognitive effort
- Modern C\# is slowly going somewhere in that direction:
  - Nominal type unions \
    `Validation`/`Option` etc. are easier to implement that way
  - "Generic math": #link("https://learn.microsoft.com/en-us/dotnet/api/system.numerics.iadditionoperators-3")[IAdditionOperators] + #link("https://learn.microsoft.com/en-us/dotnet/api/system.numerics.iadditiveidentity-2")[IAdditiveIdentity]≈ Semigroup + Monoid
  - Built-in implementation is unlikely, see #link("https://github.com/dotnet/runtime/issues/76225#issuecomment-1264640937")[related GH discussion]

#pagebreak()

=== Risk/reward balance
- Introducing a core non-MS third-party dependency is hard (case in point: JodaTime/NodaTime)
- Proper use of libs like LanguageExt is as widespread in code base as BCL types (e.g. Tasks, Nullable/NRT)
- Ecosystem around it is not really there
  - Unit testing is, while robust, not eased with ready-to-use helpers
- Licensing has recently become a widely discussed issue
  - FluentAssertion is the most obvious example
  - AutoMapper, MediatR are a bit worse impact-wise but for now not as disruptive

#pagebreak()

=== LanguageExt is a liability for a commercial project
- Technically under MIT license
- Author plans to introduce commercial licensing (see #link("https://github.com/louthy/language-ext/discussions/1289")[GH discussion]), no idea about actual pricing yet
- After a major rework the latest version is a beta version (author treats it as closer to RC with a longer test period)

== Going forward
- API contracts will always remain a pain due to interdependencies
- Solution v1 is what we do now in new services
- Solution v2 doesn't have to depend on LanguageExt
  - No similarly convenient alternatives
  - Reusable ad-hoc implementation is not complex provided we see use in it
  - Usage of value objects/Vogen is orthogonal to validation

#pagebreak()

- Understanding the problem is a requirement for considering solutions
- Scope of adoption matters

#focus-slide[
  Thanks!
]
