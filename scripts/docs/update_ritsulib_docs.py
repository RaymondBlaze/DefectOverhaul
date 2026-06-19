import re
import sys
import tempfile
from pathlib import Path
from typing import Optional

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))

from utils.cli import step, ok, run
from utils.paths import docs_dir

REPO_URL = "https://github.com/BAKAOLC/STS2-RitsuLib.git"
DOCS_SUBDIR = "docs"


# fetch

def find_pages(source: Path) -> Path | None:
    candidate = source / "pages"
    if candidate.is_dir():
        return candidate
    candidate = source / DOCS_SUBDIR / "pages"
    if candidate.is_dir():
        return candidate
    return None


def fetch_pages() -> Path:
    tmp = Path(tempfile.mkdtemp(prefix="ritsulib-docs-"))
    step("Cloning RitsuLib docs …")
    run(["git", "clone", "--depth", "1", "--filter=blob:none", "--sparse",
         REPO_URL, str(tmp)])
    run(["git", "sparse-checkout", "set", DOCS_SUBDIR], cwd=str(tmp))
    run(["git", "checkout"], cwd=str(tmp))
    pages = find_pages(tmp / DOCS_SUBDIR)
    assert pages and pages.is_dir(), "pages/ not found after clone"
    return pages


# frontmatter

def parse_frontmatter(content: str) -> tuple[dict, str]:
    fm: dict = {}
    m = re.match(r'^---\s*\n(.*?)\n(?:\.\.\.|---)\s*\n', content, re.DOTALL)
    if not m:
        return fm, content

    body = content[m.end():]
    yaml_block = m.group(1)
    current_key: Optional[str] = None

    for line in yaml_block.split("\n"):
        m2 = re.match(r'^(\w[\w_-]*)\s*:\s*(.*)', line)
        if m2:
            current_key = m2.group(1)
            value = m2.group(2).strip()
            if value.startswith("'") and value.endswith("'"):
                value = value[1:-1]
            elif value.startswith('"') and value.endswith('"'):
                value = value[1:-1]
            fm[current_key] = value if value else {}
            continue

        m3 = re.match(r'^\s{2,}(\w[\w_-]*)\s*:\s*(.*)', line)
        if m3 and current_key is not None and isinstance(fm.get(current_key), dict):
            sub_val = m3.group(2).strip()
            if sub_val.startswith("'") and sub_val.endswith("'"):
                sub_val = sub_val[1:-1]
            elif sub_val.startswith('"') and sub_val.endswith('"'):
                sub_val = sub_val[1:-1]
            fm[current_key][m3.group(1)] = sub_val

    return fm, body


def get_title(fm: dict, lang: str) -> str:
    raw = fm.get("title")
    if raw is None:
        return ""
    if isinstance(raw, str):
        return raw
    if isinstance(raw, dict):
        for candidate in (lang, "en"):
            if candidate in raw and raw[candidate]:
                return raw[candidate]
        for v in raw.values():
            if v:
                return v
    return ""


# extract

def extract_sections(
    body: str, target_lang: str,
) -> tuple[list[tuple[int, str, str]], str]:
    sections: list[dict] = []
    in_code_block = False

    lines = body.split("\n")
    i = 0
    heading_re = re.compile(r'^(#{1,6})\s+(.+?)\{lang="([^"]+)"}\s*$')
    fence_open = re.compile(r'^```')
    block_open = re.compile(r'^:::\s+(\S+)\s*$')
    block_close = re.compile(r'^:::$')

    while i < len(lines):
        line = lines[i]

        if fence_open.match(line):
            in_code_block = not in_code_block
            i += 1
            continue
        if in_code_block:
            i += 1
            continue

        hm = heading_re.match(line)
        if hm:
            level = len(hm.group(1))
            text = hm.group(2).strip()
            sec_lang = hm.group(3)
            start = i

            i += 1
            while i < len(lines):
                bl = block_open.match(lines[i])
                if bl and bl.group(1) == sec_lang:
                    break
                i += 1

            i += 1
            content_lines: list[str] = []
            while i < len(lines):
                if fence_open.match(lines[i]):
                    in_code_block = not in_code_block
                    content_lines.append(lines[i])
                    i += 1
                    continue
                if not in_code_block and block_close.match(lines[i]):
                    break
                content_lines.append(lines[i])
                i += 1

            if i < len(lines):
                i += 1
            end = i

            while content_lines and content_lines[-1].strip() == "":
                content_lines.pop()
            first_nonblank = 0
            while first_nonblank < len(content_lines) and content_lines[first_nonblank].strip() == "":
                first_nonblank += 1
            content_lines = content_lines[first_nonblank:]

            sections.append({
                "level": level, "text": text, "lang": sec_lang,
                "content": "\n".join(content_lines),
                "start_line": start, "end_line": end,
            })
            continue

        i += 1

    section_ranges = [(s["start_line"], s["end_line"]) for s in sections]
    shared_lines = [
        line for i, line in enumerate(lines)
        if not any(start <= i < end for start, end in section_ranges)
    ]
    shared_content = "\n".join(shared_lines).strip()

    target_sections = [
        (s["level"], s["text"], s["content"])
        for s in sections if s["lang"] == target_lang
    ]
    return target_sections, shared_content


# rebuild

def rebuild_markdown(
    title: str,
    sections: list[tuple[int, str, str]],
    shared_content: str,
) -> Optional[str]:
    parts: list[str] = []

    if title:
        parts.append(f"# {title}")
        parts.append("")
    if shared_content:
        parts.append(shared_content)
        parts.append("")

    for level, heading, content in sections:
        parts.append(f"{'#' * level} {heading}")
        parts.append("")
        if content:
            parts.append(content)
        parts.append("")

    if not parts:
        return None

    result = "\n".join(parts)
    result = re.sub(r"\n{4,}", "\n\n", result)
    return result.strip() + "\n"


# process

def process_file(filepath: Path, target_lang: str) -> Optional[str]:
    content = filepath.read_text(encoding="utf-8")
    fm, body = parse_frontmatter(content)
    title = get_title(fm, target_lang)
    sections, shared = extract_sections(body, target_lang)
    return rebuild_markdown(title, sections, shared)


def process_all_pages(pages_dir: Path, output_dir: Path, target_lang: str) -> int:
    count = 0
    for fpath in sorted(pages_dir.rglob("*.md")):
        rel = fpath.relative_to(pages_dir)
        out = output_dir / rel
        out.parent.mkdir(parents=True, exist_ok=True)

        md = process_file(fpath, target_lang)
        if md is None:
            print(f"  - {rel}  (no content for lang={target_lang})")
            continue

        out.write_text(md, encoding="utf-8")
        ok(str(rel))
        count += 1

    return count


# main

def main() -> None:
    import argparse
    parser = argparse.ArgumentParser(
        description="Download and extract RitsuLib docs for specified language.",
    )
    parser.add_argument(
        "--lang", nargs="?", default="zh-CN",
        help='Language of the docs (default: zh-CN). Suppored: "en", "zh-CN".',
    )
    args = parser.parse_args()

    output_dir = docs_dir() / "STS2-RitsuLib"

    pages_dir = fetch_pages()
    print()

    step("Processing pages …")
    file_count = process_all_pages(pages_dir, output_dir, args.lang)
    ok(f"Processed {file_count} pages → {output_dir}")


if __name__ == "__main__":
    main()
