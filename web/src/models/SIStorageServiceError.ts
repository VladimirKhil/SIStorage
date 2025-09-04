import WellKnownSIStorageServiceErrorCode from "./WellKnownSIStorageServiceErrorCode";

/** Defines a SIStorageService error. */
export default class SIStorageServiceError extends Error {
    /** Error code. */
    errorCode?: WellKnownSIStorageServiceErrorCode;

    /** Error status code. */
    statusCode: number;

    constructor(message: string | undefined, statusCode: number, errorCode?: WellKnownSIStorageServiceErrorCode) {
        super(message);
        this.statusCode = statusCode;
        this.errorCode = errorCode;
    }
}