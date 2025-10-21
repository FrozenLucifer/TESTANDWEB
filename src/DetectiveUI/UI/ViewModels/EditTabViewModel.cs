using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DetectiveUI.ApiClient;
using DTOs;
using DTOs.Enum;

// using RelayCommand = DetectiveUI.Views.RelayCommand;

namespace DetectiveUI.ViewModels;

public class EditTabViewModel : INotifyPropertyChanged
{
    private readonly IApiClient _apiClient;

    private GetPersonFilterDto _filter = new();
    private string _errorMessage;
    private string _successMessage;
    private bool _useSexFilter;

    public event PropertyChangedEventHandler PropertyChanged;

    public ObservableCollection<PersonDto> Persons { get; } = new();
    public ObservableCollection<ContactDto> Contacts { get; } = new();
    public ObservableCollection<RelationshipDto> Relationships { get; } = new();
    public ObservableCollection<DocumentDto> Documents { get; } = new();
    public ObservableCollection<PropertyDto> Properties { get; } = new();

    public List<SexDto> SexList { get; } = Enum.GetValues<SexDto>().ToList();
    public List<ContactTypeDto> ContactTypes { get; } = Enum.GetValues<ContactTypeDto>().ToList();
    public List<RelationshipTypeDto> RelationshipTypes { get; } = Enum.GetValues<RelationshipTypeDto>().ToList();

    public ICommand SearchCommand { get; }
    public ICommand ClearFiltersCommand { get; }
    public ICommand SearchByIdCommand { get; }
    public ICommand SaveChangesCommand { get; }
    public ICommand AddContactCommand { get; }
    public ICommand DeleteContactCommand { get; }
    public ICommand AddRelationshipCommand { get; }
    public ICommand DeleteRelationshipCommand { get; }
    public ICommand AddDocumentCommand { get; }
    public ICommand DeleteDocumentCommand { get; }
    public ICommand AddPropertyCommand { get; }
    public ICommand DeletePropertyCommand { get; }

    public EditTabViewModel(IApiClient apiClient)
    {
        _apiClient = apiClient;

        SearchCommand = new RelayCommand(async () => await SearchPersons());
        ClearFiltersCommand = new RelayCommand(ClearFilters);
        SearchByIdCommand = new RelayCommand(async () => await SearchByIdAsync());
        SaveChangesCommand = new RelayCommand(async () => await SaveChanges());
        AddContactCommand = new RelayCommand(async () => await AddContact());
        DeleteContactCommand = new RelayCommand<Guid>(async (id) => await DeleteContact(id));
        AddRelationshipCommand = new RelayCommand(async () => await AddRelationship());
        DeleteRelationshipCommand = new RelayCommand<Guid>(async (id) => await DeleteRelationship(id));
        AddDocumentCommand = new RelayCommand(async () => await AddDocument());
        DeleteDocumentCommand = new RelayCommand<Guid>(async (id) => await DeleteDocument(id));
        AddPropertyCommand = new RelayCommand(async () => await AddProperty());
        DeletePropertyCommand = new RelayCommand<Guid>(async (id) => await DeleteProperty(id));
        CreatePersonCommand = new RelayCommand(async () => await CreatePerson(), CanCreatePerson);
        ShowCreateDialogCommand = new RelayCommand(() => ShowCreateDialog = true);
        CancelCreateCommand = new RelayCommand(() =>
        {
            ShowCreateDialog = false;
            NewPersonFullName = string.Empty;
            NewPersonBirthDate = null;
        });
        DeletePersonCommand = new RelayCommand(async () => await DeletePerson(), CanDeletePerson);

        AddCharacteristicCommand = new RelayCommand(async () => await AddCharacteristic());
        DeleteCharacteristicCommand = new RelayCommand<Guid>(async (id) => await DeleteCharacteristic(id));
    }

    public GetPersonFilterDto Filter
    {
        get => _filter;
        set
        {
            _filter = value;
            OnPropertyChanged();
        }
    }

    public bool UseSexFilter
    {
        get => _useSexFilter;
        set
        {
            _useSexFilter = value;
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
                ((RelayCommand)DeletePersonCommand).NotifyCanExecuteChanged();


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

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public string SuccessMessage
    {
        get => _successMessage;
        set
        {
            _successMessage = value;
            OnPropertyChanged();
        }
    }

    public string NewContactInfo { get; set; }
    public ContactTypeDto SelectedContactType { get; set; }
    public RelationshipTypeDto SelectedRelationshipType { get; set; }
    public string RelatedPersonId { get; set; }
    public string NewPropertyName { get; set; }
    public int? NewPropertyCost { get; set; }
    public string NewDocumentInfo { get; set; }

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

    private async Task SearchByIdAsync()
    {
        if (string.IsNullOrWhiteSpace(RelatedPersonId)) return;

        try
        {
            if (Guid.TryParse(RelatedPersonId, out var personId))
            {
                await LoadPersonDetails(personId);
                var person = Persons.FirstOrDefault(p => p.Id == personId);
                if (person != null) SelectedPerson = person;
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

    private void ClearFilters()
    {
        Filter = new GetPersonFilterDto();
        UseSexFilter = false;
    }

    private async Task LoadPersonDetails(Guid personId)
    {
        try
        {
            var person = await _apiClient.GetPersonInfoAsync(personId);
            SelectedPersonDetails = person;

            var relationships = await _apiClient.GetPersonRelationshipsAsync(personId);
            Relationships.Clear();
            foreach (var rel in relationships) Relationships.Add(rel);

            Contacts.Clear();
            foreach (var contact in person.Contacts) Contacts.Add(contact);

            Documents.Clear();
            foreach (var doc in person.Documents) Documents.Add(doc);

            Properties.Clear();
            foreach (var prop in person.Properties) Properties.Add(prop);

            Characteristics.Clear();
            foreach (var charact in person.Characteristics) Characteristics.Add(charact);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при загрузке деталей: {ex.Message}";
        }
    }

    private async Task SaveChanges()
    {
        if (SelectedPerson == null) return;

        try
        {
            await _apiClient.ChangePersonGeneralInfoAsync(
                SelectedPerson.Id,
                SelectedPerson.Sex,
                SelectedPerson.FullName,
                SelectedPerson.BirthDate);

            SuccessMessage = "Изменения сохранены успешно!";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при сохранении: {ex.Message}";
        }
    }

    private async Task AddContact()
    {
        if (SelectedPerson == null || string.IsNullOrWhiteSpace(NewContactInfo)) return;

        try
        {
            await _apiClient.AddPersonContactAsync(SelectedPerson.Id, SelectedContactType, NewContactInfo);
            await LoadPersonDetails(SelectedPerson.Id);
            NewContactInfo = string.Empty;
            SuccessMessage = "Контакт добавлен успешно!";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при добавлении контакта: {ex.Message}";
        }
    }

    private async Task DeleteContact(Guid contactId)
    {
        try
        {
            await _apiClient.DeletePersonContactAsync(contactId);
            await LoadPersonDetails(SelectedPerson.Id);
            SuccessMessage = "Контакт удален успешно!";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при удалении контакта: {ex.Message}";
        }
    }

    private async Task AddRelationship()
    {
        if (SelectedPerson == null || !Guid.TryParse(RelatedPersonId, out var relatedPersonId)) return;

        try
        {
            await _apiClient.ConnectPersonsAsync(SelectedPerson.Id, relatedPersonId, SelectedRelationshipType);
            await LoadPersonDetails(SelectedPerson.Id);
            RelatedPersonId = string.Empty;
            SuccessMessage = "Связь добавлена успешно!";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при добавлении связи: {ex.Message}";
        }
    }

    private async Task DeleteRelationship(Guid person2Id)
    {
        try
        {
            await _apiClient.DeleteRelationship(SelectedPerson.Id, person2Id);
            await LoadPersonDetails(SelectedPerson.Id);
            SuccessMessage = "Связь удалена успешно!";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при удалении связи: {ex.Message}";
        }
    }

    private async Task AddDocument()
    {
        if (SelectedPerson == null || string.IsNullOrWhiteSpace(NewDocumentInfo)) return;

        try
        {
            var passportInfo = new PassportPayloadDto { Series = "1234", Number = NewDocumentInfo };
            await _apiClient.AddPersonPassportAsync(SelectedPerson.Id, passportInfo);
            await LoadPersonDetails(SelectedPerson.Id);
            NewDocumentInfo = string.Empty;
            SuccessMessage = "Документ добавлен успешно!";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при добавлении документа: {ex.Message}";
        }
    }

    private async Task DeleteDocument(Guid documentId)
    {
        try
        {
            await _apiClient.DeletePersonDocumentAsync(documentId);
            await LoadPersonDetails(SelectedPerson.Id);
            SuccessMessage = "Документ удален успешно!";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при удалении документа: {ex.Message}";
        }
    }

    private async Task AddProperty()
    {
        if (SelectedPerson == null || string.IsNullOrWhiteSpace(NewPropertyName)) return;

        try
        {
            await _apiClient.AddPersonPropertyAsync(SelectedPerson.Id, NewPropertyName, NewPropertyCost);
            await LoadPersonDetails(SelectedPerson.Id);
            NewPropertyName = string.Empty;
            NewPropertyCost = null;
            SuccessMessage = "Имущество добавлено успешно!";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при добавлении имущества: {ex.Message}";
        }
    }

    private async Task DeleteProperty(Guid propertyId)
    {
        try
        {
            await _apiClient.DeletePersonPropertyAsync(propertyId);
            await LoadPersonDetails(SelectedPerson.Id);
            SuccessMessage = "Имущество удалено успешно!";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при удалении имущества: {ex.Message}";
        }
    }

    public ICommand CreatePersonCommand { get; }
    private string _newPersonFullName;
    private DateOnly? _newPersonBirthDate;
    private SexDto _newPersonSex = SexDto.Male;

    public string NewPersonFullName
    {
        get => _newPersonFullName;
        set
        {
            _newPersonFullName = value;
            OnPropertyChanged();
            ((RelayCommand)CreatePersonCommand).NotifyCanExecuteChanged();
        }
    }

    public DateOnly? NewPersonBirthDate
    {
        get => _newPersonBirthDate;
        set
        {
            _newPersonBirthDate = value;
            OnPropertyChanged();
            ((RelayCommand)CreatePersonCommand).NotifyCanExecuteChanged();
        }
    }

    public SexDto NewPersonSex
    {
        get => _newPersonSex;
        set
        {
            _newPersonSex = value;
            OnPropertyChanged();
        }
    }


    private bool CanCreatePerson()
    {
        return !string.IsNullOrWhiteSpace(NewPersonFullName);
    }

    private async Task CreatePerson()
    {
        try
        {
            ErrorMessage = string.Empty;

            var createDto = new CreatePersonDto
            {
                FullName = NewPersonFullName,
                BirthDate = NewPersonBirthDate,
                Sex = NewPersonSex
            };

            var newPersonId = await _apiClient.CreatePersonAsync(createDto);

            await SearchPersons();
            var newPerson = Persons.FirstOrDefault(p => p.Id == newPersonId);

            if (newPerson != null)
            {
                SelectedPerson = newPerson;
            }

            SuccessMessage = "Человек успешно создан!";
            NewPersonFullName = string.Empty;
            NewPersonBirthDate = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при создании человека: {ex.Message}";
        }
    }

    private bool _showCreateDialog;

    public bool ShowCreateDialog
    {
        get => _showCreateDialog;
        set
        {
            _showCreateDialog = value;
            OnPropertyChanged();
        }
    }

    public ICommand ShowCreateDialogCommand { get; }
    public ICommand CancelCreateCommand { get; }

    public ICommand DeletePersonCommand { get; }


    private bool CanDeletePerson()
    {
        return SelectedPerson != null;
    }

    private async Task DeletePerson()
    {
        if (SelectedPerson == null) return;

        try
        {
            var result = MessageBox.Show($"Вы уверены, что хотите удалить {SelectedPerson.FullName}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                await _apiClient.DeletePersonAsync(SelectedPerson.Id);

                SelectedPerson = null;
                await SearchPersons();

                SuccessMessage = "Человек успешно удален!";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при удалении: {ex.Message}";
        }
    }

    public ObservableCollection<CharacteristicDto> Characteristics { get; } = new();

    public string NewAppearance { get; set; }
    public string NewPersonality { get; set; }
    public string NewMedicalConditions { get; set; }

    public ICommand AddCharacteristicCommand { get; }
    public ICommand DeleteCharacteristicCommand { get; }


    private async Task AddCharacteristic()
    {
        if (SelectedPerson == null ||
            (string.IsNullOrWhiteSpace(NewAppearance) &&
             string.IsNullOrWhiteSpace(NewPersonality) &&
             string.IsNullOrWhiteSpace(NewMedicalConditions)))
            return;

        try
        {
            var createDto = new CreateCharacteristicDto
            {
                PersonId = SelectedPerson.Id,
                Appearance = NewAppearance,
                Personality = NewPersonality,
                MedicalConditions = NewMedicalConditions
            };

            await _apiClient.AddCharacteristicAsync(createDto);
            await LoadPersonDetails(SelectedPerson.Id);

            NewAppearance = string.Empty;
            NewPersonality = string.Empty;
            NewMedicalConditions = string.Empty;

            SuccessMessage = "Характеристика добавлена успешно!";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при добавлении характеристики: {ex.Message}";
        }
    }

    private async Task DeleteCharacteristic(Guid characteristicId)
    {
        try
        {
            await _apiClient.DeletePersonCharacteristicAsync(characteristicId);
            await LoadPersonDetails(SelectedPerson.Id);
            SuccessMessage = "Характеристика удалена успешно!";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при удалении характеристики: {ex.Message}";
        }
    }


    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}