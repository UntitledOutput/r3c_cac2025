// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using System.Collections.Generic;

namespace Kaitai
{
    public partial class CacSave : KaitaiStruct
    {
        public static CacSave FromFile(string fileName)
        {
            return new CacSave(new KaitaiStream(fileName));
        }

        public CacSave(KaitaiStream p__io, KaitaiStruct p__parent = null, CacSave p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _magic = System.Text.Encoding.GetEncoding("ASCII").GetString(m_io.ReadBytes(4));
            _header = new MHeader(m_io, this, m_root);
            _stats = new MStats(m_io, this, m_root);
            _defs = new MDefs(m_io, this, m_root);
            _data = new MSets(m_io, this, m_root);
        }
        public partial class MSets : KaitaiStruct
        {
            public static MSets FromFile(string fileName)
            {
                return new MSets(new KaitaiStream(fileName));
            }

            public MSets(KaitaiStream p__io, CacSave p__parent = null, CacSave p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _allies = new List<sbyte>();
                for (var i = 0; i < 3; i++)
                {
                    _allies.Add(m_io.ReadS1());
                }
                _abilities = new List<Ability>();
                for (var i = 0; i < 3; i++)
                {
                    _abilities.Add(new Ability(m_io, this, m_root));
                }
                _hair = m_io.ReadS1();
                _hat = m_io.ReadS1();
                _shirt = m_io.ReadS1();
                _pants = m_io.ReadS1();
                _shoes = m_io.ReadS1();
                _enemyCount = m_io.ReadS2le();
                _enemies = new List<sbyte>();
                for (var i = 0; i < EnemyCount; i++)
                {
                    _enemies.Add(m_io.ReadS1());
                }
                _nextMap = System.Text.Encoding.GetEncoding("UTF-8").GetString(m_io.ReadBytes(16));
                _flagCount = m_io.ReadU1();
                _flags = new List<string>();
                for (var i = 0; i < FlagCount; i++)
                {
                    _flags.Add(System.Text.Encoding.GetEncoding("UTF-8").GetString(m_io.ReadBytes(8)));
                }
            }
            private List<sbyte> _allies;
            private List<Ability> _abilities;
            private sbyte _hair;
            private sbyte _hat;
            private sbyte _shirt;
            private sbyte _pants;
            private sbyte _shoes;
            private short _enemyCount;
            private List<sbyte> _enemies;
            private string _nextMap;
            private byte _flagCount;
            private List<string> _flags;
            private CacSave m_root;
            private CacSave m_parent;
            public List<sbyte> Allies { get { return _allies; } }
            public List<Ability> Abilities { get { return _abilities; } }
            public sbyte Hair { get { return _hair; } }
            public sbyte Hat { get { return _hat; } }
            public sbyte Shirt { get { return _shirt; } }
            public sbyte Pants { get { return _pants; } }
            public sbyte Shoes { get { return _shoes; } }
            public short EnemyCount { get { return _enemyCount; } }
            public List<sbyte> Enemies { get { return _enemies; } }
            public string NextMap { get { return _nextMap; } }
            public byte FlagCount { get { return _flagCount; } }
            public List<string> Flags { get { return _flags; } }
            public CacSave M_Root { get { return m_root; } }
            public CacSave M_Parent { get { return m_parent; } }
        }
        public partial class Color : KaitaiStruct
        {
            public static Color FromFile(string fileName)
            {
                return new Color(new KaitaiStream(fileName));
            }

            public Color(KaitaiStream p__io, CacSave.MHeader p__parent = null, CacSave p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _r = m_io.ReadF4le();
                _g = m_io.ReadF4le();
                _b = m_io.ReadF4le();
            }
            private float _r;
            private float _g;
            private float _b;
            private CacSave m_root;
            private CacSave.MHeader m_parent;
            public float R { get { return _r; } }
            public float G { get { return _g; } }
            public float B { get { return _b; } }
            public CacSave M_Root { get { return m_root; } }
            public CacSave.MHeader M_Parent { get { return m_parent; } }
        }
        public partial class MDefs : KaitaiStruct
        {
            public static MDefs FromFile(string fileName)
            {
                return new MDefs(new KaitaiStream(fileName));
            }

            public MDefs(KaitaiStream p__io, CacSave p__parent = null, CacSave p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _allyCount = m_io.ReadU1();
                _allies = new List<string>();
                for (var i = 0; i < AllyCount; i++)
                {
                    _allies.Add(System.Text.Encoding.GetEncoding("UTF-8").GetString(m_io.ReadBytes(16)));
                }
                _abilityCount = m_io.ReadU1();
                _abilities = new List<string>();
                for (var i = 0; i < AbilityCount; i++)
                {
                    _abilities.Add(System.Text.Encoding.GetEncoding("UTF-8").GetString(m_io.ReadBytes(32)));
                }
                _clothCount = m_io.ReadU1();
                _clothing = new List<string>();
                for (var i = 0; i < ClothCount; i++)
                {
                    _clothing.Add(System.Text.Encoding.GetEncoding("UTF-8").GetString(m_io.ReadBytes(16)));
                }
                _enmCount = m_io.ReadU1();
                _enemies = new List<string>();
                for (var i = 0; i < EnmCount; i++)
                {
                    _enemies.Add(System.Text.Encoding.GetEncoding("UTF-8").GetString(m_io.ReadBytes(16)));
                }
            }
            private byte _allyCount;
            private List<string> _allies;
            private byte _abilityCount;
            private List<string> _abilities;
            private byte _clothCount;
            private List<string> _clothing;
            private byte _enmCount;
            private List<string> _enemies;
            private CacSave m_root;
            private CacSave m_parent;
            public byte AllyCount { get { return _allyCount; } }
            public List<string> Allies { get { return _allies; } }
            public byte AbilityCount { get { return _abilityCount; } }
            public List<string> Abilities { get { return _abilities; } }
            public byte ClothCount { get { return _clothCount; } }
            public List<string> Clothing { get { return _clothing; } }
            public byte EnmCount { get { return _enmCount; } }
            public List<string> Enemies { get { return _enemies; } }
            public CacSave M_Root { get { return m_root; } }
            public CacSave M_Parent { get { return m_parent; } }
        }
        public partial class MHeader : KaitaiStruct
        {
            public static MHeader FromFile(string fileName)
            {
                return new MHeader(new KaitaiStream(fileName));
            }

            public MHeader(KaitaiStream p__io, CacSave p__parent = null, CacSave p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _name = System.Text.Encoding.GetEncoding("UTF-8").GetString(m_io.ReadBytes(16));
                _skinColor = new Color(m_io, this, m_root);
                _hairColor = new Color(m_io, this, m_root);
            }
            private string _name;
            private Color _skinColor;
            private Color _hairColor;
            private CacSave m_root;
            private CacSave m_parent;
            public string Name { get { return _name; } }
            public Color SkinColor { get { return _skinColor; } }
            public Color HairColor { get { return _hairColor; } }
            public CacSave M_Root { get { return m_root; } }
            public CacSave M_Parent { get { return m_parent; } }
        }
        public partial class Ability : KaitaiStruct
        {
            public static Ability FromFile(string fileName)
            {
                return new Ability(new KaitaiStream(fileName));
            }

            public Ability(KaitaiStream p__io, CacSave.MSets p__parent = null, CacSave p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _abilityIndex = m_io.ReadS1();
                _upgradeIndex = m_io.ReadS1();
            }
            private sbyte _abilityIndex;
            private sbyte _upgradeIndex;
            private CacSave m_root;
            private CacSave.MSets m_parent;
            public sbyte AbilityIndex { get { return _abilityIndex; } }
            public sbyte UpgradeIndex { get { return _upgradeIndex; } }
            public CacSave M_Root { get { return m_root; } }
            public CacSave.MSets M_Parent { get { return m_parent; } }
        }
        public partial class MStats : KaitaiStruct
        {
            public static MStats FromFile(string fileName)
            {
                return new MStats(new KaitaiStream(fileName));
            }

            public MStats(KaitaiStream p__io, CacSave p__parent = null, CacSave p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _glassCount = m_io.ReadU2le();
                _plasticCount = m_io.ReadU2le();
                _metalCount = m_io.ReadU2le();
                _glassCountBit = m_io.ReadU2le();
                _plasticCountBit = m_io.ReadU2le();
                _metalCountBit = m_io.ReadU2le();
                _clothScrapCount = m_io.ReadU2le();
            }
            private ushort _glassCount;
            private ushort _plasticCount;
            private ushort _metalCount;
            private ushort _glassCountBit;
            private ushort _plasticCountBit;
            private ushort _metalCountBit;
            private ushort _clothScrapCount;
            private CacSave m_root;
            private CacSave m_parent;
            public ushort GlassCount { get { return _glassCount; } }
            public ushort PlasticCount { get { return _plasticCount; } }
            public ushort MetalCount { get { return _metalCount; } }
            public ushort GlassCountBit { get { return _glassCountBit; } }
            public ushort PlasticCountBit { get { return _plasticCountBit; } }
            public ushort MetalCountBit { get { return _metalCountBit; } }
            public ushort ClothScrapCount { get { return _clothScrapCount; } }
            public CacSave M_Root { get { return m_root; } }
            public CacSave M_Parent { get { return m_parent; } }
        }
        private string _magic;
        private MHeader _header;
        private MStats _stats;
        private MDefs _defs;
        private MSets _data;
        private CacSave m_root;
        private KaitaiStruct m_parent;
        public string Magic { get { return _magic; } }
        public MHeader Header { get { return _header; } }
        public MStats Stats { get { return _stats; } }
        public MDefs Defs { get { return _defs; } }
        public MSets Data { get { return _data; } }
        public CacSave M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
