using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows.Shapes;
using TheMossadRecruiter.Models;

namespace TheMossadRecruiter
{
    public partial class MainWindowViewModel : ObservableObject
    {
        #region Fields

        [ObservableProperty]
        string yearsOfExperience;
        [ObservableProperty]
        List<Technology> technologies;
        [ObservableProperty]
        List<Candidate> candidates;
        [ObservableProperty]
        ObservableCollection<Candidate> acceptedCandidates;
        [ObservableProperty]
        Technology selectedTechnology;
        [ObservableProperty]
        Candidate selectedCandidate;
        [ObservableProperty]
        List<Candidate> filteredCandidates;

        #endregion

        public MainWindowViewModel()
        {
            InitializeData();
        }

        #region RelayCommands

        [RelayCommand]
        private void ButtonSwipeRight()
        {
            if (Candidates.Contains(SelectedCandidate))
            {
                Candidates.Remove(SelectedCandidate);

                FilterCandidates();
            }
        }
        [RelayCommand]
        private void ButtonSwipeLeft()
        {
            if (SelectedCandidate != null)
            {
                AcceptedCandidates.Add(SelectedCandidate);
                Candidates.Remove(SelectedCandidate);

                FilterCandidates();
            }
        }

        [RelayCommand]
        private void ApplyFilter()
        {
            FilterCandidates();
        }

        #endregion

        private async void InitializeData()
        {
            HttpClient client = new HttpClient();
            string techUrl = "https://app.ifs.aero/EternalBlue/api/technologies";
            string candidatesUrl = "https://app.ifs.aero/EternalBlue/api/candidates";

            // Start both API calls concurrently
            Task<string> apiCallTask1 = GetApiResponseAsync(client, techUrl);
            Task<string> apiCallTask2 = GetApiResponseAsync(client, candidatesUrl);

            // Wait for both tasks to complete
            await Task.WhenAll(apiCallTask1, apiCallTask2);

            // Retrieve results from tasks
            string result1 = await apiCallTask1;
            string result2 = await apiCallTask2;
            Technologies = JsonConvert.DeserializeObject<List<Technology>>(result1) ?? [];
            Candidates = JsonConvert.DeserializeObject<List<Candidate>>(result2) ?? [];

            AcceptedCandidates = new ObservableCollection<Candidate>();
        }

        private void FilterCandidates()
        {
            // Filter candidates based on selected technology and years of experience
            string selectedTech = SelectedTechnology.Name;

            FilteredCandidates = Candidates.Where(c =>
                c.Experience.Any(e => e.TechnologyId == SelectedTechnology.Guid &&
                                      e.YearsOfExperience >= int.Parse(YearsOfExperience))
            ).ToList();
        }

        private async Task<string> GetApiResponseAsync(HttpClient httpClient, string apiUrl)
        {
            // Make HTTP GET request asynchronously
            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            // Ensure success status code
            response.EnsureSuccessStatusCode();

            // Read response content as string
            return await response.Content.ReadAsStringAsync();
        }

    }
}
