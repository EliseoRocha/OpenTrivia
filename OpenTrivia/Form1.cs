using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenTrivia
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            client = new HttpClient();
            client.BaseAddress = new Uri("https://opentdb.com");
        }

        private static HttpClient client;

        private async void Form1_Load(object sender, EventArgs e)
        {
            //Access the list of categoies from the Web API
            HttpResponseMessage response = await client.GetAsync("api_category.php");

            if (response.IsSuccessStatusCode)
            {
                string cats = await response.Content.ReadAsStringAsync();

                CategoryResponse catResponse = JsonConvert.DeserializeObject<CategoryResponse>(cats);
                
                List<TriviaCategory> entertainment = GetEntertainmentCategories(catResponse);
                PopulateCategoryComboBox(entertainment);
            }
        }

        private void PopulateCategoryComboBox(List<TriviaCategory> entertainment)
        {
            //Could use databinding instead of loop
            //CategoriesComboBox.DataSource = catResponse.trivia_categories;
            //CategoriesComboBox.DisplayMember = nameof(TriviaCategory.name);

            foreach (TriviaCategory category in entertainment)
            {
                CategoriesComboBox.Items.Add(category);
            }
        }

        private static List<TriviaCategory> GetEntertainmentCategories(CategoryResponse catResponse)
        {
            //LINQ to Object
            //all entertainment categories sorted alphabetically
            return catResponse.trivia_categories
                .Where(c => c.name.StartsWith("Entertainment"))
                .OrderBy(c => c.name)
                .ToList();
        }

        private async void CategoriesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CategoriesComboBox.SelectedIndex < 0)
            {
                return;
            }
            //Get selected category id
            //TriviaCategory cat = (TriviaCategory)CategoriesComboBox.SelectedItem;
            TriviaCategory cat = CategoriesComboBox.SelectedItem as TriviaCategory;

            int selectedId = cat.id;

            //Get number of questions in that category
            HttpResponseMessage msg = await client.GetAsync($"api_count.php?category={selectedId}");

            if (msg.IsSuccessStatusCode)
            {
                string response = await msg.Content.ReadAsStringAsync();
                MessageBox.Show(response);
            }
        }
    }
}
