import WellKnownSIStorageServiceErrorCode from "./WellKnownSIStorageServiceErrorCode";

/** Defines an internal error. */
export default interface InternalError {
    /** Error code. */
    errorCode: WellKnownSIStorageServiceErrorCode;
}