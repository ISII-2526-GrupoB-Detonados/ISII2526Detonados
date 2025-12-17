-- ============================================
-- SCRIPT DE LIMPIEZA DE DATOS
-- Borra todos los datos pero mantiene la estructura
-- Resetea los IDENTITY para evitar duplicados
-- ============================================

PRINT 'Iniciando limpieza de base de datos...'
GO

-- Paso 1: Deshabilitar todas las foreign keys
PRINT 'Deshabilitando foreign keys...'
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'
GO

-- Paso 2: Borrar todos los datos de todas las tablas
PRINT 'Borrando datos...'
EXEC sp_MSforeachtable 'DELETE FROM ?'
GO

-- Paso 3: Resetear los IDENTITY seeds a 0
PRINT 'Reseteando IDENTITY columns...'
EXEC sp_MSforeachtable 'IF OBJECTPROPERTY(OBJECT_ID(''?''), ''TableHasIdentity'') = 1 DBCC CHECKIDENT(''?'', RESEED, 0)'
GO

-- Paso 4: Rehabilitar todas las foreign keys
PRINT 'Rehabilitando foreign keys...'
EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'
GO

PRINT '✓ Base de datos limpiada correctamente'
PRINT '✓ Lista para insertar datos frescos sin duplicados'
GO
