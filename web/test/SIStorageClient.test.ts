import SIStorageClient from '../src/SIStorageClient';
import SIStorageClientOptions from '../src/SIStorageClientOptions';

const options: SIStorageClientOptions = {
	serviceUri: 'http://vladimirkhil.com/sistorage'
};

// TODO: these are some smoke test. More tests that check packages properties and different filter and sorting parameters needed to be added later

const siStorageClient = new SIStorageClient(options);

test('Get authors', async () => {
	const authors = await siStorageClient.facets.getAuthorsAsync();
	expect(authors).not.toBeNull();
	expect(authors.length).toBeGreaterThan(0);
	expect(authors[0].id).not.toBeNull();
	expect(authors[0].name).not.toBeNull();
});

test('Get tags', async () => {
	const tags = await siStorageClient.facets.getTagsAsync();
	expect(tags).not.toBeNull();
	expect(tags.length).toBeGreaterThan(0);
	expect(tags[0].id).not.toBeNull();
	expect(tags[0].name).not.toBeNull();
});

test('Get languages', async () => {
	const languages = await siStorageClient.facets.getLanguagesAsync();
	expect(languages).not.toBeNull();
	expect(languages.length).toBeGreaterThan(0);
	expect(languages[0].id).not.toBeNull();
	expect(languages[0].code).not.toBeNull();
});

test('Get publishers', async () => {
	const publishers = await siStorageClient.facets.getPublishersAsync();
	expect(publishers).not.toBeNull();
	expect(publishers.length).toBeGreaterThan(0);
	expect(publishers[0].id).not.toBeNull();
	expect(publishers[0].name).not.toBeNull();
});

test('Get restrictions', async () => {
	const restrictions = await siStorageClient.facets.getRestrictionsAsync();
	expect(restrictions).not.toBeNull();
	expect(restrictions.length).toBeGreaterThan(0);
	expect(restrictions[0].id).not.toBeNull();
	expect(restrictions[0].name).not.toBeNull();
	expect(restrictions[0].value).not.toBeNull();
});

test('Get packages', async () => {
	const packages = await siStorageClient.packages.getPackagesAsync({ }, { count: 5 });

	expect(packages).not.toBeNull();
	expect(packages.packages.length).toBe(5);
	expect(packages.total).toBeGreaterThanOrEqual(5);
});

test('Get package', async () => {
	const packages = await siStorageClient.packages.getPackagesAsync({ }, { count: 5 });

	const packageInfo = await siStorageClient.packages.getPackageAsync(packages.packages[0].id);
	expect(packageInfo).not.toBeNull();
});
