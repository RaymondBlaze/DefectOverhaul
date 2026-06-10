"""Shared decompile helpers for libs/ scripts."""

import shutil
import subprocess
import sys
from pathlib import Path

from utils.cli import step, ok, fail


def ilspy_decompile(dll_path: Path, output_dir: Path) -> None:
    """Decompile a single DLL to *output_dir* using ilspycmd."""
    step(f"Decompiling {dll_path.name} …")

    if output_dir.exists():
        shutil.rmtree(output_dir)
    output_dir.mkdir(parents=True)

    cmd = ["ilspycmd", "--nested-directories", "-p", "--disable-updatecheck"]

    pdb_path = dll_path.with_suffix(".pdb")
    if pdb_path.is_file():
        cmd.append("-usepdb")

    cmd.extend(["-o", str(output_dir), str(dll_path)])

    result = subprocess.run(cmd, capture_output=True, text=True)
    if result.returncode:
        if result.stderr.strip():
            print(result.stderr.strip()[:500], file=sys.stderr)
        fail(f"ilspycmd failed on {dll_path.name}")

    if not any(output_dir.rglob("*")):
        fail(f"ilspycmd produced no output for {dll_path.name}")

    ok(f"→ {output_dir}")
