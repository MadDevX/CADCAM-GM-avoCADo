namespace avoCADo.Algebra
{
    public struct IntersectionData
    {
        public ISurface p;
        public ISurface q;

        public IntersectionData(ISurface a, ISurface b)
        {
            this.p = a;
            this.q = b;
        }
    }
}
