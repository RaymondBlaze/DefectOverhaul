"""Extract a Godot .pck file to a directory.

Standalone usage:
    python scripts/utils/extract_pck.py <pck_path> <output_dir>

Called from other scripts via ``extract_pck(pck_path, output_dir)``.
"""

import shutil
import subprocess
import sys
import urllib.request
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))

from utils.cli import step, ok, fail
from utils.paths import scripts_tools_dir

PCKTOOL_DIR = scripts_tools_dir() / "godotpcktool"
PCKTOOL_PATH = PCKTOOL_DIR / "godotpcktool.exe"
PCKTOOL_URL = (
    "https://github.com/hhyyrylainen/GodotPckTool/releases/download/v2.2/godotpcktool.exe"
)


def _ensure_tool() -> Path:
    if PCKTOOL_PATH.is_file():
        return PCKTOOL_PATH
    step("Downloading godotpcktool …")
    PCKTOOL_DIR.mkdir(parents=True, exist_ok=True)
    req = urllib.request.Request(PCKTOOL_URL, headers={"User-Agent": "sts2-mod-tools"})
    try:
        with urllib.request.urlopen(req, timeout=30) as resp:
            PCKTOOL_PATH.write_bytes(resp.read())
    except Exception as e:
        fail(f"Failed to download godotpcktool: {e}")
    if not PCKTOOL_PATH.is_file():
        fail("Download appeared to succeed but godotpcktool.exe is missing")
    ok(f"Downloaded godotpcktool → {PCKTOOL_PATH}")
    return PCKTOOL_PATH


def extract_pck(pck_path: str | Path, output_dir: str | Path) -> None:
    pck = Path(pck_path)
    if not pck.is_file():
        fail(f"PCK not found: {pck}")

    out = Path(output_dir)
    if out.exists():
        shutil.rmtree(out)
    out.mkdir(parents=True)

    tool = _ensure_tool()
    step(f"Extracting {pck.name} …")
    result = subprocess.run(
        [str(tool), str(pck), "-a", "e", "-o", str(out)],
        capture_output=True, text=True,
    )
    if result.returncode:
        print(result.stderr.strip()[:500], file=sys.stderr)
        fail(f"godotpcktool exited with code {result.returncode}")
    ok(f"→ {out}")


if __name__ == "__main__":
    import argparse

    parser = argparse.ArgumentParser(description="Extract Godot PCK file")
    parser.add_argument("pck", help="Path to .pck file")
    parser.add_argument("output", help="Output directory")
    args = parser.parse_args()
    extract_pck(args.pck, args.output)
