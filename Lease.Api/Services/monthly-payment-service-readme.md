# Monthly Payment Service
The approach taken for monthly payments starts with pulling leases from the database that cross the date range in some fashion (start is within range, end is within range or the whole lease is within range).  From there I created the monthly payment schedule for each lease.  This results in some unnecessary work as we may not need all monthly payment records for a lease based on the date range.

Using the monthly payments from each lease in the range, I flatten the results into a single list of payments, group the payments by year, aggregating values that are in the same month of the same year, then finally storing those results in a dictionary.  I then pull valid values out of the dictionary to create the list that ends up being used for the export.

## Alternatives
I think this would be simpler to represent if we maintained the monthly payments per lease within the database.  Much of the aggregation could be done at the database level and would substantially simplify the code to put together the export.  The most challenging part of this approach would be maintaining the database records when a lease changed.