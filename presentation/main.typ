#import "@preview/touying:0.6.1": *
#import "@preview/codly:1.3.0": *
#import "@preview/codly-languages:0.1.1": *

#import themes.metropolis: *

#show: metropolis-theme.with(
    aspect-ratio: "16-9",
    config-info(
        title: "Functional validation",
        subtitle: [Applying "functional" concepts to .NET WebAPI service design with LanguageExt],
        author: "Ivan Kashtanov"
    )
)

#show: codly-init.with(
)

#show raw.where(block: true): set text(0.75em)

#codly(
    number-format: none,
    languages: codly-languages
)

#title-slide()

#outline()

#include "sections/section-current-state.typ"
#include "sections/section-solution-v1.typ"
#include "sections/section-solution-v2.typ"
#include "sections/section-conclusions.typ"
