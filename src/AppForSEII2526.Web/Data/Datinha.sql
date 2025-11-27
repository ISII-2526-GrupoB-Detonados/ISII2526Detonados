---------------------------------------------------------------------------------------------------------------
-- USUARIOS (3 usuarios)
INSERT INTO [dbo].[AspNetUsers] ([Id], [Surname], [Name], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) 
VALUES (N'user-001', N'Lorenzo', N'Luis', N'luis.lorenzo@email.com', N'LUIS.LORENZO@EMAIL.COM', N'luis.lorenzo@email.com', N'LUIS.LORENZO@EMAIL.COM', 1, N'AQAAAAIAAYagAAAAEJ1234567890abcdefghijklmnopqrstuvwxyz', N'339BD9CA-5F0A-44C5-8906-328463C8E2DA', N'C65E85E2-F2CE-4611-B859-73BE18CC24BB', N'+34600111222', 1, 0, NULL, 1, 0)

INSERT INTO [dbo].[AspNetUsers] ([Id], [Surname], [Name], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) 
VALUES (N'user-002', N'Lopes', N'Patrik', N'patrik.lopes@email.com', N'PATRIK.LOPES@EMAIL.COM', N'patrik.lopes@email.com', N'PATRIK.LOPES@EMAIL.COM', 1, N'AQAAAAIAAYagAAAAEJ1234567890abcdefghijklmnopqrstuvwxyz', N'415AEF77-B318-43BC-96D4-57347ED71A0D', N'8D6F3E42-20F5-4EF6-ACCC-F368DB0B16EB', N'+34600222333', 1, 0, NULL, 1, 0)

INSERT INTO [dbo].[AspNetUsers] ([Id], [Surname], [Name], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) 
VALUES (N'user-003', N'Jara', N'Alejandro', N'alejandro.jara@email.com', N'ALEJANDRO.JARA@EMAIL.COM', N'alejandro.jara@email.com', N'ALEJANDRO.JARA@EMAIL.COM', 1, N'AQAAAAIAAYagAAAAEJ1234567890abcdefghijklmnopqrstuvwxyz', N'BE2980C9-97EC-4951-9E27-795F1291FD9D', N'FEAE93AF-0400-4946-B7FA-E19C5D2F649B', N'+34600333444', 1, 0, NULL, 1, 0)

-----------------------------------
-- Patrik

-- Scales 
SET IDENTITY_INSERT [dbo].[Scales] ON
INSERT INTO [dbo].[Scales] ([Id], [Name]) VALUES (1, N'Baja')
INSERT INTO [dbo].[Scales] ([Id], [Name]) VALUES (2, N'Mediocre')
INSERT INTO [dbo].[Scales] ([Id], [Name]) VALUES (3, N'Media')
INSERT INTO [dbo].[Scales] ([Id], [Name]) VALUES (4, N'Alta ')
INSERT INTO [dbo].[Scales] ([Id], [Name]) VALUES (5, N'Lujo')
SET IDENTITY_INSERT [dbo].[Scales] OFF

-- Repairs
SET IDENTITY_INSERT [dbo].[Repairs] ON
INSERT INTO [dbo].[Repairs] ([Id], [Description], [Cost], [Name], [ScaleId]) VALUES (1, N'Reemplazo del sensor de pesaje averiado', 85.5, N'Reparación de sensor', 1)
INSERT INTO [dbo].[Repairs] ([Id], [Description], [Cost], [Name], [ScaleId]) VALUES (2, N'Ajuste de calibración por desviación mínima detectada', 45, N'Calibración de precisión', 2)
INSERT INTO [dbo].[Repairs] ([Id], [Description], [Cost], [Name], [ScaleId]) VALUES (3, N'Sustitución de pantalla LCD dañada', 120, N'Reparación de pantalla', 3)
INSERT INTO [dbo].[Repairs] ([Id], [Description], [Cost], [Name], [ScaleId]) VALUES (4, N'Reparación de cableado interno oxidado por humedad', 60, N'Reparación eléctrica', 1)
INSERT INTO [dbo].[Repairs] ([Id], [Description], [Cost], [Name], [ScaleId]) VALUES (5, N'Reemplazo del módulo principal de lectura y prueba completa', 95.75, N'Reparación de módulo principal', 4)
INSERT INTO [dbo].[Repairs] ([Id], [Description], [Cost], [Name], [ScaleId]) VALUES (6, N'Limpieza interna y revisión de conectores', 35.5, N'Mantenimiento preventivo', 2)
INSERT INTO [dbo].[Repairs] ([Id], [Description], [Cost], [Name], [ScaleId]) VALUES (7, N'Actualización del firmware de control interno', 110, N'Actualización de software', 5)
INSERT INTO [dbo].[Repairs] ([Id], [Description], [Cost], [Name], [ScaleId]) VALUES (8, N'Revisión y cambio de teclado dañado por uso prolongado', 50, N'Reparación de teclado', 3)
INSERT INTO [dbo].[Repairs] ([Id], [Description], [Cost], [Name], [ScaleId]) VALUES (9, N'Reemplazo de celda de carga averiada', 80.25, N'Reparación de celda de carga', 4)
INSERT INTO [dbo].[Repairs] ([Id], [Description], [Cost], [Name], [ScaleId]) VALUES (10, N'Corrección de lecturas erróneas causadas por interferencia eléctrica', 70, N'Reparación de lectura', 1)
INSERT INTO [dbo].[Repairs] ([Id], [Description], [Cost], [Name], [ScaleId]) VALUES (11, N'Reemplazo de batería interna y prueba de autonomía', 40, N'Mantenimiento de batería', 2)
INSERT INTO [dbo].[Repairs] ([Id], [Description], [Cost], [Name], [ScaleId]) VALUES (12, N'Ajuste del sistema de pesaje tras golpe mecánico', 55.5, N'Reparación de alineación', 3)
INSERT INTO [dbo].[Repairs] ([Id], [Description], [Cost], [Name], [ScaleId]) VALUES (13, N'Sustitución del puerto de conexión USB dañado', 65, N'Reparación de puerto', 4)
INSERT INTO [dbo].[Repairs] ([Id], [Description], [Cost], [Name], [ScaleId]) VALUES (14, N'Lubricación y ajuste de piezas móviles', 30, N'Mantenimiento mecánico', 5)
INSERT INTO [dbo].[Repairs] ([Id], [Description], [Cost], [Name], [ScaleId]) VALUES (15, N'Comprobación de estabilidad y recableado parcial', 75, N'Revisión estructural', 1)
SET IDENTITY_INSERT [dbo].[Repairs] OFF

-- Receipts
SET IDENTITY_INSERT [dbo].[Receipts] ON
INSERT INTO [dbo].[Receipts] ([Id], [DeliveryAddress], [PaymentMethodTypes], [ApplicationUserId], [ReceiptDate], [TotalPrice]) VALUES (23, N'123 Calle Principal', 1, N'User-001', N'2025-10-22 10:30:00', 150.75)
INSERT INTO [dbo].[Receipts] ([Id], [DeliveryAddress], [PaymentMethodTypes], [ApplicationUserId], [ReceiptDate], [TotalPrice]) VALUES (27, N'456 Avenida Secundaria', 2, N'User-002', N'2024-10-22 12:30:00', 34.7)
INSERT INTO [dbo].[Receipts] ([Id], [DeliveryAddress], [PaymentMethodTypes], [ApplicationUserId], [ReceiptDate], [TotalPrice]) VALUES (33, N'789 Boulevard Central', 3, N'User-003', N'2025-12-21 12:15:00', 76.87)
INSERT INTO [dbo].[Receipts] ([Id], [DeliveryAddress], [PaymentMethodTypes], [ApplicationUserId], [ReceiptDate], [TotalPrice]) VALUES (38, N'101 Calle Nueva', 2, N'User-004', N'2025-12-22 13:55:00', 123.8)
SET IDENTITY_INSERT [dbo].[Receipts] OFF

-- ReceiptItems
INSERT INTO [dbo].[ReceiptItems] ([ReceiptId], [RepairId], [Model]) VALUES (23, 11, N'Modelo1')
INSERT INTO [dbo].[ReceiptItems] ([ReceiptId], [RepairId], [Model]) VALUES (27, 9, N'Modelo2')
INSERT INTO [dbo].[ReceiptItems] ([ReceiptId], [RepairId], [Model]) VALUES (33, 10, N'Modelo3')
INSERT INTO [dbo].[ReceiptItems] ([ReceiptId], [RepairId], [Model]) VALUES (38, 1, N'Modelo4')
INSERT INTO [dbo].[ReceiptItems] ([ReceiptId], [RepairId], [Model]) VALUES (2071, 1, N'string')
INSERT INTO [dbo].[ReceiptItems] ([ReceiptId], [RepairId], [Model]) VALUES (2072, 1, N'string')
INSERT INTO [dbo].[ReceiptItems] ([ReceiptId], [RepairId], [Model]) VALUES (2073, 1, N'string')
INSERT INTO [dbo].[ReceiptItems] ([ReceiptId], [RepairId], [Model]) VALUES (2074, 1, N'string')
INSERT INTO [dbo].[ReceiptItems] ([ReceiptId], [RepairId], [Model]) VALUES (2075, 1, N'string')
INSERT INTO [dbo].[ReceiptItems] ([ReceiptId], [RepairId], [Model]) VALUES (2076, 1, N'string')
INSERT INTO [dbo].[ReceiptItems] ([ReceiptId], [RepairId], [Model]) VALUES (2077, 1, N'string')
INSERT INTO [dbo].[ReceiptItems] ([ReceiptId], [RepairId], [Model]) VALUES (2078, 1, N'string')

--LuisCock


-- MODELOS
SET IDENTITY_INSERT [dbo].[Models] ON
INSERT INTO [dbo].[Models] ([Id], [NameModel]) VALUES (1, N'Xiami x3')
INSERT INTO [dbo].[Models] ([Id], [NameModel]) VALUES (2, N'Samsung Galaxy S24')
INSERT INTO [dbo].[Models] ([Id], [NameModel]) VALUES (3, N'Xiaomi 14 Pro')
INSERT INTO [dbo].[Models] ([Id], [NameModel]) VALUES (4, N'Google Pixel 8')
INSERT INTO [dbo].[Models] ([Id], [NameModel]) VALUES (5, N'OnePlus 12')
INSERT INTO [dbo].[Models] ([Id], [NameModel]) VALUES (6, N'Huawei P60 Pro')
INSERT INTO [dbo].[Models] ([Id], [NameModel]) VALUES (7, N'Motorola Edge 40')
INSERT INTO [dbo].[Models] ([Id], [NameModel]) VALUES (8, N'Oppo Find X7')
INSERT INTO [dbo].[Models] ([Id], [NameModel]) VALUES (9, N'Sony Xperia 1 V')
INSERT INTO [dbo].[Models] ([Id], [NameModel]) VALUES (10, N'Nokia G400')
SET IDENTITY_INSERT [dbo].[Models] OFF

---------------------
-- DISPOSITIVOS
SET IDENTITY_INSERT [dbo].[Devices] ON
INSERT INTO [dbo].[Devices] ([Id], [Brand], [Color], [Name], [PriceForPurchase], [PriceForRent], [QuantityForPurchase], [QuantityForRent], [Year], [ModelId]) VALUES (1, N'Xiaomi', N'Black', N'Xiaomi x3 Premium', 299.99, 50, 100, 25, 2024, 1)
INSERT INTO [dbo].[Devices] ([Id], [Brand], [Color], [Name], [PriceForPurchase], [PriceForRent], [QuantityForPurchase], [QuantityForRent], [Year], [ModelId]) VALUES (2, N'Samsung', N'White', N'Samsung Galaxy S24 Ultra', 1199.99, 55, 80, 20, 2024, 2)
INSERT INTO [dbo].[Devices] ([Id], [Brand], [Color], [Name], [PriceForPurchase], [PriceForRent], [QuantityForPurchase], [QuantityForRent], [Year], [ModelId]) VALUES (3, N'Xiaomi', N'Blue', N'Xiaomi 14 Pro Max', 899.99, 60, 90, 22, 2024, 3)
INSERT INTO [dbo].[Devices] ([Id], [Brand], [Color], [Name], [PriceForPurchase], [PriceForRent], [QuantityForPurchase], [QuantityForRent], [Year], [ModelId]) VALUES (4, N'Google', N'Gray', N'Google Pixel 8 Pro', 999.99, 65, 70, 18, 2024, 4)
INSERT INTO [dbo].[Devices] ([Id], [Brand], [Color], [Name], [PriceForPurchase], [PriceForRent], [QuantityForPurchase], [QuantityForRent], [Year], [ModelId]) VALUES (5, N'OnePlus', N'Green', N'OnePlus 12 Pro', 849.99, 70, 85, 21, 2024, 5)
INSERT INTO [dbo].[Devices] ([Id], [Brand], [Color], [Name], [PriceForPurchase], [PriceForRent], [QuantityForPurchase], [QuantityForRent], [Year], [ModelId]) VALUES (6, N'Huawei', N'Silver', N'Huawei P60 Pro', 1099.99, 45, 75, 19, 2024, 6)
INSERT INTO [dbo].[Devices] ([Id], [Brand], [Color], [Name], [PriceForPurchase], [PriceForRent], [QuantityForPurchase], [QuantityForRent], [Year], [ModelId]) VALUES (7, N'Motorola', N'Red', N'Motorola Edge 40 Neo', 599.99, 80, 95, 24, 2024, 7)
INSERT INTO [dbo].[Devices] ([Id], [Brand], [Color], [Name], [PriceForPurchase], [PriceForRent], [QuantityForPurchase], [QuantityForRent], [Year], [ModelId]) VALUES (8, N'Oppo', N'Purple', N'Oppo Find X7 Ultra', 949.99, 35, 65, 16, 2024, 8)
INSERT INTO [dbo].[Devices] ([Id], [Brand], [Color], [Name], [PriceForPurchase], [PriceForRent], [QuantityForPurchase], [QuantityForRent], [Year], [ModelId]) VALUES (9, N'Sony', N'Gold', N'Sony Xperia 1 VI', 1299.99, 75, 60, 15, 2024, 9)
INSERT INTO [dbo].[Devices] ([Id], [Brand], [Color], [Name], [PriceForPurchase], [PriceForRent], [QuantityForPurchase], [QuantityForRent], [Year], [ModelId]) VALUES (10, N'Nokia', N'Orange', N'Nokia G400 Plus', 399.99, 40, 110, 28, 2024, 10)
SET IDENTITY_INSERT [dbo].[Devices] OFF

-----------------------------
-- ALQUILERES (RENTALS) - Con direcciones personalizadas por usuario
SET IDENTITY_INSERT [dbo].[Rentals] ON
-- Alquileres de Luis (user-001) - Avenida Chad
INSERT INTO [dbo].[Rentals] ([Id], [DeliveryAddress], [PaymentMethod], [ApplicationUserId], [RentalDate], [RentalDateFrom], [RentalDateTo], [TotalPrice]) VALUES (1, N'Avenida Chad 25, Albacete', 0, N'user-001', N'2025-01-10 10:00:00', N'2025-02-10 00:00:00', N'2025-09-10 00:00:00', 10500)
INSERT INTO [dbo].[Rentals] ([Id], [DeliveryAddress], [PaymentMethod], [ApplicationUserId], [RentalDate], [RentalDateFrom], [RentalDateTo], [TotalPrice]) VALUES (4, N'Avenida Chad 25, Albacete', 2, N'user-001', N'2025-07-05 09:15:00', N'2025-07-06 00:00:00', N'2025-07-10 00:00:00', 780)
INSERT INTO [dbo].[Rentals] ([Id], [DeliveryAddress], [PaymentMethod], [ApplicationUserId], [RentalDate], [RentalDateFrom], [RentalDateTo], [TotalPrice]) VALUES (7, N'Avenida Chad 25, Albacete', 0, N'user-001', N'2025-04-12 10:30:00', N'2025-04-13 00:00:00', N'2025-04-19 00:00:00', 1530)
INSERT INTO [dbo].[Rentals] ([Id], [DeliveryAddress], [PaymentMethod], [ApplicationUserId], [RentalDate], [RentalDateFrom], [RentalDateTo], [TotalPrice]) VALUES (10, N'Avenida Chad 25, Albacete', 0, N'user-001', N'2025-01-10 08:45:00', N'2025-01-11 00:00:00', N'2025-01-15 00:00:00', 920)

-- Alquileres de Patrik (user-002) - Calle Brasileño Edgy
INSERT INTO [dbo].[Rentals] ([Id], [DeliveryAddress], [PaymentMethod], [ApplicationUserId], [RentalDate], [RentalDateFrom], [RentalDateTo], [TotalPrice]) VALUES (2, N'Calle Brasileño Edgy 15, Madrid', 1, N'user-002', N'2025-09-15 11:30:00', N'2025-09-16 00:00:00', N'2025-09-20 00:00:00', 440)
INSERT INTO [dbo].[Rentals] ([Id], [DeliveryAddress], [PaymentMethod], [ApplicationUserId], [RentalDate], [RentalDateFrom], [RentalDateTo], [TotalPrice]) VALUES (5, N'Calle Brasileño Edgy 15, Madrid', 0, N'user-002', N'2025-06-10 16:45:00', N'2025-06-12 00:00:00', N'2025-07-17 00:00:00', 4900)
INSERT INTO [dbo].[Rentals] ([Id], [DeliveryAddress], [PaymentMethod], [ApplicationUserId], [RentalDate], [RentalDateFrom], [RentalDateTo], [TotalPrice]) VALUES (8, N'Calle Brasileño Edgy 15, Madrid', 2, N'user-002', N'2025-03-01 12:00:00', N'2025-03-02 00:00:00', N'2025-03-06 00:00:00', 540)
INSERT INTO [dbo].[Rentals] ([Id], [DeliveryAddress], [PaymentMethod], [ApplicationUserId], [RentalDate], [RentalDateFrom], [RentalDateTo], [TotalPrice]) VALUES (12, N'Calle Brasileño Edgy 15, Madrid', 0, N'user-002', N'2025-10-31 13:02:56', N'2025-11-05 00:00:00', N'2025-11-10 00:00:00', 250)

-- Alquileres de Alejandro (user-003) - Calle Femboys
INSERT INTO [dbo].[Rentals] ([Id], [DeliveryAddress], [PaymentMethod], [ApplicationUserId], [RentalDate], [RentalDateFrom], [RentalDateTo], [TotalPrice]) VALUES (3, N'Calle Femboys 8, Barcelona', 0, N'user-003', N'2025-08-20 14:20:00', N'2025-08-22 00:00:00', N'2025-08-30 00:00:00', 960)
INSERT INTO [dbo].[Rentals] ([Id], [DeliveryAddress], [PaymentMethod], [ApplicationUserId], [RentalDate], [RentalDateFrom], [RentalDateTo], [TotalPrice]) VALUES (6, N'Calle Femboys 8, Barcelona', 1, N'user-003', N'2025-05-25 13:10:00', N'2025-05-26 00:00:00', N'2025-05-29 00:00:00', 510)
INSERT INTO [dbo].[Rentals] ([Id], [DeliveryAddress], [PaymentMethod], [ApplicationUserId], [RentalDate], [RentalDateFrom], [RentalDateTo], [TotalPrice]) VALUES (9, N'Calle Femboys 8, Barcelona', 1, N'user-003', N'2025-02-18 15:20:00', N'2025-02-19 00:00:00', N'2025-02-25 00:00:00', 1380)
SET IDENTITY_INSERT [dbo].[Rentals] OFF

-----------------------------
-- DISPOSITIVOS ALQUILADOS (RENTDEVICES)
INSERT INTO [dbo].[RentDevices] ([DeviceId], [RentId], [Price], [Quantity]) VALUES (1, 1, 50, 1)
INSERT INTO [dbo].[RentDevices] ([DeviceId], [RentId], [Price], [Quantity]) VALUES (1, 12, 50, 1)
INSERT INTO [dbo].[RentDevices] ([DeviceId], [RentId], [Price], [Quantity]) VALUES (2, 2, 55, 1)
INSERT INTO [dbo].[RentDevices] ([DeviceId], [RentId], [Price], [Quantity]) VALUES (3, 3, 60, 1)
INSERT INTO [dbo].[RentDevices] ([DeviceId], [RentId], [Price], [Quantity]) VALUES (4, 4, 65, 1)
INSERT INTO [dbo].[RentDevices] ([DeviceId], [RentId], [Price], [Quantity]) VALUES (5, 5, 70, 1)
INSERT INTO [dbo].[RentDevices] ([DeviceId], [RentId], [Price], [Quantity]) VALUES (6, 6, 45, 1)
INSERT INTO [dbo].[RentDevices] ([DeviceId], [RentId], [Price], [Quantity]) VALUES (7, 7, 80, 1)
INSERT INTO [dbo].[RentDevices] ([DeviceId], [RentId], [Price], [Quantity]) VALUES (8, 8, 35, 1)
INSERT INTO [dbo].[RentDevices] ([DeviceId], [RentId], [Price], [Quantity]) VALUES (9, 9, 75, 1)
INSERT INTO [dbo].[RentDevices] ([DeviceId], [RentId], [Price], [Quantity]) VALUES (10, 10, 40, 1)

--JARA