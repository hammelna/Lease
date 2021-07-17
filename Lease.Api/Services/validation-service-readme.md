# Validation Service
This service is a simple service that validates incoming records from a CSV upload file.  Each field in the record will be parsed, validated and added to a lease model.  Error messages will be returned if there are issues with the records.  

If an error in a record is found, it will return immediately instead of aggregating all issues with the record.  This could be changed to return all errors with a given record. It could be further improved by returning the line number of the record being parsed.

## Alternatives
The primary alternative to having an explicit validation service would be to add data annotations to the lease model, and allow the .NET model validation to handle validating the model.  I chose not to take this approach as there would be some complex custom validation annotations, and thought that the validation service would be easier to maintain.