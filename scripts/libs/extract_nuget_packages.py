import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))

from utils.cli import step, ok, fail
from utils.paths import root_dir, libs_dir, scripts_configs_dir
from libs.update_libs import ilspy_decompile, copy_dll_for_mcp


def resolve_nuget_dlls(package_id: str) -> list[Path]:
    project_root = root_dir()
    for cfg in ("Debug", "Release"):
        assets_path = project_root / "build" / "obj" / cfg / "project.assets.json"
        if not assets_path.is_file():
            continue
        data = json.loads(assets_path.read_text(encoding="utf-8"))

        cache_roots = list(data.get("packageFolders", {}).keys())
        if not cache_roots:
            continue

        pkg_prefix = package_id + "/"
        for key, lib_entry in data.get("libraries", {}).items():
            if not key.startswith(pkg_prefix):
                continue
            if lib_entry.get("type") != "package":
                continue

            pkg_root = Path(cache_roots[0]) / lib_entry["path"]
            targets = data.get("targets", {})
            framework = next(iter(targets), "")
            compile_assets = targets.get(framework, {}).get(key, {}).get("compile", {})
            dlls = [pkg_root / f for f in compile_assets if f.endswith(".dll")]
            if dlls:
                return dlls
            fail(f"No compile-time DLL found for {package_id}")

    fail(f"Could not resolve DLLs for {package_id} (run dotnet restore first?)")


def main() -> None:
    config_path = scripts_configs_dir() / "update_libs.json"
    if not config_path.is_file():
        fail(f"{config_path} not found")
    config = json.loads(config_path.read_text(encoding="utf-8"))

    packages = config.get("nuget_packages", [])
    if not packages:
        step("No NuGet packages configured, skipping")
        return

    step("Running dotnet restore to update project.assets.json …")
    import subprocess
    result = subprocess.run(
        ["dotnet", "restore"],
        capture_output=True, text=True,
        cwd=str(root_dir() / "godot"),
    )
    if result.returncode:
        print(result.stderr.strip()[:500], file=sys.stderr)
        fail("dotnet restore failed")
    ok("dotnet restore OK")

    for pkg in packages:
        dlls = resolve_nuget_dlls(pkg)
        for dll in dlls:
            ilspy_decompile(dll, libs_dir() / dll.stem / "src")
            copy_dll_for_mcp(dll, libs_dir() / dll.stem / "src")


if __name__ == "__main__":
    main()
