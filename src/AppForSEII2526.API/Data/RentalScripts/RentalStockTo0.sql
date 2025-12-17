-- ============================================
-- Script para testing: Poner cantidad alquiler a 0
-- ============================================

UPDATE [dbo].[Devices] 
SET QuantityForRent = 0;

SELECT Brand, Name, QuantityForRent, QuantityForPurchase 
FROM [dbo].[Devices];

PRINT 'Todas las cantidades de alquiler han sido puestas a 0'
