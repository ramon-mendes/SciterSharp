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
	public static class SciterXDom
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct HPOSITION { public IntPtr hn; public int pos; }

		public enum SCDOM_RESULT : int
		{
			SCDOM_OK = 0,
			SCDOM_INVALID_HWND = 1,
			SCDOM_INVALID_HANDLE = 2,
			SCDOM_PASSIVE_HANDLE = 3,
			SCDOM_INVALID_PARAMETER = 4,
			SCDOM_OPERATION_FAILED = 5,
			SCDOM_OK_NOT_HANDLED = (-1)
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct METHOD_PARAMS
		{
			public SciterXBehaviors.BEHAVIOR_METHOD_IDENTIFIERS methodID;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct REQUEST_PARAM
		{
			[MarshalAs(UnmanagedType.LPWStr)]
			public string name;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string value;
		}


		// alias LPCBYTE_RECEIVER = void function(LPCBYTE bytes, UINT num_bytes, LPVOID param);
		public delegate void FPTR_LPCBYTE_RECEIVER(IntPtr bytes, uint num_bytes, IntPtr param);
		// alias LPCWSTR_RECEIVER = void function(LPCWSTR str, UINT str_length, LPVOID param);
		public delegate void FPTR_LPCWSTR_RECEIVER(IntPtr str, uint str_length, IntPtr param);
		// alias LPCSTR_RECEIVER = void function(LPCSTR str, UINT str_length, LPVOID param);
		public delegate void FPTR_LPCSTR_RECEIVER(IntPtr str, uint str_length, IntPtr param);


		public enum ELEMENT_AREAS : uint
		{
			ROOT_RELATIVE = 0x01,       // - or this flag if you want to get HTMLayout window relative coordinates,
										//   otherwise it will use nearest windowed container e.g. popup window.
			SELF_RELATIVE = 0x02,       // - "or" this flag if you want to get coordinates relative to the origin
										//   of element iself.
			CONTAINER_RELATIVE = 0x03,  // - position inside immediate container.
			VIEW_RELATIVE = 0x04,       // - position relative to view - HTMLayout window

			CONTENT_BOX = 0x00, // content (inner)  box
			PADDING_BOX = 0x10, // content + paddings
			BORDER_BOX = 0x20,  // content + paddings + border
			MARGIN_BOX = 0x30,  // content + paddings + border + margins 

			BACK_IMAGE_AREA = 0x40, // relative to content origin - location of background image (if it set no-repeat)
			FORE_IMAGE_AREA = 0x50, // relative to content origin - location of foreground image (if it set no-repeat)

			SCROLLABLE_AREA = 0x60, // scroll_area - scrollable area in content box 
		}


		public enum SCITER_SCROLL_FLAGS : uint
		{
			SCROLL_TO_TOP = 0x01,
			SCROLL_SMOOTH = 0x10,
		}


		// alias BOOL function(HELEMENT he, LPVOID param) SciterElementCallback;
		public delegate bool FPTR_SciterElementCallback(IntPtr he, IntPtr param);

		public enum SET_ELEMENT_HTML : uint
		{
			SIH_REPLACE_CONTENT = 0,
			SIH_INSERT_AT_START = 1,
			SIH_APPEND_AFTER_LAST = 2,
			SOH_REPLACE = 3,
			SOH_INSERT_BEFORE = 4,
			SOH_INSERT_AFTER = 5
		}

		// ElementEventProc is defined in SciterXBehaviors.FPTR_ElementEventProc
		// alias BOOL function(LPVOID tag, HELEMENT he, UINT evtg, LPVOID prms) ElementEventProc; 
		// public delegate bool FPTR_ElementEventProc(IntPtr tag, IntPtr he, uint evtg, IntPtr prms);


		public enum ELEMENT_STATE_BITS : uint
		{
			STATE_LINK             = 0x00000001,
			STATE_HOVER            = 0x00000002,
			STATE_ACTIVE           = 0x00000004,
			STATE_FOCUS            = 0x00000008,
			STATE_VISITED          = 0x00000010,
			STATE_CURRENT          = 0x00000020,  // current (hot) item 
			STATE_CHECKED          = 0x00000040,  // element is checked (or selected)
			STATE_DISABLED         = 0x00000080,  // element is disabled
			STATE_READONLY         = 0x00000100,  // readonly input element 
			STATE_EXPANDED         = 0x00000200,  // expanded state - nodes in tree view 
			STATE_COLLAPSED        = 0x00000400,  // collapsed state - nodes in tree view - mutually exclusive with
			STATE_INCOMPLETE       = 0x00000800,  // one of fore/back images requested but not delivered
			STATE_ANIMATING        = 0x00001000,  // is animating currently
			STATE_FOCUSABLE        = 0x00002000,  // will accept focus
			STATE_ANCHOR           = 0x00004000,  // anchor in selection (used with current in selects)
			STATE_SYNTHETIC        = 0x00008000,  // this is a synthetic element - don't emit it's head/tail
			STATE_OWNS_POPUP       = 0x00010000,  // this is a synthetic element - don't emit it's head/tail
			STATE_TABFOCUS         = 0x00020000,  // focus gained by tab traversal
			STATE_EMPTY            = 0x00040000,  // empty - element is empty (text.size() == 0 && subs.size() == 0) 
												  //  if element has behavior attached then the behavior is responsible for the value of this flag.
			STATE_BUSY             = 0x00080000,  // busy; loading

			STATE_DRAG_OVER        = 0x00100000,  // drag over the block that can accept it (so is current drop target). Flag is set for the drop target block
			STATE_DROP_TARGET      = 0x00200000,  // active drop target.
			STATE_MOVING           = 0x00400000,  // dragging/moving - the flag is set for the moving block.
			STATE_COPYING          = 0x00800000,  // dragging/copying - the flag is set for the copying block.
			STATE_DRAG_SOURCE      = 0x01000000,  // element that is a drag source.
			STATE_DROP_MARKER      = 0x02000000,  // element is drop marker

			STATE_PRESSED          = 0x04000000,  // pressed - close to active but has wider life span - e.g. in MOUSE_UP it 
												  //   is still on; so behavior can check it in MOUSE_UP to discover CLICK condition.
			STATE_POPUP            = 0x08000000,  // this element is out of flow - popup 

			STATE_IS_LTR           = 0x10000000,  // the element or one of its containers has dir=ltr declared
			STATE_IS_RTL           = 0x20000000,  // the element or one of its containers has dir=rtl declared
		}

		public enum REQUEST_TYPE
		{
			GET_ASYNC,  // async GET
			POST_ASYNC, // async POST
			GET_SYNC,   // synchronous GET 
			POST_SYNC   // synchronous POST 
		}


		// alias int function(HELEMENT he1, HELEMENT he2, LPVOID param) ELEMENT_COMPARATOR;
		public delegate bool FPTR_ELEMENT_COMPARATOR(IntPtr he1, IntPtr he2, IntPtr param);


		public enum CTL_TYPE : uint
		{
			CTL_NO = 0,					// This dom element has no behavior at all.
			CTL_UNKNOWN = 1,			// This dom element has behavior but its type is unknown.
			
			CTL_EDIT = 2,				// Single line edit box.
			CTL_NUMERIC = 3,			// Numeric input with optional spin buttons.
			CTL_CLICKABLE = 4,			// toolbar button, behavior:clickable.
			CTL_BUTTON = 5,				// Command button.
			CTL_CHECKBOX = 6,			// CheckBox (button).
			CTL_RADIO = 7,				// OptionBox (button).
			CTL_SELECT_SINGLE = 8,		// Single select, ListBox or TreeView.
			CTL_SELECT_MULTIPLE = 9,	// Multiselectable select, ListBox or TreeView.
			CTL_DD_SELECT = 10,			// Dropdown single select.
			CTL_TEXTAREA = 11,			// Multiline TextBox.
			CTL_HTMLAREA = 12,			// HTML selection behavior.
			CTL_PASSWORD = 13,			// Password input element.
			CTL_PROGRESS = 14,			// Progress element.
			CTL_SLIDER = 15,			// Slider input element.  
			CTL_DECIMAL = 16,			// Decimal number input element.
			CTL_CURRENCY = 17,			// Currency input element.
			CTL_SCROLLBAR = 18,
			CTL_LIST = 19,
			CTL_RICHTEXT = 20,
			CTL_CALENDAR = 21,
			CTL_DATE = 22,
			CTL_TIME = 23,
			CTL_FILE = 24,				// file input element.
			CTL_PATH = 25,				// path input element.

			CTL_LAST_INPUT = 26,

			CTL_HYPERLINK = CTL_LAST_INPUT,
			CTL_FORM = 27,

			CTL_MENUBAR = 28,
			CTL_MENU = 29,
			CTL_MENUBUTTON = 30,

			CTL_FRAME = 31,
			CTL_FRAMESET = 32,

			CTL_TOOLTIP = 33,

			CTL_HIDDEN = 34,
			CTL_URL = 35,				// URL input element.
			CTL_TOOLBAR = 36,

			CTL_WINDOW = 37,			// has HWND attached to it

			CTL_LABEL = 38,
			CTL_IMAGE = 39,				// image/video object.  
			CTL_PLAINTEXT = 40,			// Multiline TextBox + colorizer.
		}

		public enum NODE_TYPE : uint
		{
			NT_ELEMENT,
			NT_TEXT,
			NT_COMMENT 
		}

		public enum NODE_INS_TARGET : uint
		{
			NIT_BEFORE,
			NIT_AFTER,
			NIT_APPEND,
			NIT_PREPEND,
		}
	}
}