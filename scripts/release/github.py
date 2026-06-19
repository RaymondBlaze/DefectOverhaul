import subprocess
import sys
import tempfile
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))

from utils.cli import ok, fail, step
from utils.paths import release_dir, root_dir


def release(version: str, changelog: str) -> None:
    step("Creating GitHub Release")

    artifact_path = release_dir() / "artifacts" / f"DefectOverhaul-{version}.zip"
    if not artifact_path.is_file():
        fail(f"Artifact not found at {artifact_path}")

    with tempfile.NamedTemporaryFile(
            mode="w", suffix=".md", delete=False, encoding="utf-8"
    ) as f:
        f.write(changelog)
        changelog_path = f.name

    try:
        result = subprocess.run(
            ["gh", "release", "create", version, str(artifact_path),
             "--notes-file", changelog_path],
            capture_output=True, text=True,
        )
        if result.returncode:
            msg = (result.stderr or result.stdout or "").strip()[:500]
            fail(f"Failed to create GitHub Release: {msg}")

        ok(f"Successfully created GitHub Release")
    finally:
        Path(changelog_path).unlink(missing_ok=True)

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

    release(version, changelog)
        
if __name__ == '__main__':
    main()