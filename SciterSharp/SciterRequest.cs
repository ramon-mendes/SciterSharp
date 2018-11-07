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
using System.Diagnostics;
using System.Runtime.InteropServices;
using SciterSharp.Interop;

namespace SciterSharp
{
	public class SciterRequest
	{
		private static SciterXRequest.ISciterRequestAPI _rapi = SciterX.RequestAPI;
		public readonly IntPtr _hrq;

		private SciterRequest() { }

		public SciterRequest(IntPtr hrq)
		{
			_hrq = hrq;
		}

		public string Url
		{
			get
			{
				string strval = null;
				SciterXDom.FPTR_LPCSTR_RECEIVER frcv = (IntPtr str, uint str_length, IntPtr param) =>
				{
					strval = Marshal.PtrToStringAnsi(str, (int)str_length);
				};

				_rapi.RequestUrl(_hrq, frcv, IntPtr.Zero);
				return strval;
			}
		}

		public string ContentUrl
		{
			get
			{
				string strval = null;
				SciterXDom.FPTR_LPCSTR_RECEIVER frcv = (IntPtr str, uint str_length, IntPtr param) =>
				{
					strval = Marshal.PtrToStringAnsi(str, (int)str_length);
				};

				_rapi.RequestContentUrl(_hrq, frcv, IntPtr.Zero);
				return strval;
			}
		}

		public SciterXRequest.SciterResourceType RequestedType
		{
			get
			{
				SciterXRequest.SciterResourceType rv;
				_rapi.RequestGetRequestedDataType(_hrq, out rv);
				return rv;
			}
		}

		public void Suceeded(uint status, byte[] dataOrNull = null)
		{
			_rapi.RequestSetSucceeded(_hrq, status, dataOrNull, dataOrNull == null ? 0 : (uint)dataOrNull.Length);
		}

		public void Failed(uint status, byte[] dataOrNull = null)
		{
			_rapi.RequestSetFailed(_hrq, status, dataOrNull, dataOrNull == null ? 0 : (uint)dataOrNull.Length);
		}

		public void AppendData(byte[] data)
		{
			_rapi.RequestAppendDataChunk(_hrq, data, (uint)data.Length);
		}
	}
}