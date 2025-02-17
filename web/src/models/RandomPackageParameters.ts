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

	/** Round count. */
	roundCount?: number;

	/** Table round theme count. */
	tableThemeCount?: number;

	/** Theme list round theme count. */
	themeListThemeCount?: number;

	/** Base question price. */
	baseQuestionPrice?: number;
}