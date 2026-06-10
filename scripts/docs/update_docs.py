"""
update_docs.py

Run all doc update scripts in sequence.

Usage:
    python scripts/docs/update_docs.py
"""

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))

from utils.cli import step


def main() -> None:
    step("RitsuLib docs")
    from docs.update_ritsulib_docs import main as ritsu
    ritsu()

    print()

    step("Modding tutorials")
    from docs.update_modding_tutorials import main as tutorial
    tutorial()


if __name__ == "__main__":
    main()
