using Cloo;
using OpenTK;
using System;

namespace CiliaElements.CL
{
    public class TCLTools : TCLBasis
    {
        #region Private Fields

        private string clProgramSource = @"

            typedef struct tag_bbox
            {
                double wmax;
                double xmax;
                double ymax;
                double zmax;
                double wmin;
                double xmin;
                double ymin;
                double zmin;
            } bbox;

            typedef struct tag_vec
            {
                double x;
                double y;
                double z;
            } vec;

            typedef struct tag_mtx
            {
                double r0w;
                double r0x;
                double r0y;
                double r0z;
                double r1w;
                double r1x;
                double r1y;
                double r1z;
                double r2w;
                double r2x;
                double r2y;
                double r2z;
                double r3w;
                double r3x;
                double r3y;
                double r3z;
            } mtx;

            kernel void Get2DBox(global  read_only bbox* a,  global write_only bbox* c, mtx p )
            {
                int index = get_global_id(0);
                bbox bb = a[index];
                bbox cc = c[index];

                cc.wmin = p.r2w * bb.zmin; cc.wmax=cc.wmin;
                double w = p.r2w * bb.zmax; cc.wmax = max(cc.wmax,w); cc.wmin = min(cc.wmin,w);
                w = 1 / cc.wmax;

                cc.xmin = p.r0x * bb.xmin * w; cc.xmax=cc.xmin;
                cc.ymin = p.r1y * bb.ymin * w; cc.ymax=cc.ymin;
                double x;
                double y;
                x = p.r0x * bb.xmax * w; cc.xmax = max(cc.xmax,x); cc.xmin = min(cc.xmin,x);
                y = p.r1y * bb.ymax * w; cc.ymax = max(cc.ymax,y); cc.ymin = min(cc.ymin,y);

                c[index] = cc;
            }

            kernel void MovePoints(global  read_only vec* a,  global write_only vec* c, mtx p )
            {
                int index = get_global_id(0);
                vec bb = a[index];
                vec cc = c[index];

                cc.x = p.r0x * bb.x + p.r1x * bb.y + p.r2x * bb.z + p.r3x;
                cc.y = p.r0y * bb.x + p.r1y * bb.y + p.r2y * bb.z + p.r3y;
                cc.z = p.r0z * bb.x + p.r1z * bb.y + p.r2z * bb.z + p.r3z;

                c[index] = cc;
            }

            ";

        private ComputeCommandQueue commands;
        private ComputeContext context;
        private ComputeEventList eventList = new ComputeEventList();
        private ComputeKernel Get2DBox;
        private ComputeKernel MovePoints;
        private ComputeProgram program;

        #endregion Private Fields

        #region Public Constructors

        public TCLTools(ComputeContext iContext)
        {
            context = iContext;
            program = new ComputeProgram(iContext, clProgramSource);
            program.Build(null, null, null, IntPtr.Zero);
            Get2DBox = program.CreateKernel("Get2DBox");
            MovePoints = program.CreateKernel("MovePoints");
            commands = new ComputeCommandQueue(iContext, iContext.Devices[0], ComputeCommandQueueFlags.None);
        }

        #endregion Public Constructors

        #region Internal Methods

        internal void Dispose()
        {
            commands.Dispose();
            context.Dispose();
            Get2DBox.Dispose();
            MovePoints.Dispose();
            program.Dispose();
        }

        internal SBoundingBox[] RunGet2DBox(SBoundingBox[] values, Mtx4 pMatrix)
        {
            if (values.Length == 0)
            {
                return new SBoundingBox[] { };
            }

            ComputeBuffer<SBoundingBox> a = new ComputeBuffer<SBoundingBox>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, values);
            ComputeBuffer<SBoundingBox> c = new ComputeBuffer<SBoundingBox>(context, ComputeMemoryFlags.WriteOnly, values.Length);
            Get2DBox.SetMemoryArgument(0, a);
            Get2DBox.SetMemoryArgument(1, c);
            Get2DBox.SetValueArgument<Mtx4>(2, pMatrix);
            commands.Execute(Get2DBox, null, new long[] { values.Length }, null, eventList);
            SBoundingBox[] arrC = new SBoundingBox[values.Length];
            commands.ReadFromBuffer(c, ref arrC, false, eventList);
            commands.Finish();
            return arrC;
        }

        internal Vec3[] RunMovePoints(Vec3[] pts, Mtx4 matrix)
        {
            if (pts.Length == 0)
            {
                return new Vec3[] { };
            }

            Vec3[] arrC = new Vec3[pts.Length];

            ComputeBuffer<Vec3> a = new ComputeBuffer<Vec3>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, pts);
            ComputeBuffer<Vec3> c = new ComputeBuffer<Vec3>(context, ComputeMemoryFlags.WriteOnly, pts.Length);
            MovePoints.SetMemoryArgument(0, a);
            MovePoints.SetMemoryArgument(1, c);
            MovePoints.SetValueArgument<Mtx4>(2, matrix);
            //lock (MovePoints)
            //{
            commands.Execute(MovePoints, null, new long[] { pts.Length }, null, eventList);
            commands.ReadFromBuffer(c, ref arrC, false, eventList);
            commands.Finish();
            //}
            return arrC;
        }

        #endregion Internal Methods
    }
}