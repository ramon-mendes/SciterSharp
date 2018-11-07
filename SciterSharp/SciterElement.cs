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
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SciterSharp.Interop;
using System.Collections;

namespace SciterSharp
{
	public class SciterElement
	{
		private static SciterX.ISciterAPI _api = SciterX.API;
		public IntPtr _he { get; private set; }

		public SciterElement(IntPtr he)
		{
			Debug.Assert(he != IntPtr.Zero);
			if(he == IntPtr.Zero)
				throw new ArgumentException("IntPtr.Zero received at SciterElement constructor");

			_he = he;
		}

		public SciterElement(SciterValue sv)
		{
			if(!sv.IsObject)
				throw new ArgumentException("The given SciterValue is not a TIScript Element reference");

			IntPtr he = sv.GetObjectData();
			if(he == IntPtr.Zero)
				throw new ArgumentException("IntPtr.Zero received at SciterElement constructor");

			_he = he;
		}

		public static SciterElement Create(string tagname, string text = null)
		{
			IntPtr he;
			_api.SciterCreateElement(tagname, text, out he);
			if(he != IntPtr.Zero)
				return new SciterElement(he);
			return null;
		}

		#region Query HTML
		public string Tag
		{
			get
			{
				IntPtr ptrtag;
				var r = _api.SciterGetElementType(_he, out ptrtag);
				Debug.Assert(r == SciterXDom.SCDOM_RESULT.SCDOM_OK);
				return Marshal.PtrToStringAnsi(ptrtag);
			}
		}

		public string HTML
		{
			get
			{
				string strval = null;
				SciterXDom.FPTR_LPCBYTE_RECEIVER frcv = (IntPtr bytes, uint num_bytes, IntPtr param) =>
				{
					strval = Marshal.PtrToStringAnsi(bytes, (int)num_bytes);
				};

				var r = _api.SciterGetElementHtmlCB(_he, true, frcv, IntPtr.Zero);
				if(r == SciterXDom.SCDOM_RESULT.SCDOM_OK_NOT_HANDLED)
					Debug.Assert(strval == null);
				return strval;
			}
		}

		public string InnerHTML
		{
			get
			{
				string strval = null;
				SciterXDom.FPTR_LPCBYTE_RECEIVER frcv = (IntPtr bytes, uint num_bytes, IntPtr param) =>
				{
					strval = Marshal.PtrToStringAnsi(bytes, (int)num_bytes);
				};

				var r = _api.SciterGetElementHtmlCB(_he, false, frcv, IntPtr.Zero);
				if(r == SciterXDom.SCDOM_RESULT.SCDOM_OK_NOT_HANDLED)
					Debug.Assert(strval == null);
				return strval;
			}
		}

		public string Text
		{
			get
			{
				string strval = null;
				SciterXDom.FPTR_LPCWSTR_RECEIVER frcv = (IntPtr str, uint str_length, IntPtr param) =>
				{
					strval = Marshal.PtrToStringUni(str, (int)str_length);
				};

				var r = _api.SciterGetElementTextCB(_he, frcv, IntPtr.Zero);
				if(r == SciterXDom.SCDOM_RESULT.SCDOM_OK_NOT_HANDLED)
					Debug.Assert(strval == null);
				return strval;
			}

			set
			{
				_api.SciterSetElementText(_he, value, (uint) value.Length);
			}
		}

		public void SetHTML(string html, SciterXDom.SET_ELEMENT_HTML where = SciterXDom.SET_ELEMENT_HTML.SIH_REPLACE_CONTENT)
		{
			if(html==null)
				Clear();
			else
			{
				var data = Encoding.UTF8.GetBytes(html);
				_api.SciterSetElementHtml(_he, data, (uint) data.Length, where);
			}
		}
		#endregion

		#region Attributes and Styles
		public Dictionary<string, string> Attributes
		{
			get
			{
				Dictionary<string, string> attrs = new Dictionary<string, string>();
				for(uint n = 0; n < AttributeCount; n++)
				{
					attrs[ GetAttributeName(n) ] = GetAttribute(n);
				}
				return attrs;
			}
		}

		public uint AttributeCount
		{
			get
			{
				uint count;
				_api.SciterGetAttributeCount(_he, out count);
				return count;
			}
		}

		public string GetAttribute(uint n)
		{
			string strval = null;
			SciterXDom.FPTR_LPCWSTR_RECEIVER frcv = (IntPtr str, uint str_length, IntPtr param) =>
			{
				strval = Marshal.PtrToStringUni(str, (int)str_length);
			};

			var r = _api.SciterGetNthAttributeValueCB(_he, n, frcv, IntPtr.Zero);
			if(r == SciterXDom.SCDOM_RESULT.SCDOM_OK_NOT_HANDLED)
				Debug.Assert(strval == null);
			return strval;
		}

		public string GetAttribute(string name)
		{
			string strval = null;
			SciterXDom.FPTR_LPCWSTR_RECEIVER frcv = (IntPtr str, uint str_length, IntPtr param) =>
			{
				strval = Marshal.PtrToStringUni(str, (int) str_length);
			};

			var r = _api.SciterGetAttributeByNameCB(_he, name, frcv, IntPtr.Zero);
			if(r == SciterXDom.SCDOM_RESULT.SCDOM_OK_NOT_HANDLED)
				Debug.Assert(strval == null);
			return strval;
		}

		public string GetAttributeName(uint n)
		{
			string strval = null;
			SciterXDom.FPTR_LPCSTR_RECEIVER frcv = (IntPtr str, uint str_length, IntPtr param) =>
			{
				strval = Marshal.PtrToStringAnsi(str, (int)str_length);
			};

			var r = _api.SciterGetNthAttributeNameCB(_he, n, frcv, IntPtr.Zero);
			if(r == SciterXDom.SCDOM_RESULT.SCDOM_OK_NOT_HANDLED)
				Debug.Assert(strval == null);
			return strval;
		}

		public void SetAttribute(string name, string value)
		{
			var r = _api.SciterSetAttributeByName(_he, name, value);
			Debug.Assert(r == SciterXDom.SCDOM_RESULT.SCDOM_OK);
		}

		public void RemoveAttribute(string name)
		{
			_api.SciterSetAttributeByName(_he, name, null);
		}


		public string GetStyle(string name)
		{
			string strval = null;
			SciterXDom.FPTR_LPCWSTR_RECEIVER frcv = (IntPtr str, uint str_length, IntPtr param) =>
			{
				strval = Marshal.PtrToStringUni(str, (int)str_length);
			};

			var r = _api.SciterGetStyleAttributeCB(_he, name, frcv, IntPtr.Zero);
			if(r == SciterXDom.SCDOM_RESULT.SCDOM_OK_NOT_HANDLED)
				Debug.Assert(strval == null);
			return strval;
		}
		public void SetStyle(string name, string value)
		{
			_api.SciterSetStyleAttribute(_he, name, value);
		}
		#endregion

		#region State
		public SciterXDom.ELEMENT_STATE_BITS State
		{
			get
			{
				return GetState();
			}
		}

		public SciterXDom.ELEMENT_STATE_BITS GetState()
		{
			uint bits;
			var r = _api.SciterGetElementState(_he, out bits);
			Debug.Assert(r == SciterXDom.SCDOM_RESULT.SCDOM_OK);
			return (SciterXDom.ELEMENT_STATE_BITS) bits;
		}

		public void SetState(SciterXDom.ELEMENT_STATE_BITS bitsToSet, SciterXDom.ELEMENT_STATE_BITS bitsToClear = 0, bool update = true)
		{
			var r = _api.SciterSetElementState(_he, (uint) bitsToSet, (uint) bitsToClear, update);
			Debug.Assert(r == SciterXDom.SCDOM_RESULT.SCDOM_OK);
		}
		#endregion

		public string CombineURL(string url = "")
		{
			IntPtr buffer = PInvokeUtils.NativeUtf16FromString(url, 2048);
			var res = _api.SciterCombineURL(_he, buffer, 2048);
			string s = PInvokeUtils.StringFromNativeUtf16(buffer);
			PInvokeUtils.NativeUtf16FromString_FreeBuffer(buffer);
			return s;
		}

		#region Integers
		public IntPtr GetNativeHwnd(bool rootWindow = true)
		{
			IntPtr hwnd;
			_api.SciterGetElementHwnd(_he, out hwnd, rootWindow);
			return hwnd;
		}

		public SciterWindow Window
		{
			get
			{
				IntPtr hwnd = GetNativeHwnd();
				if(hwnd != IntPtr.Zero)
					return new SciterWindow(hwnd);
				return null;
			}
		}

		public uint UID
		{
			get
			{
				uint uid;
				_api.SciterGetElementUID(_he, out uid);
				return uid;
			}
		}

		public uint Index
		{
			get
			{
				uint idx;
				_api.SciterGetElementIndex(_he, out idx);
				return idx;
			}
		}

		public uint ChildrenCount
		{
			get
			{
				uint n;
				_api.SciterGetChildrenCount(_he, out n);
				return n;
			}
		}
		#endregion

		public void Delete()
		{
			_api.SciterDeleteElement(_he);
		}
		public void Dettach()
		{
			_api.SciterDetachElement(_he);
		}

		public SciterElement Clone()
		{
			IntPtr clone_he;
			_api.SciterCloneElement(_he, out clone_he);
			return new SciterElement(clone_he);
		}

		public SciterNode ToNode()
		{
			IntPtr hn;
			_api.SciterNodeCastFromElement(_he, out hn);
			return new SciterNode(hn);
		}

		public bool Enabled // deeply enabled
		{
			get
			{
				bool b;
				_api.SciterIsElementEnabled(_he, out b);
				return b;
			}
		}

		public bool Visible // deeply visible
		{
			get
			{
				bool b;
				_api.SciterIsElementVisible(_he, out b);
				return b;
			}
		}

		#region Operators and overrides
		public static bool operator ==(SciterElement a, SciterElement b)
		{
			if((object)a == null || (object)b == null)
				return Object.Equals(a, b);
			return a._he == b._he;
		}
		public static bool operator !=(SciterElement a, SciterElement b)
		{
			return !(a == b);
		}

		public SciterElement this[uint idx]
		{
			get
			{
				return GetChild(idx);
			}
		}

		public string this[string name]
		{
			get
			{
				return GetAttribute(name);
			}
			set
			{
				SetAttribute(name, value);
			}
		}

		public override bool Equals(object o)
		{
			return Object.ReferenceEquals(this, o);
		}

		public override int GetHashCode()
		{
			return _he.ToInt32();
		}

		public override string ToString()
		{
			string tag = Tag;
			string id = GetAttribute("id");
			string classes = GetAttribute("class");
			uint childcount = this.ChildrenCount;

			StringBuilder str = new StringBuilder();
			str.Append("<" + tag);
			if(id != null)
				str.Append(" #" + id);
			if(classes != null)
				str.Append(" ." + String.Join(".", classes.Split(' ')));
			if(childcount == 0)
				str.Append(" />");
			else
				str.Append(">...</" + tag + ">");

			return str.ToString();
		}
		#endregion

		#region DOM navigation
		public SciterElement GetChild(uint idx)
		{
			IntPtr child_he;
			_api.SciterGetNthChild(_he, idx, out child_he);
			if(child_he == IntPtr.Zero)
				return null;
			return new SciterElement(child_he);
		}

		public IEnumerable<SciterElement> Children
		{
			get
			{
				var list = new List<SciterElement>();
				for(uint i = 0; i < ChildrenCount; i++)
					list.Add(this[i]);
				return list;
			}
		}

		public SciterElement Parent
		{
			get
			{
				IntPtr out_he;
				_api.SciterGetParentElement(_he, out out_he);
				if(out_he == IntPtr.Zero)
					return null;
				return new SciterElement(out_he);
			}
		}

		public SciterElement NextSibling
		{
			get
			{
				SciterElement parent = this.Parent;
				if(parent == null)
					return null;
				return parent.GetChild(this.Index + 1);
			}
		}

		public SciterElement PrevSibling
		{
			get
			{
				SciterElement parent = this.Parent;
				if(parent == null)
					return null;
				return parent.GetChild(this.Index - 1);
			}
		}

		public SciterElement FirstSibling
		{
			get
			{
				SciterElement parent = this.Parent;
				if(parent == null)
					return null;
				return parent.GetChild(0);
			}
		}

		public SciterElement LastSibling
		{
			get
			{
				SciterElement parent = this.Parent;
				if(parent == null)
					return null;
				return parent.GetChild(parent.ChildrenCount - 1);
			}
		}
		#endregion

		#region DOM query/select
		public SciterElement SelectFirstById(string id)
		{
			return SelectFirst("[id='" + id + "']");
		}

		public SciterElement SelectFirst(string selector)
		{
			SciterElement se = null;
			SciterXDom.FPTR_SciterElementCallback cbk = (IntPtr he, IntPtr param) =>
			{
				se = new SciterElement(he);
				return true;// true stops enumeration
			};
			_api.SciterSelectElementsW(_he, selector, cbk, IntPtr.Zero);
			return se;
		}

		public List<SciterElement> SelectAll(string selector)
		{
			List<SciterElement> list = new List<SciterElement>();
			SciterXDom.FPTR_SciterElementCallback cbk = (IntPtr he, IntPtr param) =>
			{
				list.Add(new SciterElement(he));
				return false;// false continue enumeration
			};
			_api.SciterSelectElementsW(_he, selector, cbk, IntPtr.Zero);
			return list;
		}

		public SciterElement SelectNearestParent(string selector)
		{
			IntPtr heFound;
			_api.SciterSelectParentW(_he, selector, 0, out heFound);
			if(heFound.ToInt32() == 0)
				return null;
			return new SciterElement(heFound);
		}
		#endregion

		#region DOM sub-tree manipulation
		public void Insert(SciterElement se, uint index = 0)
		{
			_api.SciterInsertElement(se._he, _he, index);
		}

		public void Append(SciterElement se)
		{
			_api.SciterInsertElement(se._he, _he, int.MaxValue);
		}

		public void Swap(SciterElement sewith)
		{
			_api.SciterSwapElements(_he, sewith._he);
		}

		public void Clear()
		{
			_api.SciterSetElementText(_he, null, 0);
		}

		public void TransformHTML(string html, SciterXDom.SET_ELEMENT_HTML how = SciterXDom.SET_ELEMENT_HTML.SIH_REPLACE_CONTENT)
		{
			var bytes = Encoding.UTF8.GetBytes(html);
			_api.SciterSetElementHtml(_he, bytes, (uint) bytes.Length, how);
		}
		#endregion

		#region Events
		public void AttachEvh(SciterEventHandler evh)
		{
			Debug.Assert(evh != null);
			var r = _api.SciterAttachEventHandler(_he, evh._proc, IntPtr.Zero);
			Debug.Assert(r == SciterXDom.SCDOM_RESULT.SCDOM_OK);
		}
		public void DetachEvh(SciterEventHandler evh)
		{
			Debug.Assert(evh != null);
			var r = _api.SciterDetachEventHandler(_he, evh._proc, IntPtr.Zero);
			Debug.Assert(r == SciterXDom.SCDOM_RESULT.SCDOM_OK);
		}

		public bool SendEvent(uint event_code, uint reason = 0, SciterElement heSource = null)
		{
			bool handled;
			_api.SciterSendEvent(_he, event_code, heSource == null ? IntPtr.Zero : heSource._he, new IntPtr(reason), out handled);
			return handled;
		}

		public void PostEvent(uint event_code, uint reason = 0, SciterElement heSource = null)
		{
			_api.SciterPostEvent(_he, event_code, heSource == null ? IntPtr.Zero : heSource._he, new IntPtr(reason));
		}

		public bool FireEvent(SciterXBehaviors.BEHAVIOR_EVENT_PARAMS evt, bool post = true)
		{
			bool handled;
			_api.SciterFireEvent(ref evt, post, out handled);
			return handled;
		}
		#endregion

		#region Location and Size
		public PInvokeUtils.RECT GetLocation(SciterXDom.ELEMENT_AREAS area = SciterXDom.ELEMENT_AREAS.ROOT_RELATIVE | SciterXDom.ELEMENT_AREAS.CONTENT_BOX)
		{
			PInvokeUtils.RECT rc;
			_api.SciterGetElementLocation(_he, out rc, area);
			return rc;
		}

		public PInvokeUtils.SIZE SizePadding
		{
			get
			{
				PInvokeUtils.RECT rc;
				_api.SciterGetElementLocation(_he, out rc, SciterXDom.ELEMENT_AREAS.ROOT_RELATIVE | SciterXDom.ELEMENT_AREAS.PADDING_BOX);
				return new PInvokeUtils.SIZE() { cx = rc.Width, cy = rc.Height };
			}
		}
		#endregion

		/// <summary>
		/// Test this element against CSS selector(s)
		/// </summary>
		public bool Test(string selector)
		{
			IntPtr heFound;
			_api.SciterSelectParent(_he, selector, 1, out heFound);
			return heFound != IntPtr.Zero;
		}

		public void Update(bool andForceRender = false)
		{
			_api.SciterUpdateElement(_he, andForceRender);
		}

		public void Refresh(PInvokeUtils.RECT rc)
		{
			_api.SciterRefreshElementArea(_he, rc);
		}
		public void Refresh()
		{
			_api.SciterRefreshElementArea(_he, GetLocation(SciterXDom.ELEMENT_AREAS.SELF_RELATIVE | SciterXDom.ELEMENT_AREAS.CONTENT_BOX));
		}

		#region Scripting
		public SciterValue Value
		{
			get
			{
				SciterXValue.VALUE val;
				_api.SciterGetValue(_he, out val);
				return new SciterValue(val);
			}

			set
			{
				var val = value.ToVALUE();
				_api.SciterSetValue(_he, ref val);
			}
		}

		public SciterValue ExpandoValue
		{
			get
			{
				SciterXValue.VALUE val;
				_api.SciterGetExpando(_he, out val, true);
				return new SciterValue(val);
			}
		}


		// call scripting method attached to the element (directly or through of scripting behavior)  
		// Example, script:
		//   var elem = ...
		//   elem.foo = function() {...}
		// Native code: 
		//   SciterElement elem = ...
		//   elem.CallMethod("foo");
		public SciterValue CallMethod(string name, params SciterValue[] args)
		{
			Debug.Assert(name != null);

			SciterXValue.VALUE vret;
			_api.SciterCallScriptingMethod(_he, name, SciterValue.ToVALUEArray(args), (uint) args.Length, out vret);
			return new SciterValue(vret);
		}

		// call scripting function defined on global level   
		// Example, script:
		//   function foo() {...}
		// Native code: 
		//   dom::element root = ... get root element of main document or some frame inside it
		//   root.call_function("foo"); // call the function
		public SciterValue CallFunction(string name, params SciterValue[] args)
		{
			Debug.Assert(name != null);

			SciterXValue.VALUE vret;
			_api.SciterCallScriptingFunction(_he, name, SciterValue.ToVALUEArray(args), (uint) args.Length, out vret);
			return new SciterValue(vret);
		}

		public SciterValue Eval(string script)
		{
			SciterXValue.VALUE rv;
			_api.SciterEvalElementScript(_he, script, (uint) script.Length, out rv);
			return new SciterValue(rv);
		}
		#endregion

		#region Highlight set/get
		public bool Highlight
		{
			set
			{
				if(value)
					_api.SciterSetHighlightedElement(GetNativeHwnd(), _he);
				else
					_api.SciterSetHighlightedElement(GetNativeHwnd(), IntPtr.Zero);
			}
		}
		#endregion

		#region Helpers
		public bool IsChildOf(SciterElement parent_test)
		{
			SciterElement el_it = this;
			while(true)
			{
				if(el_it._he == parent_test._he)
					return true;

				el_it = el_it.Parent;
				if(el_it == null)
					break;
			}
			return false;
		}
		#endregion
	}

	public class SciterNode
	{
		private static SciterX.ISciterAPI _api = SciterX.API;
		public IntPtr _hn { get; private set; }

		public SciterNode(IntPtr hn)
		{
			Debug.Assert(hn != IntPtr.Zero);
			if(hn == IntPtr.Zero)
				throw new ArgumentException("IntPtr.Zero received at SciterNode constructor");

			_hn = hn;
		}

		public static SciterNode MakeTextNode(string text)
		{
			IntPtr hn;
			_api.SciterCreateTextNode(text, (uint)text.Length, out hn);
			if(hn != IntPtr.Zero)
				return new SciterNode(hn);
			return null;
		}

		public static SciterNode MakeCommentNode(string text)
		{
			IntPtr hn;
			_api.SciterCreateCommentNode(text, (uint)text.Length, out hn);
			if(hn != IntPtr.Zero)
				return new SciterNode(hn);
			return null;
		}

		public uint ChildrenCount
		{
			get
			{
				uint n;
				_api.SciterNodeChildrenCount(_hn, out n);
				return n;
			}
		}

		public SciterElement ToElement()
		{
			IntPtr he;
			var r = _api.SciterNodeCastToElement(_hn, out he);
			Debug.Assert(r == SciterXDom.SCDOM_RESULT.SCDOM_OK);
			return new SciterElement(he);
		}

		public bool IsText
		{
			get
			{
				SciterXDom.NODE_TYPE nodeType;
				var r = _api.SciterNodeType(_hn, out nodeType);
				Debug.Assert(r == SciterXDom.SCDOM_RESULT.SCDOM_OK);
				return nodeType == SciterXDom.NODE_TYPE.NT_TEXT;
			}
		}
		public bool IsComment
		{
			get
			{
				SciterXDom.NODE_TYPE nodeType;
				var r = _api.SciterNodeType(_hn, out nodeType);
				Debug.Assert(r == SciterXDom.SCDOM_RESULT.SCDOM_OK);
				return nodeType == SciterXDom.NODE_TYPE.NT_COMMENT;
			}
		}
		public bool IsElement
		{
			get
			{
				SciterXDom.NODE_TYPE nodeType;
				var r = _api.SciterNodeType(_hn, out nodeType);
				Debug.Assert(r == SciterXDom.SCDOM_RESULT.SCDOM_OK);
				return nodeType == SciterXDom.NODE_TYPE.NT_ELEMENT;
			}
		}

		public SciterNode this[uint idx]
		{
			get
			{
				return GetChild(idx);
			}
		}

		public string Text
		{
			get
			{
				string strval = null;
				SciterXDom.FPTR_LPCWSTR_RECEIVER frcv = (IntPtr str, uint str_length, IntPtr param) =>
				{
					if(str != IntPtr.Zero)
						strval = Marshal.PtrToStringUni(str, (int)str_length);
				};

				var r = _api.SciterNodeGetText(_hn, frcv, IntPtr.Zero);
				if(r == SciterXDom.SCDOM_RESULT.SCDOM_OK_NOT_HANDLED)
					Debug.Assert(strval == null);
				return strval;
			}
		}

		#region DOM navigation
		public SciterNode GetChild(uint idx)
		{
			IntPtr child_hn;
			_api.SciterNodeNthChild(_hn, idx, out child_hn);
			if(child_hn == IntPtr.Zero)
				return null;
			return new SciterNode(child_hn);
		}
		#endregion
	}
}