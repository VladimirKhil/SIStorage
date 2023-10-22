import CompareMode from './CompareMode';

/** Provides an numeric value filter. */
export default interface NumericFilter {
	/** Value compare mode. */
	compareMode: CompareMode;

	/** Value to compare. */
	value: number;
}