using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionKit.Extras
{
	internal static class InputTools
	{
		/// <summary>
		/// Written by MagicaJaphet, taken from the Hooking Library. Copies the logic of most debug inputs and their single press checks, ensuring the input logic isn't being passed through more than once per input.
		/// </summary>
		/// <param name="flag"></param>
		/// <param name="keyCodes"></param>
		/// <returns></returns>
		internal static bool CheckForSingleInput(ref bool flag, params KeyCode[] keyCodes)
		{
			bool checkInput = (from k in keyCodes select Input.GetKey(k)).All(x => x);
			if (checkInput && !flag)
			{
				flag = checkInput;
				return true;
			}
			flag = checkInput;
			return false;
		}
	}
}
