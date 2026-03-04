using i_freeze.Commands.MessageBoxCommands;
using Infrastructure.DataContext;
using i_freeze.DTOs;
using i_freeze.Utilities;
using System.Net.Http;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;
using Infrastructure;

namespace i_freeze.ViewModel
{
    public class SupportViewModel : ViewModelBase
    {

        private const string ApiUrl = "Ticket";
        private static readonly HttpClient client = new HttpClient();
        private AppConfigAndLoginContext context = new AppConfigAndLoginContext();

        private string _name;
        public string Name
        {
            get { return _name; }

            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private string _phone;
        public string Phone
        {
            get { return _phone; }

            set
            {
                _phone = value;
                OnPropertyChanged();
            }
        }

        private string _email;
        public string Email
        {
            get { return _email; }

            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }

            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }


        public ICommand SubmitCommand { get; }

        public SupportViewModel()
        {
            SubmitCommand = new RelayCommand(SubmitAsync);
        }

        private async void SubmitAsync(object obj)
        {
            try
            {

                // Basic data validation
                if (string.IsNullOrWhiteSpace(Name) ||
                    string.IsNullOrWhiteSpace(Email) ||
                    string.IsNullOrWhiteSpace(Phone) ||
                    string.IsNullOrWhiteSpace(Description))
                {
                    //ShowMessageBox("All fields are required and cannot be empty.");
                    new ShowMessage("All fields are required and cannot be empty.");
                    return;
                }

                // More thorough validation can be added here (e.g., email format validation)

                Guid deviceId;
                
                // Assuming there's an asynchronous version of FirstOrDefault
                var config = context.Application_Configuration.FirstOrDefault();
                deviceId = config?.DeviceSN ?? Guid.Empty;

                 

                if (deviceId == Guid.Empty)
                {
                    new ShowMessage("Device not configured properly. Unable to fetch DeviceSN.");
                    return;
                }

                var ticket = new TicketDTO
                {
                    Id = Guid.NewGuid(),
                    DeviceId = deviceId,
                    Name = Name.Trim(),
                    Email = Email.Trim(),
                    Phone = Phone.Trim(),
                    Description = Description.Trim()
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await DeviceManagement.Token());
                //var jsonContent = new StringContent(JsonContent.SerializeObject(ticket), Encoding.UTF8, "application/json");
                var jsonContent = new StringContent(JsonSerializer.Serialize(ticket), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(DeviceManagement.MainURL + ApiUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    new ShowMessage("Ticket submitted successfully.");
                }
                else
                {
                    new ShowMessage($"Failed to submit ticket. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Log the exception if logging is available
                new ShowMessage($"An error occurred: {ex.Message}");
            }
        }

    }
}
