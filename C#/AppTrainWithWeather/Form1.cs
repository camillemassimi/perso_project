using System.Drawing; 
using System.Net;
using System.Net.Http; 
using System.Threading.Tasks; 
using Newtonsoft.Json; 
using System.Windows.Forms;


/*
Cette application utilise l'API OpenWeather pour aider les utilisateurs à planifier leurs séances d'entraînement en fonction des conditions météorologiques. 
En fournissant simplement des informations telles que le lieu d'entraînement, la fréquence, le nombre d'heures et le créneau horaire, 
l'application affiche les prévisions météorologiques et recommande les meilleurs jours pour s’entraîner.

Liens utiles :
- API 5 jours prévisions : https://www.youtube.com/watch?v=zW26MZBV-9o
- palette couleur : https://coolors.co/palette/283618-606c38-fefae0-dda15e-bc6c25
- apiKey = "066c9eb387d6ea74278f60b8aeb7ecc7"
*/


namespace AppTrainWithWeather
{
    public class CityData
    {
        public string name {get; set; }  = string.Empty; //nom ville
    }

    public class MainData
    {
        public double temp {get; set; } //temperature 
    }

    public class WeatherData
    {
        public string main {get; set; } = string.Empty; //Rain, Clouds...

        public string description {get; set; } = string.Empty; //broken clouds...
        public string icon {get; set; } = string.Empty; //ID de l'icon associé
    }

    public class AllListData
    {
        public MainData main {get; set; }
        public List<WeatherData> weather {get; set; }
        public DateTime dt_txt {get; set; } //jour et heure
    }

    public class StructureJson
    {
        public CityData city {get; set; } //mettre city pour "city":{"id":2988507,"name":"Paris","coord":{"lat":...
        public List<AllListData> list {get; set; } //mettre list pour "list":[{"dt":1705179600,"main":{"temp":271.71...
    }

    // private string apiKey = "066c9eb387d6ea74278f60b8aeb7ecc7";
    // string apiUrl = $"api.openweathermap.org/data/2.5/forecast?q={city name}&appid={API key}";

    public partial class Form1 : Form
    {

        //---------------- déclaration des objets de texte afficher sur la fenetre ----------------------
        private Label lblWelcome = new Label(); //message d'accueil 

        private Label detailConsigne= new Label(); //message pour expliquer les consignes

        private TextBox choixVille = new TextBox(); //récupérer le choix de la ville
        private Label detailChoixVille = new Label(); //message qui dit de choisir une ville


        private TextBox choixNbHeure = new TextBox(); //récupérer le choix du nb d'heure souhaite
        private Label detailchoixNbHeure = new Label(); //message qui dit de choisir nb d'heure souhaite


        private TextBox choixFrequence = new TextBox(); //récupérer le choix de la frequence souhaite
        private Label detailchoixFrequence = new Label(); //message qui dit de choisir la frequence souhaite


        private TextBox choixDispo = new TextBox(); //récupérer le choix du moment dispo pour s'entrainer
        private Label detailchoixDispo = new Label(); //message qui dit de choisir moment dispo pour s'entrainer

        //-----------------------déclaration de variale -----------------------
        private string villeChoisie = ""; //ville choisie par utilisateur
        private int nbHeureTotale = 0;//nb d'heure d'entrainement sur 5 jours
        private int frequenceEntrainement = 0;//frequence d'entrainmeent sur 5 jours
        private string dispoSemaine = ""; //choisir le moment où s'entrainer
        int momentDispoCorress = 0;


        //---------------- déclaration de l'URL de l'API et de la clef ----------------------
        private string apiKey = "ce76692a5a4496068031e748f9f50c65";
        private string apiUrl = "http://api.openweathermap.org/data/2.5/forecast?q={city name}&appid={API key}";

        //-------------------déclaration variable pour dictionnaire ------------------------
        // Dictionary<(string, int), List<object>> dicoPrevision = new Dictionary<(string, int), List<object>>(); //key = date + heure

        public class MyPrevisions
        {
            public string Date { get; set; }
            public int Hour { get; set; }
            public int Temperature { get; set; }
            public string Weather { get; set; }
            public string WeatherDetail { get; set; }
            public string Icon { get; set; }
            public string IconUrl { get; set; }
        }
        List<MyPrevisions> myListPrevisions = new List<MyPrevisions>(); //liste de toutes les previsions ranger comme dans MyPrevisions

        List<string> allDate = new List<string>(); //liste de toutes les dates
        List<string> allDateBest = new List<string>(); //liste de toutes les dates des meilleures jours


        public class MyBestDay
        {
            public string Date { get; set; }
            public int Hour { get; set; }
            public int Temperature { get; set; }
            public string Weather { get; set; }
            public string WeatherDetail { get; set; }
            public string Icon { get; set; }
            public string IconUrl { get; set; }
            public double ProcheTemperatureOptimale { get; set; } //pour recupere en valeur absolue la distance par rapport à 20°C
        }
        List<MyBestDay> myListBestDay = new List<MyBestDay>(); //liste de toutes les dates où s'entrainer


        //------------------------déclaration variables grille ou liste pour affichage
        DataGridView dataGridViewDates = new DataGridView();

        //-----------------------autres
        private List<MyBestDay> xMeilleuresDates = new List<MyBestDay>(); //pour récupérer les meilleures dates où s'entrainer


        public Form1()
        {
            InitializeComponent();
            WindowsBackground();
            WelcomeText();

            SaisieChoixVille();
            AddControls();
        }
        private void WindowsBackground()
        {
            this.BackColor = System.Drawing.ColorTranslator.FromHtml("#fefae0");
        }

        private void WelcomeText()
        {
            lblWelcome = new Label();
            lblWelcome.Text = "Prêts à courir ?\nDéterminons ensemble les meilleurs jours pour s'entraîner ! ";
            lblWelcome.Font = new Font("Arial", 20, FontStyle.Bold);
            lblWelcome.ForeColor = System.Drawing.ColorTranslator.FromHtml("#bc6c25");
            lblWelcome.TextAlign = ContentAlignment.MiddleCenter;

            // Ajuster la boîte du texte selon la taille de la police
            SizeF textSize = TextRenderer.MeasureText(lblWelcome.Text, lblWelcome.Font);
            lblWelcome.Size = new Size((int)textSize.Width, (int)textSize.Height);
            lblWelcome.Left = (this.ClientSize.Width - lblWelcome.Width) / 2; // Centrer en haut au milieu la boîte de texte
        }

        private void SaisieChoixVille()
        {
            //-----------------------BOX de texte pour indiquer qu'il faut choisir une ville
            detailConsigne = new Label();
            detailConsigne.Text = "Afin de déterminer les meilleurs jours pour t'entraîner, nous avons besoin de quelques informations.\nCommence par indiquer le 📍 lieu où tu prévois de t'entraîner cette semaine, nous pourrons ainsi récupérer les prévisions météos correspondantes. Ensuite, précise le ⌛ nombre d'heures que tu souhaites\nconsacrer à tes séances et la 🔁 fréquence hebdomadaire. Pour finir choisis le 📅 créneau de la journée où tu préfères t'entraîner.";
            detailConsigne.Font = new Font("Arial", 9);
            detailConsigne.ForeColor = System.Drawing.ColorTranslator.FromHtml("#bc6c25");
            SizeF textSize0 = TextRenderer.MeasureText(detailConsigne.Text, detailConsigne.Font);
            detailConsigne.Size = new Size((int)textSize0.Width, (int)textSize0.Height);
            detailConsigne.Location = new System.Drawing.Point(50, 110);


            //-----------------------BOX de texte pour indiquer qu'il faut choisir une ville
            detailChoixVille = new Label();
            detailChoixVille.Text = "📍 Lieu :";
            detailChoixVille.Font = new Font("Arial", 8, FontStyle.Bold);
            detailChoixVille.ForeColor = System.Drawing.ColorTranslator.FromHtml("#bc6c25");
            SizeF textSize = TextRenderer.MeasureText(detailChoixVille.Text, detailChoixVille.Font);
            detailChoixVille.Size = new Size((int)textSize.Width, (int)textSize.Height);
            detailChoixVille.Location = new System.Drawing.Point(50, 200);

            //BOX de saisie du choix de la ville
            choixVille = new TextBox();
            choixVille.Size = new System.Drawing.Size(100, 20);
            choixVille.Location = new System.Drawing.Point((int)textSize.Width + 55, 195);
            choixVille.Text = "Paris";
            choixVille.BorderStyle = BorderStyle.None; //pas de bordure
            choixVille.ForeColor = System.Drawing.ColorTranslator.FromHtml("#bc6c25");


            //-----------------------BOX de texte pour demander nb heure entrainement
            detailchoixNbHeure = new Label();
            detailchoixNbHeure.Text = "⌛ Nombre d'heures :";
            detailchoixNbHeure.Font = new Font("Arial", 8, FontStyle.Bold);
            detailchoixNbHeure.ForeColor = System.Drawing.ColorTranslator.FromHtml("#bc6c25");
            SizeF textSize2 = TextRenderer.MeasureText(detailchoixNbHeure.Text, detailchoixNbHeure.Font);
            detailchoixNbHeure.Size = new Size((int)textSize2.Width, (int)textSize2.Height);
            detailchoixNbHeure.Location = new System.Drawing.Point((int)textSize.Width + 55 + 100 + 50, 200);

            //BOX de saisie du choix nb heure entrainement
            choixNbHeure = new TextBox();
            choixNbHeure.Size = new System.Drawing.Size(100, 20);
            choixNbHeure.Location = new System.Drawing.Point((int)textSize.Width + 55 + 100 + 50 + (int)textSize2.Width + 5, 195);
            choixNbHeure.Text = "4";
            choixNbHeure.BorderStyle = BorderStyle.None; //pas de bordure
            choixNbHeure.ForeColor = System.Drawing.ColorTranslator.FromHtml("#bc6c25");


            //-------------------------BOX de texte pour demander nb frequence entrainement
            detailchoixFrequence = new Label();
            detailchoixFrequence.Text = "🔄 Fréquence :";
            detailchoixFrequence.Font = new Font("Arial", 8, FontStyle.Bold);
            detailchoixFrequence.ForeColor = System.Drawing.ColorTranslator.FromHtml("#bc6c25");
            SizeF textSize3 = TextRenderer.MeasureText(detailchoixFrequence.Text, detailchoixFrequence.Font);
            detailchoixFrequence.Size = new Size((int)textSize3.Width, (int)textSize3.Height);
            detailchoixFrequence.Location = new System.Drawing.Point((int)textSize.Width + 55 + 100 + 50 + (int)textSize2.Width + 5 + 100 + 50, 200);

            //BOX de saisie du choix nb frequence entrainement
            choixFrequence = new TextBox();
            choixFrequence.Size = new System.Drawing.Size(100, 20);
            choixFrequence.Location = new System.Drawing.Point((int)textSize.Width + 55 + 100 + 50 + (int)textSize2.Width + 5 + 100 + 50 + (int)textSize3.Width + 5, 195);
            choixFrequence.Text = "2";
            choixFrequence.BorderStyle = BorderStyle.None; //pas de bordure
            choixFrequence.ForeColor = System.Drawing.ColorTranslator.FromHtml("#bc6c25");


            //-------------------------BOX de texte pour demander dispo en semaine
            detailchoixDispo = new Label();
            detailchoixDispo.Text = "📅 Crénaux :";
            detailchoixDispo.Font = new Font("Arial", 8, FontStyle.Bold);
            detailchoixDispo.ForeColor = System.Drawing.ColorTranslator.FromHtml("#bc6c25");
            SizeF textSize4 = TextRenderer.MeasureText(detailchoixDispo.Text, detailchoixDispo.Font);
            detailchoixDispo.Size = new Size((int)textSize4.Width, (int)textSize4.Height);
            detailchoixDispo.Location = new System.Drawing.Point((int)textSize.Width + 55 + 100 + 50 + (int)textSize2.Width + 5 + 100 + 50 + (int)textSize3.Width + 5 + 100 + 50, 200);

            //BOX de saisie du choix dispo en semaine
            choixDispo = new TextBox();
            choixDispo.Size = new System.Drawing.Size(400, 20);
            choixDispo.Location = new System.Drawing.Point((int)textSize.Width + 55 + 100 + 50 + (int)textSize2.Width + 5 + 100 + 50 + (int)textSize3.Width + 5 + 100 + 50 + (int)textSize4.Width + 5, 195);
            choixDispo.Text = "Matin/Midi/Après-midi/Fin après-midi/Soir";
            choixDispo.BorderStyle = BorderStyle.None; //pas de bordure
            choixDispo.ForeColor = System.Drawing.ColorTranslator.FromHtml("#bc6c25");



            choixVille.KeyPress += ChoixVille_KeyPress;
        }

        private void ChoixVille_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Vérifiez si l'utilisateur a appuyé sur la touche "Entrée"
            if (e.KeyChar == (char)Keys.Enter)
            {
                // Récupérez la saisie de l'utilisateur à partir du TextBox
                villeChoisie = choixVille.Text;
                nbHeureTotale = int.Parse(choixNbHeure.Text);
                frequenceEntrainement = int.Parse(choixFrequence.Text);
                dispoSemaine = choixDispo.Text;

                MessageBox.Show("Tu t'entraînes à " + villeChoisie + " cette semaine :) !!!");

                // On appelle la méthode qui permet de récupérer les données méteos associées à la ville sélectionnée
                GetWeatherDataAsync(villeChoisie,nbHeureTotale,frequenceEntrainement,dispoSemaine);
            }
        }

        private async void GetWeatherDataAsync(string cityName,int nbHeure, int freq, string momentDispo)
        {
            float tempsDeSeance = (float)nbHeure / freq; //pour savoir combien de temps prend une séance
            int temperatureOptimale = 20; //temperature optimale d'entrainement

            Dictionary<string, int> correspondancesInversees = new Dictionary<string, int>
            {
                {"Matin", 9},
                {"Midi", 12},
                {"Après-midi", 15},
                {"Fin après-midi", 18},
                {"Soir", 21}
            };

            if (correspondancesInversees.TryGetValue(momentDispo, out int corressH))
            {
                momentDispoCorress = corressH;
            }
            else
            {
                // Gérer le cas où la valeur n'est pas trouvée
                MessageBox.Show("Moment de disponibilité inconnu");
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Remplacez {city name} et {API key} par les valeurs actuelles
                    string apiUrlWithParams = apiUrl.Replace("{city name}", cityName).Replace("{API key}", apiKey);

                    HttpResponseMessage response = await client.GetAsync(apiUrlWithParams);

                    if (response != null && response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(json);
                        // Désérialiser les données JSON en utilisant les classes créées
                        StructureJson DataRecuperees = JsonConvert.DeserializeObject<StructureJson>(json);

                        CityData cityData = DataRecuperees.city; //on recupere sur le niveau 1 de StructureJson la propriétés qui nous intéresse (city ou list)
                        List<AllListData> allListData = DataRecuperees.list; //on recupere sur le niveau 1 de StructureJson la propriétés qui nous intéresse (city ou list)

                        MessageBox.Show($"Nous récupérons les conditions météos pour la ville de : {cityData.name}");
                        

                        foreach (var data in allListData)
                        {
                            // Accédez aux données de chaque entrée
                            MainData mainData = data.main;
                            List<WeatherData> weatherDataList = data.weather;
                            DateTime dt_txt = data.dt_txt;
                            
                            //on récupère dans une variable chaque données :
                            string date = dt_txt.Date.ToString("dd MMMM yyyy"); // date (sans heure)
                            int heure = dt_txt.Hour; //heure (on récupère juste le 18 dans : "18:00:00")
                            int temperature = (int)Math.Round(mainData.temp - 273.15); //on met en degré
                            string meteo = weatherDataList[0].main;
                            string meteoDetail = weatherDataList[0].description;
                            string icon = weatherDataList[0].icon; 
                            string iconUrl = $"https://openweathermap.org/img/w/{icon}.png";
                            double ecartTempe = Math.Abs(temperature-temperatureOptimale);
                            

                            allDate.Add(date);

                            MyPrevisions forecast = new MyPrevisions
                            {
                                Date = date,
                                Hour = heure,
                                Temperature = temperature,
                                Weather = meteo,
                                WeatherDetail = meteoDetail,
                                Icon = icon,
                            };
                            myListPrevisions.Add(forecast);

                            if (heure == momentDispoCorress)
                            {
                                MyBestDay myBestDay = new MyBestDay
                                {
                                    Date = date,
                                    Hour = heure,
                                    Temperature = temperature,
                                    Weather = meteo,
                                    WeatherDetail = meteoDetail,
                                    Icon = icon,
                                    ProcheTemperatureOptimale = ecartTempe
                                };
                                myListBestDay.Add(myBestDay);
                            }


                            // MessageBox.Show($"Prévision pour le {date} à {heure}h :\ntempérature : {temperature} °C\ntemps : {meteo}->{meteoDetail}");
                            
                        }

                        myListBestDay = myListBestDay.OrderBy(day => Math.Abs(day.ProcheTemperatureOptimale)).ToList();
                        myListBestDay = myListBestDay.Take(freq).ToList();

                        foreach (MyBestDay bestDay in myListBestDay)
                        {
                            allDateBest.Add(bestDay.Date);
                            // MessageBox.Show($"Cette date {bestDay.Date} fait partie de la sélection !!!!");
                        }


                        AfficherDates();
                        MessageBox.Show($"Données météo récupérées avec succès !");
                    }
                    else
                    {
                        MessageBox.Show($"Erreur lors de la requête à l'API : {response.StatusCode}");
                        Console.WriteLine($"Erreur lors de la requête à l'API : {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.ToString()}");
            }
        }

        private void AfficherDates()
        {
            dataGridViewDates.EnableHeadersVisualStyles = false;
            //on réinitialise le tableau pour chaque appel de la méthode
            dataGridViewDates.Columns.Clear();
            dataGridViewDates.Rows.Clear();

            // liste de date unique
            var uniqueDates = allDate.Distinct().ToList();

            //ajout des colonnes 
            dataGridViewDates.Columns.Add("col1","Date");
            dataGridViewDates.Columns.Add("col2","Matin");
            dataGridViewDates.Columns.Add("col3","Midi");
            dataGridViewDates.Columns.Add("col4","Après-midi");
            dataGridViewDates.Columns.Add("col5","Fin d'apès-mi");
            dataGridViewDates.Columns.Add("col6","Soir");

            DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
            headerStyle.BackColor = ColorTranslator.FromHtml("#bc6c25"); // Couleur de fond
            headerStyle.ForeColor = ColorTranslator.FromHtml("#fefae0"); // Couleur du texte

            foreach (DataGridViewColumn col in dataGridViewDates.Columns)
            {
                col.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; 

                
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font(dataGridViewDates.Font, FontStyle.Bold);
                col.HeaderCell.Style.ApplyStyle(headerStyle);
            }


            foreach (var date in uniqueDates)
            {
                // Récupérez les prévisions météorologiques pour la date spécifiée
                var dateForecasts = myListPrevisions.Where(f => f.Date == date).ToList();

                // controle que les heures 9, 12, 15, 18 et 21 sont présentes
                var forecast9AM = dateForecasts.FirstOrDefault(f => f.Hour == 9);
                var forecast12PM = dateForecasts.FirstOrDefault(f => f.Hour == 12);
                var forecast3PM = dateForecasts.FirstOrDefault(f => f.Hour == 15);
                var forecast6PM = dateForecasts.FirstOrDefault(f => f.Hour == 18);
                var forecast9PM = dateForecasts.FirstOrDefault(f => f.Hour == 21);

                // MessageBox.Show($"{date} : fait partie des mailleures dates ? {estMeilleureDate}");
                dataGridViewDates.Rows.Add(
                    date,
                    forecast9AM != null ? $"{forecast9AM.Temperature.ToString()}°C\n{forecast9AM.Weather} ({forecast9AM.WeatherDetail})" : "X",
                    forecast12PM != null ? $"{forecast12PM.Temperature.ToString()}°C\n{forecast12PM.Weather} ({forecast12PM.WeatherDetail})" : "X",
                    forecast3PM != null ? $"{forecast3PM.Temperature.ToString()}°C\n{forecast3PM.Weather} ({forecast3PM.WeatherDetail})" : "X",
                    forecast6PM != null ? $"{forecast6PM.Temperature.ToString()}°C\n{forecast6PM.Weather} ({forecast6PM.WeatherDetail})" : "X",
                    forecast9PM != null ? $"{forecast9PM.Temperature.ToString()}°C\n{forecast9PM.Weather} ({forecast9PM.WeatherDetail})": "X"
                );
            }
            
            // nouvelle boucle foreach pour remplacer les données sur certaines cellules
            //récuperer l'heure defini par l'utilisateur
            MessageBox.Show($"L'heure choisie est : {momentDispoCorress} et correspond à la colonne {choixDispo.Text}"); //ex : 21 = Soir
            foreach (var date in allDateBest)
            {
                //recuperer les previsions meteorologiques pour la date specifiee
                var dateBestForcasts = myListBestDay.Where(f => f.Date == date).ToList();

                //recuperer l'index de la colonne qu'on veut
                int columnIndex = -1; //l'index des colonnes commence à 0
                foreach (DataGridViewColumn col in dataGridViewDates.Columns)
                {
                    if (col.HeaderText == choixDispo.Text)
                    {
                        columnIndex = col.Index;
                        break;
                    }
                }
                // MessageBox.Show($"L'index de la colonne {choixDispo.Text} est {columnIndex}");

                //recuperer l'index de la ligne qu'on veut
                int rowIndex = -1;
                for (int i = 0; i < dataGridViewDates.Rows.Count; i++)
                {
                    var cellValue = dataGridViewDates.Rows[i].Cells["col1"].Value;
                    if (cellValue != null && cellValue.ToString() == date)
                    {
                        // L'index de la ligne où la valeur correspond à la date est i
                        rowIndex = i;
                    }
                }
                MessageBox.Show($"L'index de la ligne correspondant à la date {date} est {rowIndex}");


                if (columnIndex != -1)
                {
                    var forecastBest = dateBestForcasts.FirstOrDefault(f => f.Hour == momentDispoCorress);
                    dataGridViewDates[columnIndex, rowIndex].Value =
                        forecastBest != null ? $"BEST DAY TO RUN\n{forecastBest.Temperature.ToString()}°C\n{forecastBest.Weather} ({forecastBest.WeatherDetail})" : "X";
                    dataGridViewDates[columnIndex, rowIndex].Style.ForeColor = ColorTranslator.FromHtml("#fefae0");
                    dataGridViewDates[columnIndex, rowIndex].Style.BackColor = ColorTranslator.FromHtml("#DDA15E");
                }

            }

            dataGridViewDates.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewDates.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;


            dataGridViewDates.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            dataGridViewDates.Dock = DockStyle.Bottom;

            int totalHeight = dataGridViewDates.ColumnHeadersHeight;

            foreach (DataGridViewRow row in dataGridViewDates.Rows)
            {
                totalHeight += row.Height;
            }

            totalHeight += 125;

            dataGridViewDates.Height = totalHeight;

            dataGridViewDates.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#fefae0");
            dataGridViewDates.DefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#bc6c25");

            dataGridViewDates.RowHeadersVisible = false;

            dataGridViewDates.ReadOnly = true; //pour ne pas modifier les cellules

            dataGridViewDates.GridColor = ColorTranslator.FromHtml("#bc6c25");

            // dataGridViewDates.Dock = DockStyle.Fill;
            Controls.Add(dataGridViewDates);
        }

        private void AddControls()
        {
            Controls.Add(lblWelcome);

            Controls.Add(detailConsigne);

            Controls.Add(choixVille);
            Controls.Add(detailChoixVille);

            Controls.Add(choixNbHeure);
            Controls.Add(detailchoixNbHeure);

            Controls.Add(choixFrequence);
            Controls.Add(detailchoixFrequence);

            Controls.Add(choixDispo);
            Controls.Add(detailchoixDispo);
        }
        
    } // end of "class Form1"
} // enf of "namespace AppTrainWithWeather"




