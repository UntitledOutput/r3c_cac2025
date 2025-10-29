meta:
  id: cac_save
  file-extension: cacs
  endian: be
seq:
  - id: magic
    size: 4
    type: str
    encoding: ASCII
  - id: header
    type: m_header
  - id: stats
    type: m_stats
  
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
    - id: metal_count_bot
      type: u2
    - id: cloth_scrap_count
      type: u2
  color:
    seq:
      - id: r
        type: f8
      - id: g
        type: f8
      - id: b
        type: f8