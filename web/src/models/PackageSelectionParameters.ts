import PackageSortDirection from './PackageSortDirection';
import PackageSortMode from './PackageSortMode';

/** Provides packages selection parameters. */
export default interface PackageSelectionParameters {
	/** Results sort mode. */
	sortMode?: PackageSortMode;

	/** Results sort direction. */
	sortDirection?: PackageSortDirection;

	/** "From" pagination parameter. */
	from?: number;

	/** "Count" pagination parameter. */
	count?: number;
}