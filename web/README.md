SIStorage service web client.

# Install

    npm install sistorage-client

# Example usage

```typescript
import SIStorageClient from 'sistorage-client';

const client = new SIStorageClient({ serviceUri: '<insert service address here>' });

// Get required facet values
const authors = await client.facets.getAuthorsAsync();

// Search package by filters
const packages = await client.packages.getPackagesAsync({ authorId: authors[0] }, { count: 5 });

```