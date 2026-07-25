-- ============================================================================
-- SEED DATA: Cursos y Productos BioGama Ecuador
-- Precios realistas para el mercado ecuatoriano (USD)
-- No ejecutar si ya existen datos (usa INSERT OR IGNORE / ON CONFLICT)
-- ============================================================================

-- ============================================================================
-- 30 CURSOS Y TALLERES
-- ============================================================================

INSERT INTO "Courses" ("Id", "Title", "Description", "Syllabus", "Price", "TotalSeats", "ReservedSeats", "ConfirmedSeats", "StartDate", "EndDate", "StartTime", "EndTime", "Modality", "Venue", "Instructor", "InstructorBio", "ImageUrl", "IsActive", "RequiresPriorKnowledge", "TargetAudience", "SpeciesId", "CreatedAt", "UpdatedAt", "DeletedAt")
VALUES
-- Cursos de Biodiversidad General
('a1000001-0000-4000-8000-000000000001', 'Introducción a la Biodiversidad del Ecuador', 'Curso fundamental sobre los ecosistemas ecuatorianos: costa, sierra, amazonía y galápagos. Ideal para principiantes.', 'Módulo 1: Ecorregiones del Ecuador\nMódulo 2: Flora representativa\nMódulo 3: Fauna emblemática\nMódulo 4: Conservación in-situ', 35.00, 30, 0, 0, '2026-09-15', '2026-09-19', '09:00:00', '13:00:00', 'Presencial', 'Centro de Interpretación – Quito', 'Dra. María Fernanda Espinoza', 'Bióloga tropical con 15 años de experiencia en investigación de ecosistemas andinos. PhD en Biología de la Conservación.', 'https://picsum.photos/seed/course-introduccin-a-la-biodiversidad-del-ecuador/800/400', true, false, 'Público general, estudiantes, turistas', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-000000000002', 'Biodiversidad Amazónica: Identificación de Especies', 'Sumérgete en la selva amazónica y aprende a identificar sus especies más representativas.', 'Teoría: Ecosistemas amazónicos\nPráctica: Recorrido guiado\nTaxonomía básica\nEspecies en peligro', 55.00, 20, 0, 0, '2026-10-05', '2026-10-09', '08:00:00', '16:00:00', 'Presencial', 'Estación Científica Yasuní – Orellana', 'Dr. Pablo Andrés Rivadeneira', 'Investigador asociado a la Estación de Biodiversidad Tiputini. Ha descubierto 3 nuevas especies de anfibios.', 'https://picsum.photos/seed/course-biodiversidad-amaznica-identificacin-de-especies/800/400', true, true, 'Estudiantes de biología, ecoturismo', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-000000000003', 'Ecosistemas Altoandinos: Páramos y Glaciares', 'Conoce los frágiles ecosistemas de páramo y su importancia hídrica para el país.', 'Geología andina\nFlora de páramo (frailejones, pajonales)\nFauna altoandina\nCambio climático y glaciares', 40.00, 25, 0, 0, '2026-11-03', '2026-11-07', '09:00:00', '14:00:00', 'Presencial', 'Reserva Ecológica Cayambe-Coca – Cayambe', 'Msc. Catalina León', 'Ingeniera ambiental especializada en restauración de páramos. Trabajó 8 años en el Fondo para la Protección del Agua (FONAG).', 'https://picsum.photos/seed/course-ecosistemas-altoandinos-pramos-y-glaciares/800/400', true, false, 'Profesionales ambientales, guías, estudiantes', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-000000000004', 'Bosque Seco Tropical: Conservación y Restauración', 'Aprende sobre el ecosistema más amenazado del Ecuador y las estrategias para su recuperación.', 'Ecología del bosque seco\nEspecies endémicas\nTécnicas de restauración\nCasos de éxito en Manabí', 30.00, 30, 0, 0, '2026-08-10', '2026-08-14', '08:30:00', '12:30:00', 'Presencial', 'Reserva Ecológica Arenillas – El Oro', 'Ing. José Miguel Álava', 'Ingeniero forestal con 12 años en proyectos de restauración ecológica en la costa ecuatoriana.', 'https://picsum.photos/seed/course-bosque-seco-tropical-conservacin-y-restauracin/800/400', true, false, 'Estudiantes, guardaparques, comunidades locales', NULL, '2026-07-25', NULL, NULL),

-- Ornitología
('a1000001-0000-4000-8000-000000000005', 'Aves del Ecuador: Identificación y Monitoreo', 'Ecuador tiene más de 1600 especies de aves. Aprende a identificarlas y monitorearlas.', 'Introducción a la ornitología\nUso de binoculares y guías\nIdentificación por cantos\nTécnicas de anillamiento', 65.00, 18, 0, 0, '2026-09-22', '2026-09-26', '06:00:00', '15:00:00', 'Presencial', 'Reserva Antisana – Napo', 'Dr. Diego Fernando Ortiz', 'Ornitólogo con 20 años de experiencia. Autor de "Aves de los Andes Ecuatorianos". Coordinador del Censo Neotropical de Aves.', 'https://picsum.photos/seed/course-aves-del-ecuador-identificacin-y-monitoreo/800/400', true, false, 'Birdwatchers, guías turísticos, biólogos', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-000000000006', 'Colibríes del Ecuador: Ecología y Conservación', 'Los colibríes son joyas de nuestros bosques. Conoce su biología y cómo protegerlos.', 'Biología de troquilinos\nAdaptaciones morfológicas\nJardines para colibríes\nEspecies amenazadas', 45.00, 22, 0, 0, '2026-10-19', '2026-10-23', '09:00:00', '13:00:00', 'Presencial', 'Jardín Botánico de Quito', 'Msc. Ana Belén Suárez', 'Bióloga especializada en polinizadores. Su investigación sobre colibríes del noroccidente de Pichincha ha sido publicada en 3 revistas internacionales.', 'https://picsum.photos/seed/course-colibres-del-ecuador-ecologa-y-conservacin/800/400', true, true, 'Fotógrafos, jardineros, biólogos', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-000000000007', 'Aves Rapaces del Ecuador', 'Descubre el fascinante mundo de las águilas, halcones y búhos ecuatorianos.', 'Identificación de rapaces diurnas y nocturnas\nBiología y comportamiento\nTécnicas de avistamiento\nConservación de rapaces', 50.00, 20, 0, 0, '2026-11-16', '2026-11-20', '07:00:00', '14:00:00', 'Presencial', 'Reserva Geobotánica Pululahua – Pichincha', 'Dr. Fernando Espinoza', 'Investigador de rapaces andinas. Miembro del Grupo de Trabajo de Rapaces Neotropicales.', 'https://picsum.photos/seed/course-aves-rapaces-del-ecuador/800/400', true, true, 'Biologos, guardaparques, fotógrafos', NULL, '2026-07-25', NULL, NULL),

-- Botánica
('a1000001-0000-4000-8000-000000000008', 'Orquídeas del Ecuador: Cultivo y Conservación', 'Ecuador tiene más de 4000 especies de orquídeas. Aprende a cultivarlas y conservarlas.', 'Morfología de orquídeas\nEspecies ecuatorianas emblemáticas\nTécnicas de cultivo y propagación\nEspecies amenazadas', 60.00, 25, 0, 0, '2026-09-08', '2026-09-12', '10:00:00', '15:00:00', 'Presencial', 'Orquideario del Jardín Botánico – Quito', 'Ing. Patricia Jiménez', 'Ingeniera agrónoma especializada en orquídeas. Presidenta de la Asociación Ecuatoriana de Orquideología.', 'https://picsum.photos/seed/course-orqudeas-del-ecuador-cultivo-y-conservacin/800/400', true, false, 'Jardineros, aficionados a orquídeas, estudiantes', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-000000000009', 'Plantas Medicinales Tradicionales', 'Saberes ancestrales y ciencia moderna en el uso de plantas curativas ecuatorianas.', 'Etnobotánica ecuatoriana\nPlantas medicinales de la sierra\nPlantas de la amazonía\nPreparación y usos seguros', 25.00, 35, 0, 0, '2026-08-24', '2026-08-28', '09:00:00', '12:00:00', 'Presencial', 'Mercado de Plantas Medicinales – Otavalo', 'Dra. Rosa Elena Quishpe', 'Médica tradicional kichwa y botánica. Combina saberes ancestrales con investigación científica.', 'https://picsum.photos/seed/course-plantas-medicinales-tradicionales/800/400', true, false, 'Público general, terapeutas, estudiantes', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-00000000000A', 'Dendrología: Identificación de Árboles Nativos', 'Aprende a identificar los árboles nativos del Ecuador por sus hojas, cortezas y frutos.', 'Morfología forestal\nClaves dicotómicas\nÁrboles de la costa\nÁrboles de la sierra\nÁrboles amazónicos', 48.00, 20, 0, 0, '2026-10-12', '2026-10-16', '08:00:00', '14:00:00', 'Presencial', 'Bosque Protector Mindo – Pichincha', 'Ing. Francisco Troya', 'Ingeniero forestal con maestría en dendrología tropical. Ha catalogado más de 300 especies arbóreas.', 'https://picsum.photos/seed/course-dendrologa-identificacin-de-rboles-nativos/800/400', true, true, 'Ingenieros forestales, estudiantes, guardaparques', NULL, '2026-07-25', NULL, NULL),

-- Herpetología
('a1000001-0000-4000-8000-00000000000B', 'Anfibios del Ecuador: Ranas y Sapos', 'Ecuador es el tercer país con más anfibios del mundo. Conoce su diversidad y amenazas.', 'Taxonomía de anfibios\nEspecies emblemáticas\nQuitridiomicosis y conservación\nMonitoreo de poblaciones', 42.00, 20, 0, 0, '2026-09-29', '2026-10-03', '09:00:00', '15:00:00', 'Presencial', 'Centro Científico Río Guajalito – Pichincha', 'Msc. Santiago Ron', 'Herpetólogo con 18 años de investigación. Curador de la colección de anfibios del QCAZ.', 'https://picsum.photos/seed/course-anfibios-del-ecuador-ranas-y-sapos/800/400', true, true, 'Biólogos, veterinarios, estudiantes', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-00000000000C', 'Serpientes del Ecuador: Mitos y Realidad', 'Desmitifica creencias populares y conoce la verdadera importancia ecológica de las serpientes.', 'Identificación de serpientes\nSerpientes venenosas vs. no venenosas\nPrimeros auxilios por mordedura\nConservación de ofidios', 35.00, 25, 0, 0, '2026-11-09', '2026-11-13', '10:00:00', '13:00:00', 'Presencial', 'Vivarium de Quito', 'Biól. Karina Salazar', 'Bióloga especializada en herpetofauna. Coordinadora del centro de rescate de ofidios de Quito.', 'https://picsum.photos/seed/course-serpientes-del-ecuador-mitos-y-realidad/800/400', true, true, 'Público general, guías turísticos, bomberos', NULL, '2026-07-25', NULL, NULL),

-- Mastozoología
('a1000001-0000-4000-8000-00000000000D', 'Mamíferos del Ecuador: Grandes y Pequeños', 'Desde el tapir amazónico hasta el murciélago más pequeño. Un recorrido por los mamíferos ecuatorianos.', 'Taxonomía de mamíferos\nMamíferos amenazados\nTécnicas de muestreo\nUso de cámaras trampa', 55.00, 18, 0, 0, '2026-10-26', '2026-10-30', '06:00:00', '16:00:00', 'Presencial', 'Parque Nacional Podocarpus – Zamora', 'Dr. Luis Albuja', 'Mastozoólogo pionero en Ecuador. Autor del libro "Mamíferos del Ecuador".', 'https://picsum.photos/seed/course-mamferos-del-ecuador-grandes-y-pequeos/800/400', true, true, 'Biólogos, veterinarios, guardaparques', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-00000000000E', 'Murciélagos: Aliados Nocturnos', 'Los murciélagos son esenciales para los ecosistemas. Aprende sobre su biología y conservación.', 'Ecología de quirópteros\nPolinización y dispersión de semillas\nMitos y verdades\nMétodos de captura y muestreo', 38.00, 20, 0, 0, '2026-08-17', '2026-08-21', '17:00:00', '22:00:00', 'Presencial', 'Reserva Maquipucuna – Pichincha', 'Msc. Jaime Velarde', 'Especialista en murciélagos neotropicales. Ha identificado nuevas especies en la costa ecuatoriana.', 'https://picsum.photos/seed/course-murcilagos-aliados-nocturnos/800/400', true, true, 'Biólogos, estudiantes, educadores ambientales', NULL, '2026-07-25', NULL, NULL),

-- Ento / Invertebrados
('a1000001-0000-4000-8000-00000000000F', 'Mariposas del Ecuador', 'Un país con más de 4000 especies de mariposas. Técnicas de identificación y cría.', 'Ciclo de vida de lepidópteros\nEspecies emblemáticas\nTécnicas de captura y colección\nMariposarios y conservación', 40.00, 25, 0, 0, '2026-12-01', '2026-12-05', '09:00:00', '13:00:00', 'Presencial', 'Mariposario Mindo – Pichincha', 'Ing. Gabriela Monteros', 'Ingeniera agroecológica. Fundadora del Mariposario Mindo, el más grande del Ecuador.', 'https://picsum.photos/seed/course-mariposas-del-ecuador/800/400', true, false, 'Público general, fotógrafos, estudiantes', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-000000000010', 'Escarabajos del Ecuador: Biodiversidad Oculta', 'Los coleópteros son el orden más diverso. Descubre su increíble variedad en Ecuador.', 'Introducción a la coleopterología\nEscarabajos coprófagos\nEscarabajos de la amazonía\nTécnicas de colección', 32.00, 20, 0, 0, '2027-01-12', '2027-01-16', '10:00:00', '14:00:00', 'Presencial', 'Estación Científica Jatun Sacha – Napo', 'PhD. Giovanni Onore', 'Entomólogo italo-ecuatoriano. Fundador del Museo de Entomología de la PUCE. 40 años de investigación.', 'https://picsum.photos/seed/course-escarabajos-del-ecuador-biodiversidad-oculta/800/400', true, true, 'Entomólogos, estudiantes, agricultores', NULL, '2026-07-25', NULL, NULL),

-- Conservación y Sostenibilidad
('a1000001-0000-4000-8000-000000000011', 'Restauración Ecológica de Bosques', 'Técnicas prácticas para restaurar bosques degradados en diferentes zonas del Ecuador.', 'Principios de restauración\nViveros forestales\nEspecies nativas para restauración\nMonitoreo y evaluación', 70.00, 25, 0, 0, '2026-11-23', '2026-11-27', '08:00:00', '16:00:00', 'Presencial', 'Hacienda Zurita – Intag, Imbabura', 'Ing. Felipe Rosero', 'Especialista en restauración ecológica con proyectos en 6 provincias del Ecuador.', 'https://picsum.photos/seed/course-restauracin-ecolgica-de-bosques/800/400', true, true, 'Ingenieros forestales, ambientales', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-000000000012', 'Cambio Climático y Conservación en Ecuador', 'Impactos del cambio climático en los ecosistemas ecuatorianos y estrategias de adaptación.', 'Ciencia del clima\nImpactos en ecosistemas andinos\nImpactos en la costa\nEstrategias de adaptación', 45.00, 30, 0, 0, '2026-08-31', '2026-09-04', '09:00:00', '13:00:00', 'Virtual', 'Zoom – En vivo desde Quito', 'Msc. Daniela Merchán', 'Climatóloga. Trabajó 10 años en el INAMHI. Autora de informes del IPCC para la región andina.', 'https://picsum.photos/seed/course-cambio-climtico-y-conservacin-en-ecuador/800/400', true, true, 'Profesionales ambientales, tomadores de decisión', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-000000000013', 'Ecoturismo Comunitario Sostenible', 'Aprende a diseñar y gestionar proyectos de turismo ecológico con comunidades locales.', 'Fundamentos del ecoturismo\nDiseño de experiencias\nGestión comunitaria\nMarketing sostenible', 85.00, 22, 0, 0, '2026-09-19', '2026-10-03', '09:00:00', '16:00:00', 'Presencial', 'Comunidad Kichwa Añangu – Napo', 'Lic. Elena Torres', 'Guía naturalista certificada. Gerente del lodge comunitario Añangu, galardonado internacionalmente.', 'https://picsum.photos/seed/course-ecoturismo-comunitario-sostenible/800/400', true, true, 'Emprendedores turísticos, comunidades, guías', NULL, '2026-07-25', NULL, NULL),

-- Agricultura y Permacultura
('a1000001-0000-4000-8000-000000000014', 'Permacultura y Diseño Sostenible', 'Principios de permacultura aplicados al contexto ecuatoriano.', 'Ética y principios de permacultura\nDiseño de fincas integradas\nSuelos y compostaje\nSistemas agroforestales', 75.00, 20, 0, 0, '2026-10-17', '2026-10-21', '08:00:00', '17:00:00', 'Presencial', 'Finca Permacultura Sumak Kawsay – Tumbaco', 'Ing. Xavier Ordóñez', 'Diseñador en permacultura certificado. Ha implementado proyectos en Ecuador, Perú y Colombia.', 'https://picsum.photos/seed/course-permacultura-y-diseo-sostenible/800/400', true, true, 'Agricultores, arquitectos, ecologistas', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-000000000015', 'Agroecología Urbana: Huertos en Casa', 'Aprende a cultivar tus propios alimentos en espacios urbanos con técnicas ecológicas.', 'Diseño de huertos urbanos\nSuelos y abonos orgánicos\nSemillas y plantación\nControl natural de plagas', 28.00, 40, 0, 0, '2026-11-02', '2026-11-06', '10:00:00', '12:00:00', 'Presencial', 'Huerto Comunitario La Floresta – Quito', 'Ing. María José Padilla', 'Agroecóloga urbana. Coordina la red de huertos comunitarios de Quito.', 'https://picsum.photos/seed/course-agroecologa-urbana-huertos-en-casa/800/400', true, false, 'Público general interesado en agricultura urbana', NULL, '2026-07-25', NULL, NULL),

-- Vida Silvestre y Fauna
('a1000001-0000-4000-8000-000000000016', 'Manejo y Rehabilitación de Fauna Silvestre', 'Técnicas para el rescate, rehabilitación y liberación de animales silvestres.', 'Legislación ambiental\nPrimeros auxilios en fauna\nNutrición y cuidados\nReintroducción y monitoreo', 120.00, 15, 0, 0, '2026-09-28', '2026-10-02', '08:00:00', '17:00:00', 'Presencial', 'Centro de Rescate de Fauna Silvestre – Guayllabamba', 'MVZ. Paulina Almeida', 'Médica veterinaria especializada en fauna silvestre. Dirige el centro de rescate más grande del país.', 'https://picsum.photos/seed/course-manejo-y-rehabilitacin-de-fauna-silvestre/800/400', true, true, 'Veterinarios, biólogos, guardaparques', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-000000000017', 'Fotografía de Naturaleza y Conservación', 'Técnicas fotográficas para capturar la belleza de la naturaleza y apoyar la conservación.', 'Equipamiento básico\nComposición y luz\nFotografía de aves\nFotografía macro\nÉtica del fotógrafo naturalista', 90.00, 18, 0, 0, '2026-12-07', '2026-12-11', '05:30:00', '15:00:00', 'Presencial', 'Reserva Yanacocha – Pichincha', 'Pablo Andrés Valdez', 'Fotógrafo de naturaleza galardonado. Colaborador de National Geographic y WWF Ecuador.', 'https://picsum.photos/seed/course-fotografa-de-naturaleza-y-conservacin/800/400', true, true, 'Fotógrafos aficionados y profesionales', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-000000000018', 'Hongos del Ecuador: Micología Básica', 'Descubre el fascinante reino fungi en los bosques ecuatorianos.', 'Introducción a la micología\nHongos comestibles y tóxicos\nTécnicas de recolección\nHongos y cambio climático', 38.00, 20, 0, 0, '2027-02-08', '2027-02-12', '09:00:00', '14:00:00', 'Presencial', 'Bosque Protector Pasochoa – Pichincha', 'Msc. Andrea Cevallos', 'Micóloga. Investigadora asociada al Herbario Nacional del Ecuador.', 'https://picsum.photos/seed/course-hongos-del-ecuador-micologa-bsica/800/400', true, false, 'Público general, chefs, biólogos', NULL, '2026-07-25', NULL, NULL),

-- Cursos prácticos / talleres
('a1000001-0000-4000-8000-000000000019', 'Elaboración de Productos Naturales', 'Taller práctico para crear cosméticos y productos de limpieza naturales y sostenibles.', 'Ingredientes naturales\nJabones artesanales\nCremas y ungüentos\nProductos de limpieza ecológicos', 35.00, 25, 0, 0, '2026-12-14', '2026-12-18', '10:00:00', '13:00:00', 'Presencial', 'Centro de Emprendimiento – Quito', 'Lic. Valeria Cárdenas', 'Química farmacéutica. Emprendedora de la línea de cosmética natural "Yurak".', 'https://picsum.photos/seed/course-elaboracin-de-productos-naturales/800/400', true, false, 'Público general, emprendedores', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-00000000001A', 'Bioconstrucción con Materiales Locales', 'Técnicas de construcción sostenible usando bambú, bahareque y tierra.', 'Materiales naturales\nCimentaciones y estructura\nBambú y caña guadúa\nAcabados naturales', 150.00, 15, 0, 0, '2027-01-18', '2027-01-22', '08:00:00', '17:00:00', 'Presencial', 'Centro de Bioconstrucción – Vilcabamba', 'Arq. Mateo Vizcaíno', 'Arquitecto bioclimático. Ha construido más de 50 viviendas sostenibles en Ecuador.', 'https://picsum.photos/seed/course-bioconstruccin-con-materiales-locales/800/400', true, true, 'Arquitectos, constructores, autoconstructores', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-00000000001B', 'Manejo Integrado de Plagas en Agricultura', 'Alternativas ecológicas al control químico de plagas para pequeños y medianos agricultores.', 'Identificación de plagas\nControl biológico\nPreparados botánicos\nMonitoreo y prevención', 42.00, 25, 0, 0, '2026-11-30', '2026-12-04', '09:00:00', '14:00:00', 'Presencial', 'Centro Agrícola de Machachi – Pichincha', 'Ing. Raúl Vaca', 'Ingeniero agrónomo especializado en control biológico de plagas.', 'https://picsum.photos/seed/course-manejo-integrado-de-plagas-en-agricultura/800/400', true, true, 'Agricultores, técnicos agrícolas', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-00000000001C', 'Sistemas Agroforestales en la Amazonía', 'Modelos productivos que integran cultivos, árboles y conservación en la amazonía ecuatoriana.', 'Diseño de SAF\nCultivos asociados (café, cacao, frutales)\nEspecies forestales\nMercados sostenibles', 65.00, 20, 0, 0, '2027-02-22', '2027-02-26', '08:00:00', '15:00:00', 'Presencial', 'Centro de Investigación INIAP – Napo', 'PhD. Juan Carlos Palacios', 'Investigador en agroforestería amazónica con 25 años de experiencia en la región.', 'https://picsum.photos/seed/course-sistemas-agroforestales-en-la-amazona/800/400', true, true, 'Agricultores amazónicos, técnicos', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-00000000001D', 'Monitoreo de Calidad de Agua', 'Técnicas de campo para evaluar la calidad del agua en ríos y lagos ecuatorianos.', 'Parámetros físico-químicos\nToma de muestras\nMacroinvertebrados como bioindicadores\nInterpretación de resultados', 80.00, 18, 0, 0, '2027-03-08', '2027-03-12', '08:00:00', '16:00:00', 'Presencial', 'Cuenca Alta del Río Pita – Pichincha', 'Msc. Verónica Andrade', 'Ingeniera hidrológica. Especialista en monitoreo de cuencas hidrográficas.', 'https://picsum.photos/seed/course-monitoreo-de-calidad-de-agua/800/400', true, true, 'Técnicos ambientales, estudiantes, guardianes del agua', NULL, '2026-07-25', NULL, NULL),

('a1000001-0000-4000-8000-00000000001E', 'Dibujo Científico de la Biodiversidad', 'Técnicas de ilustración científica aplicadas a la documentación de especies ecuatorianas.', 'Materiales y técnicas\nProporciones y anatomía\nIlustración botánica\nIlustración zoológica', 48.00, 20, 0, 0, '2027-04-05', '2027-04-09', '10:00:00', '14:00:00', 'Presencial', 'Museo de Historia Natural Gustavo Orcés – Quito', 'Ilust. David Quizhpe', 'Ilustrador científico. Trabaja para el Museo de Historia Natural y revistas científicas internacionales.', 'https://picsum.photos/seed/course-dibujo-cientfico-de-la-biodiversidad/800/400', true, false, 'Artistas, biólogos, diseñadores', NULL, '2026-07-25', NULL, NULL);

-- ============================================================================
-- 40 PRODUCTOS FÍSICOS
-- ============================================================================

INSERT INTO "PhysicalProducts" ("Id", "Name", "Description", "Price", "Stock", "ReservedStock", "MinStock", "SKU", "ImageUrl", "IsActive", "SpeciesId", "CreatedAt", "UpdatedAt", "DeletedAt")
VALUES

-- Guías de campo y libros (10)
('b2000001-0000-4000-8000-000000000001', 'Guía de Aves del Ecuador – Tomo I', 'Guía ilustrada con 416 especies de aves de la sierra y los valles interandinos. Incluye mapas de distribución y cantos.', 28.50, 80, 0, 10, 'LIB-001', 'https://picsum.photos/seed/product-gua-de-aves-del-ecuador-tomo-i/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000002', 'Guía de Aves del Ecuador – Tomo II', 'Segundo tomo con 540 especies de la amazonía y oriente ecuatoriano. Más de 600 fotografías a color.', 32.00, 60, 0, 10, 'LIB-002', 'https://picsum.photos/seed/product-gua-de-aves-del-ecuador-tomo-ii/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000003', 'Guía de Orquídeas del Ecuador', '120 especies de orquídeas ecuatorianas fotografiadas en su hábitat natural. Incluye claves de identificación.', 35.00, 45, 0, 8, 'LIB-003', 'https://picsum.photos/seed/product-gua-de-orqudeas-del-ecuador/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000004', 'Guía de Anfibios del Ecuador', '200 especies de ranas, sapos y salamandras. Incluye cantos descargables mediante código QR.', 30.00, 50, 0, 10, 'LIB-004', 'https://picsum.photos/seed/product-gua-de-anfibios-del-ecuador/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000005', 'Árboles Nativos del Ecuador – Guía de Campo', 'Reconocimiento de 150 especies arbóreas nativas. Incluye corteza, hojas, flores y frutos.', 26.00, 55, 0, 8, 'LIB-005', 'https://picsum.photos/seed/product-rboles-nativos-del-ecuador-gua-de-campo/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000006', 'Mariposas del Ecuador – Guía Visual', 'Álbum fotográfico con 300 especies de mariposas diurnas del Ecuador continental.', 34.00, 40, 0, 5, 'LIB-006', 'https://picsum.photos/seed/product-mariposas-del-ecuador-gua-visual/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000007', 'Plantas Medicinales del Ecuador', 'Compilación de 80 plantas medicinales tradicionales con usos, preparación y bases científicas.', 22.00, 70, 0, 10, 'LIB-007', 'https://picsum.photos/seed/product-plantas-medicinales-del-ecuador/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000008', 'Mapa de Ecorregiones del Ecuador', 'Mapa plegable a color 1:500,000 con todas las ecorregiones, áreas protegidas y puntos de interés.', 12.00, 100, 0, 15, 'MAP-001', 'https://picsum.photos/seed/product-mapa-de-ecorregiones-del-ecuador/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000009', 'Mapa de Áreas Protegidas del Ecuador', 'Mapa detallado de las 56 áreas protegidas del Sistema Nacional. Incluye accesos y actividades.', 14.50, 80, 0, 10, 'MAP-002', 'https://picsum.photos/seed/product-mapa-de-reas-protegidas-del-ecuador/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-00000000000A', 'Cuaderno de Campo del Naturalista', 'Cuaderno de 200 páginas en papel reciclado con secciones para dibujo, observaciones, coordenadas y checklist.', 9.50, 150, 0, 20, 'ACC-001', 'https://picsum.photos/seed/product-cuaderno-de-campo-del-naturalista/400/400', true, NULL, '2026-07-25', NULL, NULL),

-- Binoculares y óptica (5)
('b2000001-0000-4000-8000-00000000000B', 'Binoculares de Avistamiento 8x42', 'Binoculares ligeros con lentes multirecubiertas, ideales para birdwatching. Impermeables y antiempañantes.', 85.00, 25, 0, 5, 'OPT-001', 'https://picsum.photos/seed/product-binoculares-de-avistamiento-8x42/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-00000000000C', 'Binoculares Compactos 10x25', 'Binoculares de bolsillo ideales para excursiones. Peso: 290g. Incluye funda y correa.', 45.00, 30, 0, 5, 'OPT-002', 'https://picsum.photos/seed/product-binoculares-compactos-10x25/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-00000000000D', 'Lupa de Campo 20x con LED', 'Lupa plegable con iluminación LED incorporada. Ideal para identificación de plantas e insectos.', 18.00, 60, 0, 10, 'OPT-003', 'https://picsum.photos/seed/product-lupa-de-campo-20x-con-led/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-00000000000E', 'Monocular 12x50 con Trípode', 'Monocular de largo alcance con trípode de bolsillo. Ideal para avistamiento de fauna a distancia.', 55.00, 20, 0, 5, 'OPT-004', 'https://picsum.photos/seed/product-monocular-12x50-con-trpode/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-00000000000F', 'Microscopio Portátil 60x-120x', 'Microscopio de bolsillo con zoom ajustable y luz LED. Perfecto para exploración en campo.', 32.00, 35, 0, 8, 'OPT-005', 'https://picsum.photos/seed/product-microscopio-porttil-60x120x/400/400', true, NULL, '2026-07-25', NULL, NULL),

-- Kits de campo (5)
('b2000001-0000-4000-8000-000000000010', 'Kit de Muestreo Entomológico', 'Red entomológica, frascos colectores, pinzas, etiquetas y guía rápida de identificación en estuche.', 42.00, 30, 0, 5, 'KIT-001', 'https://picsum.photos/seed/product-kit-de-muestreo-entomolgico/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000011', 'Kit de Colección Botánica', 'Prensa botánica portátil, tijeras de podar, etiquetas, bolsas de papel y guía de herborización.', 38.00, 25, 0, 5, 'KIT-002', 'https://picsum.photos/seed/product-kit-de-coleccin-botnica/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000012', 'Kit de Monitoreo de Agua', 'Equipo portátil para medir pH, temperatura, turbidez y oxígeno disuelto. Incluye guía de campo.', 65.00, 18, 0, 4, 'KIT-003', 'https://picsum.photos/seed/product-kit-de-monitoreo-de-agua/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000013', 'Kit de Supervivencia en el Bosque', 'Cuchillo multiusos, brújula, silbato, encendedor, kit de primeros auxilios, cuerda y bolsa impermeable.', 28.00, 40, 0, 8, 'KIT-004', 'https://picsum.photos/seed/product-kit-de-supervivencia-en-el-bosque/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000014', 'Kit de Fotografía Macro', 'Anillo adaptador para smartphone, lente macro 15x, mini trípode flexible y difusor de luz.', 36.00, 35, 0, 8, 'KIT-005', 'https://picsum.photos/seed/product-kit-de-fotografa-macro/400/400', true, NULL, '2026-07-25', NULL, NULL),

-- Ropa y accesorios (6)
('b2000001-0000-4000-8000-000000000015', 'Camiseta BioGama – Colibrí', 'Camiseta de algodón orgánico con diseño de colibrí serrano bordado. Varios colores.', 18.00, 120, 0, 15, 'ROP-001', 'https://picsum.photos/seed/product-camiseta-biogama-colibr/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000016', 'Camiseta BioGama – Jaguar', 'Camiseta de algodón orgánico con diseño de jaguar amazónico estampado. Varios colores.', 18.00, 100, 0, 15, 'ROP-002', 'https://picsum.photos/seed/product-camiseta-biogama-jaguar/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000017', 'Gorra BioGama Explorador', 'Gorra ajustable con visera curva y logo bordado. Protección UV. Colores verde bosque y caqui.', 14.00, 80, 0, 10, 'ROP-003', 'https://picsum.photos/seed/product-gorra-biogama-explorador/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000018', 'Mochila Plegable 20L', 'Mochila ultraligera plegable de 20 litros. Ideal para excursiones de un día. Resistente al agua.', 22.00, 50, 0, 10, 'ACC-002', 'https://picsum.photos/seed/product-mochila-plegable-20l/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000019', 'Botella Reutilizable de Bambú', 'Botella térmica de 500ml con recubrimiento de bambú natural. Libre de plástico BPA.', 15.00, 90, 0, 15, 'ECO-001', 'https://picsum.photos/seed/product-botella-reutilizable-de-bamb/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-00000000001A', 'Bolsa de Tela Plegable BioGama', 'Bolsa de tela reutilizable con diseño de hojas. Capacidad 15L. Plegable en su propio bolsillo.', 5.50, 200, 0, 30, 'ECO-002', 'https://picsum.photos/seed/product-bolsa-de-tela-plegable-biogama/400/400', true, NULL, '2026-07-25', NULL, NULL),

-- Artículos para el hogar sostenible (5)
('b2000001-0000-4000-8000-00000000001B', 'Set de Cubiertos de Bambú', 'Cubiertos portátiles (tenedor, cuchillo, cuchara, pitillo y limpia) en estuche de tela.', 10.00, 100, 0, 15, 'ECO-003', 'https://picsum.photos/seed/product-set-de-cubiertos-de-bamb/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-00000000001C', 'Jabón Artesanal de Plantas Nativas', 'Jabón natural elaborado con aceites esenciales de plantas nativas ecuatorianas. 100g. Variedad de aromas.', 6.00, 150, 0, 20, 'ECO-004', 'https://picsum.photos/seed/product-jabn-artesanal-de-plantas-nativas/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-00000000001D', 'Velas de Cera de Abeja Natural', 'Velas artesanales de cera de abeja pura. Producidas por apicultores de la sierra ecuatoriana. Pack x3.', 12.00, 80, 0, 12, 'ECO-005', 'https://picsum.photos/seed/product-velas-de-cera-de-abeja-natural/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-00000000001E', 'Kit de Compostaje Doméstico', 'Compostera de 10L con tapa, tierra activada, lombrices californianas y guía ilustrada.', 38.00, 30, 0, 5, 'ECO-006', 'https://picsum.photos/seed/product-kit-de-compostaje-domstico/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-00000000001F', 'Semillas Nativas – Pack Huerto Urbano', 'Sobre con 8 variedades de semillas nativas: tomate riñón, pimiento, albahaca, lechuga, rábano, zanahoria, cilantro y frejol.', 4.50, 200, 0, 30, 'SEM-001', 'https://picsum.photos/seed/product-semillas-nativas-pack-huerto-urbano/400/400', true, NULL, '2026-07-25', NULL, NULL),

-- Juguetes educativos (3)
('b2000001-0000-4000-8000-000000000020', 'Rompecabezas Ecosistemas del Ecuador 500pz', 'Rompecabezas ilustrado con los 4 ecosistemas principales del Ecuador. Incluye guía de especies.', 22.00, 40, 0, 8, 'JUE-001', 'https://picsum.photos/seed/product-rompecabezas-ecosistemas-del-ecuador-500pz/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000021', 'Memoria de Aves del Ecuador', 'Juego de memoria con 40 cartas (20 parejas) de aves emblemáticas. Incluye datos curiosos de cada especie.', 14.00, 60, 0, 10, 'JUE-002', 'https://picsum.photos/seed/product-memoria-de-aves-del-ecuador/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000022', 'Kit de Ciencia para Niños – Insectos', 'Lupa, frasco colector, pinzas, guía de insectos comunes y libreta de campo. Edad 6+.', 18.50, 45, 0, 8, 'JUE-003', 'https://picsum.photos/seed/product-kit-de-ciencia-para-nios-insectos/400/400', true, NULL, '2026-07-25', NULL, NULL),

-- Decoración (3)
('b2000001-0000-4000-8000-000000000023', 'Lámina Botánica Ilustrada 40x60cm', 'Impresión giclée en papel de algodón de ilustraciones botánicas de orquídeas ecuatorianas. Marco incluido.', 35.00, 25, 0, 5, 'DEC-001', 'https://picsum.photos/seed/product-lmina-botnica-ilustrada-40x60cm/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000024', 'Terrario de Vidrio Soplado con Plantas', 'Terrario artesanal de vidrio soplado con suculentas, musgo y carbón activado. Diámetro 15cm.', 40.00, 20, 0, 4, 'DEC-002', 'https://picsum.photos/seed/product-terrario-de-vidrio-soplado-con-plantas/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000025', 'Set de Tarjetas Postales Biodiversidad', 'Set de 12 tarjetas postales con fotografías de fauna ecuatoriana. Impresas en papel reciclado.', 8.00, 100, 0, 15, 'DEC-003', 'https://picsum.photos/seed/product-set-de-tarjetas-postales-biodiversidad/400/400', true, NULL, '2026-07-25', NULL, NULL),

-- Libros infantiles (2)
('b2000001-0000-4000-8000-000000000026', '"Lola la Rana Viajera" – Libro Infantil', 'Cuento infantil ilustrado que sigue las aventuras de una rana arlequín por los ríos del Ecuador. 32 páginas.', 12.00, 60, 0, 10, 'LIB-008', 'https://picsum.photos/seed/product-lola-la-rana-viajera-libro-infantil/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000027', 'Pinta y Descubre las Aves del Ecuador', 'Libro para colorear con 30 aves ecuatorianas. Incluye datos curiosos y códigos QR con sus cantos.', 8.50, 80, 0, 12, 'LIB-009', 'https://picsum.photos/seed/product-pinta-y-descubre-las-aves-del-ecuador/400/400', true, NULL, '2026-07-25', NULL, NULL),

-- Comestibles (3)
('b2000001-0000-4000-8000-000000000028', 'Miel de Abeja Orgánica – 350g', 'Miel pura de abejas nativas sin pesticidas. Producida por apicultores de la Reserva Cayambe-Coca.', 9.00, 60, 0, 10, 'COM-001', 'https://picsum.photos/seed/product-miel-de-abeja-orgnica-350g/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-000000000029', 'Chocolate Artesanal de Cacao Fino 70%', 'Chocolate oscuro elaborado con cacao nacional de la provincia de Manabí. 80g. Empaque compostable.', 7.50, 80, 0, 12, 'COM-002', 'https://picsum.photos/seed/product-chocolate-artesanal-de-cacao-fino-70/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-00000000002A', 'Té de Hierbas Nativas – Caja Mix', 'Caja con 12 bolsitas de té de hierbas nativas: cedrón, tilo, hierba luisa, manzanilla y toronjil.', 6.00, 100, 0, 15, 'COM-003', 'https://picsum.photos/seed/product-t-de-hierbas-nativas-caja-mix/400/400', true, NULL, '2026-07-25', NULL, NULL),

-- Herramientas de campo (2)
('b2000001-0000-4000-8000-00000000002B', 'Cámara Trampa Digital 12MP', 'Cámara de activación por movimiento con infrarrojo nocturno, resistencia IP66, tarjeta SD 32GB.', 120.00, 15, 0, 3, 'TEC-001', 'https://picsum.photos/seed/product-cmara-trampa-digital-12mp/400/400', true, NULL, '2026-07-25', NULL, NULL),

('b2000001-0000-4000-8000-00000000002C', 'GPS de Mano Garmin eTrex 22x', 'GPS portátil con brújula electrónica, barómetro y altímetro. Resistente al agua. 25h de batería.', 180.00, 10, 0, 2, 'TEC-002', 'https://picsum.photos/seed/product-gps-de-mano-garmin-etrex-22x/400/400', true, NULL, '2026-07-25', NULL, NULL);

-- ============================================================================
-- FIN DEL SCRIPT
-- ============================================================================

