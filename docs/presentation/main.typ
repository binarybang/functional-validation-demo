#import "@preview/touying:0.6.1": *
#import "@preview/codly:1.3.0": *
#import "@preview/codly-languages:0.1.1": *

#import "themes/theme-setup.typ": *

#show: presentation-theme.with(
    aspect-ratio: "16-9",
    config-info(
        title: upper("Functional validation"),
        subtitle: [Improving service design with functional concepts]
    )
)

#show: codly-init.with(
)

#show raw.where(block: true): set text(0.8em)
#show text: set text(0.9em)

#codly(
    number-format: none,
    languages: codly-languages
)

#title-slide()

#outline(depth: 1)

#include "sections/section-current-state.typ"
#include "sections/section-solution-v1.typ"
#include "sections/section-solution-v2.typ"
#include "sections/section-conclusions.typ"
