import { getAsync } from './helpers';
import Author from './models/Author';
import Language from './models/Language';
import Publisher from './models/Publisher';
import Restriction from './models/Restriction';
import Tag from './models/Tag';

/** Provides API for working with facets. */
export default class FacetsApi {
	/**
	 * Initializes a new instance of FacetsApi class.
	 * @param baseUri Base uri to send requests to.
	 */
	constructor(private baseUri: string) {}

	/**
	 * Gets well-known package authors.
	 * @param languageId Language identifier.
	 */
	async getAuthorsAsync(languageId?: number) {
		return getAsync<Author[]>(`${this.baseUri}authors${this.createLanguageParameter(languageId)}`);
	}

	/**
	 * Gets well-known languages.
	 */
	async getLanguagesAsync() {
		return getAsync<Language[]>(`${this.baseUri}languages`);
	}

	/**
	 * Gets well-known package tags.
	 * @param languageId Language identifier.
	 */
	async getTagsAsync(languageId?: number) {
		return getAsync<Tag[]>(`${this.baseUri}tags${this.createLanguageParameter(languageId)}`);
	}

	/**
	 * Gets well-known package publishers.
	 * @param languageId Language identifier.
	 */
	async getPublishersAsync(languageId?: number) {
		return getAsync<Publisher[]>(`${this.baseUri}publishers${this.createLanguageParameter(languageId)}`);
	}

	/**
	 * Gets well-known package restrictions.
	 */
	async getRestrictionsAsync() {
		return getAsync<Restriction[]>(`${this.baseUri}restrictions`);
	}

	private createLanguageParameter(languageId?: number): string {
		return languageId === undefined ? '' : `?languageId=${languageId}`;
	}
}