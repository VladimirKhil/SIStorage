export async function getAsync<T>(uri: string) {
	const response = await fetch(uri);

	if (!response.ok) {
		throw new Error(`Error while retrieving ${uri}: ${response.status} ${await response.text()}`);
	}

	return <T>(await response.json());
}