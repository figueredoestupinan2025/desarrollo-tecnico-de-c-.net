# Validación del anexo (OCR)

El archivo `RESERVAS_AnexoPruebaTecnica_mayo2026 (5).pdf` parece estar escaneado. Para validarlo se requiere OCR.

## Paso 1: extraer (sin OCR)

```powershell
python .\scripts\ocr\anexo_extract.py
```

Si `tmp/anexo_reservas_extract.txt` sale casi vacío, continúa con OCR.

## Paso 2: hacer OCR (manual o con herramienta)

Opciones típicas:
- OCR con Docker (recomendado, sin instalar nada):
  ```powershell
  powershell -ExecutionPolicy Bypass -File .\scripts\ocr\ocr_anexo.ps1
  ```
- OCR manual (OneNote/Word/Google Drive) y copiar el texto resultante.
- Si cuentas con herramientas: `ocrmypdf` + `tesseract`.

Pega aquí el texto OCR obtenido (o guarda un archivo `tmp/anexo_reservas_ocr.txt`).

## Paso 3: checklist de requisitos (comparación)

Marca y referencia (pantalla/código/SP) dónde se cumple cada punto del anexo.

- [ ] Requisito 1:
  - Evidencia:
- [ ] Requisito 2:
  - Evidencia:
- [ ] Requisito 3:
  - Evidencia:

## Resultado

- Estado: (Cumplido / Parcial / No aplica)
- Comentarios:
