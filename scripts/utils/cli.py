"""Uniform CLI output helpers for all project scripts."""

import subprocess
import sys
from typing import Optional


def step(msg: str) -> None:
    print(f"▶ {msg}")


def ok(msg: str) -> None:
    print(f"  ✓ {msg}")


def fail(msg: str) -> None:
    print(f"  ✗ {msg}", file=sys.stderr)
    sys.exit(1)


def warn(msg: str) -> None:
    print(f"  ⚠ {msg}")


def run(cmd: list[str], cwd: Optional[str] = None) -> None:
    label = " ".join(cmd[:3])
    if len(cmd) > 3:
        label += "…"
    result = subprocess.run(cmd, capture_output=True, text=True, cwd=cwd)
    if result.returncode:
        if result.stderr.strip():
            print(result.stderr.strip()[:500], file=sys.stderr)
        fail(f"{label} exited with code {result.returncode}")
    ok(label)
