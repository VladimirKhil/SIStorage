/** Defines a packages sort mode. */
const enum PackageSortMode {
	/** Sort by name. */
	Name = 0,

	/** Sort by created date. */
	CreatedDate = 1,

	/** Sort by download count. */
    DownloadCount = 2,

	/** Sort by rating. */
    Rating = 3,
}

export default PackageSortMode;