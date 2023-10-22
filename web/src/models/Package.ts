import Round from './Round';

/** Provides SIGame package information. */
export default interface Package {
	/** Package unique global identifier. Should be unique across all package storages. */
	id: string;

	/** Package human-readable name. */
	name?: string;

	/** Package difficulty (from 1 to 10). */
	difficulty?: number;

	/** Package restrictions (age or region, for example).
	 * This is an array of claims that must be satisfied to use this package.
	 * Restrictions with the same name are joined with OR logic.
	 * Restrictions with different names are joined with AND logic. */
	restrictionIds?: number[];

	/** Package publisher identifier. */
	publisherId?: number;

	/** Package create date. You can use a string for date representation in TypeScript. */
	createDate?: Date;

	/** Package language code. */
	languageId?: number;

	/** Package location. */
	contentUri?: string;

	/** Package direct content location (this link usage would not increase download counter). */
	directContentUri?: string;

	/** Package logo location. */
	logoUri?: string;

	/** Package file size in bytes. */
	size?: number;

	/** Package rounds info. */
	rounds?: Round[];

	/** Package total question count. */
	questionCount?: number;

	/** Package content type statistic. Keys are content types; values are the counts of specified content types in the package. */
	contentTypeStatistic?: { [key: string]: number };

	/** Package download count. */
	downloadCount?: number;

	/** Package rating (from 0 to 5). */
	rating?: number;

	/** Storage-specific additional information. */
	extraInfo?: string;
  }