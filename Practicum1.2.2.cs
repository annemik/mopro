using Android.App;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using System;

namespace App2
{
    [Activity(Label = "Practicum1.2.2", MainLauncher = true)]
    public class MainActivity : Activity
    {
        Button knop; EditText dag; EditText maand; EditText jaar; TextView leeftijd; TextView vandaag; TextView verjaardag;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            TextView uitleg;
            uitleg = new TextView(this);
            uitleg.Text = "Vul hier je geboortedag, -maand en -jaar in (DD/MM/JJJJ).";
            
            // EditText dag;
            dag = new EditText(this);
            dag.Text = "DD";
                    
            // EditText maand;
            maand = new EditText(this);
            maand.Text = "MM";
           
            // EditText jaar;
            jaar = new EditText(this);
            jaar.Text = "JJJJ";
           
            knop = new Button(this);
            knop.Text = "Bereken je leeftijd!";
            knop.Click += klik;

            // TextView leeftijd;
            leeftijd = new TextView(this);
            leeftijd.Text = "";

            // TextView vandaag;
            vandaag = new TextView(this);
            vandaag.Text = "";

            // TextView verjaardag;
            verjaardag = new TextView(this);
            verjaardag.Text = "";

            LinearLayout geboortedatum;
            geboortedatum = new LinearLayout(this);
            geboortedatum.Orientation = Orientation.Horizontal;

            geboortedatum.AddView(dag);
            geboortedatum.AddView(maand);
            geboortedatum.AddView(jaar);
            
            LinearLayout hoofdstapel;
            hoofdstapel = new LinearLayout(this);
            hoofdstapel.Orientation = Orientation.Vertical;

            hoofdstapel.AddView(uitleg);
            hoofdstapel.AddView(geboortedatum);
            hoofdstapel.AddView(knop);
            hoofdstapel.AddView(vandaag);
            hoofdstapel.AddView(verjaardag);
            hoofdstapel.AddView(leeftijd);
            SetContentView(hoofdstapel);

        }

        public void klik(object o, EventArgs ea)
        {
            int dagwaarde = int.Parse(dag.Text);
            int maandwaarde = int.Parse(maand.Text);
            int jaarwaarde = int.Parse(jaar.Text);

            DateTime today = DateTime.Now;
            DateTime birthday = new DateTime(jaarwaarde, maandwaarde, dagwaarde);

            TimeSpan age = today - birthday;

            vandaag.Text = "Het is vandaag " + today + ".";

            verjaardag.Text = "Je bent geboren op " + birthday + ".";

            leeftijd.Text = "Je bent " + age.Days + " dagen oud.";
                      
        }
          
    }
}

