-- ============================================================================
-- Reset selectivo de datos de clientes en PRODUCCION (PostgreSQL / Supabase)
-- ============================================================================
-- Elimina: clientes, mascotas y todos sus registros dependientes
--          (citas, consultas, facturas, PDFs/DOCX referenciados, vacunas aplicadas
--           y su catalogo, historias clinicas, productos y su historial/stock).
-- Conserva: usuarios, servicios, especies, categorias_productos, metodos_pago,
--           MoneyTypes, worker_configs, system_configs, Reportes, logs_sistema,
--           alertas_internas, mensajes, permission_*, integration_credentials,
--           password_reset_tickets (todo lo que es ajuste/funcionamiento del
--           sistema: workers, plantillas de email, credenciales, razas/especies,
--           permisos).
--
-- IMPORTANTE:
--   1) Correr primero el SELECT de verificacion al final de este archivo
--      contra la DB para confirmar el conteo de filas antes de borrar.
--   2) Esto NO borra los archivos fisicos de PDF/DOCX en wwwroot/ o en el
--      bucket de Supabase Storage; esta accion es solo sobre la base de datos.
--   3) Ejecutar dentro de una transaccion para poder hacer ROLLBACK si algo
--      no coincide con lo esperado.
-- ============================================================================

BEGIN;

-- Nivel 5: dependen de facturas/consultas/citas/vaccine_batches
DELETE FROM detalles_factura;
DELETE FROM consultas_productos;
DELETE FROM pet_vaccinations;

-- Nivel 4: dependen de clientes/consultas/mascotas/vaccines
DELETE FROM facturas;
DELETE FROM historias_clinicas;
DELETE FROM vaccine_batches;
DELETE FROM vaccine_scheme_stages;

-- Nivel 3: consultas depende de citas + mascotas
DELETE FROM consultas;

-- Nivel 2: citas depende de mascotas; vaccines ya sin baches/etapas/aplicaciones
DELETE FROM citas;
DELETE FROM vaccines;

-- Nivel 1: mascotas depende de clientes
DELETE FROM mascotas;

-- Nivel 0: productos (con su historial y movimientos de stock) y clientes (raiz)
DELETE FROM historial_precios;
DELETE FROM movimientos_inventario;
DELETE FROM productos;
DELETE FROM clientes;

COMMIT;

-- ============================================================================
-- Verificacion post-borrado (correr despues del COMMIT)
-- ============================================================================
-- SELECT
--   (SELECT COUNT(*) FROM clientes)             AS clientes,
--   (SELECT COUNT(*) FROM mascotas)              AS mascotas,
--   (SELECT COUNT(*) FROM citas)                 AS citas,
--   (SELECT COUNT(*) FROM consultas)             AS consultas,
--   (SELECT COUNT(*) FROM facturas)              AS facturas,
--   (SELECT COUNT(*) FROM detalles_factura)      AS detalles_factura,
--   (SELECT COUNT(*) FROM consultas_productos)   AS consultas_productos,
--   (SELECT COUNT(*) FROM pet_vaccinations)      AS pet_vaccinations,
--   (SELECT COUNT(*) FROM vaccines)              AS vaccines,
--   (SELECT COUNT(*) FROM vaccine_batches)       AS vaccine_batches,
--   (SELECT COUNT(*) FROM vaccine_scheme_stages) AS vaccine_scheme_stages,
--   (SELECT COUNT(*) FROM historias_clinicas)    AS historias_clinicas,
--   (SELECT COUNT(*) FROM productos)             AS productos,
--   (SELECT COUNT(*) FROM historial_precios)     AS historial_precios,
--   (SELECT COUNT(*) FROM movimientos_inventario) AS movimientos_inventario,
--   (SELECT COUNT(*) FROM usuarios)              AS usuarios_conservados,
--   (SELECT COUNT(*) FROM servicios)             AS servicios_conservados,
--   (SELECT COUNT(*) FROM especies)              AS especies_conservadas,
--   (SELECT COUNT(*) FROM categorias_productos)  AS categorias_conservadas,
--   (SELECT COUNT(*) FROM metodos_pago)          AS metodos_pago_conservados;
