import os
from tkinter import filedialog


def config_unity_merge_tool(merge_tool_path):
    if merge_tool_path is None:
        return

    command = f'"{merge_tool_path}" merge -p "$BASE" "$REMOTE" "$LOCAL" "$MERGED"'.replace('"', '\\\"')
    os.system(f'git config mergetool.unityyamlmerge.cmd "{command}"')
    os.system(f'git config mergetool.unityyamlmerge.trustExitCode false')


if __name__ == '__main__':
    file_path = filedialog.askopenfilename(
        title='选择Program Files\\Unity\\<版本>\\Editor\\Data\\Tools\\UnityYAMLMerge.exe',
        filetypes=[('UnityYAMLMerge.exe', 'UnityYAMLMerge.exe')])

    config_unity_merge_tool(file_path)
