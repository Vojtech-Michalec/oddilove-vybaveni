CREATE TABLE IF NOT EXISTS kategorie_vybaveni (
    id     SERIAL PRIMARY KEY,
    nazev  VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS vybaveni (
    id           SERIAL PRIMARY KEY,
    nazev        VARCHAR(200) NOT NULL,
    popis        TEXT,
    pocet_kusu   INTEGER NOT NULL DEFAULT 1 CHECK (pocet_kusu >= 1),
    kategorie_id INTEGER NOT NULL REFERENCES kategorie_vybaveni(id)
);

CREATE TABLE IF NOT EXISTS vypujcka (
    id              SERIAL PRIMARY KEY,
    vybaveni_id     INTEGER NOT NULL REFERENCES vybaveni(id) ON DELETE CASCADE,
    jmeno_cloveka   VARCHAR(200) NOT NULL,
    datum_vypujcky  DATE NOT NULL DEFAULT CURRENT_DATE,
    datum_vraceni   DATE,
    poznamka        TEXT,
    CONSTRAINT datum_check CHECK (datum_vraceni IS NULL OR datum_vraceni >= datum_vypujcky)
);
