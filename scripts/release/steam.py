import json
import shutil
import subprocess
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))

from utils.cli import ok, fail, step
from utils.paths import root_dir, release_dir, workshop_dir, scripts_tools_dir


def release(changelog: str) -> None:
    step("Building Steam Workshop workspace")

    template_config_path = workshop_dir() / "workshop.json"
    workspace_config_path = release_dir() / "workshop.json"
    if not template_config_path.is_file():
        fail(f"Missing workshop.json at {template_config_path}")

    template_image_path = workshop_dir() / "image.png"
    workspace_image_path = release_dir() / "image.png"
    if not template_image_path.is_file():
        fail(f"Missing image.png at {template_image_path}")

    shutil.copy2(template_config_path, workspace_config_path)
    shutil.copy2(template_image_path, workspace_image_path)

    with open(workspace_config_path, encoding="utf-8") as f:
        config = json.load(f)

    try:
        config["changeNote"] = changelog
    except ValueError:
        pass

    with open(workspace_config_path, "w", encoding="utf-8") as f:
        json.dump(config, f, indent=2, ensure_ascii=False)
        f.write("\n")

    ok("Successfully built Steam Workshop workspace")

    step("Uploading to Steam Workshop")
    uploader = scripts_tools_dir() / "ModUploader-win-x64" / "ModUploader.exe"
    if not uploader.is_file():
        fail(f"ModUploader.exe not found at {uploader}")

    result = subprocess.run(
        [str(uploader), "upload", "-w", str(release_dir())],
        capture_output=True, text=True,
    )

    if result.returncode:
        msg = (result.stderr or result.stdout or "").strip()[:500]
        fail(f"Failed to upload to Steam Workshop: {msg}")

    ok("Successfully uploaded to Steam Workshop")


def main():
    step("Building mod")
    csproj = root_dir() / "godot" / "DefectOverhaul.csproj"
    result = subprocess.run(
        ["dotnet", "build", str(csproj)],
        capture_output=True, text=True,
    )
    if result.returncode:
        print(result.stderr or result.stdout, file=sys.stderr)
        fail("Build failed")
    ok("Build succeeded")


    from release.release_info import resolve_version, resolve_changelog

    version = resolve_version()
    changelog = resolve_changelog(version)

    release(changelog)


if __name__ == '__main__':
    main()
