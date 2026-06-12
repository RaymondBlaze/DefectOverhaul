"""
create_cards_md.py

Generate CARDS.md for each language with card images and localized names.

Usage:
    python scripts/docs/create_cards_md.py
"""

import json
import os
import re
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))

from utils.paths import libs_dir, root_dir
from utils.cli import step, ok

CARDS_IMAGES_DIR = root_dir() / "images" / "cards"
LOCALE_DIR = libs_dir() / "sts2" / "resources" / "localization"
OUTPUT_DIR = root_dir() / "docs"

LANGS = ["eng", "zhs"]

TITLES = {
    "eng": "Defect Overhaul - Cards",
    "zhs": "故障机器人重置 - 卡牌",
}


def load_card_names(lang):
    """Load {card_id -> title} from a cards.json for the given language."""
    path = LOCALE_DIR / lang / "cards.json"
    if not path.exists():
        return {}
    data = json.loads(path.read_text(encoding="utf-8"))
    names = {}
    for key, value in data.items():
        m = re.match(r"^(\w+)\.title$", key)
        if m:
            names[m.group(1)] = value
    return names


def collect_card_ids(lang):
    """Collect card IDs from image filenames for the given language."""
    pattern = re.compile(r"^(\w+)_(base|upgraded)\.png$")
    image_dir = CARDS_IMAGES_DIR / lang
    if not image_dir.exists():
        return []
    ids = set()
    for filename in os.listdir(image_dir):
        m = pattern.match(filename)
        if m:
            ids.add(m.group(1))
    return sorted(ids)


def generate_one(lang, names, card_ids):
    """Generate a language-specific CARDS.md."""
    lines = [f"# {TITLES[lang]}", ""]
    image_prefix = f"images/cards/{lang}"

    for cid in card_ids:
        title = names.get(cid, cid)
        lines.append(f"## {title}")
        lines.append("")

        base_img = f"{image_prefix}/{cid}_base.png"
        upgraded_img = f"{image_prefix}/{cid}_upgraded.png"

        lines.append("<table><tr>")
        lines.append(f'  <td><img src="../../{base_img}" alt="base" width="100%"></td>')
        lines.append(f'  <td><img src="../../{upgraded_img}" alt="upgraded" width="100%"></td>')
        lines.append("</tr></table>")
        lines.append("")

    output_path = OUTPUT_DIR / lang / "CARDS.md"
    output_path.write_text("\n".join(lines), encoding="utf-8")
    return output_path, len(card_ids)


def main():
    for lang in LANGS:
        step(f"[{lang}] Loading card names")
        names = load_card_names(lang)
        ok(f"{len(names)} names")

        step(f"[{lang}] Collecting card images from {lang}/")
        card_ids = collect_card_ids(lang)
        ok(f"{len(card_ids)} cards")

        step(f"[{lang}] Generating CARDS.md")
        output_path, count = generate_one(lang, names, card_ids)
        ok(f"Written {count} cards to {output_path}")

    ok("All done")


if __name__ == "__main__":
    main()
