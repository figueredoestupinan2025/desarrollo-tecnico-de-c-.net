$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\\..")).Path
$downloads = Join-Path $HOME "Downloads"
$inputPdf = Join-Path $downloads "RESERVAS_AnexoPruebaTecnica_mayo2026 (5).pdf"

if (!(Test-Path $inputPdf)) {
  throw "No se encontró el PDF en: $inputPdf"
}

$tmpDir = Join-Path $repoRoot "tmp"
New-Item -ItemType Directory -Path $tmpDir -Force | Out-Null

$ocrPdf = Join-Path $tmpDir "RESERVAS_AnexoPruebaTecnica_mayo2026_ocr.pdf"

Write-Host "Entrada: $inputPdf"
Write-Host "Salida OCR: $ocrPdf"

# Usa ocrmypdf dentro de Docker para evitar instalaciones locales.
# Requiere que Docker pueda descargar la imagen (si no existe localmente).
docker run --rm `
  -v "${downloads}:/in:ro" `
  -v "${tmpDir}:/out" `
  jbarlow83/ocrmypdf:latest `
  --skip-text --force-ocr `
  "/in/RESERVAS_AnexoPruebaTecnica_mayo2026 (5).pdf" `
  "/out/RESERVAS_AnexoPruebaTecnica_mayo2026_ocr.pdf"

Write-Host "Extrayendo texto del PDF OCR..."
python (Join-Path $repoRoot "scripts\\ocr\\anexo_extract.py")

Write-Host "Listo. Revisa:"
Write-Host " - $ocrPdf"
Write-Host " - $(Join-Path $tmpDir 'anexo_reservas_extract.txt')"

