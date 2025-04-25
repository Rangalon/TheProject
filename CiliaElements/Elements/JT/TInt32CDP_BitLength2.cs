using System;
using System.IO;

namespace CiliaElements.FormatJT
{
    public abstract class TInt32CDP
    {
        #region Public Fields

        public int[] Datas;

        #endregion Public Fields

        #region Protected Fields

        protected BinaryReader Reader;

        #endregion Protected Fields

        #region Private Fields

        private int outValCount;

        #endregion Private Fields

        #region Public Constructors

        public TInt32CDP(BinaryReader rdr)
        {
            Reader = rdr;
        }

        #endregion Public Constructors

        #region Public Methods

        public abstract void Load(int nb);

        public void SetOutValCount(int i)
        {
            outValCount = i;
        }

        #endregion Public Methods
    }

    public class TInt32CDPArithmetic : TInt32CDP
    {
        #region Public Constructors

        public TInt32CDPArithmetic(BinaryReader rdr)
            : base(rdr)
        { }

        #endregion Public Constructors

        #region Public Methods

        public override void Load(int nb)
        {
            Datas = new int[nb];
            for (int i = 0; i < nb; i++)
                Datas[i] = Reader.ReadInt32();
            //Reader.ReadInt32();
            //Reader.ReadInt32();
            //Reader.ReadInt16();
            ////
            //Int64 mem = Reader.BaseStream.Position;
            //List<byte> bts=new List<byte>();
            //for(int i=0;i<100;i++)
            //    bts.Add(Reader.ReadByte());
            //Reader.BaseStream.Position=mem;
        }

        #endregion Public Methods
    }

    public class TInt32CDPBitLength2 : TInt32CDP
    {
        #region Public Constructors

        public TInt32CDPBitLength2(BinaryReader rdr)
            : base(rdr)
        { }

        #endregion Public Constructors

        #region Public Methods

        public override void Load(int nb)
        {
            Datas = new int[nb];
            for (int i = 0; i < nb; i++)
                Datas[i] = Reader.ReadInt32();
        }

        #endregion Public Methods
    }

    public class TInt32CDPNull : TInt32CDP
    {
        #region Public Constructors

        public TInt32CDPNull(BinaryReader rdr)
            : base(rdr)
        { }

        #endregion Public Constructors

        #region Public Methods

        public override void Load(int nb)
        {
            Datas = new int[nb];
            for (int i = 0; i < nb; i++)
                Datas[i] = Reader.ReadInt32();
        }

        #endregion Public Methods
    }
}