// Copyright 2025 Ramon F. Mendes
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
	public abstract class SciterEventHandler
	{
#if DEBUG
		private volatile bool _is_attached = false;
		~SciterEventHandler() { Debug.Assert(!_attached_handlers.Contains(this)); Debug.Assert(_is_attached == false); }
#endif

		private static List<SciterEventHandler> _attached_handlers = new List<SciterEventHandler>();// we keep a copy of all attached instances to guard from GC removal

		public SciterEventHandler() { _proc = EventProc; Name = this.GetType().FullName; }
		public SciterEventHandler(string name) { Name = name; }
		public string Name { get; set; }
		public readonly SciterXBehaviors.FPTR_ElementEventProc _proc;// keep a copy of the delegate so it survives GC


		// Overridables
		protected virtual bool Subscription(SciterElement se, out SciterXBehaviors.EVENT_GROUPS event_groups)
		{
			event_groups = SciterXBehaviors.EVENT_GROUPS.HANDLE_ALL;
			return true;
		}

		protected virtual void Attached(SciterElement se) { }
		protected virtual void Detached(SciterElement se) { }

		protected virtual bool OnMouse(SciterElement se, SciterXBehaviors.MOUSE_PARAMS prms) { return false; }
		protected virtual bool OnKey(SciterElement se, SciterXBehaviors.KEY_PARAMS prms) { return false; }
		protected virtual bool OnFocus(SciterElement se, SciterXBehaviors.FOCUS_PARAMS prms) { return false; }

		protected virtual bool OnTimer(SciterElement se) { return false; }
		protected virtual bool OnTimer(SciterElement se, IntPtr extTimerId) { return false; }
		protected virtual bool OnSize(SciterElement se) { return false; }
		protected virtual bool OnStyleChange(SciterElement se, uint change_kind) { return false; }

		protected virtual bool OnDraw(SciterElement se, SciterXBehaviors.DRAW_PARAMS prms) { return false; }

		protected virtual bool OnMethodCall(SciterElement se, SciterXBehaviors.BEHAVIOR_METHOD_IDENTIFIERS methodID) { return false; }
		protected virtual bool OnScriptCall(SciterElement se, string name, SciterValue[] args, out SciterValue result)
		{
			result = null;

			var method = GetType().GetMethod(name);
			if(method != null)
			{
				// This base class tries to handle it by searching for a method with the same 'name'
				var mparams = method.GetParameters();

				// match signature:
				// 'void MethodName()' or 'SciterValue MethodName()'
				{
					if(mparams.Length == 0 && 
						(method.ReturnType == typeof(void) || method.ReturnType == typeof(SciterValue)))
					{
						var ret = method.Invoke(this, null);
						if(method.ReturnType == typeof(SciterValue))
							result = (SciterValue)ret;
						return true;
					}
				}

				// match signature:
				// 'void MethodName(SciterValue[] args)' or 'SciterValue MethodName(SciterValue[] args)'
				{
					if(mparams.Length==1 && mparams[0].ParameterType.Name == "SciterValue[]" &&
						(method.ReturnType == typeof(void) || method.ReturnType == typeof(SciterValue)))
					{
						object[] call_parameters = new object[] { args };
						var ret = method.Invoke(this, call_parameters);
						if(method.ReturnType == typeof(SciterValue))
							result = (SciterValue)ret;
						return true;
					}
				}

				// match signature:
				// bool MethodName(SciterElement el, SciterValue[] args, out SciterValue result)
				{
					if(method.ReturnType == typeof(bool) && mparams.Length == 3
						&& mparams[0].ParameterType.Name == "SciterElement"
						&& mparams[1].ParameterType.Name == "SciterValue[]"
						&& mparams[2].ParameterType.Name == "SciterValue&")
					{
						object[] call_parameters = new object[] { se, args, null };
						bool res = (bool)method.Invoke(this, call_parameters);
						Debug.Assert(call_parameters[2] == null || call_parameters[2].GetType().IsAssignableFrom(typeof(SciterValue)));
						result = call_parameters[2] as SciterValue;
						return res;
					}
				}
			}

			// not handled
			return false;
		}

		protected virtual bool OnEvent(SciterElement elSource, SciterElement elTarget, SciterXBehaviors.BEHAVIOR_EVENTS type, IntPtr reason, SciterValue data) { return false; }

		protected virtual bool OnDataArrived(SciterElement se, SciterXBehaviors.DATA_ARRIVED_PARAMS prms) { return false; }

		protected virtual bool OnScroll(SciterElement se, SciterXBehaviors.SCROLL_PARAMS prms) { return false; }
		protected virtual bool OnGesture(SciterElement se, SciterXBehaviors.GESTURE_PARAMS prms) { return false; }
		protected virtual bool OnExchange(SciterElement se, SciterXBehaviors.EXCHANGE_PARAMS prms) { return false; }
		protected virtual bool OnAttributeChange(SciterElement se, SciterXBehaviors.ATTRIBUTE_CHANGE_PARAMS prms) { return false; }

		protected virtual bool OnSOM(SciterElement se, SciterXBehaviors.SOM_PARAMS prms) { return false; }

		private bool EventProc(IntPtr tag, IntPtr he, uint evtg, IntPtr prms)
		{
			SciterElement se = he != IntPtr.Zero ? new SciterElement(he) : null;

			switch((SciterXBehaviors.EVENT_GROUPS)evtg)
			{
				case SciterXBehaviors.EVENT_GROUPS.SUBSCRIPTIONS_REQUEST:
					SciterXBehaviors.EVENT_GROUPS groups;
					var ret = Subscription(se, out groups);

					Marshal.WriteInt32(prms, (int)groups);
					return ret;

				case SciterXBehaviors.EVENT_GROUPS.HANDLE_INITIALIZATION:
					{
						var p = Marshal.PtrToStructure<SciterXBehaviors.INITIALIZATION_PARAMS>(prms);
						if(p.cmd == SciterXBehaviors.INITIALIZATION_EVENTS.BEHAVIOR_DETACH)
						{
#if DEBUG
							Debug.Assert(_is_attached == true);
							_is_attached = false;
#endif
							_attached_handlers.Remove(this);
							Detached(se);
						}
						else if(p.cmd == SciterXBehaviors.INITIALIZATION_EVENTS.BEHAVIOR_ATTACH)
						{
#if DEBUG
							Debug.Assert(_is_attached == false);
							_is_attached = true;
#endif
							_attached_handlers.Add(this);
							Attached(se);
						}
						return true;
					}

				case SciterXBehaviors.EVENT_GROUPS.HANDLE_SOM:
					return OnSOM(se, Marshal.PtrToStructure<SciterXBehaviors.SOM_PARAMS>(prms));

				case SciterXBehaviors.EVENT_GROUPS.HANDLE_MOUSE:
					return OnMouse(se, Marshal.PtrToStructure<SciterXBehaviors.MOUSE_PARAMS>(prms));

				case SciterXBehaviors.EVENT_GROUPS.HANDLE_KEY:
					return OnKey(se, Marshal.PtrToStructure<SciterXBehaviors.KEY_PARAMS>(prms));

				case SciterXBehaviors.EVENT_GROUPS.HANDLE_FOCUS:
					return OnFocus(se, Marshal.PtrToStructure<SciterXBehaviors.FOCUS_PARAMS>(prms));

				case SciterXBehaviors.EVENT_GROUPS.HANDLE_DRAW:
					return OnDraw(se, Marshal.PtrToStructure<SciterXBehaviors.DRAW_PARAMS>(prms));

				case SciterXBehaviors.EVENT_GROUPS.HANDLE_TIMER:
					{
						var p = Marshal.PtrToStructure<SciterXBehaviors.TIMER_PARAMS>(prms);
						return p.timerId != IntPtr.Zero ? OnTimer(se, p.timerId) : OnTimer(se);
					}

				case SciterXBehaviors.EVENT_GROUPS.HANDLE_BEHAVIOR_EVENT:
					{
						var p = Marshal.PtrToStructure<SciterXBehaviors.BEHAVIOR_EVENT_PARAMS>(prms);
						SciterElement se2 = p.he != IntPtr.Zero ? new SciterElement(p.he) : null;
						return OnEvent(se, se2, p.cmd, p.reason, new SciterValue(p.data));
					}

				case SciterXBehaviors.EVENT_GROUPS.HANDLE_METHOD_CALL:
					return OnMethodCall(se, Marshal.PtrToStructure<SciterXDom.METHOD_PARAMS>(prms).methodID);

				case SciterXBehaviors.EVENT_GROUPS.HANDLE_DATA_ARRIVED:
					return OnDataArrived(se, Marshal.PtrToStructure<SciterXBehaviors.DATA_ARRIVED_PARAMS>(prms));

				case SciterXBehaviors.EVENT_GROUPS.HANDLE_SCROLL:
					return OnScroll(se, Marshal.PtrToStructure<SciterXBehaviors.SCROLL_PARAMS>(prms));

				case SciterXBehaviors.EVENT_GROUPS.HANDLE_SIZE:
					OnSize(se);
					return false;

				case SciterXBehaviors.EVENT_GROUPS.HANDLE_SCRIPTING_METHOD_CALL:
					{
						IntPtr RESULT_OFFSET = Marshal.OffsetOf(typeof(SciterXBehaviors.SCRIPTING_METHOD_PARAMS), "result");
#if DEBUG
#if OSX
						if(IntPtr.Size == 4)
							Debug.Assert(RESULT_OFFSET.ToInt32() == 12);
#else
						if(IntPtr.Size == 4)
							Debug.Assert(RESULT_OFFSET.ToInt32() == 16);
#endif
						else if(IntPtr.Size == 8)
							Debug.Assert(RESULT_OFFSET.ToInt32() == 24);
#endif

						var p = Marshal.PtrToStructure<SciterXBehaviors.SCRIPTING_METHOD_PARAMS>(prms);
						var pw = new SciterXBehaviors.SCRIPTING_METHOD_PARAMS_Wraper(p);

						bool bOK = OnScriptCall(se, pw.name, pw.args, out pw.result);
						if(bOK && pw.result != null)
						{
							SciterXValue.VALUE vres = pw.result.ToVALUE();
							IntPtr vptr = IntPtr.Add(prms, RESULT_OFFSET.ToInt32());
							Marshal.StructureToPtr(vres, vptr, false);
						}

						return bOK;
					}

				case SciterXBehaviors.EVENT_GROUPS.HANDLE_GESTURE:
					return OnGesture(se, Marshal.PtrToStructure<SciterXBehaviors.GESTURE_PARAMS>(prms));

				case SciterXBehaviors.EVENT_GROUPS.HANDLE_EXCHANGE:
					return OnExchange(se, Marshal.PtrToStructure<SciterXBehaviors.EXCHANGE_PARAMS>(prms));

				case SciterXBehaviors.EVENT_GROUPS.HANDLE_ATTRIBUTE_CHANGE:
					OnAttributeChange(se, Marshal.PtrToStructure<SciterXBehaviors.ATTRIBUTE_CHANGE_PARAMS>(prms));
					return false;

				case SciterXBehaviors.EVENT_GROUPS.HANDLE_STYLE_CHANGE:
					OnStyleChange(se, (uint)prms);
					return false;

				default:
					Debug.Assert(false);
					return false;
			}
		}
	}
}