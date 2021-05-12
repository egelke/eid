using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egelke.Eid.Client.Test
{
    public class ReadersFixture : IDisposable
    {
        public Readers Readers { get; }

        public ReadersFixture()
        {
            Readers = new Readers(ReaderScope.User);
        }

        public void Dispose()
        {
            Readers.Dispose();
        }
    }
}
