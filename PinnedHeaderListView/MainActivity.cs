using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using PinnedHeader.Adapters;
using Xamarin.Android.OpenSource;

namespace PinnedHeader
{
	[Activity (Label = "PinnedHeaderListView", MainLauncher = true)]
	public class Activity1 : Activity
	{

		protected List<String> Contacts = new List<string>(){
			"Aquaman",
			"Atom",
			"Batman",
			"Black Widow",
			"Black Widow",
			"Black Widow",
			"Catwoman",
			"Captain America",
			"Captain Marvel",
			"Daredevil",
			"Dr. Doom",
			"Dr. Strange",
			"Flash",
			"Green Lantern",
			"Hawkeye",
			"Hulk",
			"Iron Man",
			"Martian Manhunter",
			"Nick Fury",
			"Night Wing",
			"Punisher",
			"Robin",
			"Spiderman",
			"Superman",
			"Thor", 
			"Wonder Woman"


		};



		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);


			PinnedHeaderListView list = FindViewById (Resource.Id.exampleListView) as PinnedHeaderListView;

			var adapter = new HeroesListViewAdapter (this, Contacts);

			list.SetPinnedHeaderView (LayoutInflater.Inflate(Resource.Layout.HeroListItemHeader,list,false));
			list.DividerHeight = 0;
			list.FastScrollEnabled = true;
			list.SetAdapter (adapter);

			list.SetOnScrollListener (adapter);

		

		}
	}
}


