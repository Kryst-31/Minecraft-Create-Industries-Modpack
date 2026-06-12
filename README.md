# Minecraft-Create-Industries-Modpack
A custom script for automating the updates of mods, neo-forge versions, and other add-ons between all clients. A custom .sh or .ps1 script will be created to check this GitHub repo for any changes, pull anything needed, and then launch the newly updated software.

## Workflow

- Compiled C# script (.exe) that will install the modpack instance to the user's Prism launcher download directory.
- updater.ps1 (or updater.exe) will be downloaded to the modpack directory and a shortcut will be made.
- The updater shortuct will launch Prism AFTER having checked to ensure that the modpack's mod list, config files, and minecraft version don't need to be updated.
    - If updates are required, then they will be downloaded and extracted from a zip file located in the GitHub Releases section of the modpack.