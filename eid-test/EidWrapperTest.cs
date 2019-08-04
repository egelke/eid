using Egelke.Eid.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Drawing;
using System.Linq;
using System.IO;

namespace Egelke.Eid.Client.Test
{
    
    
    /// <summary>
    ///This is a test class for EidWrapperTest and is intended
    ///to contain all EidWrapperTest Unit Tests
    ///</summary>
    [TestClass]
    public class EidWrapperTest
    {
        [TestMethod]
        [Timeout(60000)]
        public void WaitInScope()
        {
            using (Readers listen = new Readers(ReaderScope.User))
            {
                EventWaitHandle waitHandle = new AutoResetEvent(false);
                listen.CardInsert += (s, args) =>
                {
                    Assert.IsNotNull(args.Card);
                    waitHandle.Set();
                };
                waitHandle.WaitOne();
            }
        }

        /// <summary>
        ///A test for ReadCertificate
        ///</summary>
        [TestMethod]
        public void ReadProperties()
        {
            using (Readers readers = new Readers(ReaderScope.System))
            {

                EidCard target = (EidCard) readers.ListCards(EidCard.KNOWN_NAMES).AsQueryable().FirstOrDefault();
                Assert.IsNotNull(target);
                target.Open();
                using (target)
                {
                    
                    X509Certificate2 auth = target.AuthCert;
                    X509Certificate2 sign = target.SignCert;
                    X509Certificate2 ca = target.CaCert;
                    X509Certificate2 root = target.RootCert;
                    X509Certificate2 rrn = target.RrnCert;
                    Image pic = Image.FromStream(new MemoryStream(target.Picture));

                    Assert.AreNotEqual(auth.Subject, sign.Subject);
                    Assert.AreEqual(sign.Issuer, ca.Subject);
                    Assert.AreEqual(auth.Issuer, ca.Subject);
                    Assert.AreEqual(ca.Issuer, root.Subject);
                    Assert.AreEqual(root.Issuer, root.Subject);
                    Assert.AreEqual(rrn.Issuer, root.Subject);
                    Assert.AreEqual(new Size(140, 200), pic.Size);
                }
            }
        }

        [TestMethod]
        public void ReadAllFiles()
        {
            using (Readers readers = new Readers(ReaderScope.System))
            {
                EidCard target = (EidCard)readers.ListCards(EidCard.KNOWN_NAMES).AsQueryable().FirstOrDefault();
                Assert.IsNotNull(target);
                target.Open();
                using (target)
                {
                    int i = 0;
                    foreach(EidFile fileId in Enum.GetValues(typeof(EidFile)))
                    {
                        i++;
                        byte[] fileData = target.ReadRaw(fileId);
                        Assert.AreNotEqual(0, fileData.Length);
                    }
                    Assert.AreEqual(10, i);
                }
            }
        }

    }
}
