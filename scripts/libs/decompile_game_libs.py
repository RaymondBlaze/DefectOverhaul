"""Decompile game DLLs (sts2, 0Harmony, etc.) to libs/<name>/src.

Usage:
    python scripts/libs/decompile_game_libs.py
"""

import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))

from utils.cli import step, fail
from utils.paths import sts2_data_dir, libs_dir, scripts_configs_dir
from libs._decompile import ilspy_decompile


def main() -> None:
    data_dir = sts2_data_dir()
    if data_dir is None:
        fail("Sts2DataDir not set in godot/local.props")

    config_path = scripts_configs_dir() / "update_libs.json"
    if not config_path.is_file():
        fail(f"{config_path} not found")
    config = json.loads(config_path.read_text(encoding="utf-8"))

    names = config.get("game_dlls", [])
    if not names:
        step("No game DLLs configured, skipping")
        return

    for name in names:
        dll = data_dir / f"{name}.dll"
        if not dll.is_file():
            fail(f"Game DLL {name}.dll not found at {dll}")
        ilspy_decompile(dll, libs_dir() / name / "src")


if __name__ == "__main__":
    main()
