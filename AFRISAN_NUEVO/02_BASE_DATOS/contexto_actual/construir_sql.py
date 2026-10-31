"""Empaqueta los componentes en una instalación SQL única y transaccional."""
from pathlib import Path

root = Path(__file__).resolve().parent
parts = []
for name in ("01_operacion.sql", "02_soporte.sql", "03_auditoria_y_acceso.sql"):
    lines = (root / name).read_text(encoding="utf-8-sig").splitlines()
    parts.append("-- COMPONENTE: " + name + "\n" + "\n".join(line for line in lines if line.strip() not in ("BEGIN;", "COMMIT;")))
header = """-- AFRISAN | CONTEXTO ACTUAL | 25/10/2026
-- INSTALACIÓN VACÍA. Ejecutar únicamente este archivo completo.
-- Crea afrisan_actual y afrisan_bi_actual. No borra/migra afrisan ni public.
-- Se detiene si esos esquemas ya existen. No instala cuentas ni ejemplos.
-- La API actual sigue en memoria: este SQL no conecta ni adapta automáticamente EF Core.
-- Fotos: metadatos de objetos privados; la API verifica el archivo real en Storage.
-- Gramos enteros, fechas timestamptz, precios NUMERIC. No inferir consumo/emisiones.
-- La medición de límites se declara con criterio explícito; no inventa su fórmula.
-- Requiere PostgreSQL 15+; ver README y validación local antes de usar en Supabase.
BEGIN;
"""
target = root.parent / "04_Crear_Afrisan_Contexto_Actual.sql"
target.write_text(header + "\n\n".join(parts) + "\nCOMMIT;\n", encoding="utf-8")
print(target)
