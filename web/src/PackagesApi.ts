import { getAsync } from './helpers';
import InternalError from './models/InternalError';
import Package from './models/Package';
import PackageFilters from './models/PackageFilters';
import PackageSelectionParameters from './models/PackageSelectionParameters';
import PackageSortDirection from './models/PackageSortDirection';
import PackageSortMode from './models/PackageSortMode';
import PackagesPage from './models/PackagesPage';
import PackageValueFilters from './models/PackageValueFilters';
import RandomPackageParameters from './models/RandomPackageParameters';
import SIStorageServiceError from './models/SIStorageServiceError';
import WellKnownSIStorageServiceErrorCode from './models/WellKnownSIStorageServiceErrorCode';

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

	async getPackagesByValueFiltersAsync(packageFilters: PackageValueFilters, packageSelectionParameters: PackageSelectionParameters) {
		const query: Record<string, string | number> = {};

		if (packageFilters.tags) {
			query["tags"] = encodeURIComponent(packageFilters.tags.join(","));
		}

		if (packageFilters.difficulty) {
			query["difficulty"] = packageFilters.difficulty.value.toString();
			query["difficultyCompareMode"] = packageFilters.difficulty.compareMode;
		}

		if (packageFilters.publisher) {
			query["publisher"] = encodeURIComponent(packageFilters.publisher);
		}

		if (packageFilters.author) {
			query["author"] = encodeURIComponent(packageFilters.author);
		}

		if (packageFilters.restrictions && packageFilters.restrictions.length > 0) {
			query["restrictions"] = encodeURIComponent(packageFilters.restrictions.map(r => `${r.name}:${r.value}`).join(","));
		}

		if (packageFilters.language) {
			query["language"] = packageFilters.language;
		}

		if (packageFilters.searchText) {
			query["searchText"] = encodeURIComponent(packageFilters.searchText);
		}

		query["sortMode"] = packageSelectionParameters.sortMode ?? PackageSortMode.Name;
		query["sortDirection"] = packageSelectionParameters.sortDirection ?? PackageSortDirection.Ascending;
		query["from"] = (packageSelectionParameters.from ?? 0).toString();
		query["count"] = (packageSelectionParameters.count ?? 20).toString();

		const queryArgs = Object.entries(query).map(([key, value]) => `${key}=${value}`).join("&");

		return getAsync<PackagesPage>(`${this.baseUri}search?${queryArgs}`);
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
			const errorBody = await response.text();
			const errorCode = tryGetErrorCode(errorBody);

			throw new SIStorageServiceError(errorBody, response.status, errorCode);
		}

		const packageJson = await response.json();
		return packageJson as Package;
	}
}

function tryGetErrorCode(errorBody: string) {
	let errorCode: WellKnownSIStorageServiceErrorCode | undefined;

	try {
		const error = JSON.parse(errorBody) as InternalError;
		errorCode = error?.errorCode;
	} catch { /** Do nothing */ }

	return errorCode;
}