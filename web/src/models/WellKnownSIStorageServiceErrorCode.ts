/** Defines well-known SIStorage service error codes. */
const enum WellKnownSIStorageServiceErrorCode {
	/** File is empty. */
	FileEmpty,

	/** File is too large. */
	FileTooLarge,

	/** Multipart ContentType is required. */
	MultipartContentTypeRequired,

	/** Content-Length header is required. */
	ContentLengthHeaderRequired,

	/** Content-Disposition header is required. */
	ContentDispositionHeaderRequired,

	/** Disposition FileName is required. */
	DispositionFileNameRequired,

	/** Package not found. */
	PackageNotFound,
}

export default WellKnownSIStorageServiceErrorCode;