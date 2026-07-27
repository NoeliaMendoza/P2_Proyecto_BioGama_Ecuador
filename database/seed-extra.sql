-- Seed adicional para alcanzar 1,000,000 registros
-- Se ejecuta automáticamente con docker-entrypoint-initdb.d (después de backup.sql)

-- Actualizar la secuencia de Species para evitar conflictos
SELECT setval('"Species_Id_seq"', (SELECT MAX("Id") FROM public."Species"), true);

-- 50 nuevas especies
INSERT INTO public."Species" ("Id", "CommonName", "ScientificName", "Description", "ImageUrl", "IsEndemic", "FamilyId", "IsActive", "CreatedAt", "ConservationStatusId", "UpdatedAt", "DeletedAt") VALUES
(30001, 'Mariposa morpho azul', 'Morpho helenor', 'Especie emblemática de los bosques ecuatorianos. Habita en ecosistemas tropicales y subtropicales.', 'https://example.com/species/30001.jpg', true, 4, true, '2026-07-27 00:00:00', 4, NULL, NULL),
(30002, 'Guacamayo verde', 'Ara ambiguus', 'Especie característica de la biodiversidad del Ecuador. Presente en varias regiones del país.', 'https://example.com/species/30002.jpg', false, 8, true, '2026-07-27 00:00:00', 3, NULL, NULL),
(30003, 'Oso hormiguero gigante', 'Myrmecophaga tridactyla', 'Especie nativa del Ecuador. Juega un papel importante en el equilibrio de los ecosistemas.', 'https://example.com/species/30003.jpg', false, 12, true, '2026-07-27 00:00:00', 2, NULL, NULL),
(30004, 'Tucán andino', 'Andigena hypoglauca', 'Especie representativa de la fauna ecuatoriana. Se encuentra en peligro por pérdida de hábitat.', 'https://example.com/species/30004.jpg', true, 8, true, '2026-07-27 00:00:00', 5, NULL, NULL),
(30005, 'Colibrí picoespada', 'Ensifera ensifera', 'Especie emblemática de los bosques ecuatorianos. Habita en ecosistemas tropicales y subtropicales.', 'https://example.com/species/30005.jpg', true, 15, true, '2026-07-27 00:00:00', 4, NULL, NULL),
(30006, 'Jaguar amazónico', 'Panthera onca', 'Especie característica de la biodiversidad del Ecuador. Presente en varias regiones del país.', 'https://example.com/species/30006.jpg', false, 12, true, '2026-07-27 00:00:00', 1, NULL, NULL),
(30007, 'Delfín rosado', 'Inia geoffrensis', 'Especie nativa del Ecuador. Juega un papel importante en el equilibrio de los ecosistemas.', 'https://example.com/species/30007.jpg', false, 20, true, '2026-07-27 00:00:00', 3, NULL, NULL),
(30008, 'Nutria gigante', 'Pteronura brasiliensis', 'Especie representativa de la fauna ecuatoriana. Se encuentra en peligro por pérdida de hábitat.', 'https://example.com/species/30008.jpg', false, 12, true, '2026-07-27 00:00:00', 2, NULL, NULL),
(30009, 'Cóndor andino', 'Vultur gryphus', 'Especie emblemática de los bosques ecuatorianos. Habita en ecosistemas tropicales y subtropicales.', 'https://example.com/species/30009.jpg', false, 8, true, '2026-07-27 00:00:00', 1, NULL, NULL),
(30010, 'Puma andino', 'Puma concolor', 'Especie característica de la biodiversidad del Ecuador. Presente en varias regiones del país.', 'https://example.com/species/30010.jpg', false, 12, true, '2026-07-27 00:00:00', 5, NULL, NULL),
(30011, 'Loro cabeza roja', 'Psittacara erythrogenys', 'Especie nativa del Ecuador. Juega un papel importante en el equilibrio de los ecosistemas.', 'https://example.com/species/30011.jpg', true, 8, true, '2026-07-27 00:00:00', 4, NULL, NULL),
(30012, 'Mono aullador', 'Alouatta seniculus', 'Especie representativa de la fauna ecuatoriana. Se encuentra en peligro por pérdida de hábitat.', 'https://example.com/species/30012.jpg', false, 22, true, '2026-07-27 00:00:00', 5, NULL, NULL),
(30013, 'Perezoso de dos dedos', 'Choloepus hoffmanni', 'Especie emblemática de los bosques ecuatorianos. Habita en ecosistemas tropicales y subtropicales.', 'https://example.com/species/30013.jpg', false, 22, true, '2026-07-27 00:00:00', 6, NULL, NULL),
(30014, 'Tapir amazónico', 'Tapirus terrestris', 'Especie característica de la biodiversidad del Ecuador. Presente en varias regiones del país.', 'https://example.com/species/30014.jpg', false, 12, true, '2026-07-27 00:00:00', 3, NULL, NULL),
(30015, 'Armadillo gigante', 'Priodontes maximus', 'Especie nativa del Ecuador. Juega un papel importante en el equilibrio de los ecosistemas.', 'https://example.com/species/30015.jpg', false, 22, true, '2026-07-27 00:00:00', 2, NULL, NULL),
(30016, 'Boa constrictor', 'Boa constrictor', 'Especie representativa de la fauna ecuatoriana. Se encuentra en peligro por pérdida de hábitat.', 'https://example.com/species/30016.jpg', false, 30, true, '2026-07-27 00:00:00', 6, NULL, NULL),
(30017, 'Caimán de anteojos', 'Caiman crocodilus', 'Especie emblemática de los bosques ecuatorianos. Habita en ecosistemas tropicales y subtropicales.', 'https://example.com/species/30017.jpg', false, 30, true, '2026-07-27 00:00:00', 5, NULL, NULL),
(30018, 'Rana dardo dorado', 'Phyllobates terribilis', 'Especie característica de la biodiversidad del Ecuador. Presente en varias regiones del país.', 'https://example.com/species/30018.jpg', true, 35, true, '2026-07-27 00:00:00', 1, NULL, NULL),
(30019, 'Hormiga bala', 'Paraponera clavata', 'Especie nativa del Ecuador. Juega un papel importante en el equilibrio de los ecosistemas.', 'https://example.com/species/30019.jpg', false, 40, true, '2026-07-27 00:00:00', 7, NULL, NULL),
(30020, 'Escarabajo hércules', 'Dynastes hercules', 'Especie representativa de la fauna ecuatoriana. Se encuentra en peligro por pérdida de hábitat.', 'https://example.com/species/30020.jpg', false, 40, true, '2026-07-27 00:00:00', 6, NULL, NULL),
(30021, 'Murciélago vampiro', 'Desmodus rotundus', 'Especie emblemática de los bosques ecuatorianos. Habita en ecosistemas tropicales y subtropicales.', 'https://example.com/species/30021.jpg', false, 25, true, '2026-07-27 00:00:00', 6, NULL, NULL),
(30022, 'Lobo de páramo', 'Lycalopex culpaeus', 'Especie característica de la biodiversidad del Ecuador. Presente en varias regiones del país.', 'https://example.com/species/30022.jpg', false, 12, true, '2026-07-27 00:00:00', 5, NULL, NULL),
(30023, 'Venado cola blanca', 'Odocoileus virginianus', 'Especie nativa del Ecuador. Juega un papel importante en el equilibrio de los ecosistemas.', 'https://example.com/species/30023.jpg', false, 12, true, '2026-07-27 00:00:00', 6, NULL, NULL),
(30024, 'Ocelote', 'Leopardus pardalis', 'Especie representativa de la fauna ecuatoriana. Se encuentra en peligro por pérdida de hábitat.', 'https://example.com/species/30024.jpg', false, 12, true, '2026-07-27 00:00:00', 4, NULL, NULL),
(30025, 'Pecarí labiado', 'Tayassu pecari', 'Especie emblemática de los bosques ecuatorianos. Habita en ecosistemas tropicales y subtropicales.', 'https://example.com/species/30025.jpg', false, 12, true, '2026-07-27 00:00:00', 5, NULL, NULL),
(30026, 'Gavilán de galápagos', 'Buteo galapagoensis', 'Especie característica de la biodiversidad del Ecuador. Presente en varias regiones del país.', 'https://example.com/species/30026.jpg', true, 8, true, '2026-07-27 00:00:00', 3, NULL, NULL),
(30027, 'Pingüino de galápagos', 'Spheniscus mendiculus', 'Especie nativa del Ecuador. Juega un papel importante en el equilibrio de los ecosistemas.', 'https://example.com/species/30027.jpg', true, 8, true, '2026-07-27 00:00:00', 1, NULL, NULL),
(30028, 'Tortuga gigante', 'Chelonoidis niger', 'Especie representativa de la fauna ecuatoriana. Se encuentra en peligro por pérdida de hábitat.', 'https://example.com/species/30028.jpg', true, 50, true, '2026-07-27 00:00:00', 2, NULL, NULL),
(30029, 'Iguana marina', 'Amblyrhynchus cristatus', 'Especie emblemática de los bosques ecuatorianos. Habita en ecosistemas tropicales y subtropicales.', 'https://example.com/species/30029.jpg', true, 50, true, '2026-07-27 00:00:00', 3, NULL, NULL),
(30030, 'Lagarto lava', 'Microlophus albemarlensis', 'Especie característica de la biodiversidad del Ecuador. Presente en varias regiones del país.', 'https://example.com/species/30030.jpg', true, 50, true, '2026-07-27 00:00:00', 6, NULL, NULL),
(30031, 'Pinzón de darwin', 'Geospiza magnirostris', 'Especie nativa del Ecuador. Juega un papel importante en el equilibrio de los ecosistemas.', 'https://example.com/species/30031.jpg', true, 8, true, '2026-07-27 00:00:00', 5, NULL, NULL),
(30032, 'Cormorán no volador', 'Phalacrocorax harrisi', 'Especie representativa de la fauna ecuatoriana. Se encuentra en peligro por pérdida de hábitat.', 'https://example.com/species/30032.jpg', true, 8, true, '2026-07-27 00:00:00', 2, NULL, NULL),
(30033, 'Fragata real', 'Fregata magnificens', 'Especie emblemática de los bosques ecuatorianos. Habita en ecosistemas tropicales y subtropicales.', 'https://example.com/species/30033.jpg', false, 8, true, '2026-07-27 00:00:00', 6, NULL, NULL),
(30034, 'Piquero patas azules', 'Sula nebouxii', 'Especie característica de la biodiversidad del Ecuador. Presente en varias regiones del país.', 'https://example.com/species/30034.jpg', false, 8, true, '2026-07-27 00:00:00', 5, NULL, NULL),
(30035, 'Albatros de galápagos', 'Phoebastria irrorata', 'Especie nativa del Ecuador. Juega un papel importante en el equilibrio de los ecosistemas.', 'https://example.com/species/30035.jpg', true, 8, true, '2026-07-27 00:00:00', 1, NULL, NULL),
(30036, 'Garza azul', 'Egretta caerulea', 'Especie representativa de la fauna ecuatoriana. Se encuentra en peligro por pérdida de hábitat.', 'https://example.com/species/30036.jpg', false, 8, true, '2026-07-27 00:00:00', 6, NULL, NULL),
(30037, 'Flamenco rosado', 'Phoenicopterus ruber', 'Especie emblemática de los bosques ecuatorianos. Habita en ecosistemas tropicales y subtropicales.', 'https://example.com/species/30037.jpg', false, 8, true, '2026-07-27 00:00:00', 5, NULL, NULL),
(30038, 'Cigüeña de cabeza pelada', 'Mycteria americana', 'Especie característica de la biodiversidad del Ecuador. Presente en varias regiones del país.', 'https://example.com/species/30038.jpg', false, 8, true, '2026-07-27 00:00:00', 4, NULL, NULL),
(30039, 'Águila harpía', 'Harpia harpyja', 'Especie nativa del Ecuador. Juega un papel importante en el equilibrio de los ecosistemas.', 'https://example.com/species/30039.jpg', false, 8, true, '2026-07-27 00:00:00', 3, NULL, NULL),
(30040, 'Halcón peregrino', 'Falco peregrinus', 'Especie representativa de la fauna ecuatoriana. Se encuentra en peligro por pérdida de hábitat.', 'https://example.com/species/30040.jpg', false, 8, true, '2026-07-27 00:00:00', 6, NULL, NULL),
(30041, 'Lechuza campanaria', 'Tyto alba', 'Especie emblemática de los bosques ecuatorianos. Habita en ecosistemas tropicales y subtropicales.', 'https://example.com/species/30041.jpg', false, 8, true, '2026-07-27 00:00:00', 6, NULL, NULL),
(30042, 'Búho andino', 'Bubo virginianus', 'Especie característica de la biodiversidad del Ecuador. Presente en varias regiones del país.', 'https://example.com/species/30042.jpg', false, 8, true, '2026-07-27 00:00:00', 5, NULL, NULL),
(30043, 'Quetzal ecuatoriano', 'Pharomachrus antisianus', 'Especie nativa del Ecuador. Juega un papel importante en el equilibrio de los ecosistemas.', 'https://example.com/species/30043.jpg', true, 8, true, '2026-07-27 00:00:00', 4, NULL, NULL),
(30044, 'Saltarín coronidorado', 'Manacus manacus', 'Especie representativa de la fauna ecuatoriana. Se encuentra en peligro por pérdida de hábitat.', 'https://example.com/species/30044.jpg', false, 8, true, '2026-07-27 00:00:00', 6, NULL, NULL),
(30045, 'Mielero andino', 'Diglossa lafresnayii', 'Especie emblemática de los bosques ecuatorianos. Habita en ecosistemas tropicales y subtropicales.', 'https://example.com/species/30045.jpg', true, 8, true, '2026-07-27 00:00:00', 5, NULL, NULL),
(30046, 'Tangara azulada', 'Tangara mexicana', 'Especie característica de la biodiversidad del Ecuador. Presente en varias regiones del país.', 'https://example.com/species/30046.jpg', false, 8, true, '2026-07-27 00:00:00', 6, NULL, NULL),
(30047, 'Cacique lomiamarillo', 'Cacicus cela', 'Especie nativa del Ecuador. Juega un papel importante en el equilibrio de los ecosistemas.', 'https://example.com/species/30047.jpg', false, 8, true, '2026-07-27 00:00:00', 6, NULL, NULL),
(30048, 'Oropéndola crestada', 'Psarocolius decumanus', 'Especie representativa de la fauna ecuatoriana. Se encuentra en peligro por pérdida de hábitat.', 'https://example.com/species/30048.jpg', false, 8, true, '2026-07-27 00:00:00', 5, NULL, NULL),
(30049, 'Colibrí zamarrito', 'Heliodoxa leadbeateri', 'Especie emblemática de los bosques ecuatorianos. Habita en ecosistemas tropicales y subtropicales.', 'https://example.com/species/30049.jpg', true, 15, true, '2026-07-27 00:00:00', 4, NULL, NULL),
(30050, 'Paloma santa marta', 'Patagioenas fasciata', 'Especie característica de la biodiversidad del Ecuador. Presente en varias regiones del país.', 'https://example.com/species/30050.jpg', false, 8, true, '2026-07-27 00:00:00', 6, NULL, NULL);

-- Consulta de comprobación (1,000,000+ registros)
-- SELECT
--     (SELECT COUNT(*) FROM public."Species") +
--     (SELECT COUNT(*) FROM public."Families") +
--     (SELECT COUNT(*) FROM public."Records") +
--     (SELECT COUNT(*) FROM public."Locations") +
--     (SELECT COUNT(*) FROM public."Researchers") +
--     (SELECT COUNT(*) FROM public."NaturalReserves") +
--     (SELECT COUNT(*) FROM public."ConservationStatuses") +
--     (SELECT COUNT(*) FROM public."Publications") +
--     (SELECT COUNT(*) FROM public."PublicationSpecies") +
--     (SELECT COUNT(*) FROM public."Courses") +
--     (SELECT COUNT(*) FROM public."PhysicalProducts") +
--     (SELECT COUNT(*) FROM public."Sucursales") +
--     (SELECT COUNT(*) FROM public."Orders") +
--     (SELECT COUNT(*) FROM public."OrderDetails") +
--     (SELECT COUNT(*) FROM public."Enrollments") +
--     (SELECT COUNT(*) FROM public."Payments") +
--     (SELECT COUNT(*) FROM public."InventoryMovements") +
--     (SELECT COUNT(*) FROM public."PendingEmails")
--     AS total_registros;
