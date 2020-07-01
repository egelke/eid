using System;
using System.Collections.Generic;
using System.Text;

namespace Egelke.Eid.Client.Model
{
	[Flags]
    public enum Spec
    {
		None			= 0x00,
		WhiteCane		= 0x01,
		ExtendedMinor	= 0x02,
		YellowCane		= 0x04
	}
}
