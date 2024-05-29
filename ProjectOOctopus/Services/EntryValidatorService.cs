
namespace ProjectOOctopus.Services
{
    public class EntryValidatorService
    {
        private readonly Dictionary<string, bool> _validationValues = new Dictionary<string, bool>();
        private readonly Dictionary<string, ValidationData> _validationData = new Dictionary<string, ValidationData>();

        public bool AllValid => _validationValues.Values == null || _validationValues.Values.All(x => x == true);

        public EntryValidatorService()
        {

        }

        public bool TryRegisterValidation(string name, Entry entry, Func<string, bool> validationFunction, Label errorText, bool initialValue = false)
        {
            if (_validationData.ContainsKey(name))
            {
                return false;
            }

            _validationValues.Add(name, initialValue);
            ValidationData data = new ValidationData(name, entry, validationFunction, errorText);
            _validationData.Add(name, data);

            data.ValidationChanged += AnyDataValidationChanged;
            return true;
        }

        public bool TryUnregisterValidation(string name)
        {
            return _validationData.Remove(name) && _validationValues.Remove(name);
        }

        public void ClearAllValidations()
        {
            _validationData.Clear();
            _validationValues.Clear();
        }

        public void Revalidate(string name)
        {
            if (_validationData.TryGetValue(name, out ValidationData data))
            {
                data.ReEvaluate();
            }
        }

        public void RevalidateAll()
        {
            foreach (ValidationData data in _validationData.Values)
            {
                data.ReEvaluate();
            }
        }

        private void AnyDataValidationChanged(object? sender, bool arg)
        {
            ValidationData data = (ValidationData)sender;

            _validationValues[data.Name] = arg;
        }
    }

    public class ValidationData
    {
        public event EventHandler<bool> ValidationChanged;

        public string Name { get; private set; }

        public Entry EntryElement { get; private set; }
        public Func<string, bool> ValidationDelegateFunction { get; private set; }
        public Label ErrorText { get; private set; }

        public ValidationData(string name, Entry entry, Func<string, bool> validationDelegateFunction, Label errorText)
        {
            Name = name;
            EntryElement = entry;
            ValidationDelegateFunction = validationDelegateFunction;
            ErrorText = errorText;

            EntryElement.TextChanged += EntryElement_TextChanged;
        }

        private void EntryElement_TextChanged(object? sender, TextChangedEventArgs e)
        {
            ReEvaluate(e.NewTextValue);
        }

        public void ReEvaluate(string input = null)
        {
            input = string.IsNullOrEmpty(input) ? EntryElement.Text : input;

            bool isValid = ValidationDelegateFunction(input);

            ErrorText.IsVisible = !isValid;
            ValidationChanged?.Invoke(this, isValid);
        }
    }
}