from __future__ import annotations

from pathlib import Path

from pypdf import PdfReader


def extract_text(pdf_path: Path) -> str:
    reader = PdfReader(str(pdf_path))
    chunks: list[str] = []
    for i, page in enumerate(reader.pages):
        try:
            page_text = page.extract_text() or ""
        except Exception:
            page_text = ""
        chunks.append(f"\n\n===== PAGE {i + 1} =====\n\n{page_text}")
    return "".join(chunks)


def main() -> None:
    repo_root = Path(__file__).resolve().parents[2]
    tmp_dir = repo_root / "tmp"
    tmp_dir.mkdir(exist_ok=True)

    default_pdf = Path.home() / "Downloads" / "RESERVAS_AnexoPruebaTecnica_mayo2026 (5).pdf"
    pdf_path = default_pdf if default_pdf.exists() else None

    if pdf_path is None:
        raise SystemExit(
            "No se encontró el PDF del anexo en Descargas. "
            "Ajusta la ruta dentro de scripts/ocr/anexo_extract.py."
        )

    out_path = tmp_dir / "anexo_reservas_extract.txt"
    text = extract_text(pdf_path)
    out_path.write_text(text, encoding="utf-8", errors="ignore")

    print(f"PDF: {pdf_path}")
    print(f"Salida: {out_path}")
    print(f"Caracteres extraídos: {len(text)}")
    if len(text.strip()) < 500:
        print(
            "\nEl texto extraído es muy corto: el PDF parece escaneado. "
            "Para obtener texto real se requiere OCR (por ejemplo, ocrmypdf/tesseract o una herramienta de OCR manual)."
        )


if __name__ == "__main__":
    main()

