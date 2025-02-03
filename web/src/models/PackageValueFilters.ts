import NumericFilter from './NumericFilter';

/** Provides packages filtering information. */
export default interface PackageValueFilters {
	/** Publisher filter. */
	publisher?: string;

	/** Author filter. */
	author?: string;

	/** Tags filter. */
	tags?: string[];

	/** Difficulty filter. */
	difficulty?: NumericFilter;

	/** Restriction filter.
	 * Restrictions with the same name are joined with OR logic.
	 * Restrictions with different names are joined with AND logic. */
	restrictions?: { name: string, value: string }[];

	/** Language identifier. */
	language?: string;

	/** Text to find in any of the package fields. */
	searchText?: string;
}