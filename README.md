# Lease API
## Overview
This project is an example of a .NET 5 API that includes endpoints that allow for: 
- Uploading a file with lease information
- Viewing Leases that have been uploaded
- Downloading an example monthly payment file

## Assumptions
This project assumes the API is a single tenant application.  A small number of modifications would allow for this to be multi-tenant, particularly adding an Id for the organization to whom the leases belong.

## Additional Readmes
Throughout the solution there are additional markdown files that explain some of the decisions made, along with other options for how things could have been done.