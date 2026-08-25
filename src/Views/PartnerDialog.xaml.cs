using System.Windows;
using System.Windows.Controls;
using SOCDOF.Data;

namespace SOCDOF.Views;

public partial class PartnerDialog : Window
{
    private readonly int _partnerId;
    private readonly DateTime _createdAt;

    public PartnerDialog(Partner? partner = null)
    {
        InitializeComponent();
        Title = AppConfig.AppName;

        if (partner is null)
        {
            _createdAt = DateTime.Now;
            return;
        }

        _partnerId = partner.Id;
        _createdAt = partner.CreatedAt;
        DialogTitle.Text = "Partner bearbeiten";
        SaveButton.Content = "Änderungen speichern";
        NameBox.Text = partner.Name;
        EmailBox.Text = partner.Email;
        PhoneBox.Text = partner.Phone;
        AddressBox.Text = partner.Address;
    }

    public Partner ResultPartner { get; private set; } = null!;

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var name = NameBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show(
                "Bitte gib einen Namen ein.",
                AppConfig.AppName,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            NameBox.Focus();
            return;
        }

        ResultPartner = new Partner
        {
            Id = _partnerId,
            Name = name,
            Email = ToNullableValue(EmailBox.Text),
            Phone = ToNullableValue(PhoneBox.Text),
            Address = ToNullableValue(AddressBox.Text),
            CreatedAt = _createdAt
        };

        DialogResult = true;
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private static string? ToNullableValue(string value)
    {
        var trimmedValue = value.Trim();
        return string.IsNullOrWhiteSpace(trimmedValue) ? null : trimmedValue;
    }
}
