/** Defines random package parameters. */
export default interface RandomPackageParameters {
	/** Package tags. */
	tagIds?: number[];

	/** Package difficulty. */
	difficulty?: number;

	/**
	 * Package restrictions.
	 * Restrictions with the same name are joined with OR logic.
	 * Restrictions with different names are joined with AND logic.
	 */
	restrictionIds?: number[];

	/** Package language. */
	languageId?: number;
}