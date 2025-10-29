// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild



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
                _glassCount = m_io.ReadU2be();
                _plasticCount = m_io.ReadU2be();
                _metalCount = m_io.ReadU2be();
                _glassCountBit = m_io.ReadU2be();
                _plasticCountBit = m_io.ReadU2be();
                _metalCountBot = m_io.ReadU2be();
                _clothScrapCount = m_io.ReadU2be();
            }
            private ushort _glassCount;
            private ushort _plasticCount;
            private ushort _metalCount;
            private ushort _glassCountBit;
            private ushort _plasticCountBit;
            private ushort _metalCountBot;
            private ushort _clothScrapCount;
            private CacSave m_root;
            private CacSave m_parent;
            public ushort GlassCount { get { return _glassCount; } }
            public ushort PlasticCount { get { return _plasticCount; } }
            public ushort MetalCount { get { return _metalCount; } }
            public ushort GlassCountBit { get { return _glassCountBit; } }
            public ushort PlasticCountBit { get { return _plasticCountBit; } }
            public ushort MetalCountBot { get { return _metalCountBot; } }
            public ushort ClothScrapCount { get { return _clothScrapCount; } }
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
                _r = m_io.ReadF8be();
                _g = m_io.ReadF8be();
                _b = m_io.ReadF8be();
            }
            private double _r;
            private double _g;
            private double _b;
            private CacSave m_root;
            private CacSave.MHeader m_parent;
            public double R { get { return _r; } }
            public double G { get { return _g; } }
            public double B { get { return _b; } }
            public CacSave M_Root { get { return m_root; } }
            public CacSave.MHeader M_Parent { get { return m_parent; } }
        }
        private string _magic;
        private MHeader _header;
        private MStats _stats;
        private CacSave m_root;
        private KaitaiStruct m_parent;
        public string Magic { get { return _magic; } }
        public MHeader Header { get { return _header; } }
        public MStats Stats { get { return _stats; } }
        public CacSave M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
