# HttpClient Exception Repro
Project to reproduce an exception in HttpClient when uploading data that exceeds the server's max allowed.  The server returns an HTTP status code of 413, but HttpClient raises an exception.
