using Egelke.Eid.Client;
using NUnit.Framework;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Egelke.Eid.Client.Test
{
    
    
    /// <summary>
    ///This is a test class for EidWrapperTest and is intended
    ///to contain all EidWrapperTest Unit Tests
    ///</summary>
    [TestFixture]
    public class EidWrapperTest
    {
        [Test, ExpectedException(typeof(TimeoutException))]
        public void FailToReadEidCertificates()
        {
            using (Readers listen = new Readers(ReaderScope.User))
            {
                EidCard target = listen.WaitForEid(new TimeSpan(0, 0, 5));
            }
        }

        /// <summary>
        ///A test for ReadCertificate
        ///</summary>
        [Test]
        public void ReadEidCertificates()
        {
            using (Readers listen = new Readers(ReaderScope.User))
            {
                EidCard target = listen.WaitForEid(new TimeSpan(0, 5, 0));
                Assert.NotNull(target);
                using (target)
                {
                    X509Certificate2 auth = target.ReadCertificate(CertificateId.Authentication);
                    X509Certificate2 sign = target.ReadCertificate(CertificateId.Signature);
                    X509Certificate2 ca = target.ReadCertificate(CertificateId.CA);
                    X509Certificate2 root = target.ReadCertificate(CertificateId.Root);

                    Assert.AreNotEqual(auth.Subject, sign.Subject);
                    Assert.AreEqual(sign.Issuer, ca.Subject);
                    Assert.AreEqual(auth.Issuer, ca.Subject);
                    Assert.AreEqual(ca.Issuer, root.Subject);
                    Assert.AreEqual(root.Issuer, root.Subject);
                }
            }
        }

    }
}
