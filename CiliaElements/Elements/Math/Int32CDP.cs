using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiliaElements.Elements
{
    public abstract class Int32CDP
    {
        #region Public Enums

        public enum ECodecType
        {
            Null = 0,
            BitLength2 = 1,
            Arithmetic = 3,
            Chopper = 4,
            MTF = 5
        }

        #endregion Public Enums

        #region Public Methods

        public static int[] ReadInt32CDP(BinaryReader rdr) // p240
        {
            int iCur;
            int pcCode;
            int ValueCount = rdr.ReadInt32(); // ok
            if (ValueCount == 0) return new int[ValueCount];
            ECodecType CODECType = (ECodecType)rdr.ReadByte(); // ok
            if (CODECType == ECodecType.Chopper || CODECType == ECodecType.MTF)
                throw new Exception("not managed");
            else
            {
                int CodeTextLength = rdr.ReadInt32(); // ok
                int nb = (CodeTextLength + 31) / 32;
                uint[] CodeText = new uint[nb];
                for (int j = 0; j < nb; j++) CodeText[j] = rdr.ReadUInt32();

                if (CODECType == ECodecType.Arithmetic)
                {
                    uint ProbabilityContextTableEntryCount = rdr.ReadUInt32(); //Int32 Compressed Data Packet : OOB
                    uint k = ProbabilityContextTableEntryCount >> 16;
                    uint NumberSymbolBits = rdr.ReadUInt32();
                    uint NumberOccurrenceCountBits = rdr.ReadUInt32();
                    uint NumberValueBits = rdr.ReadUInt32();
                    uint MinValue = rdr.ReadUInt32();
                    List<int> lst = new List<int>();
                    for (int i = 0; i < ProbabilityContextTableEntryCount; i++) lst.Add(rdr.ReadInt32());
                    uint AlignmentBits = rdr.ReadUInt32();
                }
                else if (CODECType == ECodecType.BitLength2)
                {
                    iCur = 0;
                    pcCode = CodeTextLength;

                    int itmp = 0;
                    int nValBits = 0;
                    int nBits = 0;
                    uint uVal = 0;
                    int cBitsInMinSymbol = 0;
                    int cBitsInMaxSymbol = 0;
                    int iMinSymbol = 0;
                    int iMaxSymbol = 0;
                    int cNumCurBits = 0;

                    Array.Reverse(CodeText);
                    Stack<uint> stc = new Stack<uint>(CodeText);

                    List<int> ostc = new List<int>();

                    int nTotalBits = 0;
                    int nSyms = 0;
                    int iSymbol = 0;

                    GetUnsignedBits(ref itmp, ref nValBits, ref uVal, ref nBits, 1, stc, ref pcCode, ref iCur);
                    if (itmp == 0)
                    {
                        GetUnsignedBits(ref cBitsInMinSymbol, ref nValBits, ref uVal, ref nBits, 6, stc, ref pcCode, ref iCur);
                        GetUnsignedBits(ref cBitsInMaxSymbol, ref nValBits, ref uVal, ref nBits, 6, stc, ref pcCode, ref iCur);
                        GetSignedBits(ref iMinSymbol, ref nValBits, ref uVal, ref nBits, cBitsInMinSymbol, stc, ref pcCode, ref iCur);
                        GetSignedBits(ref iMaxSymbol, ref nValBits, ref uVal, ref nBits, cBitsInMinSymbol, stc, ref pcCode, ref iCur);
                        cNumCurBits = nBitsInSymbol(iMaxSymbol - iMinSymbol);
                        cNumCurBits++;

                        while (nBits < nTotalBits || nSyms < ValueCount)
                        {
                            GetUnsignedBits(ref iSymbol, ref nValBits, ref uVal, ref nBits, cNumCurBits, stc, ref pcCode, ref iCur);
                            iSymbol += iMinSymbol;
                            ostc.Add(iSymbol);
                            nSyms++;
                        }
                    }
                    else
                    {
                        int iMean = 0, cBlkValBits = 0, cBlkLenBits = 0;
                        GetSignedBits(ref iMean, ref nValBits, ref uVal, ref nBits, 32, stc, ref pcCode, ref iCur);
                        GetUnsignedBits(ref cBlkValBits, ref nValBits, ref uVal, ref nBits, 3, stc, ref pcCode, ref iCur);
                        GetUnsignedBits(ref cBlkLenBits, ref nValBits, ref uVal, ref nBits, 3, stc, ref pcCode, ref iCur);

                        //

                        int cMaxFieldDecr = -(1 << (cBlkValBits - 1));
                        int cMaxFieldIncr = (1 << (cBlkValBits - 1)) - 1;
                        int cCurFieldWidth = 0, cDeltaFieldWidth = 0, cRunLen = 0, k = 0;
                        for (int ii = 0; ii < ValueCount;)
                        {
                            do
                            {
                                GetSignedBits(ref cDeltaFieldWidth, ref nValBits, ref uVal, ref nBits, cBlkValBits, stc, ref pcCode, ref iCur);
                                cCurFieldWidth += cDeltaFieldWidth;
                            } while (cDeltaFieldWidth == cMaxFieldDecr || cDeltaFieldWidth == cMaxFieldIncr);

                            GetUnsignedBits(ref cRunLen, ref nValBits, ref uVal, ref nBits, cBlkLenBits, stc, ref pcCode, ref iCur);

                            for (k = ii; k < ii + cRunLen; k++)
                            {
                                GetSignedBits(ref itmp, ref nValBits, ref uVal, ref nBits, cCurFieldWidth, stc, ref pcCode, ref iCur);
                                ostc.Add(itmp + iMean);
                            }

                            ii += cRunLen;
                        }
                    }
                    return ostc.ToArray();
                }
                else
                    throw new Exception("not managed");
                return new int[ValueCount];
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static bool getNextCodeText(ref uint uVal, ref int nValBits, Stack<uint> stc, ref int pcCode, ref int iCur)
        {
            if (stc.Count == 0) uVal = 0;
            else uVal = stc.Pop();
            nValBits = System.Math.Min(32, pcCode - 32 * iCur);
            iCur++;
            return true;
        }

        private static void GetSignedBits(ref int otmp, ref int nValBits, ref uint uVal, ref int nBits, int n, Stack<uint> stc, ref int pcCode, ref int iCur)
        {
            GetUnsignedBits(ref otmp, ref nValBits, ref uVal, ref nBits, n, stc, ref pcCode, ref iCur);
            otmp <<= (32 - n);
            otmp >>= (32 - n);
        }

        private static void GetUnsignedBits(ref int otmp, ref int nValBits, ref uint uVal, ref int nBits, int n, Stack<uint> stc, ref int pcCode, ref int iCur)
        {
            if (n == 0) otmp = 0;
            else if (nValBits >= n)
            {
                otmp = (int)(uVal >> (32 - n));
                uVal <<= n;
                uint i = 0;
                if (n == 32) i = 1;
                uVal &= i - 1;
                nValBits -= n;
                nBits += n;
            }
            else
            {
                int nlBits = nValBits;
                otmp = (int)(uVal >> (32 - n));
                nBits += nlBits;
                getNextCodeText(ref uVal, ref nValBits, stc, ref pcCode, ref iCur);
                int nrBits = (n - nlBits);
                otmp |= (int)(uVal >> (32 - nrBits));
                uVal <<= nrBits;
                uint i = 0;
                if (nrBits == 32) i = 1;
                uVal &= i - 1;
                nValBits -= nrBits;
                nBits += nrBits;
            }
        }

        private static int nBitsInSymbol(int iSymbol)
        {
            if (iSymbol == 0) return 0;
            int cMax = System.Math.Abs(iSymbol);
            int nBits = 0;
            for (int i = 1; i < cMax && nBits < 31; i += i, nBits++) ;
            return nBits;
        }

        #endregion Private Methods
    }
}