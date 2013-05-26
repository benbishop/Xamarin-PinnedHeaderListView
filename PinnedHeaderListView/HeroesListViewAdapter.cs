using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Android.OpenSource;


namespace PinnedHeader.Adapters
{
	class HeroesListViewAdapter:BaseAdapter<string>, ISectionIndexer, IPinnedHeaderAdapter, Android.Widget.AbsListView.IOnScrollListener
	{
		List<string> heroNames;

		Activity context;

		protected Dictionary<string, int> AlphaIndex;
		protected string[] Sections;
		protected Java.Lang.Object[] SectionsObjects;

		public HeroesListViewAdapter(Activity context, List<string> heroes):base(){

			this.context = context;

			this.heroNames = heroes;

			InitAlphaIndex ();

		}

		void InitAlphaIndex ()
		{
			AlphaIndex = new Dictionary<string, int>();
			for (int i = 0; i < heroNames.Count; i++) { // loop through items
				var key = heroNames[i][0].ToString();
				if (!AlphaIndex.ContainsKey(key))
					AlphaIndex.Add(key, i); // add each 'new' letter to the index
			}
			Sections = new string[AlphaIndex.Keys.Count];
			AlphaIndex.Keys.CopyTo(Sections, 0); // convert letters list to string[]
			// Interface requires a Java.Lang.Object[], so we create one here
			SectionsObjects = new Java.Lang.Object[Sections.Length];
			for (int i = 0; i < Sections.Length; i++) {
				SectionsObjects[i] = new Java.Lang.String(Sections[i]);
			}
		}

		#region IPinnedHeaderAdapter implementation

		public void ConfigurePinnedHeader (View header, int position, int alpha)
		{

			var headerTextView = header as TextView;
			int section = GetSectionForPosition(position - 1);
			String title = (String) GetSections()[section];

			headerTextView.Text =(title);

		}

		public PinnedHeaderState GetHeaderState(int position)
		{
			if (AlphaIndex == null || this.Count == 0) {
				return PinnedHeaderState.PINNED_HEADER_GONE;
			}

			if (position < 0) {
				return PinnedHeaderState.PINNED_HEADER_GONE;
			}

			// The header should get pushed up if the top item shown
			// is the last item in a section for a particular letter.
			int section = GetSectionForPosition(position);
			int nextSectionPosition = GetPositionForSection(section + 1);

			if (nextSectionPosition != -1 && position == nextSectionPosition - 1) {
				return PinnedHeaderState.PINNED_HEADER_PUSHED_UP;
			}

			return PinnedHeaderState.PINNED_HEADER_VISIBLE;
		}

		#endregion

		#region IOnScrollListener implementation

		public void OnScroll (AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
		{
			if (view is PinnedHeaderListView) {
				((PinnedHeaderListView) view).configureHeaderView(firstVisibleItem);
			}     
		}

		public void OnScrollStateChanged (AbsListView view, ScrollState scrollState)
		{

		}

		#endregion

		#region ISectionIndexer implementation
		public int GetPositionForSection (int section)
		{
			return AlphaIndex[Sections[section]];
		}
		public int GetSectionForPosition (int position)
		{
			int prevSection = 0;
			for (int i = 0; i < Sections.Length; i++) {
				if (GetPositionForSection(i) > position && prevSection <= position) {
					prevSection = i; 
					break;
				}
			}
			return prevSection;
		}
		public Java.Lang.Object[] GetSections ()
		{
			return SectionsObjects;
		}
		#endregion


		#region implemented abstract members of BaseAdapter
		public override long GetItemId (int position)
		{
			return position;
		}
		public override View GetView (int position, View convertView, ViewGroup parent)
		{


			View view = convertView; // re-use an existing view, if one is available
			if (view == null) // otherwise create a new one
				view = context.LayoutInflater.Inflate(Resource.Layout.HeroListItem, null);

			if(position == 0 || GetSectionForPosition(position -1 ) != GetSectionForPosition(position)){
				var headerTextView = view.FindViewById (Resource.Id.heroListHeader) as TextView;
				headerTextView.Text = Sections [GetSectionForPosition(position -1)];
				headerTextView.Visibility = ViewStates.Visible;
			}else{
				view.FindViewById (Resource.Id.heroListHeader).Visibility = ViewStates.Gone;
			}


			var textView = view.FindViewById<TextView> (Resource.Id.heroNameText);
			textView.Text = heroNames[position];
			return view;
		}
		public override int Count {
			get {
				return heroNames.Count;
			}
		}

		public override string this [int position] {
			get {
				return heroNames [position];
			}
		}
		#endregion
	}
}

