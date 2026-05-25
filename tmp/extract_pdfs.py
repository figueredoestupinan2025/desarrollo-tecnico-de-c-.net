from pypdf import PdfReader
from pathlib import Path

pdfs = [
    (r"C:\\Users\\User\\Downloads\\PruebaTecnica_Desarrollador_mayo2026 (6).pdf", "tmp/prueba_tecnica.txt"),
    (r"C:\\Users\\User\\Downloads\\RESERVAS_AnexoPruebaTecnica_mayo2026 (5).pdf", "tmp/anexo_reservas.txt"),
]

Path('tmp').mkdir(exist_ok=True)
for pdf_path, out_path in pdfs:
    reader = PdfReader(pdf_path)
    texts = []
    for i, page in enumerate(reader.pages):
        try:
            t = page.extract_text() or ""
        except Exception:
            t = ""
        texts.append(f"\n\n===== PAGE {i+1} =====\n\n" + t)
    Path(out_path).write_text("".join(texts), encoding='utf-8', errors='ignore')
    print(f"{pdf_path} -> {out_path} pages={len(reader.pages)} chars={sum(len(t) for t in texts)}")
