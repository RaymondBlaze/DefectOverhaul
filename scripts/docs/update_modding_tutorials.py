"""
update_modding_tutorials.py

Download modding tutorials from GitHub to docs/tutorials/.

Usage:
    python scripts/docs/update_modding_tutorials.py
"""

import shutil
import sys
import tempfile
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))

from utils.cli import step, ok, run
from utils.paths import docs_dir

REPO_URL = "https://github.com/GlitchedReme/SlayTheSpire2ModdingTutorials.git"
OUTPUT_DIR = docs_dir() / "SlayTheSpire2ModdingTutorials"


def main() -> int:
    tmp = Path(tempfile.mkdtemp(prefix="sts2-tutorials-"))
    step("Cloning modding tutorials …")
    run(["git", "clone", "--depth", "1", REPO_URL, str(tmp)])

    shutil.rmtree(tmp / ".git", ignore_errors=True)

    if OUTPUT_DIR.exists():
        shutil.rmtree(OUTPUT_DIR)
    shutil.copytree(tmp, OUTPUT_DIR)

    count = len(list(OUTPUT_DIR.rglob("*")))
    ok(f"{count} files → {OUTPUT_DIR}")
    return count


if __name__ == "__main__":
    main()
