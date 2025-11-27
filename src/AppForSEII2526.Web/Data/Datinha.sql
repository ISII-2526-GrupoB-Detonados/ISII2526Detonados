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

