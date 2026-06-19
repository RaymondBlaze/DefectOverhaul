import shutil
import subprocess
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))

from utils.cli import step, ok, fail


def ilspy_decompile(dll_path: Path, output_dir: Path) -> None:
    """Decompile a single DLL to *output_dir* using ilspycmd."""
    step(f"Decompiling {dll_path.name} …")

    if output_dir.exists():
        shutil.rmtree(output_dir)
    output_dir.mkdir(parents=True)

    cmd = ["ilspycmd", "--nested-directories", "-p", "--disable-updatecheck", "-usepdb"]

    cmd.extend(["-o", str(output_dir), str(dll_path)])

    result = subprocess.run(cmd, capture_output=True, text=True)
    if result.returncode:
        if result.stderr.strip():
            print(result.stderr.strip()[:500], file=sys.stderr)
        fail(f"ilspycmd failed on {dll_path.name}")

    if not any(output_dir.rglob("*")):
        fail(f"ilspycmd produced no output for {dll_path.name}")

    ok(f"→ {output_dir}")


def copy_dll_for_mcp(dll_path: Path, output_dir: Path) -> None:
    """Copy the original DLL (and PDB, if available) to
    ``<output_dir parent>/bin/`` for ILSpy MCP tool access."""
    bin_dir = output_dir.parent / "bin"
    bin_dir.mkdir(parents=True, exist_ok=True)

    target = bin_dir / dll_path.name
    shutil.copy2(str(dll_path), str(target))
    ok(f"→ {target}")

    pdb_path = dll_path.with_suffix(".pdb")
    if pdb_path.is_file():
        pdb_target = bin_dir / pdb_path.name
        shutil.copy2(str(pdb_path), str(pdb_target))
        ok(f"→ {pdb_target}")


def main() -> None:
    step("Updating game libs reference")
    from libs.extract_game_libs import main as step_game
    step_game()

    print()

    step("Updating NuGet packages reference")
    from libs.extract_nuget_packages import main as step_nuget
    step_nuget()

    print()

    step("Extracting game PCK")
    from libs.extract_game_pck import main as step_pck
    step_pck()

    print()
    print("Done.")


if __name__ == "__main__":
    main()
