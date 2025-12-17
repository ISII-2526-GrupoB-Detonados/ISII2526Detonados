-- ============================================
-- Script para restaurar cantidades originales de alquiler
-- ============================================

UPDATE [dbo].[Devices] SET QuantityForRent = 25 WHERE Id = 1;
UPDATE [dbo].[Devices] SET QuantityForRent = 20 WHERE Id = 2;
UPDATE [dbo].[Devices] SET QuantityForRent = 22 WHERE Id = 3;
UPDATE [dbo].[Devices] SET QuantityForRent = 18 WHERE Id = 4;
UPDATE [dbo].[Devices] SET QuantityForRent = 0 WHERE Id = 5;
UPDATE [dbo].[Devices] SET QuantityForRent = 19 WHERE Id = 6;
UPDATE [dbo].[Devices] SET QuantityForRent = 24 WHERE Id = 7;
UPDATE [dbo].[Devices] SET QuantityForRent = 16 WHERE Id = 8;
UPDATE [dbo].[Devices] SET QuantityForRent = 15 WHERE Id = 9;
UPDATE [dbo].[Devices] SET QuantityForRent = 28 WHERE Id = 10;

SELECT Brand, Name, QuantityForRent, QuantityForPurchase 
FROM [dbo].[Devices];

PRINT 'Cantidades de alquiler restauradas a valores originales'
