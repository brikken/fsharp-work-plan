# Introduction
Laying out types for a work planning system.
# Notes
## Filters
Filtering happens on either
* properties of an assignment
* values derived by assignment properties (eg. due day)
Can this be made explicit using types? Or introduce a type for an expanded potential assignment and filter on that? Perhaps a type wrapping an assignment with addtional (expanded) values on the side?
## Navigation
Navigation options are determined by the Table aggregate, but they should be saved on cell level for convenience.
