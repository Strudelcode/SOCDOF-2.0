using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
using SOCDOF.Data;

namespace SOCDOF.Views;

public partial class PartnersView : UserControl
{
    private readonly ObservableCollection<Partner> _partners = new();
    private readonly ICollectionView _partnersView;

    public PartnersView()
    {
        InitializeComponent();
        _partnersView = CollectionViewSource.GetDefaultView(_partners);
        _partnersView.Filter = FilterPartner;
        PartnerGrid.ItemsSource = _partnersView;
        Loaded += PartnersView_OnLoaded;
    }

    private void PartnersView_OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= PartnersView_OnLoaded;
        LoadPartners();
    }

    private void LoadPartners()
    {
        try
        {
            using var database = AppDbContext.Create();
            var partners = database.Partners
                .AsNoTracking()
                .OrderBy(partner => partner.Name)
                .ToList();

            _partners.Clear();
            foreach (var partner in partners)
            {
                _partners.Add(partner);
            }

            _partnersView.Refresh();
            UpdateViewState();
        }
        catch (Exception exception)
        {
            ShowError("Die Partner konnten nicht geladen werden.", exception);
        }
    }

    private bool FilterPartner(object item)
    {
        if (item is not Partner partner)
        {
            return false;
        }

        var searchTerm = SearchBox?.Text.Trim();
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return true;
        }

        return Contains(partner.Name, searchTerm)
            || Contains(partner.Email, searchTerm)
            || Contains(partner.Phone, searchTerm)
            || Contains(partner.Address, searchTerm);
    }

    private static bool Contains(string? value, string searchTerm)
    {
        return !string.IsNullOrWhiteSpace(value)
            && value.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase);
    }

    private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        _partnersView?.Refresh();
        UpdateViewState();
    }

    private void NewPartnerButton_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new PartnerDialog
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        try
        {
            using var database = AppDbContext.Create();
            database.Partners.Add(dialog.ResultPartner);
            database.SaveChanges();
            LoadPartners();
        }
        catch (Exception exception)
        {
            ShowError("Der Partner konnte nicht angelegt werden.", exception);
        }
    }

    private void EditPartnerButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (PartnerGrid.SelectedItem is not Partner selectedPartner)
        {
            return;
        }

        var dialog = new PartnerDialog(selectedPartner)
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        try
        {
            using var database = AppDbContext.Create();
            var partner = database.Partners.Single(existing => existing.Id == selectedPartner.Id);
            partner.Name = dialog.ResultPartner.Name;
            partner.Email = dialog.ResultPartner.Email;
            partner.Phone = dialog.ResultPartner.Phone;
            partner.Address = dialog.ResultPartner.Address;
            database.SaveChanges();
            LoadPartners();
        }
        catch (InvalidOperationException)
        {
            LoadPartners();
            MessageBox.Show(
                "Der Partner wurde zwischenzeitlich gelöscht.",
                AppConfig.AppName,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception exception)
        {
            ShowError("Der Partner konnte nicht gespeichert werden.", exception);
        }
    }

    private void DeletePartnerButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (PartnerGrid.SelectedItem is not Partner selectedPartner)
        {
            return;
        }

        var confirmation = MessageBox.Show(
            $"Möchtest du den Partner '{selectedPartner.Name}' wirklich löschen?",
            AppConfig.AppName,
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        if (confirmation != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            using var database = AppDbContext.Create();
            var partner = database.Partners.Find(selectedPartner.Id);
            if (partner is null)
            {
                LoadPartners();
                return;
            }

            database.Partners.Remove(partner);
            database.SaveChanges();
            LoadPartners();
        }
        catch (DbUpdateException)
        {
            MessageBox.Show(
                "Der Partner kann nicht gelöscht werden, weil noch Verkäufe auf ihn verweisen.",
                AppConfig.AppName,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception exception)
        {
            ShowError("Der Partner konnte nicht gelöscht werden.", exception);
        }
    }

    private void PartnerGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var hasSelection = PartnerGrid.SelectedItem is Partner;
        EditPartnerButton.IsEnabled = hasSelection;
        DeletePartnerButton.IsEnabled = hasSelection;
    }

    private void UpdateViewState()
    {
        var hasPartners = _partners.Count > 0;
        var hasVisiblePartners = _partnersView.Cast<object>().Any();
        PartnerGrid.Visibility = hasPartners && hasVisiblePartners
            ? Visibility.Visible
            : Visibility.Collapsed;
        EmptyPartnerState.Visibility = hasPartners && hasVisiblePartners
            ? Visibility.Collapsed
            : Visibility.Visible;
        PartnerToolbar.Visibility = hasPartners ? Visibility.Visible : Visibility.Collapsed;
        EditPartnerButton.IsEnabled = hasVisiblePartners && PartnerGrid.SelectedItem is Partner;
        DeletePartnerButton.IsEnabled = hasVisiblePartners && PartnerGrid.SelectedItem is Partner;
        PartnerCountText.Text = hasPartners
            ? $"{_partners.Count} Partner"
            : "Noch keine Einträge";

        if (hasPartners && !hasVisiblePartners)
        {
            PartnerCountText.Text = "Keine Treffer für die aktuelle Suche";
        }
    }

    private static void ShowError(string message, Exception exception)
    {
        System.Diagnostics.Trace.TraceError("{0} {1}", message, exception);
        MessageBox.Show(
            message,
            AppConfig.AppName,
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
}
