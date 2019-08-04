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
        EventWaitHandle waitHandle = new AutoResetEvent(false);

        [TestMethod]
        [Timeout(60000)]
        public void WaitInScope()
        {
            using (Readers listen = new Readers(ReaderScope.User))
            {
                listen.CardInsert += (s, args) =>
                {
                    Card target = args.Card;
                    EidCard eidTarget = target as EidCard;
                    Assert.IsNotNull(target);
                    using (target)
                    {
                        target.Open();
                        if (eidTarget != null)
                        {
                            X509Certificate2 root = eidTarget.RootCert;
                            Assert.AreEqual(root.Issuer, root.Subject);
                            waitHandle.Set();
                        }
                    }
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
                readers.CardInsert += (s, e) => TestProps(e.Card as EidCard);
                EidCard target = readers.ListCards(EidCard.KNOWN_NAMES).AsQueryable().FirstOrDefault() as EidCard;
                if (target != null)
                {
                    TestProps(target);
                }
                waitHandle.WaitOne();
            }
        }

        private void TestProps(EidCard target)
        {
            try
            {
                Assert.IsNotNull(target);
                using (target)
                {
                    target.Open();
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
            finally
            {
                waitHandle.Set();
            }
        }

        [TestMethod]
        public void ReadAllFiles()
        {
            using (Readers readers = new Readers(ReaderScope.System))
            {
                readers.CardInsert += (s, e) => TestProps(e.Card as EidCard);
                EidCard target = readers.ListCards(EidCard.KNOWN_NAMES).AsQueryable().FirstOrDefault() as EidCard;
                if (target != null)
                {
                    TestFiles(target);
                }
                waitHandle.WaitOne();
            }
        }

        private void TestFiles(EidCard target)
        {
            try
            {
                Assert.IsNotNull(target);

                using (target)
                {
                    target.Open();
                    int i = 0;
                    foreach (EidFile fileId in Enum.GetValues(typeof(EidFile)))
                    {
                        i++;
                        byte[] fileData = target.ReadRaw(fileId);
                        Assert.AreNotEqual(0, fileData.Length);
                    }
                    Assert.AreEqual(10, i);
                }
            }
            finally
            {
                waitHandle.Set();
            }
        }

    }
}
