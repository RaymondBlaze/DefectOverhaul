"""Run all lib update steps in sequence: game DLLs → NuGet packages → PCK.

Usage:
    python scripts/libs/update_libs.py
"""

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))

from utils.cli import step


def main() -> None:
    step("Decompiling game libs")
    from libs.decompile_game_libs import main as step_game
    step_game()

    print()

    step("Decompiling NuGet packages")
    from libs.decompile_nuget_packages import main as step_nuget
    step_nuget()

    print()

    step("Extracting game PCK")
    from libs.extract_game_pck import main as step_pck
    step_pck()

    print()
    print("Done.")


if __name__ == "__main__":
    main()
