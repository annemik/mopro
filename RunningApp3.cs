using Android.Widget;
using Android.OS;
using Android.App;
using Android.Views;
using Android.Graphics;
using Android.Content;
using Android.Content.PM;
using System;
using Android;
using Android.Hardware;   // vanwege SensorManager
using Android.Locations;
using Android.Runtime;

namespace App3
{
    [Activity(Label = "App3", MainLauncher = true)]
    public class MainActivity : Activity
    {
        TextView RunningApp;
        Button Startknop; Button Stopknop; Button Centreerknop;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ARView kaart;
            kaart = new ARView(this);
            //Titel
            RunningApp = new TextView(this);
            RunningApp.TextSize = 40;
            RunningApp.Text = "Running App! ";
            RunningApp.SetTextColor(Color.Blue);

            //Start + stop knop
            Startknop = new Button(this); // bool maken 'aan het verzamelen' default false, if toevoegen zodat hij alleen verzamelt als je op start drukt
            Startknop.TextSize = 20;
            Startknop.Text = "Start";
            Startknop.SetTextColor(Color.Pink);
            Stopknop = new Button(this);
            Stopknop.TextSize = 20;
            Stopknop.Text = "Stop";
            Stopknop.SetTextColor(Color.Pink);
            Centreerknop= new Button(this); //centrum=huidig, this.invalidate();
            Centreerknop.TextSize = 20;
            Centreerknop.Text = "Centreer";
            Centreerknop.SetTextColor(Color.Pink);

            //De knoppen
            LinearLayout knoppen;
            knoppen = new LinearLayout(this);
            knoppen.AddView(Startknop);
            knoppen.AddView(Stopknop);
            knoppen.AddView(Centreerknop);
            knoppen.Orientation = Orientation.Horizontal;

            //Overzicht
            LinearLayout Overzicht;
            Overzicht = new LinearLayout(this);
            Overzicht.AddView(RunningApp);
            Overzicht.AddView(knoppen);
            Overzicht.AddView(kaart);
            Overzicht.Orientation = Orientation.Vertical;

            this.SetContentView(Overzicht);
        }

        public class ARView : View, ILocationListener
        {

            Bitmap geo; float Schaal; PointF centrum = new PointF(138300, 454300); float midx; float midy;
            bool pinching = false; 
            

            public ARView(Context context) : base(context)
            {
                BitmapFactory.Options opt = new BitmapFactory.Options();
                opt.InScaled = false;
                geo = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.Kaart, opt);
                this.Touch += RaakAan;

                LocationManager lm = (LocationManager)context.GetSystemService(Context.LocationService);
                Criteria crit = new Criteria();
                crit.Accuracy = Accuracy.Fine;
                string lp = lm.GetBestProvider(crit, true);
                lm.RequestLocationUpdates(lp, 0, 0, this);

                Schaal = 1;
                maximaal = new PointF();
                minimaal = new PointF();
            }

            static float Afstand(PointF p1, PointF p2)
            {
                float a = p1.X - p2.X;
                float b = p1.Y - p2.Y;
                return (float)Math.Sqrt(a * a + b * b);
            }
            private PointF start1;
            private PointF start2;
            private PointF huidig1;
            private PointF huidig2;
            private PointF dragstartpunt; //in schermpixels
            private PointF maximaal;
            private PointF minimaal;
            private float oudeSchaal;

            public void RaakAan(object o, TouchEventArgs tea)
            {
                huidig1 = new PointF(tea.Event.GetX(0), tea.Event.GetY(0));
                if (tea.Event.PointerCount == 2)
                {
                    pinching = true;
                    Console.WriteLine("wel aan het pinchen");
                    huidig2 = new PointF(tea.Event.GetX(1), tea.Event.GetY(1));
                    if (tea.Event.Action == MotionEventActions.Pointer2Down)
                    {
                        start1 = huidig1;
                        start2 = huidig2;
                        oudeSchaal = Schaal;
                        
                    }
                                        
                    float oud = Afstand(start1, start2);
                    float nieuw = Afstand(huidig1, huidig2);

                    if (oud != 0 && nieuw != 0)
                    {
                        float factor = nieuw / oud;
                        this.Schaal = (float)Math.Max(Math.Min(oudeSchaal * factor, 2.5), 0.32);
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             this.Invalidate();
                    }
                }
                
                else if (!pinching)
                {
                    
                    Console.WriteLine("niet aan het pinchen");
                    if (tea.Event.Action == MotionEventActions.Down)
                    {
                        dragstartpunt = new PointF(tea.Event.GetX(), tea.Event.GetY());
                    }

                    else if (dragstartpunt!=null)
                    {
                        
                        float x = tea.Event.GetX();
                        float sx = x - dragstartpunt.X;
                        float px = sx / Schaal;
                        float ax = px / 0.4f;
                        centrum.X -= ax;
                        
                        float y = tea.Event.GetY();
                        float sy = y - dragstartpunt.Y;
                        float py = sy / Schaal;
                        float ay = -py / 0.4f;
                        centrum.Y -= ay;

                        dragstartpunt = new PointF(tea.Event.GetX(), tea.Event.GetY());
                        
                        minimaal.X = (136500);
                        maximaal.X = (141500);
                        minimaal.Y = (453500);
                        maximaal.Y = (457500); 

                        if (!(centrum.X > minimaal.X && centrum.X < maximaal.X))
                        {
                            if (!(centrum.X > minimaal.X))
                                centrum.X = minimaal.X;

                            if (!(centrum.X < maximaal.X))
                                centrum.X = maximaal.X;
                        }

                        if (!(centrum.Y > minimaal.Y && centrum.Y < maximaal.Y))
                        {
                            if (!(centrum.Y > minimaal.Y))
                                centrum.Y = minimaal.Y;

                            if (!(centrum.Y < maximaal.Y))
                                centrum.Y = maximaal.Y;
                        }
                    }
                }

                if (tea.Event.Action == MotionEventActions.Up)
                {
                    pinching = false;
                }

                this.Invalidate();
            }

            PointF huidig = null; // in meters!
            string info;

            public void OnLocationChanged(Location loc)
            {
                huidig = Kaart.Projectie.Geo2RD(loc);
                double noord = loc.Latitude;
                double oost = loc.Longitude;
                info = $"{noord} graden noorderbreedte, {oost} graden oosterlengte";
                this.Invalidate();
            }

            protected override void OnDraw(Canvas canvas)
            {
                base.OnDraw(canvas);
                Paint verf = new Paint();
                verf.Color = Color.Magenta;
                //verf.TextSize = 20;
                midx = (centrum.X - 136000) * 0.4f;
                midy = -(centrum.Y - 458000) * 0.4f;

                // Schaal = this.Width / geo.Width;
                Matrix mat = new Matrix();
                mat.PostTranslate(-midx, -midy);
                mat.PostScale(Schaal, Schaal);

                mat.PostTranslate(canvas.Width / 2, canvas.Height / 2);

                canvas.DrawBitmap(geo, mat, verf);

                if (huidig != null)
                {
                    float ax = huidig.X - centrum.X;
                    float px = ax * 0.4f;
                    float sx = px * Schaal;
                    float x = this.Width / 2 + sx;

                    float ay = huidig.Y - centrum.Y;
                    float py = ay * 0.4f;
                    float sy = py * Schaal;
                    float y = this.Width / 2 + -sy;
                    
                    canvas.DrawCircle(x, y, 20, verf);
                    canvas.DrawText($"{info}", 100, 100, verf);
                }

                /*canvas.DrawText($"Min: {minimaal.X} br: {minimaal.Y}", 100, 100, verf);
                canvas.DrawText($"Centrum: {centrum.X} br:{centrum.Y}", 100, 200, verf);*/
            }

            public void OnProviderDisabled(string provider)
            {
                throw new NotImplementedException();
            }

            public void OnProviderEnabled(string provider)
            {
                throw new NotImplementedException();
            }

            public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
            {
                throw new NotImplementedException();
            }
        }
    }
}
