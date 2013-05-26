/*
 * This Class was adapted from The Android Open Source Project:
 * http://code.google.com/p/android-playground/source/browse/trunk/PinnedHeaderListViewSample/src/net/peterkuterna/android/apps/pinnedheader/PinnedHeaderListView.java?r=2
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
/**
 * A ListView that maintains a header pinned at the top of the list. The
 * pinned header can be pushed up and dissolved as needed.
 */
namespace Xamarin.Android.OpenSource
{
	public enum PinnedHeaderState
	{		PINNED_HEADER_GONE
, 		PINNED_HEADER_VISIBLE
, 		PINNED_HEADER_PUSHED_UP
}
	;
	/*
	 * Adapter interface class. The list adapter must implement this interface
	 */
	public interface IPinnedHeaderAdapter
	{



		PinnedHeaderState GetHeaderState(int position);
		/**
         * Configures the pinned header view to match the first visible list item.
         *
         * @param header pinned header view.
         * @param position position of the first visible list item.
         * @param alpha fading of the header view, between 0 and 255.
         */
		void ConfigurePinnedHeader (View header, int position, int alpha);
	}

	public class PinnedHeaderListView : ListView
	{

		private static int MAX_ALPHA = 255;
		private IPinnedHeaderAdapter mAdapter;
		private View mHeaderView;
		private bool mHeaderViewVisible;
		private int mHeaderViewWidth;
		private int mHeaderViewHeight;

		public PinnedHeaderListView (Context context) :
			base (context)
		{
			Initialize ();
		}

		public PinnedHeaderListView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		public PinnedHeaderListView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}

		public void SetPinnedHeaderView (View view)
		{
			mHeaderView = view;

			// Disable vertical fading when the pinned header is present
			// TODO change ListView to allow separate measures for top and bottom fading edge;
			// in this particular case we would like to disable the top, but not the bottom edge.
			if (mHeaderView != null) {
				SetFadingEdgeLength (0);
			}
			RequestLayout ();
		}

		public override void SetAdapter (IListAdapter adapter)
		{
			base.SetAdapter (adapter);
			mAdapter = (IPinnedHeaderAdapter)adapter;
		}

		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
			if (mHeaderView != null) {
				MeasureChild (mHeaderView, widthMeasureSpec, heightMeasureSpec);
				mHeaderViewWidth = mHeaderView.MeasuredWidth;
				mHeaderViewHeight = mHeaderView.MeasuredHeight;
			}
		}

		protected override void OnLayout (bool changed, int left, int top, int right, int bottom)
		{
			base.OnLayout (changed, left, top, right, bottom);
			if (mHeaderView != null) {
				mHeaderView.Layout (0, 0, mHeaderViewWidth, mHeaderViewHeight);
				configureHeaderView (FirstVisiblePosition);
			}
		}

		public void configureHeaderView (int position)
		{
			if (mHeaderView == null) {
				return;
			}

			PinnedHeaderState state = mAdapter.GetHeaderState(position);
			switch (state) {
			case PinnedHeaderState.PINNED_HEADER_GONE:
				{
					mHeaderViewVisible = false;
					break;
				}

			case PinnedHeaderState.PINNED_HEADER_VISIBLE:
				{
					mAdapter.ConfigurePinnedHeader (mHeaderView, position, MAX_ALPHA);
					if (mHeaderView.Top != 0) {
						mHeaderView.Layout (0, 0, mHeaderViewWidth, mHeaderViewHeight);
					}
					mHeaderViewVisible = true;
					break;
				}

			case PinnedHeaderState.PINNED_HEADER_PUSHED_UP:
				{
					View firstView = GetChildAt (0);
					int bottom = firstView.Bottom;
					int itemHeight = firstView.Height;
					int headerHeight = mHeaderView.Height;
					int y;
					int alpha;
					if (bottom < headerHeight) {
						y = (bottom - headerHeight);
						alpha = MAX_ALPHA * (headerHeight + y) / headerHeight;
					} else {
						y = 0;
						alpha = MAX_ALPHA;
					}
					mAdapter.ConfigurePinnedHeader (mHeaderView, position, alpha);
					if (mHeaderView.Top != y) {
						mHeaderView.Layout (0, y, mHeaderViewWidth, mHeaderViewHeight + y);
					}
					mHeaderViewVisible = true;
					break;
				}
			}
		}
		protected override void DispatchDraw (Canvas canvas)
		{
			base.DispatchDraw (canvas);
			if (mHeaderViewVisible) {
				DrawChild(canvas, mHeaderView, DrawingTime);
			}
		}
	}
}

