using Android.App;
using Android.Widget;
using Android.OS;
using AppPlay.Models;
using System.Collections.Generic;
using Android;
using Android.Content.PM;
using Android.Content;
using Android.Provider;
using Android.Database;  
using System;


namespace AppPlay
{
    [Activity(Label = "AppPlay", MainLauncher = true)] 
    public class MainActivity : Activity
    {
        private int TRACK_Column, _ID_Column, DATA_Column, YEAR_Column;
        private int DURATION_Column, ALBUM_ID_Column, ALBUM_Column, ARTIST_Column;
        List<Datos> items;
        ListView listData;
        AudioAdapter audioAdapter;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            // haciendo referencia del control ListView1, esta en el (Activity) Main.axml
            listData = FindViewById<ListView>(Resource.Id.listView1);
            items = new List<Datos>();
            
            //para validar la version de Android
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                //string permission = Manifest.Permission.ReadExternalStorage;
                //string permission2 = Manifest.Permission.WriteExternalStorage;

                if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted
                    || CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted) 
                {   
                    RequestPermissions(new string[] {
                    Manifest.Permission.WriteExternalStorage,
                    Manifest.Permission.ReadExternalStorage
                    },1);
                    
                }

            }
            listData.ItemClick += (object sender, AdapterView.ItemClickEventArgs args) => listView_ItemClick(sender, args);
            audioCursor();   
        }
        //metodo del evento 'listData.ItemClick' para seleccionar una canciony traer todos sus datos
        private void listView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = this.audioAdapter.GetItemAtPosition(e.Position);
            String urlAlbum = item.ArtistAlbum;
            String urlAudio = item.ArrPath; //path de la musica en el movil
            var intent = new Intent(this, typeof(PlayerActivity));
            intent.PutExtra("urlAlbum", urlAlbum);
            intent.PutExtra("urlAudio", urlAudio);
            this.StartActivity(intent);
        }

        private void audioCursor()
        {
            try
            {
                string[] information = {
                    MediaStore.Audio.Media.InterfaceConsts.Id,
                    MediaStore.Audio.Media.InterfaceConsts.Data,
                    MediaStore.Audio.Media.InterfaceConsts.Track,
                    MediaStore.Audio.Media.InterfaceConsts.Year,
                    MediaStore.Audio.Media.InterfaceConsts.Duration,
                    MediaStore.Audio.Media.InterfaceConsts.AlbumId,
                    MediaStore.Audio.Media.InterfaceConsts.Album,
                    MediaStore.Audio.Media.InterfaceConsts.AlbumKey,
                    MediaStore.Audio.Media.InterfaceConsts.Title,
                    MediaStore.Audio.Media.InterfaceConsts.TitleKey,
                    MediaStore.Audio.Media.InterfaceConsts.ArtistId,
                    MediaStore.Audio.Media.InterfaceConsts.Artist
                };

                string orderBy = MediaStore.Audio.Media.InterfaceConsts.Id;

                
                //traer todas las listas de audio dentro del dispositivo movil consultando la memoria del movil
                ICursor audioCursor = ContentResolver.Query(MediaStore.Audio.Media.ExternalContentUri,
                                        information, null, null, orderBy);

                #region nota 2
                //la informacion que me retorna la consulta
                #endregion

                _ID_Column = audioCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Id);
                DATA_Column = audioCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Data);
                YEAR_Column = audioCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Year);
                DURATION_Column = audioCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Duration);
                ALBUM_ID_Column = audioCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.AlbumId);
                ALBUM_Column = audioCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Album);
                TRACK_Column = audioCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Title);
                ARTIST_Column = audioCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Artist);
                //var c = true;
                while (audioCursor.MoveToNext())
                {
                    var audioTitle = audioCursor.GetString(TRACK_Column);
                    var artist = audioCursor.GetString(ARTIST_Column);
                    var time = audioCursor.GetString(DURATION_Column);
                    var time2 = convertDuration(Convert.ToInt64(time));
                    var arrPath = audioCursor.GetString(DATA_Column);
                    items.Add(new Datos() { Title = audioTitle, Artist = artist, Time = time2, ArrPath=arrPath });

                    //var audioTitle = "El Malo";
                    //var artist = "Aventura";
                    //var time = 3600000;
                    //var time2 = convertDuration(Convert.ToInt64(time));
                    //items.Add(new Datos() { Title = audioTitle, Artist = artist, Time = time2 });

                    //audioTitle = "Voy despues";
                    //artist = "El Nene La Amenaza";
                    //time = 90000;
                    //time2 = convertDuration(Convert.ToInt64(time));
                    //items.Add(new Datos() { Title = audioTitle, Artist = artist, Time = time2 });

                    //c = false;
                }   
                audioCursor.Close();

                listData.Adapter = audioAdapter = new AudioAdapter(this, items);
        }
        catch (Exception ex)
        {                
            Toast.MakeText(this, "Mensaje de error: " + ex.Message, ToastLength.Long).Show();
            Toast.MakeText(this, "Reinicie la aplicacion para una implementacion satisfactoria", ToastLength.Long).Show();

                //para cerrar la aplicacion
            //this.FinishAffinity();  //Cerrando todas las tareas
            //Finish();         //Ya cuando esta lista para cerrar
            //Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }

    }

        private string convertDuration(long duration) //miliseconds
        {
            string outTime = null;  
            try
            {
                long hours = 0, minutes = 0, seconds = 0;
                if (duration>= 3600000) 
                {
                    hours = duration / 3600000;
                    outTime =outTime+ hours.ToString()+ ":";
                }                                          
                if ((duration-(hours* 3600000))>= 60000)
                {
                    minutes = (duration - (hours * 3600000))/ 60000;
                }
                if (hours>0)
                {
                    outTime = outTime + minutes.ToString("0#") + ":";
                }
                else
                {
                    outTime= outTime + minutes.ToString() + ":";
                }
                
                if ((duration - (hours * 3600000) - (minutes * 60000)) >= 1000)
                {
                    seconds = (duration - (hours * 3600000) - (minutes * 60000)) / 1000;
                }
                outTime = outTime + seconds.ToString("0#");
            }
            catch (Exception e)
            {
                string strError = e.StackTrace;
                outTime = "0:00";
            }

            return outTime;
        }
    }
}

