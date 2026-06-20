package main

import (
	"archive/zip"
	"encoding/json"
	"errors"
	"fmt"
	"io"
	"net/http"
	"os"
	"path/filepath"
	"strings"
	"time"
)

func httpGetHelper(url string) []byte {
	httpClient := http.Client{
		Timeout: time.Second * 30,
	}

	req, err := http.NewRequest("GET", url, nil)
	if err != nil {
		panic(err)
	}

	resp, err := httpClient.Do(req)
	if err != nil {
		panic(err)
	}

	defer resp.Body.Close()
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		panic(err)
	}

	if resp.StatusCode < 200 || resp.StatusCode >= 300 {
		// show response body for debugging
		panic(fmt.Errorf("HTTP %d: %s", resp.StatusCode, string(body)))
	}

	return body
}

func handlePanic(err error) {
	if err != nil {
		panic(err)
	}
}

func main() {
	// --- COMMONLY USED VARIABLES ---
	manifestPath := "./github-manifest.json"
	repoManifestUrl := "https://raw.githubusercontent.com/Kryst-31/Minecraft-Create-Industries-Modpack/refs/heads/main/manifest.json"
	latestReleaseUrl := "https://api.github.com/repos/Kryst-31/Minecraft-Create-Industries-Modpack/releases/latest"
	modsBackupDirectory := "./mods-backup"

	// Change directory to the location of the executable
	executablePath, err := os.Executable()
	handlePanic(err)
	exPath := filepath.Dir(executablePath)

	err = os.Chdir(exPath)
	handlePanic(err)

	// Check if there is a manifest file in the current directory
	if _, err := os.Stat(manifestPath); errors.Is(err, os.ErrNotExist) {
		// If not, create one from the GitHub repository (each attribute is set to NULL be default)
		manifestBytes := httpGetHelper(repoManifestUrl)

		// Create manifest file
		os.WriteFile(manifestPath, manifestBytes, 0644)
	}

	// Get the latest release information on the GitHub page
	releaseBytes := httpGetHelper(latestReleaseUrl)

	var releaseJson map[string]any

	err = json.Unmarshal(releaseBytes, &releaseJson)
	handlePanic(err)

	// Gather the release version name
	modpackVersion := releaseJson["tag_name"]

	// Get the current modpack version listed in the local manifest
	localManifest, err := os.ReadFile(manifestPath)
	handlePanic(err)

	var manifestJson map[string]any

	err = json.Unmarshal(localManifest, &manifestJson)
	handlePanic(err)

	localVersion, _ := manifestJson["modpackVersionRelease"].(string)
	modpackVersionStr, _ := modpackVersion.(string)

	// If the local version and latest release versions are different, update mods list to the latest directory
	if modpackVersionStr != localVersion {
		// The GitHub API returns `assets` as an array of objects. Each element
		// is unmarshaled as map[string]interface{} when using a generic
		// `map[string]any` for the whole response. Assert the slice and then
		// cast each element to a map to access `name` and `browser_download_url`.
		assetsSlice, ok := releaseJson["assets"].([]interface{})
		if !ok {
			panic("assets is not an array in GitHub release JSON")
		}

		// Gather the mods.zip asset in the latest release
		for _, a := range assetsSlice {
			assetMap, ok := a.(map[string]interface{})
			if !ok {
				continue
			}

			assetName, _ := assetMap["name"].(string)

			if assetName == "mods.zip" {
				// `browser_download_url` is the direct download link for release assets
				assetUrl, _ := assetMap["browser_download_url"].(string)

				modsZipBytes := httpGetHelper(assetUrl)

				// save mods.zip bytes to local zip file
				os.WriteFile(assetName, modsZipBytes, 0644)

				// if mods-backup currently exists, delete it and all contents
				if _, err := os.Stat(modsBackupDirectory); err == nil {
					err = os.RemoveAll(modsBackupDirectory)
					handlePanic(err)
				}

				// if mods directory currently exists, change the directory name to mods-backup
				if _, err := os.Stat("./mods"); err == nil {
					err := os.Rename("./mods", modsBackupDirectory)
					handlePanic(err)
				}

				// create the mods directory (since the old one was renamed)
				err = os.Mkdir("./mods", 0755)
				handlePanic(err)

				// unzip new mods.zip contents to mods directory
				r, err := zip.OpenReader(assetName)
				handlePanic(err)
				defer r.Close()

				for _, f := range r.File {
					if f.Name == "mods/" {
						continue
					}

					prefix := strings.TrimSpace(f.Name[:5])

					var modPath string

					if prefix == "mods/" {
						modPath = f.Name
					} else {
						modPath = "mods/" + f.Name
					}

					modFileReader, err := f.Open()
					handlePanic(err)

					modFile, err := io.ReadAll(modFileReader)
					handlePanic(err)

					modFileReader.Close()

					err = os.WriteFile(modPath, modFile, 0644)
				}

				r.Close()

				// clean up mods.zip file when done processing
				err = os.Remove("./mods.zip")
				handlePanic(err)
			}

		}

		// Update local manifest to reflect the new release version
		manifestJson["modpackVersionRelease"] = modpackVersionStr

		newManifest, err := json.MarshalIndent(manifestJson, "", "    ")
		handlePanic(err)

		err = os.WriteFile(manifestPath, newManifest, 0644)

		fmt.Println("Updated modlist to " + modpackVersionStr)
	} else {
		fmt.Println("No new version available.")
	}

}
