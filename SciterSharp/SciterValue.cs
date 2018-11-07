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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SciterSharp.Interop;
using System.Reflection;

namespace SciterSharp
{
	using FUNCTOR = Func<SciterValue[], SciterValue>;

	class SciterValueFunctor : SciterValue
	{
	}

	public class SciterValue
	{
		private SciterXValue.VALUE _data;
		private static SciterX.ISciterAPI _api = SciterX.API;

		public static readonly SciterValue Undefined;
		public static readonly SciterValue Null;

		static SciterValue()
		{
			Undefined = new SciterValue();
			Null = new SciterValue();
			Null._data.t = (uint) SciterXValue.VALUE_TYPE.T_NULL;
		}

		public static SciterXValue.VALUE[] ToVALUEArray(SciterValue[] values)
		{
			return values.Select(sv => sv._data).ToArray();
		}

		public SciterXValue.VALUE ToVALUE()
		{
			SciterXValue.VALUE vcopy;
			_api.ValueInit(out vcopy);
			_api.ValueCopy(out vcopy, ref _data);
			return vcopy;
		}

		public override string ToString()
		{
			string what = "";
			if(IsUndefined) what = "IsUndefined";
			if(IsBool) what = "IsBool";
			if(IsInt) what = "IsInt";
			if(IsFloat) what = "IsFloat";
			if(IsString) what = "IsString " + Get("");
			if(IsSymbol) what = "IsSymbol";
			if(IsErrorString) what = "IsErrorString";
			if(IsDate) what = "IsDate";
			if(IsCurrency) what = "IsCurrency";
			if(IsMap) what = "IsMap";
			if(IsArray) what = "IsArray";
			if(IsFunction) what = "IsFunction";
			if(IsBytes) what = "IsBytes";
			if(IsObject) what = "IsObject";
			if(IsDomElement) what = "IsDomElement";
			if(IsNativeFunction) what = "IsNativeFunction";
			if(IsColor) what = "IsColor";
			if(IsDuration) what = "IsDuration";
			if(IsAngle) what = "IsAngle";
			if(IsNull) what = "IsNull";
			if(IsObjectNative) what = "IsObjectNative";
			if(IsObjectArray) what = "IsObjectArray";
			if(IsObjectFunction) what = "IsObjectFunction";
			if(IsObjectObject) what = "IsObjectObject";
			if(IsObjectClass) what = "IsObjectClass";
			if(IsObjectError) what = "IsObjectError";
#if WINDOWS
			if(IsDate)
				what += " - " + GetDate().ToString();
#endif
			if(IsMap) what = "IsMap " + Regex.Replace(ToJSONString(), @"\t|\n|\r", "");

			return what;// + " - SciterValue JSON: " + Regex.Replace(ToJSONString(), @"\t|\n|\r", "");
		}

		public SciterValue() { _api.ValueInit(out _data); }
		~SciterValue() { _api.ValueClear(out _data); }
		
		public SciterValue(SciterValue vother)
		{
			_api.ValueInit(out _data);
			_api.ValueCopy(out _data, ref vother._data);
		}
		public SciterValue(SciterXValue.VALUE srcv)
		{
			_api.ValueInit(out _data);
			_api.ValueCopy(out _data, ref srcv);
		}

		public SciterValue(bool v)		{ _api.ValueInit(out _data); _api.ValueIntDataSet(ref _data, v ? 1 : 0, (uint) SciterXValue.VALUE_TYPE.T_BOOL, 0); }
		public SciterValue(int v)		{ _api.ValueInit(out _data); _api.ValueIntDataSet(ref _data, v, (uint) SciterXValue.VALUE_TYPE.T_INT, 0); }
		public SciterValue(uint v)		{ _api.ValueInit(out _data); _api.ValueIntDataSet(ref _data, (int) v, (uint) SciterXValue.VALUE_TYPE.T_INT, 0); }
		public SciterValue(double v)	{ _api.ValueInit(out _data); _api.ValueFloatDataSet(ref _data, v, (uint) SciterXValue.VALUE_TYPE.T_FLOAT, 0); }
		public SciterValue(string str)	{ _api.ValueInit(out _data); _api.ValueStringDataSet(ref _data, str, (uint) str.Length, (uint) SciterXValue.VALUE_UNIT_TYPE_STRING.UT_STRING_STRING); }
		public SciterValue(byte[] bs)	{ _api.ValueInit(out _data); _api.ValueBinaryDataSet(ref _data, bs, (uint) bs.Length, (uint) SciterXValue.VALUE_TYPE.T_BYTES, 0); }
		public SciterValue(DateTime dt) { _api.ValueInit(out _data); _api.ValueInt64DataSet(ref _data, dt.ToFileTime(), (uint)SciterXValue.VALUE_TYPE.T_DATE, 0); }
		public SciterValue(IEnumerable<SciterValue> col)
		{
			_api.ValueInit(out _data);
			int i = 0;
			foreach(var item in col)
				SetItem(i++, item);
		}
		private SciterValue(IConvertible ic)
		{
			_api.ValueInit(out _data);

			if(ic is bool)
				_api.ValueIntDataSet(ref _data, (bool) ic==true ? 1 : 0, (uint) SciterXValue.VALUE_TYPE.T_BOOL, 0);
			else if(ic is int)
				_api.ValueIntDataSet(ref _data, (int) ic, (uint) SciterXValue.VALUE_TYPE.T_INT, 0);
			else if(ic is uint)
				_api.ValueIntDataSet(ref _data, (int)(uint) ic, (uint)SciterXValue.VALUE_TYPE.T_INT, 0);
			else if(ic is double)
				_api.ValueFloatDataSet(ref _data, (double) ic, (uint) SciterXValue.VALUE_TYPE.T_FLOAT, 0);
			else if(ic is string)
				_api.ValueStringDataSet(ref _data, (string) ic, (uint) ((string) ic).Length, (uint) SciterXValue.VALUE_UNIT_TYPE_STRING.UT_STRING_STRING);
			else
				throw new Exception("Can not create a SciterValue from type '" + ic.GetType() + "'");
		}
		public SciterValue(Func<SciterValue[], SciterValue> func)
		{
			SciterXValue.FPTR_NATIVE_FUNCTOR_INVOKE fnfi;
			SciterXValue.FPTR_NATIVE_FUNCTOR_RELEASE fnfr;
			GCHandle fnfi_gch = new GCHandle();
			GCHandle fnfr_gch = new GCHandle();
			GCHandle func_gch = GCHandle.Alloc(func);

			fnfi = (IntPtr tag, uint argc, IntPtr argv, out SciterXValue.VALUE retval) =>
			{
				// Get the list of SciterXValue.VALUE from the ptr
				SciterValue[] args = new SciterValue[argc];
				for(int i = 0; i < argc; i++)
					args[i] = new SciterValue((SciterXValue.VALUE)Marshal.PtrToStructure(IntPtr.Add(argv, i * Marshal.SizeOf(typeof(SciterXValue.VALUE))), typeof(SciterXValue.VALUE)));

				retval = func(args).ToVALUE();
				return true;
			};

			fnfr = (IntPtr tag) =>
			{
				// seems to never be called -> Sciter engine bug
				fnfi_gch.Free();
				fnfr_gch.Free();
				func_gch.Free();
				return true;
			};

			fnfi_gch = GCHandle.Alloc(fnfi, GCHandleType.Normal);
			fnfr_gch = GCHandle.Alloc(fnfr, GCHandleType.Normal);
			func_gch = GCHandle.Alloc(func, GCHandleType.Normal);

			_api.ValueInit(out _data);
			_api.ValueNativeFunctorSet(ref _data, fnfi, fnfr, IntPtr.Zero);
		}
		public SciterValue(Action<SciterValue[]> func)
		{
			SciterXValue.FPTR_NATIVE_FUNCTOR_INVOKE fnfi;
			SciterXValue.FPTR_NATIVE_FUNCTOR_RELEASE fnfr;
			GCHandle fnfi_gch = new GCHandle();
			GCHandle fnfr_gch = new GCHandle();
			GCHandle func_gch = GCHandle.Alloc(func);

			fnfi = (IntPtr tag, uint argc, IntPtr argv, out SciterXValue.VALUE retval) =>
			{
				// Get the list of SciterXValue.VALUE from the ptr
				SciterValue[] args = new SciterValue[argc];
				for(int i = 0; i < argc; i++)
					args[i] = new SciterValue((SciterXValue.VALUE)Marshal.PtrToStructure(IntPtr.Add(argv, i * Marshal.SizeOf(typeof(SciterXValue.VALUE))), typeof(SciterXValue.VALUE)));

				func(args);
				retval = new SciterXValue.VALUE();
				return true;
			};

			fnfr = (IntPtr tag) =>
			{
				// seems to never be called -> Sciter engine bug
				fnfi_gch.Free();
				fnfr_gch.Free();
				func_gch.Free();
				return true;
			};

			fnfi_gch = GCHandle.Alloc(fnfi, GCHandleType.Normal);
			fnfr_gch = GCHandle.Alloc(fnfr, GCHandleType.Normal);
			func_gch = GCHandle.Alloc(func, GCHandleType.Normal);

			_api.ValueInit(out _data);
			_api.ValueNativeFunctorSet(ref _data, fnfi, fnfr, IntPtr.Zero);
		}

		public static SciterValue CreateFunctor(object f)
		{
			SciterValue sv = new SciterValue();
			List<MethodInfo> l = new List<MethodInfo>();
			foreach(var mi in f.GetType().GetMethods())
			{
				var mparams = mi.GetParameters();
				if(mi.Attributes.HasFlag(MethodAttributes.Public) && mi.ReturnType==typeof(SciterValue) && mparams.Length==1 && mparams[0].ParameterType==typeof(SciterValue[]))
				{
					sv[mi.Name] = new SciterValueFunctor();
				}
			}
			return sv;
		}


		/// <summary>
		/// Constructs a TIScript array T[] where T is a basic type like int or string
		/// </summary>
		public static SciterValue FromList<T>(IList<T> list) where T : /*struct,*/ IConvertible
		{
			Debug.Assert(list != null);

			SciterValue sv = new SciterValue();
			if(list.Count==0)
			{
				_api.ValueIntDataSet(ref sv._data, 0, (uint) SciterXValue.VALUE_TYPE.T_ARRAY, 0);
				return sv;
			}

			for(int i = 0; i < list.Count; i++)
				sv.SetItem(i, new SciterValue(list[i]));
			return sv;
		}

		/// <summary>
		/// Constructs a TIScript key-value object from a dictionary with string as keys and T as values, where T is a basic type like int or string
		/// </summary>
		public static SciterValue FromDictionary<T>(IDictionary<string, T> dic) where T : /*struct,*/ IConvertible
		{
			Debug.Assert(dic != null);

			SciterValue sv = new SciterValue();
			foreach(var item in dic)
				sv.SetItem(new SciterValue(item.Key), new SciterValue(item.Value));
			return sv;
		}

		/// <summary>
		/// Constructs a TIScript key-value object from a dictionary with string as keys and SciterValue as values
		/// </summary>
		public static SciterValue FromDictionary(IDictionary<string, SciterValue> dic)
		{
			Debug.Assert(dic != null);

			SciterValue sv = new SciterValue();
			foreach(var item in dic)
				sv.SetItem(new SciterValue(item.Key), new SciterValue(item.Value));
			return sv;
		}

		/// <summary>
		/// Returns SciterValue representing error.
		/// If such value is used as a return value from native function the script runtime will throw an error in script rather than returning that value.
		/// </summary>
		public static SciterValue MakeError(string msg)
		{
			if(msg==null)
				return null;

			SciterValue sv = new SciterValue();
			_api.ValueStringDataSet(ref sv._data, msg, (uint) msg.Length, (uint) SciterXValue.VALUE_UNIT_TYPE_STRING.UT_STRING_ERROR);
			return sv;
		}
		public static SciterValue MakeSymbol(string sym)
		{
			if(sym == null)
				return null;
			SciterValue sv = new SciterValue();
			_api.ValueStringDataSet(ref sv._data, sym, (uint)sym.Length, (uint)SciterXValue.VALUE_UNIT_TYPE_STRING.UT_STRING_SYMBOL);
			return sv;
		}
		public static SciterValue MakeColor(uint abgr)
		{
			SciterValue sv = new SciterValue();
			_api.ValueIntDataSet(ref sv._data, (int) abgr, (uint)SciterXValue.VALUE_TYPE.T_COLOR, 0);
			return sv;
		}
		public static SciterValue MakeDuration(double seconds)
		{
			SciterValue sv = new SciterValue();
			_api.ValueFloatDataSet(ref sv._data, seconds, (uint)SciterXValue.VALUE_TYPE.T_DURATION, 0);
			return sv;
		}
		public static SciterValue MakeAngle(double seconds)
		{
			SciterValue sv = new SciterValue();
			_api.ValueFloatDataSet(ref sv._data, seconds, (uint)SciterXValue.VALUE_TYPE.T_ANGLE, 0);
			return sv;
		}

		public bool IsUndefined			{ get { return _data.t == (uint)SciterXValue.VALUE_TYPE.T_UNDEFINED; } }
		public bool IsBool				{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_BOOL; } }
		public bool IsInt				{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_INT; } }
		public bool IsFloat				{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_FLOAT; } }
		public bool IsString			{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_STRING; } }
		public bool IsSymbol			{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_STRING && _data.u == (uint) SciterXValue.VALUE_UNIT_TYPE_STRING.UT_STRING_SYMBOL; } }
		public bool IsErrorString		{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_STRING && _data.u == (uint) SciterXValue.VALUE_UNIT_TYPE_STRING.UT_STRING_ERROR; } }
		public bool IsDate				{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_DATE; } }
		public bool IsCurrency			{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_CURRENCY; } }
		public bool IsMap				{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_MAP; } }
		public bool IsArray				{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_ARRAY; } }
		public bool IsFunction			{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_FUNCTION; } }
		public bool IsBytes				{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_BYTES; } }
		public bool IsObject			{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_OBJECT; } }
		public bool IsDomElement		{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_DOM_OBJECT; } }
		public bool IsNativeFunction	{ get { return _api.ValueIsNativeFunctor(ref _data) != 0; } }
		public bool IsColor				{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_COLOR; } }
		public bool IsDuration			{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_DURATION; } }
		public bool IsAngle				{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_ANGLE; } }
		public bool IsNull				{ get { return _data.t == (uint)SciterXValue.VALUE_TYPE.T_NULL; } }

		public bool IsObjectNative		{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_OBJECT && _data.u == (uint) SciterXValue.VALUE_UNIT_TYPE_OBJECT.UT_OBJECT_NATIVE; } }
		public bool IsObjectArray		{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_OBJECT && _data.u == (uint) SciterXValue.VALUE_UNIT_TYPE_OBJECT.UT_OBJECT_ARRAY; } }
		public bool IsObjectFunction	{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_OBJECT && _data.u == (uint) SciterXValue.VALUE_UNIT_TYPE_OBJECT.UT_OBJECT_FUNCTION; } }
		public bool IsObjectObject		{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_OBJECT && _data.u == (uint) SciterXValue.VALUE_UNIT_TYPE_OBJECT.UT_OBJECT_OBJECT; } }
		public bool IsObjectClass		{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_OBJECT && _data.u == (uint) SciterXValue.VALUE_UNIT_TYPE_OBJECT.UT_OBJECT_CLASS; } }
		public bool IsObjectError		{ get { return _data.t == (uint) SciterXValue.VALUE_TYPE.T_OBJECT && _data.u == (uint) SciterXValue.VALUE_UNIT_TYPE_OBJECT.UT_OBJECT_ERROR; } }

		public bool Get(bool defv)
		{
			int v;
			if(_api.ValueIntData(ref _data, out v) == (int) SciterXValue.VALUE_RESULT.HV_OK)
				return v!=0;
			return defv;
		}

		public int Get(int defv)
		{
			int v;
			if(_api.ValueIntData(ref _data, out v) == (int) SciterXValue.VALUE_RESULT.HV_OK)
				return v;
			return defv;
		}

		public double Get(double defv)
		{
			double v;
			if(_api.ValueFloatData(ref _data, out v) == (int) SciterXValue.VALUE_RESULT.HV_OK)
				return v;
			return defv;
		}

		public string Get(string defv)
		{
			IntPtr ret_ptr;
			uint ret_length;
			if(_api.ValueStringData(ref _data, out ret_ptr, out ret_length) == (int) SciterXValue.VALUE_RESULT.HV_OK)
				return Marshal.PtrToStringUni(ret_ptr, (int) ret_length);
			return defv;
		}

		public byte[] GetBytes()
		{
			IntPtr ret_ptr;
			uint ret_length;
			if(_api.ValueBinaryData(ref _data, out ret_ptr, out ret_length) == (int) SciterXValue.VALUE_RESULT.HV_OK)
			{
				byte[] ret = new byte[ret_length];
				Marshal.Copy(ret_ptr, ret, 0, (int) ret_length);
				return ret;
			}
			return null;
		}

		public RGBAColor GetColor()
		{
			Debug.Assert(IsColor);
			return new RGBAColor((uint) Get(0));
		}

		public double GetAngle()
		{
			Debug.Assert(IsAngle);
			return Get(0.0);
		}

		public double GetDuration()
		{
			Debug.Assert(IsDuration);
			return Get(0.0);
		}

#if WINDOWS || OSX
		public DateTime GetDate()
		{
			long v;
			_api.ValueInt64Data(ref _data, out v);
			return DateTime.FromFileTime(v);
		}
#endif
		
		public static SciterValue FromJSONString(string json, SciterXValue.VALUE_STRING_CVT_TYPE ct = SciterXValue.VALUE_STRING_CVT_TYPE.CVT_JSON_LITERAL)
		{
			SciterValue val = new SciterValue();
			_api.ValueFromString(ref val._data, json, (uint) json.Length, (uint) ct);
			return val;
		}

		public string ToJSONString(SciterXValue.VALUE_STRING_CVT_TYPE how = SciterXValue.VALUE_STRING_CVT_TYPE.CVT_JSON_LITERAL)
		{
			if(how==SciterXValue.VALUE_STRING_CVT_TYPE.CVT_SIMPLE && IsString)
				return Get("");

			SciterValue outdata = new SciterValue(this);
			_api.ValueToString(ref outdata._data, how);
			return outdata.Get("");
		}

		public void Clear()
		{
			_api.ValueClear(out _data);
		}

		public int Length
		{
			get
			{
				int count;
				_api.ValueElementsCount(ref _data, out count);
				return count;
			}
		}


		public SciterValue this[int key]
		{
			get
			{
				return GetItem(key);
			}
			set
			{
				SetItem(key, value);
			}
		}

		public SciterValue this[string key]
		{
			get
			{
				return GetItem(key);
			}
			set
			{
				SetItem(key, value);
			}
		}


		public void SetItem(int i, SciterValue val)
		{
			var vr = _api.ValueNthElementValueSet(ref _data, i, ref val._data);
			Debug.Assert(vr == SciterXValue.VALUE_RESULT.HV_OK);
		}

		public void SetItem(SciterValue key, SciterValue val)
		{
			var vr = _api.ValueSetValueToKey(ref _data, ref key._data, ref val._data);
			Debug.Assert(vr == SciterXValue.VALUE_RESULT.HV_OK);
		}

		public void SetItem(string key, SciterValue val)
		{
			SciterValue svkey = SciterValue.MakeSymbol(key);
			var vr = _api.ValueSetValueToKey(ref _data, ref svkey._data, ref val._data);
			Debug.Assert(vr == SciterXValue.VALUE_RESULT.HV_OK);
		}

		public void Append(SciterValue val)
		{
			_api.ValueNthElementValueSet(ref _data, Length, ref val._data);
		}

		public SciterValue GetItem(int n)
		{
			SciterValue val = new SciterValue();
			_api.ValueNthElementValue(ref _data, n, out val._data);
			return val;
		}

		public SciterValue GetItem(SciterValue vkey)
		{
			SciterValue vret = new SciterValue();
			_api.ValueGetValueOfKey(ref _data, ref vkey._data, out vret._data);
			return vret;
		}

		public SciterValue GetItem(string strkey)
		{
			SciterValue vret = new SciterValue();
			SciterValue vkey = SciterValue.MakeSymbol(strkey);
			_api.ValueGetValueOfKey(ref _data, ref vkey._data, out vret._data);
			return vret;
		}

		public SciterValue GetKey(int n)
		{
			SciterValue vret = new SciterValue();
			_api.ValueNthElementKey(ref _data, n, out vret._data);
			return vret;
		}

		public List<SciterValue> Keys
		{
			get
			{
				if(!IsObject && !IsMap)
					throw new ArgumentException("This SciterValue is not an object");
				if(IsObject)
					throw new ArgumentException("Plz, call Isolate() for this SciterValue");
				List<SciterValue> keys = new List<SciterValue>();
				for(int i = 0; i < Length; i++)
					keys.Add(GetKey(i));
				return keys;
			}
		}
		/*public void SetObjectData(IntPtr p)
		{
			Debug.Assert(data.u == (uint) SciterXValue.VALUE_UNIT_TYPE_OBJECT.UT_OBJECT_NATIVE);
			_api.ValueBinaryDataSet(ref data, );
		}*/
		public IntPtr GetObjectData()
		{
			IntPtr p;
			uint dummy;
			_api.ValueBinaryData(ref _data, out p, out dummy);
			return p;
		}


		public SciterValue Call(IList<SciterValue> args, SciterValue self = null, string url_or_script_name = null)
		{
			if(!IsFunction && !IsObjectFunction)
				throw new Exception("Can't Call() this SciterValue because it is not a function");

			SciterValue rv = new SciterValue();
			SciterXValue.VALUE[] arr_VALUE = args.Select(sv => sv._data).ToArray();
			if(self == null)
				self = SciterValue.Undefined;

			_api.ValueInvoke(ref _data, ref self._data, (uint) args.Count, args.Count==0 ? null : arr_VALUE, out rv._data, null);
			return rv;
		}

		public SciterValue Call(params SciterValue[] args)
		{
			return Call((IList<SciterValue>) args);
		}

		public void Isolate()
		{
			_api.ValueIsolate(ref _data);
		}


		public IEnumerable<SciterValue> AsEnumerable()
		{
			if(!IsArray && !IsObject && !IsMap)
				throw new ArgumentException("This SciterValue is not an array or object");
			for(int i = 0; i < Length; i++)
			{
				yield return this[i];
			}
		}

		public IDictionary<SciterValue, SciterValue> AsDictionary()
		{
			if(!IsObject && !IsMap)
				throw new ArgumentException("This SciterValue is not an object");

			Dictionary<SciterValue, SciterValue> dic = new Dictionary<SciterValue, SciterValue>();
			for(int i = 0; i < Length; i++)
			{
				dic[GetKey(i)] = GetItem(i);
			}

			return dic;
		}
	}
}