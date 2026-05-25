# OCR del anexo (PDF escaneado)

El archivo `RESERVAS_AnexoPruebaTecnica_mayo2026 (5).pdf` parece estar escaneado, por lo que `pypdf` casi no puede extraer texto.

## Extracción rápida (sin OCR)

Genera un `.txt` con lo que sea extraíble:

```powershell
python .\scripts\ocr\anexo_extract.py
```

Salida esperada: `tmp/anexo_reservas_extract.txt`.

## OCR (opcional)

Si necesitas validar requisitos del anexo, haz OCR con una herramienta externa y vuelve a contrastar el texto resultante con el sistema.

Opciones:
- OCR con Docker (recomendado, sin instalar nada):
  ```powershell
  powershell -ExecutionPolicy Bypass -File .\scripts\ocr\ocr_anexo.ps1
  ```
- `ocrmypdf` + `tesseract` (si las tienes instaladas).
- OCR manual (por ejemplo, exportando a Word/OneNote y copiando texto reconocido).
