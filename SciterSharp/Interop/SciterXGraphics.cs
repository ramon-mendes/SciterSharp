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
	public static class SciterXGraphics
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct COLOR_STOP
		{
			uint color;
			float offset;// 0.0 ... 1.0
		}

		public enum GRAPHIN_RESULT : int
		{
			GRAPHIN_PANIC = -1,		// e.g. not enough memory
			GRAPHIN_OK = 0,
			GRAPHIN_BAD_PARAM = 1,	// bad parameter
			GRAPHIN_FAILURE = 2,	// operation failed, e.g. restore() without save()
			GRAPHIN_NOTSUPPORTED = 3// the platform does not support requested feature
		}

		public enum DRAW_PATH_MODE
		{
			DRAW_FILL_ONLY = 1,
			DRAW_STROKE_ONLY = 2,
			DRAW_FILL_AND_STROKE = 3,
		}

		public enum SCITER_LINE_JOIN_TYPE
		{
			SCITER_JOIN_MITER = 0,
			SCITER_JOIN_ROUND = 1,
			SCITER_JOIN_BEVEL = 2,
			SCITER_JOIN_MITER_OR_BEVEL = 3,
		}

		public enum SCITER_LINE_CAP_TYPE
		{
			SCITER_LINE_CAP_BUTT = 0,
			SCITER_LINE_CAP_SQUARE = 1,
			SCITER_LINE_CAP_ROUND = 2,
		}

		public enum SCITER_TEXT_ALIGNMENT
		{
			TEXT_ALIGN_DEFAULT,
			TEXT_ALIGN_START,
			TEXT_ALIGN_END,
			TEXT_ALIGN_CENTER,
		}

		public enum SCITER_TEXT_DIRECTION
		{
			TEXT_DIRECTION_DEFAULT,
			TEXT_DIRECTION_LTR,
			TEXT_DIRECTION_RTL,
			TEXT_DIRECTION_TTB,
		}

		public enum SCITER_IMAGE_ENCODING
		{
			SCITER_IMAGE_ENCODING_RAW,// [a,b,g,r,a,b,g,r,...] vector
			SCITER_IMAGE_ENCODING_PNG,
			SCITER_IMAGE_ENCODING_JPG,
			SCITER_IMAGE_ENCODING_WEBP,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SCITER_TEXT_FORMAT
		{
			[MarshalAs(UnmanagedType.LPWStr)]
			public string fontFamily;
			public uint fontWeight; // 100...900, 400 - normal, 700 - bold
			public bool fontItalic;
			public float fontSize;   // dips
			public float lineHeight; // dips
			public SCITER_TEXT_DIRECTION textDirection;
			public SCITER_TEXT_ALIGNMENT textAlignment; // horizontal alignment
			public SCITER_TEXT_ALIGNMENT lineAlignment; // a.k.a. vertical alignment for roman writing systems
			[MarshalAs(UnmanagedType.LPWStr)]
			public string localeName;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ISciterGraphicsAPI
		{
			public FPTR_imageCreate				imageCreate;
			public FPTR_imageCreateFromPixmap	imageCreateFromPixmap;
			public FPTR_imageAddRef				imageAddRef;
			public FPTR_imageRelease			imageRelease;
			public FPTR_imageGetInfo			imageGetInfo;
			public FPTR_imageClear				imageClear;
			public FPTR_imageLoad				imageLoad;
			public FPTR_imageSave				imageSave;

			public FPTR_RGBA					RGBA;

			public FPTR_gCreate					gCreate;
			public FPTR_gAddRef					gAddRef;
			public FPTR_gRelease				gRelease;

			public FPTR_gLine					gLine;
			public FPTR_gRectangle				gRectangle;
			public FPTR_gRoundedRectangle		gRoundedRectangle;
			public FPTR_gEllipse				gEllipse;
			public FPTR_gArc					gArc;
			public FPTR_gStar					gStar;
			public FPTR_gPolygon				gPolygon;
			public FPTR_gPolyline				gPolyline;

			public FPTR_pathCreate				pathCreate;
			public FPTR_pathAddRef				pathAddRef;
			public FPTR_pathRelease				pathRelease;
			public FPTR_pathMoveTo				pathMoveTo;
			public FPTR_pathLineTo				pathLineTo;
			public FPTR_pathArcTo				pathArcTo;
			public FPTR_pathQuadraticCurveTo	pathQuadraticCurveTo;
			public FPTR_pathBezierCurveTo		pathBezierCurveTo;
			public FPTR_pathClosePath			pathClosePath;
			public FPTR_gDrawPath				gDrawPath;

			public FPTR_gRotate					gRotate;
			public FPTR_gTranslate				gTranslate;
			public FPTR_gScale					gScale;
			public FPTR_gSkew					gSkew;
			public FPTR_gTransform				gTransform;

			public FPTR_gStateSave				gStateSave;
			public FPTR_gStateRestore			gStateRestore;

			public FPTR_gLineWidth				gLineWidth;
			public FPTR_gLineJoin				gLineJoin;
			public FPTR_gLineCap				gLineCap;
			public FPTR_gLineColor				gLineColor;
			public FPTR_gFillColor				gFillColor;
			public FPTR_gLineGradientLinear		gLineGradientLinear;
			public FPTR_gFillGradientLinear		gFillGradientLinear;
			public FPTR_gLineGradientRadial		gLineGradientRadial;
			public FPTR_gFillGradientRadial		gFillGradientRadial;
			public FPTR_gFillMode				gFillMode;

			public FPTR_textCreateForElement	textCreateForElement;
			public FPTR_textCreate				textCreate;
			public FPTR_textAddRef				textAddRef;
			public FPTR_textRelease				textRelease;
			public FPTR_textGetMetrics			textGetMetrics;
			public FPTR_textSetBox				textSetBox;
			public FPTR_gDrawText				gDrawText;

			public FPTR_gDrawImage				gDrawImage;

			public FPTR_gWorldToScreen			gWorldToScreen;
			public FPTR_gScreenToWorld			gScreenToWorld;

			public FPTR_gPushClipBox			gPushClipBox;
			public FPTR_gPushClipPath			gPushClipPath;
			public FPTR_gPopClip				gPopClip;

			public FPTR_imagePaint				imagePaint;

			public FPTR_vWrapGfx				vWrapGfx;
			public FPTR_vWrapImage				vWrapImage;
			public FPTR_vWrapPath				vWrapPath;
			public FPTR_vWrapText				vWrapText;
			public FPTR_vUnWrapGfx				vUnWrapGfx;
			public FPTR_vUnWrapImage			vUnWrapImage;
			public FPTR_vUnWrapPath				vUnWrapPath;
			public FPTR_vUnWrapText				vUnWrapText;


			#region SECTION: image primitives

			// GRAPHIN_RESULT SCFN(imageCreate)( HIMG* poutImg, UINT width, UINT height, BOOL withAlpha );
			public delegate GRAPHIN_RESULT FPTR_imageCreate(out IntPtr poutImg, uint width, uint height, bool withAlpha);

			// construct image from B[n+0],G[n+1],R[n+2],A[n+3] data.
			// Size of pixmap data is pixmapWidth*pixmapHeight*4
			// GRAPHIN_RESULT SCFN(imageCreateFromPixmap)( HIMG* poutImg, UINT pixmapWidth, UINT pixmapHeight, BOOL withAlpha, const BYTE* pixmapPixels );
			public delegate GRAPHIN_RESULT FPTR_imageCreateFromPixmap(out IntPtr poutImg, uint pixmapWidth, uint pixmapHeight, bool withAlpha, IntPtr pixmap);

			// GRAPHIN_RESULT SCFN(imageAddRef)( HIMG himg );
			public delegate GRAPHIN_RESULT FPTR_imageAddRef(IntPtr himg);

			// GRAPHIN_RESULT SCFN(imageRelease)( HIMG himg );
			public delegate GRAPHIN_RESULT FPTR_imageRelease(IntPtr himg);

			// GRAPHIN_RESULT SCFN(imageGetInfo)( HIMG himg, UINT* width, UINT* height, BOOL* usesAlpha );
			public delegate GRAPHIN_RESULT FPTR_imageGetInfo(IntPtr himg, out uint width, out uint height, out bool usesAlpha);

			// GRAPHIN_RESULT SCFN(imageClear)( HIMG himg, COLOR byColor );
			public delegate GRAPHIN_RESULT FPTR_imageClear(IntPtr himg, uint byColor);

			// GRAPHIN_RESULT SCFN(imageLoad)( const BYTE* bytes, UINT num_bytes, HIMG* pout_img ); // load png/jpeg/etc. image from stream of bytes
			public delegate GRAPHIN_RESULT FPTR_imageLoad(byte[] bytes, uint num_bytes, out IntPtr pout_img);

			/*
			GRAPHIN_RESULT SCFN(imageSave) // save png/jpeg/etc. image to stream of bytes
				( HIMG himg,
				image_write_function* pfn,
				void* prm, //function and its param passed "as is"
				UINT bpp, //24,32 if alpha needed
				UINT quality // png: 0, jpeg:, 10 - 100 );
			*/
			public delegate bool image_write_function(IntPtr prm, IntPtr data, uint data_length);

			public delegate bool image_paint_function(IntPtr prm, IntPtr hgfx, uint width, uint height);

			public delegate GRAPHIN_RESULT FPTR_imageSave(IntPtr himg, image_write_function pfn, IntPtr prm, SCITER_IMAGE_ENCODING bpp, uint quality);

			#endregion

			#region SECTION: graphics primitives and drawing operations
			// COLOR SCFN(RGBA)(UINT red, UINT green, UINT blue, UINT alpha /*= 255*/);
			public delegate uint FPTR_RGBA(uint red, uint green, uint blue, uint alpha);

			// GRAPHIN_RESULT SCFN(gCreate)(HIMG img, HGFX* pout_gfx );
			public delegate GRAPHIN_RESULT FPTR_gCreate(IntPtr img, out IntPtr pout_gfx);

			// GRAPHIN_RESULT SCFN(gAddRef) (HGFX hgfx);
			public delegate GRAPHIN_RESULT FPTR_gAddRef(IntPtr hgfx);

			// GRAPHIN_RESULT SCFN(gRelease) (HGFX hgfx)
			public delegate GRAPHIN_RESULT FPTR_gRelease(IntPtr hgfx);

			// Draws line from x1,y1 to x2,y2 using current lineColor and lineGradient.
			// GRAPHIN_RESULT SCFN(gLine) ( HGFX hgfx, POS x1, POS y1, POS x2, POS y2 );
			public delegate GRAPHIN_RESULT FPTR_gLine(IntPtr hgfx, float x1, float y1, float x2, float y2);

			// Draws rectangle using current lineColor/lineGradient and fillColor/fillGradient with (optional) rounded corners.
			// GRAPHIN_RESULT SCFN(gRectangle) ( HGFX hgfx, POS x1, POS y1, POS x2, POS y2 );
			public delegate GRAPHIN_RESULT FPTR_gRectangle(IntPtr hgfx, float x1, float y1, float x2, float y2);

			// Draws rounded rectangle using current lineColor/lineGradient and fillColor/fillGradient with (optional) rounded corners.
			// GRAPHIN_RESULT SCFN(gRoundedRectangle) ( HGFX hgfx, POS x1, POS y1, POS x2, POS y2, const DIM* radii8 /*DIM[8] - four rx/ry pairs */);
			public delegate GRAPHIN_RESULT FPTR_gRoundedRectangle(IntPtr hgfx, float x1, float y1, float x2, float y2, float[] radii8);

			// Draws circle or ellipse using current lineColor/lineGradient and fillColor/fillGradient.
			// GRAPHIN_RESULT SCFN(gEllipse) ( HGFX hgfx, POS x, POS y, DIM rx, DIM ry );
			public delegate GRAPHIN_RESULT FPTR_gEllipse(IntPtr hgfx, float x, float y, float rx, float ry);

			// Draws closed arc using current lineColor/lineGradient and fillColor/fillGradient.
			// GRAPHIN_RESULT SCFN(gArc) ( HGFX hgfx, POS x, POS y, POS rx, POS ry, ANGLE start, ANGLE sweep );
			public delegate GRAPHIN_RESULT FPTR_gArc(IntPtr hgfx, float x, float y, float rx, float ry, float start, float sweep);

			// Draws star.
			// GRAPHIN_RESULT SCFN(gStar) ( HGFX hgfx, POS x, POS y, DIM r1, DIM r2, ANGLE start, UINT rays );
			public delegate GRAPHIN_RESULT FPTR_gStar(IntPtr hgfx, float x, float y, float r1, float r2, float start, uint rays);

			// Closed polygon.
			// GRAPHIN_RESULT SCFN(gPolygon) ( HGFX hgfx, const POS* xy, UINT num_points );
			public delegate GRAPHIN_RESULT FPTR_gPolygon(IntPtr hgfx, float[] xy, uint num_points);

			// Polyline.
			// GRAPHIN_RESULT SCFN(gPolyline) ( HGFX hgfx, const POS* xy, UINT num_points );
			public delegate GRAPHIN_RESULT FPTR_gPolyline(IntPtr hgfx, float[] xy, uint num_points);
			#endregion

			#region SECTION: Path operations

			// GRAPHIN_RESULT SCFN(pathCreate) ( HPATH* path );
			public delegate GRAPHIN_RESULT FPTR_pathCreate(out IntPtr path);

			// GRAPHIN_RESULT SCFN(pathAddRef) ( HPATH path );
			public delegate GRAPHIN_RESULT FPTR_pathAddRef(IntPtr path);

			// GRAPHIN_RESULT SCFN(pathRelease) ( HPATH path );
			public delegate GRAPHIN_RESULT FPTR_pathRelease(IntPtr gfx);

			// GRAPHIN_RESULT SCFN(pathMoveTo) ( HPATH path, POS x, POS y, BOOL relative );
			public delegate GRAPHIN_RESULT FPTR_pathMoveTo(IntPtr path, float x, float y, bool relative);

			// GRAPHIN_RESULT SCFN(pathLineTo) ( HPATH path, POS x, POS y, BOOL relative );
			public delegate GRAPHIN_RESULT FPTR_pathLineTo(IntPtr path, float x, float y, bool relative);

			// GRAPHIN_RESULT SCFN(pathArcTo) ( HPATH path, POS x, POS y, ANGLE angle, DIM rx, DIM ry, BOOL is_large_arc, BOOL clockwise, BOOL relative );
			public delegate GRAPHIN_RESULT FPTR_pathArcTo(IntPtr path, float x, float y, float angle, float rx, float ry, bool is_large_arc, bool clockwise, bool relative);

			// GRAPHIN_RESULT SCFN(pathQuadraticCurveTo) ( HPATH path, POS xc, POS yc, POS x, POS y, BOOL relative );
			public delegate GRAPHIN_RESULT FPTR_pathQuadraticCurveTo(IntPtr path, float xc, float yc, float x, float y, bool relative);

			// GRAPHIN_RESULT SCFN(pathBezierCurveTo) ( HPATH path, POS xc1, POS yc1, POS xc2, POS yc2, POS x, POS y, BOOL relative );
			public delegate GRAPHIN_RESULT FPTR_pathBezierCurveTo(IntPtr path, float xc1, float yc1, float xc2, float yc2, float x, float y, bool relative);

			// GRAPHIN_RESULT SCFN(pathClosePath) ( HPATH path );
			public delegate GRAPHIN_RESULT FPTR_pathClosePath(IntPtr path);

			// GRAPHIN_RESULT SCFN(gDrawPath) ( HGFX hgfx, HPATH path, DRAW_PATH_MODE dpm );
			public delegate GRAPHIN_RESULT FPTR_gDrawPath(IntPtr gfx, IntPtr path, DRAW_PATH_MODE dpm);

			#endregion

			#region SECTION: affine tranformations

			// GRAPHIN_RESULT SCFN(gRotate) ( HGFX hgfx, ANGLE radians, POS* cx /*= 0*/, POS* cy /*= 0*/ );
			public delegate GRAPHIN_RESULT FPTR_gRotate(IntPtr hgfx, float radians, ref float cx, ref float cy);

			// GRAPHIN_RESULT SCFN(gTranslate) ( HGFX hgfx, POS cx, POS cy );
			public delegate GRAPHIN_RESULT FPTR_gTranslate(IntPtr hgfx, float cx, float cy);

			// GRAPHIN_RESULT SCFN(gScale) ( HGFX hgfx, DIM x, DIM y );
			public delegate GRAPHIN_RESULT FPTR_gScale(IntPtr hgfx, float x, float y);

			// GRAPHIN_RESULT SCFN(gSkew) ( HGFX hgfx, DIM dx, DIM dy );
			public delegate GRAPHIN_RESULT FPTR_gSkew(IntPtr hgfx, float dx, float dy);

			// all above in one shot
			// GRAPHIN_RESULT SCFN(gTransform) ( HGFX hgfx, POS m11, POS m12, POS m21, POS m22, POS dx, POS dy );
			public delegate GRAPHIN_RESULT FPTR_gTransform(IntPtr hgfx, float m11, float m12, float m21, float m22, float dx, float dy);

			#endregion// end of affine tranformations.

			#region SECTION: state save/restore

			// GRAPHIN_RESULT SCFN(gStateSave) ( HGFX hgfx );
			public delegate GRAPHIN_RESULT FPTR_gStateSave(IntPtr gfx);

			// GRAPHIN_RESULT SCFN(gStateRestore) ( HGFX hgfx );
			public delegate GRAPHIN_RESULT FPTR_gStateRestore(IntPtr gfx);

			#endregion// end of state save/restore

			#region SECTION: drawing attributes

			// set line width for subsequent drawings.
			// GRAPHIN_RESULT SCFN(gLineWidth) ( HGFX hgfx, DIM width );
			public delegate GRAPHIN_RESULT FPTR_gLineWidth(IntPtr hgfx, float width);

			// GRAPHIN_RESULT SCFN(gLineJoin) ( HGFX hgfx, SCITER_LINE_JOIN_TYPE type );
			public delegate GRAPHIN_RESULT FPTR_gLineJoin(IntPtr hgfx, SCITER_LINE_JOIN_TYPE type);

			// GRAPHIN_RESULT SCFN(gLineCap) ( HGFX hgfx, SCITER_LINE_CAP_TYPE type);
			public delegate GRAPHIN_RESULT FPTR_gLineCap(IntPtr hgfx, SCITER_LINE_CAP_TYPE type);

			//GRAPHIN_RESULT SCFN
			//      (*gNoLine ( HGFX hgfx ) { gLineWidth(hgfx,0.0); }

			// COLOR for solid lines/strokes
			// GRAPHIN_RESULT SCFN(gLineColor) ( HGFX hgfx, COLOR color);
			public delegate GRAPHIN_RESULT FPTR_gLineColor(IntPtr gfx, uint color);

			// COLOR for solid fills
			// GRAPHIN_RESULT SCFN(gFillColor) ( HGFX hgfx, COLOR color );
			public delegate GRAPHIN_RESULT FPTR_gFillColor(IntPtr gfx, uint color);

			//inline void
			//      graphics_no_fill ( HGFX hgfx ) { graphics_fill_color(hgfx, graphics_rgbt(0,0,0,0xFF)); }

			// setup parameters of linear gradient of lines.
			// GRAPHIN_RESULT SCFN(gLineGradientLinear)( HGFX hgfx, POS x1, POS y1, POS x2, POS y2, COLOR_STOP* stops, UINT nstops );
			public delegate GRAPHIN_RESULT FPTR_gLineGradientLinear(IntPtr hgfx, float x1, float y1, float x2, float y2, COLOR_STOP[] stops, uint nstops);

			// setup parameters of linear gradient of fills.
			// GRAPHIN_RESULT SCFN(gFillGradientLinear)( HGFX hgfx, POS x1, POS y1, POS x2, POS y2, COLOR_STOP* stops, UINT nstops );
			public delegate GRAPHIN_RESULT FPTR_gFillGradientLinear(IntPtr hgfx, float x1, float y1, float x2, float y2, COLOR_STOP[] stops, uint nstops);

			// setup parameters of line gradient radial fills.
			// GRAPHIN_RESULT SCFN(gLineGradientRadial)( HGFX hgfx, POS x, POS y, DIM rx, DIM ry, COLOR_STOP* stops, UINT nstops );
			public delegate GRAPHIN_RESULT FPTR_gLineGradientRadial(IntPtr hgfx, float x, float y, float rx, float ry, COLOR_STOP[] stops, uint nstops);

			// setup parameters of gradient radial fills.
			// GRAPHIN_RESULT SCFN(gFillGradientRadial)( HGFX hgfx, POS x, POS y, DIM rx, DIM ry, COLOR_STOP* stops, UINT nstops );
			public delegate GRAPHIN_RESULT FPTR_gFillGradientRadial(IntPtr hgfx, float x, float y, float rx, float ry, COLOR_STOP[] stops, uint nstops);

			// GRAPHIN_RESULT SCFN(gFillMode) ( HGFX hgfx, BOOL even_odd /* false - fill_non_zero */ );
			public delegate GRAPHIN_RESULT FPTR_gFillMode(IntPtr hgfx, bool even_odd);

			#endregion

			#region SECTION: text

			// create text layout using element's styles
			// GRAPHIN_RESULT SCFN(textCreateForElement)(HTEXT* ptext, LPCWSTR text, UINT textLength, HELEMENT he );
			public delegate GRAPHIN_RESULT FPTR_textCreateForElement(out IntPtr ptext, [MarshalAs(UnmanagedType.LPWStr)]string text, uint textLength, IntPtr he);

			// create text layout using explicit format declaration
			// GRAPHIN_RESULT SCFN(textCreate)(HTEXT* ptext, LPCWSTR text, UINT textLength, const SCITER_TEXT_FORMAT* format );
			public delegate GRAPHIN_RESULT FPTR_textCreate(out IntPtr htext, [MarshalAs(UnmanagedType.LPWStr)]string text, uint textLength, ref SCITER_TEXT_FORMAT format);

			public delegate GRAPHIN_RESULT FPTR_textAddRef(IntPtr htext);
			public delegate GRAPHIN_RESULT FPTR_textRelease(IntPtr htext);

			// GRAPHIN_RESULT SCFN(textGetMetrics)(HTEXT text, DIM* minWidth, DIM* maxWidth, DIM* height, DIM* ascent, DIM* descent, UINT* nLines);
			public delegate GRAPHIN_RESULT FPTR_textGetMetrics(IntPtr htext, out float minWidth, out float maxWidth, out float height, out float ascent, out float descent, out uint nLines);

			// GRAPHIN_RESULT SCFN(textSetBox)(HTEXT text, DIM width, DIM height);
			public delegate GRAPHIN_RESULT FPTR_textSetBox(IntPtr htext, float width, float height);

			// draw text with position (1..9 on MUMPAD) at px,py
			// Ex: gDrawText( 100,100,5) will draw text box with its center at 100,100 px
			// GRAPHIN_RESULT SCFN(gDrawText) ( HGFX hgfx, HTEXT text, POS px, POS py, UINT position );
			public delegate GRAPHIN_RESULT FPTR_gDrawText(IntPtr hgfx, IntPtr text, float px, float py, uint position);

			#endregion

			#region SECTION: image rendering

			// draws img onto the graphics surface with current transformation applied (scale, rotation).
			// GRAPHIN_RESULT SCFN(gDrawImage) ( HGFX hgfx, HIMG himg, POS x, POS y, DIM* w /*= 0*/, DIM* h /*= 0*/, UINT* ix /*= 0*/, UINT* iy /*= 0*/, UINT* iw /*= 0*/, UINT* ih, /*= 0*/ float* opacity /*= 0, if provided is in 0.0 .. 1.0*/ );
			public delegate GRAPHIN_RESULT FPTR_gDrawImage(
				IntPtr hgfx,
				IntPtr himg,
				float x,
				float y,
				IntPtr w, //ref float w /*= 0*/,
				IntPtr h, //ref float h /*= 0*/,
				IntPtr ix, //ref uint ix /*= 0*/,
				IntPtr iy, //ref uint iy /*= 0*/,
				IntPtr iw, //ref uint iw /*= 0*/,
				IntPtr ih, //ref uint ih, /*= 0*/
				IntPtr opacity);// ref float opacity /*= 0, if provided is in 0.0 .. 1.0*/ );

			#endregion

			#region SECTION: coordinate space

			// GRAPHIN_RESULT SCFN(gWorldToScreen) ( HGFX hgfx, POS* inout_x, POS* inout_y);
			public delegate GRAPHIN_RESULT FPTR_gWorldToScreen(IntPtr hgfx, ref float inout_x, ref float inout_y);

			//inline GRAPHIN_RESULT
			//      graphics_world_to_screen ( HGFX hgfx, POS* length)
			//{
			//   return graphics_world_to_screen ( hgfx, length, 0);
			//}

			// GRAPHIN_RESULT SCFN(gScreenToWorld) ( HGFX hgfx, POS* inout_x, POS* inout_y);
			public delegate GRAPHIN_RESULT FPTR_gScreenToWorld(IntPtr hgfx, ref float inout_x, ref float inout_y);

			//inline GRAPHIN_RESULT
			//      graphics_screen_to_world ( HGFX hgfx, POS* length)
			//{
			//   return graphics_screen_to_world (hgfx, length, 0);
			//}

			#endregion

			#region SECTION: clipping

			// GRAPHIN_RESULT SCFN(gPushClipBox) ( HGFX hgfx, POS x1, POS y1, POS x2, POS y2, float opacity /*=1.f*/);
			public delegate GRAPHIN_RESULT FPTR_gPushClipBox(IntPtr hgfx, float x1, float y1, float x2, float y2, float opacity /*=1.f*/);

			// GRAPHIN_RESULT SCFN(gPushClipPath) ( HGFX hgfx, HPATH hpath, float opacity /*=1.f*/);
			public delegate GRAPHIN_RESULT FPTR_gPushClipPath(IntPtr hgfx, IntPtr hpath, float opacity /*=1.f*/);

			// pop clip layer previously set by gPushClipBox or gPushClipPath
			// GRAPHIN_RESULT SCFN(gPopClip) ( HGFX hgfx);
			public delegate GRAPHIN_RESULT FPTR_gPopClip(IntPtr hgfx);

			#endregion

			#region SECTION: image painter

			public delegate GRAPHIN_RESULT FPTR_imagePaint(IntPtr himg, image_paint_function pPainter, IntPtr prm);

			#endregion

			#region SECTION: VALUE interface

			public delegate GRAPHIN_RESULT FPTR_vWrapGfx(IntPtr hgfx, out SciterXValue.VALUE toValue);

			public delegate GRAPHIN_RESULT FPTR_vWrapImage(IntPtr himg, out SciterXValue.VALUE toValue);

			public delegate GRAPHIN_RESULT FPTR_vWrapPath(IntPtr hpath, out SciterXValue.VALUE toValue);

			public delegate GRAPHIN_RESULT FPTR_vWrapText(IntPtr htext, out SciterXValue.VALUE toValue);

			public delegate GRAPHIN_RESULT FPTR_vUnWrapGfx(ref SciterXValue.VALUE fromValue, out IntPtr phgfx);

			public delegate GRAPHIN_RESULT FPTR_vUnWrapImage(ref SciterXValue.VALUE fromValue, out IntPtr phimg);

			public delegate GRAPHIN_RESULT FPTR_vUnWrapPath(ref SciterXValue.VALUE fromValue, out IntPtr phpath);

			public delegate GRAPHIN_RESULT FPTR_vUnWrapText(ref SciterXValue.VALUE fromValue, out IntPtr phtext);

			#endregion
		}
	}
}