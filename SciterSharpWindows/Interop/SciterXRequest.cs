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
	public static class SciterXRequest
	{
		public enum REQUEST_RESULT
		{
			REQUEST_PANIC = -1, // e.g. not enough memory
			REQUEST_OK = 0,
			REQUEST_BAD_PARAM = 1,  // bad parameter
			REQUEST_FAILURE = 2,    // operation failed, e.g. index out of bounds
			REQUEST_NOTSUPPORTED = 3 // the platform does not support requested feature
		}

		public enum REQUEST_RQ_TYPE : uint
		{
			RRT_GET = 1,
			RRT_POST = 2,
			RRT_PUT = 3,
			RRT_DELETE = 4,
			RRT_FORCE_DWORD = 0xffffffff
		}

		public enum SciterResourceType : uint
		{
			RT_DATA_HTML = 0,
			RT_DATA_IMAGE = 1,
			RT_DATA_STYLE = 2,
			RT_DATA_CURSOR = 3,
			RT_DATA_SCRIPT = 4,
			RT_DATA_RAW = 5,
			RT_DATA_FONT,
			RT_DATA_SOUND,    // wav bytes
			RT_DATA_FORCE_DWORD = 0xffffffff
		}

		public enum REQUEST_STATE : uint
		{
			RS_PENDING = 0,
			RS_SUCCESS = 1, // completed successfully
			RS_FAILURE = 2, // completed with failure

			RS_FORCE_DWORD = 0xffffffff
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ISciterRequestAPI
		{
			public FPTR_RequestUse						RequestUse;
			public FPTR_RequestUnUse					RequestUnUse;
			public FPTR_RequestUrl						RequestUrl;
			public FPTR_RequestContentUrl				RequestContentUrl;
			public FPTR_RequestGetRequestType			RequestGetRequestType;
			public FPTR_RequestGetRequestedDataType		RequestGetRequestedDataType;
			public FPTR_RequestGetReceivedDataType		RequestGetReceivedDataType;
			public FPTR_RequestGetNumberOfParameters	RequestGetNumberOfParameters;
			public FPTR_RequestGetNthParameterName		RequestGetNthParameterName;
			public FPTR_RequestGetNthParameterValue		RequestGetNthParameterValue;
			public FPTR_RequestGetTimes					RequestGetTimes;
			public FPTR_RequestGetNumberOfRqHeaders		RequestGetNumberOfRqHeaders;
			public FPTR_RequestGetNthRqHeaderName		RequestGetNthRqHeaderName;
			public FPTR_RequestGetNthRqHeaderValue		RequestGetNthRqHeaderValue;
			public FPTR_RequestGetNumberOfRspHeaders	RequestGetNumberOfRspHeaders;
			public FPTR_RequestGetNthRspHeaderName		RequestGetNthRspHeaderName;
			public FPTR_RequestGetNthRspHeaderValue		RequestGetNthRspHeaderValue;
			public FPTR_RequestGetCompletionStatus		RequestGetCompletionStatus;
			public FPTR_RequestGetProxyHost				RequestGetProxyHost;
			public FPTR_RequestGetProxyPort				RequestGetProxyPort;
			public FPTR_RequestSetSucceeded				RequestSetSucceeded;
			public FPTR_RequestSetFailed				RequestSetFailed;
			public FPTR_RequestAppendDataChunk			RequestAppendDataChunk;
			public FPTR_RequestSetRqHeader				RequestSetRqHeader;
			public FPTR_RequestSetRspHeader				RequestSetRspHeader;
			public FPTR_RequestGetData					RequestGetData;


			// a.k.a AddRef()
			// REQUEST_RESULT SCFN(RequestUse)( HREQUEST rq );
			public delegate REQUEST_RESULT FPTR_RequestUse(IntPtr rq);

			// a.k.a Release()
			// REQUEST_RESULT SCFN(RequestUnUse)( HREQUEST rq );
			public delegate REQUEST_RESULT FPTR_RequestUnUse(IntPtr rq);

			// get requested URL
			// REQUEST_RESULT SCFN(RequestUrl)( HREQUEST rq, LPCSTR_RECEIVER* rcv, LPVOID rcv_param );
			public delegate REQUEST_RESULT FPTR_RequestUrl(IntPtr rq, SciterXDom.FPTR_LPCSTR_RECEIVER rcv, IntPtr rcv_param);

			// get real, content URL (after possible redirection)
			// REQUEST_RESULT SCFN(RequestContentUrl)( HREQUEST rq, LPCSTR_RECEIVER* rcv, LPVOID rcv_param );
			public delegate REQUEST_RESULT FPTR_RequestContentUrl(IntPtr rq, SciterXDom.FPTR_LPCSTR_RECEIVER rcv, IntPtr rcv_param);

			// get requested data type
			// REQUEST_RESULT SCFN(RequestGetRequestType)( HREQUEST rq, REQUEST_RQ_TYPE* pType );
			public delegate REQUEST_RESULT FPTR_RequestGetRequestType(IntPtr rq, out REQUEST_RQ_TYPE pType);

			// get requested data type
			// REQUEST_RESULT SCFN(RequestGetRequestedDataType)( HREQUEST rq, SciterResourceType* pData );
			public delegate REQUEST_RESULT FPTR_RequestGetRequestedDataType(IntPtr rq, out SciterResourceType pData);

			// get received data type, string, mime type
			// REQUEST_RESULT SCFN(RequestGetReceivedDataType)( HREQUEST rq, LPCSTR_RECEIVER* rcv, LPVOID rcv_param );
			public delegate REQUEST_RESULT FPTR_RequestGetReceivedDataType(IntPtr rq, SciterXDom.FPTR_LPCSTR_RECEIVER rcv, IntPtr rcv_param);


			// get number of request parameters passed
			// REQUEST_RESULT SCFN(RequestGetNumberOfParameters)( HREQUEST rq, UINT* pNumber );
			public delegate REQUEST_RESULT FPTR_RequestGetNumberOfParameters(IntPtr rq, out uint pNumber);

			// get nth request parameter name
			// REQUEST_RESULT SCFN(RequestGetNthParameterName)( HREQUEST rq, UINT n, LPCWSTR_RECEIVER* rcv, LPVOID rcv_param  );
			public delegate REQUEST_RESULT FPTR_RequestGetNthParameterName(IntPtr rq, uint n, SciterXDom.FPTR_LPCWSTR_RECEIVER rcv, IntPtr rcv_param);

			// get nth request parameter value
			// REQUEST_RESULT SCFN(RequestGetNthParameterValue)( HREQUEST rq, UINT n, LPCWSTR_RECEIVER* rcv, LPVOID rcv_param  );
			public delegate REQUEST_RESULT FPTR_RequestGetNthParameterValue(IntPtr rq, uint n, SciterXDom.FPTR_LPCWSTR_RECEIVER rcv, IntPtr rcv_param);

			// get request times , ended - started = milliseconds to get the requst
			// REQUEST_RESULT SCFN(RequestGetTimes)( HREQUEST rq, UINT* pStarted, UINT* pEnded );
			public delegate REQUEST_RESULT FPTR_RequestGetTimes(IntPtr rq, out uint pStarted, out uint pEnded);

			// get number of request headers
			// REQUEST_RESULT SCFN(RequestGetNumberOfRqHeaders)( HREQUEST rq, UINT* pNumber );
			public delegate REQUEST_RESULT FPTR_RequestGetNumberOfRqHeaders(IntPtr rq, out uint pNumber);

			// get nth request header name 
			// REQUEST_RESULT SCFN(RequestGetNthRqHeaderName)( HREQUEST rq, UINT n, LPCWSTR_RECEIVER* rcv, LPVOID rcv_param );
			public delegate REQUEST_RESULT FPTR_RequestGetNthRqHeaderName(IntPtr rq, uint n, SciterXDom.FPTR_LPCWSTR_RECEIVER rcv, IntPtr rcv_param);

			// get nth request header value 
			// REQUEST_RESULT SCFN(RequestGetNthRqHeaderValue)( HREQUEST rq, UINT n, LPCWSTR_RECEIVER* rcv, LPVOID rcv_param );
			public delegate REQUEST_RESULT FPTR_RequestGetNthRqHeaderValue(IntPtr rq, uint n, SciterXDom.FPTR_LPCWSTR_RECEIVER rcv, IntPtr rcv_param);

			// get number of response headers
			// REQUEST_RESULT SCFN(RequestGetNumberOfRspHeaders)( HREQUEST rq, UINT* pNumber );
			public delegate REQUEST_RESULT FPTR_RequestGetNumberOfRspHeaders(IntPtr rq, out uint pNumber);

			// get nth response header name 
			// REQUEST_RESULT SCFN(RequestGetNthRspHeaderName)( HREQUEST rq, UINT n, LPCWSTR_RECEIVER* rcv, LPVOID rcv_param );
			public delegate REQUEST_RESULT FPTR_RequestGetNthRspHeaderName(IntPtr rq, uint n, SciterXDom.FPTR_LPCWSTR_RECEIVER rcv, IntPtr rcv_param);

			// get nth response header value 
			// REQUEST_RESULT SCFN(RequestGetNthRspHeaderValue)( HREQUEST rq, UINT n, LPCWSTR_RECEIVER* rcv, LPVOID rcv_param );
			public delegate REQUEST_RESULT FPTR_RequestGetNthRspHeaderValue(IntPtr rq, uint n, SciterXDom.FPTR_LPCWSTR_RECEIVER rcv, IntPtr rcv_param);

			// get completion status (CompletionStatus - http response code : 200, 404, etc.)
			// REQUEST_RESULT SCFN(RequestGetCompletionStatus)( HREQUEST rq, REQUEST_STATE* pState, UINT* pCompletionStatus );
			public delegate REQUEST_RESULT FPTR_RequestGetCompletionStatus(IntPtr rq, out REQUEST_STATE pState, out uint pCompletionStatus);

			// get proxy host
			// REQUEST_RESULT SCFN(RequestGetProxyHost)( HREQUEST rq, LPCSTR_RECEIVER* rcv, LPVOID rcv_param );
			public delegate REQUEST_RESULT FPTR_RequestGetProxyHost(IntPtr rq, SciterXDom.FPTR_LPCSTR_RECEIVER rcv, IntPtr rcv_param);

			// get proxy port
			// REQUEST_RESULT SCFN(RequestGetProxyPort)( HREQUEST rq, UINT* pPort );
			public delegate REQUEST_RESULT FPTR_RequestGetProxyPort(IntPtr rq, out uint pPort);

			// mark reequest as complete with status and data 
			// REQUEST_RESULT SCFN(RequestSetSucceeded)( HREQUEST rq, UINT status, LPCBYTE dataOrNull, UINT dataLength);
			public delegate REQUEST_RESULT FPTR_RequestSetSucceeded(IntPtr rq, uint status, byte[] dataOrNull, uint dataLength);

			// mark reequest as complete with failure and optional data 
			// REQUEST_RESULT SCFN(RequestSetFailed)( HREQUEST rq, UINT status, LPCBYTE dataOrNull, UINT dataLength );
			public delegate REQUEST_RESULT FPTR_RequestSetFailed(IntPtr rq, uint status, byte[] dataOrNull, uint dataLength);

			// append received data chunk 
			// REQUEST_RESULT SCFN(RequestAppendDataChunk)( HREQUEST rq, LPCBYTE data, UINT dataLength );
			public delegate REQUEST_RESULT FPTR_RequestAppendDataChunk(IntPtr rq, byte[] data, uint dataLength);

			// set request header (single item)
			// REQUEST_RESULT SCFN(RequestSetRqHeader)( HREQUEST rq, LPCWSTR name, LPCWSTR value );
			public delegate REQUEST_RESULT FPTR_RequestSetRqHeader(IntPtr rq, [MarshalAs(UnmanagedType.LPWStr)]string name, [MarshalAs(UnmanagedType.LPWStr)]string value);

			// set respone header (single item)
			// REQUEST_RESULT SCFN(RequestSetRspHeader)( HREQUEST rq, LPCWSTR name, LPCWSTR value );
			public delegate REQUEST_RESULT FPTR_RequestSetRspHeader(IntPtr rq, [MarshalAs(UnmanagedType.LPWStr)]string name, [MarshalAs(UnmanagedType.LPWStr)]string value);

			// set received data type, string, mime type
			// REQUEST_RESULT SCFN(RequestSetReceivedDataType)( HREQUEST rq, LPCSTR type );
			public delegate REQUEST_RESULT FPTR_RequestSetReceivedDataType(IntPtr rq, [MarshalAs(UnmanagedType.LPStr)]string type);

			// set received data encoding, string
			// REQUEST_RESULT SCFN(RequestSetReceivedDataEncoding)( HREQUEST rq, LPCSTR encoding );
			public delegate REQUEST_RESULT FPTR_RequestSetReceivedDataEncoding(IntPtr rq, [MarshalAs(UnmanagedType.LPStr)]string encoding);

			// get received (so far) data
			// REQUEST_RESULT SCFN(RequestGetData)( HREQUEST rq, LPCBYTE_RECEIVER* rcv, LPVOID rcv_param );
			public delegate REQUEST_RESULT FPTR_RequestGetData(IntPtr rq, SciterXDom.FPTR_LPCBYTE_RECEIVER rcv, IntPtr rcv_param);
		}
	}
}