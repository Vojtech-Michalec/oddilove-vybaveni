INSERT INTO kategorie_vybaveni (nazev) VALUES
    ('Stan / přístřeší'),
    ('Lano / lezení'),
    ('Nástroje'),
    ('Elektronika'),
    ('Oblečení / výstroj'),
    ('Vaření / strava'),
    ('Ostatní')
ON CONFLICT (nazev) DO NOTHING;
