import FacetsApi from './FacetsApi';
import PackagesApi from './PackagesApi';
import SIStorageClientOptions from './SIStorageClientOptions';

/** Defines SIStorage service client. */
export default class SIStorageClient {
	/** API for working with facets. */
	public facets: FacetsApi;

	/** API for working with packages. */
	public packages: PackagesApi;

	/**
	 * Initializes a new instance of SIStorageClient class.
	 * @param options Client options.
	 */
	constructor(public options: SIStorageClientOptions) {
		this.facets = new FacetsApi(`${options.serviceUri}/api/v1/facets/`);
		this.packages = new PackagesApi(`${options.serviceUri}/api/v1/packages/`);
	}
}