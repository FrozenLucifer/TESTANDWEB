using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DetectiveUI.ApiClient;
using DetectiveUI.Views;
using DTOs;
using DTOs.Enum;

namespace DetectiveUI.ViewModels;

public class ReadTabViewModel : INotifyPropertyChanged
{
    private readonly IApiClient _apiClient;

    private GetPersonFilterDto _filter = new();
    private string _errorMessage;
    private bool _useSexFilter;

    public bool UseSexFilter
    {
        get => _useSexFilter;
        set
        {
            _useSexFilter = value;
            OnPropertyChanged();
        }
    }

    public List<SexDto> SexList { get; } = Enum.GetValues<SexDto>().ToList();

    public ObservableCollection<PersonDto> Persons { get; } = new();
    public ObservableCollection<RelationshipDto> Relationships { get; } = new();
    public ObservableCollection<ContactDto> Contacts { get; } = new();
    public ObservableCollection<DocumentDto> Documents { get; } = new();
    public ObservableCollection<PropertyDto> Properties { get; } = new();
    
    public ObservableCollection<CharacteristicDto> Characteristics { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand SearchCommand { get; }
    public ICommand ClearFiltersCommand { get; }

    public ICommand SearchByIdCommand { get; }
    public ICommand GoToRelatedPersonCommand { get; }
    public ICommand CopyIdCommand { get; }

    public GetPersonFilterDto Filter
    {
        get => _filter;
        set
        {
            _filter = value;
            OnPropertyChanged();
        }
    }

    private PersonDto? _selectedPerson;
    private PersonFullDto? _selectedPersonDetails;

    public PersonDto? SelectedPerson
    {
        get => _selectedPerson;
        set
        {
            if (_selectedPerson != value)
            {
                _selectedPerson = value;
                OnPropertyChanged();

                if (value != null)
                {
                    _ = LoadPersonDetails(value.Id);
                }
            }
        }
    }

    public PersonFullDto? SelectedPersonDetails
    {
        get => _selectedPersonDetails;
        set
        {
            _selectedPersonDetails = value;
            OnPropertyChanged();
        }
    }

    private string _searchById;

    public string SearchById
    {
        get => _searchById;
        set
        {
            _searchById = value;
            OnPropertyChanged();
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public ReadTabViewModel(IApiClient apiClient)
    {
        _apiClient = apiClient;

        SearchCommand = new RelayCommand(async () => await SearchPersons());
        ClearFiltersCommand = new RelayCommand(ClearFilters);
        SearchByIdCommand = new RelayCommand(async () => await SearchByIdAsync());
    }

    private async Task SearchByIdAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchById)) return;

        try
        {
            if (Guid.TryParse(SearchById, out var personId))
            {
                await LoadPersonDetails(personId);

                var person = Persons.FirstOrDefault(p => p.Id == personId);

                if (person != null)
                {
                    SelectedPerson = person;
                }
                else
                {
                    await SearchPersons();
                }
            }
            else
            {
                ErrorMessage = "Неверный формат ID";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при поиске по ID: {ex.Message}";
        }
    }

    private async Task SearchPersons()
    {
        try
        {
            ErrorMessage = string.Empty;

            var filterCopy = new GetPersonFilterDto
            {
                FullName = Filter.FullName,
                MinBirthDate = Filter.MinBirthDate,
                MaxBirthDate = Filter.MaxBirthDate,
                Limit = Filter.Limit,
                Sex = UseSexFilter ? Filter.Sex : null
            };

            var persons = await _apiClient.GetPersonsAsync(filterCopy);

            Persons.Clear();

            foreach (var person in persons)
            {
                Persons.Add(person);
            }
            

        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при поиске: {ex.Message}";
        }
    }

    private void ClearFilters()
    {
        Filter = new GetPersonFilterDto();
        UseSexFilter = false;
    }

    private async Task LoadPersonDetails(Guid personId)
    {
        try
        {
            ErrorMessage = string.Empty;
            var person = await _apiClient.GetPersonInfoAsync(personId);
            SelectedPersonDetails = person;

            var relationships = await _apiClient.GetPersonRelationshipsAsync(personId);

            Relationships.Clear();

            foreach (var rel in relationships)
            {
                Relationships.Add(rel);
            }
            Contacts.Clear();

            foreach (var contact in person.Contacts)
            {
                Contacts.Add(contact);
            }

            Documents.Clear();

            foreach (var doc in person.Documents)
            {
                Documents.Add(doc);
            }

            Properties.Clear();

            foreach (var prop in person.Properties)
            {
                Properties.Add(prop);
            }
            
            Characteristics.Clear();
            foreach (var charact in person.Characteristics)
            {
                Characteristics.Add(charact);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при загрузке деталей: {ex.Message}";
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}