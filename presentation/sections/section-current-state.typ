#import "@preview/touying:0.6.1": *
#import themes.metropolis: *

= Legacy state

== API contracts are the base

- Modeling the data structures is minimal with API contracts often being the only representation.
- API contracts are modeled in a way that makes them easier to 
  - consume them from the frontend application
  - copy them when automation doesn't provide reusable contracts

This leads to various issues with service design.

== Inconsistent state

- Contracts are designed based on what the frontend input model provides
- Backend models do not have invariants encoded in them
- Constant possibility of inconsistent state

#alternatives( 
[
```cs
// Optional data
public class InsuranceApplicationRequest {
    public bool CustomerHasIssue { get; set; }
    // This must not be NULL when CustomerHasIssue is true
    public int? IssueYear { get; set; }
    // This must not be NULL when CustomerHasIssue is true
    public string IssueReason { get; set; }
}

```

],
[

```cs
// Alternatives that depend on a certain property
public class PurchaseRequest {
    public CustomerCategory CustomerCategory { get; set; }
    // This is only present if CustomerCategory is Category1
    public decimal? CategoryOneAmount { get; set; }
    // This is only present if CustomerCategory is Category2
    public decimal? CategoryTwoEstimate { get; set; }
    // This is only present if CustomerCategory is Category2
    public string? CategoryTwoJustification { get; set; }
}

```
])