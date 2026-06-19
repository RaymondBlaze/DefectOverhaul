import subprocess
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))

from utils.cli import ok, fail, step
from utils.paths import root_dir

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

    from release.github import release as github_release
    github_release(version, changelog)

    from release.steam import release as steam_release
    steam_release(changelog)


if __name__ == "__main__":
    main()