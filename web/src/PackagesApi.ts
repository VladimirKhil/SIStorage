import { getAsync } from './helpers';
import Package from './models/Package';
import PackageFilters from './models/PackageFilters';
import PackageSelectionParameters from './models/PackageSelectionParameters';
import PackageSortDirection from './models/PackageSortDirection';
import PackageSortMode from './models/PackageSortMode';
import PackagesPage from './models/PackagesPage';
import RandomPackageParameters from './models/RandomPackageParameters';

/** Provides API for working with packages. */
export default class PackagesApi {
	/**
	 * Initializes a new instance of PackagesApi class.
	 * @param baseUri Base uri to send requests to.
	 */
	constructor(private baseUri: string) {}

	/** Gets package by id. */
	async getPackageAsync(packageId: string) {
		return getAsync<Package>(`${this.baseUri}${packageId}`);
	}

	/**
	 * Gets (searches) packages by filters.
	 * @param packageFilters Package filters.
	 * @param packageSelectionParameters Package sort and pagination parameters.
	 */
	async getPackagesAsync(packageFilters: PackageFilters, packageSelectionParameters: PackageSelectionParameters) {
		const query: Record<string, string | number> = {};

		if (packageFilters.tagIds) {
			query["tagIds"] = packageFilters.tagIds.join(",");
		}

		if (packageFilters.difficulty) {
			query["difficulty"] = packageFilters.difficulty.value.toString();
			query["difficultyCompareMode"] = packageFilters.difficulty.compareMode;
		}

		if (packageFilters.publisherId) {
			query["publisherId"] = packageFilters.publisherId.toString();
		}

		if (packageFilters.authorId) {
			query["authorId"] = packageFilters.authorId.toString();
		}

		if (packageFilters.restrictionIds && packageFilters.restrictionIds.length > 0) {
			query["restrictionIds"] = packageFilters.restrictionIds.join(",");
		}

		if (packageFilters.languageId) {
			query["languageId"] = packageFilters.languageId.toString();
		}

		if (packageFilters.searchText) {
			query["searchText"] = encodeURIComponent(packageFilters.searchText);
		}

		query["sortMode"] = packageSelectionParameters.sortMode ?? PackageSortMode.Name;
		query["sortDirection"] = packageSelectionParameters.sortDirection ?? PackageSortDirection.Ascending;
		query["from"] = (packageSelectionParameters.from ?? 0).toString();
		query["count"] = (packageSelectionParameters.count ?? 20).toString();

		const queryArgs = Object.entries(query).map(([key, value]) => `${key}=${value}`).join("&");

		return getAsync<PackagesPage>(`${this.baseUri}?${queryArgs}`);
	}

	/** Gets random package. */
	async getRandomPackageAsync(randomPackageParameters: RandomPackageParameters) {
		const response = await fetch(`${this.baseUri}random`, {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(randomPackageParameters)
		});

		if (!response.ok) {
			throw new Error(`${response.status} ${await response.text()}`);
		}

		const packageJson = await response.json();
		return packageJson as Package;
	}
}