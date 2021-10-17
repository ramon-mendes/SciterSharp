// Copyright 2016 Ramon F. Mendes
//
// This file is part of SciterSharp.
// 
// SciterSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SciterSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with SciterSharp.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SciterSharp.Interop
{
	public static class SciterXValue
	{
		public enum VALUE_RESULT : int
		{
			HV_OK_TRUE = -1,
			HV_OK = 0,
			HV_BAD_PARAMETER = 1,
			HV_INCOMPATIBLE_TYPE = 2
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct VALUE
		{
			public uint     t;// type: enum VALUE_TYPE
			public uint     u;// unit
			public ulong    d;// data
		}

		public enum VALUE_TYPE : uint
		{
			T_UNDEFINED = 0,
			T_NULL = 1,
			T_BOOL = 2,
			T_INT = 3,
			T_FLOAT = 4,
			T_STRING = 5,
			T_DATE = 6,     // INT64 - contains a 64-bit value representing the number of 100-nanosecond intervals since January 1, 1601 (UTC), a.k.a. FILETIME on Windows
			T_CURRENCY = 7, // INT64 - 14.4 fixed number. E.g. dollars = int64 / 10000; 
			T_LENGTH = 8,   // length units, value is int or float, units are VALUE_UNIT_TYPE
			T_ARRAY = 9,
			T_MAP = 10,
			T_FUNCTION = 11,    // named tuple , like array but with name tag
			T_BYTES = 12,		// sequence of bytes - e.g. image data
			T_OBJECT = 13,		// scripting object proxy (TISCRIPT/SCITER)
			T_DOM_OBJECT = 14,  // DOM object (CSSS!), use get_object_data to get HELEMENT 
			//T_RESOURCE = 15,  // 15 - other thing derived from tool::resource
			//T_RANGE = 16,     // 16 - N..M, integer range.
			T_DURATION = 17,	// double, seconds
			T_ANGLE = 18,		// double, radians
			T_COLOR = 19,		// [unsigned] INT, ABGR
		}

		public enum VALUE_UNIT_TYPE : uint
		{
			UT_EM = 1, //height of the element's font. 
			UT_EX = 2, //height of letter 'x' 
			UT_PR = 3, //%
			UT_SP = 4, //%% "springs", a.k.a. flex units
			reserved1 = 5, 
			reserved2 = 6, 
			UT_PX = 7, //pixels
			UT_IN = 8, //inches (1 inch = 2.54 centimeters). 
			UT_CM = 9, //centimeters. 
			UT_MM = 10, //millimeters. 
			UT_PT = 11, //points (1 point = 1/72 inches). 
			UT_PC = 12, //picas (1 pica = 12 points). 
			UT_DIP = 13,
			reserved3 = 14,
			reserved4 = 15,
			UT_URL   = 16,  // url in string
		}

		public enum VALUE_UNIT_TYPE_DATE : uint
		{
			DT_HAS_DATE         = 0x01, // date contains date portion
			DT_HAS_TIME         = 0x02, // date contains time portion HH:MM
			DT_HAS_SECONDS      = 0x04, // date contains time and seconds HH:MM:SS
			DT_UTC              = 0x10, // T_DATE is known to be UTC. Otherwise it is local date/time
		}

		public enum VALUE_UNIT_TYPE_OBJECT : uint
		{
			UT_OBJECT_ARRAY  = 0,   // type T_OBJECT of type Array
			UT_OBJECT_OBJECT = 1,   // type T_OBJECT of type Object
			UT_OBJECT_CLASS  = 2,   // type T_OBJECT of type Class (class or namespace)
			UT_OBJECT_NATIVE = 3,   // type T_OBJECT of native Type with data slot (LPVOID)
			UT_OBJECT_FUNCTION = 4, // type T_OBJECT of type Function
			UT_OBJECT_ERROR = 5,    // type T_OBJECT of type Error
		}

		// Sciter or TIScript specific
		public enum VALUE_UNIT_TYPE_STRING : uint
		{
			UT_STRING_STRING = 0,		// string
			UT_STRING_ERROR  = 1,		// is an error string
			UT_STRING_SECURE = 2,		// secure string ("wiped" on destroy)
			UT_STRING_SYMBOL = 0xffff	// symbol in tiscript sense
		}

		// Native functor
		// alias NATIVE_FUNCTOR_INVOKE = void function(VOID* tag, UINT argc, const VALUE* argv, VALUE* retval);// retval may contain error definition
		public delegate bool FPTR_NATIVE_FUNCTOR_INVOKE(IntPtr tag, uint argc, IntPtr argv, out VALUE retval);
		// alias NATIVE_FUNCTOR_RELEASE = void function(VOID* tag);
		public delegate bool FPTR_NATIVE_FUNCTOR_RELEASE(IntPtr tag);

		// alias BOOL function(LPVOID param, const VALUE* pkey, const VALUE* pval) KeyValueCallback;
		public delegate bool FPTR_KeyValueCallback(IntPtr param, ref VALUE pkey, ref VALUE pval);


		public enum VALUE_STRING_CVT_TYPE : uint
		{
			CVT_SIMPLE,        //< simple conversion of terminal values 
			CVT_JSON_LITERAL,  //< json literal parsing/emission 
			CVT_JSON_MAP,      //< json parsing/emission, it parses as if token '{' already recognized
			CVT_XJSON_LITERAL, //< x-json parsing/emission, date is emitted as ISO8601 date literal, currency is emitted in the form DDDD$CCC
		}
	}
}