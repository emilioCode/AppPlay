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
using Android.Media;

namespace AppPlay
{
    [Activity(Label = "PlayerActivity")]
    public class PlayerActivity : Activity
    {
        MediaPlayer _player;
        MediaController mediaController;
        LinearLayout linearLayout;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Player);
            linearLayout = FindViewById<LinearLayout>(Resource.Id.linearLayoutPlayer);
            String urlAlbum = this.Intent.GetStringExtra("urlAlbum");
            String urlAudio = this.Intent.GetStringExtra("urlAudio");

            _player = new MediaPlayer();
            mediaController = new MediaController(this, true);
            playAudio(urlAudio);
        }

        private void playAudio(String urlAudio)
        {
            _player.SetDataSource(urlAudio);
            _player.Prepare();
            _player.Start();
        }
    }
}