import NumericFilter from './NumericFilter';

/** Provides packages filtering information. */
export default interface PackageFilters {
	/** Publisher filter. */
	publisherId?: number;

	/** Author filter. */
	authorId?: number;

	/** Tags filter. */
	tagIds?: number[];

	/** Difficulty filter. */
	difficulty?: NumericFilter;

	/** Restriction filter.
	 * Restrictions with the same name are joined with OR logic.
	 * Restrictions with different names are joined with AND logic. */
	restrictionIds?: number[];

	/** Language identifier. */
	languageId?: number;

	/** Text to find in any of the package fields. */
	searchText?: string;
}