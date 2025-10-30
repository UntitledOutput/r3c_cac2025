meta:
  id: cac_save
  file-extension: cacs
  endian: le
seq:
  - id: magic
    size: 4
    type: str
    encoding: ASCII
  - id: header
    type: m_header
  - id: stats
    type: m_stats
  - id: defs
    type: m_defs
  - id: data
    type: m_sets
  
types:
  m_header:
    seq:
      - id: name
        type: str
        encoding: UTF-8
        size: 16
      - id: skin_color
        type: color
      - id: hair_color
        type: color
  m_stats:
    seq:
    - id: glass_count
      type: u2
    - id: plastic_count
      type: u2
    - id: metal_count
      type: u2
    - id: glass_count_bit
      type: u2
    - id: plastic_count_bit
      type: u2
    - id: metal_count_bit
      type: u2
    - id: cloth_scrap_count
      type: u2
  m_defs:
    seq:
    - id: ally_count
      type: u1
    - id: allies
      type: str
      size: 16
      encoding: UTF-8
      repeat: expr
      repeat-expr: ally_count
    - id: ability_count
      type: u1
    - id: abilities
      type: str
      size: 32
      encoding: UTF-8
      repeat: expr
      repeat-expr: ability_count
    - id: cloth_count
      type: u1
    - id: clothing
      type: str
      size: 16
      encoding: UTF-8
      repeat: expr
      repeat-expr: cloth_count
    - id: enm_count
      type: u1
    - id: enemies
      type: str
      size: 16
      encoding: UTF-8
      repeat: expr
      repeat-expr: enm_count
  m_sets:
    seq:
    - id: allies
      type: s1
      repeat: expr
      repeat-expr: 3
    - id: abilities
      type: ability
      repeat: expr
      repeat-expr: 3
    - id: hair
      type: s1
    - id: hat
      type: s1
    - id: shirt
      type: s1
    - id: pants
      type: s1
    - id: shoes
      type: s1
    - id: enemy_count
      type: u2
    - id: enemies
      type: s1
      repeat: expr
      repeat-expr: enemy_count
      
  ability:
    seq:
    - id: ability_index
      type: s1
    - id: upgrade_index
      type: s1
  color:
    seq:
      - id: r
        type: f4
      - id: g
        type: f4
      - id: b
        type: f4