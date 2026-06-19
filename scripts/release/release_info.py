import json
import re
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))

from utils.cli import fail
from utils.paths import release_dir, changelog_path


def resolve_version() -> str:
    manifest = release_dir() / "content" / "DefectOverhaul.json"
    if not manifest.is_file():
        fail(f"Build manifest not found at {manifest} — build may have failed")
    with open(manifest, encoding="utf-8") as f:
        data = json.load(f)
    return data["version"]


def resolve_changelog(version: str) -> str:
    path = changelog_path()

    if not path.is_file():
        raise FileNotFoundError(f"CHANGELOG.md not found at {path}")

    text = path.read_text(encoding="utf-8")

    pattern = re.compile(rf"^##\s+{re.escape(version)}\s*$", re.MULTILINE)
    match = pattern.search(text)
    if not match:
        raise ValueError(
            f"Version '{version}' not found in CHANGELOG.md "
            f"(looking for '## {version}')"
        )

    start = match.end()
    next_match = re.search(r"^##\s", text[start:], re.MULTILINE)
    end = start + next_match.start() if next_match else len(text)

    return text[start:end].strip()
