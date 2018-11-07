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

#pragma warning disable 0169

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SciterSharp.Interop
{
	public static class SciterXBehaviors
	{
		public enum EVENT_GROUPS : uint
		{
			HANDLE_INITIALIZATION = 0x0000, /** attached/detached */
			HANDLE_MOUSE = 0x0001,          /** mouse events */
			HANDLE_KEY = 0x0002,            /** key events */
			HANDLE_FOCUS = 0x0004,          /** focus events, if this flag is set it also means that element it attached to is focusable */
			HANDLE_SCROLL = 0x0008,         /** scroll events */
			HANDLE_TIMER = 0x0010,          /** timer event */
			HANDLE_SIZE = 0x0020,           /** size changed event */
			HANDLE_DRAW = 0x0040,           /** drawing request (event) */
			HANDLE_DATA_ARRIVED = 0x080,    /** requested data () has been delivered */
			HANDLE_BEHAVIOR_EVENT        = 0x0100, /** logical, synthetic events:
													BUTTON_CLICK, HYPERLINK_CLICK, etc.,
													a.k.a. notifications from intrinsic behaviors */
			HANDLE_METHOD_CALL           = 0x0200, /** behavior specific methods */
			HANDLE_SCRIPTING_METHOD_CALL = 0x0400, /** behavior specific methods */
			HANDLE_TISCRIPT_METHOD_CALL  = 0x0800, /** behavior specific methods using direct tiscript::value's */

			HANDLE_EXCHANGE              = 0x1000, /** system drag-n-drop */
			HANDLE_GESTURE               = 0x2000, /** touch input events */

			HANDLE_ALL                   = 0xFFFF, /* all of them */

			SUBSCRIPTIONS_REQUEST        = 0xFFFFFFFF, /* special value for getting subscription flags */
		}


		// alias BOOL function(LPVOID tag, HELEMENT he, UINT evtg, LPVOID prms) ElementEventProc;
		public delegate bool FPTR_ElementEventProc(IntPtr tag, IntPtr he, uint evtg, IntPtr prms);
		// alias BOOL function(LPCSTR, HELEMENT, LPElementEventProc*, LPVOID*) SciterBehaviorFactory;
		public delegate bool FPTR_SciterBehaviorFactory([MarshalAs(UnmanagedType.LPStr)]string s, IntPtr he, out FPTR_ElementEventProc proc, out IntPtr tag);

		[Flags]
		public enum PHASE_MASK : uint
		{
			BUBBLING = 0,		// bubbling (emersion) phase
			SINKING  = 0x8000,	// capture (immersion) phase, this flag is or'ed with EVENTS codes below
			HANDLED  = 0x10000
		}

		[Flags]
		public enum MOUSE_BUTTONS : uint
		{
			MAIN_MOUSE_BUTTON = 0x1,
			PROP_MOUSE_BUTTON = 0x2,
			MIDDLE_MOUSE_BUTTON = 0x4,
		}

		[Flags]
		public enum KEYBOARD_STATES : uint
		{
			CONTROL_KEY_PRESSED = 0x1,
			SHIFT_KEY_PRESSED = 0x2,
			ALT_KEY_PRESSED = 0x4
		}
	
		public enum INITIALIZATION_EVENTS : uint
		{
			BEHAVIOR_DETACH = 0,
			BEHAVIOR_ATTACH = 1
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct INITIALIZATION_PARAMS
		{
			public INITIALIZATION_EVENTS cmd;
		}

		public enum DRAGGING_TYPE : uint
		{
			NO_DRAGGING,
			DRAGGING_MOVE,
			DRAGGING_COPY,
		}
		
		public enum MOUSE_EVENTS : uint
		{
			MOUSE_ENTER = 0,
			MOUSE_LEAVE,
			MOUSE_MOVE,
			MOUSE_UP,
			MOUSE_DOWN,
			MOUSE_DCLICK,
			MOUSE_WHEEL,
			MOUSE_TICK,
			MOUSE_IDLE,
			DROP        = 9,
			DRAG_ENTER  = 0xA,
			DRAG_LEAVE  = 0xB,  
			DRAG_REQUEST = 0xC,
			MOUSE_CLICK = 0xFF,
			DRAGGING = 0x100,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MOUSE_PARAMS
		{
			public MOUSE_EVENTS			cmd;// MOUSE_EVENTS
			public IntPtr				target;// HELEMENT
			public PInvokeUtils.POINT	pos;// POINT
			public PInvokeUtils.POINT	pos_view;// POINT
			public uint				button_state;// UINT ->> actually SciterXBehaviorsMOUSE_BUTTONS, but for MOUSE_EVENTS.MOUSE_WHEEL event it is the delta
			public KEYBOARD_STATES	alt_state;// UINT
			public uint				cursor_type;// UINT
			public bool				is_on_icon;// BOOL
			public IntPtr			dragging;// HELEMENT
			public uint				dragging_mode;// UINT
		}

		public enum CURSOR_TYPE : uint
		{
			CURSOR_ARROW,
			CURSOR_IBEAM,
			CURSOR_WAIT,
			CURSOR_CROSS,
			CURSOR_UPARROW,
			CURSOR_SIZENWSE,
			CURSOR_SIZENESW,
			CURSOR_SIZEWE,
			CURSOR_SIZENS,
			CURSOR_SIZEALL,
			CURSOR_NO,
			CURSOR_APPSTARTING,
			CURSOR_HELP,
			CURSOR_HAND,
			CURSOR_DRAG_MOVE,
			CURSOR_DRAG_COPY,
		}
		
		public enum KEY_EVENTS : uint
		{
			KEY_DOWN = 0,
			KEY_UP,
			KEY_CHAR
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct KEY_PARAMS
		{
			public KEY_EVENTS	cmd;
			public IntPtr		target;// HELEMENT
			public uint			key_code;
			public KEYBOARD_STATES alt_state;
		}

		public enum FOCUS_EVENTS : uint
		{
			FOCUS_OUT = 0,      // container got focus on element inside it, target is an element that got focus
			FOCUS_IN = 1,       // container lost focus from any element inside it, target is an element that lost focus
			FOCUS_GOT = 2,      // target element got focus
			FOCUS_LOST = 3,     // target element lost focus
			FOCUS_REQUEST = 4,	// bubbling event/request, gets sent on child-parent chain to accept/reject focus to be set on the child (target)
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct FOCUS_PARAMS
		{
			public FOCUS_EVENTS	cmd;
			public IntPtr		target;// HELEMENT
			public bool			by_mouse_click;
			public bool			cancel;
		}
	
		public enum SCROLL_EVENTS : uint
		{
			SCROLL_HOME = 0,
			SCROLL_END,
			SCROLL_STEP_PLUS,
			SCROLL_STEP_MINUS,
			SCROLL_PAGE_PLUS,
			SCROLL_PAGE_MINUS,
			SCROLL_POS,
			SCROLL_SLIDER_RELEASED,
			SCROLL_CORNER_PRESSED,
			SCROLL_CORNER_RELEASED,
		}

		public enum SCROLL_SOURCE
		{
			SCROLL_SOURCE_UNKNOWN,
			SCROLL_SOURCE_KEYBOARD,  // SCROLL_PARAMS::reason <- keyCode
			SCROLL_SOURCE_SCROLLBAR, // SCROLL_PARAMS::reason <- SCROLLBAR_PART 
			SCROLL_SOURCE_ANIMATOR,
		}

		public enum SCROLLBAR_PART
		{
			SCROLLBAR_BASE,
			SCROLLBAR_PLUS,
			SCROLLBAR_MINUS,
			SCROLLBAR_SLIDER,
			SCROLLBAR_PAGE_MINUS,
			SCROLLBAR_PAGE_PLUS,
			SCROLLBAR_CORNER,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SCROLL_PARAMS
		{
			public SCROLL_EVENTS	cmd;
			public IntPtr			target;// HELEMENT
			public int				pos;
			public bool				vertical;
			public SCROLL_SOURCE	source;	// SCROLL_SOURCE
			public uint				reason; // key or SCROLLBAR_PART
		}

		public enum GESTURE_CMD : uint
		{
			GESTURE_REQUEST = 0,
			GESTURE_ZOOM,
			GESTURE_PAN,
			GESTURE_ROTATE,
			GESTURE_TAP1,
			GESTURE_TAP2,
		}
	
		public enum GESTURE_STATE : uint
		{
			GESTURE_STATE_BEGIN   = 1,
			GESTURE_STATE_INERTIA = 2,
			GESTURE_STATE_END     = 4,
		}

		public enum GESTURE_TYPE_FLAGS : uint
		{
			GESTURE_FLAG_ZOOM               = 0x0001,
			GESTURE_FLAG_ROTATE             = 0x0002,
			GESTURE_FLAG_PAN_VERTICAL       = 0x0004,
			GESTURE_FLAG_PAN_HORIZONTAL     = 0x0008,
			GESTURE_FLAG_TAP1               = 0x0010,
			GESTURE_FLAG_TAP2               = 0x0020,
			GESTURE_FLAG_PAN_WITH_GUTTER    = 0x4000,
			GESTURE_FLAG_PAN_WITH_INERTIA   = 0x8000,
			GESTURE_FLAGS_ALL               = 0xFFFF,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct GESTURE_PARAMS
		{
			public GESTURE_CMD	cmd;
			public IntPtr		target;
			public PInvokeUtils.POINT	pos;
			public PInvokeUtils.POINT	pos_view;
			/// <summary>
			/// GESTURE_TYPE_FLAGS or GESTURE_STATE combination
			/// </summary>
			public uint					flags;
			public uint					delta_time;
			public PInvokeUtils.SIZE	delta_xy;
			public double				delta_v;
		}

		public enum DRAW_EVENTS : uint
		{
			DRAW_BACKGROUND = 0,
			DRAW_CONTENT = 1,
			DRAW_FOREGROUND = 2,
			DRAW_OUTLINE = 3,
		}

		//struct SCITER_GRAPHICS;

		[StructLayout(LayoutKind.Sequential)]
		public struct DRAW_PARAMS
		{
			public DRAW_EVENTS			cmd;
			public IntPtr				gfx;	// HGFX - hdc to paint on
			public PInvokeUtils.RECT	area;	// element area, to get invalid area to paint use GetClipBox,
			public uint					reserved;	// for DRAW_BACKGROUND/DRAW_FOREGROUND - it is a border box
													// for DRAW_CONTENT - it is a content box
		}

		public enum CONTENT_CHANGE_BITS : uint // for CONTENT_CHANGED reason
		{
			CONTENT_ADDED = 0x01,
			CONTENT_REMOVED = 0x02,
		}

		public enum BEHAVIOR_EVENTS : uint
		{
			BUTTON_CLICK = 0,				// click on button
			BUTTON_PRESS = 1,				// mouse down or key down in button
			BUTTON_STATE_CHANGED = 2,		// checkbox/radio/slider changed its state/value
			EDIT_VALUE_CHANGING = 3,		// before text change
			EDIT_VALUE_CHANGED = 4,			// after text change
			SELECT_SELECTION_CHANGED = 5,	// selection in <select> changed
			SELECT_STATE_CHANGED = 6,		// node in select expanded/collapsed, heTarget is the node

			POPUP_REQUEST   = 7,			// request to show popup just received,
											//     here DOM of popup element can be modifed.
			POPUP_READY     = 8,			// popup element has been measured and ready to be shown on screen,
											//     here you can use functions like ScrollToView.
			POPUP_DISMISSED = 9,			// popup element is closed,
											//     here DOM of popup element can be modifed again - e.g. some items can be removed
											//     to free memory.

			MENU_ITEM_ACTIVE = 0xA,			// menu item activated by mouse hover or by keyboard,
			MENU_ITEM_CLICK = 0xB,			// menu item click,
											//   BEHAVIOR_EVENT_PARAMS structure layout
											//   BEHAVIOR_EVENT_PARAMS.cmd - MENU_ITEM_CLICK/MENU_ITEM_ACTIVE
											//   BEHAVIOR_EVENT_PARAMS.heTarget - owner(anchor) of the menu
											//   BEHAVIOR_EVENT_PARAMS.he - the menu item, presumably <li> element
											//   BEHAVIOR_EVENT_PARAMS.reason - BY_MOUSE_CLICK | BY_KEY_CLICK


			CONTEXT_MENU_REQUEST = 0x10,	// "right-click", BEHAVIOR_EVENT_PARAMS::he is current popup menu HELEMENT being processed or NULL.
											// application can provide its own HELEMENT here (if it is NULL) or modify current menu element.

			VISIUAL_STATUS_CHANGED = 0x11,	// broadcast notification, sent to all elements of some container being shown or hidden
			DISABLED_STATUS_CHANGED = 0x12,	// broadcast notification, sent to all elements of some container that got new value of :disabled state

			POPUP_DISMISSING = 0x13,		// popup is about to be closed

			CONTENT_CHANGED = 0x15,			// content has been changed, is posted to the element that gets content changed,  reason is combination of CONTENT_CHANGE_BITS.
											// target == NULL means the window got new document and this event is dispatched only to the window.
			CLICK = 0x16,					// generic click
			CHANGE = 0x17,					// generic change

			// "grey" event codes  - notfications from behaviors from this SDK
			HYPERLINK_CLICK = 0x80,			// hyperlink click

			//TABLE_HEADER_CLICK,			// click on some cell in table header,
			//								//     target = the cell,
			//								//     reason = index of the cell (column number, 0..n)
			//TABLE_ROW_CLICK,				// click on data row in the table, target is the row
			//								//     target = the row,
			//								//     reason = index of the row (fixed_rows..n)
			//TABLE_ROW_DBL_CLICK,			// mouse dbl click on data row in the table, target is the row
			//								//     target = the row,
			//								//     reason = index of the row (fixed_rows..n)

			ELEMENT_COLLAPSED	= 0x90,		// element was collapsed, so far only behavior:tabs is sending these two to the panels
			ELEMENT_EXPANDED	= 0x91,		// element was expanded,

			ACTIVATE_CHILD		= 0x92,		// activate (select) child,
											// used for example by accesskeys behaviors to send activation request, e.g. tab on behavior:tabs.

			//DO_SWITCH_TAB = ACTIVATE_CHILD,// command to switch tab programmatically, handled by behavior:tabs
			//                               // use it as HTMLayoutPostEvent(tabsElementOrItsChild, DO_SWITCH_TAB, tabElementToShow, 0);

			//INIT_DATA_VIEW,				// request to virtual grid to initialize its view

			//ROWS_DATA_REQUEST,			// request from virtual grid to data source behavior to fill data in the table
											// parameters passed throug DATA_ROWS_PARAMS structure.

			UI_STATE_CHANGED	= 0x95,		// ui state changed, observers shall update their visual states.
											// is sent for example by behavior:richtext when caret position/selection has changed.

			FORM_SUBMIT			= 0x96,		// behavior:form detected submission event. BEHAVIOR_EVENT_PARAMS::data field contains data to be posted.
											// BEHAVIOR_EVENT_PARAMS::data is of type T_MAP in this case key/value pairs of data that is about 
											// to be submitted. You can modify the data or discard submission by returning true from the handler.
			FORM_RESET			= 0x97,		// behavior:form detected reset event (from button type=reset). BEHAVIOR_EVENT_PARAMS::data field contains data to be reset.
											// BEHAVIOR_EVENT_PARAMS::data is of type T_MAP in this case key/value pairs of data that is about 
											// to be rest. You can modify the data or discard reset by returning true from the handler.

			DOCUMENT_COMPLETE	= 0x98,		// document in behavior:frame or root document is complete.

			HISTORY_PUSH = 0x99,			// requests to behavior:history (commands)
			HISTORY_DROP = 0x9A,
			HISTORY_PRIOR = 0x9B,
			HISTORY_NEXT = 0x9C,
			HISTORY_STATE_CHANGED = 0x9D,	// behavior:history notification - history stack has changed

			CLOSE_POPUP = 0x9E,				// close popup request,
			REQUEST_TOOLTIP = 0x9F,			// request tooltip, evt.source <- is the tooltip element.

			ANIMATION			= 0xA0,		// animation started (reason=1) or ended(reason=0) on the element.
			DOCUMENT_CREATED	= 0xC0,		// document created, script namespace initialized. target -> the document
			DOCUMENT_CLOSE_REQUEST = 0xC1,	// document is about to be closed, to cancel closing do: evt.data = sciter::value("cancel");
			DOCUMENT_CLOSE		= 0xC2,		// last notification before document removal from the DOM
			DOCUMENT_READY		= 0xC3,       // document has got DOM structure, styles and behaviors of DOM elements. Script loading run is complete at this moment. 
			DOCUMENT_PARSED		= 0xC4,      // document just finished parsing - has got DOM structure. This event is generated before DOCUMENT_READY


			VIDEO_INITIALIZED = 0xD1,		// <video> "ready" notification   
			VIDEO_STARTED     = 0xD2,		// <video> playback started notification   
			VIDEO_STOPPED     = 0xD3,		// <video> playback stoped/paused notification   
			VIDEO_BIND_RQ     = 0xD4,		// <video> request for frame source binding, 
											//   If you want to provide your own video frames source for the given target <video> element do the following:
											//   1. Handle and consume this VIDEO_BIND_RQ request 
											//   2. You will receive second VIDEO_BIND_RQ request/event for the same <video> element
											//      but this time with the 'reason' field set to an instance of sciter::video_destination interface.
											//   3. add_ref() it and store it for example in worker thread producing video frames.
											//   4. call sciter::video_destination::start_streaming(...) providing needed parameters
											//      call sciter::video_destination::render_frame(...) as soon as they are available
											//      call sciter::video_destination::stop_streaming() to stop the rendering (a.k.a. end of movie reached)
		
			PAGINATION_STARTS  = 0xE0,		// behavior:pager starts pagination
			PAGINATION_PAGE    = 0xE1,		// behavior:pager paginated page no, reason -> page no
			PAGINATION_ENDS    = 0xE2,		// behavior:pager end pagination, reason -> total pages
		
			FIRST_APPLICATION_EVENT_CODE = 0x100
			// all custom event codes shall be greater
			// than this number. All codes below this will be used
			// solely by application - HTMLayout will not interpret it
			// and will do just dispatching.
			// To send event notifications with these codes use
			// HTMLayoutSend/PostEvent API.
		}

		public enum EVENT_REASON : uint
		{
			BY_MOUSE_CLICK,
			BY_KEY_CLICK,
			SYNTHESIZED,
			BY_MOUSE_ON_ICON,
		}

		public enum EDIT_CHANGED_REASON : uint
		{
			BY_INS_CHAR,
			BY_INS_CHARS,
			BY_DEL_CHAR,
			BY_DEL_CHARS,
			BY_UNDO_REDO,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct BEHAVIOR_EVENT_PARAMS
		{
			public BEHAVIOR_EVENTS cmd;
			public IntPtr	heTarget;// HELEMENT
			public IntPtr	he;// HELEMENT
			public IntPtr	reason;// UINT_PTR
			public SciterXValue.VALUE data;// SCITER_VALUE
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct TIMER_PARAMS
		{
			public IntPtr timerId;// UINT_PTR
		}
	
		public enum BEHAVIOR_METHOD_IDENTIFIERS : uint
		{
			DO_CLICK = 0,
			/*GET_TEXT_VALUE = 1,
			SET_TEXT_VALUE,
			TEXT_EDIT_GET_SELECTION,
			TEXT_EDIT_SET_SELECTION,
			TEXT_EDIT_REPLACE_SELECTION,
			SCROLL_BAR_GET_VALUE,
			SCROLL_BAR_SET_VALUE,
			TEXT_EDIT_GET_CARET_POSITION, 
			TEXT_EDIT_GET_SELECTION_TEXT,
			TEXT_EDIT_GET_SELECTION_HTML,
			TEXT_EDIT_CHAR_POS_AT_XY,*/
			IS_EMPTY      = 0xFC,
			GET_VALUE     = 0xFD,
			SET_VALUE     = 0xFE,
			FIRST_APPLICATION_METHOD_ID = 0x100
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SCRIPTING_METHOD_PARAMS
		{
			public IntPtr name;// LPCSTR
			public IntPtr argv;// VALUE*
			public uint argc;
			public SciterXValue.VALUE result;	// plz note, Sciter will internally call ValueClear to this VALUE,
												// that is, it own this data, so always assign a copy with a positive ref-count of your VALUE to this variable
												// you will know that if you get an "Access Violation" error
		}

		// SCRIPTING_METHOD_PARAMS wraper
		public struct SCRIPTING_METHOD_PARAMS_Wraper
		{
			public SCRIPTING_METHOD_PARAMS_Wraper(SCRIPTING_METHOD_PARAMS prms)
			{
				name = Marshal.PtrToStringAnsi(prms.name);
				args = new SciterValue[prms.argc];
				result = SciterValue.Undefined;

				for(int i = 0; i < prms.argc; i++)
					args[i] = new SciterValue( (SciterXValue.VALUE) Marshal.PtrToStructure(IntPtr.Add(prms.argv, i * Marshal.SizeOf(typeof(SciterXValue.VALUE))), typeof(SciterXValue.VALUE)) );
			}

			public string name;
			public SciterValue[] args;
			public SciterValue result;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct TISCRIPT_METHOD_PARAMS
		{
			public IntPtr vm;// tiscript_VM*
			public TIScript.tiscript_value tag;
			public TIScript.tiscript_value result;
		}
	
		// GET_VALUE/SET_VALUE methods params
		[StructLayout(LayoutKind.Sequential)]
		public struct VALUE_PARAMS 
		{
			public uint methodID;
			public SciterXValue.VALUE val;// SCITER_VALUE
		}

		// IS_EMPTY method params
		[StructLayout(LayoutKind.Sequential)]
		public struct IS_EMPTY_PARAMS
		{
			public uint methodID;
			public uint is_empty; // !0 - is empty
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DATA_ARRIVED_PARAMS
		{
			public IntPtr	initiator;// HELEMENT
			public byte[]	data;// LPCBYTE
			public uint	dataSize;
			public uint	dataType;
			public uint	status;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string uri;// LPCWSTR
		}
	}
}

#pragma warning restore 0169