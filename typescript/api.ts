class ApiClient {
    async getRom(filename: string) : Promise<ArrayBuffer> {
        let rom = await window.fetch(filename)
            .then(response => response.arrayBuffer());
        return rom;
    }

}