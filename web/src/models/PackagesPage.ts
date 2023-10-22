import Package from './Package';

/** Defines packages result page. */
export default interface PackagesPage {
	/** Collection of returned packages. */
	packages: Package[];

	/** Total number of packages. Used for requesting the next page. */
	total: number;
}