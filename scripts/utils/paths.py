import xml.etree.ElementTree as ET
from functools import cache
from pathlib import Path

SCRIPTS_DIR = Path(__file__).resolve().parent.parent


@cache
def scripts_dir() -> Path:
    return SCRIPTS_DIR


@cache
def root_dir() -> Path:
    return scripts_dir().parent


@cache
def scripts_configs_dir() -> Path:
    return scripts_dir() / "configs"


@cache
def scripts_tools_dir() -> Path:
    return scripts_dir() / "tools"


@cache
def docs_dir() -> Path:
    return root_dir() / "docs"


@cache
def libs_dir() -> Path:
    return root_dir() / "libs"


@cache
def build_dir() -> Path:
    return root_dir() / "build"

@cache
def release_dir() -> Path:
    return build_dir() / "release"


@cache
def workshop_dir() -> Path:
    return root_dir() / "workshop"


@cache
def changelog_path() -> Path:
    return root_dir() / "CHANGELOG.md"



LOCAL_PROPS_PATH = root_dir() / "godot" / "local.props"


@cache
def _local_props_value(tag: str) -> str | None:
    if not LOCAL_PROPS_PATH.is_file():
        return None
    tree = ET.parse(str(LOCAL_PROPS_PATH))
    pg = tree.getroot().find("PropertyGroup")
    if pg is None:
        return None
    el = pg.find(tag)
    if el is None or not el.text:
        return None
    return el.text.strip()


@cache
def sts2_dir() -> Path | None:
    raw = _local_props_value("Sts2Dir")
    return Path(raw) if raw else None


@cache
def sts2_data_dir() -> Path | None:
    raw = _local_props_value("Sts2DataDir")
    if raw is None:
        return None
    s2 = sts2_dir()
    if s2 is not None:
        raw = raw.replace("$(Sts2Dir)", str(s2))
    return Path(raw)
