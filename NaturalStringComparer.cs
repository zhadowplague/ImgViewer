using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ImgViewer
{
	public class NaturalStringComparer : IComparer<string>
	{
		[DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		private static extern int StrCmpLogicalW(string x, string y);

		public int Compare(string x, string y)
		{
			return StrCmpLogicalW(x ?? string.Empty, y ?? string.Empty);
		}
	}
}
