"""Extract game PCK to libs/sts2/resources.

Usage:
    python scripts/libs/extract_game_pck.py
"""

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))

from utils.cli import step, fail
from utils.paths import sts2_dir, libs_dir, scripts_configs_dir
from utils.extract_pck import extract_pck
import json


def main() -> None:
    s2_dir = sts2_dir()
    if s2_dir is None:
        fail("Sts2Dir not set in godot/local.props")

    config_path = scripts_configs_dir() / "update_libs.json"
    if not config_path.is_file():
        fail(f"{config_path} not found")
    config = json.loads(config_path.read_text(encoding="utf-8"))

    if not config.get("extract_game_pck", False):
        step("PCK extraction disabled in config, skipping")
        return

    pck_path = s2_dir / "SlayTheSpire2.pck"
    if not pck_path.is_file():
        fail(f"Game PCK not found at {pck_path}")

    out = libs_dir() / "sts2" / "resources"
    extract_pck(str(pck_path), str(out))


if __name__ == "__main__":
    main()
