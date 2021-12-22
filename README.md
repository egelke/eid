.Net eID Client
===

Native .Net client of the Belgium eID card, directly uses the WinSCard library of Windows.  It allows to read the files on the Belgium eID card.

Installing
---
Install the Egelke.Eid.Client NuGet package:

    PM> Install-Package Egelke.Eid.Client

Prerequisites
---
* Compatible reader
* Fedict minidriver (part of eID Middleware)
* Resent windows (tested on Windows 11)

Getting Started
---
    using (Readers readers = new Readers(ReaderScope.User))
    {
        EidCard eid = (EidCard)readers.ListCards().Where(c => c is EidCard).FirstOrDefault();
        using (eid)
        {
            eid.Open();
            X509Certificate2 auth = eid.AuthCert;
            Model.Identity identity = eid.Identity;
        }
    }

License
---
GNU Lesser General Public License
