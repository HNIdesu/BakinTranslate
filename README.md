# BakinTranslate

**BakinTranslate** is a solution designed to enable translation of games developed with [RPG Developer Bakin](https://store.steampowered.com/app/1036640/RPG_Developer_Bakin/).

## How to Use

1. **Unpack game resources**  
   Use [BakinExtractor](https://github.com/HNIdesu/BakinExtractor) to unpack the `data.rbpack` file.

2. **Export translation dictionary**  
   Run the following command using `BakinTranslate.CLI.exe` to generate a dictionary from the game data:

   ```
   BakinTranslate.CLI.exe dump <game_directory> <unpack_directory> [-o output_path]
   ```

   This will create a file named `dic.txt` by default.

3. **Edit translations**  
   The dictionary file consists of key-value pairs, one per line. Each line uses `\t` as the separator:

   ```
   original_text<TAB>translated_text
   ```

   Modify the value to change how the game text is displayed.

4. **Inject the dictionary into the game**  
   Place the `dic.txt` file in the game's `data` directory. **The filename must be `dic.txt`.**

5. **Enable dictionary support by replacing the player executable**  
   Rename the original `bakinplayer.exe` to `bakinplayer.exe.bak`, and replace it with the version provided by this project. This enables dictionary-based translation functionality.

6. **Launch the game**  
   Start the game using the game launcher. Translations should now take effect.


## Troubleshooting
If the game fails to launch or encounters unexpected behavior, please check the error.log file located in the same directory as the game executable. This file may contain useful information to help diagnose and resolve the issue.

If you're unsure how to interpret the log or need further assistance, feel free to open an issue on the GitHub repository with a detailed description and a copy of the error.log content.

## License

This project is licensed under the [MIT License](./LICENSE).