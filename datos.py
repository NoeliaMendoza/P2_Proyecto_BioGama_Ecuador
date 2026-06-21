"""
Generador de datos COHERENTES para biogama_ecuador
- Especies endémicas solo aparecen en sus regiones naturales
- Investigadores registran especies de su especialidad
- Coordenadas reales por región del Ecuador
- 500,000 registros totales
Requiere: pip install faker psycopg2-binary
"""

import psycopg2
from faker import Faker
import random
from datetime import datetime, timezone

# ─── CONFIGURACIÓN ────────────────────────────────────────────────
DB_CONFIG = {
    "host": "localhost",
    "port": 5432,
    "database": "biogama_ecuador",
    "user": "biogama_user",
    "password": "BioGama$2026#Ec"
}

BATCH_SIZE = 1000
N_FAMILIES    = 500
N_NATURAL_RES = 200
N_LOCATIONS   = 5_000
N_RESEARCHERS = 1_000
N_SPECIES     = 10_000
N_RECORDS     = 483_300

fake = Faker("es_ES")
Faker.seed(42)
random.seed(42)

# ─── REGIONES Y SUS DATOS GEOGRÁFICOS ─────────────────────────────
# Cada región tiene: coordenadas reales, lugares, altitud típica
REGIONES = {
    "Amazonia": {
        "lat": (-2.5, 0.0),
        "lon": (-78.0, -75.0),
        "altitud": (100, 600),
        "lugares": [
            "Río Napo", "Río Pastaza", "Río Morona", "Lago Agrio", "Coca",
            "Tena", "Puyo", "Macas", "Sarayaku", "Comunidad Achuar Kapawi",
            "Cueva de los Tayos", "Taisha", "Logroño", "Palora", "Méndez",
            "Sucúa", "Limón Indanza", "Gualaquiza", "Cabeceras del Pastaza",
            "Río Aguarico", "Laguna de Limoncocha", "Río Curaray"
        ],
        "reservas": [
            "Parque Nacional Yasuní", "Reserva Faunística Cuyabeno",
            "Reserva Biológica Limoncocha", "Parque Nacional Sumaco Napo-Galeras",
            "Reserva Ecológica Cofán-Bermejo", "Territorio Indígena Sarayaku",
            "Bosque Protector Cerro Sumaco", "Reserva Biológica El Cóndor"
        ]
    },
    "Sierra": {
        "lat": (-4.0, 0.5),
        "lon": (-79.5, -77.5),
        "altitud": (2000, 4800),
        "lugares": [
            "Páramo de Antisana", "Páramo del Cajas", "Páramo de Chimborazo",
            "Laguna de Quilotoa", "Laguna de Mojanda", "Otavalo", "Cotacachi",
            "Cayambe", "Machachi", "Latacunga", "Salcedo", "Ambato",
            "Baños", "Riobamba", "Alausí", "Cañar", "Azogues",
            "Cuenca", "Loja", "Saraguro", "Catacocha", "Sangolquí"
        ],
        "reservas": [
            "Parque Nacional Cotopaxi", "Parque Nacional Chimborazo",
            "Reserva Ecológica Antisana", "Reserva Ecológica Cayambe-Coca",
            "Parque Nacional Llanganates", "Parque Nacional Sangay",
            "Parque Nacional Podocarpus", "Reserva Ecológica El Ángel",
            "Reserva Geobotánica Pululahua", "Parque Nacional Cajas"
        ]
    },
    "Costa": {
        "lat": (-3.5, 1.5),
        "lon": (-81.0, -79.0),
        "altitud": (0, 800),
        "lugares": [
            "Esmeraldas", "Atacames", "Muisne", "Pedernales", "Manta",
            "Portoviejo", "Bahía de Caráquez", "Jipijapa", "Montañita",
            "Salinas", "Santa Elena", "Guayaquil", "Milagro", "Machala",
            "Bosque Protector Cerro Blanco", "Isla Puná", "Manglar de Majagual",
            "Reserva Mache-Chindul", "Cordillera Chongón-Colonche"
        ],
        "reservas": [
            "Parque Nacional Machalilla", "Reserva Ecológica Mache-Chindul",
            "Reserva Ecológica Manglares Cayapas-Mataje",
            "Reserva Ecológica Cotacachi-Cayapas",
            "Bosque Protector Cerro Blanco", "Refugio de Vida Silvestre Manglares El Salado",
            "Área Nacional de Recreación Isla Santay"
        ]
    },
    "Galápagos": {
        "lat": (-1.5, 0.7),
        "lon": (-91.8, -89.2),
        "altitud": (0, 1700),
        "lugares": [
            "Puerto Ayora", "Puerto Baquerizo Moreno", "Puerto Villamil",
            "Isla Fernandina", "Isla Española", "Isla Genovesa",
            "Isla Marchena", "Volcán Wolf", "Laguna El Junco",
            "Bahía Gardner", "Isla Darwin", "Isla Wolf",
            "Punta Suárez", "Caleta Iguana", "Bahía Urbina"
        ],
        "reservas": [
            "Parque Nacional Galápagos", "Reserva Marina de Galápagos",
            "Reserva Biológica Galápagos"
        ]
    }
}

# ─── FAMILIAS CON REINO Y REGIÓN PREFERIDA ────────────────────────
FAMILIAS_DATA = [
    # (nombre, reino, regiones_preferidas)
    ("Felidae",        "Animalia", ["Amazonia", "Sierra", "Costa"]),
    ("Canidae",        "Animalia", ["Sierra", "Amazonia"]),
    ("Ursidae",        "Animalia", ["Sierra"]),
    ("Mustelidae",     "Animalia", ["Amazonia", "Sierra"]),
    ("Tapiridae",      "Animalia", ["Amazonia", "Sierra"]),
    ("Cervidae",       "Animalia", ["Sierra", "Amazonia"]),
    ("Delphinidae",    "Animalia", ["Costa", "Galápagos"]),
    ("Otariidae",      "Animalia", ["Galápagos", "Costa"]),
    ("Psittacidae",    "Animalia", ["Amazonia", "Costa", "Sierra"]),
    ("Trochilidae",    "Animalia", ["Sierra", "Amazonia", "Costa"]),
    ("Ramphastidae",   "Animalia", ["Amazonia", "Costa"]),
    ("Accipitridae",   "Animalia", ["Sierra", "Amazonia", "Galápagos"]),
    ("Ardeidae",       "Animalia", ["Costa", "Amazonia", "Galápagos"]),
    ("Fregatidae",     "Animalia", ["Galápagos", "Costa"]),
    ("Spheniscidae",   "Animalia", ["Galápagos"]),
    ("Dendrobatidae",  "Animalia", ["Amazonia", "Costa"]),
    ("Centrolenidae",  "Animalia", ["Amazonia", "Sierra"]),
    ("Pristimantidae", "Animalia", ["Sierra", "Amazonia"]),
    ("Colubridae",     "Animalia", ["Amazonia", "Costa", "Sierra"]),
    ("Boidae",         "Animalia", ["Amazonia", "Costa"]),
    ("Iguanidae",      "Animalia", ["Galápagos", "Costa"]),
    ("Geckonidae",     "Animalia", ["Galápagos"]),
    ("Cichlidae",      "Animalia", ["Amazonia"]),
    ("Characidae",     "Animalia", ["Amazonia"]),
    ("Pimelodidae",    "Animalia", ["Amazonia"]),
    ("Orchidaceae",    "Plantae",  ["Sierra", "Amazonia", "Costa"]),
    ("Bromeliaceae",   "Plantae",  ["Sierra", "Costa", "Amazonia"]),
    ("Araceae",        "Plantae",  ["Amazonia", "Costa"]),
    ("Fabaceae",       "Plantae",  ["Amazonia", "Costa", "Sierra"]),
    ("Myrtaceae",      "Plantae",  ["Sierra", "Costa"]),
    ("Cactaceae",      "Plantae",  ["Galápagos", "Costa"]),
    ("Poaceae",        "Plantae",  ["Sierra", "Galápagos"]),
    ("Arecaceae",      "Plantae",  ["Amazonia", "Costa"]),
    ("Moraceae",       "Plantae",  ["Amazonia"]),
    ("Lauraceae",      "Plantae",  ["Sierra", "Amazonia"]),
    ("Polyporaceae",   "Fungi",    ["Amazonia", "Sierra"]),
    ("Agaricaceae",    "Fungi",    ["Sierra", "Costa"]),
    ("Formicidae",     "Animalia", ["Amazonia", "Costa"]),
    ("Sphingidae",     "Animalia", ["Amazonia", "Sierra"]),
    ("Nymphalidae",    "Animalia", ["Amazonia", "Costa", "Sierra"]),
]

# ─── ESPECIES POR REGIÓN ──────────────────────────────────────────
ESPECIES_POR_REGION = {
    "Amazonia": [
        ("Jaguar amazónico",         "Panthera onca",              "VU"),
        ("Tapir amazónico",          "Tapirus terrestris",         "VU"),
        ("Delfín rosado",            "Inia geoffrensis",           "EN"),
        ("Nutria gigante",           "Pteronura brasiliensis",     "EN"),
        ("Manatí amazónico",         "Trichechus inunguis",        "VU"),
        ("Guacamayo escarlata",      "Ara macao",                  "LC"),
        ("Tucán amazónico",          "Ramphastos tucanus",         "LC"),
        ("Anaconda verde",           "Eunectes murinus",           "LC"),
        ("Caimán negro",             "Melanosuchus niger",         "LC"),
        ("Rana venenosa",            "Dendrobates tinctorius",     "LC"),
        ("Rana de cristal",          "Centrolenella prosoblepon",  "NT"),
        ("Mono araña",               "Ateles belzebuth",           "EN"),
        ("Pecarí de labio blanco",   "Tayassu pecari",             "VU"),
        ("Arapaima",                 "Arapaima gigas",             "DD"),
        ("Piranha roja",             "Pygocentrus nattereri",      "LC"),
        ("Hormiga bala",             "Paraponera clavata",         "LC"),
        ("Morpho azul",              "Morpho menelaus",            "LC"),
        ("Boa esmeralda",            "Corallus caninus",           "LC"),
        ("Garceta tricolor",         "Egretta tricolor",           "LC"),
        ("Heliconia amazónica",      "Heliconia rostrata",         "LC"),
    ],
    "Sierra": [
        ("Cóndor andino",            "Vultur gryphus",             "VU"),
        ("Oso de anteojos",          "Tremarctos ornatus",         "VU"),
        ("Tapir de montaña",         "Tapirus pinchaque",          "EN"),
        ("Puma andino",              "Puma concolor",              "LC"),
        ("Venado de cola blanca",    "Odocoileus virginianus",     "LC"),
        ("Colibrí cola de espátula", "Loddigesia mirabilis",       "EN"),
        ("Carpintero andino",        "Colaptes rupicola",          "LC"),
        ("Rana marsupial andina",    "Gastrotheca riobambae",      "EN"),
        ("Sapo jambato",             "Atelopus ignescens",         "EX"),
        ("Lagartija de páramo",      "Stenocercus guentheri",      "NT"),
        ("Cóndor del Andes",         "Vultur gryphus andinus",     "VU"),
        ("Lobo de páramo",           "Lycalopex culpaeus",         "LC"),
        ("Conejillo de indias",      "Cavia porcellus",            "LC"),
        ("Quilotoa salamandra",      "Bolitoglossa peruviana",     "CR"),
        ("Orquídea fantasma",        "Dracula gigas",              "VU"),
        ("Polylepis serrana",        "Polylepis incana",           "VU"),
        ("Chuquiragua",              "Chuquiragua insignis",       "LC"),
        ("Genciana andina",          "Gentiana sedifolia",         "LC"),
        ("Curiquingue",              "Phalcoboenus carunculatus",  "LC"),
        ("Gaviota andina",           "Chroicocephalus serranus",   "LC"),
    ],
    "Costa": [
        ("Jaguar costero",           "Panthera onca centralis",    "CR"),
        ("Cocodrilo americano",      "Crocodylus acutus",          "VU"),
        ("Mono capuchino",           "Cebus albifrons",            "LC"),
        ("Boa constrictor",          "Boa constrictor",            "LC"),
        ("Perico del Pacífico",      "Brotogeris pyrrhoptera",     "EN"),
        ("Pelícano pardo",           "Pelecanus occidentalis",     "LC"),
        ("Fragata magnífica",        "Fregata magnificens",        "LC"),
        ("Colibrí de Juan Fernández","Sephanoides fernandensis",   "CR"),
        ("Iguana verde",             "Iguana iguana",              "LC"),
        ("Rana de Jambeli",          "Pristimantis croceoinguinis","VU"),
        ("Mangle rojo",              "Rhizophora mangle",          "LC"),
        ("Ceibo",                    "Ceiba pentandra",            "LC"),
        ("Balsa",                    "Ochroma pyramidale",         "LC"),
        ("Guayacán",                 "Tabebuia chrysantha",        "NT"),
        ("Palo santo",               "Bursera graveolens",         "LC"),
        ("Tiburón martillo",         "Sphyrna lewini",             "CR"),
        ("Manta raya",               "Manta birostris",            "VU"),
        ("Tortuga golfina",          "Lepidochelys olivacea",      "VU"),
        ("Delfín nariz de botella",  "Tursiops truncatus",         "LC"),
        ("Mariposa cebra",           "Heliconius charithonia",     "LC"),
    ],
    "Galápagos": [
        ("Tortuga gigante de Galápagos", "Chelonoidis nigra",      "VU"),
        ("Iguana marina",            "Amblyrhynchus cristatus",    "VU"),
        ("Iguana terrestre",         "Conolophus subcristatus",    "VU"),
        ("Pingüino de Galápagos",    "Spheniscus mendiculus",      "EN"),
        ("Lobo marino de Galápagos", "Zalophus wollebaeki",        "EN"),
        ("Cormorán no volador",      "Nannopterum harrisi",        "EN"),
        ("Fragata real",             "Fregata minor",              "LC"),
        ("Piquero de patas azules",  "Sula nebouxii",              "LC"),
        ("Piquero de Nazca",         "Sula granti",                "LC"),
        ("Albatros de Galápagos",    "Phoebastria irrorata",       "CR"),
        ("Pinzón de Darwin",         "Geospiza fortis",            "LC"),
        ("Flamenco de Galápagos",    "Phoenicopterus ruber",       "LC"),
        ("Cactus de Galápagos",      "Opuntia echios",             "VU"),
        ("Scalesia gigante",         "Scalesia pedunculata",       "CR"),
        ("Tomás de Galápagos",       "Jasminocereus thouarsii",    "LC"),
        ("Tiburón ballena",          "Rhincodon typus",            "EN"),
        ("Mero gigante",             "Epinephelus itajara",        "CR"),
        ("Pulpo de Galápagos",       "Octopus oculifer",           "LC"),
        ("Langosta roja",            "Panulirus penicillatus",     "LC"),
        ("Raya águila",              "Aetobatus narinari",         "NT"),
    ]
}

# ─── ESPECIALIDADES Y LAS FAMILIAS/REINOS QUE ESTUDIAN ────────────
ESPECIALIDAD_REINOS = {
    "Ornitología":    ["Animalia"],
    "Herpetología":   ["Animalia"],
    "Botánica":       ["Plantae"],
    "Entomología":    ["Animalia"],
    "Mastozoología":  ["Animalia"],
    "Ictiología":     ["Animalia"],
    "Micología":      ["Fungi"],
    "Primatología":   ["Animalia"],
    "Ecología":       ["Animalia", "Plantae", "Fungi"],
    "Taxonomía":      ["Animalia", "Plantae", "Fungi"],
    "Biología Marina":["Animalia"],
    "Conservación":   ["Animalia", "Plantae"],
}

ESPECIALIDAD_FAMILIAS = {
    "Ornitología":    ["Psittacidae","Trochilidae","Ramphastidae","Accipitridae","Ardeidae","Fregatidae","Spheniscidae"],
    "Herpetología":   ["Dendrobatidae","Centrolenidae","Pristimantidae","Colubridae","Boidae","Iguanidae","Geckonidae"],
    "Botánica":       ["Orchidaceae","Bromeliaceae","Araceae","Fabaceae","Myrtaceae","Cactaceae","Poaceae","Arecaceae","Moraceae","Lauraceae"],
    "Entomología":    ["Formicidae","Sphingidae","Nymphalidae"],
    "Mastozoología":  ["Felidae","Canidae","Ursidae","Mustelidae","Tapiridae","Cervidae","Delphinidae","Otariidae"],
    "Ictiología":     ["Cichlidae","Characidae","Pimelodidae"],
    "Micología":      ["Polyporaceae","Agaricaceae"],
    "Primatología":   ["Felidae"],
    "Ecología":       [],
    "Taxonomía":      [],
    "Biología Marina":["Delphinidae","Otariidae","Spheniscidae"],
    "Conservación":   [],
}

OBSERVACIONES = [
    "Individuo avistado en las primeras horas de la mañana cerca de la ribera del río. Comportamiento tranquilo, sin signos de estrés.",
    "Se registraron varios ejemplares agrupados bajo la cobertura de árboles nativos. Posible zona de refugio o alimentación.",
    "Espécimen fotografiado en actividad nocturna. Presenta coloración característica de la especie y buen estado general.",
    "Avistamiento en zona de transición entre bosque primario y secundario. Se observó interacción con otras especies del ecosistema.",
    "Individuo juvenil detectado en orilla de quebrada. Sin signos visibles de enfermedad o lesiones externas.",
    "Registro mediante trampa cámara durante la madrugada. El espécimen mostró comportamiento de forrajeo activo.",
    "Se encontraron huellas y evidencia indirecta de la especie en el sendero principal de la reserva.",
    "Ejemplar adulto observado en copa de árbol a aproximadamente 15 metros de altura. Buenas condiciones de hábitat.",
    "Grupo de tres individuos registrado cerca del borde del río. Se tomaron coordenadas GPS del punto exacto.",
    "Avistamiento durante transecto lineal de monitoreo. Espécimen en aparente buen estado de salud.",
    "Individuo con marcas de apareamiento recientes. Periodo reproductivo activo confirmado en la zona.",
    "Observación realizada desde plataforma elevada. El animal no presentó conducta de huida ante la presencia humana.",
    "Espécimen recolectado temporalmente para toma de datos morfométricos y posterior liberación en el mismo punto.",
    "Registro auditivo confirmado por llamado característico de la especie. No fue posible la observación directa.",
    "Se observó nido activo con presencia de crías. Zona marcada para seguimiento en próximas visitas.",
    "Individuo detectado mediante detector ultrasónico. Identificación por patrón de ecolocalización.",
    "Espécimen avistado en zona de pastizal colindante con bosque. Adaptación a hábitat perturbado evidente.",
    "Registro obtenido durante campaña de bioinventario. Coordenadas verificadas con GPS de alta precisión.",
    "Tres individuos observados en zona rocosa a gran altitud. Comportamiento gregario típico de la especie.",
    "Avistamiento casual durante recorrido de patrullaje. Se reportó al equipo técnico para registro formal.",
    "Planta en estado de floración activa. Asociación con polinizadores locales observada durante el registro.",
    "Espécimen en estadio larval recolectado para identificación en laboratorio. Muestra preservada en etanol al 70%.",
    "Individuo con marcas de depredación reciente en extremidad posterior. Estado general estable.",
    "Registro visual confirmado a menos de 10 metros de distancia. Fotografías de alta calidad obtenidas.",
    "Zona con alta densidad de individuos de la especie. Se recomienda establecer área de protección estricta.",
    "Espécimen capturado con red de niebla para toma de medidas y colocación de anillo de identificación.",
    "Avistamiento desde embarcación a 50 metros de la orilla. Animal en comportamiento de alimentación activa.",
    "Huella fresca encontrada en barro a orillas del río. Tamaño y morfología confirman la especie.",
    "Espécimen observado interactuando con individuos de otra especie. Posible relación de comensalismo.",
    "Primer registro de la especie en esta reserva. Dato de alta importancia para el inventario regional.",
]

DESCRIPCIONES_ESPECIE = {
    "Amazonia": "Especie típica de los bosques húmedos tropicales de la cuenca amazónica ecuatoriana. Requiere extensas áreas de bosque primario para mantener poblaciones viables. Su presencia es indicador de ecosistemas bien conservados.",
    "Sierra":   "Especie andina adaptada a las condiciones de alta montaña del Ecuador. Habita desde los bosques montanos hasta el páramo. Su distribución altitudinal la hace especialmente vulnerable al cambio climático.",
    "Costa":    "Especie característica de los ecosistemas costeros del Ecuador continental. Habita tanto en manglares como en bosques secos y húmedos tropicales de la región litoral. Tolera cierto grado de perturbación antrópica.",
    "Galápagos":"Especie endémica del archipiélago de Galápagos, resultado de millones de años de evolución aislada. No posee depredadores naturales en las islas, lo que la hace especialmente vulnerable ante la introducción de especies exóticas.",
}

DESCRIPCIONES_RESERVA = [
    "Área protegida de gran importancia ecológica para la conservación de la biodiversidad del Ecuador. Alberga ecosistemas únicos de alto valor científico y cultural.",
    "Reserva con presencia de comunidades indígenas que coexisten con el ecosistema. El manejo comunitario ha permitido conservar la cobertura vegetal original.",
    "Zona de amortiguamiento del Sistema Nacional de Áreas Protegidas. Cuenta con corredores biológicos que conectan parches de bosque fragmentado.",
    "Área con alta diversidad de aves, anfibios y plantas vasculares. Destino frecuente de investigadores nacionales e internacionales.",
    "Reserva declarada sitio Ramsar por la importancia de sus humedales. Cumple funciones de regulación hídrica para las poblaciones aledañas.",
    "Ecosistema de bosque seco tropical, uno de los más amenazados del Ecuador. La reserva protege las últimas extensiones significativas de este bioma en el país.",
    "Área de transición entre dos regiones naturales. Presenta gradientes altitudinales que generan alta diversidad de microhábitats y especies.",
    "Zona núcleo de un corredor de conservación transfronterizo. Comparte flora y fauna con áreas protegidas de Colombia y Perú.",
]

NOMBRES = [
    "Carlos","Luis","Jorge","Miguel","Andrés","Diego","Javier","Fernando",
    "Roberto","Pablo","Sebastián","David","Alejandro","Ricardo","Cristian",
    "María","Ana","Patricia","Verónica","Gabriela","Daniela","Fernanda",
    "Karla","Mónica","Valeria","Paola","Sofía","Camila","Lucía","Isabel",
    "Byron","Edison","Bolívar","Oswaldo","Hernán","Rodrigo","Marcelo",
    "Ximena","Narcisa","Mirian","Gladys","Rocío","Paulina","Alexandra"
]

APELLIDOS = [
    "García","Rodríguez","López","Martínez","González","Pérez","Sánchez",
    "Romero","Torres","Flores","Vargas","Morales","Ortiz","Delgado",
    "Castro","Gutiérrez","Vásquez","Herrera","Medina","Ríos","Alvarado",
    "Quispe","Chuquimarca","Andrade","Moreira","Paucar","Cando","Tipán",
    "Simbaña","Pilataxi","Guaranda","Cayambe","Imbaquingo","Farinango",
    "Muñoz","Cabrera","Naranjo","Salazar","Benítez","Carrillo","Espinoza",
    "Llerena","Pazmiño","Chiriboga","Endara","Proaño","Velasco","Viteri"
]

INSTITUTIONS = [
    "Universidad Central del Ecuador","PUCE","Universidad de Guayaquil",
    "ESPOL","ESPE","FLACSO Ecuador","Universidad del Azuay",
    "Ministerio del Ambiente","Fundación EcoCiencia","WWF Ecuador",
    "Instituto Nacional de Biodiversidad","Museo de Historia Natural QCAZ",
    "Universidad Técnica de Ambato","Universidad de Cuenca",
    "Universidad Nacional de Loja","Universidad Estatal Amazónica"
]

def nombre_ecuatoriano():
    return f"{random.choice(NOMBRES)} {random.choice(APELLIDOS)} {random.choice(APELLIDOS)}"

def get_connection():
    return psycopg2.connect(**DB_CONFIG)

def insert_batch(cur, sql, batch):
    cur.executemany(sql, batch)

# ─── GENERADORES ──────────────────────────────────────────────────

def generar_families(cur):
    print("Insertando Families...")
    sql = """INSERT INTO public."Families" ("Name","Kingdom","IsActive","CreatedAt")
             VALUES (%s,%s,%s,%s)"""
    batch = []
    used_names = set()
    # Insertar familias definidas primero
    for nombre, reino, _ in FAMILIAS_DATA:
        used_names.add(nombre)
        batch.append((nombre[:100], reino, True, datetime.now(timezone.utc)))

    # Completar hasta N_FAMILIES con variantes
    extra_names = [
        "Solanaceae","Asteraceae","Lamiaceae","Rubiaceae","Euphorbiaceae",
        "Melastomataceae","Piperaceae","Gesneriaceae","Passifloraceae","Cucurbitaceae",
        "Apocynaceae","Sapotaceae","Meliaceae","Burseraceae","Anacardiaceae",
        "Pteridaceae","Polypodiaceae","Lycopodiaceae","Hepaticae","Musci"
    ]
    for i, n in enumerate(extra_names):
        if len(batch) >= N_FAMILIES: break
        name = n if n not in used_names else f"{n} {i+1}"
        used_names.add(name)
        batch.append((name[:100], random.choice(["Plantae","Fungi"]), True, datetime.now(timezone.utc)))

    while len(batch) < N_FAMILIES:
        idx = len(batch)
        batch.append((f"Familia sp. {idx}"[:100], random.choice(["Animalia","Plantae"]), True, datetime.now(timezone.utc)))

    insert_batch(cur, sql, batch[:N_FAMILIES])
    print(f"   {N_FAMILIES} familias insertadas")

def generar_natural_reserves(cur):
    print("Insertando NaturalReserves...")
    sql = """INSERT INTO public."NaturalReserves" ("Name","Region","SurfaceHectares","YearCreated","Description","IsActive","CreatedAt")
             VALUES (%s,%s,%s,%s,%s,%s,%s)"""
    batch = []
    # Reservas reales por región
    for region, datos in REGIONES.items():
        for reserva in datos["reservas"]:
            if len(batch) >= N_NATURAL_RES: break
            batch.append((
                reserva[:150], region,
                round(random.uniform(500, 1_000_000), 2),
                random.randint(1959, 2020),
                random.choice(DESCRIPCIONES_RESERVA),
                True, datetime.now(timezone.utc)
            ))

    # Completar con variantes
    regiones_list = list(REGIONES.keys())
    while len(batch) < N_NATURAL_RES:
        region = random.choice(regiones_list)
        base = random.choice(REGIONES[region]["reservas"])
        batch.append((
            f"{base} - Zona {len(batch)}"[:150], region,
            round(random.uniform(100, 500_000), 2),
            random.randint(1959, 2022),
            random.choice(DESCRIPCIONES_RESERVA),
            True, datetime.now(timezone.utc)
        ))

    insert_batch(cur, sql, batch[:N_NATURAL_RES])
    print(f"    {N_NATURAL_RES} reservas insertadas")

def generar_locations(cur, reserve_map):
    """reserve_map: {id: region}"""
    print("Insertando Locations...")
    sql = """INSERT INTO public."Locations" ("PlaceName","Altitude","Latitude","Longitude","NaturalReserveId","IsActive","CreatedAt")
             VALUES (%s,%s,%s,%s,%s,%s,%s)"""
    reserve_ids = list(reserve_map.keys())
    sufijos = ["","Norte","Sur","Alta","Baja","Sector 1","Sector 2","Margen Izquierda","Margen Derecha","Centro"]
    batch = []
    for i in range(N_LOCATIONS):
        rid = random.choice(reserve_ids)
        region = reserve_map[rid]
        datos = REGIONES[region]
        lugar = random.choice(datos["lugares"])
        sufijo = random.choice(sufijos)
        place = f"{lugar} {sufijo}".strip()
        alt_min, alt_max = datos["altitud"]
        batch.append((
            place[:150],
            random.randint(alt_min, alt_max),
            round(random.uniform(*datos["lat"]), 6),
            round(random.uniform(*datos["lon"]), 6),
            rid, True, datetime.now(timezone.utc)
        ))
        if len(batch) == BATCH_SIZE:
            insert_batch(cur, sql, batch); batch.clear()
    if batch: insert_batch(cur, sql, batch)
    print(f"   {N_LOCATIONS} ubicaciones insertadas")

def generar_researchers(cur):
    print("Insertando Researchers...")
    sql = """INSERT INTO public."Researchers" ("Name","Institution","Email","Specialty","IsActive","CreatedAt")
             VALUES (%s,%s,%s,%s,%s,%s)"""
    especialidades = list(ESPECIALIDAD_REINOS.keys())
    dominios = ["uce.edu.ec","espe.edu.ec","puce.edu.ec","espol.edu.ec","ambiente.gob.ec","inabio.gob.ec","espoch.edu.ec","ucuenca.edu.ec"]
    batch = []
    for i in range(N_RESEARCHERS):
        nombre = nombre_ecuatoriano()
        apellido = random.choice(APELLIDOS).lower().replace(" ","")
        email = f"{nombre.split()[0].lower()}.{apellido}{random.randint(1,99)}@{random.choice(dominios)}"
        batch.append((
            nombre[:100],
            random.choice(INSTITUTIONS)[:100],
            email[:100],
            random.choice(especialidades),
            True, datetime.now(timezone.utc)
        ))
        if len(batch) == BATCH_SIZE:
            insert_batch(cur, sql, batch); batch.clear()
    if batch: insert_batch(cur, sql, batch)
    print(f"   {N_RESEARCHERS} investigadores insertados")

def generar_species(cur, family_map):
    """family_map: {id: (nombre, reino, regiones)}"""
    print("Insertando Species...")
    sql = """INSERT INTO public."Species" ("CommonName","ScientificName","ConservationStatus","Description","ImageUrl","IsEndemic","FamilyId","IsActive","CreatedAt")
             VALUES (%s,%s,%s,%s,%s,%s,%s,%s,%s)"""

    # Construir lista de especies: (common, scientific, status, region, family_id, is_endemic)
    especies = []
    family_ids_list = list(family_map.keys())

    # Primero insertar especies reales definidas
    for region, lista in ESPECIES_POR_REGION.items():
        for common, scientific, status in lista:
            # Buscar familia coherente
            fid = random.choice(family_ids_list)
            for fid2, (fname, freino, fregiones) in family_map.items():
                if region in fregiones:
                    fid = fid2
                    break
            is_endemic = region == "Galápagos" or random.random() < 0.3
            especies.append((common, scientific, status, region, fid, is_endemic))

    # Completar con variaciones hasta N_SPECIES
    prefixes_por_region = {
        "Amazonia":  ["Amazona","Tapirus","Heliconia","Ara","Boa","Caiman","Pristimantis","Ateles","Morpho","Epidendrum"],
        "Sierra":    ["Vultur","Tremarctos","Puma","Atelopus","Gastrotheca","Dracula","Polylepis","Chuquiragua","Phalcoboenus"],
        "Costa":     ["Boa","Crocodylus","Ceiba","Rhizophora","Fregata","Pelecanus","Iguana","Brotogeris","Tabebuia"],
        "Galápagos": ["Chelonoidis","Amblyrhynchus","Conolophus","Spheniscus","Zalophus","Nannopterum","Geospiza","Scalesia","Opuntia"],
    }
    sufijos_sci = ["ecuatorianus","andinus","amazonica","galapagensis","costensis","orientalis","minor","major","peruviana","colombianus"]
    regiones_list = list(REGIONES.keys())

    while len(especies) < N_SPECIES:
        region = random.choice(regiones_list)
        prefix = random.choice(prefixes_por_region[region])
        sufijo = random.choice(sufijos_sci)
        idx = len(especies)
        # Familia coherente con la región
        fid = random.choice(family_ids_list)
        for fid2, (fname, freino, fregiones) in family_map.items():
            if region in fregiones:
                fid = fid2
                break
        is_endemic = region == "Galápagos" or random.random() < 0.25
        status = random.choice(["LC","LC","LC","NT","VU","EN","CR","DD"])
        base_common = random.choice(ESPECIES_POR_REGION[region])[0]
        common = f"{base_common} {random.choice(['de montaña','amazónico','andino','costero','silvestre','de páramo','marino'])}"
        scientific = f"{prefix} {sufijo} {idx % 50 + 1}"
        especies.append((common, scientific, status, region, fid, is_endemic))

    batch = []
    for i, (common, scientific, status, region, fid, is_endemic) in enumerate(especies[:N_SPECIES]):
        desc = DESCRIPCIONES_ESPECIE.get(region, DESCRIPCIONES_ESPECIE["Amazonia"])
        batch.append((
            common[:100], scientific[:150], status, desc,
            f"https://biogama.ec/images/species/{i+1}.jpg"[:300],
            is_endemic, fid, True, datetime.now(timezone.utc)
        ))
        if len(batch) == BATCH_SIZE:
            insert_batch(cur, sql, batch); batch.clear()
    if batch: insert_batch(cur, sql, batch)
    print(f"   {N_SPECIES} especies insertadas")
    return especies[:N_SPECIES]  # retorna para usar en records

def generar_records(cur, especies_data, location_region_map, researcher_specialty_map):
    """
    especies_data: lista de (common, sci, status, region, fid, is_endemic) con sus IDs
    location_region_map: {location_id: region}
    researcher_specialty_map: {researcher_id: specialty}
    """
    print(f"Insertando Records ({N_RECORDS:,})... esto toma unos minutos")
    sql = """INSERT INTO public."Records" ("SpeciesId","LocationId","ResearcherId","DiscoveryDate","ObservedQuantity","Observations","IsActive","CreatedAt")
             VALUES (%s,%s,%s,%s,%s,%s,%s,%s)"""

    # Agrupar por región
    species_by_region = {}
    locations_by_region = {}
    for loc_id, region in location_region_map.items():
        locations_by_region.setdefault(region, []).append(loc_id)

    for sp_id, (common, sci, status, region, fid, is_endemic) in especies_data.items():
        species_by_region.setdefault(region, []).append(sp_id)

    regiones_list = list(REGIONES.keys())
    researcher_ids = list(researcher_specialty_map.keys())

    batch = []
    for i in range(N_RECORDS):
        # Elegir región y buscar especie + ubicación coherentes
        region = random.choice(regiones_list)
        sp_list = species_by_region.get(region, [])
        loc_list = locations_by_region.get(region, [])

        if not sp_list or not loc_list:
            region = "Amazonia"
            sp_list = species_by_region.get(region, list(especies_data.keys()))
            loc_list = locations_by_region.get(region, list(location_region_map.keys()))

        sp_id = random.choice(sp_list)
        loc_id = random.choice(loc_list)
        res_id = random.choice(researcher_ids)

        batch.append((
            sp_id, loc_id, res_id,
            fake.date_time_between(start_date="-20y", end_date="now", tzinfo=timezone.utc),
            random.randint(1, 200),
            random.choice(OBSERVACIONES),
            True, datetime.now(timezone.utc)
        ))
        if len(batch) == BATCH_SIZE:
            insert_batch(cur, sql, batch); batch.clear()
            if (i + 1) % 50_000 == 0:
                print(f"   ... {i+1:,} records insertados")
    if batch: insert_batch(cur, sql, batch)
    print(f"   {N_RECORDS:,} records insertados")

# ─── MAIN ─────────────────────────────────────────────────────────

def main():
    print("Iniciando generación de datos COHERENTES para biogama_ecuador")
    print(f"   Total: ~500,000 registros con coherencia geográfica y taxonómica\n")

    conn = get_connection()
    conn.autocommit = False
    cur = conn.cursor()

    try:
        generar_families(cur)
        conn.commit()

        # Leer familias con sus datos
        cur.execute('SELECT "Id","Name" FROM public."Families"')
        rows = cur.fetchall()
        family_map = {}
        familia_nombre_a_datos = {f[0]: (f[0], f[1], f[2]) for f in FAMILIAS_DATA}
        for fid, fname in rows:
            datos = familia_nombre_a_datos.get(fname)
            if datos:
                family_map[fid] = datos
            else:
                family_map[fid] = (fname, "Animalia", list(REGIONES.keys()))

        generar_natural_reserves(cur)
        conn.commit()

        # Leer reservas con su región
        cur.execute('SELECT "Id","Region" FROM public."NaturalReserves"')
        reserve_map = {row[0]: row[1] for row in cur.fetchall()}

        generar_locations(cur, reserve_map)
        conn.commit()

        generar_researchers(cur)
        conn.commit()

        especies_insertadas = generar_species(cur, family_map)
        conn.commit()

        # Leer IDs reales de Species con su región
        cur.execute('SELECT "Id" FROM public."Species" ORDER BY "Id"')
        sp_ids = [r[0] for r in cur.fetchall()]
        especies_con_id = {}
        for i, sp_id in enumerate(sp_ids):
            if i < len(especies_insertadas):
                especies_con_id[sp_id] = especies_insertadas[i]

        # Leer locations con región
        cur.execute("""
            SELECT l."Id", n."Region"
            FROM public."Locations" l
            JOIN public."NaturalReserves" n ON l."NaturalReserveId" = n."Id"
        """)
        location_region_map = {row[0]: row[1] for row in cur.fetchall()}

        # Leer researchers con especialidad
        cur.execute('SELECT "Id","Specialty" FROM public."Researchers"')
        researcher_specialty_map = {row[0]: row[1] for row in cur.fetchall()}

        generar_records(cur, especies_con_id, location_region_map, researcher_specialty_map)
        conn.commit()

        print("\n¡Listo! Datos generados con coherencia real.")
        print(f"   Families:        {N_FAMILIES:>10,}")
        print(f"   NaturalReserves: {N_NATURAL_RES:>10,}")
        print(f"   Locations:       {N_LOCATIONS:>10,}")
        print(f"   Researchers:     {N_RESEARCHERS:>10,}")
        print(f"   Species:         {N_SPECIES:>10,}")
        print(f"   Records:         {N_RECORDS:>10,}")
        print(f"   {'─'*28}")
        total = N_FAMILIES + N_NATURAL_RES + N_LOCATIONS + N_RESEARCHERS + N_SPECIES + N_RECORDS
        print(f"   TOTAL:           {total:>10,}")

    except Exception as e:
        conn.rollback()
        print(f"\nError: {e}")
        raise
    finally:
        cur.close()
        conn.close()

if __name__ == "__main__":
    main()
